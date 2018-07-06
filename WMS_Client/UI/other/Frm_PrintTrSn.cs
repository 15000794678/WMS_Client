using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrmBLL;
using RefWebService_BLL;
using TX.Framework.WindowUI.Forms;
using System.IO;
using LabelManager2;
using Phicomm_WMS.DB;
using DevComponents.DotNetBar;
using Phicomm_WMS.OUTIO;

namespace Phicomm_WMS.UI
{
    public partial class Frm_PrintTrSn : Office2007Form /*MainForm*/
    {
        public Frm_PrintTrSn()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 存储SAP_PO信息,以料号存储
        /// </summary>
        Dictionary<string, Dictionary<string, string>> dic_sap_po = new Dictionary<string, Dictionary<string, string>>();
        /// <summary>
        /// 提示消息类型
        /// </summary>
        public enum mLogMsgType { Incoming, Outgoing, Normal, Warning, Error }
        /// <summary>
        /// 提示消息文字颜色
        /// </summary>
        private Color[] mLogMsgTypeColor = { Color.Green, Color.Blue, Color.Black, Color.Orange, Color.Red };

        public void SendMsg(mLogMsgType msgtype, string msg)
        {
            try
            {
                this.rtbmsg.Invoke(new EventHandler(delegate
                {
                    rtbmsg.TabStop = false;
                    rtbmsg.SelectedText = string.Empty;
                    rtbmsg.SelectionFont = new Font(rtbmsg.SelectionFont, FontStyle.Bold);
                    rtbmsg.SelectionColor = mLogMsgTypeColor[(int)msgtype];
                    rtbmsg.AppendText(msg + "\n");
                    rtbmsg.ScrollToCaret();
                }));
            }
            catch
            {
            }
        }

        private void Frm_PrintTrSn_Load(object sender, EventArgs e)
        {
            System.DateTime dt = System.DateTime.Now;
            cbx_Year.Items.Add(dt.ToString("yyyy"));
            cbx_Year.Items.Add(dt.AddYears(-1).ToString("yyyy"));
            cbx_Year.SelectedIndex = 0;

            //material_no, material_desc, vender_no, vender_name, qty, income_qty, status, inventory_id
            dgv_sap_po.Columns.Add("material_no", "物料料号");
            dgv_sap_po.Columns.Add("qty", "应收数量");
            dgv_sap_po.Columns.Add("income_qty", "实收数量");
            dgv_sap_po.Columns.Add("status", "状态");
            dgv_sap_po.Columns.Add("material_desc", "物料描述");
            dgv_sap_po.Columns.Add("vender_name", "厂家名称");
            dgv_sap_po.Columns.Add("inventory_id", "PO_ID");
            dgv_sap_po.Columns.Add("stock_no", "物料凭证单");
            Clear_Control();
            txt_stock_no.Focus();
        }
        private void Clear_Control()
        {
            Lab_KP_NO.Text = string.Empty;
            lab_KP_DESC.Text = string.Empty;
            lab_VENDER_ID.Text = string.Empty;
            lab_VENDER_NAME.Text = string.Empty;
            txt_DATE_CODE.Text = string.Empty;
            Lab_FIFO_DC.Text = string.Empty;
            txt_LOT_ID.Text = string.Empty;
            txt_qty.Text = string.Empty;
            num_PringQty.Value = 1;
            Chk_DateCode.Checked = true;
            Chk_DateCode_CheckedChanged(null, null);

        }

        private void Chk_DateCode_CheckedChanged(object sender, EventArgs e)
        {

            if (!Chk_DateCode.Checked)
            {
                txt_DATE_CODE.Enabled = true;
                txt_DATE_CODE.Text = System.DateTime.Now.ToString("yyyyMMdd");
                Lab_FIFO_DC.Text = System.DateTime.Now.ToString("yyyyMMdd");
                txt_LOT_ID.Text = Lab_FIFO_DC.Text;
                txt_DATE_CODE.ReadOnly = true;
                txt_qty.Focus();
                txt_qty.SelectAll();
            }
            else
            {
                Lab_FIFO_DC.Text = "";
                txt_DATE_CODE.ReadOnly = false;
                txt_DATE_CODE.Enabled = true;
                txt_DATE_CODE.Text = "";
                txt_DATE_CODE.Focus();
                txt_DATE_CODE.SelectAll();
            }

        }
        private DataTable Get_SAP_PO()
        {
            dic_sap_po.Clear();
            Dictionary<string, object> mst = new Dictionary<string, object>();
            mst.Add("STOCK_NO", txt_stock_no.Text);
            mst.Add("STOCK_YEAR", cbx_Year.Text);
            //需要修改，从r_inventory_id表格里面获取数据
            //DataTable dt = FrmBLL.ReleaseData.arrByteToDataTable(refWebtR_Sap_Po.Instance.Get_Sap_Po(FrmBLL.ReleaseData.DictionaryToJson(mst), "ROWID,KP_NO,OQC_STATUS,QTY,INCOME_QTY,STATUS,KP_DESC,VENDER_ID,VENDER_NAME,PO_ID ,PO_ITEM,OQC_NO,FACTORY AS PLANT,STOCK_NO,STOCK_YEAR,REMARK1,PROCESS"));
            DataTable dt = new DataTable();

            foreach (DataRow dr in dt.Rows)
            {
                if (!dic_sap_po.ContainsKey(dr["KP_NO"].ToString()))
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    string Colnum = "KP_NO,OQC_STATUS,QTY,INCOME_QTY,STATUS,KP_DESC,VENDER_ID,VENDER_NAME,PO_ID,PO_ITEM,OQC_NO,PLANT,STOCK_NO,STOCK_YEAR,PROCESS";
                    foreach (string str in Colnum.Split(','))
                    {
                        dic.Add(str.Trim(), dr[str.Trim()].ToString());
                    }
                    dic_sap_po.Add(dr["KP_NO"].ToString(), dic);
                }
                else
                {
                    dic_sap_po[dr["KP_NO"].ToString()]["QTY"] = (Convert.ToInt32(dic_sap_po[dr["KP_NO"].ToString()]["QTY"]) + Convert.ToInt32(dr["QTY"].ToString())).ToString();
                }
            }

            return dt;
        }

        private DataTable GetRInventoryId()
        {
            try
            {
                dic_sap_po.Clear();

                Dictionary<string, object> _dic = new Dictionary<string, object>();
                _dic.Add("stock_no", txt_stock_no.Text);
                _dic.Add("stock_year", cbx_Year.Text);

                SearchRInventoryId sr = new SearchRInventoryId(_dic);
                sr.ExecuteQuery();
                DataTable dt = sr.GetResult();

                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    string Colnum = "material_no, qty, income_qty, status, material_desc, vender_name, inventory_id, stock_no";
                    foreach (string str in Colnum.Split(','))
                    {
                        dic.Add(str.Trim(), "");
                    }
                    dic["material_no"] = dr["material_no"].ToString();
                    dic["qty"] = dr["qty"].ToString();
                    dic["income_qty"] = dr["income_qty"].ToString();
                    dic["status"] = dr["status"].ToString();
                    dic["material_desc"] = dr["material_desc"].ToString();
                    dic["vender_no"] = dr["vender_no"].ToString();
                    dic["vender_name"] = dr["vender_name"].ToString();
                    dic["inventory_id"] = dr["inventory_id"].ToString();                    
                    dic["stock_no"] = txt_stock_no.Text;

                    if (!dic_sap_po.ContainsKey(dic["material_no"]))
                    {
                        dic_sap_po.Add(dic["material_no"], dic);
                    }
                    else
                    {
                        dic_sap_po[dic["material_no"]] = dic;
                    }
                }

                return dt;
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return null;
            }
        }


        public void ShowHint(string msg, Color cl)
        {
            using (Phicomm_WMS.UI.MsgFrm mf = new Phicomm_WMS.UI.MsgFrm(msg, cl))
            {
                mf.BringToFront();
                mf.ShowDialog();
                mf.Dispose();
            }
        }
        private void txt_stock_no_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_stock_no.Text) && e.KeyCode == Keys.Enter)
            {
                dgv_sap_po.Rows.Clear();
                DataTable dt = GetRInventoryId(); // Get_SAP_PO();

                if (dic_sap_po.Count > 0)
                {
                    foreach (Dictionary<string, string> temp in dic_sap_po.Values)
                    {
                        string KP_STATUS = temp["status"];

                        switch (Convert.ToInt32(temp["status"].ToString()))
                        {
                            case 2://0
                                KP_STATUS = "待收料";
                                break;
                            case 3://1
                                KP_STATUS = "收料中";
                                break;
                            case 4://2
                                KP_STATUS = "已完成";
                                break;
                            default:
                                KP_STATUS = "状态异常";
                                break;
                        }
                        //"material_no, qty, income_qty, status, material_desc, vender_name, inventory_id, stock_no"
                        dgv_sap_po.Rows.Add(temp["material_no"],
                                            temp["qty"],
                                            temp["income_qty"],
                                            KP_STATUS,
                                            temp["material_desc"],
                                            //temp["vender_no"],
                                            temp["vender_name"],
                                            temp["inventory_id"],
                                            temp["stock_no"]);
                    }
                }
                else
                {
                    ShowHint("此单号没有数据,请确认....", Color.Red);
                }
                txt_stock_no.SelectAll();
            }
        }

        private void dgv_sap_po_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                Lab_KP_NO.Text = dgv_sap_po.Rows[e.RowIndex].Cells["material_no"].Value.ToString();
                lab_KP_DESC.Text = dic_sap_po[Lab_KP_NO.Text]["material_desc"];
                lab_VENDER_ID.Text = dic_sap_po[Lab_KP_NO.Text]["vender_no"];
                lab_VENDER_NAME.Text = dic_sap_po[Lab_KP_NO.Text]["vender_name"];
            }
        }
        private bool Check_DataFormat(string DateTimeStr)
        {
            try
            {
                System.DateTime.ParseExact(DateTimeStr, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                return true;
            }
            catch
            {
                ShowHint("先进先出日期格式错误", Color.Red);
                return false;
            }
        }
        private void txt_DATE_CODE_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string datefifo = "";
                if (!string.IsNullOrEmpty(txt_DATE_CODE.Text.ToString()))
                {
                    datefifo = FrmBLL.ChangeFIFODateCode.fifo_datecode(txt_DATE_CODE.Text.ToString().ToUpper().Trim());
                    if (string.IsNullOrEmpty(datefifo))
                        Lab_FIFO_DC.Text = System.DateTime.Now.ToString("yyyyMMdd");
                    else
                        Lab_FIFO_DC.Text = datefifo;

                    txt_LOT_ID.Text = Lab_FIFO_DC.Text;
                    if (!Check_DataFormat(Lab_FIFO_DC.Text))
                    {
                        txt_DATE_CODE.Focus();
                        return;
                    }
                    txt_DATE_CODE.Enabled = false;
                    txt_qty.Focus();
                    txt_qty.SelectAll();
                }
            }
        }

        private void txt_LOT_ID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_qty.Focus();
            }
        }

        private void txt_qty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                num_PringQty.Focus();
                num_PringQty.Select(0, num_PringQty.Value.ToString().Length);
            }
        }

        private void num_PringQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                imbt_Print.Focus();
            }
        }

        private void imbt_Print_Click(object sender, EventArgs e)
        {
            string datecode_fifo = "";
            string filepatch = @"C:\Label";
            if (!Directory.Exists(filepatch))
            {
                Directory.CreateDirectory(filepatch);
            }
            filepatch = @"C:\Label\FEIXUN_LOT2.Lab";
            if (!File.Exists(filepatch))  //判断Label档案是否存在
            {
                string mdl = "FEIXUN_LOT2.lab";
                string labpath = string.Format(@"C:\{0}\{1}", "Label", mdl);
                SendMsg(mLogMsgType.Warning, "条码档没有找到, 请检查路径：" + filepatch);
                MessageBox.Show("条码档没有找到, 请检查路径：" + filepatch);
                return;
                //SendMsg(mLogMsgType.Warning, "条码档没有找到,正在从Ftp下载..");
                //FrmBLL.Ftp_MyFtp ftp = new FrmBLL.Ftp_MyFtp();
                //ftp.DownloadFile(mdl, "C:\\Label\\", mdl);
                //SendMsg(mLogMsgType.Normal, "条码档下载完成");
            }
            #region 判断需要列印的资料是否都存在
            if (string.IsNullOrEmpty(Lab_KP_NO.Text))
            {
                ShowHint("请选择需要打印的物料料号！！", Color.Red);
                return;
            }
            if (string.IsNullOrEmpty(Lab_KP_NO.Text))
            {
                ShowHint("请选择需要打印的物料料号！！", Color.Red);
                return;
            }
            if (string.IsNullOrEmpty(txt_DATE_CODE.Text) && Chk_DateCode.Checked && !txt_DATE_CODE.Enabled)
            {
                ShowHint("请输入DateCode！！", Color.Red);
                txt_DATE_CODE.Focus();
                txt_DATE_CODE.SelectAll();
                return;
            }
            if (string.IsNullOrEmpty(txt_LOT_ID.Text))
            {
                ShowHint("请输入生产批次！！", Color.Red);
                txt_LOT_ID.Focus();
                txt_LOT_ID.SelectAll();
                return;
            }
            if (string.IsNullOrEmpty(txt_qty.Text.ToString()))
            {
                ShowHint("请输入打印数量！！", Color.Red);
                txt_qty.Focus();
                txt_qty.SelectAll();
                return;
            }
            #endregion
            #region 判断打印数量和打印张数是否为数字
            try
            {
                int m = 0;
                m = int.Parse(this.txt_qty.Text.Trim());
                if (m < 1)
                {
                    ShowHint("打印数量不可以小于1", Color.Red);
                    return;
                }
            }
            catch
            {
                ShowHint("你输入的打印数量不是数字~!", Color.Red);
                return;
            }
            #endregion

            if (!Chk_DateCode.Checked)
                datecode_fifo = System.DateTime.Now.ToString("yyyyMMdd");
            else
            {
                datecode_fifo = Lab_FIFO_DC.Text; // txt_fifo_dc.Text.Trim();
                if (string.IsNullOrEmpty(datecode_fifo))
                {
                    ShowHint("请输入DateCode！！", Color.Red);
                    return;
                }
            }
            if (!Check_DataFormat(datecode_fifo))
                return;
            int pring_qty = Convert.ToInt32(num_PringQty.Value);
            //string _tr_sn = "1804120000381";
            string _tr_sn = refWebtR_Tr_Sn.Instance.Get_tr_sn_current(pring_qty);
            List<Dictionary<string, object>> Ls_DicPrint = new List<Dictionary<string, object>>();
            for (int i = 0; i < pring_qty; i++)
            {
                Dictionary<string, object> dic_Print = new Dictionary<string, object>();
                _tr_sn = (Convert.ToInt64(_tr_sn) + 1).ToString();
                //if (is_scrap)//如果是mark板,trsn前一位增加X标示
                //    f_trsn = "X" + tr_sn;
                //else
                //    f_trsn = tr_sn;
                dic_Print.Add("TR_SN", _tr_sn);
                dic_Print.Add("PART_NO", Lab_KP_NO.Text);
                dic_Print.Add("DATE_CODE", txt_DATE_CODE.Text);
                dic_Print.Add("UNIT_SIZE", txt_qty.Text);
                dic_Print.Add("VENDER_CODE", lab_VENDER_ID.Text);
                dic_Print.Add("LOT_ID", txt_LOT_ID.Text);
                dic_Print.Add("EMP_NO", MyData.GetUser());
                dic_Print.Add("REMARK", lab_KP_DESC.Text);
                // dic_Print.Add("STORLOC", publicfuntion.Get_Tr_Sn_LocId(mst["KP_NO"].ToString(), mst["PLANT"].ToString()));
                Ls_DicPrint.Add(dic_Print);
                SendMsg(mLogMsgType.Normal, string.Format(string.Format("TR_SN:{0}", _tr_sn)));
            }

            SendMsg(mLogMsgType.Incoming, string.Format("开始打印条码,累计[{0}]张条码.", Ls_DicPrint.Count));
            // PrintLabel(filepatch, Ls_DicPrint);
            PrintLabel_2(filepatch, Ls_DicPrint);
            Clear_Control();
        }

        public void PrintLabel_2(string filepatch, List<Dictionary<string, object>> LsDic)
        {
            LabelManager2.ApplicationClass lbl = new LabelManager2.ApplicationClass();
            lbl.Documents.Open(filepatch, false);// 调用设计好的label文件
            Document doc = lbl.ActiveDocument;
            SendMsg(mLogMsgType.Incoming, "清空模板变量...");
            try
            {
                for (int i = 0; i < doc.Variables.FormVariables.Count; i++)
                {
                    doc.Variables.FormVariables.Item(doc.Variables.FormVariables.Item(i + 1).Name).Value = "";
                }
                SendMsg(mLogMsgType.Normal, string.Format("传送打印信息...."));
                foreach (Dictionary<string, object> lsD in LsDic)
                {
                    foreach (KeyValuePair<string, object> kvp in lsD)
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
                            SendMsg(mLogMsgType.Warning, string.Format("填充打印变量失败:{0}->{1},错误信息：", kvp.Key, kvp.Value.ToString()) + ex.Message);
                        }
                    }
                    doc.PrintDocument(1);
                }
                SendMsg(mLogMsgType.Incoming, string.Format("打印信息已输出,唯一条码[{0}]～～[{1}]", LsDic[0]["TR_SN"], LsDic[LsDic.Count - 1]["TR_SN"]));
            }
            catch (Exception ex)
            {
                SendMsg(mLogMsgType.Error, ex.Message);
            }
            finally
            {
                doc.Close();
                lbl.Quit();
            }
        }

        private void dgv_sap_po_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Frm_PrintTrSn_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Owner.Show();
        }
    }
}
