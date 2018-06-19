using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class BotContinueWorking : BaseDbQuery
    {
        private int _result;

        public BotContinueWorking(int stationId)
            : base("select " + "F_AtStationBotContinueWorking(" + stationId + ")", DbName
                )
        {
            
        }

        protected override List<string> ProcessSql(string sql)
        {
            List<string> sqlList = new List<string> { sql };
            return sqlList;
        }

        protected override void ProcessResultSet(MySqlDataReader reader)
        {
            try
            {
                if (reader.Read())
                {
                    _result = reader.IsDBNull(0)?0:reader.GetInt32(0);
                }
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
