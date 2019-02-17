using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using EntitiesLibrary;

namespace WcfServiceProject
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "SqlDBService" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы SqlDBService.svc или SqlDBService.svc.cs в обозревателе решений и начните отладку.
    public class SqlDBService : ISqlDBService
    {
        public List<Employee> GetEmployees()
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                {
                    List<Employee> employeesList = new List<Employee>();
                    myConnection.Open();
                    SqlCommand getEmployeesCommand = new SqlCommand("sp_GetEmployees", myConnection);
                    getEmployeesCommand.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = getEmployeesCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employeesList.Add(new Employee(
                                reader.GetSafeValue<int>(0),
                                reader.GetSafeValue<string>(1),
                                reader.GetSafeValue<string>(2),
                                reader.GetSafeValue<string>(3),
                                reader.GetSafeValue<string>(4)));
                        }
                        reader.Close();

                        return employeesList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.ToString());
            }
        }

        public List<Tag> GetTags()
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                {
                    List<Tag> tagsList = new List<Tag>();
                    myConnection.Open();
                    SqlCommand getTagsCommand = new SqlCommand("sp_GetTags", myConnection);
                    getTagsCommand.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = getTagsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tagsList.Add(new Tag(
                                reader.GetSafeValue<string>(0),
                                reader.GetSafeValue<string>(1)));
                        }
                        reader.Close();

                        return tagsList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.ToString());
            }
        }

        public List<Mail> GetMails(bool isSender, int chosenID)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                {
                    List<Mail> mailList = new List<Mail>();
                    myConnection.Open();

                    SqlParameter personID = new SqlParameter("@personID", chosenID);
                    SqlParameter paramIsSender = new SqlParameter("@paramIsSender", isSender);

                    SqlCommand getMailsCommand = new SqlCommand("sp_GetMailByPersonRole", myConnection);
                    getMailsCommand.CommandType = CommandType.StoredProcedure;
                    getMailsCommand.Parameters.Add(personID);
                    getMailsCommand.Parameters.Add(paramIsSender);

                    SqlParameter paramID = new SqlParameter("@paramID", SqlDbType.Int);
                    SqlParameter paramIsSenderInner = new SqlParameter("@paramIsSender", true);

                    SqlCommand getMailEmployeesCommand = new SqlCommand("sp_GetEmployeesByMailRole", myConnection);
                    getMailEmployeesCommand.CommandType = CommandType.StoredProcedure;

                    SqlCommand getMailTagsCommand = new SqlCommand("sp_GetTagsByMail", myConnection);
                    getMailTagsCommand.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader mailReader = getMailsCommand.ExecuteReader())
                    {
                        while (mailReader.Read())
                        {
                            paramID.Value = mailReader.GetSafeValue<int>(0);
                            getMailEmployeesCommand.Parameters.Add(paramID);
                            getMailEmployeesCommand.Parameters.Add(paramIsSenderInner);

                            List<Employee> SentFromTo = new List<Employee>();
                            for (int i = 0; i < 2; i++)
                            {
                                using (SqlDataReader employeeReader = getMailEmployeesCommand.ExecuteReader(CommandBehavior.SingleRow))
                                {
                                    while (employeeReader.Read())
                                    {
                                        SentFromTo.Add(new Employee(
                                        employeeReader.GetSafeValue<int>(0),
                                        employeeReader.GetSafeValue<string>(1),
                                        employeeReader.GetSafeValue<string>(2),
                                        employeeReader.GetSafeValue<string>(3),
                                        employeeReader.GetSafeValue<string>(4)));
                                    }
                                    paramIsSenderInner.Value = !(bool)paramIsSenderInner.Value;
                                    employeeReader.Close();
                                }
                            }
                            getMailEmployeesCommand.Parameters.Clear();

                            List<Tag> tagsList = new List<Tag>();
                            getMailTagsCommand.Parameters.Add(paramID);
                            using (SqlDataReader tagsReader = getMailTagsCommand.ExecuteReader())
                            {
                                while (tagsReader.Read())
                                {
                                    tagsList.Add(new Tag(
                                        tagsReader.GetSafeValue<string>(0),
                                        tagsReader.GetSafeValue<string>(1)));
                                }
                                tagsReader.Close();
                            }
                            getMailTagsCommand.Parameters.Clear();

                            mailList.Add(new Mail(
                                    mailReader.GetSafeValue<int>(0),
                                    mailReader.GetSafeValue<string>(1),
                                    mailReader.GetSafeValue<DateTime>(2),
                                    SentFromTo[0],
                                    SentFromTo[1],
                                    mailReader.GetSafeValue<string>(3),
                                    tagsList));
                        }
                        mailReader.Close();

                        return mailList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.ToString());
            }
        }

        public void SetMailsData(Mail mailData)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                {
                    myConnection.Open();
                    SqlCommand setDataCommand = new SqlCommand("sp_SetMailsData", myConnection);
                    setDataCommand.CommandType = CommandType.StoredProcedure;

                    string tagNamesStr = null;
                    if (mailData.Tags.Count > 0)
                    {
                        tagNamesStr = mailData.Tags[0].Name;
                        for (int i = 1; i < mailData.Tags.Count; i++)
                            tagNamesStr = string.Format("{0},{1}", tagNamesStr, mailData.Tags[i].Name);
                    }

                    List<SqlParameter> param = new List<SqlParameter>();

                    param.Add(new SqlParameter("@cID", Convert.ToInt32(mailData.ID)));
                    param.Add(new SqlParameter("@cName", Convert.ToString(mailData.Name)));
                    param.Add(new SqlParameter("@cDate", Convert.ToDateTime(mailData.RegistrationDate)));
                    param.Add(new SqlParameter("@cSent", Convert.ToInt32(mailData.SentFrom.ID)));
                    param.Add(new SqlParameter("@cRecieved", Convert.ToInt32(mailData.SentTo.ID)));
                    param.Add(new SqlParameter("@cContents", Convert.ToString(mailData.Contents)));
                    param.Add(new SqlParameter("@cTagNameString", string.IsNullOrEmpty(tagNamesStr) ? (object)DBNull.Value : Convert.ToString(tagNamesStr)));

                    foreach (var n in param)
                        setDataCommand.Parameters.Add(n);

                    setDataCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex1)
            {
                throw new FaultException(ex1.Message);
            }
        }

        public void DeleteMailsData(int ID)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                {
                    myConnection.Open();
                    SqlCommand deleteDataCommand = new SqlCommand("sp_DeleteMailsData", myConnection);
                    deleteDataCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter cID = new SqlParameter("@cID", Convert.ToInt32(ID));
                    deleteDataCommand.Parameters.Add(cID);

                    deleteDataCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        private string GetConnectionString()
        {
            return "Data Source=DESKTOP-RSMRVQT\\SQLEXPRESS;Initial Catalog=Testmail;Integrated Security=True;MultipleActiveResultSets=true";
        }
    }
}
