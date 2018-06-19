using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class WorkingStateChecker : BaseDbQuery
    {
        private int _result = -3;

        public WorkingStateChecker(int stationId)
            : base("Select " + "F_CheckStationWorkingState(" + stationId + ")", DbName)
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
                    _result = reader.IsDBNull(0)? 0:reader.GetInt32(0);
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

        /*
         概述：
            每个工作站在生成叫车任务前先调用该函数进行站点可操作判断。如果该站点还有上次遗留的任务没有完成就不能进行后续生成搬运任务的动作。
            参数：
            pStationId: 调用该函数的站点编号
            返回值：
            0: 检查无误，可以进行后续操作
            -1：当前站点还有未完成的任务
            -2：当前站点不存在，先检查数据库配置            
         */
    }
}
