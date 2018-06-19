using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Phicomm_WMS.OUTIO;
using Phicomm_WMS.DB;
using System.Threading;
using log4net;
using System.Diagnostics;
using WHC.OrderWater.Commons;

namespace WMS_Client.UI
{
    public partial class PickFrm : Office2007Form
    {
        private readonly ILog Log = LogManager.GetLogger("PickFrm");
        private Dictionary<int, Dictionary<Label,Color>> _dicShelfId = new Dictionary<int, Dictionary<Label,Color>>();

        private bool runFlag = true;  //该窗体是否存在标志，窗体未关闭前，一直为true
        private bool findFlag = false; //查找货架是否到达标志，未到达时一直搜索，到时时停止搜索
        //private bool blingFlag = false; //闪烁标志
        private AutoResetEvent eventStartFind = new AutoResetEvent(false);  //需要查找货架是否到达信号量
        private AutoResetEvent eventExecuteInput = new AutoResetEvent(false); //需要解析输入的信号量
        //private AutoResetEvent eventBlingStart = new AutoResetEvent(false); //闪烁信号量
        //private int blingType = 0;  //闪烁类型，0=不闪烁，1=闪烁储位编号，2=闪烁KPNO，3=闪烁数量

        private string _lastScanLocId = "";  //记录上一次的储位编号
        private int _allOutCnt = 0;  //整盘出库时计数
        private DataTable _dtShelf = new DataTable(); //货架信息
        private int _taskType = 0;
        private enum PickTaskType { Normal=0, Split=1, Human=2};

        public PickFrm()
        {
            InitializeComponent();
        }

        private void PickFrm_Load(object sender, EventArgs e)
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

            Thread t6 = new Thread(PushTrSnThread);
            t6.Start();

            try
            {
                DBPCaller.InitPickProcess(MyData.GetUser(), MyData.GetStationId());
            }
            catch (Exception ex)
            {
                ShowHint("DeinitProcess:" + ex.Message, Color.Red);
            }
        }

        private void InitUI()
        {
            this.TitleText = "欢迎使用, " + MyData.GetUser() + " !";
            label_stationid.Text = "拣选站点 " + MyData.GetStationId().ToString();
            label_KpNo.Text = "";
            label_StockNo.Text = "";
            label_Qty.Text = "";
            label_LocId.Text = "";
            label_cnt.Text = "";

            comboBox_WoidType.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            comboBox_TaskType.SelectedIndex = 0;

            //货架信息
            _dicShelfId.Clear();
            tableLayoutPanel_ShelfId.Controls.Clear();
            for (int i=0; i<20; i++)
            {
                int index = (5 - i / 4) * 10 + (i % 4 + 1);

                Label lb = new Label();
                lb.Name = "Label_ShelId" + i.ToString();
                FontFamily myFontFamily = new FontFamily("幼圆"); //采用哪种字体
                lb.Font = new Font(myFontFamily, 30, FontStyle.Regular);
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
                //Trace.WriteLine("Debug: index=" + index.ToString());        
            }
        }

        private void textBox_StockNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_StockNo.Text))
            {
                MessageBox.Show("单号不能为空!");
                return;
            }

            if (comboBox_WoidType.SelectedIndex<1)
            {
                MessageBox.Show("请先选择一个有效的出库类型");
                return;
            }

            label_stationid.Text = "拣选站点 " + MyData.GetStationId().ToString() + ", " + textBox_StockNo.Text;

            Thread t1 = new Thread(new ParameterizedThreadStart(DownloadThread));
            t1.Start(comboBox_WoidType.SelectedIndex);
        }

        private void EnableUI(bool en)
        {
            if (!runFlag) return;

            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {
                    comboBox_WoidType.Enabled = en;
                    textBox_StockNo.Enabled = en;
                    button_CreatePickTask.Enabled = en;
                    button_CreatePickTask.Focus();           
                }));
            }
        }

        private void ShowData(DataTable dt)
        {
            try
            {
                if (!runFlag) return;

                if (dt == null || dt.Rows.Count == 0)
                {
                    return;
                }

                if (this.IsHandleCreated)
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        dataGridView1.Rows.Clear();                        
                        foreach (DataRow dr in dt.Rows)
                        {
                            dataGridView1.Rows.Add(
                                    dr["WOID"].ToString(),
                                    dr["KP_NO"].ToString(),
                                    dr["QTY"].ToString(),
                                    dr["Send_QTY"].ToString(),
                                    dr["FromFactory"].ToString(),
                                    dr["FromLoc"].ToString(),
                                    dr["MoveType"].ToString(),
                                    dr["ToFactory"].ToString(),
                                    dr["ToLoc"].ToString(),
                                    "-",
                                    dr["SubType"].ToString(),
                                    dr["KPDESC"].ToString()
                                    );
                        }
                    }));
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private bool RefreshData(string str)
        {
            if (!runFlag) return false;

            bool result = false;
            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {
                    int row = dataGridView1.Rows.Count;
                    int col = dataGridView1.Columns.Count;
                    for (int i=0; i<row; i++)
                    {
                        if (dataGridView1[1, i].Value==null)
                        {
                            return;
                        }
                        if (str.Contains(dataGridView1[1, i].Value.ToString().Trim()))
                        {
                            dataGridView1[9, i].Value = "缺料";                           
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }));
            }

            return result;            
        }

 
        private void CreateTask()
        {
            try
            {                
                EnableUI(false);

                //检查上一次该站点是否有遗留任务未完成
                if (!CheckStationReady())
                {
                    return;
                }
                
                if (!CreatePickTask(textBox_StockNo.Text.Trim(), MyData.GetStockNoType(), _taskType))
                {
                    return;
                }                      

                if (_taskType==0)
                {
                    UpdateShelfCount(2, MyData.GetStationId());
                }
                else if (_taskType==1)
                {
                    UpdateShelfCount(3, MyData.GetStationId()); //手动出库
                    //刷新手动出库界面
                    GetHumanPickData(MyData.GetStockNo(), MyData.GetStationId());
                    ShowHint("请至手动出库页面查看明细!", Color.Lime);
                }
            }
            catch (Exception ex)
            {
                Log.Error("CreateTask:" + ex.Message);
                ShowHint("CreateTask:" + ex.Message, Color.Red);
            }
            finally
            {
                EnableUI(true);
            }     
        }

        private bool GetHumanPickData(string woid, int stationId)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //dic.Add("StationId", stationId);
                dic.Add("StockOutNo", woid);

                SearchMaterialManualPick sm = new SearchMaterialManualPick(dic);
                sm.ExecuteQuery();

                DataTable dt = sm.GetResult();
                if (dt==null || dt.Rows.Count==0)
                {
                    ShowHint("获取不到手动出库数据，请检查", Color.Red);
                    return false;
                }

                int cnt = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    //刷新数据
                    if (IsHandleCreated)
                    {
                        Invoke(new EventHandler(delegate
                        {
                            dataGridView_HumanPick.Rows.Add(cnt.ToString(),
                                                            dr["StockOutNo"].ToString(),
                                                            dr["MaterialId"].ToString(),
                                                            dr["BoxBarcode"].ToString(),
                                                            dr["Qty"].ToString(),
                                                            dr["FifoDC"].ToString()
                                                            );
                            cnt++;
                        }));
                    }
                }            

                return true;
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return false;
            }
        }

        private bool UpdateHumanPickData(string woid, string materialno, string locid)
        {
            try
            {
                int i = 0;
                for(i=0; i< dataGridView_HumanPick.Rows.Count; i++)
                {
                    if (dataGridView_HumanPick[2, i].Value.ToString().Trim().Equals(materialno) && 
                        dataGridView_HumanPick[3, i].Value.ToString().Trim().Equals(locid))
                    {
                        break;
                    }
                }
                if (i == dataGridView_HumanPick.Rows.Count)
                {
                    ShowHint("当前物料储位为：" + locid + ", 料号为：" + materialno + ", 请确认是否取错储位！", Color.Red);
                    return true;
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("StockOutNo", woid);
                dic.Add("MaterialId", materialno);
                dic.Add("BoxBarcode", locid);
                SearchMaterialManualPick sm = new SearchMaterialManualPick(dic);
                sm.ExecuteQuery();

                DataTable dt = sm.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    //刷新数据                
                    dataGridView_HumanPick[4, i].Value = "0";
                }
                else
                {
                    dataGridView_HumanPick[4, i].Value = dt.Rows[0]["Qty"].ToString();
                }

                dataGridView_HumanPick.ClearSelection();
                return true;
            }
            catch (Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return false;
            }
        }

        public void ShowHint(string msg, Color cl)
        {
            using (MsgFrm mf = new MsgFrm(msg, cl))
            {
                mf.BringToFront();
                mf.ShowDialog();
                mf.Dispose();
            }
        }

        public bool CheckStationReady()
        {
            try
            {
                int result = DBPCaller.CheckStationWorkState(MyData.GetStationId());
                if (result == 0) //0: 检查无误，可以进行后续操作
                {
                    return true;
                }

                if (result == -2)
                {
                    ShowHint("当前站点不存在，先检查数据库配置, " + MyData.GetStationId(), Color.Red);
                    return false;
                }

                if (result != -1)
                {
                    ShowHint("未知错误, " + MyData.GetStationId(), Color.Red);
                    return false;
                }

                //剩下是等于1的情况
                string lastStockNo = DBFunc.GetImcompletePickNo(MyData.GetStationId());
                if (string.IsNullOrEmpty(lastStockNo.Trim()) ||
                    lastStockNo.Trim().Equals(textBox_StockNo.Text.Trim()))
                {
                    return true;
                }

                ShowHint("上一个单号：" + lastStockNo + " 未完成, 请继续!", Color.Red);

                return false;
            }
            catch(Exception ex)
            {
                Log.Error("CheckStationReady: " + ex.Message);
                ShowHint("CheckStationReady: " + ex.Message, Color.Red);
                return false;
            }
        }

        public bool CreatePickTask(string woid, int stockNoType, int type)
        {
            try
            {
                if (stockNoType<0 || stockNoType>6)
                {
                    ShowHint("该工单的出库类型错误，请核对", Color.Red);
                    return false;
                }

                string result = string.Empty;

                if (type == (int)PickTaskType.Normal)//不拆盘出库
                {
                    result = DBPCaller.CreatePickTask(woid, stockNoType);
                }
                else if (type==(int)PickTaskType.Split)//拆盘出库
                {
                    result = DBPCaller.CreatePartialPickTask(woid, stockNoType);
                }
                else if (type==(int)PickTaskType.Human)//指定周期
                {                    
                    result = DBPCaller.CreatePickTask2(woid, stockNoType, 1);
                }

                if (result.ToUpper().Trim().Equals("OK"))
                {
                    ShowHint("生成任务成功!", Color.Lime);
                    return true;
                }                

                //以下是缺料信息
                if (RefreshData(result))
                {
                    ShowHint("生成任务成功，但部分物料缺料，详细请见表格!", Color.Lime);
                    return true;
                }
                else
                {
                    ShowHint("生成任务失败，全部物料缺料!", Color.Red);
                    return false;
                }
            }
            catch(Exception ex)
            {
                Log.Error("CreatePickTask：" + ex.Message);
                ShowHint("CreatePickTask: " + ex.Message, Color.Red);
                return false;
            }
        }

        private void textBox_BarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_BarCode.Text.Trim()))
            {
                return;
            }

            eventExecuteInput.Set();
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

        private void UpdateCnt(string woid, string locid)
        {
            if (!runFlag) return;

            try
            {
                List<string> listTrSn_out = DBFunc.SearchTrSnFromRInventoryDetailByStockOutNo(woid, locid);
                List<string> listTrSn_in = DBFunc.SearchTrSnFromRInventoryDetailByLocId(locid);
                int num_out = 0;
                int num_in = 0;

                if (listTrSn_out != null)
                {
                    num_out = listTrSn_out.Count;
                }
                if (listTrSn_in!=null)
                {
                    num_in = listTrSn_in.Count;
                }

                //刷新数据
                if (this.IsHandleCreated)
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        label_cnt.Text = num_out.ToString() + " / " + num_in.ToString();
                    }));
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
                Trace.WriteLine("Debug: " + ex.Message);
                return;
            }            
        }

        private bool AgvLeave(int stationId)
        {
            try
            {
                int result = DBPCaller.AgvReturn(stationId);
                if (result==-3)
                {
                    ShowHint("当前站点还有未完成拣选的出库物料", Color.Red);
                    return false;
                }
                else if (result == -2)
                {
                    ShowHint("当前站点还有未完成拣选的入库物料", Color.Red);
                    return false;
                }
                else if (result==-1)
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
        
        private void RefreshUI(int cnt, int col, DataTable dt)
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
                            HighLight(MyData._lastArrive.select, Color.DarkGray, 0);
                           
                            MyData._lastArrive.select = 0;
                            MyData._lastArrive.stockno = "";
                            MyData._lastArrive.locid = "";
                            MyData._lastArrive.kpno = "";
                            MyData._lastArrive.qty = "";

                            label_ShelfId.Text = "";
                            label_StockNo.Text = MyData._lastArrive.stockno;
                            label_LocId.Text = MyData._lastArrive.locid;
                            label_LocId.BackColor = Color.Transparent;
                            label_KpNo.Text = MyData._lastArrive.kpno;
                            label_Qty.Text = MyData._lastArrive.qty.Equals("0")? "" : MyData._lastArrive.qty.ToString();
                            label_Qty.BackColor = Color.Transparent;

                            Trace.WriteLine("Debug: ------RefreshUI Stop------");
                            
                            return;
                        }

                        //Trace.WriteLine("Debug:Col=" + col.ToString());
                        //需要更新
                        if (MyData._lastArrive.stockno!=null &&
                            MyData._lastArrive.locid!=null &&
                            MyData._lastArrive.kpno!=null &&
                            MyData._lastArrive.qty!=null &&
                            MyData._lastArrive.stockno.Equals(dt.Rows[0]["StockNo"].ToString().Trim()) &&
                            MyData._lastArrive.locid.Equals(dt.Rows[0]["BoxBarcode"].ToString().Trim()) &&
                            MyData._lastArrive.kpno.Equals(dt.Rows[0]["MaterialId"].ToString().Trim()) &&
                            MyData._lastArrive.qty.Equals(dt.Rows[0]["Qty"].ToString().Trim()) &&
                            MyData._lastArrive.col==col &&
                            MyData._lastArrive.cnt==cnt)
                        {
                            findFlag = true;
                            return;
                        }

                        if (MyData._lastArrive.locid == null ||
                            !MyData._lastArrive.locid.Equals(dt.Rows[0]["BoxBarcode"].ToString().Trim()))
                        {
                            label_Hint.Text = "请扫储位编号";
                            HighLight(MyData._lastArrive.select, Color.DarkGray, 0);

                            //label_cnt.Text = "0";
                            //高亮显示
                            label_LocId.BackColor = Color.Lime;
                            label_Qty.BackColor = Color.Transparent;
                            textBox_BarCode.Focus();
                        }
                        else /*if (!MyData._lastArrive.kpno.Equals(dt.Rows[0]["MaterialId"].ToString().Trim()) ||
                                MyData._lastArrive.qty.Equals(dt.Rows[0]["Qty"].ToString().Trim()))*/
                        {
                            label_Hint.Text = "请扫物料唯一条码";
                            HighLight(MyData._lastArrive.select, Color.DarkGray, 0);
                            textBox_BarCode.Focus();
                        }

                        string boxBarCode = dt.Rows[0]["BoxBarcode"].ToString();
                        if (boxBarCode.Length < 3)
                        {
                            Trace.WriteLine("Debug: RefreshUI: BoxBarCode lenght wrong," + boxBarCode);
                            Log.Error("RefreshUI: BoxBarCode lenght wrong");
                            //return;
                        }

                        #region 刷新货架信息
                        if (col == 2)
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
                        MyData._lastArrive.qty = dt.Rows[0]["Qty"].ToString().Trim();
                        MyData._lastArrive.allout = dt.Rows[0]["AllOut"].ToString().Trim();
                        MyData._lastArrive.col = col;
                        MyData._lastArrive.cnt = cnt;

                        label_ShelfId.Text = dt.Rows[0]["PodId"].ToString().Trim() + ",    " + 
                                             (dt.Rows[0]["PodSide"].ToString().Trim().Equals("0")?"正面" : "反面");

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
                        if (!label_Qty.Text.Trim().Equals(MyData._lastArrive.qty.ToString()))
                        {
                            label_Qty.Text = MyData._lastArrive.qty.ToString();
                        }
                        if (MyData._lastArrive.allout.Equals("1"))
                        {
                            HighLight(MyData._lastArrive.select, Color.Yellow, cnt);
                        }
                        else
                        {
                            HighLight(MyData._lastArrive.select, Color.Lime, cnt);
                        }

                        //保存工单号
                        MyData.SetStockNo(MyData._lastArrive.stockno);

                        findFlag = true; 

                        Trace.WriteLine("Debug: ------RefreshUI Stop------");
                    }));
                }
            }
            catch(Exception ex)
            {
                Log.Error("RefreshUI: " + ex.Message);
                Trace.WriteLine("Debug: " + ex.Message);
                Trace.WriteLine("Debug: ------RefreshUI Stop------");
            }
        }

        private void HighLight(int index, Color cl, int cnt)
        {
            if (_dicShelfId.ContainsKey(index))
            {
                Dictionary<Label, Color> dicColor = _dicShelfId[index];
                foreach (KeyValuePair<Label, Color> _dic in dicColor)
                {
                    if (cnt == 0)
                    {
                        _dic.Key.Text = "";
                    }
                    else
                    {
                        //_dic.Key.Text = cnt.ToString();
                    }
                    _dic.Key.BackColor = cl;
                    break;
                }
            }            
        }

        private void PickFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            runFlag = false;
            findFlag = true;
            //blingFlag = false;

            //eventBlingStart.Set();
            eventExecuteInput.Set();
            eventStartFind.Set();


            try
            {
                DBPCaller.DeinitProcess(MyData.GetStationId());
            }
            catch (Exception ex)
            {
                ShowHint("DeinitProcess:" + ex.Message, Color.Red);
            }
        }

        private void textBox_sn1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter)
                {
                    return;
                }

                if (string.IsNullOrEmpty(textBox_sn1.Text.Trim()))
                {
                    Print("推送信息到线边仓的工单不能为空!");
                    textBox_sn1.Text = "";
                    textBox_sn1.Focus();
                    return;
                }

                textBox_sn1.Enabled = false;
                textBox_sn2.Enabled = false;
                PushTrSnByStockNo(textBox_sn1.Text.Trim());
                richTextBox1.ScrollToCaret();
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Debug: " + ex.Message);
                ShowHint(ex.Message, Color.Red);
            }
            finally
            {
                textBox_sn1.Enabled = true;
                textBox_sn2.Enabled = true;
            }
        }

        private void textBox_sn2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter)
                {
                    return;
                }

                if (string.IsNullOrEmpty(textBox_sn2.Text.Trim()))
                {
                    Print("推送信息到线边仓的物料唯一条码不能为空!");
                    return;
                }

                if (string.IsNullOrEmpty(textBox_StockNo.Text.Trim()))
                {
                    Print("请至下载页面输入一个单号!");
                    return;
                }

                if (comboBox_WoidType.SelectedIndex < 1)
                {
                    Print("请至下载页面选择一个有效的出库类型!");
                    return;
                }

                if (!DBFunc.CheckTrSnHasStockOut(textBox_sn2.Text.Trim(), textBox_StockNo.Text.Trim()))
                {
                    Print("该TrSn出库还未完成，或者出库单号错误，请检查!");
                    return;
                }

                if (InsertTrSn(textBox_StockNo.Text.Trim(), comboBox_WoidType.SelectedIndex,  textBox_sn2.Text.Trim().ToUpper()))
                {
                    Print("推送TrSn=" + textBox_sn2.Text.Trim() + " 的信息到线边仓成功！");                    
                }
                else
                {
                    Print("推送TrSn=" + textBox_sn2.Text.Trim() + " 的信息到线边仓失败！");
                    ShowHint("推送: " + textBox_sn2.Text.Trim() + " 的信息到线边仓失败！", Color.Red);
                }
                richTextBox1.ScrollToCaret();
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Debug: " + ex.Message);
                ShowHint(ex.Message, Color.Red);
            }
        }

        private void textBox_sapno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_sapno.Text.Trim()))
            {
                Print("过账的工单不能为空!");
                textBox_sapno.Text = "";
                textBox_sapno.Focus();
                return;
            }

            if (comboBox1.SelectedIndex<1)
            {
                Print("请选择出库类型");
                return;
            }

            //过账
            //推送信息
            if (MyData.GetStockNoType() == (int)MyData.PickWoType.Normal ||
                MyData.GetStockNoType() == (int)MyData.PickWoType.Super)
            {
                PushTrSnByStockNo(MyData.GetStockNo());
            }
            SapPick(textBox_sapno.Text.Trim(), comboBox1.SelectedIndex);
            richTextBox1.ScrollToCaret();
        }

        private bool SapPick(string woid, int stockNoType)
        {
            try
            {
                if (stockNoType<0 || stockNoType>6)
                {
                    ShowHint("出库类型不匹配，请检查", Color.Red);
                    return false;
                }

                //if (stockNoType==(int)MyData.PickWoType.Normal)
                //{
                //    ShowHint("工单发料过账功能暂时关闭", Color.Red);
                //    return false;
                //}

                //先判断要过账的工单是否存在
                try
                {
                    if (!DBFunc.CheckWoIdInvalid(woid))
                    {
                        Trace.WriteLine("Debug: 在r_sap_material_shipping表中找不到该工单号，请确认单号");
                        ShowHint("在r_sap_material_shipping表中找不到该工单号，请确认单号", Color.Red);
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    Trace.WriteLine("Debug: 在r_sap_material_shipping表中检查工单是否存在时异常，工单：" + woid + ", 异常" + ex.Message);
                    ShowHint("在r_sap_material_shipping表中检查工单是否存在时异常，工单：" + woid + ", 异常" + ex.Message, Color.Red);
                    return false;
                }

                string aim_plant = "";
                string aim_stoloc = "";
                string result = "";
                bool res = false;

                try
                {
                    if (!DBFunc.UpdateSapLocation(woid, stockNoType, ref aim_plant, ref aim_stoloc, ref result))
                    {
                        ShowHint("错误：" + result, Color.Red);
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    Trace.WriteLine("Debug: 更新该工单的目标工厂目标库位到r_sap_material_shipping时异常：" + ex.Message);
                    ShowHint("更新该工单的目标工厂目标库位到r_sap_material_shipping时异常：" + ex.Message, Color.Red);
                    return false;
                }
                
                string remark = string.Empty; //用于显示过账凭证
                if (stockNoType == (int)MyData.PickWoType.Normal)
                {
                    if (SapPickNormal(woid, "541,511,311,261", ref remark))
                    {
                        res = true;
                    }
                }
                else if (stockNoType == (int)MyData.PickWoType.Delivery)
                {
                    if (SapPickNormal(woid, "645", ref remark))
                    {
                        res = UpdateDeliveryRequsitionDetailStatus(woid);
                    }
                }
                else if (stockNoType == (int)MyData.PickWoType.Reserve)
                {
                    if (SapPickNormal(woid, "201", ref remark))
                    {
                        res = UpdateReserveRequisitionDetailStatus(woid);
                    }
                }
                else if (stockNoType == (int)MyData.PickWoType.OutSource)
                {
                    if (SapPickNormal(woid, "541,511", ref remark))
                    {
                        res = UpdateOutSourceRequisitionDetailStatus(woid);
                    }
                }
                else if (stockNoType == (int)MyData.PickWoType.Transfer)
                {                    
                    if (SapPickNormal(woid, "311", ref remark))
                    {
                        res = UpateTransferRequsitionDetailStatus(woid);
                    }
                }
                else if (stockNoType == (int)MyData.PickWoType.Super)
                {
                    if (SapPickNormal(woid, "541,511,311", ref remark))
                    {
                        res = UpdateSuperRequisitionDetail(woid);
                    }
                }

                Print(remark);
                return res;
            }
            catch(Exception ex)
            {
                Print(ex.Message);
                Log.Error(ex.Message);
                Trace.WriteLine("Debug: " + ex.Message);
                ShowHint(ex.Message, Color.Red);
                return false;
            }
        }

        private bool SapPickNormal(string woid, string movetype, ref string result)
        {
            try
            {
                string[] mt1 = movetype.Split(',');

                if (mt1.Length == 0)
                {
                    ShowHint("移动类型参数不正确:" + movetype, Color.Red);
                    return false;
                }

                List<Dictionary<string, object>> listSapUploadItem = new List<Dictionary<string, object>>(); //保存过账信息
                foreach (string mt in mt1)
                {                    
                    //根据单号，移动类型 查询过账信息
                    DataTable dt = null;
                    try
                    {
                        dt = DBFunc.SearchRSapMaterialShippingByMoveType(woid, mt, "OUT");
                        if (dt == null || dt.Rows.Count == 0)
                        {
                            //ShowHint("该工单的移动类型:" + mt + " 查询不到数据，请检查", Color.Red);
                            //return false;
                            continue;
                        }

                        //已过账，保存凭证
                        //if (dt.Rows[0]["UPLOAD_FLAG"].ToString().Equals("Y"))
                        //{
                        //    result += "移动类型" + mt + "(Old): " + dt.Rows[0]["REMARK"].ToString().Trim() + "\r\n";
                        //    continue;
                        //}
                    }
                    catch(Exception ex)
                    {
                        Trace.WriteLine("Debug: " + ex.Message);
                        ShowHint("在r_sap_material_shipping表中查询时异常，woid=" + woid + ", movetype=" + mt + ", " + ex.Message, Color.Red);
                        return false;
                    }


                    listSapUploadItem.Clear();
                    //如果已经有一部分移动类型已经过过账
                    foreach (DataRow dr in dt.Rows)
                    {                         
                        //未过账部分,拼凑数据
                        Dictionary<string, object> uploadItem = new Dictionary<string, object>
                                                                {
                                                                    {"SHIPPING_NO", woid},
                                                                    {"material_no", dr["material_no"].ToString()},
                                                                    {"QTY", dr["SHIP_QTY"].ToString()},
                                                                    {"PLANT", dr["PLANT"].ToString()},
                                                                    {"STORE_LOC", dr["STORE_LOC"].ToString()},
                                                                    {"MOVE_TYPE", mt},
                                                                    {"MOVE_PLANT", dr["MOVE_PLANT"].ToString()},
                                                                    {"MOVE_STLOC", dr["MOVE_STLOC"].ToString()},
                                                                    {"RESERV_NO", ""},
                                                                    {"RES_ITEM", ""},
                                                                };
                        if (mt.Equals("261"))
                        {                            
                            try
                            {
                                DataTable dtReserveNo = DBFunc.SearchReserveNoFromRErpWoBomInfoByKpNo(woid, dr["material_no"].ToString());
                                if (dtReserveNo == null || dtReserveNo.Rows.Count == 0)
                                {
                                    ShowHint("该工单移动类型261过账时找不到rel_requireid, rel_projectid信息", Color.Red);
                                    return false;
                                }

                                uploadItem["RESERV_NO"] = dtReserveNo.Rows[0]["Rel_Requireid"];
                                uploadItem["RES_ITEM"] = dtReserveNo.Rows[0]["Rel_ProjectId"];
                            }
                            catch(Exception ex)
                            {
                                Trace.WriteLine("Debug: " + ex.Message);
                                ShowHint("移动类型261时查询rel_requireid, rel_projectid信息时异常，woid=" + woid + ",kpno=" + dr["material_no"].ToString() + ", ex=" + ex.Message, Color.Red);
                                return false;
                            }
                        }

                        listSapUploadItem.Add(uploadItem);
                    }

                    //过账
                    try
                    {
                        string remark = "";
                        string tranno = "";
                        if (!SapHelper.GetPI018(listSapUploadItem, mt, ref remark, ref tranno))
                        {
                            result += "移动类型" + mt + "(New): " + remark + "r\n";
                            //数据库回写r_sap_material_shipping 
                            DBFunc.UpateSapMaterialShipping(dt, remark, tranno, "0", "N");

                            ShowHint(result, Color.Red);
                            return false;
                        }
                        else
                        {
                            result += "移动类型" + mt + "(New): " + remark + "\r\n";

                            //数据库回写r_sap_material_shipping 
                            DBFunc.UpateSapMaterialShipping(dt, remark, tranno, "1", "Y");
                        }
                    }
                    catch(Exception ex)
                    {
                        Trace.WriteLine("Debug: " + ex.Message);
                        ShowHint("调用PI018接口异常：" +ex.Message, Color.Red);
                        return false;
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
                return true;
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Debug: " + ex.Message);
                Log.Error(ex.Message);
                Print(ex.Message);
                ShowHint(ex.Message, Color.Red);
                return false;
            }
        }

        private void Print(string str)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {
                    richTextBox1.AppendText(str + "\r\n");
                    //richTextBox1.ScrollToCaret();
                }));
            }
        }

        private void PickFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.F5)
            {
                textBox_BarCode.Text = "F5";
                eventExecuteInput.Set();                              
            }
            else if (e.KeyCode==Keys.F6)
            {
                dataGridView_HumanPick.Rows.Clear();
                GetHumanPickData(MyData.GetStockNo(), MyData.GetStationId());
                dataGridView_HumanPick.ClearSelection();
                textBox_HumanInput.Text = "";
                textBox_HumanInput.Focus();
            }
            else if (e.KeyCode==Keys.F9)
            {
                StartFindShelf();
            }
            else if (e.KeyCode==Keys.F10)
            {
                StopFindShelf();
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

        //private void SetBlingType(int type)
        //{
        //    blingType = type;
        //    blingFlag = true;

        //    eventBlingStart.Set();
        //}
        private void ShowQty()
        {
            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {
                    label_Qty.BackColor = Color.Lime;
                    label_LocId.BackColor = Color.Transparent;
                }));
            }
        }

        #region 背景闪烁部分代码，暂时不用，只显示背景色即可
        //private void BlingOn()
        //{
        //    if (!runFlag) return;

        //    if (this.IsHandleCreated)
        //    {
        //        this.Invoke(new EventHandler(delegate
        //        {
        //            if (blingType == 1)
        //            {
        //                //label_LocId.Text = "1000028121";
        //                label_LocId.BackColor = Color.Lime;
        //                label_KpNo.BackColor = Color.Transparent;
        //                label_Qty.BackColor = Color.Transparent;
        //            }
        //            else if (blingType == 2)
        //            {
        //                label_LocId.BackColor = Color.Transparent;
        //                label_KpNo.BackColor = Color.Lime;
        //                label_Qty.BackColor = Color.Transparent;
        //            }
        //            else if (blingType == 3)
        //            {
        //                label_LocId.BackColor = Color.Transparent;
        //                label_KpNo.BackColor = Color.Transparent;
        //                label_Qty.BackColor = Color.Lime;
        //            }
        //        }));
        //    }
        //}

        //private void BlingOff()
        //{
        //    if (!runFlag) return;

        //    if (this.IsHandleCreated)
        //    {
        //        this.Invoke(new EventHandler(delegate
        //        {                    
        //            label_LocId.BackColor = Color.Transparent;
        //            label_KpNo.BackColor = Color.Transparent;
        //            label_Qty.BackColor = Color.Transparent;
        //        }));
        //    }
        //}

        //private void BlingThread()
        //{
        //    int delay = 500;

        //    while(runFlag)
        //    {
        //        eventBlingStart.WaitOne();
        //        if (!runFlag) return;

        //        while(blingFlag)
        //        {
        //            BlingOn();
        //            if (!blingFlag) break;
        //            if (!runFlag) return;
        //            Thread.Sleep(delay);

        //            BlingOff();
        //            if (!blingFlag) break;
        //            if (!runFlag) return;
        //            Thread.Sleep(delay);
        //        }
        //    }
        //}
        #endregion

        private void KeepAliveThread()
        {
            int i = 0;
            while (runFlag)
            {
                try
                {
                    DBPCaller.KeepAlive(MyData.GetStationId());
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Debug: KeepAlive," + ex.Message);
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
        }

        private void PushTrSnThread()
        {
            while(runFlag)
            {
                if (string.IsNullOrEmpty(MyData.GetStockNo()))
                {
                    Thread.Sleep(500);
                    continue;
                }

                if (MyData.GetStockNoType()==0)
                {
                    Thread.Sleep(500);
                    continue;
                }

                PushTrSnByStockNo(MyData.GetStockNo());

                //延时3S
                Thread.Sleep(500);
                if (!runFlag) return;
                Thread.Sleep(500);
                if (!runFlag) return;
                Thread.Sleep(500);
                if (!runFlag) return;
                Thread.Sleep(500);
                if (!runFlag) return;
                Thread.Sleep(500);
                if (!runFlag) return;
                Thread.Sleep(500);
                if (!runFlag) return;
            }
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

        private void button_select_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "xls文件(*.xls)|*.xls|xlsx文件(*.xlsx)|*.xls|所有文件(*.*)|*.*";
            if (ofd.ShowDialog()==DialogResult.Cancel)
            {
                return;
            }

            textBox_filepath.Text = ofd.FileName;

            ShowData(textBox_filepath.Text);
        }

        private void ShowData(string filepath)
        {
            try
            {
                dataGridView_import.Rows.Clear();

                DataSet ds = ExcelHelper.ExcelToDataSet(filepath, "Sheet1$", true, ExcelHelper.ExcelType.Excel2003);
                if (ds==null || ds.Tables.Count==0)
                {
                    ShowHint("该文件中无数据", Color.Red);
                    return;
                }

                DataTable dt = ds.Tables[0];
                if (dt==null || dt.Rows.Count==0)
                {
                    ShowHint("该文件中无数据", Color.Red);
                    return;
                }

                int cnt = 1;
                foreach(DataRow dr in dt.Rows)
                {
                    if (string.IsNullOrEmpty(dr[0].ToString().Trim()))
                    {
                        return;
                    }
                    dataGridView_import.Rows.Add(cnt,
                                                dr[0].ToString(),
                                                dr[1].ToString(),
                                                dr[2].ToString(),
                                                dr[3].ToString(),
                                                dr[4].ToString(),
                                                dr[5].ToString(),
                                                dr[6].ToString(),
                                                dr[7].ToString(),
                                                dr[8].ToString(),
                                                dr[9].ToString(),
                                                dr[10].ToString(),
                                                dr[11].ToString(),
                                                dr[12].ToString(),
                                                dr[13].ToString()
                                                );
                    cnt++;
                }

                dataGridView_import.ClearSelection();
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Debug: ShowData," + ex.Message);
                ShowHint(ex.Message, Color.Red);
            }
        }

        private void button_import_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView_import.Rows.Count==0)
                {
                    return;
                }

                string woid = dataGridView_import[1, 0].Value.ToString();

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("woid", woid);

                SearchErpWoBomInfo se = new SearchErpWoBomInfo(dic);
                se.ExecuteQuery();
                DataTable dt = se.GetResult();
                if (dt!=null && dt.Rows.Count>0)
                {//delete from r_erp_wo_bom_info where woid='1234567890'
                    ShowHint("该工单已经导入过，不支持重复导入", Color.Red);
                    return;
                }

                //重新读取数据
                DataSet ds = ExcelHelper.ExcelToDataSet(textBox_filepath.Text, "Sheet1$", true, ExcelHelper.ExcelType.Excel2003);
                if (ds == null || ds.Tables.Count == 0)
                {
                    ShowHint("该文件中无数据", Color.Red);
                    return;
                }

                dt = ds.Tables[0];
                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("该文件中无数据", Color.Red);
                    return;
                }

                int cnt = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (string.IsNullOrEmpty(dr[0].ToString()))
                    {
                        continue;
                    }

                    cnt++;                  
                    try
                    {                        
                        InsertErpWoBomInfo ie = new InsertErpWoBomInfo(dr);
                        ie.ExecuteUpdate();
                    }
                    catch (Exception ex)
                    {
                        ShowHint("导入失败：" + ex.Message, Color.Red);
                        return;
                    }
                }

                ShowHint("导入成功， 共导入 " + cnt.ToString() + " 条数据", Color.Lime);               
            }
            catch(Exception ex)
            {
                ShowHint("导入失败：" + ex.Message, Color.Red);
            }
        }

        private void button_export_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView_export.Rows.Count==0)
                {
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "xls文件(*.xls)|*.xls|xlsx文件(*.xlsx)|*.xls|所有文件(*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                if (string.IsNullOrEmpty(textBox_woidexport.Text.Trim()))
                {
                    ShowHint("请输入工单!", Color.Red);
                    return;
                }

                DataTable dt = new DataTable();
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("SHIPPING_NO", textBox_woidexport.Text.Trim());
                    dic.Add("DEB_CRED", "OUT");

                    SearchSapMaterialShipping sm = new SearchSapMaterialShipping(dic);
                    sm.ExecuteQuery();
                    dt = sm.GetResult();
                }
                catch (Exception ex)
                {
                    ShowHint("查询r_sap_material_shipping表格异常" + ex.Message, Color.Red);
                    return;
                }

                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("查询不到该工单信息，请确认单号!", Color.Red);
                    return;
                }

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                try
                {
                    ExcelHelper.DataSetToExcel(ds, sfd.FileName);
                }
                catch(Exception ex)
                {
                    ShowHint("保存数据异常："  +ex.Message, Color.Red);
                    return;
                }

                ShowHint("数据已经成功保存文件： " + sfd.FileName + " 中！", Color.Lime);                
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
            }
        }

        private void textBox_woidexport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_woidexport.Text.Trim()))
            {
                ShowHint("请输入工单!", Color.Red);
                return;
            }

            DataTable dt = new DataTable();
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("SHIPPING_NO", textBox_woidexport.Text.Trim());
                dic.Add("DEB_CRED", "OUT");

                SearchSapMaterialShipping sm = new SearchSapMaterialShipping(dic);
                sm.ExecuteQuery();
                dt = sm.GetResult();
            }
            catch(Exception ex)
            {
                ShowHint("查询r_sap_material_shipping表格异常" + ex.Message, Color.Red);
                return;
            }

            if (dt==null || dt.Rows.Count==0)
            {
                ShowHint("查询不到该工单信息，请确认单号!", Color.Red);
                return;
            }

            dataGridView_export.Rows.Clear();
            int cnt = 0;
            try
            {                
                foreach (DataRow dr in dt.Rows)
                {
                    cnt++;
                    dataGridView_export.Rows.Add(
                            cnt,
                            dr["SHIPPING_NO"].ToString(),
                            dr["material_no"].ToString(),
                            dr["SAP_QTY"].ToString(),
                            dr["SHIP_QTY"].ToString(),
                            dr["PLANT"].ToString(),
                            dr["STORE_LOC"].ToString(),
                            dr["MOVE_TYPE"].ToString(),
                            dr["MOVE_PLANT"].ToString(),
                            dr["MOVE_STLOC"].ToString(),
                            dr["UPLOAD_FLAG"].ToString(),
                            dr["TRANSACTION_NO"].ToString(),
                            dr["REMARK"].ToString()
                        );
                }

                dataGridView_export.ClearSelection();
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
            }
        }

        private void textBox_HumanInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_HumanInput.Text.Trim()))
            {
                ShowHint("条码为空", Color.Red);
                textBox_HumanInput.Text = "";
                textBox_HumanInput.Focus();
                return;
            }

            if (textBox_HumanInput.Text.Trim().ToUpper().Equals("F6"))
            {
                dataGridView_HumanPick.Rows.Clear();
                GetHumanPickData(MyData.GetStockNo(), MyData.GetStationId());
                dataGridView_HumanPick.ClearSelection();
                textBox_HumanInput.Text = "";
                textBox_HumanInput.Focus();
                return;
            }

            try
            {
                string barcode = textBox_HumanInput.Text.Trim().ToUpper();
                string trsn = barcode;
                string materialno = "";
                string locid = "";
                string status = "";

                richTextBox2.AppendText("\r\n条码：" + barcode + "\r\n");
                #region Step1:TrSn物料条码规则检查
                if (barcode.Contains("&"))
                {
                    if (barcode.Split('&').Length!=5)
                    {
                        ShowHint("扫描五合一条码错误!", Color.Red);
                        return;
                    }
                    trsn = barcode.Split('&')[0].Trim();
                }

                if (trsn.Length!=13 && trsn.Length!=18)
                {
                    ShowHint("扫描TrSn长度错误!", Color.Red);
                    return;
                }

                if (!trsn.All(c => ((c <= '9' && c >= '0') || (c <= 'Z' && c >= 'A'))))
                {
                    ShowHint("TrSn中有未知字符，请重新扫描!", Color.Red);
                    return;
                }
                richTextBox2.AppendText("trsn：" + trsn + "\r\n");
                #endregion

                #region Step2:查询该物料的在库状态, 获取料号，储位信息
                //根据工单，出库，料号查询已出库量            
                Dictionary<string, object> dic = new Dictionary<string, object>();

                //通过trsn查询料号                
                dic.Add("tr_sn", trsn);
                SearchRInventoryDetail sr = new SearchRInventoryDetail(dic);
                sr.ExecuteQuery();
                DataTable dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("在r_Inventory_detail表中查询不到该TrSn:" + trsn, Color.Red);
                    return;
                }
                //检查是否处于在库状态, 状态必须为1，或者2（在本工单分拣过）
                status = dt.Rows[0]["STATUS"].ToString().Trim();
                if (status.Equals("2"))
                {
                    if (!dt.Rows[0]["stock_out_no"].ToString().Trim().Equals(MyData.GetStockNo()))
                    {
                        ShowHint("该盘物料已经在其他工单：" + dt.Rows[0]["stock_out_no"].ToString().Trim() + " 中分拣过，请注意！！！", Color.Red);
                    }
                    else
                    {
                        ShowHint("该盘物料已经在本工单分拣过!", Color.Lime);
                    }
                    return;
                }
                else if (!status.Equals("1"))
                {//状态：0表示待接收，1表示接收完成，2表示已出库,6原材料锁定，5过期锁定
                    ShowHint("该盘物料状态为：" + status + ", TrSn=" + trsn + ", 状态：0表示待接收，1表示接收完成，2表示已出库,6原材料锁定，5过期锁定", Color.Red);
                    return;
                }
                richTextBox2.AppendText("状态：" + status + "\r\n");

                //检查料号
                materialno = dt.Rows[0]["KP_NO"].ToString().Trim();
                if (string.IsNullOrEmpty(materialno))
                {
                    ShowHint("在r_inventory_detail表中查询到该trsn的料号为空，请检查!", Color.Red);
                    return;
                }
                richTextBox2.AppendText("料号：" + materialno + "\r\n");
                //检查储位
                locid = dt.Rows[0]["LOC_ID"].ToString().Trim();
                if (string.IsNullOrEmpty(locid))
                {
                    ShowHint("在r_inventory_detail表中查询到该trsn的储位为空，请检查!", Color.Red);
                    return;
                }
                richTextBox2.AppendText("储位：" + locid + "\r\n");

                dic.Clear();
                dic.Add("BoxBarcode", locid);
                dic.Add("StockOutNo", MyData.GetStockNo());
                dic.Add("MaterialId", materialno);
                SearchMaterialManualPick sm = new SearchMaterialManualPick(dic);
                sm.ExecuteQuery();
                dt = sm.GetResult();
                if (dt==null||dt.Rows.Count==0)
                {
                    ShowHint("储位不符, 该盘属于：" + locid, Color.Red);
                    return;
                }
                #endregion

                #region Step3:检查该工单的该料号出库数量是否已经足够，r_sap_material_shippping
                //检查出库数量是否足够
                //int sap_qty = 0;//需求量
                //int ship_qty = 0;//已发量
                //dic.Clear();
                //dic.Add("SHIPPING_NO", textBox_StockNo.Text.Trim());
                //dic.Add("material_no", materialno);
                //dic.Add("DEB_CRED", "OUT");
                //SearchSapMaterialShipping ss = new SearchSapMaterialShipping(dic);
                //ss.ExecuteQuery();
                //dt = ss.GetResult();
                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    sap_qty = int.Parse(dt.Rows[0]["SAP_QTY"].ToString());
                //    ship_qty = int.Parse(dt.Rows[0]["SHIP_QTY"].ToString());
                //    if (sap_qty <= ship_qty)
                //    {
                //        ShowHint("该工单的该料号已经出库完成!", Color.Red);
                //        return;
                //    }
                //}
                #endregion

                #region Step4:出库数据远程绑定
                //出库
                int result = DBPCaller.CompletePickMaterialByTrSn2(MyData.GetStationId(), MyData.GetStockNo(), locid, trsn, MyData.GetStockNoType());
                Trace.WriteLine("Debug: CompletePickMaterialByLocId返回：" + result);
                if (result == 0 || result == 1 || result == 2)
                {
                    //dothing();
                    //保存TrSn    
                    List<string> listTrSn = new List<string>();
                    listTrSn.Add(trsn);
                    SaveTrSn(listTrSn, MyData.GetStockNo(), MyData.GetStockNoType());
                    richTextBox2.AppendText("绑定成功！\r\n");
                }
                else
                {
                    ShowHint("CompletePickMaterialByTrSn2：" + result.ToString(), Color.Red);
                    return;
                }
                #endregion

                #region Step5:从远程再次刷新下本地数据
                //重新刷新数据
                UpdateHumanPickData(MyData.GetStockNo(), materialno, locid);
                #endregion

                #region Step6:满足条件时过账
                //过账
                if (result==1)
                {
                    if (MyData.GetStockNoType() == (int)MyData.PickWoType.Normal ||
                        MyData.GetStockNoType() == (int)MyData.PickWoType.Super)
                    {
                        PushTrSnByStockNo(MyData.GetStockNo());
                    }
                    //SapPick(MyData.GetStockNo(), MyData.GetStockNoType());
                    ShowHint("工单出库过账功能已经关闭，请手动过账", Color.Lime);                    
                    return;
                }
                #endregion
            }
            catch (Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
            }
            finally
            {
                richTextBox2.ScrollToCaret();
                textBox_HumanInput.Text = "";
                textBox_HumanInput.Focus();
            }
        }

        private void textBox_locid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_locid.Text.Trim()))
            {
                return;
            }

            if (textBox_locid.Text.Trim().Length!=10)
            {
                ShowHint("储位号必须为10位", Color.Red);
                return;
            }

            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("loc_id", textBox_locid.Text.Trim());
                dic.Add("status", "1");

                SearchRInventoryDetail sr = new SearchRInventoryDetail(dic);
                sr.ExecuteQuery();
                DataTable dt = sr.GetResult();

                if (dt == null || dt.Rows.Count == 0)
                {
                    Print("查询不到该储位中的数据!");
                    return;
                }

                Print("在储位 " + textBox_locid.Text.Trim() + "  中共查询到：" + dt.Rows.Count.ToString() + " 条记录!");
                foreach(DataRow dr in dt.Rows)
                {
                    Print(dr["TR_SN"].ToString().ToUpper());
                }
                Print("结束！\r\n");
                richTextBox1.ScrollToCaret();
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return;
            }
        }

        private void textBox_trsn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_trsn.Text.Trim()))
            {
                return;
            }

            string trSn = textBox_trsn.Text.Trim().ToUpper();
            if (textBox_trsn.Text.Contains("&"))
            {
                trSn = trSn.Split('&')[0];
            }

            if (trSn.Length != 13 && trSn.Length != 18)
            {
                ShowHint("物料唯一条码长度必须为13位或18位", Color.Red);
                return;
            }

            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("tr_sn", trSn);

                SearchRInventoryDetail sr = new SearchRInventoryDetail(dic);
                sr.ExecuteQuery();
                DataTable dt = sr.GetResult();

                if (dt == null || dt.Rows.Count == 0)
                {
                    Print("查询不到该TrSn信息!");
                    return;
                }

                Print("查询信息如下：");
                Print("物料唯一条码：" + dt.Rows[0]["TR_SN"].ToString());
                Print("数量：" + dt.Rows[0]["QTY"].ToString());
                Print("入库单：" + dt.Rows[0]["STOCK_NO"].ToString());
                Print("出库单：" + dt.Rows[0]["STOCK_OUT_NO"].ToString());
                Print("储位：" + dt.Rows[0]["LOC_ID"].ToString());
                Print("周期：" + dt.Rows[0]["DATE_CODE"].ToString());
                Print("状态：" + dt.Rows[0]["STATUS"].ToString() + ", (0表示待接收，1表示接收完成，2表示已出库, 6原材料锁定，5过期锁定)");
                Print("入库时间：" + dt.Rows[0]["INVENTORY_DATE"].ToString());
                Print("结束！\r\n");
                richTextBox1.ScrollToCaret();
            }
            catch (Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return;
            }
        }

        private void 暂停任务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_StockNo.Text.Trim()) ||
                comboBox_WoidType.SelectedIndex<0)
            {
                ShowHint("请先完善下载页面信息", Color.Red);
                return;
            }

            if (string.IsNullOrEmpty(MyData.GetStockNo()) ||
                string.IsNullOrEmpty(MyData.GetStockSubType()) ||
                MyData.GetStockNoType()==0)
            {
                ShowHint("请在完善下载页面信息", Color.Red);
                return;
            }

            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("StockOutNo", MyData.GetStockNo());

                SearchMaterialPickAssign sm = new SearchMaterialPickAssign(dic);
                sm.ExecuteQuery();
                DataTable dt = sm.GetResult();
                if (dt==null || dt.Rows.Count==0)
                {
                    ShowHint("此工单无任务需要暂停!", Color.Red);
                    return;
                }
            }
            catch(Exception ex)
            {
                ShowHint("SearchMaterialPickAssign:" + ex.Message, Color.Red);
                return;
            }
            
            //暂停任务
            if (MessageBox.Show("确认暂停工单：" + MyData.GetStockNo() + " 的所有出库任务？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)!=DialogResult.Yes)
            {
                return;
            }

            try
            {
                StopPickTask sp = new StopPickTask(MyData.GetStationId());
                sp.ExecuteQuery();
            }
            catch(Exception ex)
            {
                ShowHint("StopPickTask:" + ex.Message, Color.Red);
                return;
            }

            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("StockOutNo", MyData.GetStockNo());

                SearchMaterialPickAssign sm = new SearchMaterialPickAssign(dic);
                sm.ExecuteQuery();
                DataTable dt = sm.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("暂停任务成功!", Color.Lime);
                }
                else
                {
                    ShowHint("暂停任务失败!", Color.Lime);
                }
            }
            catch (Exception ex)
            {
                ShowHint("SearchMaterialPickAssign:" + ex.Message, Color.Red);
                return;
            }
        }

        private void button_CreatePickTask_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_StockNo.Text))
            {
                MessageBox.Show("单号不能为空！");
                return;
            }

            if (comboBox_WoidType.SelectedIndex < 1)
            {
                MessageBox.Show("请先选择一个有效的出库类型！");
                return;
            }

            if (comboBox_TaskType.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择一个任务类型！");
                return;
            }

            if (dataGridView1.Rows.Count == 0)
            {
                ShowHint("请先输入工单回车下载数据，再生成出库任务！", Color.Red);
                return;
            }
            dataGridView_HumanPick.Rows.Clear();

            if (MessageBox.Show("出库类型：" + comboBox_TaskType.Items[comboBox_TaskType.SelectedIndex].ToString(), "提醒", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
            {
                return;
            }

            if (comboBox_TaskType.SelectedIndex == (int)PickTaskType.Split)
            {
                //选择A材出库界面
                using (SetPickStation sp = new SetPickStation())
                {
                    if (sp.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                }
            }

            _taskType = comboBox_TaskType.SelectedIndex;
            Thread t1 = new Thread(CreateTask);
            t1.Start();
        }
    }
}
