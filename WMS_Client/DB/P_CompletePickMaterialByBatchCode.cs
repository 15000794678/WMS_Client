using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class CompletePickMaterialByBatchCode : BaseDbStoredProcedureCaller
    {
        private int _result = -99;

        public CompletePickMaterialByBatchCode(int stationId, string stockOutNo, string locId, string trSn, string materialNo,
            int qty, int stockouttype)
            : base(DbName, "P_CompletePickMaterialByBatchCode",
                new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = stationId },
                new MySqlParameter("@pStockNo", MySqlDbType.String) { Value = stockOutNo },
                new MySqlParameter("@pBoxBarcode", MySqlDbType.String) { Value = locId },
                new MySqlParameter("@pTrSn", MySqlDbType.String) { Value = trSn },
                new MySqlParameter("@pMaterialNo", MySqlDbType.String) { Value = materialNo },
                new MySqlParameter("@pQty", MySqlDbType.Int32) { Value = qty },
                new MySqlParameter("@pStockOutType", MySqlDbType.Int32) { Value = stockouttype },
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
