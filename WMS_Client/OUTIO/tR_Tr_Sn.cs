using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Phicomm_WMS.OUTIO
{
    class tR_Tr_Sn
    {
        public static bool InsertRTrSn(string woid, DataTable dt, ref string result)
        {
            try
            {
                WMS_Client.tR_Tr_Sn.tR_Tr_Sn tt = new WMS_Client.tR_Tr_Sn.tR_Tr_Sn();                

                List<WMS_Client.tR_Tr_Sn.TrSnFields> listField = new List<WMS_Client.tR_Tr_Sn.TrSnFields>();

                foreach (DataRow dr in dt.Rows)
                {
                    WMS_Client.tR_Tr_Sn.TrSnFields item = new WMS_Client.tR_Tr_Sn.TrSnFields();
                    item.PO_ID = "NA";
                    item.TR_SN = dr["TR_SN"].ToString();
                    item.KP_NO = dr["KP_NO"].ToString();
                    item.KP_DESC = dr["KP_DESC"].ToString();
                    item.VENDER_ID = dr["VENDER_ID"].ToString();
                    item.VENDER_NAME = dr["VENDER_NAME"].ToString();
                    item.DATE_CODE = dr["DATE_CODE"].ToString();
                    item.LOT_CODE = string.IsNullOrEmpty(dt.Rows[0]["lot_code"].ToString().Trim()) ? "NA" : dr["lot_code"].ToString();
                    item.QTY = int.Parse(dr["QTY"].ToString());
                    item.STOCK_ID = dr["STOCK_ID"].ToString();
                    item.LOC_ID = dr["LOC_ID"].ToString();
                    item.TANSFER_NO = "NA";
                    item.URGENT_PN = "NA";
                    item.STOCK_NO = woid;
                    item.STOCK_DATE = Convert.ToDateTime(dr["INVENTORY_DATE"].ToString());
                    item.WOID = woid;
                    item.STATUS = dr["STATUS"].ToString();
                    item.REMARK1 = "NA";
                    item.REMARK2 = "NA";
                    item.UPDATE_DATE = DateTime.Now;
                    item.FIFO_DC = dr["FIFO_DATECODE"].ToString();

                    listField.Add(item);
                }

                WMS_Client.tR_Tr_Sn.TrSnFields[] itemField = listField.ToArray();

                result = tt.Insert_R_TrSn(itemField);
                if (result.ToUpper().Trim().Equals("OK"))
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
                throw ex;
            }
        }

        //冯东接口
        public static bool DelTrSn(string trsn)
        {
            try
            {
                WMS_Client.tR_Tr_Sn.tR_Tr_Sn tt = new WMS_Client.tR_Tr_Sn.tR_Tr_Sn();                
                string res = tt.Del_TR_SN(trsn);
                if (res.ToUpper().Trim().ToUpper().Equals("OK"))
                {
                    return true;
                }

                throw new Exception(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
