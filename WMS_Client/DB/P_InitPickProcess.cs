using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class InitPickProcess : BaseDbStoredProcedureCaller
    {

        public InitPickProcess(string employeeId, int stationId)
            : base(DbName, "P_InitPickProcess",
                new MySqlParameter("@pStationID", MySqlDbType.Int32) { Value = stationId },
                new MySqlParameter("@pEmployeeId", MySqlDbType.String) { Value = employeeId })
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
                
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}