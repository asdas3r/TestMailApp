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
    public partial class FormIncoming : Form
    {
        FormMain mainForm;
        List<Mail> mailList;
        bool isIncoming;

        public FormIncoming(Form callingForm, bool ifIncoming)
        {
            mainForm = callingForm as FormMain;
            InitializeComponent();
            listView1.FullRowSelect = true;
            isIncoming = ifIncoming;
        }

        private void FormIncoming_Load(object sender, EventArgs e)
        {
            mainForm.MakeButtonsActive(false);
            SwitchInOut();
            List<ListViewItem> lvList = new List<ListViewItem>();
            mailList.Sort( (x, y) => -DateTime.Compare(x.RegistrationDate, y.RegistrationDate));
            for (int i = 0; i < mailList.Count; i++)
            {
                List<Tag> tags = mailList[i].Tags;
                string tagsString = "";
                for (int j = 0; j < tags.Count - 1; j++)
                {
                    tagsString = string.Concat(tagsString, tags[j].Name, ", ");
                }
                if (tags.Count != 0)
                    tagsString = string.Concat(tagsString, tags[tags.Count - 1].Name);
                ListViewItem item = new ListViewItem(new string[] { mailList[i].Name, mailList[i].RegistrationDate.ToShortDateString(), isIncoming ? mailList[i].SentFrom.Info : mailList[i].SentTo.Info, tagsString, mailList[i].Contents });
                item.Tag = i;
                lvList.Add(item);
            }

            foreach (var n in lvList)
                listView1.Items.Add(n);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count != 0)
                mainForm.MakeButtonsActive(true);
            else
                mainForm.MakeButtonsActive(false);
        }

        public Mail selectedMail()
        {
            return (mailList[Convert.ToInt32(listView1.SelectedItems[0].Tag)]);
        }

        private void SwitchInOut()
        {
            this.columnHeader2.Text = isIncoming ? "Отправитель" : "Получатель";
            mailList = DataAccess.GetMails(!isIncoming, FormMain.chosenID);
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
                mainForm.OpenSelectedItem(selectedMail());
        }
    }
}
