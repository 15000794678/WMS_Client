using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Phicomm_WMS.DB
{
    public class StopPickTask : BaseDbQuery
    {
        public StopPickTask(int stationId)
            : base("Select " + "F_StopPickTask(" + stationId + ")", DbName)
        {

        }

        protected override void ProcessResultSet(MySqlDataReader reader)
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

        protected override List<string> ProcessSql(string sql)
        {
            List<string> sqlList = new List<string> { sql };
            return sqlList;
        }
    }
}
