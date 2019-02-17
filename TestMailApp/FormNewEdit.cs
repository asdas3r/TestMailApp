using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EntitiesLibrary;

namespace TestMailApp
{
    public partial class FormNewEdit : Form
    {
        Mail mailData;
        FormMain mainForm;

        public FormNewEdit(Form callingForm)
        {
            mainForm = callingForm as FormMain;
            InitializeComponent();
            labelHeader.Text = "Добавление нового письма";
        }

        public FormNewEdit(Form callingForm, Mail d)
        {
            mainForm = callingForm as FormMain;
            InitializeComponent();
            mailData = d;
            labelHeader.Text = "Изменение письма";
        }

        public void ReadOnly()
        {
            this.labelHeader.Text = "Просмотр письма";
            this.textBox1.ReadOnly = true;
            this.dateTimePicker1.Enabled = false;
            this.buttonAccept.Visible = false;
            this.buttonBack.Enabled = true;
            this.checkedListBox1.SelectionMode = SelectionMode.None;
            this.textBox3.ReadOnly = true;
        }

        private void FormNewEdit_Load(object sender, EventArgs e)
        {
            List<Tag> tags = DataAccess.GetTags();
            foreach (var t in tags)
            { 
                checkedListBox1.Items.Add(t.Name);
            }

            comboBox1.DataSource = new List<Employee>(DataAccess.GetEmployees());
            comboBox1.DisplayMember = "Info";
            comboBox1.ValueMember = "ID";

            comboBox2.DataSource = new List<Employee>(DataAccess.GetEmployees());
            comboBox2.DisplayMember = "Info";
            comboBox2.ValueMember = "ID";

            LoadEditInfo();
        }

        private void LoadEditInfo()
        {
            if (mailData == null)
                return;

            textBox1.Text = mailData.Name;
            dateTimePicker1.Value = mailData.RegistrationDate;
            comboBox1.SelectedIndex = comboBox1.FindString(mailData.SentFrom.Info);
            comboBox2.SelectedIndex = comboBox2.FindString(mailData.SentTo.Info);

            foreach (var n in mailData.Tags)
            {
                if (checkedListBox1.Items.Contains(n.Name))
                {
                    checkedListBox1.SetItemChecked(checkedListBox1.Items.IndexOf(n.Name), true);
                }
            }

            textBox3.Text = mailData.Contents;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            mainForm.RefreshForm(mainForm.isOnIncoming);
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            Mail formMailData = GetMailFormData();

            if (formMailData == null)
            {
                return;
            }

            if (mailData == null || !mailData.Equals(formMailData))
                DataAccess.SetMailsData(formMailData);
            buttonBack.PerformClick();
        }

        private Mail GetMailFormData()
        {
            Mail data = new Mail();
            if (mailData == null)
                data.ID = 0;
            else
                data.ID = mailData.ID;
            if (textBox1.Text == "")
            {
                MessageBox.Show("Поле 'Название' не может быть пустым. Заполните это поле и повторите попытку.", "Ошибка");
                return null;
            }
            if (dateTimePicker1.Value.Date > DateTime.Today)
            {
                MessageBox.Show("Дата регистрации письма не должна быть больше сегодняшней. Измените дату и повторите попытку.", "Ошибка");
                return null;
            }
            
            data.Name = textBox1.Text;
            data.RegistrationDate = dateTimePicker1.Value;
            data.SentFrom = DataAccess.GetEmployees().Find( x => x.ID == Convert.ToInt32(comboBox1.SelectedValue));
            data.SentTo = DataAccess.GetEmployees().Find(x => x.ID == Convert.ToInt32(comboBox2.SelectedValue));

            List<Tag> tagList = new List<Tag>();
            foreach (var n in DataAccess.GetTags())
            {
                if (checkedListBox1.Items.Contains(n.Name) && checkedListBox1.GetItemChecked(checkedListBox1.Items.IndexOf(n.Name)))
                {
                    tagList.Add(n);
                }
            }
            data.Tags = tagList;
            data.Contents = textBox3.Text;

            return data;
        }

        private void dateTimePicker1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
