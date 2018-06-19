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
using log4net;
using System.Diagnostics;
using System.Threading;
using Phicomm_WMS.DataProcess;

namespace WMS_Client.UI
{
    public partial class SelectionFrm : Office2007Form
    {
        private readonly ILog Log = LogManager.GetLogger("SelectionFrm");
        private Dictionary<string, HolderFrm> _dicHolderView = new Dictionary<string, HolderFrm>();
        private Dictionary<string, CapacityType> _dicHolderData = new Dictionary<string, CapacityType>();
        private string allTrSn = string.Empty;

        private bool runFlag = true;
        private AutoResetEvent eventExecuteInput = new AutoResetEvent(false);

        //本地管理需要用到的一些变量
        private string _lastHolderId = "";
        private int _lastHolderCnt = 0;
        private int _lastScanTrSn = 0;
        private string _lastKpNo = "";
        private string _lastDateCode = "";
        private string _lastFifoDateCode = "";

        public SelectionFrm()
        {
            InitializeComponent();
        }

        private void SelectionFrm_Load(object sender, EventArgs e)
        {
            InitUI();

            InitHolderData(MyData.GetStationId(), 111);
            InitHolderView();

            Thread t3 = new Thread(ExecuteThread);
            t3.Start();
        }

        private void InitUI()
        {
            this.TitleText = "Hi, " + MyData.GetUser() + ", 欢迎使用!";
            label_stationid.Text = "拣选站点 " + MyData.GetStationId().ToString();
            comboBox_staus.SelectedIndex = 0;
            comboBox_stockout.SelectedIndex = 1;
            comboBox_stockout.Enabled = false;

            RefreshStockNoType();
        }

        private void InitHolderData(int stationId, int deviceTypeId)
        {
            try
            {
                Trace.WriteLine("Debug: InitHolderData, stationId=" + stationId + ", deviceTypeId=" + deviceTypeId);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("StationID", MyData.GetStationId());
                dic.Add("DeviceTypeID", deviceTypeId);

                SearchDevice sd = new SearchDevice(dic);
                sd.ExecuteQuery();

                DataTable dt = sd.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    Trace.WriteLine("Debug: 根据站点号和周转箱类型查询不到周转箱序号信息");
                    ShowHint("根据站点号和周转箱类型查询不到周转箱序号信息", Color.Red);
                    return;
                }

                _dicHolderData.Clear();
                foreach(DataRow dr in dt.Rows)
                {
                    CapacityType ct = new CapacityType();
                    ct.current = 0;
                    ct.total = 0;

                    _dicHolderData.Add(dr["DeviceID"].ToString().Trim(), ct);                         
                }            
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Debug: exception=" + ex.Message);
                ShowHint(ex.Message, Color.Red);
            }
        }

        private void InitHolderView()
        {
            try
            {
                Trace.WriteLine("Debug: InitHolderView");
                _dicHolderView.Clear();
                if (_dicHolderData.Count == 0)
                {
                    return;
                }

                _dicHolderView.Clear();
                tableLayoutPanel_Holder.Controls.Clear();
                foreach (KeyValuePair<string, CapacityType> obj in _dicHolderData)
                {
                    Trace.WriteLine("Debug: holderId=" + obj.Key + ", " + obj.Value.current.ToString() + "/" + obj.Value.total.ToString());

                    HolderFrm hf = new HolderFrm();
                    hf.SetValue(obj.Key, obj.Value.current.ToString(), obj.Value.total.ToString());
                    hf.Dock = DockStyle.Fill;
                    hf.TopLevel = false;
                    if (obj.Value.current > 0)
                    {
                        hf.BackColor = Color.FromArgb(194, 217, 247);
                    }
                    else
                    {
                        hf.BackColor = Color.DarkGray;
                    }
                    hf.Parent = tableLayoutPanel_Holder;
                    tableLayoutPanel_Holder.Controls.Add(hf);
                    hf.Show();

                    _dicHolderView.Add(obj.Key, hf);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Debug: exception=" + ex.Message);
                ShowHint("InitHolderView:" + ex.Message, Color.Red);
            }
        }

        private bool GetHolderCapacity(string woid, int holder)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("stock_no", woid);
                dic.Add("holder_id", holder);

                SearchRHolderLocidRelationShip sr = new SearchRHolderLocidRelationShip(dic);
                sr.ExecuteQuery();

                DataTable dt = sr.GetResult();
                if (dt==null || dt.Rows.Count==0)
                {
                    return false;
                }

                if (_dicHolderData.ContainsKey(dt.Rows[0]["holder_id"].ToString().Trim()))
                {
                    //CapacityType ct = new CapacityType();
                    //ct.current = int.Parse(dt.Rows[0]["on_qty"].ToString().Trim());
                    //ct.total = int.Parse(dt.Rows[0]["max_qty"].ToString().Trim());

                    _dicHolderData[dt.Rows[0]["holder_id"].ToString().Trim()].current = int.Parse(dt.Rows[0]["on_qty"].ToString().Trim());
                    _dicHolderData[dt.Rows[0]["holder_id"].ToString().Trim()].total = int.Parse(dt.Rows[0]["max_qty"].ToString().Trim());
                }

                return true;
            }
            catch(Exception ex)
            {
                ShowHint("GetHolderCapacity:" + ex.Message, Color.Red);
                return false;
            }
        }

        private void GetHolderCapacity(string woid)
        {
            try
            {
                Trace.WriteLine("Debug: GetHolderCapacity");

                //全部清零
                foreach(KeyValuePair<string, CapacityType> obj in _dicHolderData)
                {
                    obj.Value.current = 0;
                    obj.Value.total = 0;
                }

                //从数据库中捞取
                Dictionary <string, object> dic = new Dictionary<string, object>();
                dic.Add("stock_no", woid);
                SearchRHolderLocidRelationShip sr = new SearchRHolderLocidRelationShip(dic);
                sr.ExecuteQuery();
                DataTable dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    return;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    if (_dicHolderData.ContainsKey(dr["holder_id"].ToString().Trim()))
                    {
                        Trace.Write("Debug: holder_id=" + dr["holder_id"].ToString().Trim() + 
                                    ", " + dr["on_qty"].ToString().Trim() + "/" + dr["max_qty"].ToString().Trim());

                        _dicHolderData[dr["holder_id"].ToString().Trim()].current = int.Parse(dr["on_qty"].ToString().Trim());
                        _dicHolderData[dr["holder_id"].ToString().Trim()].total = int.Parse(dr["max_qty"].ToString().Trim());
                    }
                }                
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                ShowHint("GetHolderCapacity:" + ex.Message, Color.Red);
            }
        }



        //重新初始化Holder的显示，在改变周转箱类型时调用
        private void UpdateHolderView()
        {
            try
            {
                Trace.WriteLine("Debug: UpdateHolderView");
                if (_dicHolderData.Count==0)
                {                    
                    return;
                }

                foreach (KeyValuePair<string, CapacityType> obj in _dicHolderData)
                {
                    if (_dicHolderView.ContainsKey(obj.Key))
                    {
                        Trace.WriteLine("Debug: holderId=" + obj.Key + ", " + obj.Value.current.ToString() + "/" + obj.Value.total.ToString());

                        _dicHolderView[obj.Key].SetCnt(obj.Value.current.ToString(), obj.Value.total.ToString());
                        if (obj.Value.current>0)
                        {
                            _dicHolderView[obj.Key].BackColor = Color.FromArgb(194, 217, 247);
                        }
                        else if (obj.Value.current==0)
                        {
                            _dicHolderView[obj.Key].BackColor = Color.DarkGray;
                        }
                    }
                }                
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Debug: exception=" + ex.Message);
                ShowHint("InitHolderView:" + ex.Message, Color.Red);
            }
        }

        //更新周转箱中数量时显示
        private void UpdateHolderView(string holderId, Color cl)
        {
            if (!runFlag) return;

            try
            {
                if (this.IsHandleCreated)
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        if (_dicHolderData.ContainsKey(holderId) &&
                            _dicHolderView.ContainsKey(holderId))
                        {
                            _dicHolderView[holderId].SetCnt(_dicHolderData[holderId].current.ToString(),
                            _dicHolderData[holderId].total.ToString());

                            if (_dicHolderData[holderId].current!=0 &&
                                _dicHolderData[holderId].current == _dicHolderData[holderId].total)
                            {
                                _dicHolderView[holderId].BackColor = Color.Red;
                            }
                            else
                            {
                                _dicHolderView[holderId].BackColor = cl;
                            }
                        }
                    }));
                }
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
            }
        }

        private void RefreshStockNoType()
        {
            try
            {                
                comboBox_WoidType.SelectedIndex = -1;
                comboBox_WoidType.Items.Clear();
                comboBox_WoidType.Items.Add("-----请选择-----");
                comboBox_WoidType.SelectedIndex = 0;

                DataTable dt = GetWoIdType(7);
                if (dt == null || dt.Rows.Count==0)
                {
                    return;
                }
                foreach(DataRow dr in dt.Rows)
                {
                    comboBox_WoidType.Items.Add(dr["StockNoType"].ToString() + "-" + 
                                                dr["WoidType"].ToString() + " - " + 
                                                dr["MoveName"].ToString());
                }

                comboBox_WoidType.Items.Add("8-1000X-工单退料");                
            }
            catch(Exception ex)
            {
                ShowHint("查询工单类型异常：" + ex.Message, Color.Red);
            }
        }

        private DataTable GetWoIdType(int stockNoType)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();

                dic.Add("StockNoType", stockNoType.ToString());

                SearchTbFactoryMove sr = new SearchTbFactoryMove(dic);
                sr.ExecuteQuery();
                
                return sr.GetResult();
            }
            catch(Exception ex)
            {
                ShowHint("GetWoIdType: " + ex.Message, Color.Red);
                return null;
            }
        }

        private void textBox_StockNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter)
                {
                    return;
                }               

                if (string.IsNullOrEmpty(textBox_StockNo.Text.Trim()))
                {
                    MessageBox.Show("请输入工单号!");
                    return;
                }                

                if (comboBox_WoidType.SelectedIndex<1)
                {
                    ShowHint("请选择类型", Color.Red);
                    return;
                }

                if (CheckWoid(textBox_StockNo.Text.Trim()))
                {
                    if (SetSubType())
                    {
                        UpdateTable();
                        UpdateHolderInfo();
                    }
                }

                label_stationid.Text = "拣选站点 " + MyData.GetStationId().ToString() + ", " + textBox_StockNo.Text;
                textBox_stockin.Text = textBox_StockNo.Text;
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
            }
        }

        public bool CheckWoid(string woid)
        {            
            Dictionary<string, object> dic = new Dictionary<string, object>();
            DataTable dt = new DataTable();

            //查询工单是否存在
            try
            {
                dic.Add("stock_no", woid);

                SearchRInventoryId sr = new SearchRInventoryId(dic);
                sr.ExecuteQuery();
                dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("在r_inventory_id中查询不到该工单，请核对单号!", Color.Red);
                    return false;
                }
                if (dt.Rows[0]["stockno_type"].ToString().Trim().Equals("8"))
                {
                    if (string.IsNullOrEmpty(dt.Rows[0]["sub_type"].ToString().Trim()))
                    {
                        ShowHint("在r_inventory_id中该退料工单的sub_type为空，请先完善信息!", Color.Red);
                        return false;
                    }
                    else
                    {
                        MyData.SetStockNo(woid);
                        MyData.SetStockNoType(int.Parse(dt.Rows[0]["stockno_type"].ToString().Trim()));
                        MyData.SetStockSubType(dt.Rows[0]["sub_type"].ToString().Trim());
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowHint("SearchRInventoryId: " + ex.Message, Color.Red);
                return false;
            }

            //物料待检验
            dt = GetData(woid, "0");
            if (dt!=null && dt.Rows.Count>0)
            {
                ShowHint("该工单有物料待检验！", Color.Red);
            }

            //Hold状态
            dt = GetData(woid, "1");
            if (dt != null && dt.Rows.Count > 0)
            {
                ShowHint("该工单有物料已Hold！", Color.Red);
            }

            //以下是查询到的该工单类型为7的情况
            dt = GetData(woid, "4");
            if (dt==null || dt.Rows.Count==0)
            {
                dt = GetData(woid, "5");
                if (dt != null && dt.Rows.Count > 0)
                {
                    comboBox_staus.SelectedIndex = 2;
                }
            }
            if (dt!=null && dt.Rows.Count>0)
            {
                if (string.IsNullOrEmpty(dt.Rows[0]["sub_type"].ToString().Trim()))                
                {
                    ShowHint("有物料已进周转箱或已上架(状态4，5)，但是sub_type字段为空!", Color.Red);
                    return false;
                }
                else
                {
                    MyData.SetStockNo(woid);
                    MyData.SetStockNoType(int.Parse(dt.Rows[0]["stockno_type"].ToString().Trim()));
                    MyData.SetStockSubType(dt.Rows[0]["sub_type"].ToString().Trim());
                    return true;
                }
            }

            //写入sub_type
            if (comboBox_WoidType.SelectedIndex==comboBox_WoidType.Items.Count-1)
            {
                ShowHint("该工单类型在R_Inventory_Id中不为8， 请重新选中!", Color.Red);
                return false;
            }
            try
            {
                string subtype = comboBox_WoidType.Items[comboBox_WoidType.SelectedIndex].ToString().Split('-')[1];
                string stocknotype = comboBox_WoidType.Items[comboBox_WoidType.SelectedIndex].ToString().Split('-')[0];

                UpdateRInventoryIdSubType ut = new UpdateRInventoryIdSubType(woid, subtype.Trim());
                ut.ExecuteUpdate();

                MyData.SetStockNo(woid);
                MyData.SetStockNoType(int.Parse(stocknotype));
                MyData.SetStockSubType(subtype);

                return true;
            }
            catch(Exception ex)
            {
                ShowHint("更新Sub_Type异常：" + ex.Message, Color.Red);
                return false;
            }            
        }

        private bool SetSubType()
        {
            comboBox_WoidType.SelectedIndex = 0;

            if (MyData.GetStockNoType()==8)
            {
                comboBox_WoidType.SelectedIndex = comboBox_WoidType.Items.Count - 1;
                return true;
            }
            for (int i=1; i<comboBox_WoidType.Items.Count; i++)
            {
                if (comboBox_WoidType.Items[i].ToString().Contains(MyData.GetStockSubType()))
                {
                    comboBox_WoidType.SelectedIndex = i;
                    return true;
                }
            }

            return false;
        }

        public DataTable GetData(string woid, string status)
        {
            //查询数据
            Dictionary<string, object> dic = new Dictionary<string, object>();
            DataTable dt = new DataTable();

            try
            {
                dic.Add("stock_no", woid);
                dic.Add("status", status);

                SearchRInventoryId sr = new SearchRInventoryId(dic);
                sr.ExecuteQuery();
                dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                ShowHint("SearchRInventoryId: " + ex.Message, Color.Red);
            }

            return dt;
        }

        public void ShowData(DataGridView dgv, DataTable dt)
        {
            if (dgv==null)
            {
                return;
            }
            dgv.Rows.Clear();

            if (dt ==null || dt.Rows.Count==0)
            {
                return;
            }
            foreach (DataRow dr in dt.Rows)
            {
                dgv.Rows.Add(false,
                            dr["material_no"].ToString(),
                            dr["qty"].ToString(),
                            dr["income_qty"].ToString(),
                            dr["material_desc"].ToString(),
                            dr["vender_name"].ToString());
            }
            dgv.ClearSelection();
        }

        public void ShowData2(DataGridView dgv, DataTable dt)
        {
            if (dgv == null)
            {
                return;
            }
            dgv.Rows.Clear();

            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }

            dgv.Rows.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                dgv.Rows.Add(
                            dr["material_no"].ToString(),
                            dr["qty"].ToString(),
                            dr["income_qty"].ToString(),
                            dr["material_desc"].ToString(),
                            dr["vender_name"].ToString());
            }
            dgv.ClearSelection();
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

        private void UpdateHolderInfo()
        {
            if (string.IsNullOrEmpty(textBox_StockNo.Text.Trim()))
            {
                ShowHint("请先在分拣页面将单号填写完整!", Color.Red);
                return;
            }
            
            if (_dicHolderData.Count==0)
            {
                InitHolderData(MyData.GetStationId(), 111);
                InitHolderView();
            }

            if (_dicHolderData.Count > 0)
            {
                GetHolderCapacity(textBox_StockNo.Text.Trim());
                
                UpdateHolderView();                
            }                 
        }
        private void SelectionFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.F1)
            {
                //全选
                button_select_Click(sender, e);
            }
            else if (e.KeyCode == Keys.F2)
            {
                //要求分拣
                button_selection_Click(sender, e);
            }
            else if (e.KeyCode == Keys.F9)
            {
                textBox_BarCode.Enabled = false;

                //刷新周转箱信息
                UpdateHolderInfo();

                textBox_BarCode.Enabled = true;
                textBox_BarCode.Focus();

                //更新信息
                _lastHolderId = "";
                _lastHolderCnt = 0;

                _lastScanTrSn = 0; //扫入正确trSn的次数

                _lastKpNo = "";
                _lastDateCode = "";
                _lastFifoDateCode = "";

                allTrSn = "";
                dataGridView_TrSn.Rows.Clear(); //清空表格

                label_cnt.Text = "周转箱：" + _lastHolderId + " 中已扫入：" + dataGridView_TrSn.Rows.Count + " 盘物料";
            }
            else if (e.KeyCode == Keys.F4)
            {
                //textBox_BarCode.Enabled = false;
                ////叫车，自动入库
                //CallAgv(MyData.GetStationId(), MyData.GetStockNo());
                //UpdateHolderInfo();
                //textBox_BarCode.Enabled = true;
                //textBox_BarCode.Focus();
            }
            else if (e.KeyCode == Keys.F5)
            {
                //textBox_BarCode.Enabled = false;
                ////手动入库
                //CreateHumanTask(MyData.GetStationId(), MyData.GetStockNo());
                //UpdateHolderInfo();
                //textBox_BarCode.Enabled = true;
                //textBox_BarCode.Focus();
            }
        }

        private void comboBox_WoidType_DropDown(object sender, EventArgs e)
        {
            RefreshStockNoType();
        }

        private void button_select_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0) return;
                    
            string _selectValue = dataGridView1.Rows[0].Cells["Column1"].EditedFormattedValue.ToString();
            bool res = !_selectValue.Equals("True");

            for (int i=0; i<dataGridView1.Rows.Count; i++)
            {                
                ((DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells["Column1"]).Value = res;
            }
            if (res)
            {
                button_select.Text = "全不选[F1]";
            }
            else
            {
                button_select.Text = "全选[F1]";
            }
        }

        private void button_selection_Click(object sender, EventArgs e)
        { 
            try
            {
                button_select.Enabled = false;
                button_selection.Enabled = false;

                for(int i=0; i<dataGridView1.Rows.Count; i++)
                {
                    if (!dataGridView1.Rows[i].Cells["Column1"].EditedFormattedValue.ToString().Equals("True"))
                    {
                        continue;
                    }

                    if (!CheckMaterialInfo(dataGridView1.Rows[i].Cells["Column2"].Value.ToString().Trim()))
                    {
                        ((DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells["Column1"]).Value = false;
                        continue;
                    }

                    //分拣
                    if (!UpdateStatus(textBox_StockNo.Text, dataGridView1.Rows[i].Cells["Column2"].Value.ToString().Trim(), "2", "3", MyData.GetStationId()))
                    {
                        UpdateTable();
                        return;
                    }
                }

                UpdateTable();
                ShowHint("分拣完成", Color.Red);
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
            }
            finally
            {
                button_select.Enabled = true;
                button_selection.Enabled = true;
            }
        }

        private bool CheckMaterialInfo(string kpno)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("material_no", kpno);

                SearchBMaterial sm = new SearchBMaterial(dic);
                sm.ExecuteQuery();
                DataTable dt = sm.GetResult();

                if (dt==null || dt.Rows.Count==0)
                {
                    ShowHint("查询不到该料号的基础信息，请检查！", Color.Red);
                    return false;
                }

                if (string.IsNullOrEmpty(dt.Rows[0]["material_level"].ToString()) ||
                    string.IsNullOrEmpty(dt.Rows[0]["process"].ToString()) ||
                    string.IsNullOrEmpty(dt.Rows[0]["material_type"].ToString()) ||
                        string.IsNullOrEmpty(dt.Rows[0]["material_subtype"].ToString()))
                {
                    ShowHint("物料信息未维护，请确认！", Color.Red);
                    return false;
                }

                return true;
            }
            catch(Exception ex)
            {
                Log.Error("CheckMaterialInfo:" + ex.Message);
                return false;
            }
        }

        private bool UpdateStatus(string woid, string kpno, string oldstatus, string newstatus, int stationId)
        {
            try
            {
                UpdateRInventoryIdStatus ur = new UpdateRInventoryIdStatus(woid, kpno, oldstatus, newstatus, stationId);

                ur.ExecuteUpdate();

                return true;
            }
            catch(Exception ex)
            {
                ShowHint("UpdateRInventoryId: " + ex.Message, Color.Red);
                return false;
            }
        }

        private void UpdateTable()
        {
            DataTable dt1 = GetData(textBox_StockNo.Text.Trim(), "2");
            ShowData(dataGridView1, dt1);
            DataTable dt2 = GetData(textBox_StockNo.Text.Trim(), (comboBox_staus.SelectedIndex + 3).ToString());
            ShowData2(dataGridView2, dt2);            
        }

        private void comboBox_staus_DropDown(object sender, EventArgs e)
        {

        }

        private void textBox_BarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_BarCode.Text))
            {
                return;
            }

            if (string.IsNullOrEmpty(MyData.GetStockNo()) ||
                string.IsNullOrEmpty(MyData.GetStockSubType()))
            {
                ShowHint("请先到分拣页面填写单号然后回车获取信息", Color.Red);
                textBox_BarCode.Text = "";
                textBox_BarCode.Focus();
                return;
            }

            InputBarCode(textBox_BarCode.Text.ToUpper().Trim());
        }

        private void comboBox_HolderType_DropDown(object sender, EventArgs e)
        {

        }

        //private void UpdateHolderType(int deviceTypeId)
        //{            
        //    try
        //    {
        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        dic.Add("StationID", MyData.GetStationId());
        //        dic.Add("DeviceTypeID", deviceTypeId);

        //        SearchDevice sd = new SearchDevice(dic);
        //        sd.ExecuteQuery();

        //        DataTable dt = sd.GetResult();
        //        if (dt == null || dt.Rows.Count == 0)
        //        {
        //            ShowHint("根据站点号和周转箱类型查询不到周转箱序号信息", Color.Red);
        //            return;
        //        }

        //        tableLayoutPanel_Holder.Controls.Clear();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            //tableLayoutPanel_Holder.Controls.Add();
        //            HolderFrm hf = new HolderFrm();
        //            hf.SetValue(dr["DeviceID"].ToString(), "0", "0");
        //            hf.Dock = DockStyle.Fill;
        //            hf.TopLevel = false;
        //            hf.Parent = tableLayoutPanel_Holder;
        //            tableLayoutPanel_Holder.Controls.Add(hf);
        //            hf.Show();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowHint(ex.Message, Color.Red);
        //    }
        //}

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
                        textBox_BarCode.Focus();
                    }
                    button_AutoReplenish.Enabled = en;
                    button_HumanReplenish.Enabled = en;
                }));
            }
        }

        private void InputBarCode(string str)
        {
            try
            {
                textBox_BarCode.Enabled = false;
                button_AutoReplenish.Enabled = false;
                button_HumanReplenish.Enabled = false;

                if (str.Contains("&")) //输入的是五合一条码
                {
                    if (CheckTrSn(str))
                    {
                        //周转箱内扫入第一盘物料时自动上传
                        if (_lastScanTrSn==0)
                        {
                            _lastScanTrSn = 1;
                            eventExecuteInput.Set(); //绑定
                        }
                        else
                        {
                            _lastScanTrSn++;
                        }
                    }
                }
                else  //输入的是周转箱
                {
                    //检查周转箱编号是否合法
                    if (!_dicHolderData.ContainsKey(str))
                    {
                        ShowHint("请扫入正确的周转箱编号", Color.Red);
                        return;
                    }
                    if (!_dicHolderView.ContainsKey(str))
                    {
                        ShowHint("View与Data不匹配", Color.Red);
                        return;
                    }

                    //
                    if (_lastHolderCnt == 0)  //第一次扫入周转箱
                    {
                        ResetHoderId(str);
                    }
                    else if (_lastHolderCnt == 1)  //第二次扫入
                    {
                        if (_lastHolderId.Equals(str))   //第二次扫入同样的周转箱编号，则上传绑定数据
                        {
                            //进周转箱
                            eventExecuteInput.Set();  //上传数据                              
                        }
                        else   //第二次扫入与第一次不一样的周转箱编号
                        {
                            if (dataGridView_TrSn.Rows.Count > 0)   //如果存在缓存数据，则提示用户是否需要更换周转箱编号
                            {
                                if (MessageBox.Show("请确认是否更换周转箱为：" + str + ", 上一次的周转箱为：" + _lastHolderId + ", 更换周转箱后未保存的数据将丢失！", "提示",
                                                    MessageBoxButtons.OKCancel,
                                                    MessageBoxIcon.Warning,
                                                    MessageBoxDefaultButton.Button1) == DialogResult.OK)
                                {                                    
                                    //更换了周转箱, 需要清空信息
                                    ResetHoderId(str);
                                }
                                else
                                {
                                    //do nothing();
                                }
                            }
                            else   //如果不存在缓存数据，直接更换周转箱编号
                            {
                                //更换了周转箱, 需要清空信息
                                ResetHoderId(str);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
            }
            finally
            {
                button_AutoReplenish.Enabled = true;
                button_HumanReplenish.Enabled = true;
                textBox_BarCode.Enabled = true;
                textBox_BarCode.Text = "";
                textBox_BarCode.Focus();
            }
        }

        //重置HolderId
        private void ResetHoderId(string str)
        {
            Print("\r\n已更换周转箱为：" + str);
            if (_dicHolderView.ContainsKey(_lastHolderId) && _dicHolderData.ContainsKey(_lastHolderId))
            {
                if (_dicHolderData[_lastHolderId].current > 0)
                {
                    _dicHolderView[_lastHolderId].BackColor = Color.FromArgb(194, 217, 247);
                }
                else
                {
                    _dicHolderView[_lastHolderId].BackColor = Color.DarkGray;//Color.FromArgb(194, 217, 247); //取消选中
                }  
            }
            if (_dicHolderView.ContainsKey(str))
            {
                _dicHolderView[str].BackColor = Color.Yellow; //选中
            }
            _lastHolderId = str;
            _lastHolderCnt = 1;

            _lastScanTrSn = 0; //扫入正确trSn的次数

            _lastKpNo = "";
            _lastDateCode = "";
            _lastFifoDateCode = "";

            allTrSn = "";
            dataGridView_TrSn.Rows.Clear(); //清空表格

            label_cnt.Text = "周转箱：" + _lastHolderId + " 中已扫入：" + dataGridView_TrSn.Rows.Count + " 盘物料";
        }

        private void ExecuteThread()
        {
            while(runFlag)
            {
                try
                {
                    eventExecuteInput.WaitOne();
                    if (!runFlag) return;

                    EnableInput(false);

                    if (!LKHolder())
                    {                        
                        Print("周转箱：" + _lastHolderId + " 中信息上传失败， 请检查！");
                        ShowHint("周转箱：" + _lastHolderId + " 中信息上传失败， 请检查！", Color.Red);
                    }
                    else
                    {
                        Print("周转箱：" + _lastHolderId + " 中信息全部上传成功！");
                        ResetTable();

                    }                    
                }
                catch(Exception ex)
                {
                    Log.Error(ex.Message);
                    ShowHint(ex.Message, Color.Red);
                }
                finally
                {
                    EnableInput(true);                    
                }
            }
        }

        private void ResetTable()
        {
            if (!runFlag) return;

            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {
                    dataGridView_TrSn.Rows.Clear();
                }));
            }
        }

        private bool CheckTrSn(string str)
        {
            try
            {
                str = str.ToUpper().Trim();
                if (!str.Contains("&"))
                {
                    ShowHint("五合一条码规则不匹配", Color.Red);
                    return false;
                }

                if (str.Split('&').Length != 5)
                {
                    ShowHint("五合一条码规则不匹配", Color.Red);
                    return false;
                }

                string[] listStr = str.Split('&');
                string trSn = listStr[0];
                string materialId = listStr[1];
                string factorId = listStr[2];
                string dateCode = listStr[3];
                int qty = int.Parse(listStr[4]);
                string fifoDateCode = DataCodeProcess.fifo_datecode(dateCode.ToUpper().Trim());

                //检查TrSn长度，规则
                if (trSn.Length != 18 && trSn.Length != 13)
                {
                    ShowHint("TrSn长度不正确,应为13或18位，当前TrSn=" + trSn + ",长度：" + trSn.Length, Color.Red);
                    return false;
                }
                if (!trSn.All(c => ((c <= '9' && c >= '0') || (c <= 'Z' && c >= 'A'))))
                {
                    ShowHint("TrSn不正确：" + trSn, Color.Red);
                    return false;
                }

                if (string.IsNullOrEmpty(fifoDateCode))
                {
                    ShowHint("该物料的fifoDataCode计算出来为0，异常", Color.Red);
                    return false;
                }

                //本地重复性检查
                if (allTrSn.Contains(trSn))
                {
                    ShowHint("该盘已在本地扫描过", Color.Red);
                    return false;
                }

                //查询物料等级
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("material_no", materialId);
                SearchBMaterial sb = new SearchBMaterial(dic);
                sb.ExecuteQuery();
                DataTable dt = sb.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("在b_material表中查询不到该料号：" + materialId, Color.Red);
                    return false;
                }
                if (string.IsNullOrEmpty(dt.Rows[0]["material_level"].ToString().Trim()))
                {
                    ShowHint("在b_material表中查询到该料号的物料等级为空，请检查：" + materialId, Color.Red);
                    return false;
                }

                //如果物料等级为2，需要更新算法
                if (dt.Rows[0]["material_level"].ToString().Trim().Equals("2"))
                {
                    fifoDateCode = DataCodeProcess.GetFirstDayOfMonth(fifoDateCode);
                }

                if (string.IsNullOrEmpty(_lastKpNo))
                {//该周转箱的第一排料
                    _lastKpNo = materialId;
                    _lastDateCode = dateCode;
                    _lastFifoDateCode = fifoDateCode;
                }
                else
                {
                    if (!materialId.Equals(_lastKpNo))
                    {
                        ShowHint("该盘料号：" + materialId + ", 上一盘料号：" + _lastKpNo + ", 不能放入一个周转箱", Color.Red);
                        return false;
                    }
                    //else if (!dateCode.Equals(_lastDateCode))
                    //{
                    //    ShowHint("该盘DateCode：" + dateCode + ", 上一盘DateCode：" + _lastDateCode + ", 不能放入一个周转箱", Color.Red);
                    //    return false;
                    //}
                    else if (!fifoDateCode.Equals(_lastFifoDateCode))
                    {
                        ShowHint("该盘fifoDC：" + fifoDateCode + ", 上一盘fifoDC：" + _lastFifoDateCode + ", 不能放入一个周转箱", Color.Red);
                        return false;
                    }
                }

                allTrSn = allTrSn + "," + trSn;

                AddTable(trSn, materialId, dateCode, fifoDateCode, qty, factorId);

                label_cnt.Text = "周转箱：" + _lastHolderId + " 中已扫入：" + dataGridView_TrSn.Rows.Count + " 盘物料";
                Print("\r\n已扫入：" + trSn + ", \r\n" + str);
                return true;
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return false;
            }
        }

        private void AddTable(string trSn, string materialId, string dateCode, string fifoDateCode, int qty, string factorId)
        {
            if (!runFlag) return;

            //if (this.IsHandleCreated)
            //{
            //    this.Invoke(new EventHandler(delegate
            //    {
                    int count = dataGridView_TrSn.Rows.Count + 1;

                    dataGridView_TrSn.Rows.Add(count,
                                               trSn, 
                                               "N",
                                               materialId, 
                                               dateCode, 
                                               fifoDateCode,
                                               qty,
                                               factorId);

                    dataGridView_TrSn.ClearSelection();
                    //dataGridView_TrSn.Rows[count-1].Selected = true;
                    dataGridView_TrSn.FirstDisplayedScrollingRowIndex = count - 1;
                    textBox_BarCode.Text = "";
            //    }));
            //}
        }

        //调用远端接口，判断条码类型，是储位编号还是物料条码
        //private bool CheckBarcode(string barcode, ref int type)
        //{
        //    try
        //    {
        //        int result = DBPCaller.CheckBarcode(barcode);
        //        if (result == 2 /*周转箱编号*/ || result == 3 /*TR_SN*/)
        //        {
        //            type = result;
        //            return true;
        //        }
        //        else if (result==1)
        //        {
        //            ShowHint("扫入的是储位编号，请扫描正确的条码!", Color.Red);
        //        }                
        //        else if (result == 4)
        //        {
        //            ShowHint("扫入的是出库单号，请扫描正确的条码!", Color.Red);
        //        }
        //        else if (result == 5)
        //        {
        //            ShowHint("扫入的是入库单号，请扫描正确的条码!", Color.Red);
        //        }
        //        else
        //        {
        //            ShowHint("未知条码，请扫正确的条码", Color.Red);
        //        }

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("CheckBarcode: " + ex.Message);
        //        ShowHint("CheckBarcode: " + ex.Message, Color.Red);
        //        return false;
        //    }
        //}

        private bool LKHolder()
        {
            bool result = true;
            int res = -99;
            for (int i = 0; i < dataGridView_TrSn.Rows.Count; i++)
            {
                try
                {
                    if (dataGridView_TrSn[2,i].Value.ToString().Trim().Equals("Y"))
                    {
                        continue;
                    }

                    res = DBPCaller.LinkHolderMaterial(dataGridView_TrSn[1, i].Value.ToString().ToUpper(),
                                                 MyData.GetStockNo(),
                                                 dataGridView_TrSn[3, i].Value.ToString(),
                                                 dataGridView_TrSn[4, i].Value.ToString(),
                                                 dataGridView_TrSn[5, i].Value.ToString(),
                                                 int.Parse(dataGridView_TrSn[6, i].Value.ToString()),
                                                 int.Parse(_lastHolderId),
                                                 MyData.GetStationId(),
                                                 dataGridView_TrSn[7, i].Value.ToString(),
                                                 MyData.GetStockSubType(),
                                                 MyData.GetStockNoType()
                                                 );
                    Trace.WriteLine("Debug: LinkHolderMaterial return" + res);

                    if (res == 1 ||  //料号正确，继续刷入条码   
                        res == 2 ||  //已刷入当前料号的最后一盘，请更换周转箱
                        res == 3)     //所有物料已经进到周转箱，可以叫车              
                    {
                        Log.Error("LinkHolderMaterial返回类型: " + res + ", trsn=" + dataGridView_TrSn[1, i].Value.ToString() + " 已删除！");
                        UpdateUploadStatus(i, "Y");
                        //删除TrSn
                        try
                        {
                            Phicomm_WMS.OUTIO.tR_Tr_Sn.DelTrSn(dataGridView_TrSn[1, i].Value.ToString());
                        }
                        catch (Exception ex)
                        {
                            Log.Error("DelTrSn ex: " + ex.Message);
                            ShowHint("DelTrSn ex: " + ex.Message, Color.Red);
                        }
                    }
                    else
                    {
                        Log.Error("LinkHolderMaterial返回类型: " + res + ", trsn=" + dataGridView_TrSn[1, i].Value.ToString() + " 未删除！");
                        ShowHint("LinkHolderMaterial返回类型：" + res, Color.Red);
                        UpdateUploadStatus(i, "F");
                        result = false;

                        if (dataGridView_TrSn.Rows.Count == 1) //如果是单盘上传失败时
                        {
                            ResetTable();

                            //从本地TrSn列表中移除该TrSn, 可以再次扫描，
                            allTrSn = "";
                            _lastScanTrSn = 0;
                            _lastKpNo = "";
                            _lastDateCode = "";
                            _lastFifoDateCode = "";
                        }
                        break;
                    }                                      
                }
                catch (Exception ex)
                {
                    ShowHint(ex.Message, Color.Red);
                    Log.Error("LKHolder：" + ex.Message);
                    result = false;
                }
            }

            //全部更新
            GetHolderCapacity(MyData.GetStockNo(), int.Parse(_lastHolderId));
            if (res == 1) //料号正确，继续刷入条码                      
            {
                UpdateHolderView(_lastHolderId, Color.Yellow);
            }
            else if (res == 2) //已刷入当前料号的最后一盘，请更换周转箱
            {
                UpdateHolderView(_lastHolderId, Color.Lime);
            }
            else if (res == 3) //所有物料已经进到周转箱，可以叫车
            {
                UpdateHolderView(_lastHolderId, Color.Orange);
                ShowHint("所有物料都已进到周转箱，本工单已完成！", Color.Lime);
            }
            else
            {
                UpdateHolderView(_lastHolderId, Color.Yellow);
            }

            return result;          
        }

        private void UpdateUploadStatus(int row, string status)
        {
            if (!runFlag) return;

            if (row>=dataGridView_TrSn.Rows.Count)
            {
                return;
            }

            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {
                    dataGridView_TrSn[2, row].Value = status;
                }));
            }
        }

        private void Print(string str)
        {
            if (!runFlag) return;

            if (this.IsHandleCreated)
            {
                this.Invoke(new EventHandler(delegate
                {
                    richTextBox1.AppendText(str + "\r\n");
                    richTextBox1.ScrollToCaret();
                    textBox_BarCode.Focus();
                }));
            }
        }

        private void SelectionFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            runFlag = false;
            eventExecuteInput.Set();
        }

        private void CallAgv(int stationId, string stockno)
        {
            try
            {
                string str = DBPCaller.CheckMaterialIntoHolders(stationId, stockno);

                Trace.WriteLine("Debug: CheckMaterialIntoHolders return " + str);
                ShowHint(str, Color.Red);
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
            }
        }

        private void CreateHumanTask(int stationId, string stockno)
        {
            try
            {
                string str = DBPCaller.CheckMaterialIntoHolders2(stationId, stockno, 1);

                Trace.WriteLine("Debug: CheckMaterialIntoHolders2 return " + str);
                ShowHint(str, Color.Red);
            }
            catch (Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
            }
        }

        private void textBox_sn1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_sn1.Text))
            {
                ShowHint("请先输入单号", Color.Red);
                return;
            }

            DataTable dt = new DataTable();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                dic.Add("stock_no", textBox_sn1.Text);
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
            catch (Exception ex)
            {
                ShowHint("SearchRInventoryId:" + ex.Message, Color.Red);
                return;
            }

            try
            {
                dic.Clear();
                dic.Add("stock_no", textBox_sn1.Text);
                //dic.Add("stock_out_no", "");
                dic.Add("status", "0");

                SearchRInventoryDetail sd = new SearchRInventoryDetail(dic);
                sd.ExecuteQuery();
                dt = sd.GetResult();

                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("该工单无数据需要推送", Color.Lime);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowHint("SearchRInventoryDetail:" + ex.Message, Color.Red);
                return;
            }


            richTextBox2.AppendText("共需要推送：" + dt.Rows.Count + " 条数据!\r\n");
            foreach (DataRow dr in dt.Rows)
            {
                if (!string.IsNullOrEmpty(dr["tr_sn"].ToString()))
                {
                    try
                    {
                        if (Phicomm_WMS.OUTIO.tR_Tr_Sn.DelTrSn(dr["tr_sn"].ToString()))
                        {
                            richTextBox2.AppendText("删除TrSn=" + dr["tr_sn"].ToString() + " 的线边仓信息成功!\r\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        richTextBox2.AppendText("TrSn=" + dr["tr_sn"].ToString() + ", " + ex.Message + "\r\n");
                    }
                }
            }

            richTextBox2.AppendText("删除完毕");
        }

        private void textBox_sn2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_sn2.Text))
            {
                ShowHint("请先输入单号", Color.Red);
                return;
            }

            string str = textBox_sn2.Text;
            if (str.Contains("&"))
            {
                str = str.Split('&')[0];
            }
            if (Phicomm_WMS.OUTIO.tR_Tr_Sn.DelTrSn(str))
            {
                richTextBox2.AppendText("删除TrSn=" + str + " 的线边仓信息成功!\r\n");
                textBox_sn2.Text = "";
                textBox_sn2.Focus();
            }
            else
            {
                richTextBox2.AppendText("删除TrSn=" + str + " 的线边仓信息失败!\r\n");
                ShowHint("删除TrSn=" + str + " 的线边仓信息失败!", Color.Red);
            }
        }

        private void textBox_stockin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_stockin.Text.Trim()))
            {
                textBox_stockin.BackColor = Color.Red;
                return;
            }

            if (string.IsNullOrEmpty(textBox_stockout.Text.Trim()))
            {
                textBox_stockout.BackColor = Color.Red;
                return;
            }

            if (comboBox_stockout.SelectedIndex < 1)
            {
                comboBox_stockout.BackColor = Color.Red;
                comboBox_stockout.Focus();
                return;
            }

            textBox_stockin.BackColor = Color.White;
            textBox_stockout.BackColor = Color.White;
            comboBox_stockout.BackColor = Color.White;

            LoadData(textBox_stockin.Text.Trim(), textBox_stockout.Text.Trim());
            dataGridView_nuo.ClearSelection();
        }

        private void textBox_stockout_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox_stockout.Text.Trim()))
            {
                textBox_stockout.BackColor = Color.Red;
                textBox_stockout.Focus();
                return;
            }

            textBox_stockin.Text = MyData.GetStockNo();
            if (string.IsNullOrEmpty(textBox_stockin.Text.Trim()))
            {
                textBox_stockin.BackColor = Color.Red;
                textBox_stockin.Focus();
                return;
            }

            if (comboBox_stockout.SelectedIndex < 1)
            {
                comboBox_stockout.BackColor = Color.Red;
                comboBox_stockout.Focus();
                return;
            }

            textBox_stockin.BackColor = Color.White;
            textBox_stockout.BackColor = Color.White;
            comboBox_stockout.BackColor = Color.White;

            LoadData(textBox_stockin.Text.Trim(), textBox_stockout.Text.Trim());
            dataGridView_nuo.ClearSelection();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text.ToUpper().Trim()))
            {
                ShowHint("请扫入五合一条码", Color.Red);
                return;
            }

            if (string.IsNullOrEmpty(textBox_stockin.Text.ToUpper().Trim()))
            {
                ShowHint("请输入入库单号", Color.Red);
                return;
            }

            if (string.IsNullOrEmpty(textBox_stockout.Text.ToUpper().Trim()))
            {
                ShowHint("请输入出库单号", Color.Red);
                return;
            }

            if (comboBox_stockout.SelectedIndex < 1)
            {
                ShowHint("请选择工单类型", Color.Red);
                return;
            }

            if (dataGridView_nuo.Rows.Count == 0)
            {
                ShowHint("请输入入库单号，出库单号并回车！", Color.Red);
                return;
            }

            dataGridView_nuo.ClearSelection();  //清除选中
            textBox1.Enabled = false;
            try
            { 
                //1.检查五合一条码规则
                string str = textBox1.Text.ToUpper().Trim();
                if (!str.Contains("&"))
                {
                    ShowHint("请扫描五合一条码", Color.Red);
                    return;
                }
                if (str.Split('&').Length != 5)
                {
                    ShowHint("请扫描正确的五合一条码", Color.Red);
                    return;
                }
                string[] listStr = str.Split('&');
                string trSn = listStr[0];
                string materialId = listStr[1];
                string factorId = listStr[2];
                string dateCode = listStr[3];
                int qty = int.Parse(listStr[4]);
                string fifoDateCode = DataCodeProcess.fifo_datecode(dateCode.ToUpper().Trim());
                //检查TrSn长度，规则
                if (trSn.Length != 18 && trSn.Length != 13)
                {
                    ShowHint("TrSn长度不正确,应为13或18位，当前TrSn=" + trSn + ",长度：" + trSn.Length, Color.Red);
                    return;
                }
                if (!trSn.All(c => ((c <= '9' && c >= '0') || (c <= 'Z' && c >= 'A'))))
                {
                    ShowHint("TrSn不正确：" + trSn, Color.Red);
                    return;
                }
                if (string.IsNullOrEmpty(fifoDateCode))
                {
                    ShowHint("该物料的fifoDataCode计算出来为0，异常", Color.Red);
                    return;
                }

                //2.检查料号是否在表格范围内
                int i = 0;
                for (; i < dataGridView_nuo.Rows.Count; i++)
                {
                    if (dataGridView_nuo[2, i].Value.ToString().Trim().Equals(materialId))
                    {
                        break;
                    }
                }
                if (i== dataGridView_nuo.Rows.Count)
                {
                    ShowHint("该料号不属于该入库单", Color.Red);
                    return;
                }
                if (string.IsNullOrEmpty(dataGridView_nuo[6, i].Value.ToString().Trim()))
                {
                    ShowHint("该料号属于该入库单，但是不属于出库单", Color.Red);
                    return;
                }

                //3. 检查数量关系
                int in_qty = int.Parse(dataGridView_nuo[3, i].Value.ToString().Trim());
                int in_realqty = int.Parse(dataGridView_nuo[4, i].Value.ToString().Trim());
                int out_qty = int.Parse(dataGridView_nuo[7, i].Value.ToString().Trim());
                int out_realqty = int.Parse(dataGridView_nuo[8, i].Value.ToString().Trim());
                if (in_qty<=in_realqty)
                {
                    ShowHint("该料号已经完成入库，请检查来料数量!", Color.Red);
                    return;
                }
                if (out_qty <= out_realqty)
                {
                    ShowHint("该料号已经完成出库，请检查出库数量!", Color.Red);
                    return;
                }

                //调用储存过程方法绑定，同时完成入库和出库
                int result = 0;
                if (!ReplenishToPickDirectly(MyData.GetStationId(), MyData.GetStockNo(), MyData.GetStockNoType(), MyData.GetStockSubType(), textBox_stockout.Text.Trim(), comboBox_stockout.SelectedIndex,
                    trSn, materialId, dateCode, fifoDateCode, qty, factorId, ref result))
                {
                    return;
                }

                //更新表格数量，本地更新
                dataGridView_nuo[4, i].Value = (in_realqty + qty).ToString();
                dataGridView_nuo[8, i].Value = (out_realqty + qty).ToString();
                //dataGridView_nuo.Rows[i].Selected = true;

                richTextBox3.AppendText("条码：" + textBox1.Text.ToUpper().Trim() + "\r\n" + " 已同时完成入库出库!\r\n");
                richTextBox3.ScrollToCaret();
            }
            catch (Exception ex)
            {
                ShowHint("检查五合一条码： " + ex.Message, Color.Red);
                LoadData(textBox_stockin.Text.Trim(), textBox_stockout.Text.Trim());
                return;
            }
            finally
            {
                textBox1.Enabled = true;
                textBox1.Text = "";
                textBox1.Focus();
            }
        }   
        
        private bool ReplenishToPickDirectly(int stationId, string stockInNo, int stockInType, string subType, string stockOutNo, int stockOutType,
                string trSn, string materialNo, string dateCode, string fifoDc, int qty, string vendorNo, ref int result)
        {
            try
            {
                result = DBPCaller.ReplenishToPickDirectly(stationId, stockInNo, stockInType, subType, stockOutNo, stockOutType,
                                                                      trSn, materialNo, dateCode, fifoDc, qty, vendorNo);
                Trace.WriteLine("ReplenishToPickDirectly 返回 " + result);
                if (result == 1)
                {
                    return true;
                }
                else if (result == -1)
                {
                    ShowHint("返回-1， 该料号不属于当前入库点", Color.Red);
                    return false;
                }
                else if (result == -2)
                {
                    ShowHint("返回-2， 物料基础信息未维护", Color.Red);
                    return false;
                }
                else if (result == -3)
                {
                    ShowHint("返回-3， 物料最大数量维护错误", Color.Red);
                    return false;
                }
                else if (result == -4)
                {
                    ShowHint("返回-4， 当前料状态不是IQC检验完成，不能分拣", Color.Red);
                    return false;
                }
                else if (result == -5)
                {
                    ShowHint("返回-5， 重复条码", Color.Red);
                    return false;
                }
                else if (result == -6)
                {
                    ShowHint("返回-6， 入库单上当前物料已经收完，请检查工单数量", Color.Red);
                    return false;
                }
                else if (result == -7)
                {
                    ShowHint("返回-7， 当前料号不需要出库", Color.Red);
                    return false;
                }
                else if (result == -8)
                {
                    ShowHint("返回-8，工厂编号不存在，请检查sub_type", Color.Red);
                    return false;
                }
                else if (result == -9)
                {
                    ShowHint("返回-9，入库单对应的料号已收完，但还有未更新的数量, 请检查工单数量！", Color.Red);
                    return false;
                }
                else if (result == -10)
                {
                    ShowHint("返回-10， SQL错误", Color.Red);
                    return false;
                }
                else
                {
                    ShowHint("返回：" + result, Color.Red);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowHint("ReplenishToPickDirectly：" + ex.Message, Color.Red);
                return false;
            }
        }             

        private void LoadData(string stockin, string stockout)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            DataTable dt_in1 = new DataTable();
            DataTable dt_in2 = new DataTable();
            DataTable dt_out = new DataTable();
            DataTable dt_level = new DataTable(); //物料等级

            //获取入库数据
            try
            {
                //status=3
                dic.Add("stock_no", stockin); //入库单号
                dic.Add("status", 3);  //要求分拣的物料
                SearchRInventoryId sr = new SearchRInventoryId(dic);
                sr.ExecuteQuery();
                dt_in1 = sr.GetResult();

                //status=4
                dic.Clear();
                dic.Add("stock_no", stockin); //入库单号
                dic.Add("status", 4);  //要求分拣的物料
                sr = new SearchRInventoryId(dic);
                sr.ExecuteQuery();
                dt_in2 = sr.GetResult();

                if ((dt_in1 == null || dt_in1.Rows.Count == 0) &&
                    (dt_in2 == null || dt_in2.Rows.Count == 0))                
                {
                    ShowHint("查询不到该入库单数据的分拣数据，请检查单号，或者检查是否已分拣!", Color.Red);
                    return;
                }                
            }
            catch (Exception ex)
            {
                ShowHint("SearchRInventoryId : " + ex.Message, Color.Red);
                return;
            }

            //获取出库数据
            try
            {
                if (comboBox_stockout.SelectedIndex == 1) //工单发料
                {
                    dic.Clear();
                    dic.Add("woid", stockout);
                    SearchErpWoBomInfo se = new SearchErpWoBomInfo(dic);
                    se.ExecuteQuery();

                    dt_out = se.GetResult();
                    if (dt_out == null || dt_out.Rows.Count == 0)
                    {
                        ShowHint("查询不到该出库单数据，请检查单号!", Color.Red);
                        return;
                    }
                }
                else
                {
                    ShowHint("暂不支持出工单发料以外的出库类型", Color.Red);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return;
            }

            //显示入库数据，物料等级
            int i = 1;
            dataGridView_nuo.Rows.Clear();            
            foreach (DataRow dr in dt_in1.Rows)
            {
                if (string.IsNullOrEmpty(dr["material_no"].ToString().Trim()))
                {
                    ShowHint("入库单上有料号为空，请后台检查！", Color.Red);
                    return;
                }
                
                dataGridView_nuo.Rows.Add(i.ToString(),  //序号0
                                          stockin, //入库单号1
                                          dr["material_no"].ToString(), //入库料号2
                                          dr["qty"].ToString(), //入库数量3
                                          dr["income_qty"].ToString(),//入库实际数量4
                                          "",//出库单号5
                                          "",//出库物料6
                                          "",//出库需求量7
                                          ""//实发量8
                                          );
                i++;
            }
            foreach (DataRow dr in dt_in2.Rows)
            {
                if (string.IsNullOrEmpty(dr["material_no"].ToString().Trim()))
                {
                    ShowHint("入库单上有料号为空，请后台检查！", Color.Red);
                    return;
                }
                dataGridView_nuo.Rows.Add(i.ToString(),  //序号0
                                          stockin, //入库单号1
                                          dr["material_no"].ToString(), //入库料号2
                                          dr["qty"].ToString(), //入库数量3
                                          dr["income_qty"].ToString(),//入库实际数量4
                                          "",//出库单号5
                                          "",//出库物料6
                                          "",//出库需求量7
                                          ""//实发量8
                                          );
                i++;
            }

            //匹配出库数据
            bool findMatch = false;
            try
            {
                for (i = 0; i < dataGridView_nuo.Rows.Count; i++)
                {
                    foreach (DataRow dr in dt_out.Rows)
                    {
                        if (dr["KP_NO"].ToString().Trim().Equals(dataGridView_nuo[2, i].Value.ToString().Trim()))
                        {
                            dataGridView_nuo[5, i].Value = stockout;
                            dataGridView_nuo[6, i].Value = dr["KP_NO"].ToString();
                            dataGridView_nuo[7, i].Value = dr["QTY"].ToString();
                            dataGridView_nuo[8, i].Value = dr["Send_QTy"].ToString();

                            findMatch = true;
                            break;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return;
            }
            
            if (!findMatch)
            {
                ShowHint("入库单与出库单物料号匹配！", Color.Red);
            }     
        }

        private void button_AutoReplenish_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(MyData.GetStockNo()))
            {
                ShowHint("请在分拣页面填写工单，并回车确认！", Color.Red);
                return;
            }

            if (MessageBox.Show("是否叫车？", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)!=DialogResult.Yes)
            {
                return;
            }

            textBox_BarCode.Enabled = false;
            button_AutoReplenish.Enabled = false;
            button_HumanReplenish.Enabled = false;

            //叫车，自动入库
            CallAgv(MyData.GetStationId(), MyData.GetStockNo());
            UpdateHolderInfo();

            button_AutoReplenish.Enabled = true;
            button_HumanReplenish.Enabled = true;
            textBox_BarCode.Enabled = true;
            textBox_BarCode.Focus();
        }

        private void button_HumanReplenish_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(MyData.GetStockNo()))
            {
                ShowHint("请在分拣页面填写工单，并回车确认！", Color.Red);
                return;
            }

            if (MessageBox.Show("是否手动入库？", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            textBox_BarCode.Enabled = false;
            button_AutoReplenish.Enabled = false;
            button_HumanReplenish.Enabled = false;

            //手动入库
            CreateHumanTask(MyData.GetStationId(), MyData.GetStockNo());
            UpdateHolderInfo();

            button_AutoReplenish.Enabled = true;
            button_HumanReplenish.Enabled = true;
            textBox_BarCode.Enabled = true;
            textBox_BarCode.Focus();
        }

        private void comboBox_staus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(MyData.GetStockNo()) ||
                string.IsNullOrEmpty(MyData.GetStockSubType()))
            {
                return;
            }

            DataTable dt2 = GetData(MyData.GetStockNo(), (comboBox_staus.SelectedIndex + 3).ToString());
            ShowData2(dataGridView2, dt2);
        }
    }
}
