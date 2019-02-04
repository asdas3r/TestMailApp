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
    public partial class FormMain : Form
    {
        FormIncoming form1;
        Form activeForm;

        public FormMain()
        {
            form1 = new FormIncoming();
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ChangeProps(form1);
            SwitchControl(form1);
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            if (activeForm != null)
            {
                //label1.Text = panel1.Width.ToString();
                SwitchControl(activeForm);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SwitchControl(form1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
    }
}
