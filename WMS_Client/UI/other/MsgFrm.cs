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
    public partial class MsgFrm : Office2007Form
    {
        //private string str = "";

        public MsgFrm(string msg, Color cl)
        {
            InitializeComponent();

            label1.Text = msg;
            label1.BackColor = cl;
        }

        private void MsgFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.F3)
            {
                this.Close();    
                return;
            }

            //if (e.KeyCode==Keys.Enter)
            //{
            //    if (str.Trim().ToUpper().Equals("F3"))
            //    {
            //        this.Close();
            //        return;
            //    }
            //    else
            //    {
            //        str = "";
            //    }
            //}
            //else
            //{
            //    int Key = (int)e.KeyCode;
            //    if ((Key >= '0' && Key <= '9') ||
            //        (Key >= 'a' && Key <= 'z') ||
            //        (Key >= 'A' && Key <= 'Z'))
            //    {
            //        byte[] data = new byte[1];
            //        data[0] = (byte)Key;
            //        str += Encoding.Default.GetString(data);
            //    }
            //}
        }
    }
}
