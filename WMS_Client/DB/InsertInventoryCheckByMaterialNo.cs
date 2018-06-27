using System.Collections.Generic;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class InsertInventoryCheckByMaterialNo : BaseDbUpdater
    {
        private List<string> _sqlList = new List<string>();

        public InsertInventoryCheckByMaterialNo(string checkdate, string kpno)
            : base("insert function", DbName)
        {
            string sql = "Insert Into inventorycheckbymaterialno (check_date,material_no,status) Values('" + checkdate + "','" + kpno + "', '0')";

            _sqlList.Add(sql);
        }

        protected override List<string> ProcessSql(string sql)
        {
            return _sqlList;
        }
    }
}