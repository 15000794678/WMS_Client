using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class InitPartialPickStation : BaseDbQuery
    {
        public InitPartialPickStation(string user, int stationId)
            : base("Select " + "F_InitPartialPickStation(" + stationId + ",'" + user + "')", DbName)
        {

        }

        protected override void ProcessResultSet(MySqlDataReader reader)
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

        protected override List<string> ProcessSql(string sql)
        {
            List<string> sqlList = new List<string> { sql };
            return sqlList;
        }
    }
}
