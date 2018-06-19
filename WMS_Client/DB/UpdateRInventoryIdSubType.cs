using System.Collections.Generic;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class UpdateRInventoryIdSubType : BaseDbUpdater
    {
        private List<string> _sqlList = new List<string>();

        public UpdateRInventoryIdSubType(string woid, string subtype)
            : base("Update r_inventory_id Set sub_type='" + subtype + "' Where stock_no='" + woid + "'", DbName)
        {
            _sqlList.Add(Sql);
        }

        protected override List<string> ProcessSql(string sql)
        {
            return _sqlList;
        }
    }
}