using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class SearchRHolderLocidRelationShip : BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchRHolderLocidRelationShip(Dictionary<string,object> dic)
            : base(
                "Select holder_id, loc_id, max_qty, on_qty, material_no, date_code from r_holder_locid_relationship ",
                DbName)
        {
            _dt.Columns.Add("holder_id", typeof(int));
            _dt.Columns.Add("loc_id", typeof(string));
            _dt.Columns.Add("max_qty", typeof(int));
            _dt.Columns.Add("on_qty", typeof(int));
            _dt.Columns.Add("material_no", typeof(string));
            _dt.Columns.Add("date_code", typeof(string));

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
                    _dt.Rows.Add(reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                 reader.IsDBNull(1) ? "" : reader.GetString(1),
                                 reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                                 reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                 reader.IsDBNull(4) ? "" : reader.GetString(4),
                                 reader.IsDBNull(5) ? "" : reader.GetString(5)
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
