using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class SearchTbFactoryMove: BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchTbFactoryMove(Dictionary<string, object> dic)
            : base(
                "Select StockNoType,WoidType,StepType,MoveType,MoveName,FromFactory,FromStockId,ToFactory,ToStockId from tb_factory_move",
                DbName)
        {
            _dt.Columns.Add("StockNoType", typeof(int));
            _dt.Columns.Add("WoidType", typeof(string));
            _dt.Columns.Add("StepType", typeof(int));
            _dt.Columns.Add("MoveType", typeof(string));
            _dt.Columns.Add("MoveName", typeof(string));
            _dt.Columns.Add("FromFactory", typeof(string));
            _dt.Columns.Add("FromStockId", typeof(string));
            _dt.Columns.Add("ToFactory", typeof(string));
            _dt.Columns.Add("ToStockId", typeof(string));

            if (dic != null && dic.Count > 0)
            {
                Sql += " Where ";
                foreach (KeyValuePair<string, object> obj in dic)
                {
                    Sql += " " + obj.Key + "='" + obj.Value + "' AND ";
                }
                Sql = Sql.Substring(0, Sql.Length - 5);
            }
        }

        protected override List<string> ProcessSql(string sql)
        {
            List<string> sqlList = new List<string> {sql};
            return sqlList;
        }

        protected override void ProcessResultSet(MySqlDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = _dt.NewRow();

                    dr["StockNoType"] = reader.IsDBNull(0)? 0:reader.GetInt32(0);
                    dr["WoidType"] = reader.IsDBNull(1)? 0:reader.GetInt32(1);
                    dr["StepType"] = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                    dr["MoveType"] = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    dr["MoveName"] = reader.IsDBNull(4) ? "" : reader.GetString(4);
                    dr["FromFactory"] = reader.IsDBNull(5) ? "" : reader.GetString(5);
                    dr["FromStockId"] = reader.IsDBNull(6) ? "" : reader.GetString(6);
                    dr["ToFactory"] = reader.IsDBNull(7) ? "" : reader.GetString(7);
                    dr["ToStockId"] = reader.IsDBNull(8) ? "" : reader.GetString(8);

                    _dt.Rows.Add(dr);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader?.Close();
            }
        }

        public DataTable GetResult()
        {
            return _dt;
        }
    }
}
