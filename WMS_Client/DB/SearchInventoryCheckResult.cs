using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class SearchInventoryCheckResult : BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchInventoryCheckResult(Dictionary<string, object> dic)
            : base(
                "Select loc_id, material_no, sys_qty, sys_reel_cnt, plant, stock_id, real_qty, real_reel_cnt, user, date, remark from  inventorycheckresult ",
                DbName)
        {
            _dt.Columns.Add("LocId", typeof(string));
            _dt.Columns.Add("KpNo", typeof(string));
            _dt.Columns.Add("Qty", typeof(int));
            _dt.Columns.Add("Count", typeof(int));
            _dt.Columns.Add("Plant", typeof(string));
            _dt.Columns.Add("StockId", typeof(string));
            _dt.Columns.Add("RealQty", typeof(int));
            _dt.Columns.Add("RealCount", typeof(int));
            _dt.Columns.Add("User", typeof(string));
            _dt.Columns.Add("Date", typeof(string));
            _dt.Columns.Add("Remark", typeof(string));

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
            List<string> sqlList = new List<string> { sql };

            return sqlList;
        }

        protected override void ProcessResultSet(MySqlDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    _dt.Rows.Add(reader.IsDBNull(0) ? "" : reader.GetString(0),
                                 reader.IsDBNull(1) ? "" : reader.GetString(1),
                                 reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                                 reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                 reader.IsDBNull(4) ? "" : reader.GetString(4),
                                 reader.IsDBNull(5) ? "" : reader.GetString(5),
                                 reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                 reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                 reader.IsDBNull(8) ? "" : reader.GetString(8),
                                 reader.IsDBNull(9) ? "" : reader.GetString(9),
                                 reader.IsDBNull(10) ? "" : reader.GetString(10)
                                 );
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

        public DataTable GetResult()
        {
            return _dt;
        }
    }
}
