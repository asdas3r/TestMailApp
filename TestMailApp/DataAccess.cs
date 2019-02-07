using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

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
            SqlDataReader dataReader = null;

            try
            {
                myConnection.Open();
                SqlCommand getEmployeesCommand = new SqlCommand("select * from [dbo].[Employees]", myConnection);
                dataReader = getEmployeesCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    employeesList.Add(new Employee(dataReader.GetInt32(0), dataReader.GetSafeString(1), dataReader.GetString(2), dataReader.GetSafeString(3), dataReader.GetSafeString(4)));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                dataReader.Close();
                myConnection.Close();
            }

            return employeesList;
        }

        public List<Tag> GetTags()
        {
            List<Tag> tagsList = new List<Tag>();
            SqlDataReader dataReader = null;

            try
            {
                myConnection.Open();
                SqlCommand getTagsCommand = new SqlCommand("select * from [dbo].[Tags]", myConnection);
                dataReader = getTagsCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    tagsList.Add(new Tag(dataReader.GetSafeString(0), dataReader.GetSafeString(1)));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                dataReader.Close();
                myConnection.Close();
            }

            return tagsList;
        }

        public List<Mail> GetMails(bool isSender, int chosenID)
        {
            List<Mail> mailsList = new List<Mail>();
            SqlDataReader dataReader = null;
            SqlDataReader innerReader = null;

            try
            {
                myConnection.Open();
                SqlParameter personID = new SqlParameter("@personID", SqlDbType.Int, 4);
                personID.Value = chosenID;
                SqlParameter paramIsSender = new SqlParameter("@paramIsSender", SqlDbType.Bit, 1);
                paramIsSender.Value = isSender;

                SqlCommand getMailsCommand = new SqlCommand("select dbo.Mail.ID, dbo.Mail.Name, dbo.Mail.RegistrationDate, dbo.Mail.Text " +
                    "from dbo.Mail INNER JOIN dbo.MailEmployees ON dbo.Mail.ID = dbo.MailEmployees.Mail_ID " +
                    "where (dbo.MailEmployees.IsSender = @paramIsSender) AND (dbo.MailEmployees.Employee_ID = @personID)", myConnection);
                getMailsCommand.Parameters.Add(personID);
                getMailsCommand.Parameters.Add(paramIsSender);
                dataReader = getMailsCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    Mail mailData = new Mail();
                    mailData.ID = dataReader.GetInt32(0);
                    mailData.Name = dataReader.GetSafeString(1);
                    mailData.RegistrationDate = dataReader.GetDateTime(2).Date;
                    mailData.Contents = dataReader.GetSafeString(3);

                    SqlCommand innerCommand = new SqlCommand();
                    innerCommand.Connection = myConnection;

                    SqlParameter paramID = new SqlParameter("@paramID", SqlDbType.Int, 4);
                    paramID.Value = Convert.ToInt32(mailData.ID);
                    SqlParameter paramIsSender2 = new SqlParameter("@paramIsSender2", SqlDbType.Bit, 1);
                    paramIsSender2.Value = Convert.ToBoolean(isSender);

                    innerCommand.CommandText = "select [dbo].[Employees].* " +
                        "from [dbo].[Employees] INNER JOIN [dbo].[MailEmployees] ON [dbo].[Employees].[ID] = [dbo].[MailEmployees].[Employee_ID] INNER JOIN [dbo].[Mail] ON [dbo].[MailEmployees].[Mail_ID] = [dbo].[Mail].[ID] " +
                                               "where  ([dbo].[MailEmployees].[Mail_ID] = @paramID) AND (NOT ([dbo].[MailEmployees].[IsSender] = @paramIsSender2))";
                    innerCommand.Parameters.Add(paramID);
                    innerCommand.Parameters.Add(paramIsSender2);
                    innerReader = innerCommand.ExecuteReader();
                    while (innerReader.Read())
                        mailData.SentFromTo = new Employee(innerReader.GetInt32(0), innerReader.GetSafeString(1), innerReader.GetSafeString(2), innerReader.GetSafeString(3), innerReader.GetSafeString(4));
                    innerReader.Close();

                    innerCommand.CommandText = "select [dbo].[Tags].* " +
                        "from [dbo].[MailTags] INNER JOIN [dbo].[Tags] ON [dbo].[MailTags].[Tag_Name] = [dbo].[Tags].[Name] " +
                                               "where  [dbo].[MailTags].[Mail_ID] = @paramID " +
                                               "order by [dbo].[Tags].[Name]";
                    innerReader = innerCommand.ExecuteReader();
                    List<Tag> tagsList = new List<Tag>();
                    while (innerReader.Read())
                    {

                        tagsList.Add(new Tag(innerReader.GetSafeString(0), innerReader.GetSafeString(1)));
                    }
                    mailData.SetTags(tagsList);
                    innerReader.Close();
                    innerCommand.Parameters.Clear();

                    mailsList.Add(mailData);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                dataReader.Close();
                myConnection.Close();
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
    }
}
