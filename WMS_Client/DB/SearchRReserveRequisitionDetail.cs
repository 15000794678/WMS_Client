using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System;

namespace Phicomm_WMS.DB
{
    public class SearchRReserveRequisitionDetail : BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchRReserveRequisitionDetail(string woid)
            : base(
                "Select woid, material_no, qty, send_qty, PLANT, STORE_LOC, MOVE_TYPE, MOVE_PLANT, MOVE_STLOC, STATUS, sub_type, ID From r_reserve_requisition_detail Where woid='" + woid + "'",
                DbName)
        {
            _dt.Columns.Add("woid", typeof(string));
            _dt.Columns.Add("material_no", typeof(string));
            _dt.Columns.Add("qty", typeof(int));
            _dt.Columns.Add("send_qty", typeof(int));
            _dt.Columns.Add("PLANT", typeof(string));
            _dt.Columns.Add("STORE_LOC", typeof(string));
            _dt.Columns.Add("MOVE_TYPE", typeof(string));
            _dt.Columns.Add("MOVE_PLANT", typeof(string));
            _dt.Columns.Add("MOVE_STLOC", typeof(string));
            _dt.Columns.Add("STATUS", typeof(string));
            _dt.Columns.Add("sub_type", typeof(string));
            _dt.Columns.Add("ID", typeof(int));
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
                                 reader.IsDBNull(6) ? "" : reader.GetString(6),
                                 reader.IsDBNull(7) ? "" : reader.GetString(7),
                                 reader.IsDBNull(8) ? "" : reader.GetString(8),
                                 reader.IsDBNull(9) ? "" : reader.GetString(9),
                                 reader.IsDBNull(10) ? "" : reader.GetString(10),
                                 reader.IsDBNull(11) ? 0 : reader.GetInt32(11)
                        );
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

        public DataTable GetResult()
        {
            return _dt;
        }
    }
}
