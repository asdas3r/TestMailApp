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
using ClassLibrary;

namespace TestMailApp
{
    public partial class FormMain : Form
    {
        FormIncoming form1;
        FormNewEdit form2;
        Form activeForm;
        Thread th1;

        public static int chosenID;

        public FormMain(int ID)
        {
            chosenID = ID;
            InitializeComponent();
            SetActiveUser();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            RefreshForm();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            if (activeForm != null)
            {
                SwitchControl(activeForm);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            form1 = new FormIncoming(this);
            ChangeProps(form1);
            SwitchControl(form1);
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            form2 = new FormNewEdit(this);
            ChangeProps(form2);
            SwitchControl(form2);
            MakeButtonsActive(false);
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            form2 = new FormNewEdit(this, form1.selectedMail());
            ChangeProps(form2);
            SwitchControl(form2);
            MakeButtonsActive(false);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DialogResult ans = MessageBox.Show("Будет произведено удаление записи. Восстановление записи будет невозможно. Вы уверены?", "Удаление", MessageBoxButtons.OKCancel);

            if (ans == DialogResult.OK)
            {
                new DataAccess().DeleteMailsData(form1.selectedMail().ID);
                RefreshForm();
            }
        }

        public void ChangeProps(Form frm)
        {
            frm.TopLevel = false;
            frm.StartPosition = FormStartPosition.Manual;
            frm.WindowState = FormWindowState.Maximized;
            frm.FormBorderStyle = FormBorderStyle.None;
        }

        public void SwitchControl(Form frm)
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(frm);
            frm.Size = panel1.Size;
            frm.Show();
            activeForm = frm;
        }

        public void MakeButtonsActive(bool ifActive)
        {
            this.buttonDelete.Visible = ifActive;
            this.buttonUpdate.Visible = ifActive;
        }

        public void RefreshForm()
        {
            buttonIncoming.PerformClick();
        }

        public void OpenSelectedItem(Mail selected)
        {
            form2 = new FormNewEdit(this, selected);
            ChangeProps(form2);
            SwitchControl(form2);
            MakeButtonsActive(false);
            form2.Disable();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.MinimumSize = this.MinimumSize;
            form1.Size = this.Size;
            form1.DesktopLocation = this.DesktopLocation;
            form1.StartPosition = FormStartPosition.Manual;

            this.Close();

            th1 = new Thread(() => Application.Run(form1));
            th1.SetApartmentState(ApartmentState.STA);
            th1.Start();
        }

        private void SetActiveUser()
        {
            Employee person = new DataAccess().GetEmployees().Find(x => x.ID == Convert.ToInt32(chosenID));
            label1.Text = "Пользователь: \n" +
                person.FullInfo;
        }
    }
}
