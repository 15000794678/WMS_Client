using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class CompletePartialPicklByTrSn : BaseDbStoredProcedureCaller
    {
        private int _result = -99;
        private int _num = 0;

        public CompletePartialPicklByTrSn(int stationId, string stockOutNo, string trSn, string locId, int stockouttype)
            : base(DbName, "P_CompletePartialPickByTrSn",
                new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = stationId },
                new MySqlParameter("@pStockNo", MySqlDbType.String) { Value = stockOutNo },
                new MySqlParameter("@pTrSn", MySqlDbType.String) { Value = trSn },
                new MySqlParameter("@pBoxBarcode", MySqlDbType.String) { Value = locId },
                new MySqlParameter("@pStockOutType", MySqlDbType.Int32) { Value = stockouttype },
                new MySqlParameter("@pSendQty", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
                new MySqlParameter("@pResult", MySqlDbType.Int32) { Direction = ParameterDirection.Output })
        {
        }

        protected override void ProcessResult(MySqlDataReader reader)
        {
            try
            {
                _result = (int)DicParameters["@pResult"].Value;
                _num = (int)DicParameters["@pSendQty"].Value;
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

        public int GetResult()
        {
            return _result;
        }

        public int GetNum()
        {
            return _num;
        }
    }
}
