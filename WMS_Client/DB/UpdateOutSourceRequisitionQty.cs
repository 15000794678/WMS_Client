using System.Collections.Generic;
using System.Linq;

namespace Phicomm_WMS.DB
{
    public class UpateOutSourceRequisitionQty : BaseDbUpdater
    {
        private List<string> _sqlList;

        public UpateOutSourceRequisitionQty(string stockno, string materialno, string qty) :
            base("Update r_outsource_requisition_detail SET qty='" + qty + "' Where woid='" + stockno + "' and material_no='" + materialno + "'", DbName)
        {

        }


        protected override List<string> ProcessSql(string sql)
        {
            _sqlList = new List<string> { sql };
            return _sqlList;
        }
    }
}