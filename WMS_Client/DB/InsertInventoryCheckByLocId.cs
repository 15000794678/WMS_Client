using System.Collections.Generic;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class InsertInventoryCheckByLocId : BaseDbUpdater
    {
        private List<string> _sqlList = new List<string>();

        public InsertInventoryCheckByLocId(string checkdate, string kpno, string plant, string stock_id)
            : base("insert function", DbName)
        {
            string sql = "Insert Into inventorycheckbylocid (check_date,loc_id,plant,stock_id,status) Values('" + checkdate + "','" + kpno + "','" + plant + "','" + stock_id + "', '0')";

            _sqlList.Add(sql);
        }

        protected override List<string> ProcessSql(string sql)
        {
            return _sqlList;
        }
    }
}