using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;

namespace WMS_Client.UI
{
    public partial class PasswordFrm : Office2007Form
    {
        public PasswordFrm()
        {
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请先输入密码！");
                textBox1.Text = string.Empty;
                textBox1.Focus();
            }

            button1_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("admin"))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("密码输入错误，请重新输入！");
                textBox1.Text = string.Empty;
                textBox1.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
