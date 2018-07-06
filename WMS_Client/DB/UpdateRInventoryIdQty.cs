using System.Collections.Generic;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class UpdateRInventoryIdQty : BaseDbUpdater
    {
        private List<string> _sqlList = new List<string>();

        public UpdateRInventoryIdQty(string id, string qty, string goodqty, string badqty)
            : base("Update r_inventory_id Set qty='" + qty + "', goodproduct_qty='" + goodqty + "', badproduct_qty='" + badqty + "', status='2' Where id='" + id + "'", DbName)
        {

            _sqlList.Add(Sql);
        }

        protected override List<string> ProcessSql(string sql)
        {
            return _sqlList;
        }
    }
}