using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace TestMailApp
{
    public partial class Form1 : Form
    {
        List<Employee> employeesList;
        DataAccess dataAccess;
        Thread th1;

        public Form1()
        {
            InitializeComponent();

            dataAccess = new DataAccess();
            employeesList = new List<Employee>();
            employeesList = dataAccess.GetEmployees();

            listBox1.DataSource = employeesList;
            listBox1.DisplayMember = "Info";
            listBox1.ValueMember = "ID";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedValue == null)
            {
                MessageBox.Show("Не произведен выбор пользователя", "Ошибка!");
            }
            else
            {
                FormMain form2 = new FormMain(Convert.ToInt32(listBox1.SelectedValue));
                form2.MinimumSize = this.MinimumSize;
                form2.Size = this.Size;
                form2.DesktopLocation = this.DesktopLocation;
                
                this.Close();

                th1 = new Thread(() => Application.Run(form2));
                th1.SetApartmentState(ApartmentState.STA);
                th1.Start();
            }
        }
    }
}
