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
    static class DataAccess
    {
        public static List<Employee> GetEmployees()
        {
            List<Employee> employeesList = new List<Employee>();

            try
            {
                DBServiceClient dws = new DBServiceClient();
                employeesList = dws.GetEmployees();
                dws.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return employeesList;
        }

        public static List<Tag> GetTags()
        {
            List<Tag> tagsList = new List<Tag>();

            try
            {
                DBServiceClient dws = new DBServiceClient();
                tagsList = dws.GetTags();
                dws.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return tagsList;
        }

        public static List<Mail> GetMails(bool isSender, int chosenID)
        {
            List<Mail> mailsList = new List<Mail>();

            try
            {
                DBServiceClient dws = new DBServiceClient();
                mailsList = dws.GetMails(isSender, chosenID);
                dws.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return mailsList;
        }

        public static void SetMailsData(Mail mailData)
        {
            try
            {
                DBServiceClient dws = new DBServiceClient();
                dws.SetMailsData(mailData);
                dws.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void DeleteMailsData(int ID)
        {
            try
            {
                DBServiceClient dws = new DBServiceClient();
                dws.DeleteMailsData(ID);
                dws.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
