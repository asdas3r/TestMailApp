using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void FormNewEdit_Load(object sender, EventArgs e)
        {
            List<Tag> tags = new DataAccess().GetTags();
            foreach (var t in tags)
            { 
                checkedListBox1.Items.Add(t.Name);
            }

            List<Employee> employeesList = new DataAccess().GetEmployees();
            comboBox1.DataSource = employeesList;
            comboBox1.DisplayMember = "Info";
            comboBox1.ValueMember = "ID";

            LoadEditInfo();
        }

        private void LoadEditInfo()
        {
            if (mailData == null)
                return;

            textBox1.Text = mailData.Name;
            dateTimePicker1.Value = mailData.RegistrationDate;
            comboBox1.SelectedIndex = comboBox1.FindString(mailData.SentFromTo.Info);

            foreach (var n in mailData.GetTags())
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
            mainForm.RefreshForm();
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            Mail formMailData = GetMailFormData();
            if (formMailData == null)
            {
                MessageBox.Show("Не все данные введены корректно", "Ошибка!");
                return;
            }

            if (mailData == null || !mailData.Equals(formMailData))
                new DataAccess().SetMailsData(formMailData, FormMain.chosenID);
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
                return null;
            data.Name = textBox1.Text;
            data.RegistrationDate = dateTimePicker1.Value;
            data.SentFromTo = new DataAccess().GetEmployees().Find( x => x.ID == Convert.ToInt32(comboBox1.SelectedValue));

            List<Tag> tagList = new List<Tag>();
            foreach (var n in new DataAccess().GetTags())
            {
                if (checkedListBox1.Items.Contains(n.Name) && checkedListBox1.GetItemChecked(checkedListBox1.Items.IndexOf(n.Name)))
                {
                    tagList.Add(n);
                }
            }
            data.SetTags(tagList);
            data.Contents = textBox3.Text;

            return data;
        }
    }
}
