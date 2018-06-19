using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class DeinitProcess : BaseDbStoredProcedureCaller
    {
        public DeinitProcess(int stationId) : 
            base(DbName, "P_DeinitProcess",
            new MySqlParameter("@pStationId", MySqlDbType.Int32) { Value = stationId })
        {

        }

        protected override void ProcessResult(MySqlDataReader reader)
        {
        }
    }
}
