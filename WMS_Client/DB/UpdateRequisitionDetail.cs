using System.Collections.Generic;
using System.Linq;

namespace Phicomm_WMS.DB
{
    public class UpateRequisitionDetail : BaseDbUpdater
    {

        private List<string> _sqlList;
  
        public UpateRequisitionDetail(int ID) :
            base("Update r_requisition_detail SET STATUS='1' where ID='" + ID.ToString() + "'", DbName)
        {

        }


        protected override List<string> ProcessSql(string sql)
        {
            _sqlList = new List<string> { sql };
            return _sqlList;
        }
    }
}