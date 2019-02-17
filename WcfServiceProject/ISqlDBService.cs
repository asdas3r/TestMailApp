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
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "ISqlDBService" в коде и файле конфигурации.
    [ServiceContract]
    public interface ISqlDBService
    {

        [OperationContract]
        List<Employee> GetEmployees();

        [OperationContract]
        List<Tag> GetTags();

        [OperationContract]
        List<Mail> GetMails(bool isSender, int chosenID);

        [OperationContract]
        void SetMailsData(Mail data);

        [OperationContract]
        void DeleteMailsData(int ID);
    }
}
