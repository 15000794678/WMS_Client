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

namespace Phicomm_WMS.UI
{
    public partial class ReplenishFrm : Office2007Form
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

                    if (textBox_BarCode.Text.Trim().ToUpper().Equals("F4"))
                    {
                        //叫车

                    }
                    else if (textBox_BarCode.Text.Trim().ToUpper().Equals("F5"))  //小车离开
                    {
                        if (AgvLeave(MyData.GetStationId()))
                        {
                            UpdateShelfCount(1, MyData.GetStationId());
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
                int result = 0;

                //远程检查条码类型
                if (!CheckBarcode(str, ref result))
                {
                    return;
                }

                if (result == 1) //储位
                {
                    if (!str.Equals(MyData._lastArrive.locid))
                    {
                        ShowHint("储位号错误，请扫描正确的储位号！", Color.Red);
                        return;
                    }

                    _lastScanLocId = str;
                    ShowMessage("请接着扫周转箱编号");
                    ShowHolder();              
                }
                else if (result == 2) //周转箱编号
                {                    
                    if (!_lastScanLocId.Equals(MyData._lastArrive.locid))
                    {
                        ShowHint("请先扫描储位编号！", Color.Red);
                        return;
                    }

                    if (!str.Equals(MyData._lastArrive.hoderid))
                    {
                        ShowHint("请扫描正确的周转箱编号！", Color.Red);
                        return;
                    }

                    //入库                    
                    if (!CompleteReplenishMaterial(MyData.GetStationId(), _lastScanLocId, int.Parse(str), MyData.GetStockNo(), ref result))
                    {
                        return;
                    }

                    //更新货架信息
                    StartFindShelf();
                    
                    if (result==1) //入库成功，所有站点都没有待入库的料号
                    {
                        //SapReplenish(MyData.GetStockNo());
                        ShowHint("入库过账功能暂时关闭!", Color.Lime);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("ExecuteInput:" + ex.Message);

                //更新货架信息
                StartFindShelf();
            }
        }

        //调用远端接口，判断条码类型，是储位编号还是物料条码
        private bool CheckBarcode(string barcode, ref int type)
        {
            try
            {
                int result = DBPCaller.CheckBarcode(barcode);
                if (result == 1 /*储位编号*/ || result == 2 /*周转箱*/)
                {
                    type = result;
                    return true;
                }
                else if (result == 3)
                {
                    ShowHint("扫入的是物料唯一条码，请扫正确的条码!", Color.Red);
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

        /*
         *  2: 入库成功，当前站点没有待入库的料号，但是其他站点有
            1: 入库成功，所有站点都没有待入库的料号
            0：入库成功，且当前站点还有未入库的料号
            -1：当前站点不存在对应任务，入库失败
            */
        private bool CompleteReplenishMaterial(int stationId, string locId, int holderId, string stockno, ref int result)
        {
            try
            {
                result = DBPCaller.CompleteReplenishMaterial(stationId, locId, holderId, stockno);
                Trace.WriteLine("Debug: CompleteReplenishMaterial return " + result);
                if (result==0 || result==1 || result==2)
                {
                    return true;
                }
                else if (result==-2)
                {
                    ShowHint("周转箱编号错误，请扫描正确的周转箱！", Color.Red);
                }
                else if (result==-1)
                {
                    ShowHint("入库失败，当前站点不存在对应任务！请将周转箱取下！", Color.Red);
                }
                else
                {
                    ShowHint("CompleteReplenishMaterial返回：" + result, Color.Red);
                }

                return false;
            }
            catch(Exception ex)
            {
                ShowHint("CompleteReplenishMaterial:" + ex.Message, Color.Red);
                return false;
            }
        }

        private bool GetStockNoType(string woid, ref int stockNoType)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("stock_no", woid);

                SearchRInventoryId sr = new SearchRInventoryId(dic);
                sr.ExecuteQuery();

                DataTable dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("在表R_Inventory_Id表中查不到此工单： " + woid, Color.Red);
                    return false;
                }

                stockNoType = int.Parse(dt.Rows[0]["stockno_type"].ToString());

                return true;
            }
            catch (Exception ex)
            {
                ShowHint("GetStockNoType:" + ex.Message, Color.Red);
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
            catch (Exception ex)
            {
                ShowHint("UpdateShelfCount: " + ex.Message, Color.Red);
                return false;
            }
        }
    }
}
