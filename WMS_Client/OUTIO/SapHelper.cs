using System;
using System.Collections.Generic;
using System.Data;
using Phicomm_WMS.DeliveryList_OUT;
using Phicomm_WMS.GoodsMvtCreate_OUT;
using Phicomm_WMS.ReservationDetail_OUT;
using Phicomm_WMS.ZPI_PIINTF_MES_SUBCON_PURCHASE;

namespace Phicomm_WMS.OUTIO
{
    class SapHelper
    {
        public static DataTable GetPI023(string woid)
        {
            try
            {
                ZPI_PIINTF_MES_SUBCON_PURCHASEService service = new ZPI_PIINTF_MES_SUBCON_PURCHASEService();
                service.Credentials = new System.Net.NetworkCredential("WMSPIUser", "feixunIT*2014");

                List <ZPP_S_SUBCON_PURCHASE> listZpp = new List<ZPP_S_SUBCON_PURCHASE>();
                ZPP_S_SUBCON_PURCHASE[] itemZpp = listZpp.ToArray();

                ZPP_S_MESSAGE msg = service.ZPI_PIINTF_MES_SUBCON_PURCHASE(woid, ref itemZpp);

                if (msg.TYPE.Equals("E"))
                {
                    //MessageBox.Show("GetPI023返回：" + msg.MESSAGE);
                    return null;
                }

                if (itemZpp.Length == 0)
                {
                    //MessageBox.Show("GetPI023查不到数据");
                    return null;
                }
                
                DataTable dt = new DataTable();
                dt.Columns.Add("WOID", typeof(string)); //出库单号
                dt.Columns.Add("EBELP", typeof(string)); //采购凭证的项目编号
                dt.Columns.Add("LOEKZ", typeof(string)); //采购凭证中的删除标识
                dt.Columns.Add("MATNR_SUB", typeof(string)); //物料号,MATNR_SUB
                dt.Columns.Add("MAKTX", typeof(string)); //物料描述（短文本）
                dt.Columns.Add("BDMNG", typeof(string)); //采购订单数量
                dt.Columns.Add("MEINS", typeof(string)); //采购订单的计量单位
                dt.Columns.Add("WEAKS", typeof(string)); //工厂
                dt.Columns.Add("LGORT", typeof(string)); //库存地点
                dt.Columns.Add("LIFNR", typeof(string)); //供应商账号号，相当于目标工厂
                foreach (ZPP_S_SUBCON_PURCHASE rr in itemZpp)
                {
                    dt.Rows.Add(woid, rr.EBELP, rr.LOEKZ, rr.MATNR_SUB, rr.MAKTX,
                                rr.BDMNG, "PC", rr.WERKS, rr.LGORT, rr.LIFNR);
                }
                
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //售出
        public static DataTable GetPI021(string woid)
        {
            try
            {
                DeliveryList_OUTService service = new DeliveryList_OUTService();
                service.Credentials = new System.Net.NetworkCredential("WMSPIUser", "feixunIT*2014");

                DeliveryList_Req req = new DeliveryList_Req();
                req.i_vbeln = woid;//交货单号码

                DeliveryList_Res res = new DeliveryList_Res();
                res = service.DeliveryList_OUT(req);

                //解析结果
                string s1 = res.E_WBSTK;
                DeliveryList_ResES_DELIVERY_STO_HEADER header = res.ES_DELIVERY_STO_HEADER;
                DeliveryList_ResE_RETURN res2 = res.E_RETURN;
                if (!res2.TYPE.Equals("S"))
                {
                    //MessageBox.Show("GetPI021返回：" + res2.MESSAGE);
                    return null;
                }

                DeliveryList_ResITEM[] item = res.ET_DELIVERY_ITEM;
                if (item.Length == 0)
                {
                    //MessageBox.Show("查不到数据");
                    return null;
                }
                
                DataTable dt = new DataTable();
                dt.Columns.Add("LGMNG", typeof(string));//以仓库保管单位级的实际交货数量
                dt.Columns.Add("LGORT", typeof(string));//库存地点
                dt.Columns.Add("MATNR", typeof(string));//物料号
                dt.Columns.Add("MEINS", typeof(string));//基本计量单位
                dt.Columns.Add("POSNR_VL", typeof(string));//交货项目
                dt.Columns.Add("VBELN_VL", typeof(string));//交货
                dt.Columns.Add("WEAKS", typeof(string));//工厂

                foreach (DeliveryList_ResITEM rr in item)
                {
                    dt.Rows.Add(rr.LGMNG, rr.LGORT, rr.MATNR, rr.MEINS, rr.POSNR_VL, rr.VBELN_VL, rr.WERKS);
                }
                
                return dt;
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }

        //预留
        public static DataTable GetPI020(string woid)
        {
            try
            {
                ReservationDetail_OUTService service = new ReservationDetail_OUTService();

                service.Credentials = new System.Net.NetworkCredential("WMSPIUser", "feixunIT*2014");

                ReservationDetail_Req req = new ReservationDetail_Req();
                req.I_RESERVATION = woid;

                ReservationDetail_Res res = new ReservationDetail_Res();

                res = service.ReservationDetail_OUT(req);

                ReservationDetail_ResES_RETURN res2 = res.ES_RETURN;
                if (!res2.TYPE.Equals("S"))
                {
                    //MessageBox.Show("GetPI020返回：" + res2.MESSAGE);
                    return null;
                }
                ReservationDetail_ResItem[] item = res.ET_RESERVATION_ITEMS;
                if (item.Length == 0)
                {
                    ///MessageBox.Show("GetPI020返回：无数据");
                    return null;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("RES_NO", typeof(string));
                dt.Columns.Add("RES_ITEM", typeof(string));
                dt.Columns.Add("DELETE_IND", typeof(string));
                dt.Columns.Add("MOVEMENT", typeof(string));
                dt.Columns.Add("WITHDRAWN", typeof(string));
                dt.Columns.Add("MATERIAL", typeof(string));
                dt.Columns.Add("PLANT", typeof(string));
                dt.Columns.Add("STORE_LOC", typeof(string));
                dt.Columns.Add("REQ_QUAN", typeof(string));
                dt.Columns.Add("BASE_UOM", typeof(string));
                dt.Columns.Add("FIXED_QUAN", typeof(string));
                dt.Columns.Add("WITHD_QUAN", typeof(string));
                dt.Columns.Add("MOVE_TYPE", typeof(string));
                dt.Columns.Add("MOVE_PLANT", typeof(string));
                dt.Columns.Add("MOVE_STLOC", typeof(string));
                dt.Columns.Add("DEB_CRED", typeof(string));

                foreach (ReservationDetail_ResItem rr in item)
                {
                    dt.Rows.Add(rr.RES_NO, rr.RES_ITEM, rr.DELETE_IND, rr.MOVEMENT, rr.WITHDRAWN, rr.MATERIAL,
                        rr.PLANT, rr.STORE_LOC, rr.REQ_QUAN, rr.BASE_UOM, rr.FIXED_QUAN, rr.WITHD_QUAN, rr.MOVE_TYPE,
                        rr.MOVE_PLANT, rr.MOVE_STLOC, rr.DEB_CRED);
                }

                return dt;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("GetPI020异常：" + ex.Message);
                throw ex;
            }
        }

        //过账
        public static bool GetPI018(List<Dictionary<string, object>> dicList, string mt, int stockNoType, ref string remark, ref string tranno)
        {
            try
            {
                GoodsMvtCreate_OUTService service = new GoodsMvtCreate_OUTService();
                service.Credentials = new System.Net.NetworkCredential("WMSPIUser", "feixunIT*2014");

                #region 拼凑传入要过账的信息
                GoodsMvtCreate_req dt = new GoodsMvtCreate_req();
                dt.I_GOACTION = "A08"; 
                dt.I_REFDOC = "R10";
                if (mt.Equals("541") || mt.Equals("542"))
                {
                    dt.I_GOACTION = "A08"; //541,542
                    dt.I_REFDOC = "R10";
                }
                else if (mt.Equals("512") || mt.Equals("511"))
                {
                    dt.I_GOACTION = "A01";
                    dt.I_REFDOC = "R10";
                }
                else if (mt.Equals("311"))
                {
                    dt.I_GOACTION = "A08";
                    dt.I_REFDOC = "R10";
                }
                else if (mt.Equals("261"))
                {
                    dt.I_GOACTION = "A07";
                    dt.I_REFDOC = "R08";
                }

                GoodsMvtCreate_reqIS_GOODSMVT_HEADER header = new GoodsMvtCreate_reqIS_GOODSMVT_HEADER();
                header.HEADER_TXT = "";
                header.PSTNG_DATE = System.DateTime.Now.ToString("yyyyMMdd");
                header.DOC_DATE = System.DateTime.Now.ToString("yyyyMMdd");
                if (mt.Equals("511") || mt.Equals("541"))
                {
                    header.BILL_OF_LADING = dicList[0]["SHIPPING_NO"].ToString();
                }

                GoodsMvtCreate_reqItem[] listItem = new GoodsMvtCreate_reqItem[dicList.Count];

                int x = 0;
                string remark_temp = "";
                foreach (Dictionary<string, object> dic in dicList)
                {
                    GoodsMvtCreate_reqItem item = new GoodsMvtCreate_reqItem();
                    item.MATERIAL = dic["material_no"].ToString();//发货物料
                    item.PLANT = dic["PLANT"].ToString();//发货工厂
                    item.STGE_LOC = dic["STORE_LOC"].ToString();//库存地点
                    item.MOVE_TYPE = dic["MOVE_TYPE"].ToString();//移动类型（库存管理）
                    if (mt.Equals("511") || mt.Equals("512") || mt.Equals("541") || mt.Equals("542"))
                    {
                        item.STCK_TYPE = "1";//库存类型
                    }
                    //item.SPEC_STOCK = "";//特殊库存标识
                    //item.SALES_ORD = "";//销售订单数
                    //item.S_ORD_ITEM = "";//销售订单中的项目编号
                    item.ENTRY_QNT = dic["QTY"].ToString();//以输入单位计的数量
                    item.ENTRY_UOM = "PC";//条目单位
                    if (mt.Equals("541") || mt.Equals("542") || mt.Equals("511"))
                    {
                        item.VENDOR = string.Format("{0:D10}", int.Parse(dic["MOVE_PLANT"].ToString()));//供应商帐户号
                    }
                    //item.PO_ITEM = "";//采购凭证的项目编号
                    //item.PO_NUMBER = "";//采购订单编号
                    //item.UNLOAD_PT = "";//卸货点    
                    //item.ITEM_TEXT = "";
                    if (mt.Equals("261"))
                    {
                        item.ORDERID = dic["SHIPPING_NO"].ToString();//订单号
                    }
                    //item.RESERV_NO = "";//预留/相关需求的编号
                    //item.RES_ITEM = "";//预留/相关需求的项目编号
                    if (mt.Equals("512") || mt.Equals("311"))
                    {
                        item.MOVE_PLANT = dic["MOVE_PLANT"].ToString();//收货/发货工厂
                    }
                    if (mt.Equals("311"))
                    {
                        item.MOVE_STLOC = dic["MOVE_STLOC"].ToString();//收货/发货库存地点	
                    }
                    item.MVT_IND = "";//移动标识                                      
                    if (mt.Equals("261"))
                    {
                        item.RESERV_NO = dic["RESERV_NO"].ToString();
                        item.RES_ITEM = dic["RES_ITEM"].ToString();
                        if (stockNoType == (int)MyData.PickWoType.Normal)
                        {
                            item.MOVE_REAS = "0001";//移动原因, 正常发料
                        }
                        else if (stockNoType== (int)MyData.PickWoType.Discard)
                        {
                            item.MOVE_REAS = "0003";//移动原因，制程报废
                        }
                    }
                    //item.NO_MORE_GR = "";//交货已完成标识	
                    //item.DELIV_NUMB = "";//交货
                    //item.DELIV_ITEM = "";//交货项目

                    listItem[x] = item;

                    x++;

                    remark_temp = dic["SHIPPING_NO"].ToString();
                }
                dt.IT_GOODSMVT_ITEM = listItem;
                header.HEADER_TXT = remark_temp;
                dt.IS_GOODSMVT_HEADER = header;
                #endregion

                //通信
                GoodsMvtCreate_res res = service.GoodsMvtCreate_OUT(dt);

                remark = res.ES_RETURN.MESSAGE;
                tranno = res.E_MATERIALDOCUMENT;
                //分析结果
                if (res.ES_RETURN.TYPE.ToUpper().Trim().Equals("S"))
                {
                    return true;
                }

                return false; 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
