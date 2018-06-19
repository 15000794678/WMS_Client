using System.Collections.Generic;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class InsertErpWoBomInfo : BaseDbUpdater
    {
        private List<string> _sqlList = new List<string>();

        public InsertErpWoBomInfo(DataRow dr)
            : base("insert function", DbName)
        {
            //foreach (DataRow dr in dt.Rows)
            {
                string sql = "Insert Into r_erp_wo_bom_info (woid,material_no,factory,loc,qty,send_qty,move_type,kpdesc,process,rel_requireid,rel_projectid,createtime,UNIT_QTY,sub_type) Values('" +
                    dr[0].ToString() + "','" + dr[1].ToString() + "','" + dr[2].ToString() + "','" + dr[3].ToString() + "','" + dr[4].ToString() + "','" +
                    dr[5].ToString() + "','" + dr[6].ToString() + "','" + dr[7].ToString() + "','" + dr[8].ToString() + "','" +
                    dr[9].ToString() + "','" + dr[10].ToString() + "','" + dr[11].ToString() + "','" + dr[12].ToString() + "','" +
                    dr[13].ToString() + "')";

                _sqlList.Add(sql);
                
            }
        }

        protected override List<string> ProcessSql(string sql)
        {
            return _sqlList;
        }
    }
}