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

namespace Phicomm_WMS.UI
{
    public partial class PickFrm : Office2007Form
    {
        //将TrSn推送到线边仓
        public bool InsertTrSn(string woid, int stockNoType, string trsn)
        {
            string _woid = woid;

            //需要推送的：发料，超领
            //不需要推送的：售出，委外，移库
            //不确定的：预留
            if (stockNoType == (int)MyData.PickWoType.Delivery ||
                stockNoType == (int)MyData.PickWoType.OutSource ||
                stockNoType == (int)MyData.PickWoType.Transfer)
            {
                Print("该工单类型为：" + stockNoType + ", 属于售出，委外，移库中的一种，不需要推送TrSn信息到线边仓");
                return true;
            }
            
            //超领需要去掉最后面两位流水号
            if (stockNoType == (int)MyData.PickWoType.Super)
            {
                _woid = woid.Substring(0, woid.Length - 2);
                Print("该工单类型属于超领，去掉工单最后两位流水号，去前为：" + woid + ", 去后为：" + _woid);
            }
            else if (stockNoType == (int)MyData.PickWoType.Discard)
            {
                _woid = woid.Substring(0, woid.Length - 3);
                Print("该工单类型属于制程报废，去掉工单最后三流水号，去前为：" + woid + ", 去后为：" + _woid);
            }
            
            try
            {
                Trace.WriteLine("Debug:----InsertTrSn Start----");

                DataTable dt = new DataTable();
                dt.Columns.Add("TR_SN", typeof(string));
                dt.Columns.Add("KP_NO", typeof(string));
                dt.Columns.Add("KP_DESC", typeof(string));
                dt.Columns.Add("VENDER_ID", typeof(string));
                dt.Columns.Add("VENDER_NAME", typeof(string));
                dt.Columns.Add("DATE_CODE", typeof(string));
                dt.Columns.Add("LOT_CODE", typeof(string));
                dt.Columns.Add("QTY", typeof(int));
                dt.Columns.Add("STOCK_ID", typeof(string));
                dt.Columns.Add("LOC_ID", typeof(string));
                dt.Columns.Add("STATUS", typeof(string));
                dt.Columns.Add("FIFO_DATECODE", typeof(string));
                dt.Columns.Add("INVENTORY_DATE", typeof(string));
                
                //通过TrSn查表R_Inventory_Detail
                DataTable dt1 = new DataTable();
                try
                {
                    dt1 = DBFunc.SearchFromRInventoryDetailByTrSn(trsn);
                    if (dt1 == null || dt1.Rows.Count == 0)
                    {
                        Print("在RInventoryDetail中找不到该TrSn信息：" + trsn);
                        //ShowHint("在RInventoryDetail中找不到该TrSn信息：" + trsn, Color.Red);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    //Print("SearchFromRInventoryDetailByTrSn异常:" + ex.Message);
                    ShowHint("SearchFromRInventoryDetailByTrSn异常:" + ex.Message, Color.Red);
                    return false;
                }

                dt1.Rows[0]["KP_DESC"] = "NA";
                dt1.Rows[0]["VENDER_NAME"] = "NA";
                try
                {
                    dt1.Rows[0]["KP_DESC"] = DBFunc.SearchDescFromBMaterialByKpNo(dt1.Rows[0]["KP_NO"].ToString());
                }
                catch (Exception ex)
                {
                    ShowHint("SearchDescFromBMaterialByKpNo异常:" + ex.Message, Color.Red);
                }

                //如果R_Inventory_Id中查不到信息，则去B_Material中查询
                if (string.IsNullOrEmpty(dt1.Rows[0]["KP_DESC"].ToString().Trim()) ||
                    string.IsNullOrEmpty(dt1.Rows[0]["VENDER_NAME"].ToString().Trim()))
                {
                    //通过KP_NO查表R_Inventory_ID
                    try
                    {
                        DataTable dt2 = DBFunc.SearchFromRInventoryIdByKpNo(dt1.Rows[0]["KP_NO"].ToString());

                        dt1.Rows[0]["KP_DESC"] = dt2.Rows[0]["material_desc"].ToString();
                        dt1.Rows[0]["VENDER_NAME"] = dt2.Rows[0]["vender_name"].ToString();
                    }
                    catch (Exception ex)
                    {
                        Print("SearchFromRInventoryIdByKpNo异常:" + ex.Message);
                    }
                }

                dt.Rows.Add(
                    dt1.Rows[0]["TR_SN"],
                    dt1.Rows[0]["KP_NO"],
                    dt1.Rows[0]["KP_DESC"],
                    dt1.Rows[0]["VENDER_ID"],
                    dt1.Rows[0]["VENDER_NAME"],
                    dt1.Rows[0]["DATE_CODE"],
                    dt1.Rows[0]["LOT_CODE"],
                    dt1.Rows[0]["QTY"],
                    dt1.Rows[0]["STOCK_ID"],
                    dt1.Rows[0]["LOC_ID"],
                    dt1.Rows[0]["STATUS"],
                    dt1.Rows[0]["FIFO_DATECODE"],
                    dt1.Rows[0]["INVENTORY_DATE"]
                    );                

                string result = string.Empty;

                if (Phicomm_WMS.OUTIO.tR_Tr_Sn.InsertRTrSn(_woid, dt, ref result) ||
                    result.Contains("Duplicate"))
                {                   
                    DBFunc.UpdateTbInsertTrSn(woid, trsn);
                    return true;
                }

                Print("推送信息到线边仓失败：" + result);
                //ShowHint("推送失败：" + result, Color.Red);
                return false;
            }
            catch (Exception ex)
            {
                Print("InsertRTrSn:" + ex.Message);
                ShowHint("InsertRTrSn:" + ex.Message, Color.Red);
                return false;
            }
            finally
            {
                Trace.WriteLine("Debug:----InsertTrSn Stop----");
            }
        }

        //将listTrSn保存到本地进行缓存
        public bool SaveTrSn(List<string> listTrsn, string woid, int stockNoType)
        {
            //需要推送的：发料，超领
            //不需要推送的：售出，委外，移库
            //不确定的：预留
            if (stockNoType != (int)MyData.PickWoType.Normal &&
                stockNoType != (int)MyData.PickWoType.Super &&
                stockNoType != (int)MyData.PickWoType.Discard)
            {
                return true;
            }

            if (listTrsn == null || listTrsn.Count == 0)
            {
                return false;
            }

            try
            {
                return DBFunc.SaveTrSn(listTrsn, woid, stockNoType);
            }
            catch (Exception ex)
            {
                ShowHint("InsertRTrSn:" + ex.Message, Color.Red);
                return false;
            }
        }

        //1.读取本地缓存的TrSn, 2.推送到线边仓
        private void PushTrSnByStockNo(string woid, int stockNoType)
        {
            if (stockNoType != (int)MyData.PickWoType.Normal &&
                stockNoType != (int)MyData.PickWoType.Super &&
                stockNoType != (int)MyData.PickWoType.Discard)
            {
                return;
            }

            try
            {
                DataTable dt = DBFunc.SearchTbInsertTrSnByStockNo(woid);
                if (dt == null || dt.Rows.Count == 0)
                {
                    //Print("该工单没有需要推送的TrSn信息");
                    return;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (InsertTrSn(dt.Rows[0]["Stock_No"].ToString(), int.Parse(dt.Rows[0]["StockNo_Type"].ToString()), dr["Tr_Sn"].ToString().ToUpper()))
                    {                        
                        Print(DateTime.Now.ToString() + "推送TrSn=" + dr["Tr_Sn"].ToString() + ", ID=" + dr["ID"].ToString() + "的信息到线边仓成功！");
                        //ShowHint("推送信息到线边仓成功!", Color.Lime);
                    }
                    else
                    {
                        Print(DateTime.Now.ToString() + "推送TrSn=" + dr["Tr_Sn"].ToString() + ", ID=" + dr["ID"].ToString() + "的信息到线边仓失败！");
                        //ShowHint("推送信息到线边仓失败!", Color.Red);
                    }
                }                
            }
            catch (Exception ex)
            {
                Log.Error("推送信息到线边仓异常：" + ex.Message);
                Print("推送信息到线边仓异常：" + ex.Message);
            }
        }

    }
}
