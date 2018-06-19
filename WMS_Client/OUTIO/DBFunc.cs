using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phicomm_WMS.DB;
using System.Data;

namespace Phicomm_WMS.OUTIO
{
    class DBFunc
    {
        public static string GetImcompletePickNo(int stationId)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("StationId", stationId);

                SearchMaterialPickAssign sp = new SearchMaterialPickAssign(dic);
                sp.ExecuteQuery();

                DataTable dt = sp.GetResult();

                if (dt == null || dt.Rows.Count == 0)
                {
                    return string.Empty;
                }

                return dt.Rows[0]["StockOutNo"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string SearchDescFromBMaterialByKpNo(string kpno)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("material_no", kpno);

                SearchBMaterial sb = new SearchBMaterial(dic);
                sb.ExecuteQuery();

                DataTable dt = sb.GetResult();
                if (dt!=null && dt.Rows.Count>0)
                {
                    return dt.Rows[0]["material_desc"].ToString();
                }

                return string.Empty;
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        public static DataTable SearchFromRInventoryDetailByTrSn(string trsn)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("tr_sn", trsn);

                SearchRInventoryDetail sb = new SearchRInventoryDetail(dic);
                sb.ExecuteQuery();

                return sb.GetResult();                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable SearchFromRInventoryIdByKpNo(string kpno)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("material_no", kpno);

                SearchRInventoryId sr = new SearchRInventoryId(dic);
                sr.ExecuteQuery();
                return sr.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SaveTrSn(List<string> listTrsn, string woid, int stockNoType)
        {
            try
            {              
                Phicomm_WMS.DB.InsertTrSn it = new Phicomm_WMS.DB.InsertTrSn(listTrsn, woid, stockNoType);
                it.ExecuteUpdate();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //查询储位中未出库数量
        public static List<string> SearchTrSnFromRInventoryDetailByLocId(string locid)
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
                    return null;
                }

                List<string> listTrSn = new List<string>();
                foreach(DataRow dr in dt.Rows)
                {
                    listTrSn.Add(dr["TR_SN"].ToString());
                }

                return listTrSn;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        //查询该工单已出库的数量
        public static List<string> SearchTrSnFromRInventoryDetailByStockOutNo(string woid, string locid)
        {
            try
            {
                //查询TrSn
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("stock_out_no", woid);
                dic.Add("loc_id", locid);
                dic.Add("status", "2");

                SearchRInventoryDetail sb = new SearchRInventoryDetail(dic);
                sb.ExecuteQuery();
                DataTable dt = sb.GetResult();

                if (dt == null || dt.Rows.Count == 0)
                {
                    return null;
                }

                List<string> listTrSn = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    listTrSn.Add(dr["TR_SN"].ToString());
                }

                return listTrSn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable SearchTbInsertTrSnByStockNo(string stockno)
        {
            try
            {
                SearchTbInsertTrSn st = new SearchTbInsertTrSn(stockno);
                st.ExecuteQuery();

                return st.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static bool CheckTrSnHasStockOut(string trsn, string stockno)
        {
            try
            {
                //查询TrSn
                Dictionary<string, object> dic = new Dictionary<string, object>();

                dic.Add("tr_sn", trsn);
                //dic.Add("status", "2");
                dic.Add("stock_out_no", stockno);

                SearchRInventoryDetail sb = new SearchRInventoryDetail(dic);
                sb.ExecuteQuery();
                DataTable dt = sb.GetResult();

                if (dt == null || dt.Rows.Count == 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CheckWoIdInvalid(string woid)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("SHIPPING_NO", woid);
                dic.Add("DEB_CRED", "OUT");

                SearchSapMaterialShipping ss = new SearchSapMaterialShipping(dic);
                ss.ExecuteQuery();
                DataTable dt = ss.GetResult();

                if (dt!=null && dt.Rows.Count>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }           
        }

        public static DataTable SearchRSapMaterialShippingByMoveType(string woid, string movetype, string deb_cred)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("SHIPPING_NO", woid);
                dic.Add("MOVE_TYPE", movetype);
                dic.Add("DEB_CRED", deb_cred);
                dic.Add("UPLOAD_FLAG", "N");

                SearchSapMaterialShipping ss = new SearchSapMaterialShipping(dic);
                ss.ExecuteQuery();

                return ss.GetResult();                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable SearchReserveNoFromRErpWoBomInfoByKpNo(string woid, string kpno)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("woid", woid);
                dic.Add("material_no", kpno);

                SearchErpWoBomInfo si = new SearchErpWoBomInfo(dic);
                si.ExecuteQuery();
                return si.GetResult();
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        public static void UpateSapMaterialShipping(DataTable dt, string remark, string transactionno, string status, string upload)
        {
            try
            {
                UpateSapMaterialShipping ul = new UpateSapMaterialShipping(dt, remark, transactionno, status, upload);
                ul.ExecuteUpdate();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        public static bool UpdateSapLocation(string woid, int stocknotype, ref string aim_plant, ref string aim_stoloc, ref string result)
        {
            try
            {
                if (stocknotype!= (int)MyData.PickWoType.Transfer && stocknotype != (int)MyData.PickWoType.OutSource)
                {
                    return true;
                }

                if (stocknotype == (int)MyData.PickWoType.Transfer)
                {
                    SearchTransferRequisitionDetail sr = new SearchTransferRequisitionDetail(woid);
                    sr.ExecuteQuery();
                    DataTable dt = sr.GetResult();
                    if (dt==null && dt.Rows.Count == 0)
                    {
                        result = "移库表中查询不到该工单记录";
                        return false;
                    }

                    aim_plant = dt.Rows[0]["factory_id"].ToString();
                    aim_stoloc = dt.Rows[0]["aim_stock_no"].ToString();                    
                }
                else if (stocknotype == (int)MyData.PickWoType.OutSource) //委外
                {
                    SearchOutSourceRequisitionDetail sr = new SearchOutSourceRequisitionDetail(woid);
                    sr.ExecuteQuery();
                    DataTable dt = sr.GetResult();
                    if (dt == null && dt.Rows.Count == 0)
                    {
                        result = "移库表中查询不到该工单记录";
                        return false;
                    }

                    aim_plant = dt.Rows[0]["LIFNR"].ToString();
                    aim_stoloc = "NA";
                }

                if (string.IsNullOrEmpty(aim_plant) || string.IsNullOrEmpty(aim_stoloc))
                {
                    result = "该工单的目标工厂和目标库位为空，请检查";
                    return false;
                }

                //可能需要更新r_sap_material_shipping里面的目标工厂和目标库位
                UpateSapMaterialShippingLocation us = new UpateSapMaterialShippingLocation(woid, aim_plant, aim_stoloc);
                us.ExecuteUpdate();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateTbInsertTrSn(string woid, string trsn)
        {
            try
            {
                UpdateTbInsertTrSn ut = new UpdateTbInsertTrSn(woid, trsn);

                ut.ExecuteUpdate();

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
    }
}
