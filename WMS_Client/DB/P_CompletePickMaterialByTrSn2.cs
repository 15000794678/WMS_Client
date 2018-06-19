using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class CompletePickMaterialByTrSn2 : BaseDbStoredProcedureCaller
    {
        private int _result = -99;

        public CompletePickMaterialByTrSn2(int stationId, string stockOutNo, string trSn, string locId, int stockouttype, int type)
            : base(DbName, "P_CompletePickMaterialByTrSn2",
                new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = stationId },
                new MySqlParameter("@pStockNo", MySqlDbType.String) { Value = stockOutNo },
                new MySqlParameter("@pTrSn", MySqlDbType.String) { Value = trSn },
                new MySqlParameter("@pBoxBarcode", MySqlDbType.String) { Value = locId },
                new MySqlParameter("@pStockOutType", MySqlDbType.Int32) { Value = stockouttype },
                new MySqlParameter("@pIsManual", MySqlDbType.Int32) { Value = type },
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
