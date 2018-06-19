using System.Collections.Generic;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class InsertDeliveryRequisitonDetail : BaseDbUpdater
    {
        private List<string> _sqlList = new List<string>();

        public InsertDeliveryRequisitonDetail(DataTable dt)
            : base("insert function", DbName)
        {
            foreach (DataRow dr in dt.Rows)
            {
                string sql = "Insert Into r_delivery_requisition_detail (woid,material_no,qty,send_qty,PLANT,STORE_LOC,MOVE_TYPE,MOVE_PLANT,MOVE_STLOC,sub_type) Values('" +
                    dr["WOID"].ToString() + "','" + dr["KP_NO"].ToString() + "','" + dr["QTY"].ToString() + "','" + dr["Send_QTY"].ToString() + "','" +
                    dr["FromFactory"].ToString() + "','" + dr["FromLoc"].ToString() + "','" +
                    dr["MoveType"].ToString() + "','" + dr["ToFactory"].ToString() + "','" + dr["ToLoc"].ToString() + "','" +
                    dr["SubType"].ToString() + "')";

                _sqlList.Add(sql);
            }
        }

        protected override List<string> ProcessSql(string sql)
        {
            return _sqlList;
        }
    }
}