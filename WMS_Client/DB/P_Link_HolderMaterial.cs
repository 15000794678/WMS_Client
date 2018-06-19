using System.Collections.Generic;
using System.Data;
using log4net;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class LinkHolderMaterial : BaseDbStoredProcedureCaller
    {
        private int _result = -99;

        public LinkHolderMaterial(string trSn, string stockno, string materialId, string dateCode, string fifoDateCode,
            int qty, int holderId, int stationId, string venderId, string woidtype, int stocknotype)
            : base(
                DbName, "P_Link_HolderMaterial",
                new MySqlParameter("@pTrSn", MySqlDbType.String) { Value = trSn },
                new MySqlParameter("@pStockNo", MySqlDbType.String) { Value = stockno },
                new MySqlParameter("@pMaterialNo", MySqlDbType.String) { Value = materialId },
                new MySqlParameter("@pDateCode", MySqlDbType.String) { Value = dateCode },
                new MySqlParameter("@pFifoDc", MySqlDbType.String) { Value = fifoDateCode },
                new MySqlParameter("@pQty", MySqlDbType.Int64) { Value = qty },
                new MySqlParameter("@pHolderId", MySqlDbType.String) { Value = holderId },
                new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = stationId },
                new MySqlParameter("@pVendorNo", MySqlDbType.String) { Value = venderId },
                new MySqlParameter("@pWoidType", MySqlDbType.String) { Value = woidtype },
                new MySqlParameter("@pStockNoType", MySqlDbType.Int32) { Value = stocknotype },
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
