using System.Collections.Generic;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class InsertRInventoryId : BaseDbUpdater
    {
        private List<string> _sqlList = new List<string>();

        public InsertRInventoryId(string woid, string kpno, string qty, string goodqty, string badqty, string status, string kpdesc, string stockno_type, string subtype)
            : base("insert function", DbName)
        {            
            string sql = "Insert Into r_inventory_id (stock_no, material_no, qty, goodproduct_qty, badproduct_qty, status, material_desc, stockno_type, sub_type) Values('" +
                woid + "','" + kpno + "','" + qty + "','" + goodqty + "','" + badqty + "','" + 
                status + "','" + kpdesc + "','" + stockno_type + "','" + subtype + "')";

            _sqlList.Add(sql);            
        }

        protected override List<string> ProcessSql(string sql)
        {
            return _sqlList;
        }
    }
}