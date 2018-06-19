using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class SearchTaskCount : BaseDbQuery
    {
        private int _cnt = 0;

        public SearchTaskCount(int id, int stationId)
            : base("select count(distinct(Stock_No)) from tb_insert_trsn", DbName)
        {
            if (id==0)
            {
                Sql = "select count(distinct(Stock_No)) from tb_insert_trsn";
            }
            else if (id == 1)
            {
                Sql = "select count(DISTINCT(PodId)) from v_lk_holderpod_buffer where StationId='" + stationId + "'";
            }
            else if (id==2)
            {
                Sql = "select count(distinct(PodId)) from materialpickassign where StationId='" + stationId + "'";
            }
            else if (id==3)
            {
                Sql = "select count(distinct(PodId)) from material_manual_pick where StationId='" + stationId + "'";
            }
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
                while (reader.Read())
                {
                    _cnt = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                }
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

        public int GetResult()
        {
            return _cnt;
        }
    }
}
