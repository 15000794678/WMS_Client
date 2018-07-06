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

namespace Phicomm_WMS.UI
{
    public partial class PickFrm : Office2007Form
    {
        private Dictionary<int, Dictionary<Label, Color>> _dicStockCount = new Dictionary<int, Dictionary<Label, Color>>();
        //private bool stopStockCount = true;
        private AutoResetEvent eventFindShelf = new AutoResetEvent(false);
        private bool findStockCount = false; //找到盘点货架
        private int _lastStockCountSelect = 0;
        private string _lastStockCountLoc = string.Empty;
        private bool _lastScanLoc = false;

        private void InitStockCountTab()
        {
            comboBox_pd.SelectedIndex = 0;

            label_pd_locid.Text = "";
            label_pd_kpno.Text = "";
            label_pd_qty.Text = "";
            label_pd_count.Text = "";
            label_pd_plant.Text = "";
            label_pd_stockid.Text = "";
            label_pd_ShelfId.Text = "";
            label_pd_ShelfCnt.Text = "";

            //初始化出库页面货架信息
            _dicStockCount.Clear();
            tableLayoutPanel_stockcount.Controls.Clear();
            for (int i = 0; i < 20; i++)
            {
                int index = (5 - i / 4) * 10 + (i % 4 + 1);

                Label lb = new Label();
                lb.Name = "Label_pd_ShelId" + i.ToString();
                FontFamily myFontFamily = new FontFamily("幼圆"); //采用哪种字体
                lb.Font = new Font(myFontFamily, 30, FontStyle.Regular);
                //lb.Text = index.ToString();
                lb.AutoSize = false;
                lb.Dock = DockStyle.Fill;
                lb.TextAlign = ContentAlignment.MiddleCenter;
                lb.Margin = new System.Windows.Forms.Padding(3);
                lb.BackColor = Color.DarkGray;

                tableLayoutPanel_stockcount.Controls.Add(lb);

                Dictionary<Label, Color> dicColor = new Dictionary<Label, Color>();
                dicColor.Add(lb, Color.DarkGray);

                if (_dicShelfId.ContainsKey(index))
                {
                    _dicStockCount[index] = dicColor;
                }
                else
                {
                    _dicStockCount.Add(index, dicColor);
                }    
            }

            dataGridView_stockcount.Rows.Clear();
            richTextBox_stockcount.Clear();

            Thread t1 = new Thread(StockCountThread);
            t1.Start();
        }

        private void button_stockcount_Click(object sender, EventArgs e)
        {
            if (button_stockcount.Text.Trim().Equals("开始盘点"))
            {              
                if (CreateStockCountTask())
                {
                    button_stockcount.Text = "停止盘点";
                    AddLog("开始盘点");

                    findStockCount = false;
                    eventFindShelf.Set();                    
                }               
            }
            else
            {//停止盘点
                findStockCount = true;
                eventFindShelf.Set();
                button_stockcount.Text = "开始盘点";
                AddLog("停止盘点");
            }
        }

        private bool CreateStockCountTask()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            DataTable dt = new DataTable();
            string date = string.Format("{0:D4}{1:D2}{2:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day); ;
            string plant = "";
            string stock_id = "";
            //string woid = "";
            int qty = 0;
            try
            {
                SearchMaterialPickAssign sm = new SearchMaterialPickAssign(dic);
                sm.ExecuteQuery();

                dt = sm.GetResult();
                if (dt != null && dt.Rows.Count > 0)
                {
                    date = dt.Rows[0]["StockOutNo"].ToString().Trim();
                    qty = int.Parse(dt.Rows[0]["Qty"].ToString().Trim());
                    if (qty != -1)                    
                    {
                        ShowHint("请先完成单号：" + woid + " 的出库任务", Color.Red);
                        return false;
                    }
                }

                if (qty==-1)   //存在盘点任务
                {
                    dic.Clear();
                    dic.Add("check_date", date);
                    dic.Add("status", "0");
                    SearchInventoryCheckByMaterialNo sc = new SearchInventoryCheckByMaterialNo(dic);
                    sc.ExecuteQuery();
                    dt = sc.GetResult();
                    if (dt!=null && dt.Rows.Count>0)
                    {
                        plant = dt.Rows[0]["Plant"].ToString();
                        stock_id = dt.Rows[0]["Stock_Id"].ToString();
                        string kpno = dt.Rows[0]["KpNo"].ToString();
                        if (kpno.Trim().Equals("*"))
                        {
                            ShowHint("请先完成：" + date + " 的全部盘点任务！", Color.Red);
                            comboBox_pd.SelectedIndex = 1;
                        }
                        else
                        {
                            ShowHint("请先完成：" + date + " 的料号盘点任务！", Color.Red);
                            comboBox_pd.SelectedIndex = 2;
                        }
                    }
                    else
                    {
                        ShowHint("请先完成：" + date + " 的储位盘点任务！", Color.Red);
                        comboBox_pd.SelectedIndex = 3;
                    }

                    return true;
                }

                if (comboBox_pd.SelectedIndex == 0)
                {
                    ShowHint("请选择一个盘点类型!", Color.Red);
                    comboBox_pd.Focus();
                    return false;
                }
                else if (comboBox_pd.SelectedIndex == 1) //全部盘点
                {
                    using (SetStockDate ss = new SetStockDate())
                    {
                        if (ss.ShowDialog() != DialogResult.OK)
                        {
                            return false;
                        }

                        date = ss.GetDate();
                        plant = ss.GetPlant();
                        stock_id = ss.GetStockId();
                    }
                }                
                else if (comboBox_pd.SelectedIndex == 2)//料号盘点，导入数据
                {                   
                    using (ImportKpNoData ic = new ImportKpNoData())
                    {
                        if (ic.ShowDialog()!=DialogResult.OK)
                        {
                            return false;
                        }

                        date = ic.GetWoId();
                        plant = ic.GetPlant();
                        stock_id = ic.GetStockId();
                    }

                    //查询导入数据是否需要盘点，status=0
                    try
                    {
                        dic.Clear();
                        dic.Add("check_date", date);
                        dic.Add("plant", plant);
                        dic.Add("stock_id", stock_id);
                        dic.Add("status", "0");
                        SearchInventoryCheckByMaterialNo sc = new SearchInventoryCheckByMaterialNo(dic);
                        sc.ExecuteQuery();
                        dt = sc.GetResult();
                        if (dt == null || dt.Rows.Count == 0)
                        {
                            ShowHint("无料号需要盘点", Color.Red);                    
                            return false;
                        }                        
                    }
                    catch (Exception ex)
                    {
                        ShowHint("SearchInventoryCheckByMaterialNo:" + ex.Message, Color.Red);
                        return false;
                    }
                }
                else if (comboBox_pd.SelectedIndex==3)
                {
                    using (ImportLocIdData ic = new ImportLocIdData())
                    {
                        if (ic.ShowDialog() != DialogResult.OK)
                        {
                            return false;
                        }

                        date = ic.GetWoId();
                        plant = ic.GetPlant();
                        stock_id = ic.GetStockId();
                    }

                    //查询导入数据是否需要盘点，status=0
                    try
                    {
                        dic.Clear();
                        dic.Add("check_date", date);
                        dic.Add("plant", plant);
                        dic.Add("stock_id", stock_id);
                        dic.Add("status", "0");
                        SearchInventoryCheckByLocId sc = new SearchInventoryCheckByLocId(dic);
                        sc.ExecuteQuery();
                        dt = sc.GetResult();
                        if (dt == null || dt.Rows.Count == 0)
                        {
                            ShowHint("无料号需要盘点", Color.Red);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowHint("SearchInventoryCheckByLocId:" + ex.Message, Color.Red);
                        return false;
                    }
                }
                
                //生成盘点任务               
                string result = DBPCaller.CreateCheckTask(date, comboBox_pd.SelectedIndex - 1);
                if (!result.ToUpper().Trim().Equals("OK"))                
                {
                    AddLog("生成任务失败:" + result);
                    ShowHint("生成盘点任务失败:" + result, Color.Red);
                    return false;
                }
                AddLog("盘点日期：" + date);                           
            }
            catch (Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return false;
            }

            try
            {
                SearchTaskCount st = new SearchTaskCount(2, MyData.GetStationId());
                st.ExecuteQuery();

                int result = st.GetResult();

                label_pd_ShelfCnt.Text = result.ToString();

                if (result > 0)
                {
                    AddLog("生成盘点任务成功!");
                    ShowHint("生成盘点任务成功!", Color.Lime);
                    return true;
                }
                else
                {
                    AddLog("生成任务失败，不需要搬运货架！");
                    ShowHint("生成任务失败，不需要搬运货架！", Color.Red);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowHint("获取货架数目异常: " + ex.Message, Color.Red);
                return false;
            }
        }

        private void StockCountThread()
        {
            while (runFlag)
            {
                eventFindShelf.WaitOne();
                if (!runFlag) break;

                while (!findStockCount)
                {
                    GetShelfInfo();

                    if (findStockCount) break;

                    if (!runFlag) return;

                    Thread.Sleep(100);
                }
            }            
        }

        private void GetShelfInfo()
        {
            try
            {
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                int cnt = 0;

                dt1 = DBPCaller.AtStationPod(MyData.GetStationId());
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    //打印货架信息
                    Trace.WriteLine("Debug: PodId=" + dt1.Rows[0]["PodId"].ToString() +
                                    ", PodName=" + dt1.Rows[0]["PodName"].ToString() +
                                    ", Row=" + dt1.Rows[0]["Row"].ToString() +
                                    ", Column=" + dt1.Rows[0]["Column"].ToString()
                                    );

                    dt2 = DBPCaller.PickAtStationLoc(MyData.GetStationId());
                    _dtShelf = dt2;

                    if (dt2 != null && dt2.Rows.Count > 0)
                    {
                        //打印出货储位等信息
                        Trace.WriteLine("Debug: StockNo=" + dt2.Rows[0]["StockNo"].ToString() +
                                        ",ShowText=" + dt2.Rows[0]["ShowText"].ToString() +
                                        ",PodId=" + dt2.Rows[0]["PodId"].ToString() +
                                        ",PodSide=" + dt2.Rows[0]["PodSide"].ToString() +
                                        ",ShelfId=" + dt2.Rows[0]["ShelfId"].ToString() +
                                        ",BoxBarcode=" + dt2.Rows[0]["BoxBarcode"].ToString() +
                                        ",BoxId=" + dt2.Rows[0]["BoxId"].ToString() +
                                        ",MaterialId=" + dt2.Rows[0]["MaterialId"].ToString() +
                                        ",MaterialName=" + dt2.Rows[0]["MaterialName"].ToString() +
                                        ",Qty=" + dt2.Rows[0]["Qty"].ToString() +
                                        ",AllOut=" + dt2.Rows[0]["AllOut"].ToString()
                                        );
                    }

                    if (runFlag)
                    {
                        UpdateStockCountUI(cnt, int.Parse(dt1.Rows[0]["Column"].ToString()), _dtShelf);
                    }
                }
                else
                {
                    if (runFlag)
                    {
                        UpdateStockCountUI(0, 0, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("RefreshUI:" + ex.Message);
                //ShowHint("RefreshUI:" + ex.Message, Color.Red);
                Trace.WriteLine("Debug: QueryShelfInformation : " + ex.Message);
            }
        }

        private void HighLightStockCount(int index, Color cl, int cnt)
        {
            if (_dicStockCount.ContainsKey(index))
            {
                Dictionary<Label, Color> dicColor = _dicStockCount[index];
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

        private void UpdateStockCountUI(int cnt, int col, DataTable dt)
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
                            HighLightStockCount(_lastStockCountSelect, Color.DarkGray, 0);
                            
                            label_pd_locid.Text = "";
                            label_pd_kpno.Text = "";
                            label_pd_qty.Text = "";
                            label_pd_count.Text = "";
                            label_pd_plant.Text = "";
                            label_pd_stockid.Text = "";
                            //label_pd_locid.BackColor = Color.Lime;
                            tableLayoutPanel23.BackColor = Color.Transparent;

                            dataGridView_stockcount.Rows.Clear();

                            textBox_stockcount.Focus();
                            
                            Trace.WriteLine("Debug: ------RefreshUI Stop------");

                            return;
                        }
                       
                        //不需要更新
                        if (_lastStockCountLoc.Equals(dt.Rows[0]["BoxBarcode"].ToString().Trim()))
                        {
                            return;
                        }

                        //需要更新
                        _lastStockCountLoc = dt.Rows[0]["BoxBarcode"].ToString().Trim();
                        label_pd_locid.Text = _lastStockCountLoc;
                        HighLightStockCount(_lastStockCountSelect, Color.DarkGray, 0);

                        label_pd_ShelfId.Text = dt.Rows[0]["PodId"].ToString().Trim() + ",    " +
                                            (dt.Rows[0]["PodSide"].ToString().Trim().Equals("0") ? "正面" : "反面");

                        //高亮显示
                        tableLayoutPanel23.BackColor = Color.Transparent;
                        label_pd_count.BackColor = Color.Transparent;
                        textBox_stockcount.Focus();
                        _lastScanLoc = false;

                        #region 刷新货架信息
                        if (col == 2)
                        {
                            foreach (KeyValuePair<int, Dictionary<Label, Color>> obj in _dicStockCount)
                            {                                
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
                            foreach (KeyValuePair<int, Dictionary<Label, Color>> obj in _dicStockCount)
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

                        _lastStockCountSelect = int.Parse(_lastStockCountLoc.Substring(_lastStockCountLoc.Length - 2, 2));                        
                        HighLightStockCount(_lastStockCountSelect, Color.Lime, cnt);

                        UpdateStockCountData(_lastStockCountLoc);

                        //保存工单号
                        MyData.SetStockNo(dt.Rows[0]["StockNo"].ToString().Trim());

                        findStockCount = true;

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

        private void UpdateStockCountData(string locid)
        {            
            try
            {                
                //查询TrSn
                Dictionary<string, object> dic = new Dictionary<string, object>();

                dic.Add("loc_id", locid);
                dic.Add("status", "1");

                SearchRInventoryDetail sb = new SearchRInventoryDetail(dic);
                sb.ExecuteQuery();
                DataTable dt = sb.GetResult();

                if (dt == null || dt.Rows.Count == 0)
                {
                    label_pd_kpno.Text = "";
                    label_pd_qty.Text = "";
                    label_pd_count.Text = "";
                    label_pd_plant.Text = "";
                    label_pd_stockid.Text = "";
                    dataGridView_stockcount.Rows.Clear();
                    return;
                }

                label_pd_kpno.Text = dt.Rows[0]["KP_NO"].ToString();                
                label_pd_plant.Text = dt.Rows[0]["PLANT"].ToString();
                label_pd_stockid.Text = dt.Rows[0]["STOCK_ID"].ToString();

                int qty = 0;
                int count = 0;
                dataGridView_stockcount.Rows.Clear();
                try
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        count++;
                        qty += int.Parse(dr["QTY"].ToString());
                        dataGridView_stockcount.Rows.Add(false,
                                                         count.ToString(),
                                                         dr["TR_SN"].ToString(),
                                                         dr["DATE_CODE"].ToString(),
                                                         dr["QTY"].ToString());
                    }

                    label_pd_qty.Text = qty.ToString();
                    label_pd_count.Text = count.ToString();
                    dataGridView_stockcount.ClearSelection();
                }
                catch(Exception ex)
                {
                    ShowHint(ex.Message, Color.Red);
                    return;
                }
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return;
            }
        }

        private void AddLog(string str)
        {
            richTextBox_stockcount.ScrollToCaret();
            richTextBox_stockcount.AppendText(str + "\r\n");            
            textBox_stockcount.Focus();
        }

        private void textBox_stockcount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(textBox_stockcount.Text.ToUpper().Trim()))
                {
                    ShowHint("请输入条码!", Color.Red);
                    return;
                }

                string str = textBox_stockcount.Text.ToUpper().Trim();
                if (str.Equals("F5"))
                {
                    AgvLeave(MyData.GetStationId());
                    //获取货架信息
                    UpdateStockCountShelfCnt(2, MyData.GetStationId());
                    AddLog("小车离开成功！");
                    return;
                }

                if (string.IsNullOrEmpty(label_pd_locid.Text) || string.IsNullOrEmpty(label_pd_kpno.Text))
                {
                    ShowHint("请先等待小车到达，再扫入条码!", Color.Red);
                    return;
                }

                if (str.Equals("F7")) //成功跳到下一个储位
                {
                    if (_lastScanLoc == false)
                    {
                        ShowHint("请先扫描储位编号!", Color.Red);
                        return;
                    }

                    if (!ScanTrSnDone())
                    {
                        ShowHint("当前还有物料未扫完!", Color.Red);             
                        return;
                    }

                    //上传信息
                    if (UploadCheckData(int.Parse(label_pd_qty.Text.Trim()), int.Parse(label_pd_count.Text.Trim())))
                    {
                        StartFindStockCountShelf();
                    }
                    return;
                }
                else if (str.Equals("F8")) //失败跳到下一个储位
                {
                    if (_lastScanLoc == false)
                    {
                        ShowHint("请先扫描储位编号!", Color.Red);
                        return;
                    }
                    using (SetStockCount ss = new SetStockCount())
                    {
                        ss.SetLocId(label_pd_locid.Text.Trim());
                        ss.SetKpNo(label_pd_kpno.Text.Trim());
                        ss.SetQty(label_pd_qty.Text.Trim());
                        ss.SetCount(label_pd_count.Text.Trim());
                        ss.SetPlant(label_pd_plant.Text.Trim());
                        ss.SetStockId(label_pd_stockid.Text.Trim());

                        if (ss.ShowDialog()==DialogResult.OK)
                        {
                            int realqty = ss.GetRealQty();
                            int realcount = ss.GetRealCount();

                            //上传信息
                            if (UploadCheckData(realqty, realcount))
                            {
                                StartFindStockCountShelf();
                            }
                        }
                    }
                    return;                   
                }
                else if (str.Equals("F9"))
                {
                    StartFindStockCountShelf();
                    return;
                }
                else if (str.Equals("F10"))
                {
                    StopFindStockCountShelf();
                    return;
                }
                else if (str.Contains("&"))
                {
                    if (str.Split('&').Length == 5)
                    {
                        str = str.Split('&')[0];
                    }
                    else
                    {
                        ShowHint("请扫描正确的五合一条码!", Color.Red);
                        return;
                    }
                }

                //检查条码类型
                int result = 0;
                if (!CheckBarcode(str, ref result))
                {
                    return;
                }

                if (result == 1 /*储位编号*/)
                {
                    if (!str.Equals(_lastStockCountLoc))
                    {
                        ShowHint("请扫正确的储位编号！", Color.Red);
                        return;
                    }

                    _lastScanLoc = true;
                    tableLayoutPanel23.BackColor = Color.Yellow;
                    AddLog("当前储位：" + _lastStockCountLoc);
                }
                else if (result == 3 /*TR_SN*/)
                {
                    if (_lastScanLoc == false)
                    {
                        ShowHint("请先扫描储位编号!", Color.Red);
                        return;
                    }

                    AddLog("当前TrSn：" + str);
                    if (SelectTrSn(str))  //已扫描过的在本地进行标记
                    {
                        if (ScanTrSnDone())  //全部扫描完毕
                        {
                            //上传信息
                            if (UploadCheckData(int.Parse(label_pd_qty.Text.Trim()), int.Parse(label_pd_count.Text.Trim())))
                            {
                                StartFindStockCountShelf();
                            }
                        }
                    }                    
                }
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
                return;
            }
            finally
            {
                textBox_stockcount.Text = "";
                textBox_stockcount.Focus();
            }
        }

        private bool UpdateStockCountShelfCnt(int id, int stationId)
        {
            try
            {
                SearchTaskCount st = new SearchTaskCount(id, stationId);
                st.ExecuteQuery();

                int result = st.GetResult();

                //if (this.IsHandleCreated)
                //{
                //    this.Invoke(new EventHandler(delegate
                //    {
                        label_pd_ShelfCnt.Text = result.ToString();
                //    }));
                //}

                if (result == 0)
                {
                    findStockCount = true; //停止查找货架    
                    button_stockcount.Text = "开始盘点";
                    ShowHint("该站点所有的盘点任务已经完成!", Color.Lime);
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowHint("UpdateShelfCount: " + ex.Message, Color.Red);
                return false;
            }
        }

        private void StartFindStockCountShelf()
        {
            findStockCount = false;
            eventFindShelf.Set();
        }

        private void StopFindStockCountShelf()
        {
            findStockCount = true;
        }

        //标记扫入的trSn
        private bool SelectTrSn(string trSn)
        {
            for(int i=0; i<dataGridView_stockcount.Rows.Count; i++)
            {
                if (dataGridView_stockcount[2, i].Value.ToString().Equals(trSn))
                {                    
                    ((DataGridViewCheckBoxCell)dataGridView_stockcount.Rows[i].Cells["StockCountCheck"]).Value = true;
                    return true;
                }
            }

            ShowHint("该trSn不在该储位!", Color.Red);
            AddLog("该trSn不在该储位!");
            return false;
        }

        //检查trSn是否扫描完毕
        private bool ScanTrSnDone()
        {
            int count = 0;
            for (int i = 0; i < dataGridView_stockcount.Rows.Count; i++)
            {
                if (dataGridView_stockcount.Rows[i].Cells["StockCountCheck"].EditedFormattedValue.ToString().Equals("True"))
                {
                    count++;
                }
            }

            if (count == 0 || count == dataGridView_stockcount.Rows.Count)
            {
                return true;
            }

            AddLog("当前还有： " + (dataGridView_stockcount.Rows.Count-count).ToString() + " 盘物料未扫描!");
            //ShowHint("当前还有： " + (dataGridView_stockcount.Rows.Count - count).ToString() + " 盘物料未扫描!", Color.Red);
            return false;
        }

        private bool UploadCheckData(int realqty, int realcount)
        {
            try
            {
                string locid = label_pd_locid.Text.Trim();
                string kpno = label_pd_kpno.Text.Trim();
                int qty = int.Parse(label_pd_qty.Text.Trim());
                int count = int.Parse(label_pd_count.Text.Trim());
                string plant = label_pd_plant.Text.Trim();
                string stockid = label_pd_stockid.Text.Trim();

                string res = DBPCaller.CompleteLocCheck(locid, kpno, qty, count, realqty, realcount, plant, stockid, MyData.GetUser(), MyData.GetStockNo());
                if (res.ToUpper().Trim().Equals("OK"))
                {
                    AddLog("绑定成功!");
                    return true;
                }

                AddLog("绑定失败：" + res);
                ShowHint("绑定失败：CompleteLocCheck返回：" + res, Color.Red);
                return false;
            }
            catch(Exception ex)
            {
                AddLog("绑定异常： " + ex.Message);
                ShowHint(ex.Message, Color.Red);
                return false;
            }
        }

        private void 导出数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ExportCountData es = new ExportCountData())
            {
                es.ShowDialog();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            DataTable dt = new DataTable();
            string woid = string.Empty;

            try
            {                
                SearchMaterialPickAssign sm = new SearchMaterialPickAssign(dic);
                sm.ExecuteQuery();
                dt = sm.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("无盘点任务需要暂停!", Color.Red);
                    return;
                }

                woid = dt.Rows[0]["StockOutNo"].ToString();
            }
            catch (Exception ex)
            {
                ShowHint("SearchMaterialPickAssign:" + ex.Message, Color.Red);
                return;
            }

            //暂停任务
            if (MessageBox.Show("确认暂停日期：" + woid + " 的所有盘点任务？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                StopPickTask sp = new StopPickTask(woid);
                sp.ExecuteQuery();

                ShowHint("暂停盘点任务成功!", Color.Lime);
            }
            catch (Exception ex)
            {
                ShowHint("暂停盘点任务失败:" + ex.Message, Color.Red);
                return;
            }            
        }
    }
}
