using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Threading;
using log4net;
using Phicomm_WMS.OUTIO;
using System.Diagnostics;
using Phicomm_WMS.DB;
using WHC.OrderWater.Commons;

namespace Phicomm_WMS.UI
{
    public partial class ReplenishFrm : Office2007Form
    {
        private readonly ILog Log = LogManager.GetLogger("ReplenishFrm");
        private Dictionary<int, Dictionary<Label, Color>> _dicShelfId = new Dictionary<int, Dictionary<Label, Color>>();


        private bool runFlag = true;  //该窗体是否存在标志，窗体未关闭前，一直为true
        private bool findFlag = false; //查找货架是否到达标志，未到达时一直搜索，到时时停止搜索
        private AutoResetEvent eventStartFind = new AutoResetEvent(false);  //需要查找货架是否到达信号量
        private AutoResetEvent eventExecuteInput = new AutoResetEvent(false); //需要解析输入的信号量

        private string _lastScanLocId = "";  //记录上一次的储位编号
        //private int _allOutCnt = 0;  //整盘出库时计数
        private DataTable _dtShelf = new DataTable(); //货架信息

        public ReplenishFrm()
        {
            InitializeComponent();
        }

        private void ReplenishFrm_Load(object sender, EventArgs e)
        {
            InitUI();

            //搜索货架线程
            Thread t3 = new Thread(QueryThread);
            t3.Start();

            //执行输入线程
            Thread t4 = new Thread(ExecuteThread);
            t4.Start();

            Thread t5 = new Thread(KeepAliveThread);
            t5.Start();

            //StartFindShelf();

        }

        private void InitUI()
        {
            this.TitleText = "欢迎使用, " + MyData.GetUser() + " !";
            label_stationid.Text = "拣选站点 " + MyData.GetStationId().ToString();
            label_KpNo.Text = "";
            label_StockNo.Text = "";
            label_holderId.Text = "";
            label_LocId.Text = "";
            tabPage_handreplenish.Hide();

            //货架信息
            _dicShelfId.Clear();
            tableLayoutPanel_ShelfId.Controls.Clear();
            for (int i = 0; i < 20; i++)
            {
                int index = (5 - i / 4) * 10 + (i % 4 + 1);

                Label lb = new Label();
                lb.Name = "Label_ShelId" + i.ToString();
                //lb.Text = index.ToString();
                lb.AutoSize = false;
                lb.Dock = DockStyle.Fill;
                lb.TextAlign = ContentAlignment.MiddleCenter;
                lb.Margin = new System.Windows.Forms.Padding(3);
                lb.BackColor = Color.DarkGray;

                tableLayoutPanel_ShelfId.Controls.Add(lb);

                Dictionary<Label, Color> dicColor = new Dictionary<Label, Color>();
                dicColor.Add(lb, Color.DarkGray);

                if (_dicShelfId.ContainsKey(index))
                {
                    _dicShelfId[index] = dicColor;
                }
                else
                {
                    _dicShelfId.Add(index, dicColor);
                }
            }

        }

        private void ReplenishFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            runFlag = false;
            findFlag = true;

            eventExecuteInput.Set();
            eventStartFind.Set();
        }

        private void RefreshUI(int row, int col, DataTable dt)
        {
            if (!runFlag) return;

            try
            {
                if (this.IsHandleCreated)
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        Trace.WriteLine("Debug: ------RefreshUI Start------");
                        if (dt == null || dt.Rows.Count == 0)
                        {//清空显示
                            HighLight(MyData._lastArrive.select, Color.DarkGray);

                            MyData._lastArrive.select = 0;
                            MyData._lastArrive.stockno = "";
                            MyData._lastArrive.locid = "";
                            MyData._lastArrive.kpno = "";
                            MyData._lastArrive.hoderid = "";

                            label_ShelfId.Text = "";
                            label_StockNo.Text = MyData._lastArrive.stockno;
                            label_LocId.Text = MyData._lastArrive.locid;
                            label_LocId.BackColor = Color.Transparent;
                            label_KpNo.Text = MyData._lastArrive.kpno;
                            label_holderId.Text = MyData._lastArrive.hoderid;
                            label_holderId.BackColor = Color.Transparent;

                            Trace.WriteLine("Debug: ------RefreshUI Stop------");

                            return;
                        }

                        Trace.WriteLine("Debug: Col" + col.ToString());
                        //需要更新
                        if (MyData._lastArrive.stockno != null &&
                            MyData._lastArrive.locid != null &&
                            MyData._lastArrive.kpno != null &&
                            MyData._lastArrive.hoderid != null &&
                            MyData._lastArrive.stockno.Equals(dt.Rows[0]["StockNo"].ToString().Trim()) &&
                            MyData._lastArrive.locid.Equals(dt.Rows[0]["BoxBarcode"].ToString().Trim()) &&
                            MyData._lastArrive.kpno.Equals(dt.Rows[0]["MaterialId"].ToString().Trim()) &&
                            MyData._lastArrive.hoderid.Equals(dt.Rows[0]["HoderId"].ToString().Trim()) &&
                            MyData._lastArrive.col==col)
                        {
                            findFlag = true;
                            return;
                        }

                        if (MyData._lastArrive.locid == null ||
                            !MyData._lastArrive.locid.Equals(dt.Rows[0]["BoxBarcode"].ToString().Trim()))
                        {
                            label_Hint.Text = "请扫储位编号";
                            HighLight(MyData._lastArrive.select, Color.DarkGray);
                            label_LocId.BackColor = Color.Lime;
                            label_holderId.BackColor = Color.Transparent;
                            textBox_BarCode.Focus();
                        }
                        else /*if (!MyData._lastArrive.kpno.Equals(dt.Rows[0]["MaterialId"].ToString().Trim()) ||
                                MyData._lastArrive.qty.Equals(dt.Rows[0]["Qty"].ToString().Trim()))*/
                        {
                            label_Hint.Text = "请扫周转箱编号";
                            textBox_BarCode.Focus();
                        }

                        string boxBarCode = dt.Rows[0]["BoxBarcode"].ToString();
                        if (boxBarCode.Length < 3)
                        {
                            Log.Error("RefreshUI: BoxBarCode lenght wrong");
                            //return;
                        }

                        #region 刷新货架信息
                        if (col == 2 )
                        {
                            foreach (KeyValuePair<int, Dictionary<Label, Color>> obj in _dicShelfId)
                            {
                                //Trace.WriteLine("Debug: " + obj.Key.ToString() + ", " + (obj.Key % 10).ToString() + ", " + col);
                                if (obj.Key % 10 > col)
                                {
                                    Dictionary<Label, Color> dic = obj.Value;
                                    foreach (KeyValuePair<Label, Color> cl in dic)
                                    {
                                        cl.Key.BackColor = Color.Transparent;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<int, Dictionary<Label, Color>> obj in _dicShelfId)
                            {
                                Dictionary<Label, Color> dic = obj.Value;
                                foreach (KeyValuePair<Label, Color> cl in dic)
                                {
                                    cl.Key.BackColor = Color.DarkGray;
                                    break;
                                }
                            }
                        }
                        #endregion

                        MyData._lastArrive.select = int.Parse(boxBarCode.Substring(boxBarCode.Length - 2, 2));
                        MyData._lastArrive.stockno = dt.Rows[0]["StockNo"].ToString().Trim();
                        MyData._lastArrive.locid = dt.Rows[0]["BoxBarcode"].ToString().Trim();
                        MyData._lastArrive.kpno = dt.Rows[0]["MaterialId"].ToString().Trim();
                        MyData._lastArrive.hoderid = dt.Rows[0]["HoderId"].ToString().Trim();
                        MyData._lastArrive.col = col;

                        label_ShelfId.Text = dt.Rows[0]["PodId"].ToString().Trim() + ",    " +
                                             (dt.Rows[0]["PodSide"].ToString().Trim().Equals("0") ? "正面" : "反面");

                        if (!label_StockNo.Text.Trim().Equals(MyData._lastArrive.stockno))
                        {
                            label_StockNo.Text = MyData._lastArrive.stockno;
                        }
                        if (!label_LocId.Text.Trim().Equals(MyData._lastArrive.locid))
                        {
                            label_LocId.Text = MyData._lastArrive.locid;
                        }
                        if (!label_KpNo.Text.Trim().Equals(MyData._lastArrive.kpno))
                        {
                            label_KpNo.Text = MyData._lastArrive.kpno;
                        }
                        if (!label_holderId.Text.Trim().Equals(MyData._lastArrive.hoderid.ToString()))
                        {
                            label_holderId.Text = MyData._lastArrive.hoderid.ToString();
                        }                        
                        HighLight(MyData._lastArrive.select, Color.Lime);
                        

                        //保存工单号
                        MyData.SetStockNo(MyData._lastArrive.stockno);

                        findFlag = true;

                        Trace.WriteLine("Debug: ------RefreshUI Stop------");
                    }));
                }
            }
            catch (Exception ex)
            {
                Log.Error("RefreshUI: " + ex.Message);
                Trace.WriteLine("Debug: " + ex.Message);
                Trace.WriteLine("Debug: ------RefreshUI Stop------");
            }
        }

        private void HighLight(int index, Color cl)
        {
            if (_dicShelfId.ContainsKey(index))
            {
                Dictionary<Label, Color> dicColor = _dicShelfId[index];
                foreach (KeyValuePair<Label, Color> _dic in dicColor)
                {
                    _dic.Key.BackColor = cl;
                    break;
                }
            }
        }

        private void ShowMessage(string str)
        {
            if (!runFlag) return;

            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {
                    label_Hint.Text = str;
                }));
            }
        }

        private void EnableInput(bool en)
        {
            if (!runFlag) return;

            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {
                    textBox_BarCode.Enabled = en;
                    if (en)
                    {
                        textBox_BarCode.Text = "";
                        textBox_BarCode.Focus();
                    }
                }));
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

        private void KeepAliveThread()
        {
            int i = 0;

            try
            {
                DBPCaller.InitReplenishProcess(MyData.GetUser(), MyData.GetStationId());
            }
            catch (Exception ex)
            {
                ShowHint("InitReplenishProcess:" + ex.Message, Color.Red);
            }

            while (runFlag)
            {
                try
                {
                    DBPCaller.KeepAlive(MyData.GetStationId());
                }
                catch (Exception ex)
                {
                    Log.Error("KeepAlive:" + ex.Message);
                }

                while (runFlag)
                {
                    if (i < 30)
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                        break;
                    }
                    Thread.Sleep(100);
                }
            }

            try
            {
                DBPCaller.DeinitProcess(MyData.GetStationId());
            }
            catch (Exception ex)
            {
                ShowHint("DeinitProcess:" + ex.Message, Color.Red);
            }
        }

        private bool AgvLeave(int stationId)
        {
            try
            {
                int result = DBPCaller.AgvReturn(stationId);
                if (result == -3)
                {
                    ShowHint("当前站点还有未完成拣选的出库物料", Color.Red);
                    return false;
                }
                else if (result == -2)
                {
                    ShowHint("当前站点还有未完成拣选的入库物料", Color.Red);
                    return false;
                }
                else if (result == -1)
                {
                    ShowHint("当前站点没有可用任务", Color.Red);
                    return false;
                }
                else
                {
                    //ShowHint("小车离开成功", Color.Lime);
                    ShowMessage("小车离开成功");
                    return true;
                }
            }
            catch (Exception ex)
            {
                ShowHint("AgvReturn: " + ex.Message, Color.Red);
                return false;
            }
        }

        private void StartFindShelf()
        {
            findFlag = false;
            eventStartFind.Set();
        }

        private void StopFindShelf()
        {
            findFlag = true;
            ShowHint("已停止查找货架信息", Color.Lime);
        }

        private void SapReplenish(string woid)
        {
            if (string.IsNullOrEmpty(woid))
            {
                return;
            }

            int stockNoType = 0;
            if (!GetStockNoType(woid, ref stockNoType))
            {
                return;
            }

            if (stockNoType!=7 && stockNoType!=8)
            {
                ShowHint("查询到该工单的类型为：" + stockNoType + ", 异常", Color.Red);
                return;
            }

            try
            {
                string result = "";
                string remark = "";
                string tranno = "";

                //根据工单类型，选择movetype的优先级
                string[] movetype = null;
                if (stockNoType == 7)
                {
                    movetype = new string[] { "311" };
                }
                else if (stockNoType == 8)
                {
                    movetype = new string[] { "311", "512", "542" };
                }

                //根据movetype优先级来过账
                foreach (string mt in movetype)
                {
                    //查询r_sap_material_shipping数据库
                    DataTable dt = new DataTable();
                    try
                    {
                        dt = DBFunc.SearchRSapMaterialShippingByMoveType(woid, mt, "IN");
                    }
                    catch(Exception ex)
                    {
                        ShowHint("SearchRSapMaterialShippingByMoveType:" + ex.Message, Color.Red);
                        return;
                    }

                    if (dt==null || dt.Rows.Count==0)
                    {
                        continue;
                    }

                    List <Dictionary<string, object>> listSapUploadItem = new List<Dictionary<string, object>>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> dd = new Dictionary<string, object>
                        {
                            {"SHIPPING_NO", woid},
                            {"material_no", dr["material_no"].ToString()},
                            {"QTY", dr["SHIP_QTY"].ToString()},
                            {"PLANT", dr["PLANT"].ToString()},
                            {"STORE_LOC", dr["STORE_LOC"].ToString()},
                            {"MOVE_TYPE", mt},
                            {"MOVE_PLANT", dr["MOVE_PLANT"].ToString()},
                            {"MOVE_STLOC", dr["MOVE_STLOC"].ToString()}
                        };

                        listSapUploadItem.Add(dd);
                    }

                    try
                    {
                        //调PI018接口
                        if (SapHelper.GetPI018(listSapUploadItem, mt, stockNoType, ref remark, ref tranno))
                        {
                            result += "移动类型：" + mt + "(New), " + remark + "\r\n";
                            //数据库回写r_sap_material_shipping 
                            DBFunc.UpateSapMaterialShipping(dt, remark, tranno, "1", "Y");
                        }
                        else
                        {
                            result += "移动类型：" + mt + "(New), " + remark + "\r\n";
                            //数据库回写r_sap_material_shipping 
                            DBFunc.UpateSapMaterialShipping(dt, remark, tranno, "0", "N");
                            ShowHint(result, Color.Red);
                            return;
                        }
                    }
                    catch(Exception ex)
                    {
                        ShowHint("调用PI018接口异常" + ex.Message, Color.Red);
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(result))
                {
                    ShowHint(result, Color.Lime);
                }
                else
                {
                    ShowHint("本次无数据需要过账！", Color.Red);
                }                
            }
            catch (Exception ex)
            {
                ShowHint("SAP过账：" + ex.Message, Color.Red);
                return;
            }
        }

        private void ShowHolder()
        {
            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {
                    label_holderId.BackColor = Color.Lime;
                    label_LocId.BackColor = Color.Transparent;
                }));
            }
        }

        private void textBox_sapno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_sapno.Text.Trim()))
            {
                ShowHint("请先输入单号", Color.Red);
                return;
            }

            SapReplenish(textBox_sapno.Text.Trim());
        }

        private void ShowShelfCount(int cnt)
        {
            if (!runFlag) return;

            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {
                    label_ShelfCnt.Text = cnt.ToString();
                }));
            }
        }

        private void textBox_BarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_BarCode.Text.Trim()))
            {
                return;
            }

            eventExecuteInput.Set();
        }

        private void ReplenishFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.F4)
            {

            }
            else if (e.KeyCode == Keys.F5)
            {
                textBox_BarCode.Text = "F5";
                eventExecuteInput.Set();
            }

            else if (e.KeyCode == Keys.F9)
            {
                StartFindShelf();
            }
            else if (e.KeyCode == Keys.F10)
            {
                StopFindShelf();
            }
        }

        private void Print(string str)
        {
            richTextBox1.AppendText(str + "\r\n");
        }

        private void textBox_trsn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_trsn.Text))
            {
                ShowHint("请先输入单号", Color.Red);
                return;
            }

            DataTable dt = new DataTable();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                dic.Add("stock_no", textBox_trsn.Text);
                dic.Add("stockno_type", 8);

                SearchRInventoryId sr = new SearchRInventoryId(dic);
                sr.ExecuteQuery();
                dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("在R_Inventory_Id中查询不到该工单或者该工单类型为7，无数据需要删除", Color.Lime);
                    return;
                }
            }
            catch(Exception ex)
            {
                ShowHint("SearchRInventoryId:" + ex.Message, Color.Red);
                return;
            }

            try
            {
                dic.Clear();   
                dic.Add("stock_no", textBox_trsn.Text);
                //dic.Add("stock_out_no", "");
                dic.Add("status", "1");

                SearchRInventoryDetail sd = new SearchRInventoryDetail(dic);
                sd.ExecuteQuery();
                dt = sd.GetResult();

                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("该工单无数据需要推送", Color.Lime);
                    return;
                }
            }
            catch(Exception ex)
            {
                ShowHint("SearchRInventoryDetail:" + ex.Message, Color.Red);
                return;
            }
            

            Print("共需要推送：" + dt.Rows.Count + " 条数据!");
            foreach(DataRow dr in dt.Rows)
            {
                if (!string.IsNullOrEmpty(dr["tr_sn"].ToString()))
                {
                    try
                    {
                        if (Phicomm_WMS.OUTIO.tR_Tr_Sn.DelTrSn(dr["tr_sn"].ToString()))
                        {
                            Print("删除TrSn=" + dr["tr_sn"].ToString() + " 的线边仓信息成功!");
                        }
                    }
                    catch(Exception ex)
                    {
                        Print("TrSn=" + dr["tr_sn"].ToString() + ", " + ex.Message);
                    }
                }               
            }

            Print("删除完毕");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                ShowHint("请先输入单号", Color.Red);
                return;
            }

            string str = textBox1.Text;
            if (str.Contains("&"))
            {
                str = str.Split('&')[0];
            }
            if (Phicomm_WMS.OUTIO.tR_Tr_Sn.DelTrSn(str))
            {
                Print("删除TrSn=" + str + " 的线边仓信息成功!");
                textBox1.Text = "";
                textBox1.Focus();
            }
            else
            {
                Print("删除TrSn=" + str + " 的线边仓信息失败!");
                ShowHint("删除TrSn=" + str + " 的线边仓信息失败!", Color.Red);
            }
        }

        private void textBox_human_stockno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_human_stockno.Text.Trim()))
            {
                ShowHint("请输入单号!", Color.Red);
                return;
            }

            DataTable dt = new DataTable();
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("stock_no", textBox_human_stockno.Text.Trim());

                SearchRHolderLocidRelationShipManual sr = new SearchRHolderLocidRelationShipManual(dic);
                sr.ExecuteQuery();
                dt = sr.GetResult();

                if (dt==null || dt.Rows.Count==0)
                {
                    ShowHint("查询不到该工单的手动入库报表", Color.Red);
                    return;
                }                
            }
            catch(Exception ex)
            {
                ShowHint("SearchRHolderLocidRelationShip： " + ex.Message, Color.Red);
                return;
            }

            try
            {
                dataGridView_replenish.Rows.Clear();
                for(int i=0; i<dt.Rows.Count; i++)
                {
                    dataGridView_replenish.Rows.Add(
                                                    false,
                                                    i+1,
                                                    textBox_human_stockno.Text.Trim(),
                                                    dt.Rows[i]["material_no"].ToString(),
                                                    dt.Rows[i]["holder_id"].ToString(),
                                                    dt.Rows[i]["loc_id"].ToString(),
                                                    dt.Rows[i]["on_qty"].ToString(),
                                                    dt.Rows[i]["max_qty"].ToString(),
                                                    dt.Rows[i]["date_code"].ToString()
                                                    );
                }
                dataGridView_replenish.ClearSelection();
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return;
            }
        }

        private void button_selectall_Click(object sender, EventArgs e)
        {
            if (dataGridView_replenish.Rows.Count==0)
            {
                ShowHint("表格中没有数据，操作无效！", Color.Red);
                return;
            }

            string _selectValue = dataGridView_replenish.Rows[0].Cells["Column8"].EditedFormattedValue.ToString();
            bool res = !_selectValue.Equals("True");

            for (int i = 0; i < dataGridView_replenish.Rows.Count; i++)
            {
                ((DataGridViewCheckBoxCell)dataGridView_replenish.Rows[i].Cells["Column8"]).Value = res;
            }
            if (res)
            {
                button_selectall.Text = "全不选[F1]";
            }
            else
            {
                button_selectall.Text = "全选[F1]";
            }

        }

        private void button_replenish_Click(object sender, EventArgs e)
        {
            if (dataGridView_replenish.Rows.Count==0)
            {
                ShowHint("表格中没有数据需要入库，操作无效！", Color.Red);
                return;
            }

            //调用方法一个一个入库
            textBox_human_stockno.Enabled = false;
            button_replenish.Enabled = false;
            button_selectall.Enabled = false;

            richTextBox2.Clear();
            richTextBox2.BackColor = Color.White;
            richTextBox2.AppendText("依次入库\r\n");
            ReplenishAll();

            textBox_human_stockno.Enabled = true;
            button_replenish.Enabled = true;
            button_selectall.Enabled = true;
        }

        private void ReplenishAll()
        {
            bool res = true;
            int result = -99;
            for(int i=0; i<dataGridView_replenish.Rows.Count; i++)
            {
                try
                {
                    if (!dataGridView_replenish.Rows[i].Cells["Column8"].EditedFormattedValue.ToString().Equals("True"))
                    {
                        continue;
                    }

                    richTextBox2.AppendText("周转箱： " + dataGridView_replenish[4, i].Value.ToString() + 
                                            ", 储位：" + dataGridView_replenish[5, i].Value.ToString() + "\r\n");
                    if (!CompleteReplenishMaterial2(MyData.GetStationId(),
                                      dataGridView_replenish[2, i].Value.ToString(), //工单
                                      int.Parse(dataGridView_replenish[4, i].Value.ToString()), //周转箱
                                      dataGridView_replenish[5, i].Value.ToString(), ref result)) //储位
                    {
                        ((DataGridViewCheckBoxCell)dataGridView_replenish.Rows[i].Cells["Column8"]).Value = false;
                        dataGridView_replenish.Rows[i].Selected = true;
                        dataGridView_replenish.FirstDisplayedScrollingRowIndex = i;
                        richTextBox2.AppendText("失败\r\n");
                        richTextBox2.BackColor = Color.Red;

                        res = false;
                        return;
                    }
                    richTextBox2.AppendText("完成入库!\r\n");
                }        
                catch (Exception ex)
                {
                    ((DataGridViewCheckBoxCell)dataGridView_replenish.Rows[i].Cells["Column8"]).Value = false;
                    Trace.WriteLine(ex.Message);
                    res = false;
                }
            }

            if (res && result==1)
            {
                richTextBox2.AppendText("过账中...\r\n");
                //SapReplenish(textBox_human_stockno.Text.Trim());
                ShowHint("工单入库过账功能暂时关闭!", Color.Red);
            }

            if (res)
            {
                richTextBox2.AppendText("完成\r\n");
                richTextBox2.BackColor = Color.Lime;
            }
        }

        private bool CompleteReplenishMaterial2(int stationId, string stockno, int holderId, string locId, ref int result)
        {
            try
            {
                result = DBPCaller.CompleteReplenishMaterial2(stationId, locId, holderId, stockno, 1);
                Trace.WriteLine("Debug: CompleteReplenishMaterial return " + result);
                if (result == 0 || result == 1 || result == 2)
                {
                    return true;
                }
                else if (result == -2)
                {
                    ShowHint("周转箱编号错误，请扫描正确的周转箱！", Color.Red);
                }
                else if (result == -1)
                {
                    ShowHint("入库失败，当前站点不存在对应任务！请将周转箱取下！", Color.Red);
                }
                else
                {
                    ShowHint("CompleteReplenishMaterial2返回" + result, Color.Red);
                }

                return false;
            }
            catch(Exception ex)
            {
                ShowHint("ReplenishMaterial : " + ex.Message, Color.Red);
                return false;
            }
        }

        private void button_export_Click(object sender, EventArgs e)
        {
            if (dataGridView_replenish.Rows.Count==0)
            {
                ShowHint("无数据需要导出！", Color.Red);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "xls文件(*.xls)|*.xls|xlsx文件(*.xlsx)|*.xls|所有文件(*.*)|*.*"; ;
            if (sfd.ShowDialog()==DialogResult.Cancel)
            {
                return;
            }

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("序号", typeof(string));
            dt.Columns.Add("工单", typeof(string));
            dt.Columns.Add("料号", typeof(string));
            dt.Columns.Add("周转箱", typeof(string));
            dt.Columns.Add("储位", typeof(string));
            dt.Columns.Add("已入数量", typeof(string));
            dt.Columns.Add("总量", typeof(string));
            dt.Columns.Add("周期", typeof(string));

            try
            {   
                for (int i = 0; i < dataGridView_replenish.Rows.Count; i++)
                {
                    dt.Rows.Add(dataGridView_replenish[1, i].Value.ToString(),
                                dataGridView_replenish[2, i].Value.ToString(),
                                dataGridView_replenish[3, i].Value.ToString(),
                                dataGridView_replenish[4, i].Value.ToString(),
                                dataGridView_replenish[5, i].Value.ToString(),
                                dataGridView_replenish[6, i].Value.ToString(),
                                dataGridView_replenish[7, i].Value.ToString(),
                                dataGridView_replenish[8, i].Value.ToString()
                                );
                }
                ds.Tables.Add(dt);
            }
            catch(Exception ex)
            {
                ShowHint("将数据转换成DataSet异常：" + ex.Message, Color.Red);
                return;
            }

            try
            {
                ExcelHelper.DataSetToExcel(ds, sfd.FileName);
            }
            catch(Exception ex)
            {
                ShowHint("保存到文件异常：" + ex.Message, Color.Red);
                return;
            }

            ShowHint("已成功导出到: " + sfd.FileName + " 中!", Color.Lime);
        }
    }
}
