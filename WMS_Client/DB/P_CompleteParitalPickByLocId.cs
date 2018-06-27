using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class CompletePartialPickByLocId : BaseDbStoredProcedureCaller
    {
        private int _result = -99;
        //private List<string> _listTrSn = new List<string>();

        public CompletePartialPickByLocId(int stationId, string stockOutNo, string locId, int stockouttype)
            : base(DbName, "P_CompletePartialPickByLocId",
            new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = stationId },
            new MySqlParameter("@pStockNo", MySqlDbType.String) { Value = stockOutNo },
            new MySqlParameter("@pBoxBarcode", MySqlDbType.Int32) { Value = locId },
            new MySqlParameter("@pStockOutType", MySqlDbType.Int32) { Value = stockouttype },
            new MySqlParameter("@pResult", MySqlDbType.Int32) { Direction = ParameterDirection.Output })
        {

        }

        public int GetResult()
        {
            return _result;
        }

        protected override void ProcessResult(MySqlDataReader reader)
        {
            try
            {
                
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

        protected override void ProcessParms()
        {
            try
            {
                _result = (int)DicParameters["@pResult"].Value;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
