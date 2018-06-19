using System.Collections.Generic;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class UpdateTbInsertTrSn : BaseDbUpdater
    {
        private List<string> _sqlList = new List<string>();

        public UpdateTbInsertTrSn(string woid, string trsn)
            : base("Update tb_insert_trsn Set UPLOAD='Y' Where Stock_No='" + woid + "' AND Tr_Sn='" + trsn + "'", DbName)
        {
            _sqlList.Add(Sql);
        }

        protected override List<string> ProcessSql(string sql)
        {
            return _sqlList;
        }
    }
}