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

namespace Phicomm_WMS.UI
{
    public partial class ReplenishFrm : Office2007Form
    {
        //查询货架主线程，查询到信息后停止
        private void QueryThread()
        {
            while (runFlag)
            {
                eventStartFind.WaitOne(); //等待启动查找的信号量到来
                if (!runFlag) break;

                ShowMessage("等待货架到达...");
                while (!findFlag)
                {
                    QueryShelfInformation();

                    if (findFlag) break;

                    if (!runFlag) return;

                    Thread.Sleep(100);
                }

                if (!runFlag) break;
            }
        }

        //进行一次查询
        private void QueryShelfInformation()
        {
            try
            {
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();

                Trace.WriteLine("Debug: ------AtStationPod Start------");
                dt1 = DBPCaller.AtStationPod(MyData.GetStationId());
                Trace.WriteLine("Debug: ------AtStationPod Stop------");
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    //打印货架信息
                    Trace.WriteLine("Debug: PodId=" + dt1.Rows[0]["PodId"].ToString() +
                                    ", PodName=" + dt1.Rows[0]["PodName"].ToString() +
                                    ", Row=" + dt1.Rows[0]["Row"].ToString() +
                                    ", Column=" + dt1.Rows[0]["Column"].ToString()
                                    );

                    Trace.WriteLine("Debug: ------PickAtStationLoc Start------");
                    dt2 = DBPCaller.ReplenishAtStationLoc(MyData.GetStationId());
                    Trace.WriteLine("Debug: ------PickAtStationLoc Stop------");
                    _dtShelf = dt2;

                    if (_dtShelf!=null && _dtShelf.Rows.Count > 0)
                    {
                        Trace.WriteLine("Debug: StockNo=" + _dtShelf.Rows[0]["StockNo"].ToString() +
                                        ", PodId=" + _dtShelf.Rows[0]["PodId"].ToString() +
                                        ", ShelfId=" + _dtShelf.Rows[0]["ShelfId"].ToString() +
                                        ", BoxId=" + _dtShelf.Rows[0]["BoxId"].ToString() +
                                        ", MaterialId=" + _dtShelf.Rows[0]["MaterialId"].ToString() +
                                        ", MaterialName=" + _dtShelf.Rows[0]["MaterialName"].ToString() +
                                        ", BoxBarcode=" + _dtShelf.Rows[0]["BoxBarcode"].ToString() +
                                        ", HoderId=" + _dtShelf.Rows[0]["HoderId"].ToString() +
                                        ", PodSide=" + _dtShelf.Rows[0]["PodSide"].ToString()
                                        );
                    }

                    if (runFlag)
                    {
                        Trace.WriteLine("Debug: ------RefreshUI Ready Start------");
                        RefreshUI(int.Parse(dt1.Rows[0]["Row"].ToString()), int.Parse(dt1.Rows[0]["Column"].ToString()), _dtShelf);
                        Trace.WriteLine("Debug: ------RefreshUI Ready Stop------");
                    }
                }
                else
                {
                    if (runFlag)
                    {
                        Trace.WriteLine("Debug: ------RefreshUI Ready Start------");
                        RefreshUI(0, 0, null);
                        Trace.WriteLine("Debug: ------RefreshUI Ready Stop------");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("RefreshUI:" + ex.Message);
                //ShowHint("RefreshUI:" + ex.Message, Color.Red);
            }
        }
    }
}
