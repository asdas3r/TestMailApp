﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using EntitiesLibrary;

namespace TestMailApp
{
    public partial class FormMain : Form
    {
        FormIncoming form1;
        FormNewEdit form2;
        Form activeForm;
        Thread th1;
        public bool isOnIncoming { get; set; }

        public static int chosenID;

        public FormMain(int ID)
        {
            chosenID = ID;
            InitializeComponent();
            SetActiveUser();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            buttonNew.Click += new System.EventHandler(buttonActive_Click);
            buttonOutgoing.Click += new System.EventHandler(buttonActive_Click);
            buttonIncoming.Click += new System.EventHandler(buttonActive_Click);
            RefreshForm(true);
            isOnIncoming = true;
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
            form1 = new FormIncoming(this, true);
            ChangeProps(form1);
            SwitchControl(form1);
            isOnIncoming = true;
        }

        private void buttonOutgoing_Click(object sender, EventArgs e)
        {
            form1 = new FormIncoming(this, false);
            ChangeProps(form1);
            SwitchControl(form1);
            isOnIncoming = false;
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            form2 = new FormNewEdit(this);
            ChangeProps(form2);
            SwitchControl(form2);
            MakeButtonsActive(false);
            isOnIncoming = true;
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
                DataAccess.DeleteMailsData(form1.selectedMail().ID);
                RefreshForm(isOnIncoming);
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

        public void RefreshForm(bool toIncoming)
        {
            if (toIncoming)
                buttonIncoming.PerformClick();
            else
                buttonOutgoing.PerformClick();
        }

        public void OpenSelectedItem(Mail selected)
        {
            form2 = new FormNewEdit(this, selected);
            ChangeProps(form2);
            SwitchControl(form2);
            MakeButtonsActive(false);
            form2.ReadOnly();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.MinimumSize = this.MinimumSize;
            form1.Size = this.Size;
            form1.DesktopLocation = this.DesktopLocation;
            form1.StartPosition = FormStartPosition.Manual;
            form1.SetSelection(chosenID);

            this.Close();

            th1 = new Thread(() => Application.Run(form1));
            th1.SetApartmentState(ApartmentState.STA);
            th1.Start();
        }

        private void SetActiveUser()
        {
            Employee person = DataAccess.GetEmployees().Find(x => x.ID == Convert.ToInt32(chosenID));
            label1.Text = "Пользователь: \n" +
                person.FullInfo;
        }

        private void buttonActive_Click(object sender, EventArgs e)
        {
            var buttons = tableLayoutPanel2.Controls.OfType<Button>();
            foreach (var button in buttons)
            {
                button.BackColor = SystemColors.Control;
                button.UseVisualStyleBackColor = true;
            }

            Button clicked = (Button)sender;
            clicked.BackColor = Color.LightSalmon;
        }
    }
}
