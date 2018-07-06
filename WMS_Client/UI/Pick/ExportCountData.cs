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
using WHC.OrderWater.Commons;

namespace Phicomm_WMS.UI
{
    public partial class ExportCountData : Office2007Form
    {
        public ExportCountData()
        {
            InitializeComponent();
        }

        private void button_query_Click(object sender, EventArgs e)
        {            
            dataGridView1.Rows.Clear();

            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                if (!string.IsNullOrEmpty(textBox_datestart.Text.Trim()))
                {
                    dic.Add("remark", textBox_datestart.Text.Trim());
                }
                if (!string.IsNullOrEmpty(textBox_user.Text.Trim()))
                {
                    dic.Add("user", textBox_user.Text.Trim());
                }
                if (!string.IsNullOrEmpty(textBox_plant.Text.Trim()))
                {
                    dic.Add("plant", textBox_plant.Text.Trim());
                }
                if (!string.IsNullOrEmpty(textBox_stockid.Text.Trim()))
                {
                    dic.Add("stock_id", textBox_stockid.Text.Trim());
                }

                SearchInventoryCheckResult si = new SearchInventoryCheckResult(dic);
                si.ExecuteQuery();
                DataTable dt = si.GetResult();
                if (dt==null || dt.Rows.Count==0)
                {
                    MessageBox.Show("查询不到数据");
                    return;
                }

                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {                   
                    dataGridView1.Rows.Add(i,
                                        dr["Date"].ToString(),
                                        dr["LocId"].ToString(),
                                        dr["KpNo"].ToString(),
                                        dr["Qty"].ToString(),
                                        dr["Count"].ToString(),
                                        dr["RealQty"].ToString(),
                                        dr["RealCount"].ToString(),
                                        dr["Plant"].ToString(),
                                        dr["StockId"].ToString(),
                                        dr["User"].ToString(),
                                        dr["Remark"].ToString()
                                        );
                    if (int.Parse(dr["Qty"].ToString()) != int.Parse(dr["RealQty"].ToString()) ||
                        int.Parse(dr["Count"].ToString()) != int.Parse(dr["RealCount"].ToString()))
                    {
                        dataGridView1.Rows[i-1].DefaultCellStyle.BackColor = Color.Yellow;
                    }
                    i++;
                }
                
                dataGridView1.ClearSelection();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void button_export_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("当前无数据需要保存");
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "xls文件(*.xls)|*.xls|xlsx文件(*.xlsx)|*.xls|所有文件(*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                
                DataTable dt = new DataTable();
                dt.Columns.Add("No", typeof(string));
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("LocId", typeof(string));
                dt.Columns.Add("KpNo", typeof(string));
                dt.Columns.Add("Qty", typeof(string));
                dt.Columns.Add("Count", typeof(string));                
                dt.Columns.Add("RealQty", typeof(string));
                dt.Columns.Add("RealCount", typeof(string));
                dt.Columns.Add("Plant", typeof(string));
                dt.Columns.Add("StockId", typeof(string));
                dt.Columns.Add("User", typeof(string));                
                dt.Columns.Add("Remark", typeof(string));

                for(int i=0; i<dataGridView1.Rows.Count; i++)
                {
                    dt.Rows.Add(dataGridView1[0, i].Value.ToString(),
                                dataGridView1[1, i].Value.ToString(),
                                dataGridView1[2, i].Value.ToString(),
                                dataGridView1[3, i].Value.ToString(),
                                dataGridView1[4, i].Value.ToString(),
                                dataGridView1[5, i].Value.ToString(),
                                dataGridView1[6, i].Value.ToString(),
                                dataGridView1[7, i].Value.ToString(),
                                dataGridView1[8, i].Value.ToString(),
                                dataGridView1[9, i].Value.ToString(),
                                dataGridView1[10, i].Value.ToString(),
                                dataGridView1[11, i].Value.ToString()
                        );
                }
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                try
                {
                    ExcelHelper.DataSetToExcel(ds, sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存数据异常：" + ex.Message);
                    return;
                }

                MessageBox.Show("数据已经成功保存文件： " + sfd.FileName + " 中！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExportStockCountData_Load(object sender, EventArgs e)
        {
            button_query_Click(sender, e);
        }

        private void textBox_datestart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            button_query_Click(sender, e);
        }

        private void textBox_user_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            button_query_Click(sender, e);
        }
    }
}
