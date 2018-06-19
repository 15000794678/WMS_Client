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
        }
    }
}