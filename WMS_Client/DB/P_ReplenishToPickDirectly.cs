using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class ReplenishToPickDirectly : BaseDbStoredProcedureCaller
    {
        private int _result = -99;
        
        public ReplenishToPickDirectly(int stationId, string stockInNo, int stockInType, string subType, string stockOutNo, int stockOutType,
                string trSn, string materialNo, string dateCode, string fifoDc, int qty, string vendorNo)
            : base(DbName, "P_ReplenishToPickDirectly",
                new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = stationId },
                new MySqlParameter("@pStockInNo", MySqlDbType.String) { Value = stockInNo },
                new MySqlParameter("@pStockInNoType", MySqlDbType.Int32) { Value = stockInType},
                new MySqlParameter("@pWoidType", MySqlDbType.Int32) { Value = subType },
                new MySqlParameter("@pStockOutNo", MySqlDbType.String) { Value = stockOutNo },
                new MySqlParameter("@pStockOutNoType", MySqlDbType.Int32) { Value = stockOutType },
                new MySqlParameter("@pTrSn", MySqlDbType.String) { Value = trSn },
                new MySqlParameter("@pMaterialNo", MySqlDbType.String) { Value = materialNo },
                new MySqlParameter("@pDateCode", MySqlDbType.String) { Value = dateCode },
                new MySqlParameter("@pFifoDc", MySqlDbType.String) { Value = fifoDc },
                new MySqlParameter("@pQty", MySqlDbType.Int32) { Value = qty },
                new MySqlParameter("@pVendorNo", MySqlDbType.String) { Value = vendorNo },
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
