using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class CompleteReplenishMaterial : BaseDbStoredProcedureCaller
    {
        private int _result = -99;

        public CompleteReplenishMaterial(int stationId, string boxBarcode, int holderId, string stockno)
            : base(DbName, "P_CompleteReplenishMaterial",
                  new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = stationId },
                  new MySqlParameter("@pBoxBarcode", MySqlDbType.String) { Value = boxBarcode },
                  new MySqlParameter("@pHolderId", MySqlDbType.Int32) { Value = holderId },
                  new MySqlParameter("@pStockNo", MySqlDbType.String) { Value = stockno },
                  new MySqlParameter("@pResult", MySqlDbType.Int32) { Direction = ParameterDirection.Output })
        {
        }

        protected override void ProcessResult(MySqlDataReader reader)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader?.Close();
            }
        }

        protected override void ProcessParms()
        {
            try
            {
                _result = (int)DicParameters["@pResult"].Value;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public int GetResult()
        {
            return _result;
        }
    }
}
