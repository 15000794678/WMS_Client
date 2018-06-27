using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class CheckMaterialIntoHolders2 : BaseDbStoredProcedureCaller
    {
        private string _promoteText = string.Empty;

        public CheckMaterialIntoHolders2(int pickStationId, string stockNo, int pIsManual) :
            base(DbName, "P_CheckAllMaterialIntoHolders2",
                new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = pickStationId },
                new MySqlParameter("@pStockNo", MySqlDbType.String) { Value = stockNo },
                new MySqlParameter("@pIsManual", MySqlDbType.Int32) { Value = pIsManual },
                new MySqlParameter("@pResult", MySqlDbType.String) { Direction = ParameterDirection.Output })
        {
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
                _promoteText = (string)DicParameters["@pResult"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetResult()
        {
            return _promoteText;
        }
    }
}
