using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
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
                SqlParameter personID = new SqlParameter("@personID", System.Data.SqlDbType.Int, 4);
                personID.Value = chosenID;
                SqlParameter paramIsSender = new SqlParameter("@paramIsSender", System.Data.SqlDbType.Bit, 1);
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

                    SqlParameter paramID = new SqlParameter("@paramID", System.Data.SqlDbType.Int, 4);
                    paramID.Value = mailData.ID;
                    SqlParameter paramIsSender2 = new SqlParameter("@paramIsSender2", System.Data.SqlDbType.Bit, 1);
                    paramIsSender2.Value = isSender;

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
                    mailData.SetTags(tagsList.ToArray());
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
    }
}
