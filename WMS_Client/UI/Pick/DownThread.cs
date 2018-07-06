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
        private void DownloadThread(object obj)
        {
            try
            {
                EnableUI(false);

                DataTable dt = GetData(textBox_StockNo.Text.Trim(), int.Parse(obj.ToString()));
                if (dt == null || dt.Rows.Count == 0)
                {
                    return;
                }

                ShowData(dt);

                UpdateShelfCount(2, MyData.GetStationId());
            }
            catch (Exception ex)
            {
                Log.Error("ShowData：" + ex.Message);
            }
            finally
            {
                EnableUI(true);
            }
        }

        private DataTable GetData(string woid, int stockNoType)
        {
            try
            {
                DataTable dt = new DataTable();
                /*
                Normal = 1, //工单发料
                SaleOut = 2,//售出
                Reserve = 3,//预留
                OutSource = 4,//委外
                Transfer = 5,//移库
                Super = 6, //超领
                Discard = 7, //制程报废
                */
                if (stockNoType == (int)MyData.PickWoType.Normal)
                {
                    dt = GetErpData(woid);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ShowHint("查询不到该工单数据，请确认单号，出库类型!", Color.Red);
                        return null;
                    }
                }
                else if (stockNoType == (int)MyData.PickWoType.Delivery)
                {
                    dt = GetDeliveryData(woid);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ShowHint("查询不到该工单数据，请确认单号，出库类型!", Color.Red);
                        return null;
                    }                    
                }
                else if (stockNoType == (int)MyData.PickWoType.Reserve)
                {
                    dt = GetReserveData(woid);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ShowHint("查询不到该工单数据，请确认单号，出库类型!", Color.Red);
                        return null;
                    }
                }
                else if (stockNoType == (int)MyData.PickWoType.OutSource)
                {
                    dt = GetOutSourceData2(woid);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ShowHint("查询不到该工单数据，请确认单号，出库类型!", Color.Red);
                        return null;
                    }                    
                }
                else if (stockNoType == (int)MyData.PickWoType.Transfer)
                {
                    dt = GetTransferData(textBox_StockNo.Text.Trim());
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ShowHint("查询不到该工单数据，请确认单号，出库类型!", Color.Red);
                        return null;
                    }

                    //更新subtype字段
                    if (!UpateTransferRequsitionDetailSubType(textBox_StockNo.Text.Trim(), dt.Rows[0]["SubType"].ToString()))
                    {
                        return null;
                    }
                }
                else if (stockNoType == (int)MyData.PickWoType.Super)
                {
                    dt = GetSuperData(textBox_StockNo.Text.Trim());
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ShowHint("查询不到该工单数据，请确认单号，出库类型!", Color.Red);
                        return null;
                    }
                }
                else if (stockNoType==(int)MyData.PickWoType.Discard)
                {
                    //获取制程报废数据
                    dt = GetScrapData(textBox_StockNo.Text.Trim());
                    if (dt==null || dt.Rows.Count==0)
                    {
                        ShowHint("查询不到该工单数据，请确认单号，出库类型!", Color.Red);
                        return null;
                    }
                }
                else
                {
                    ShowHint("暂时不支持其他的出库类型", Color.Red);
                    return null;
                }

                if (dt != null && dt.Rows.Count > 0 &&
                    string.IsNullOrEmpty(dt.Rows[0]["SubType"].ToString().Trim()))
                {
                    ShowHint("该工单的SubType字段未填写，请先完善数据库!", Color.Red);
                    return null;
                }

                MyData.SetStockNo(woid);
                MyData.SetStockNoType(stockNoType);
                MyData.SetStockSubType(dt.Rows[0]["SubType"].ToString().Trim());

                return dt;
            }
            catch (Exception ex)
            {
                ShowHint("GetData异常：" + ex.Message, Color.Red);
                return null;
            }
        }

        private DataTable GetErpData(string woid)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("woid", woid);

                SearchErpWoBomInfo si = new SearchErpWoBomInfo(dic);
                si.ExecuteQuery();

                return si.GetResult();
            }
            catch (Exception ex)
            {
                Log.Error("GetErpData exception：" + ex.Message);
                ShowHint("GetErpData:" + ex.Message, Color.Red);
                return null;
            }
        }

        private DataTable GetTransferData(string woid)
        {
            try
            {
                SearchTransferRequisitionDetail si = new SearchTransferRequisitionDetail(woid);
                si.ExecuteQuery();

                DataTable dt = si.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(dt.Rows[0]["woid"].ToString().Trim()) ||
                    string.IsNullOrEmpty(dt.Rows[0]["material_no"].ToString().Trim()) ||
                    string.IsNullOrEmpty(dt.Rows[0]["res_stock_no"].ToString().Trim()) ||
                    string.IsNullOrEmpty(dt.Rows[0]["aim_stock_no"].ToString().Trim()) ||
                    string.IsNullOrEmpty(dt.Rows[0]["factory_id"].ToString().Trim()) ||
                    string.IsNullOrEmpty(dt.Rows[0]["status"].ToString().Trim()) ||
                    string.IsNullOrEmpty(dt.Rows[0]["sub_type"].ToString().Trim()))
                {
                    ShowHint("该工单中有字段为空，请检查", Color.Red);
                    return null;
                }

                string woidType = GetWoidTypeByPlant((int)MyData.PickWoType.Transfer,
                                                      dt.Rows[0]["factory_id"].ToString(),
                                                      dt.Rows[0]["res_stock_no"].ToString());

                if (string.IsNullOrEmpty(woidType))
                {
                    ShowHint("从SAP下载的数据在tb_factory_move中找不到WoidType: StockNoType=" + ((int)MyData.PickWoType.Transfer).ToString() +
                        ", " + dt.Rows[0]["FromFactory"].ToString() + ", " + dt.Rows[0]["FromLoc"].ToString(), Color.Red);
                    return null;
                }

                DataTable _dt = new DataTable();
                _dt.Columns.Add("WOID", typeof(string));
                _dt.Columns.Add("KP_NO", typeof(string));
                _dt.Columns.Add("QTY", typeof(int));
                _dt.Columns.Add("Send_QTY", typeof(int));
                _dt.Columns.Add("FromFactory", typeof(string));
                _dt.Columns.Add("FromLoc", typeof(string));
                _dt.Columns.Add("MoveType", typeof(string));
                _dt.Columns.Add("ToFactory", typeof(string));//NA
                _dt.Columns.Add("ToLoc", typeof(string));//NA            
                _dt.Columns.Add("KPDESC", typeof(string));
                _dt.Columns.Add("SubType", typeof(string));
                _dt.Columns.Add("StockNoType", typeof(int)); //默认为1
                _dt.Columns.Add("Rel_Requireid", typeof(string));
                _dt.Columns.Add("Rel_ProjectId", typeof(string));
                _dt.Columns.Add("CreateTime", typeof(string));

                foreach (DataRow dr in dt.Rows)
                {
                    _dt.Rows.Add(dr["woid"],
                                 dr["material_no"],
                                 dr["qty"],
                                 dr["send_qty"],
                                 dr["factory_id"],
                                 dr["res_stock_no"],
                                 "NA",
                                 dr["factory_id"],
                                 dr["aim_stock_no"],
                                 "NA",
                                 dr["sub_type"],
                                 5,
                                 "NA",
                                 "NA",
                                 "NA"
                                 );
                }

                return _dt;
            }
            catch (Exception ex)
            {
                Log.Error("GetTransferData exception：" + ex.Message);
                ShowHint("GetTransferData:" + ex.Message, Color.Red);
                return null;
            }
        }

        private DataTable GetSuperData(string woid)
        {
            try
            {
                SearchRequisitionDetail sr = new SearchRequisitionDetail(woid);
                sr.ExecuteQuery();

                DataTable dt = sr.GetResult();

                DataTable _dt = new DataTable();
                _dt.Columns.Add("WOID", typeof(string));
                _dt.Columns.Add("KP_NO", typeof(string));
                _dt.Columns.Add("QTY", typeof(int));
                _dt.Columns.Add("Send_QTY", typeof(int));
                _dt.Columns.Add("FromFactory", typeof(string));
                _dt.Columns.Add("FromLoc", typeof(string));
                _dt.Columns.Add("MoveType", typeof(string));
                _dt.Columns.Add("ToFactory", typeof(string));//NA
                _dt.Columns.Add("ToLoc", typeof(string));//NA            
                _dt.Columns.Add("KPDESC", typeof(string));
                _dt.Columns.Add("SubType", typeof(string));
                _dt.Columns.Add("StockNoType", typeof(int)); //默认为1
                _dt.Columns.Add("Rel_Requireid", typeof(string));
                _dt.Columns.Add("Rel_ProjectId", typeof(string));
                _dt.Columns.Add("CreateTime", typeof(string));

                foreach (DataRow dr in dt.Rows)
                {
                    _dt.Rows.Add(dr["woid"],
                                 dr["material_no"],
                                 dr["qty"],
                                 dr["send_qty"],
                                 "NA",
                                 "NA",
                                 "NA",
                                 "NA",
                                 "NA",
                                 dr["kpesc"],
                                 dr["sub_type"],
                                 6,
                                 "NA",
                                 "NA",
                                 "NA"
                                 );
                }

                return _dt;
            }
            catch (Exception ex)
            {
                Log.Error("GetSuperData exception：" + ex.Message);
                MessageBox.Show("GetSuperData:" + ex.Message);
                return null;
            }
        }

        private DataTable GetScrapData(string woid)
        {
            try
            {
                SearchRScrapRequisitionHead ss = new SearchRScrapRequisitionHead(woid);
                ss.ExecuteQuery();
                DataTable dt = ss.GetResult();
                if (dt==null || dt.Rows.Count==0)
                {
                    ShowHint("在r_scrap_requisition_head表中查询不到工单：" + woid + " 的数据，请检查！", Color.Red);
                    return null;
                }
                if (!dt.Rows[0]["cnd"].ToString().Trim().Equals("2"))
                {
                    ShowHint("该单号为签核或已驳回，请检查！", Color.Red);
                    return null;
                }

                SearchRScrapRequisitionDetail sr = new SearchRScrapRequisitionDetail(woid);
                sr.ExecuteQuery();

                dt = sr.GetResult();
                if (dt==null || dt.Rows.Count==0)
                {
                    ShowHint("在r_scrap_requisition_detail表中查询不到工单：" + woid + " 的数据，请检查！", Color.Red);
                    return null;
                }

                DataTable _dt = new DataTable();
                _dt.Columns.Add("WOID", typeof(string));
                _dt.Columns.Add("KP_NO", typeof(string));
                _dt.Columns.Add("QTY", typeof(int));
                _dt.Columns.Add("Send_QTY", typeof(int));
                _dt.Columns.Add("FromFactory", typeof(string));
                _dt.Columns.Add("FromLoc", typeof(string));
                _dt.Columns.Add("MoveType", typeof(string));
                _dt.Columns.Add("ToFactory", typeof(string));//NA
                _dt.Columns.Add("ToLoc", typeof(string));//NA            
                _dt.Columns.Add("KPDESC", typeof(string));
                _dt.Columns.Add("SubType", typeof(string));
                _dt.Columns.Add("StockNoType", typeof(int)); //默认为1
                _dt.Columns.Add("Rel_Requireid", typeof(string));
                _dt.Columns.Add("Rel_ProjectId", typeof(string));
                _dt.Columns.Add("CreateTime", typeof(string));

                foreach (DataRow dr in dt.Rows)
                {
                    _dt.Rows.Add(dr["woid"],
                                 dr["material_no"],
                                 dr["qty"],
                                 dr["send_qty"],
                                 "NA",
                                 "NA",
                                 "NA",
                                 "NA",
                                 "NA",
                                 dr["kpesc"],
                                 dr["sub_type"],
                                 6,
                                 "NA",
                                 "NA",
                                 "NA"
                                 );
                }

                return _dt;
            }
            catch (Exception ex)
            {
                Log.Error("GetScrapData exception：" + ex.Message);
                MessageBox.Show("GetScrapData:" + ex.Message);
                return null;
            }
        }

        private string GetWoidTypeByPlant(int stockNoType, string plant, string loc)
        {
            try
            {
                if (string.IsNullOrEmpty(plant.Trim()) || string.IsNullOrEmpty(loc.Trim()))
                {
                    return string.Empty;
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("StockNoType", stockNoType);
                dic.Add("FromFactory", plant);
                dic.Add("FromStockId", loc);

                SearchTbFactoryMove sm = new SearchTbFactoryMove(dic);
                sm.ExecuteQuery();

                DataTable dt = sm.GetResult();
                if (dt!=null && dt.Rows.Count>0)
                {
                    return dt.Rows[0]["WoidType"].ToString();
                }

                return string.Empty;
            }
            catch(Exception ex)
            {
                ShowHint("GetWoidTypeByPlan异常：" + ex.Message, Color.Red);
                return string.Empty;
            }
        }

        private DataTable GetDeliveryData(string woid)
        {
            try
            {
                DataTable _dt = new DataTable();
                _dt.Columns.Add("WOID", typeof(string));
                _dt.Columns.Add("KP_NO", typeof(string));
                _dt.Columns.Add("QTY", typeof(int));
                _dt.Columns.Add("Send_QTY", typeof(int));
                _dt.Columns.Add("FromFactory", typeof(string));
                _dt.Columns.Add("FromLoc", typeof(string));
                _dt.Columns.Add("MoveType", typeof(string));
                _dt.Columns.Add("ToFactory", typeof(string));//NA
                _dt.Columns.Add("ToLoc", typeof(string));//NA            
                _dt.Columns.Add("KPDESC", typeof(string));
                _dt.Columns.Add("SubType", typeof(string));
                _dt.Columns.Add("StockNoType", typeof(int)); //默认为1
                _dt.Columns.Add("Rel_Requireid", typeof(string));
                _dt.Columns.Add("Rel_ProjectId", typeof(string));
                _dt.Columns.Add("CreateTime", typeof(string));

                SearchRDeliveryRequisitionDetail sr = new SearchRDeliveryRequisitionDetail(woid);
                sr.ExecuteQuery();

                DataTable dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    dt = SapHelper.GetPI021(woid);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ShowHint("从SAP下载失败!", Color.Red);
                        return null;
                    }

                    string woidType = GetWoidTypeByPlant((int)MyData.PickWoType.Delivery,
                                                          dt.Rows[0]["WEAKS"].ToString(),
                                                          dt.Rows[0]["LGORT"].ToString());
                    if (string.IsNullOrEmpty(woidType))
                    {
                        ShowHint("从SAP下载的数据在tb_factory_move中找不到WoidType: StockNoType=" + ((int)MyData.PickWoType.Delivery).ToString() + 
                            ", " + dt.Rows[0]["WEAKS"].ToString() + ", " + dt.Rows[0]["LGORT"].ToString(), Color.Red);
                        return null;
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        _dt.Rows.Add(dr["VBELN_VL"],
                                     dr["MATNR"],
                                     dr["LGMNG"],
                                     0,
                                     dr["WEAKS"],
                                     dr["LGORT"],
                                     "NA",
                                     "NA",
                                     "NA",
                                     "NA",
                                     woidType,
                                     2,
                                     "NA",
                                     "NA",
                                     "NA"
                                     );
                    }

                    InsertDeliveryRequisitonDetail(_dt);
                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        _dt.Rows.Add(dr["woid"],
                                     dr["material_no"],
                                     dr["qty"],
                                     dr["send_qty"],
                                     dr["PLANT"],
                                     dr["STORE_LOC"],
                                     dr["MOVE_TYPE"],
                                     dr["MOVE_PLANT"],
                                     dr["MOVE_STLOC"],
                                     "NA",
                                     dr["sub_type"],
                                     2,
                                     "NA",
                                     "NA",
                                     "NA"
                                     );
                    }
                }

                return _dt;
            }
            catch(Exception ex)
            {
                Log.Error("GetSaleOutData exception:" + ex.Message);
                ShowHint("GetSaleOutData: " + ex.Message, Color.Red);
                return null;
            }
        }

        public bool IsXiGao(string materialno)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("material_no", materialno);

                SearchBMaterial sb = new SearchBMaterial(dic);
                sb.ExecuteQuery();

                DataTable dt = sb.GetResult();
                if (dt!=null && dt.Rows.Count>0 &&
                    dt.Rows[0]["is_soider_paste_kp"].ToString().Trim().Equals("Y"))
                {
                    return true;
                }
                else
                {
                    return false;
                }                
            }
            catch (Exception ex)
            {
                ShowHint("从b_material中读取" + materialno + "锡膏属性时异常：" + ex.Message, Color.Red);
                return false;
            }
        }

        private DataTable GetReserveData(string woid)
        {
            try
            {
                DataTable _dt = new DataTable();
                _dt.Columns.Add("WOID", typeof(string));
                _dt.Columns.Add("KP_NO", typeof(string));
                _dt.Columns.Add("QTY", typeof(int));
                _dt.Columns.Add("Send_QTY", typeof(int));
                _dt.Columns.Add("FromFactory", typeof(string));
                _dt.Columns.Add("FromLoc", typeof(string));
                _dt.Columns.Add("MoveType", typeof(string));
                _dt.Columns.Add("ToFactory", typeof(string));//NA
                _dt.Columns.Add("ToLoc", typeof(string));//NA            
                _dt.Columns.Add("KPDESC", typeof(string));
                _dt.Columns.Add("SubType", typeof(string));
                _dt.Columns.Add("StockNoType", typeof(int)); //默认为1
                _dt.Columns.Add("Rel_Requireid", typeof(string));
                _dt.Columns.Add("Rel_ProjectId", typeof(string));
                _dt.Columns.Add("CreateTime", typeof(string));

                SearchRReserveRequisitionDetail sr = new SearchRReserveRequisitionDetail(woid);
                sr.ExecuteQuery();

                DataTable dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    dt = SapHelper.GetPI020(woid);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ShowHint("从SAP下载失败!", Color.Red);
                        return null;
                    }

                    string woidType = GetWoidTypeByPlant((int)MyData.PickWoType.Reserve,
                                                          dt.Rows[0]["PLANT"].ToString(),
                                                          dt.Rows[0]["STORE_LOC"].ToString());
                    if (string.IsNullOrEmpty(woidType))
                    {
                        ShowHint("从SAP下载的数据在tb_factory_move中找不到WoidType: StockNoType=" + ((int)MyData.PickWoType.Reserve).ToString() +
                            ", " + dt.Rows[0]["PLANT"].ToString() + ", " + dt.Rows[0]["STORE_LOC"].ToString(), Color.Red);
                        return null;
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        int num = (int)(float.Parse(dr["REQ_QUAN"].ToString()));
                        if (IsXiGao(dr["MATERIAL"].ToString().Trim()))
                        {
                            num = num * 1000;
                        }
                        _dt.Rows.Add(dr["RES_NO"],
                                     dr["MATERIAL"],
                                     num,  
                                     0,
                                     dr["PLANT"],
                                     dr["STORE_LOC"],
                                     dr["MOVE_TYPE"],
                                     dr["MOVE_PLANT"],
                                     dr["MOVE_STLOC"],
                                     "NA",
                                     woidType,
                                     3,
                                     "NA",
                                     "NA",
                                     "NA"
                                     );
                    }

                    InsertReserveRequisitonDetail(_dt);
                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        _dt.Rows.Add(dr["woid"],
                                     dr["material_no"],
                                     dr["qty"],
                                     dr["send_qty"],
                                     dr["PLANT"],
                                     dr["STORE_LOC"],
                                     dr["MOVE_TYPE"],
                                     dr["MOVE_PLANT"],
                                     dr["MOVE_STLOC"],
                                     "NA",
                                     dr["sub_type"],
                                     3,
                                     "NA",
                                     "NA",
                                     "NA"
                                     );
                    }
                }

                return _dt;
            }
            catch (Exception ex)
            {
                Log.Error("GetReserveData exception:" + ex.Message);
                ShowHint("GetReserveData: " + ex.Message, Color.Red);
                return null;
            }
        }

        private DataTable GetOutSourceData(string woid)
        {
            try
            {
                DataTable _dt = new DataTable();
                _dt.Columns.Add("WOID", typeof(string));
                _dt.Columns.Add("KP_NO", typeof(string));
                _dt.Columns.Add("QTY", typeof(int));
                _dt.Columns.Add("Send_QTY", typeof(int));
                _dt.Columns.Add("FromFactory", typeof(string));
                _dt.Columns.Add("FromLoc", typeof(string));
                _dt.Columns.Add("MoveType", typeof(string));
                _dt.Columns.Add("ToFactory", typeof(string));//NA
                _dt.Columns.Add("ToLoc", typeof(string));//NA            
                _dt.Columns.Add("KPDESC", typeof(string));
                _dt.Columns.Add("SubType", typeof(string));
                _dt.Columns.Add("StockNoType", typeof(int)); //默认为1
                _dt.Columns.Add("Rel_Requireid", typeof(string));
                _dt.Columns.Add("Rel_ProjectId", typeof(string));
                _dt.Columns.Add("CreateTime", typeof(string));
                
                SearchOutSourceRequisitionDetail sr = new SearchOutSourceRequisitionDetail(woid);
                sr.ExecuteQuery();

                DataTable dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {//如果没有下载过，则直接插入数据
                    dt = SapHelper.GetPI023(woid);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ShowHint("从SAP下载失败!", Color.Red);
                        return null;
                    }
                    string woidType = "";
                    if (dt.Rows[0]["WEAKS"].ToString().Equals("2200"))
                    {
                        woidType = "10001";
                    }
                    else if (dt.Rows[0]["WEAKS"].ToString().Equals("2100"))
                    {
                        woidType= "10003";
                    }
                    else
                    {
                        ShowHint("该工单的目标工厂必须为2100，2200，当前为：" + dt.Rows[0]["WEAKS"].ToString(), Color.Red);
                        return null;
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        float num = float.Parse(dr["BDMNG"].ToString());
                        _dt.Rows.Add(dr["WOID"],
                                     dr["MATNR_SUB"],
                                     (int)num,
                                     0,
                                     dr["WEAKS"],
                                     dr["LGORT"],
                                     "NA",
                                     dr["LIFNR"],
                                     "NA",
                                     dr["MAKTX"],
                                     woidType,
                                     4,
                                     "NA",
                                     "NA",
                                     "NA"
                                     );
                    }

                    InsertOutSourceRequisitonDetail(_dt);
                }
                else  //下载过，则需要更新用量
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        _dt.Rows.Add(dr["woid"].ToString(),
                                     dr["material_no"],
                                     dr["qty"],
                                     dr["send_qty"],
                                     dr["PLANT"],
                                     dr["STORE_LOC"],
                                     "NA",
                                     dr["LIFNR"],
                                     "NA",
                                     dr["kpesc"],
                                     dr["sub_type"],
                                     4,
                                     "NA",
                                     "NA",
                                     "NA"
                                     );
                    }
                }

                return _dt;
            }
            catch (Exception ex)
            {
                Log.Error("GetOutSource exception:" + ex.Message);
                ShowHint("GetOutSource: " + ex.Message, Color.Red);
                return null;
            }
        }

        //数据去重处理
        private DataTable GetUnduplicateData(DataTable dt)
        {
            DataTable dt_res = new DataTable();

            dt_res.Columns.Add("WOID", typeof(string)); //出库单号
            dt_res.Columns.Add("EBELP", typeof(string)); //采购凭证的项目编号
            dt_res.Columns.Add("LOEKZ", typeof(string)); //采购凭证中的删除标识
            dt_res.Columns.Add("MATNR_SUB", typeof(string)); //物料号,MATNR_SUB
            dt_res.Columns.Add("MAKTX", typeof(string)); //物料描述（短文本）
            dt_res.Columns.Add("BDMNG", typeof(string)); //采购订单数量
            dt_res.Columns.Add("MEINS", typeof(string)); //采购订单的计量单位
            dt_res.Columns.Add("WEAKS", typeof(string)); //工厂
            dt_res.Columns.Add("LGORT", typeof(string)); //库存地点
            dt_res.Columns.Add("LIFNR", typeof(string)); //供应商账号号，相当于目标工厂

            foreach (DataRow dr in dt.Rows)
            {
                if (dt_res.Rows.Count > 0)
                {
                    bool res = false;
                    foreach (DataRow dr2 in dt_res.Rows)
                    {
                        if (dr["MATNR_SUB"].ToString().Equals(dr2["MATNR_SUB"].ToString()))  //如果料号相等
                        {
                            dr2["BDMNG"] = (float.Parse(dr["MATNR_SUB"].ToString()) + float.Parse(dr2["MATNR_SUB"].ToString())).ToString();
                            res = true;
                            break;
                        }
                    }

                    if (!res)
                    {
                        dt_res.Rows.Add(dr["WOID"].ToString(), dr["EBELP"].ToString(), dr["LOEKZ"].ToString(),
                                        dr["MATNR_SUB"].ToString(), dr["MAKTX"].ToString(), dr["BDMNG"].ToString(),
                                        dr["MEINS"].ToString(), dr["WEAKS"].ToString(), dr["LGORT"].ToString(),
                                        dr["LIFNR"].ToString());
                    }
                }
                else
                {
                    dt_res.Rows.Add(dr["WOID"].ToString(), dr["EBELP"].ToString(), dr["LOEKZ"].ToString(),
                                    dr["MATNR_SUB"].ToString(), dr["MAKTX"].ToString(), dr["BDMNG"].ToString(),
                                    dr["MEINS"].ToString(), dr["WEAKS"].ToString(), dr["LGORT"].ToString(),
                                    dr["LIFNR"].ToString());
                }
            }

            return dt_res;
        }

        private DataTable GetOutSourceData2(string woid)
        {
            try
            {
                DataTable _dt = new DataTable();
                _dt.Columns.Add("WOID", typeof(string));
                _dt.Columns.Add("KP_NO", typeof(string));
                _dt.Columns.Add("QTY", typeof(int));
                _dt.Columns.Add("Send_QTY", typeof(int));
                _dt.Columns.Add("FromFactory", typeof(string));
                _dt.Columns.Add("FromLoc", typeof(string));
                _dt.Columns.Add("MoveType", typeof(string));
                _dt.Columns.Add("ToFactory", typeof(string));//NA
                _dt.Columns.Add("ToLoc", typeof(string));//NA            
                _dt.Columns.Add("KPDESC", typeof(string));
                _dt.Columns.Add("SubType", typeof(string));
                _dt.Columns.Add("StockNoType", typeof(int)); //默认为1
                _dt.Columns.Add("Rel_Requireid", typeof(string));
                _dt.Columns.Add("Rel_ProjectId", typeof(string));
                _dt.Columns.Add("CreateTime", typeof(string));
                
                DataTable dt_src = new DataTable(); //SAP下载的原始数据
                DataTable dt_sap = new DataTable(); //SAP下载后去重的数据
                DataTable dt_des = new DataTable(); //本地数据
                //从SAP下载原始数据
                dt_src = SapHelper.GetPI023(woid);
                if (dt_src == null || dt_src.Rows.Count == 0)
                {
                    ShowHint("从SAP下载失败!", Color.Red);
                    return null;
                }
                string woidType = "";
                if (dt_src.Rows[0]["WEAKS"].ToString().Equals("2200"))
                {
                    woidType = "10001";
                }
                else if (dt_src.Rows[0]["WEAKS"].ToString().Equals("2100"))
                {
                    woidType = "10003";
                }
                else
                {
                    ShowHint("该工单的目标工厂必须为2100，2200，当前为：" + dt_src.Rows[0]["WEAKS"].ToString(), Color.Red);
                    return null;
                }
                //远程数据去重复
                dt_sap = GetUnduplicateData(dt_src);

                //查询本地记录
                SearchOutSourceRequisitionDetail sr = new SearchOutSourceRequisitionDetail(woid);
                sr.ExecuteQuery();
                dt_des = sr.GetResult();
                if (dt_des == null || dt_des.Rows.Count == 0)
                {//如果没有下载过，则直接插入数据                    
                    foreach (DataRow dr in dt_src.Rows)
                    {
                        float num = float.Parse(dr["BDMNG"].ToString());
                        _dt.Rows.Add(dr["WOID"],
                                     dr["MATNR_SUB"],
                                     (int)num,
                                     0,
                                     dr["WEAKS"],
                                     dr["LGORT"],
                                     "NA",
                                     dr["LIFNR"],
                                     "NA",
                                     dr["MAKTX"],
                                     woidType,
                                     4,
                                     "NA",
                                     "NA",
                                     "NA"
                                     );
                    }

                    if (!InsertOutSourceRequisitonDetail(_dt))  //批量插入数据
                    {
                        return null;
                    }
                }
                else  //下载过，则需要更新用量
                {
                    foreach (DataRow dr in dt_sap.Rows)
                    {
                        bool res = false;
                        //查找料号是否出现过             
                        foreach (DataRow dr2 in dt_des.Rows)
                        {
                            if (dr["MATNR_SUB"].ToString().Trim().Equals(dr2["material_no"].ToString().Trim()))
                            {//料号相等
                                res = true;
                                if (!dr["BDMNG"].ToString().Trim().Equals(dr2["qty"].ToString().Trim()))
                                {//数量有更新
                                    if (!UpdateOutSourceRequisitionDetailQty(dr["WOID"].ToString(), dr["MATNR_SUB"].ToString(), dr["BDMNG"].ToString()))
                                    {
                                        return null;
                                    }
                                }
                                break;
                            }
                        }

                        //新的料号，插入一行记录到表格中
                        if (!res)
                        {
                            //插入数据
                            DataTable dt_insert = new DataTable();
                            dt_insert.Columns.Add("WOID", typeof(string));
                            dt_insert.Columns.Add("KP_NO", typeof(string));
                            dt_insert.Columns.Add("QTY", typeof(int));
                            dt_insert.Columns.Add("Send_QTY", typeof(int));
                            dt_insert.Columns.Add("FromFactory", typeof(string));
                            dt_insert.Columns.Add("FromLoc", typeof(string));
                            dt_insert.Columns.Add("MoveType", typeof(string));
                            dt_insert.Columns.Add("ToFactory", typeof(string));//NA
                            dt_insert.Columns.Add("ToLoc", typeof(string));//NA            
                            dt_insert.Columns.Add("KPDESC", typeof(string));
                            dt_insert.Columns.Add("SubType", typeof(string));
                            dt_insert.Columns.Add("StockNoType", typeof(int)); //默认为1
                            dt_insert.Columns.Add("Rel_Requireid", typeof(string));
                            dt_insert.Columns.Add("Rel_ProjectId", typeof(string));
                            dt_insert.Columns.Add("CreateTime", typeof(string));

                            float num2 = float.Parse(dr["BDMNG"].ToString());
                            dt_insert.Rows.Add(dr["WOID"],
                                         dr["MATNR_SUB"],
                                         (int)num2,
                                         0,
                                         dr["WEAKS"],
                                         dr["LGORT"],
                                         "NA",
                                         dr["LIFNR"],
                                         "NA",
                                         dr["MAKTX"],
                                         woidType,
                                         4,
                                         "NA",
                                         "NA",
                                         "NA"
                                         );
                            if (!InsertOutSourceRequisitonDetail(dt_insert)) //插入单条数据
                            {
                                return null;
                            }
                        }
                    }

                    //查询本地记录
                    SearchOutSourceRequisitionDetail ss = new SearchOutSourceRequisitionDetail(woid);
                    ss.ExecuteQuery();
                    dt_des = ss.GetResult();
                    if (dt_des != null && dt_des.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt_des.Rows)
                        {
                            _dt.Rows.Add(dr["woid"].ToString(),
                                         dr["material_no"],
                                         dr["qty"],
                                         dr["send_qty"],
                                         dr["PLANT"],
                                         dr["STORE_LOC"],
                                         "NA",
                                         dr["LIFNR"],
                                         "NA",
                                         dr["kpesc"],
                                         dr["sub_type"],
                                         4,
                                         "NA",
                                         "NA",
                                         "NA"
                                         );
                        }
                    }
                }

                return _dt;
            }
            catch (Exception ex)
            {
                Log.Error("GetOutSource exception:" + ex.Message);
                ShowHint("GetOutSource: " + ex.Message, Color.Red);
                return null;
            }
        }

        private bool InsertDeliveryRequisitonDetail(DataTable dt)
        {
            try
            {
                InsertDeliveryRequisitonDetail ir = new InsertDeliveryRequisitonDetail(dt);
                ir.ExecuteUpdate();

                return true;
            }
            catch(Exception ex)
            {
                ShowHint("InsertDeliveryRequisitonDetail:" + ex.Message, Color.Red);
                return false;
            }
        }

        private bool InsertReserveRequisitonDetail(DataTable dt)
        {
            try
            {
                InsertReserveRequisitonDetail ir = new InsertReserveRequisitonDetail(dt);
                ir.ExecuteUpdate();

                return true;
            }
            catch (Exception ex)
            {
                ShowHint("InsertReserveRequisitonDetail:" + ex.Message, Color.Red);
                return false;
            }
        }

        private bool InsertOutSourceRequisitonDetail(DataTable dt)
        {
            try
            {
                InsertOutSourceRequisitonDetail ir = new InsertOutSourceRequisitonDetail(dt);
                ir.ExecuteUpdate();

                return true;
            }
            catch (Exception ex)
            {
                ShowHint("InsertOutSourceRequisitonDetail:" + ex.Message, Color.Red);
                return false;
            }
        }

        private bool UpateTransferRequsitionDetailSubType(string woid, string subtype)
        {
            try
            {
                Dictionary<string, object> dic1 = new Dictionary<string, object>();
                dic1.Add("sub_type", subtype);

                Dictionary<string, object> dic2 = new Dictionary<string, object>();
                dic2.Add("woid", woid);

                UpateTransferRequsitionDetail ur = new UpateTransferRequsitionDetail(dic1, dic2);
                ur.ExecuteUpdate();

                return true;
            }
            catch(Exception ex)
            {
                ShowHint("UpateTransferRequsitionDetailSubType:" + ex.Message, Color.Red);
                return false;
            }
        }

        private bool UpateTransferRequsitionDetailStatus(string woid)
        {
            try
            {
                SearchTransferRequisitionDetail sr = new SearchTransferRequisitionDetail(woid);
                sr.ExecuteQuery();
                DataTable dt = sr.GetResult();

                if (dt==null || dt.Rows.Count==0)
                {
                    return true;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["status"].ToString().Equals("0") &&
                        (dr["qty"].ToString().Trim().Equals("0") ||
                        int.Parse(dr["qty"].ToString()) <= int.Parse(dr["send_qty"].ToString())))
                    {
                        Dictionary<string, object> dic1 = new Dictionary<string, object>();
                        dic1.Add("status", "1");

                        Dictionary<string, object> dic2 = new Dictionary<string, object>();
                        dic2.Add("id", int.Parse(dr["id"].ToString()));

                        UpateTransferRequsitionDetail ur = new UpateTransferRequsitionDetail(dic1, dic2);
                        ur.ExecuteUpdate();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowHint("UpateTransferRequsitionDetailSubType:" + ex.Message, Color.Red);
                return false;
            }
        }

        public bool UpdateDeliveryRequsitionDetailStatus(string stockno)
        {
            try
            {
                SearchRDeliveryRequisitionDetail sdr = new SearchRDeliveryRequisitionDetail(stockno);
                sdr.ExecuteQuery();
                DataTable dt = sdr.GetResult();

                if (dt==null || dt.Rows.Count == 0)
                {
                    return true;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["STATUS"].ToString().Equals("0") &&
                        (dr["qty"].ToString().Trim().Equals("0") ||
                        int.Parse(dr["qty"].ToString()) <= int.Parse(dr["send_qty"].ToString())))
                    {
                        Dictionary<string, object> dic1 = new Dictionary<string, object>();
                        dic1.Add("STATUS", "1");

                        Dictionary<string, object> dic2 = new Dictionary<string, object>();
                        dic2.Add("ID", dr["ID"].ToString());

                        UpateTransferRequsitionDetail ur = new UpateTransferRequsitionDetail(dic1, dic2);
                        ur.ExecuteUpdate();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowHint("UpdateDeliveryRequsitionDetailStatus:" + ex.Message, Color.Red);
                return false;
            }
        }

        public bool UpdateReserveRequisitionDetailStatus(string stockno)
        {
            try
            {
                SearchRReserveRequisitionDetail sdr = new SearchRReserveRequisitionDetail(stockno);
                sdr.ExecuteQuery();
                DataTable dt = sdr.GetResult();

                if (dt == null || dt.Rows.Count == 0)
                {
                    return true;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["STATUS"].ToString().Equals("0") &&
                        (dr["qty"].ToString().Trim().Equals("0") ||
                        int.Parse(dr["qty"].ToString()) <= int.Parse(dr["send_qty"].ToString())))
                    {
                        //
                        UpateReserveRequisitionDetail ur = new UpateReserveRequisitionDetail(int.Parse(dr["ID"].ToString()));
                        ur.ExecuteUpdate();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowHint("UpdateReserveRequisitionDetailStatus:" + ex.Message, Color.Red);
                return false;
            }
        }

        public bool UpdateOutSourceRequisitionDetailQty(string stockno, string materialno, string qty)
        {
            try
            {
                UpateOutSourceRequisitionQty us = new UpateOutSourceRequisitionQty(stockno, materialno, qty);
                us.ExecuteUpdate();                    

                return true;
            }
            catch (Exception ex)
            {
                ShowHint("UpdateOutSourceRequisitionDetailQty:" + ex.Message, Color.Red);
                return false;
            }
        }

        public bool UpdateOutSourceRequisitionDetailStatus(string stockno)
        {
            try
            {
                SearchOutSourceRequisitionDetail sr = new SearchOutSourceRequisitionDetail(stockno);
                sr.ExecuteQuery();
                DataTable dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    return true;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["STATUS"].ToString().Equals("0") &&
                        (dr["qty"].ToString().Trim().Equals("0") ||
                        int.Parse(dr["qty"].ToString()) <= int.Parse(dr["send_qty"].ToString())))
                    {
                        //
                        UpateOutSourceRequisitionDetail ur = new UpateOutSourceRequisitionDetail(int.Parse(dr["ID"].ToString()));
                        ur.ExecuteUpdate();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowHint("UpdateReserveRequisitionDetailStatus:" + ex.Message, Color.Red);
                return false;
            }
        }

        private bool UpdateSuperRequisitionDetail(string stockno)
        {
            try
            {
                SearchRequisitionDetail sdr = new SearchRequisitionDetail(stockno);
                sdr.ExecuteQuery();
                DataTable dt = sdr.GetResult();

                if (dt == null || dt.Rows.Count == 0)
                {
                    return true;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["status"].ToString().Equals("0") &&
                        (dr["qty"].ToString().Trim().Equals("0") ||
                        int.Parse(dr["qty"].ToString()) <= int.Parse(dr["send_qty"].ToString())))
                    {
                        //
                        UpateRequisitionDetail ur = new UpateRequisitionDetail(int.Parse(dr["ID"].ToString()));
                        ur.ExecuteUpdate();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowHint("UpateSuperRequisitionDetail:" + ex.Message, Color.Red);
                return false;
            }
        }

        //
        private bool UpdateDiscardRequisitionDetail(string stockno)
        {
            try
            {
                SearchRScrapRequisitionDetail sdr = new SearchRScrapRequisitionDetail(stockno);
                sdr.ExecuteQuery();
                DataTable dt = sdr.GetResult();

                if (dt == null || dt.Rows.Count == 0)
                {
                    return true;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["status"].ToString().Equals("0") &&
                        (dr["qty"].ToString().Trim().Equals("0") ||
                        int.Parse(dr["qty"].ToString()) <= int.Parse(dr["send_qty"].ToString())))
                    {
                        //
                        UpateScrapRequisitionDetail ur = new UpateScrapRequisitionDetail(int.Parse(dr["ID"].ToString()));
                        ur.ExecuteUpdate();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowHint("UpdateDiscardRequisitionDetail:" + ex.Message, Color.Red);
                return false;
            }
        }
    }
}
