using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class CompletePickMaterialByTrSn : BaseDbStoredProcedureCaller
    {
        private int _result = -99;

        public CompletePickMaterialByTrSn(int stationId, string stockOutNo, string trSn, string locId, int stockouttype)
            : base(DbName, "P_CompletePickMaterialByTrSn",
                new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = stationId },
                new MySqlParameter("@pStockNo", MySqlDbType.String) { Value = stockOutNo },
                new MySqlParameter("@pTrSn", MySqlDbType.String) { Value = trSn },
                new MySqlParameter("@pBoxBarcode", MySqlDbType.String) { Value = locId },
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
