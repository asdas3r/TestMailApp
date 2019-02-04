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
    public partial class FormIncoming : Form
    {
        public FormIncoming()
        {
            InitializeComponent();
            listView1.FullRowSelect = true;
        }

        private void FormIncoming_Load(object sender, EventArgs e)
        {
            List<Mail> mailList = new DataAccess().GetMails(false, 4);
            label1.Text = mailList.Count.ToString();
            List<ListViewItem> lvList = new List<ListViewItem>();

            for (int i = 0; i < mailList.Count; i++)
            {
                Tag[] tags = mailList[i].GetTags();
                string tagsString = "";
                for (int j = 0; j < tags.Length - 1; j++)
                {
                    tagsString = string.Concat(tagsString, tags[j].Name, ", ");
                }
                tagsString = string.Concat(tagsString, tags[tags.Length - 1].Name);
                ListViewItem item = new ListViewItem(new string[] { mailList[i].Name, mailList[i].RegistrationDate.ToString(), mailList[i].SentFromTo.Info, tagsString, mailList[i].Contents });
                item.Tag = mailList[i].ID;
                lvList.Add(item);
            }

            foreach (var n in lvList)
                listView1.Items.Add(n);
        }
    }
}
