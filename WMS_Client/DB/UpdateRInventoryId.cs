using System.Collections.Generic;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class UpdateRInventoryIdStatus : BaseDbUpdater
    {
        private List<string> _sqlList = new List<string>();

        public UpdateRInventoryIdStatus(string woid, string kpno, string oldstatus, string newstatus, int stationId)
            : base("Update r_inventory_id Set status='" + newstatus + "', station_id='" + stationId + "' Where stock_no='" + woid + "' and material_no='" + kpno + "' and status='" + oldstatus + "'", DbName)
        {
            _sqlList.Add(Sql);
        }

        protected override List<string> ProcessSql(string sql)
        {
            return _sqlList;
        }
    }
}