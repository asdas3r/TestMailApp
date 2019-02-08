using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using TestMailApp.WS;
using ClassLibrary;

namespace TestMailApp
{
    class DataAccess
    {
        SqlConnection myConnection;

        public DataAccess()
        {
            myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Testmail"].ConnectionString);
        }

        public List<Employee> GetEmployees()
        {
            List<Employee> employeesList = new List<Employee>();

            try
            {
                WebService ws = new WebService();
                DataSet ds = ws.GetEmployees();
                if (ds == null)
                    return null;
                foreach (DataRow row in ds.Tables["Employees"].Rows)
                {
                    employeesList.Add(new Employee(Convert.ToInt32(row["ID"].ToString()), row["Surname"].ToString(), row["Name"].ToString(), CheckNullString(row["Patronymic"].ToString()), row["Email"].ToString()));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return employeesList;
        }

        public List<Tag> GetTags()
        {
            List<Tag> tagsList = new List<Tag>();

            try
            {
                WebService ws = new WebService();
                DataSet ds = ws.GetTags();
                //tagsList = ds.Tables[0].AsEnumerable().Select(d => new Tag(d.Field<string>("Name"), d.Field<string>("Description"))).ToList();
                if (ds == null)
                    return null;
                foreach (DataRow row in ds.Tables["Tags"].Rows)
                {
                    tagsList.Add(new Tag(row["Name"].ToString(), CheckNullString(row["Description"].ToString())));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return tagsList;
        }

        public List<Mail> GetMails(bool isSender, int chosenID)
        {
            List<Mail> mailsList = new List<Mail>();

            try
            {
                WebService ws = new WebService();
                DataSet ds = ws.GetMails(isSender, chosenID);

                int rowNumber = 0;
                foreach (DataRow row in ds.Tables["Mails"].Rows)
                {
                    Mail newMail = new Mail();
                    newMail.ID = Convert.ToInt32(row["ID"].ToString());
                    newMail.Name = row["Name"].ToString();
                    newMail.RegistrationDate = Convert.ToDateTime(row["RegistrationDate"].ToString());
                    newMail.Contents = row["Text"].ToString();

                    foreach (var erow in ds.Tables["Mails"].Rows[rowNumber].GetChildRows("MailToEmployees"))
                    {
                        newMail.SentFromTo = new Employee(Convert.ToInt32(erow["ID"].ToString()), erow["Surname"].ToString(), erow["Name"].ToString(), CheckNullString(erow["Patronymic"].ToString()), erow["Email"].ToString());
                    }

                    List<Tag> tagList = new List<Tag>();
                    foreach (var trow in ds.Tables["Mails"].Rows[rowNumber].GetChildRows("MailToTags"))
                    {
                        tagList.Add(new Tag(trow["Name"].ToString(), CheckNullString(trow["Description"].ToString())));
                    }
                    newMail.SetTags(tagList);
                    
                    mailsList.Add(newMail);
                    rowNumber++;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return mailsList;
        }

        public void SetMailsData(Mail data, int recieverID)
        {
            SqlTransaction tran;
            try
            {
                myConnection.Open();
                tran = myConnection.BeginTransaction();
                SqlCommand setDataCommand = myConnection.CreateCommand();
                setDataCommand.Transaction = tran;

                SqlParameter[] param = new SqlParameter[7];
                param[0] = new SqlParameter("@recID", SqlDbType.Int, 4);
                param[0].Value = Convert.ToInt32(recieverID);
                param[1] = new SqlParameter("@cID", SqlDbType.Int, 4);
                param[1].Value = Convert.ToInt32(data.ID);
                param[2] = new SqlParameter("@cName", SqlDbType.NVarChar, 50);
                param[2].Value = Convert.ToString(data.Name);
                param[3] = new SqlParameter("@cDate", SqlDbType.Date, 3);
                param[3].Value = Convert.ToDateTime(data.RegistrationDate);
                param[4] = new SqlParameter("@cSent", SqlDbType.Int, 4);
                param[4].Value = Convert.ToInt32(data.SentFromTo.ID);
                param[5] = new SqlParameter("@cContents", SqlDbType.NVarChar, -1);
                param[5].Value = Convert.ToString(data.Contents);
                param[6] = new SqlParameter("@cTag", SqlDbType.NVarChar, 50);

                try
                {
                    if (data.ID == 0)
                        AddToSQL(data, param, setDataCommand);
                    else
                        UpdateToSQL(data, param, setDataCommand);
                    tran.Commit();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    try
                    {
                        tran.Rollback();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void AddToSQL(Mail data, SqlParameter[] param, SqlCommand comm)
        {
            comm.Parameters.Add(param[2]);
            comm.Parameters.Add(param[3]);
            comm.Parameters.Add(param[5]);
            comm.CommandText = "insert into [dbo].[Mail] values (@cName, @cDate, @cContents) " +
                                        "select CAST (SCOPE_IDENTITY() AS int)";
            param[1].Value = comm.ExecuteScalar();

            comm.CommandText = "insert into [dbo].[MailEmployees] values (@cID, @cSent, 1) " +
                "insert into [dbo].[MailEmployees] values (@cID, @recID, 0)";
            comm.Parameters.Add(param[1]);
            comm.Parameters.Add(param[4]);
            comm.Parameters.Add(param[0]);
            comm.ExecuteNonQuery();

            comm.CommandText = "insert into [dbo].[MailTags] values (@cID, @cTag)";
            comm.Parameters.Add(param[6]);
            foreach (var n in data.GetTags())
            {
                param[6].Value = Convert.ToString(n.Name);
                comm.ExecuteNonQuery();
            }
        }

        private void UpdateToSQL(Mail data, SqlParameter[] param, SqlCommand comm)
        {
            comm.Parameters.Add(param[2]);
            comm.Parameters.Add(param[3]);
            comm.Parameters.Add(param[5]);
            comm.Parameters.Add(param[1]);
            comm.CommandText = "update [dbo].[Mail] SET [Name] = @cName, [RegistrationDate] = @cDate, [Text] = @cContents " +
                                        "where [ID] = @cID";
            comm.ExecuteNonQuery();
            comm.CommandText = "update [dbo].[MailEmployees] SET [Employee_ID] = @cSent " +
                                        "where [Mail_ID] = @cID AND [IsSender] = 1";
            comm.Parameters.Add(param[4]);
            comm.Parameters.Add(param[0]);
            comm.ExecuteNonQuery();

            comm.CommandText = "delete from [dbo].[MailTags] where [Mail_ID] = @cID";
            comm.ExecuteNonQuery();

            comm.CommandText = "insert into [dbo].[MailTags] values (@cID, @cTag)";
            comm.Parameters.Add(param[6]);
            foreach (var n in data.GetTags())
            {
                param[6].Value = Convert.ToString(n.Name);
                comm.ExecuteNonQuery();
            }
        }

        public void DeleteMailsData(int ID)
        {
            SqlTransaction tran;
            try
            {
                myConnection.Open();
                tran = myConnection.BeginTransaction();
                SqlCommand command = myConnection.CreateCommand();
                command.Transaction = tran;

                SqlParameter cID = new SqlParameter("@cID", SqlDbType.Int, 4);
                cID.Value = Convert.ToInt32(ID);

                try
                {
                    command.Parameters.Add(cID);
                    command.CommandText = "delete from [dbo].[MailEmployees] where [Mail_ID] = @cID";
                    command.ExecuteNonQuery();

                    command.CommandText = "delete from [dbo].[MailTags] where [Mail_ID] = @cID";
                    command.ExecuteNonQuery();

                    command.CommandText = "delete from [dbo].[Mail] where [ID] = @cID";
                    command.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    try
                    {
                        tran.Rollback();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public static string CheckNullString(string str)
        {
            if (str.Equals("Отсутствует"))
                return string.Empty;
            else
                return str;
        }
    }
}
