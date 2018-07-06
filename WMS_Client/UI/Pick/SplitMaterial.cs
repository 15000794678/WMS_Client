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
using LabelManager2;
using System.IO;
using Phicomm_WMS.OUTIO;
using System.Threading;

namespace Phicomm_WMS.UI
{
    public partial class SplitMaterial : Office2007Form
    {       
        private string _filePath = @"C:\Label\FEIXUN_LOT2.Lab";
        private string _trSn = string.Empty;
        private string _trSn2 = string.Empty;
        private int _sendQty = 0;
        private DataTable _dt = new DataTable();

        public SplitMaterial()
        {
            InitializeComponent();
        }

        public void SetLabel(string trSn, string trSn2, int sendQty)
        {
            _trSn = trSn;
            _trSn2 = trSn2;
            _sendQty = sendQty;
        }

        private void ShowHint(string msg, Color cl)
        {
            using (MsgFrm mf = new MsgFrm(msg, cl))
            {
                mf.BringToFront();
                mf.ShowDialog();
                mf.Dispose();
            }
        }

        private DataTable GetTrSnInfo(string trSn)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            DataTable dt = new DataTable();

            try
            {
                dic.Add("tr_sn", trSn);
                SearchRInventoryDetail sr = new SearchRInventoryDetail(dic);
                sr.ExecuteQuery();
                dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("在r_inventory_detail表中查询不到该trSn:" + trSn, Color.Red);
                    return null;
                }

                if (string.IsNullOrEmpty(dt.Rows[0]["KP_NO"].ToString()))
                {
                    ShowHint("该TrSn" + trSn + "在r_inventory_detail表中的料号为空!", Color.Red);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ShowHint("GetDataInfoByTrSn: " + ex.Message, Color.Red);
                return null;
            }

            try
            {
                dic.Clear();

                dic.Add("material_no", dt.Rows[0]["KP_NO"].ToString());
                SearchBMaterial sb = new SearchBMaterial(dic);
                sb.ExecuteQuery();
                DataTable dt2 = sb.GetResult();

                dt.Rows[0]["KP_DESC"] = dt2.Rows[0]["material_desc"];

                if (string.IsNullOrEmpty(dt.Rows[0]["KP_DESC"].ToString().Trim()))
                {
                    ShowHint("料号：" + dt.Rows[0]["KP_NO"].ToString() + "在b_material中的物料描述为空!", Color.Red);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ShowHint("SearchBMaterial: " + ex.Message, Color.Red);
                return null;
            }

            return dt;
        }

        private void PrintLabel(string filepatch, List<Dictionary<string, object>> LsDic)
        {
            LabelManager2.ApplicationClass lbl = new LabelManager2.ApplicationClass();
            lbl.Documents.Open(filepatch, false);// 调用设计好的label文件
            Document doc = lbl.ActiveDocument;
            try
            {
                for (int i = 0; i < doc.Variables.FormVariables.Count; i++)
                {
                    doc.Variables.FormVariables.Item(doc.Variables.FormVariables.Item(i + 1).Name).Value = "";
                }
                foreach (Dictionary<string, object> dic in LsDic)
                {
                    foreach (KeyValuePair<string, object> kvp in dic)
                    {
                        try
                        {
                            doc.Variables.FormVariables.Item(kvp.Key).Value = kvp.Value.ToString(); //给参数传值
                            if (kvp.Key == "TR_SN")
                            {
                                String Tr_Sn_Prefix = kvp.Value.ToString().Substring(0, 6);
                                if (kvp.Value.ToString().Contains("X"))
                                    Tr_Sn_Prefix = kvp.Value.ToString().Substring(0, 7);
                                doc.Variables.FormVariables.Item("TR_SN_PREFIX").Value = Tr_Sn_Prefix; //TR_SN前缀
                                doc.Variables.Counters.Item("COUNT1").BaseType = LabelManager2.enumCounterBase.lppxBaseDecimal; //修改公式为10进制
                                doc.Variables.Counters.Item("COUNT1").Value = kvp.Value.ToString().Substring(Tr_Sn_Prefix.Length, kvp.Value.ToString().Length - Tr_Sn_Prefix.Length);//填充开始流水号
                                doc.Variables.Counters.Item("COUNT1").MaxValue = "9999999";
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("填充打印变量失败:{0}->{1},错误信息：", kvp.Key, kvp.Value.ToString()) + ex.Message);
                        }
                    }
                    doc.PrintDocument(1);
                }
            }
            catch (Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
            }
            finally
            {
                doc.Close();
                lbl.Quit();
            }
        }

        public bool LoadData()
        {
            try
            {
                //检查打印模板是否存在
                _filePath = @"C:\Label\FEIXUN_LOT2.Lab";
                if (!File.Exists(_filePath))  //判断Label档案是否存在
                {
                    string mdl = "FEIXUN_LOT2.lab";
                    string labpath = string.Format(@"C:\{0}\{1}", "Label", mdl);
                    ShowHint("条码档没有找到, 请检查路径：" + _filePath, Color.Red);
                    return false;
                }

                //根据trSn查询条码信息
                _dt = GetTrSnInfo(_trSn);
                if (_dt == null || _dt.Rows.Count == 0)
                {
                    return false;
                }

                //数据显示
                textBox_trsn1.Text = _trSn;
                textBox_qty1.Text = (int.Parse(_dt.Rows[0]["QTY"].ToString()) + _sendQty).ToString();
                textBox_trsn2.Text = _trSn;
                textBox_qty2.Text = _dt.Rows[0]["QTY"].ToString();
                textBox_trsn3.Text = _trSn2;
                textBox_qty3.Text = _sendQty.ToString();
                textBox_kpno.Text = _dt.Rows[0]["KP_NO"].ToString();
                textBox_kpdesc.Text = _dt.Rows[0]["KP_DESC"].ToString();
                textBox_vendorid.Text = _dt.Rows[0]["VENDER_ID"].ToString();
                textBox_datecode.Text = _dt.Rows[0]["DATE_CODE"].ToString();
                textBox_lotcode.Text = _dt.Rows[0]["LOT_CODE"].ToString();
                
                Thread t1 = new Thread(DoPrint);

                t1.Start();

                return true;
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                button1.Enabled = true;
                button2.Enabled = true;
                label_hint.Text = "分盘标签打印异常!";
                label_hint.BackColor = Color.Red;
                //this.DialogResult = DialogResult.OK;
                return false;
            }
        }

        private void DoPrint()
        {
            //数据转换
            Dictionary<string, object> dic1 = new Dictionary<string, object>();
            dic1.Add("TR_SN", textBox_trsn2.Text.Trim());
            dic1.Add("PART_NO", textBox_kpno.Text.Trim()/*_dt.Rows[0]["KP_NO"].ToString()*/);
            dic1.Add("DATE_CODE", textBox_datecode.Text.Trim()/*_dt.Rows[0]["DATE_CODE"].ToString()*/);
            dic1.Add("UNIT_SIZE", textBox_qty2.Text.Trim());
            dic1.Add("VENDER_CODE", textBox_vendorid.Text.Trim()/*_dt.Rows[0]["VENDER_ID"].ToString()*/);
            dic1.Add("LOT_ID", textBox_lotcode.Text.Trim()/*_dt.Rows[0]["LOT_CODE"].ToString()*/);
            dic1.Add("REMARK", textBox_kpdesc.Text.Trim()/*_dt.Rows[0]["KP_DESC"].ToString()*/);
            dic1.Add("STORLOC", string.Format("{0}/{1}", _dt.Rows[0]["STOCK_ID"].ToString(), _dt.Rows[0]["LOC_ID"].ToString()));
            dic1.Add("EMP_NO", MyData.GetUser());

            Dictionary<string, object> dic2 = new Dictionary<string, object>();
            dic2.Add("TR_SN", textBox_trsn3.Text.Trim());
            dic2.Add("PART_NO", textBox_kpno.Text.Trim()/*_dt.Rows[0]["KP_NO"].ToString()*/);
            dic2.Add("DATE_CODE", textBox_datecode.Text.Trim()/*_dt.Rows[0]["DATE_CODE"].ToString()*/);
            dic2.Add("UNIT_SIZE", textBox_qty3.Text.Trim());
            dic2.Add("VENDER_CODE", textBox_vendorid.Text.Trim()/*_dt.Rows[0]["VENDER_ID"].ToString()*/);
            dic2.Add("LOT_ID", textBox_lotcode.Text.Trim()/*_dt.Rows[0]["LOT_CODE"].ToString()*/);
            dic2.Add("REMARK", textBox_kpdesc.Text.Trim()/*_dt.Rows[0]["KP_DESC"].ToString()*/);
            dic2.Add("STORLOC", string.Format("{0}/{1}", _dt.Rows[0]["STOCK_ID"].ToString(), _dt.Rows[0]["LOC_ID"].ToString()));
            dic2.Add("EMP_NO", MyData.GetUser());

             List<Dictionary<string, object>> _LsDicPrint = new List<Dictionary<string, object>>();
            _LsDicPrint.Clear();
            _LsDicPrint.Add(dic1);
            _LsDicPrint.Add(dic2);
            PrintLabel(_filePath, _LsDicPrint);

            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {                    
                    button1.Enabled = true;
                    button2.Enabled = true;
                    label_hint.Text = "分盘标签打印完毕!";
                    label_hint.BackColor = Color.Lime;

                    //this.DialogResult = DialogResult.OK;
                }));
            }            
        }

        private void SplitMaterial_Load(object sender, EventArgs e)
        {
            this.TitleText = "分盘标签打印中...";
            label_hint.Text = "分盘标签打印中...";
            label_hint.BackColor = Color.Yellow;
            LoadData();            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
