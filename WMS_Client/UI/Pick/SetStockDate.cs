using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Phicomm_WMS.DB;

namespace Phicomm_WMS.UI
{
    public partial class SetStockDate : Office2007Form
    {
        private string _date = string.Empty;
        private string _plant = string.Empty;
        private string _stock_id = string.Empty;

        public SetStockDate()
        {
            InitializeComponent();
        }

        public string GetDate()
        {
            return _date;
        }

        public string GetPlant()
        {
            return _plant;
        }

        public string GetStockId()
        {
            return _stock_id;
        }

        private void SetStockDate_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void textBox1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                MessageBox.Show("盘点日期不能为空!");
                return;
            }

            textBox2.Focus();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                MessageBox.Show("工厂不能为空!");
                return;
            }

            textBox3.Focus();
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox3.Text.Trim()))
            {
                MessageBox.Show("库位不能为空!");
                textBox3.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                MessageBox.Show("工厂不能为空!");
                textBox2.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                MessageBox.Show("盘点日期不能为空!");
                textBox1.Focus();
                return;
            }

            try
            {
                _date = textBox1.Text.Trim();
                _plant = textBox2.Text.Trim();
                _stock_id = textBox3.Text.Trim();
                if (_date.Length != 10)
                {
                    MessageBox.Show("长度不对，应为8位");
                    return;
                }
                if (!_date.All(c => (c >= '0' && c <= '9')))
                {
                    MessageBox.Show("输入格式不对，格式必须为：20180608，请重新输入!");
                    return;
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();

                //检查要盘点的信息是否有记录
                dic.Clear();
                dic.Add("plant", _plant);
                dic.Add("stock_id", _stock_id);
                dic.Add("status", "1");
                SearchRInventoryDetail sr = new SearchRInventoryDetail(dic);
                sr.ExecuteQuery();
                DataTable dt = sr.GetResult();
                if (dt==null || dt.Rows.Count==0)
                {
                    MessageBox.Show("根据工厂和库位查询不到在库物料，请修改盘点条件!");
                    textBox3.Focus();
                    return;
                }

                //检查盘点是否重复
                dic.Clear();
                dic.Add("check_date", _date);
                dic.Add("plant", _plant);
                dic.Add("stock_id", _stock_id);
                SearchInventoryCheckByMaterialNo sc = new SearchInventoryCheckByMaterialNo(dic);
                sc.ExecuteQuery();
                dt = sc.GetResult();
                if (dt != null && dt.Rows.Count > 0)
                {
                    string status = dt.Rows[0]["Status"].ToString();
                    if (status.Equals("1"))
                    {
                        MessageBox.Show("该日期，工厂，库位的条件已经盘点过，请修改盘点信息！");
                        textBox1.Text = "";
                        textBox1.Focus();
                        return;
                    }
                }
                else
                {
                    //插入盘点信息
                    InsertInventoryCheckByMaterialNo ic = new InsertInventoryCheckByMaterialNo(_date, "*", _plant, _stock_id);
                    ic.ExecuteUpdate();
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                textBox1.Text = "";
                textBox1.Focus();
            }
        }
    }
}
