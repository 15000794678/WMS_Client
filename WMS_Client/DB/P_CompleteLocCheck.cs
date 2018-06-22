using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class CompleteLocCheck : BaseDbStoredProcedureCaller
    {
        private string _result = "";

        public CompleteLocCheck(string locid, string kpno, int qty, int cnt, int realqty, int realcnt, string plant, 
                        string stockid, string user, string remark)
            : base(DbName, "P_CompleteLocCheck",
                new MySqlParameter("@pLocId", MySqlDbType.String) { Value = locid },
                new MySqlParameter("@pMaterialNo", MySqlDbType.String) { Value = kpno },
                new MySqlParameter("@pSysQty", MySqlDbType.Int32) { Value = qty },
                new MySqlParameter("@pSysReelCnt", MySqlDbType.Int32) { Value = cnt },
                new MySqlParameter("@pRealQty", MySqlDbType.Int32) { Value = realqty },
                new MySqlParameter("@pRealReelCnt", MySqlDbType.Int32) { Value = realcnt },
                new MySqlParameter("@pPlant", MySqlDbType.String) { Value = plant },
                new MySqlParameter("@pStockId", MySqlDbType.String) {Value = stockid  },
                new MySqlParameter("@pUser", MySqlDbType.String) { Value = user },
                new MySqlParameter("@pRemark", MySqlDbType.String) {Value = remark }
                )
        {
        }

        protected override void ProcessResult(MySqlDataReader reader)
        {
            try
            {
                if (reader.Read())
                {
                    _result = reader.GetString("Result");
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader?.Close();
            }
        }

        public string GetResult()
        {
            return _result;
        }
    }
}
