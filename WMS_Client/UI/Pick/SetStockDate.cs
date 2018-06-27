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
    public partial class SetStockDate : Office2007Form
    {
        private string _date = string.Empty;

        public SetStockDate()
        {
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                return;
            }

            try
            {
                _date = textBox1.Text.Trim();
                if (_date.Length != 8)
                {
                    MessageBox.Show("长度不对，应为8位");
                    return;
                }

                if (!_date.All(c => (c >= '0' && c <= '9')))
                {
                    MessageBox.Show("输入格式不对，格式必须为：20180608，请重新输入!");
                    return;
                }

                this.DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                textBox1.Text = "";
                textBox1.Focus();
            }
        }

        public string GetDate()
        {
            return _date;
        }
    }
}
