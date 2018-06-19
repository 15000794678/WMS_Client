using System.Collections.Generic;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class InsertTrSn : BaseDbUpdater
    {
        private List<string> _sqlList = new List<string>();

        public InsertTrSn(List<string> trsn, string woid, int stockNoType)
            : base("insert function", DbName)
        {
            foreach (string sn in trsn)
            {
                string sql = "Insert Into tb_insert_trsn (Tr_Sn,Stock_No,StockNo_Type,UPLOAD,Time) Values('" +
                    sn + "','" + woid + "','" + stockNoType.ToString() + "','N','" + System.DateTime.Now.ToString() + "')";

                _sqlList.Add(sql);
            }
        }

        protected override List<string> ProcessSql(string sql)
        {
            return _sqlList;
        }
    }
}