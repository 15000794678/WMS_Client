using log4net;
using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class CheckMaterialIntoHolders : BaseDbStoredProcedureCaller
    {
        private string _promoteText = string.Empty;

        public CheckMaterialIntoHolders(int pickStationId, string stockNo) :
            base(DbName, "P_CheckAllMaterialIntoHolders",
                new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = pickStationId },
                new MySqlParameter("@pStockNo", MySqlDbType.String) { Value = stockNo },
                new MySqlParameter("@pResult", MySqlDbType.String) { Direction = ParameterDirection.Output })
        {
        }

        protected override void ProcessResult(MySqlDataReader reader)
        {
            try
            {
                _promoteText = DicParameters["@pResult"].Value.ToString();
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
            return _promoteText;
        }
    }
}
