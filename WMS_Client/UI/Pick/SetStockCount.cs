using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;

namespace Phicomm_WMS.UI
{
    public partial class SetStockCount : Office2007Form
    {
        private int _realqty = 0;
        private int _realcount = 0;

        public SetStockCount()
        {
            InitializeComponent();
        }

        public void SetLocId(string str)
        {
            label_pd_locid.Text = str;
            textBox_pd_qty.Focus();
        }

        public void SetKpNo(string str)
        {
            label_pd_kpno.Text = str;
            textBox_pd_qty.Focus();
        }
        
        public void SetQty(string str)
        {
            label_pd_qty.Text = str;
            textBox_pd_qty.Focus();
        }

        public void SetCount(string str)
        {
            label_pd_count.Text = str;
            textBox_pd_qty.Focus();
        }

        public void SetPlant(string str)
        {
            label_pd_plant.Text = str;
            textBox_pd_qty.Focus();
        }

        public void SetStockId(string str)
        {
            label_pd_stockid.Text = str;
            textBox_pd_qty.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_pd_qty.Text.Trim()))
            {
                textBox_pd_qty.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox_pd_count.Text.Trim()))
            {
                textBox_pd_count.Focus();
                return;
            }

            string realqty = textBox_pd_qty.Text.Trim();
            string realcount = textBox_pd_count.Text.Trim();
            if (!realqty.All(c => ((c <= '9' && c >= '0'))))
            {
                MessageBox.Show("请填写正确的实际数量!");
                return;
            }

            if (!realcount.All(c=>((c<='9' && c>='0'))))
            {
                MessageBox.Show("请填写正确的实际盘数!");
                return;
            }

            try
            {
                _realqty = int.Parse(realqty);
                _realcount = int.Parse(realcount);

                DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public int GetRealQty()
        {
            return _realqty;
        }

        public int GetRealCount()
        {
            return _realcount;
        }
    }
}
