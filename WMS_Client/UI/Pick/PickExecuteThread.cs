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
using RefWebService_BLL;

namespace WMS_Client.UI
{
    public partial class PickFrm : Office2007Form
    {
        //条码输入处理线程
        private void ExecuteThread()
        {
            while (runFlag)
            {
                try
                {
                    eventExecuteInput.WaitOne();  //等待输入到来
                    if (!runFlag) return;

                    //禁止输入
                    EnableInput(false);

                    if (textBox_BarCode.Text.Trim().ToUpper().Equals("F5"))  //小车离开
                    {
                        if (AgvLeave(MyData.GetStationId()))
                        {
                            UpdateShelfCount(2, MyData.GetStationId());
                        }
                    }
                    else if (textBox_BarCode.Text.Trim().ToUpper().Equals("F9")) //查找货架信息，找到则停止，找不到则一直继续
                    {
                        StartFindShelf();
                    }
                    else if (textBox_BarCode.Text.Trim().ToUpper().Equals("F10"))
                    {
                        StopFindShelf();
                    }
                    else  //其他条码输入
                    {
                        if (MyData._lastArrive.select == 0)
                        {
                            ShowHint("请等待车辆到达后，再扫入条码", Color.Lime);
                            continue;
                        }

                        if (MyData.GetStockNoType() == 0)
                        {
                            ShowHint("请至下载页面选择出库类型，输入单号!", Color.Red);
                            continue;
                        }

                        ExecuteInput(textBox_BarCode.Text.ToUpper().Trim());
                    }
                }
                catch (Exception ex)
                {
                    ShowHint("" + ex.Message, Color.Red);
                }
                finally
                {
                    //打开输入
                    EnableInput(true);
                }
            }
        }

        private void ExecuteInput(string str)
        {
            try
            {
                int splitnum = 0; //分盘数量
                int result = 0;
                bool saveTrSnFlag = false;
                List<string> listTrSn = new List<string>();

                //本地检查并保存五合一条码
                if (str.Contains("&"))
                {
                    if (!CheckMaterialTrSn(str))
                    {
                        ShowHint("当前输入的五合一条码不满足规则：" + str, Color.Red);
                        return;
                    }
                    listTrSn.Add(str.Split('&')[0]);
                }
                else
                {
                    listTrSn.Add(str);
                }

                //远程检查条码类型
                if (!CheckBarcode(listTrSn[0].ToUpper(), ref result))
                {
                    return;
                }

                if (result == 1) //储位
                {
                    if (!str.Equals(MyData._lastArrive.locid))
                    {
                        _allOutCnt = 0;
                        ShowHint("储位号错误，请扫描正确的储位号！", Color.Red);
                        return;
                    }

                    UpdateCnt(MyData.GetStockNo(), MyData._lastArrive.locid);  //扫完储位后刷新数量

                    _lastScanLocId = str;
                    if (MyData._lastArrive.allout.Equals("1"))
                    {
                        if (_allOutCnt == 0)
                        {
                            _allOutCnt++;
                            ShowMessage("再扫一次储位号全盘出料");
                            ShowQty();  //扫过一次储位后跳到数量
                        }
                        else
                        {
                            _allOutCnt = 0;

                            try
                            {
                                //保存所有的trSn
                                listTrSn = DBFunc.SearchTrSnFromRInventoryDetailByLocId(_lastScanLocId);
                                if (listTrSn == null || listTrSn.Count == 0)
                                {
                                    ShowHint("整盘出库失败，找不到该储位对应的TrSn", Color.Red);
                                    return;
                                }
                            }
                            catch(Exception ex)
                            {
                                ShowHint("SearchTrSnFromRInventoryDetailByLocId exception:" + ex.Message, Color.Red);
                                return;
                            }

                            //全盘出料
                            if (!PartialPickMaterialAllOut(_lastScanLocId, ref result))
                            {
                                return;
                            }

                            saveTrSnFlag = true;
                        }
                    }
                    else
                    {
                        //高亮数量
                        ShowQty();  //不是全盘出料时, 扫描储位后转为闪烁数量
                        ShowMessage("请扫描物料条码");
                    }
                }
                else if (result == 3) //物料编号
                {
                    _allOutCnt = 0;
                    if (!_lastScanLocId.Equals(MyData._lastArrive.locid))
                    {
                        ShowHint("请先扫描储位号！", Color.Red);
                        return;
                    }

                    _allOutCnt = 0;

                    //一盘一盘扫
                    if (!PartialPickMaterial(_lastScanLocId, listTrSn[0].ToUpper(), ref splitnum, ref result))
                    {
                        return;
                    }
                   
                    saveTrSnFlag = true;
                }
                else
                {
                    _allOutCnt = 0;
                }

                //取料成功
                if (saveTrSnFlag)
                {                                  
                    //如果要分盘
                    if (result == 3 && listTrSn.Count==1)  
                    {
                        if (splitnum > 0)
                        {
                            string newTrSn = refWebtR_Tr_Sn.Instance.Get_tr_sn_current(1); //获取一个新的trSn

                            if (!CompleteReelSplit(listTrSn[0].ToUpper(), newTrSn, splitnum, ref result))
                            {
                                return;
                            }

                            SplitTrSn(listTrSn[0].ToUpper(), newTrSn, splitnum);

                            //要推送的trSn是新生成的trSn
                            listTrSn.Clear();
                            listTrSn.Add(newTrSn);
                        }
                    }

                    //更新货架信息
                    StartFindShelf();

                    //保存TrSn
                    SaveTrSn(listTrSn, MyData.GetStockNo(), MyData.GetStockNoType());                    

                    UpdateCnt(MyData.GetStockNo(), _lastScanLocId);

                    //全部出库完毕，则过账
                    if (result == 1) /*料出库成功，且所有站点物料全部出库，自动过账*/
                    {
                        findFlag = true; //不再刷新
                        if (MyData.GetStockNoType() == (int)MyData.PickWoType.Normal ||
                            MyData.GetStockNoType() == (int)MyData.PickWoType.Super)
                        {
                            PushTrSnByStockNo(MyData.GetStockNo());
                        }
                        //SapPick(MyData.GetStockNo(), MyData.GetStockNoType();
                        ShowHint("出库过账功能暂时关闭！", Color.Lime);                       
                    }                    
                }
            }
            catch (Exception ex)
            {
                Log.Error("ExecuteInput:" + ex.Message);

                UpdateCnt(MyData.GetStockNo(), _lastScanLocId);

                //更新货架信息
                StartFindShelf();
            }
        }

        private bool SplitTrSn(string trSn, string trSn2, int sendQty)
        {
            using (SplitMaterial sm = new SplitMaterial())
            {
                sm.SetLabel(trSn, trSn2, sendQty);

                if (sm.ShowDialog() != DialogResult.OK)
                {
                    ShowHint("打印失败, OldTrSn=" + trSn + ", NewTrSn=" + trSn2 + ", SendQty=" + sendQty.ToString() + ", 请记录该信息，并至条码补印页面补印条码！", Color.Red);
                    return false;
                }

                return true;
            }
        }

        private bool CompleteReelSplit(string trSn, string trSn2, int sendQty, ref int result)
        {
            try
            {
                CompleteReelSplit cr = new CompleteReelSplit(MyData.GetStationId(), MyData.GetStockNo(), MyData.GetStockNoType(), trSn, trSn2, sendQty);
                cr.ExecuteQuery();

                result = cr.GetResult();
                if (result==0 || result==1 || result==2)
                {
                    return true;
                }
                else if (result==-1)
                {
                    ShowHint("CompleteReelSplit返回-1， 当前TrSn所在的储位不需要出库, 请联系管理员!", Color.Red);
                    return false;
                }
                else if (result == -2)
                {
                    ShowHint("CompleteReelSplit返回-2， 发出数量大于等于当前盘物料数量，不满足拆盘条件, 请联系管理员!", Color.Red);
                    return false;
                }
                else 
                {
                    ShowHint("CompleteReelSplit返回" + result + ", 请联系管理员!", Color.Red);
                    return false;
                }
            }
            catch(Exception ex)
            {
                ShowHint("CompleteReelSplit: " + ex.Message, Color.Red);
                return false;
            }
        }

        //检查五合一条码规则
        private bool CheckMaterialTrSn(string str)
        {
            //规则：200125170816001774&805000769&200125&1729&2000
            if (!str.Contains("&"))
            {
                ShowHint("扫入条码不符合五合一规则", Color.Red);
                return false;
            }

            string[] listStr = str.Split('&');
            if (listStr.Length != 5)
            {
                ShowHint("扫入条码不符合五合一规则", Color.Red);
                return false;
            }

            if (listStr[0].Length!=13 && listStr[0].Length!=18)
            {
                ShowHint("TrSn必须为13或18为，当前为：" + listStr[0].Length + " 位", Color.Red);
                return false;
            }

            if (!listStr[0].All(c => ((c <= '9' && c >= '0') || (c <= 'Z' && c >= 'A'))))
            {
                ShowHint("TrSn不正确：" + listStr[0], Color.Red);
                return false;
            }

            return true;
        }

        /*
        -3: 不是当前储位的物料
        -2: 储位和料号的绑定关系错误
        -1: 当前料号已分拣
        0: 料出库成功，且当前站点还有未出库的料
        1: 料出库成功，且所有站点物料全部出库
        2: 当前站点分配的出库任务已完成，但是其他站点还有物料未出库
        */
        public bool PickMaterial(string locId, string barcode, ref int result)  //type=0,五合一条码，1=TrSn, 2=全盘出
        {
            try
            {
                List<string> listTrSn = new List<string>();
                string trSn = barcode;

                if (barcode.Contains("&"))
                {
                    trSn = barcode.Split('&')[0];
                }
                listTrSn.Add(trSn);

                Trace.WriteLine("Debug:----PickMaterial Start----");
                int res = DBPCaller.CompletePickMaterialByTrSn(MyData.GetStationId(),
                                                        MyData._lastArrive.stockno,
                                                        locId,
                                                        trSn,
                                                        MyData.GetStockNoType());
                Trace.WriteLine("Debug:----PickMaterial Stop----");
                Log.Error("CompletePickMaterialByTrSn返回：" + res);
                Trace.WriteLine("Debug: CompletePickMaterialByTrSn返回：" + res);
                if (res == 0 || res == 1 || res == 2)
                {
                    result = res;
                    return true;
                }
                else
                {
                    ShowHint("CompletePickMaterialByTrsn未定义的返回：" + res.ToString(), Color.Red);
                }

                return false;
            }
            catch (Exception ex)
            {
                result = -99;
                Log.Error("CompletePickMaterialByTrsn异常，返回：" + ex.Message);
                ShowHint("CompletePickMaterialByTrsn异常，返回：" + ex.Message, Color.Red);
                return false;
            }
        }

        public bool PartialPickMaterial(string locId, string barcode, ref int num, ref int result)  //type=0,五合一条码，1=TrSn, 2=全盘出
        {
            try
            {
                List<string> listTrSn = new List<string>();
                string trSn = barcode;

                if (barcode.Contains("&"))
                {
                    trSn = barcode.Split('&')[0];
                }
                listTrSn.Add(trSn);

                Trace.WriteLine("Debug:----PartilPickMaterial Start----");
                int res = DBPCaller.CompletePartialPickByTrSn(MyData.GetStationId(),
                                                        MyData._lastArrive.stockno,
                                                        locId,
                                                        trSn,
                                                        MyData.GetStockNoType(),
                                                        ref num);
                Trace.WriteLine("Debug:----PartilPickMaterial Stop----");
                Log.Error("CompletePartialPickByTrSn返回：" + res);
                Trace.WriteLine("Debug: CompletePartialPickByTrSn返回：" + res);
                if (res == 0 || res == 1 || res == 2 || res==3)
                {
                    result = res;
                    return true;
                }
                else
                {
                    ShowHint("CompletePartialPickByTrSn未定义的返回：" + res.ToString(), Color.Red);
                }

                return false;
            }
            catch (Exception ex)
            {
                result = -99;
                Log.Error("CompletePartialPickByTrSn异常，返回：" + ex.Message);
                ShowHint("CompletePartialPickByTrSn异常，返回：" + ex.Message, Color.Red);
                return false;
            }
        }

        //整盘出库
        public bool PickMaterialAllOut(string locId, ref int result)  //type=0,五合一条码，1=TrSn, 2=全盘出
        {
            try
            {
                Trace.WriteLine("Debug:----PickMaterialAllOut Start----");
                int res = DBPCaller.CompletePickMaterialByLocId(MyData.GetStationId(),
                                                                   MyData._lastArrive.stockno,
                                                                   locId,
                                                                   MyData.GetStockNoType());
                Trace.WriteLine("Debug:----PickMaterialAllOut Stop----");
                Log.Error("CompletePickMaterialByLocId返回：" + res);
                Trace.WriteLine("Debug: CompletePickMaterialByLocId返回：" + res);
                if (res == 0 || res == 1 || res == 2)
                {
                    result = res;
                    return true;
                }
                else
                {
                    ShowHint("CompletePickMaterialByLocId未定义的返回：" + res.ToString(), Color.Red);
                }

                return false;
            }
            catch (Exception ex)
            {
                result = -99;
                Log.Error("CompletePickMaterialByLocId异常：" + ex.Message);
                ShowHint("CompletePickMaterialByLocId异常：" + ex.Message, Color.Red);
                return false;
            }
        }

        //整盘出库， 带拆盘检查功能
        public bool PartialPickMaterialAllOut(string locId, ref int result)  //type=0,五合一条码，1=TrSn, 2=全盘出
        {
            try
            {
                Trace.WriteLine("Debug:----PartialPickMaterialAllOut Start----");
                int res = DBPCaller.CompletePartialPickByLocId(MyData.GetStationId(),
                                                                MyData._lastArrive.stockno,
                                                                locId,
                                                                MyData.GetStockNoType());
                Trace.WriteLine("Debug:----PartialPickMaterialAllOut Stop----");
                Log.Error("CompletePartialPickByLocId返回：" + res);
                Trace.WriteLine("Debug: CompletePartialPickByLocId返回：" + res);
                if (res == 0 || res == 1 || res == 2)
                {
                    result = res;
                    return true;
                }     
                else if (res==3)
                {
                    ShowHint("CompletePartialPickByLocId返回3，请联系系统管理员!", Color.Red);
                }           
                else
                {
                    ShowHint("CompletePartialPickByLocId返回：" + res.ToString(), Color.Red);
                }

                return false;
            }
            catch (Exception ex)
            {
                result = -99;
                Log.Error("CompletePartialPickByLocId异常：" + ex.Message);
                ShowHint("CompletePartialPickByLocId异常：" + ex.Message, Color.Red);
                return false;
            }
        }

        //调用远端接口，判断条码类型，是储位编号还是物料条码
        private bool CheckBarcode(string barcode, ref int type)
        {
            try
            {
                int result = DBPCaller.CheckBarcode(barcode);
                Trace.WriteLine("Debug: CheckBarcode return " + result);
                if (result == 1 /*储位编号*/ || result == 3 /*TR_SN*/)
                {
                    type = result;
                    return true;
                }
                else if (result == 2)
                {
                    ShowHint("扫入的是周转箱编号，请扫描正确的条码!", Color.Red);
                }
                else if (result == 4)
                {
                    ShowHint("扫入的是出库单号，请扫描正确的条码!", Color.Red);
                }
                else if (result == 5)
                {
                    ShowHint("扫入的是入库单号，请扫描正确的条码!", Color.Red);
                }
                else
                {
                    ShowHint("未知条码，请扫正确的条码", Color.Red);
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error("CheckBarcode: " + ex.Message);
                ShowHint("CheckBarcode: " + ex.Message, Color.Red);
                return false;
            }
        }

        private bool UpdateShelfCount(int id, int stationId)
        {
            try
            {
                SearchTaskCount st = new SearchTaskCount(id, stationId);
                st.ExecuteQuery();

                int result = st.GetResult();

                ShowShelfCount(result);

                return true;
            }
            catch(Exception ex)
            {
                ShowHint("UpdateShelfCount: " + ex.Message, Color.Red);
                return false;
            }
        }

    }
}
