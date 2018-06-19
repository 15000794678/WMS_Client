using System.Collections.Generic;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class InsertOutSourceRequisitonDetail : BaseDbUpdater
    {
        private List<string> _sqlList = new List<string>();

        public InsertOutSourceRequisitonDetail(DataTable dt)
            : base("insert function", DbName)
        {
            foreach (DataRow dr in dt.Rows)
            {
                string sql = "Insert Into r_outsource_requisition_detail (woid,material_no,qty,send_qty,PLANT,STORE_LOC,LIFNR,sub_type,LOEKZ,kpesc,MEINS) Values('" +
                    dr["WOID"].ToString() + "','" + dr["KP_NO"].ToString() + "','" + dr["QTY"].ToString() + "','" + dr["Send_QTY"].ToString() + "','" +
                    dr["FromFactory"].ToString() + "','" + dr["FromLoc"].ToString() + "','" +
                    dr["ToFactory"].ToString() + "','" + dr["SubType"].ToString() + "','0','" + dr["KPDESC"].ToString() + "','PC')";

                _sqlList.Add(sql);
            }
        }

        protected override List<string> ProcessSql(string sql)
        {
            return _sqlList;
        }
    }
}