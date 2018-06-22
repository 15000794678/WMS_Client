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

namespace WMS_Client.UI
{
    public partial class PickFrm : Office2007Form
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
            }
        }

        //测试用,UI界面刷新只要10ms
        private void QueryShelfInformationTest()
        {
            int[] index = new int[] { 11, 12, 13, 14,
                                          21, 22, 23, 34,
                                          31, 32, 33, 34,
                                          41, 42, 43, 44,
                                          51, 52, 53, 54};

            DataTable _dt = new DataTable();
            _dt.Columns.Add("StockNo", typeof(string));
            _dt.Columns.Add("ShowText", typeof(string));
            _dt.Columns.Add("PodId", typeof(int));
            _dt.Columns.Add("PodSide", typeof(int));
            _dt.Columns.Add("ShelfId", typeof(int));
            _dt.Columns.Add("BoxBarcode", typeof(string));
            _dt.Columns.Add("BoxId", typeof(int));
            _dt.Columns.Add("MaterialId", typeof(string));
            _dt.Columns.Add("MaterialName", typeof(string));
            _dt.Columns.Add("Qty", typeof(int));
            _dt.Columns.Add("AllOut", typeof(int));

            foreach (int cnt in index)
            {
                Trace.WriteLine("Debug: ------QueryShelfInformationTest Start------");
                _dt.Rows.Clear();
                _dt.Rows.Add("123456" + cnt.ToString(),
                             "",
                             0,
                             0,
                             0,
                             "10000680" + cnt.ToString(),
                             123,
                             "MID",
                             "MDESC",
                             cnt,
                             0);
                if (cnt % 2 == 1)
                {
                    Trace.WriteLine("Debug: ------RefreshUI Ready Start------");
                    RefreshPickUI(5, 4, _dt);
                    Trace.WriteLine("Debug: ------RefreshUI Ready Stop------");
                }
                else
                {
                    Trace.WriteLine("Debug: ------RefreshUI Ready Start------");
                    RefreshPickUI(5, 2, _dt);
                    Trace.WriteLine("Debug: ------RefreshUI Ready Stop------");
                }
                Trace.WriteLine("Debug: ------QueryShelfInformationTest Stop------");

                if (!runFlag) { return; }
                Thread.Sleep(500);
            }
        }

        //进行一次查询
        private void QueryShelfInformation()
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
                        //cnt = GeTrSnCnt(dt2.Rows[0]["BoxBarcode"].ToString());
                    }

                    if (runFlag)
                    {
                        RefreshPickUI(cnt, int.Parse(dt1.Rows[0]["Column"].ToString()), _dtShelf);
                    }
                }
                else
                {
                    if (runFlag)
                    {
                        RefreshPickUI(0, 0, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("RefreshUI:" + ex.Message);
                Trace.WriteLine("Debug: QueryShelfInformation : " + ex.Message);
            }
        }

        private int GeTrSnCnt(string locid)
        {
            try
            {
                List<string> listTrSn = DBFunc.SearchTrSnFromRInventoryDetailByLocId(locid);

                return listTrSn.Count;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Trace.WriteLine("Debug: " + ex.Message);
                return -1;
            }
        }
    }
}
