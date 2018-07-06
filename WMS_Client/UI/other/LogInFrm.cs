using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Phicomm_WMS.OUTIO;

namespace Phicomm_WMS.UI
{
    public partial class LogInFrm : Office2007Form
    {
        public LogInFrm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox_user.Text))
                {
                    MessageBox.Show("账号不能为空！");
                    textBox_user.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(textBox_password.Text))
                {
                    MessageBox.Show("密码不能为空！");
                    textBox_password.Focus();
                    return;
                }

                CheckAccount.CheckUser(textBox_user.Text, textBox_password.Text);

                MyData.SetUser(textBox_user.Text);
                MyData.SetPassword(textBox_password.Text);

                this.DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBox_password.Text = "";
                textBox_password.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void textBox_user_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            button1_Click(sender, e);
        }

        private void textBox_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            button1_Click(sender, e);
        }

        private void LogInFrm_Load(object sender, EventArgs e)
        {
            //using (MsgFrm mf = new MsgFrm("Hello", Color.Yellow))
            //{
            //    mf.ShowDialog();
            //    mf.Dispose();
            //}
        }
    }
}
