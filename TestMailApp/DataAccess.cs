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
using EntitiesLibrary;
using System.ServiceModel;

namespace TestMailApp
{
    class DataAccess
    {
        public  List<Employee> GetEmployees()
        {
            List<Employee> employeesList = new List<Employee>();

            try
            {
                SqlDBServiceClient dws = new SqlDBServiceClient();
                employeesList = dws.GetEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return employeesList;
        }

        public List<Tag> GetTags()
        {
            List<Tag> tagsList = new List<Tag>();

            try
            {
                SqlDBServiceClient dws = new SqlDBServiceClient();
                tagsList = dws.GetTags();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return tagsList;
        }

        public List<Mail> GetMails(bool isSender, int chosenID)
        {
            List<Mail> mailsList = new List<Mail>();

            try
            {
                SqlDBServiceClient dws = new SqlDBServiceClient();
                mailsList = dws.GetMails(isSender, chosenID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return mailsList;
        }

        public void SetMailsData(Mail mailData)
        {
            try
            {
                SqlDBServiceClient dws = new SqlDBServiceClient();

                dws.SetMailsData(mailData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void DeleteMailsData(int ID)
        {
            try
            {
                SqlDBServiceClient dws = new SqlDBServiceClient();
                dws.DeleteMailsData(ID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
