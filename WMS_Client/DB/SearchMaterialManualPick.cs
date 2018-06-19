using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class SearchMaterialManualPick : BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchMaterialManualPick(Dictionary<string, object> dic)
            : base(
                "Select StationId, StockOutNo, MaterialId, FifoDC, PodId, PodSide, ShelfId, BoxBarcode, BoxId, Qty from  material_manual_pick ",
                DbName)
        {
            _dt.Columns.Add("StationId", typeof(int));
            _dt.Columns.Add("StockOutNo", typeof(string));
            _dt.Columns.Add("MaterialId", typeof(string));
            _dt.Columns.Add("FifoDC", typeof(string));
            _dt.Columns.Add("PodId", typeof(string));
            _dt.Columns.Add("PodSide", typeof(string));
            _dt.Columns.Add("ShelfId", typeof(string));
            _dt.Columns.Add("BoxBarcode", typeof(string));
            _dt.Columns.Add("BoxId", typeof(int));
            _dt.Columns.Add("Qty", typeof(int));

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
                                 reader.IsDBNull(2) ? "" : reader.GetString(2),
                                 reader.IsDBNull(3) ? "" : reader.GetString(3),
                                 reader.IsDBNull(4) ? "" : reader.GetString(4),
                                 reader.IsDBNull(5) ? "" : reader.GetString(5),
                                 reader.IsDBNull(6) ? "" : reader.GetString(6),
                                 reader.IsDBNull(7) ? "" : reader.GetString(7),
                                 reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                                 reader.IsDBNull(9) ? 0 : reader.GetInt32(9)
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
