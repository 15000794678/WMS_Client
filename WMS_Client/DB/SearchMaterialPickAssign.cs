using System.Collections.Generic;
using MySql.Data.MySqlClient;

using System.Data;

namespace Phicomm_WMS.DB
{
    public class SearchMaterialPickAssign : BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchMaterialPickAssign(Dictionary<string, object> dic)
            : base(
                "Select StationId,StockOutNo,MaterialId,FifoDC,PodId,PodSide,ShelfId,BoxBarCode,BoxId,Qty, ProcessStatus,TaskId from materialpickassign",
                DbName)
        {
            _dt.Columns.Add("StationId", typeof(int));
            _dt.Columns.Add("StockOutNo", typeof(string));
            _dt.Columns.Add("MaterialId", typeof(string));
            _dt.Columns.Add("FifoDC", typeof(string));
            _dt.Columns.Add("PodId", typeof(string));
            _dt.Columns.Add("PodSide", typeof(string));
            _dt.Columns.Add("ShelfId", typeof(string));
            _dt.Columns.Add("BoxBarCode", typeof(string));
            _dt.Columns.Add("BoxId", typeof(int));
            _dt.Columns.Add("Qty", typeof(int));
            _dt.Columns.Add("ProcessStatus", typeof(int));
            _dt.Columns.Add("TaskId", typeof(int));

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
                    DataRow dr = _dt.NewRow();
                    dr["StationId"] = reader.IsDBNull(0) ? 0:reader.GetInt32(0);
                    dr["StockOutNo"] = reader.IsDBNull(1) ? "":reader.GetString(1);
                    dr["MaterialId"] = reader.IsDBNull(2) ? "":reader.GetString(2);
                    dr["FifoDC"] = reader.IsDBNull(3) ? "":reader.GetString(3);
                    dr["PodId"] = reader.IsDBNull(4) ? "":reader.GetString(4);
                    dr["PodSide"] = reader.IsDBNull(5) ? "":reader.GetString(5);
                    dr["ShelfId"] = reader.IsDBNull(6) ? "" : reader.GetString(6);
                    dr["BoxBarCode"] = reader.IsDBNull(7) ? "" : reader.GetString(7);
                    dr["BoxId"] = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
                    dr["Qty"] = reader.IsDBNull(9) ? 0 : reader.GetInt32(9);
                    dr["ProcessStatus"] = reader.IsDBNull(10) ? 0 : reader.GetInt32(10);
                    dr["TaskId"] = reader.IsDBNull(11) ? 0 : reader.GetInt32(11);

                    _dt.Rows.Add(dr);
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
