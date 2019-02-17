using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using EntitiesLibrary;

namespace WebApplicationProject
{
    /// <summary>
    /// Сводное описание для dbWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Чтобы разрешить вызывать веб-службу из скрипта с помощью ASP.NET AJAX, раскомментируйте следующую строку. 
    // [System.Web.Script.Services.ScriptService]
    public class dbWebService : System.Web.Services.WebService
    {
        [WebMethod]
        public string HelloWorld()
        {
            return "Привет всем!";
        }

        [WebMethod]
        public DataSet GetEmployees()
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    List<Employee> employeesList = new List<Employee>();
                    myConnection.Open();
                    SqlCommand getEmployeesCommand = new SqlCommand("select * from [dbo].[Employees]", myConnection);
                    adapter.SelectCommand = getEmployeesCommand;

                    DataSet data = new DataSet("Employees");
                    adapter.Fill(data, "Employees");
                    myConnection.Close();

                    if (data.Tables.Count == 0)
                        return null;
                    return data;
                }
            }
        }

        [WebMethod]
        public DataSet GetTags()
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString())) {
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    myConnection.Open();
                    SqlCommand getEmployeesCommand = new SqlCommand("select * from [dbo].[Tags]", myConnection);
                    adapter.SelectCommand = getEmployeesCommand;

                    DataSet data = new DataSet("Tags");
                    adapter.Fill(data, "Tags");
                    myConnection.Close();

                    if (data.Tables.Count == 0)
                        return null;

                    for (int i = 0; i < data.Tables["Tags"].Rows.Count; i++)
                    {
                        if (Convert.IsDBNull(data.Tables["Tags"].Rows[i]["Description"]))
                        {
                            data.Tables["Tags"].Rows[i]["Description"] = "Отсутствует";
                        }
                    }

                    return data;
                }
            }
        }

        [WebMethod]
        public DataSet GetMails(bool isSender, int chosenID)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                SqlDataAdapter adapterMails = new SqlDataAdapter();
                SqlDataAdapter adapterMailTags = new SqlDataAdapter();
                SqlDataAdapter adapterMailEmployees = new SqlDataAdapter();
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
                adapterMails.SelectCommand = getMailsCommand;

                DataSet data = new DataSet("MailsWithEmployeesTags");
                adapterMails.Fill(data, "Mails");
                DataSet mailDS = new DataSet("Mails");
                mailDS = data.Copy();

                getMailsCommand.Parameters.Clear();
                SqlCommand getMailTagsCommand = new SqlCommand();
                getMailTagsCommand.Connection = myConnection;
                SqlCommand getMailEmployeesCommand = new SqlCommand();
                getMailEmployeesCommand.Connection = myConnection;
                SqlParameter paramID = new SqlParameter("@paramID", SqlDbType.Int, 4);
                SqlParameter paramID2 = new SqlParameter("@paramID2", SqlDbType.Int, 4);

                getMailTagsCommand.CommandText = "select [dbo].[MailTags].[Mail_ID], [dbo].[Tags].* " +
                        "from [dbo].[MailTags] INNER JOIN [dbo].[Tags] ON [dbo].[MailTags].[Tag_Name] = [dbo].[Tags].[Name] " +
                                               "where  [dbo].[MailTags].[Mail_ID] = @paramID " +
                                               "order by [dbo].[Tags].[Name]";
                getMailTagsCommand.Parameters.Add(paramID);
                adapterMailTags.SelectCommand = getMailTagsCommand;

                getMailEmployeesCommand.CommandText = "select [dbo].[MailEmployees].[Mail_ID], [dbo].[Employees].* " +
                   "from [dbo].[Employees] INNER JOIN [dbo].[MailEmployees] ON [dbo].[Employees].[ID] = [dbo].[MailEmployees].[Employee_ID] INNER JOIN [dbo].[Mail] ON [dbo].[MailEmployees].[Mail_ID] = [dbo].[Mail].[ID] " +
                                          "where  ([dbo].[MailEmployees].[Mail_ID] = @paramID2) AND (NOT ([dbo].[MailEmployees].[IsSender] = @paramIsSender))";
                getMailEmployeesCommand.Parameters.Add(paramID2);
                getMailEmployeesCommand.Parameters.Add(paramIsSender);
                adapterMailEmployees.SelectCommand = getMailEmployeesCommand;

                for (int i = 0; i < mailDS.Tables["Mails"].Rows.Count; i++)
                {
                    int mailID = Convert.ToInt32(mailDS.Tables["Mails"].Rows[i]["ID"].ToString());
                    paramID.Value = Convert.ToInt32(mailID);
                    paramID2.Value = Convert.ToInt32(mailID);
                    adapterMailTags.Fill(data, "MailTags");
                    adapterMailEmployees.Fill(data, "MailEmployees");
                }

                myConnection.Close();

                if (data.Tables.Count == 0)
                    return null;

                if (data.Tables.Contains("MailTags"))
                {
                    for (int i = 0; i < data.Tables["MailTags"].Rows.Count; i++)
                    {
                        if (Convert.IsDBNull(data.Tables["MailTags"].Rows[i]["Description"]))
                        {
                            data.Tables["MailTags"].Rows[i]["Description"] = "Отсутствует";
                        }
                    }

                    DataRelation mailToTags = data.Relations.Add("MailToTags", data.Tables["Mails"].Columns["ID"], data.Tables["MailTags"].Columns["Mail_ID"]);
                    mailToTags.Nested = true;
                    if (data.Tables.Contains("MailEmployees"))
                    {
                        DataRelation mailToEmployees = data.Relations.Add("MailToEmployees", data.Tables["Mails"].Columns["ID"], data.Tables["MailEmployees"].Columns["Mail_ID"]);
                        mailToEmployees.Nested = true;
                    }
                }

                return data;
            }
        }

        [WebMethod]
        public void SetMailsData(string[] mail, string[] tags, int recieverID)
        {
            
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                SqlTransaction tran;
                myConnection.Open();
                tran = myConnection.BeginTransaction();
                SqlCommand setDataCommand = myConnection.CreateCommand();
                setDataCommand.Transaction = tran;

                SqlParameter[] param = new SqlParameter[7];
                param[0] = new SqlParameter("@recID", SqlDbType.Int, 4);
                param[0].Value = Convert.ToInt32(recieverID);
                param[1] = new SqlParameter("@cID", SqlDbType.Int, 4);
                param[1].Value = Convert.ToInt32(mail[0]);
                param[2] = new SqlParameter("@cName", SqlDbType.NVarChar, 50);
                param[2].Value = Convert.ToString(mail[1]);
                param[3] = new SqlParameter("@cDate", SqlDbType.Date, 3);
                param[3].Value = Convert.ToDateTime(mail[2]);
                param[4] = new SqlParameter("@cSent", SqlDbType.Int, 4);
                param[4].Value = Convert.ToInt32(mail[3]);
                param[5] = new SqlParameter("@cContents", SqlDbType.NVarChar, -1);
                param[5].Value = Convert.ToString(mail[4]);
                param[6] = new SqlParameter("@cTag", SqlDbType.NVarChar, 50);

                if (Convert.ToInt32(mail[0]) == 0)
                    AddToSQL(tags, param, setDataCommand);
                else
                    UpdateToSQL(tags, param, setDataCommand);
                tran.Commit();
                myConnection.Close();
            }
        }

        private void AddToSQL(string[] tags, SqlParameter[] param, SqlCommand comm)
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
            foreach (var n in tags)
            {
                param[6].Value = Convert.ToString(n);
                comm.ExecuteNonQuery();
            }
        }

        private void UpdateToSQL(string[] tags, SqlParameter[] param, SqlCommand comm)
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
            foreach (var n in tags)
            {
                param[6].Value = Convert.ToString(n);
                comm.ExecuteNonQuery();
            }
        }

        [WebMethod]
        public void DeleteMailsData(int ID)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                SqlTransaction tran;
                myConnection.Open();
                tran = myConnection.BeginTransaction();
                SqlCommand command = myConnection.CreateCommand();
                command.Transaction = tran;

                SqlParameter cID = new SqlParameter("@cID", SqlDbType.Int, 4);
                cID.Value = Convert.ToInt32(ID);

                command.Parameters.Add(cID);
                command.CommandText = "delete from [dbo].[MailEmployees] where [Mail_ID] = @cID";
                command.ExecuteNonQuery();

                command.CommandText = "delete from [dbo].[MailTags] where [Mail_ID] = @cID";
                command.ExecuteNonQuery();

                command.CommandText = "delete from [dbo].[Mail] where [ID] = @cID";
                command.ExecuteNonQuery();
                tran.Commit();
            }
        }

        private string GetConnectionString()
        {
            return "Data Source=DESKTOP-RSMRVQT\\SQLEXPRESS;Initial Catalog=Testmail;Integrated Security=True";
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
