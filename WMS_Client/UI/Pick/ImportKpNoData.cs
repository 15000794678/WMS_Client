using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using WHC.OrderWater.Commons;
using Phicomm_WMS.DB;

namespace Phicomm_WMS.UI
{
    public partial class ImportKpNoData : Office2007Form
    {
        private string _woid = string.Empty;
        private string _plant = string.Empty;
        private string _stock_id = string.Empty;

        public ImportKpNoData()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "xls文件(*.xls)|*.xls|xlsx文件(*.xlsx)|*.xls|所有文件(*.*)|*.*";
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                textBox1.Text = ofd.FileName;
            }

            DataTable dt = new DataTable();
            try
            {
                DataSet ds = ExcelHelper.ExcelToDataSet(textBox1.Text, "Sheet1$", true, ExcelHelper.ExcelType.Excel2003);
                if (ds == null || ds.Tables.Count == 0)
                {
                    MessageBox.Show("文件中无数据!");
                    return;
                }
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("ExcelToDataSet:" + ex.Message);
                return;
            }

            if (dt == null || dt.Rows.Count==0)
            {
                MessageBox.Show("文件中无数据!");
                return;
            }

            dataGridView1.Rows.Clear();            
            foreach (DataRow dr in dt.Rows)
            {
                dataGridView1.Rows.Add(dr[0].ToString(),
                                       dr[1].ToString(),//dr["CheckDate"].ToString(),
                                       dr[2].ToString(),//dr["KpNo"].ToString()
                                       dr[3].ToString(),
                                       dr[4].ToString()
                                   );
            }

            dataGridView1.ClearSelection();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count==0)
            {
                MessageBox.Show("无数据需要导入！");
                return;
            }

            string checkdate = "";
            string kpno = "";
            string plant = "";
            string stock_id = "";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    checkdate = dataGridView1[1, i].Value.ToString();
                    kpno = dataGridView1[2, i].Value.ToString();
                    plant = dataGridView1[3, i].Value.ToString();
                    stock_id = dataGridView1[4, i].Value.ToString();

                    dic.Clear();
                    dic.Add("check_date", checkdate);
                    dic.Add("material_no", kpno);
                    dic.Add("plant", plant);
                    dic.Add("stock_id", stock_id);                  
                    SearchInventoryCheckByMaterialNo sc = new SearchInventoryCheckByMaterialNo(dic);
                    sc.ExecuteQuery();
                    DataTable dt = sc.GetResult();
                    if (dt!=null && dt.Rows.Count>0)
                    {
                        continue;
                    }

                    InsertInventoryCheckByMaterialNo ic = new InsertInventoryCheckByMaterialNo(checkdate, kpno, plant, stock_id);
                    ic.ExecuteUpdate();
                }

                _woid = dataGridView1[1, 0].Value.ToString();
                _plant = dataGridView1[3, 0].Value.ToString();
                _stock_id = dataGridView1[4, 0].Value.ToString();

                MessageBox.Show("导入成功!");

                DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                MessageBox.Show("导入异常：" + ex.Message);
            }
        }

        public string GetWoId()
        {
            return _woid;
        }

        public string GetPlant()
        {
            return _plant;
        }

        public string GetStockId()
        {
            return _stock_id;
        }
    }
}
