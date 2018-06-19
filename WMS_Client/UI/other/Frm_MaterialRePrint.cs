using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.IO;
using LabelManager2;
using Phicomm_WMS.DB;
using System.Diagnostics;
using Phicomm_WMS.OUTIO;

namespace WMS_Client.UI
{
    public partial class Frm_Material_RePrint : Office2007Form
    {
        public Frm_Material_RePrint()
        {
            InitializeComponent();
        }

        public void ShowHint(string msg, Color cl)
        {
            using (WMS_Client.UI.MsgFrm mf = new WMS_Client.UI.MsgFrm(msg, cl))
            {
                mf.BringToFront();
                mf.ShowDialog();
                mf.Dispose();
            }
        }

        private DataTable GetDataInfoByTrSn(string trsn)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            DataTable dt = new DataTable();

            try
            {
                dic.Add("tr_sn", trsn);
                SearchRInventoryDetail sr = new SearchRInventoryDetail(dic);
                sr.ExecuteQuery();
                dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(dt.Rows[0]["KP_NO"].ToString()))
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ShowHint("SearchRInventoryDetail: " + trsn + ", " + ex.Message, Color.Red);
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

                return dt;
            }
            catch (Exception ex)
            {
                ShowHint("SearchBMaterial: " + ex.Message, Color.Red);
                return null;
            }            
        }

        
        private void txt_tr_sn_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_tr_sn.Text) && e.KeyCode == Keys.Enter)
            {

                try
                {
                    string filepatch = @"C:\Label\FEIXUN_LOT2.Lab";
                    if (!File.Exists(filepatch))  //判断Label档案是否存在
                    {
                        string mdl = "FEIXUN_LOT2.lab";
                        string labpath = string.Format(@"C:\{0}\{1}", "Label", mdl);
                        ShowHint("条码档没有找到, 请检查路径：" + filepatch, Color.Red);
                        return;
                    }

                    string _StrErr = string.Empty;
                    DataTable _dt = GetDataInfoByTrSn(txt_tr_sn.Text);//根据TrSn查询信息
                    if (_dt!=null && _dt.Rows.Count > 0)
                    {
                        #region  填充控件值
                        FrmBLL.publicfuntion.Fill_Control(panelEx3,_dt);
                        #endregion

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("TR_SN", _dt.Rows[0]["TR_SN"].ToString());
                        dic.Add("PART_NO", _dt.Rows[0]["KP_NO"].ToString());
                        dic.Add("DATE_CODE", _dt.Rows[0]["DATE_CODE"].ToString());
                        dic.Add("UNIT_SIZE", _dt.Rows[0]["QTY"].ToString());
                        dic.Add("VENDER_CODE", _dt.Rows[0]["VENDER_ID"].ToString());
                        dic.Add("LOT_ID", _dt.Rows[0]["LOT_CODE"].ToString());
                        dic.Add("REMARK", _dt.Rows[0]["KP_DESC"].ToString());
                        dic.Add("STORLOC", string.Format("{0}/{1}", _dt.Rows[0]["STOCK_ID"].ToString(), _dt.Rows[0]["LOC_ID"].ToString()));
                        dic.Add("EMP_NO", MyData.GetUser());

                        List<Dictionary<string, object>> Ls_DicPrint = new List<Dictionary<string, object>>();
                        Ls_DicPrint.Add(dic);

                        PrintLabel_2(filepatch, Ls_DicPrint);
                    }
                    else
                    {
                        ShowHint("TR_SN 错误", Color.Red);
                    }
                }
                catch (Exception ex)
                {
                    ShowHint(ex.Message, Color.Red);
                }
                finally
                {
                    txt_tr_sn.Text = string.Empty;
                }
            }
        }

        private void Frm_Material_RePrint_Load(object sender, EventArgs e)
        {
            LabelFlePatch.Text = "C:\\Label\\FEIXUN_LOT2.LAB";
            txt_tr_sn.Focus();
        }

        public void PublicPrintLabel(Dictionary<string, string> dic)
        {
            if (!File.Exists(LabelFlePatch.Text))  //判断条码文件是否存在
            {
                MessageBox.Show("条码档没有找到,路径:" + LabelFlePatch.Text);
                return;
            }

            ApplicationClass lbl = new ApplicationClass();
            try
            {
                lbl.Documents.Open(LabelFlePatch.Text, false);// 调用设计好的label文件
                Document doc = lbl.ActiveDocument;          
                for (int i = 0; i < doc.Variables.FormVariables.Count; i++)
                {
                    doc.Variables.FormVariables.Item(doc.Variables.FormVariables.Item(i + 1).Name).Value = "";
                }
                Trace.WriteLine("Debug: " + string.Format("模板变量清空完成,共计{0}个...", doc.Variables.FormVariables.Count));
                foreach (KeyValuePair<string, string> _DicKeyValues in dic)
                {
                    try
                    {
                        doc.Variables.FormVariables.Item(_DicKeyValues.Key).Value = _DicKeyValues.Value; //给参数传值                     
                        Trace.WriteLine("Debug: " + string.Format("填充打印变量完成:{0}->{1}", _DicKeyValues.Key, _DicKeyValues.Value));
                    }
                    catch
                    {
                    }
                }
                int Num = Convert.ToInt32(numPrintQty.Value);        //打印数量          
                doc.PrintDocument(Num);               //打印
                Trace.WriteLine("Debug: 打印完成");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Debug: 发生异常" + ex.Message);
                MessageBox.Show("发生异常" + ex.Message);
            }
            finally
            {
                lbl.Quit(); //退出
            }
        }

        public void PrintLabel_2(string filepatch, List<Dictionary<string, object>> LsDic)
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
                foreach (KeyValuePair<string, object> kvp in LsDic[0])
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
                doc.PrintDocument(LsDic.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                doc.Close();
                lbl.Quit();
            }
        }
    }
}