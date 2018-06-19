using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class CompleteReelSplit : BaseDbStoredProcedureCaller
    {
        private int _result = -99;

        public CompleteReelSplit(int stationId, string stockOutNo, int stockouttype, string oldTrSn, string newTrSn, int sendQty)
            : base(DbName, "P_CompleteReelSplit",
                new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = stationId },
                new MySqlParameter("@pStockOutNo", MySqlDbType.String) { Value = stockOutNo },
                new MySqlParameter("@pStockOutType", MySqlDbType.Int32) { Value = stockouttype },
                new MySqlParameter("@pOldTrSn", MySqlDbType.String) { Value = oldTrSn },
                new MySqlParameter("@pNewTrSn", MySqlDbType.String) { Value = newTrSn },
                new MySqlParameter("@pSendQty", MySqlDbType.Int32) { Value = sendQty },
                new MySqlParameter("@pResult", MySqlDbType.Int32) { Direction = ParameterDirection.Output })
        {
        }

        protected override void ProcessResult(MySqlDataReader reader)
        {
            try
            {
                _result = (int)DicParameters["@pResult"].Value;
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
    }
}
