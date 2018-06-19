using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class SearchTransferRequisitionDetail : BaseDbQuery
    {
        private DataTable _dt = new DataTable();
        
        public SearchTransferRequisitionDetail(string woid)
            : base(
                "Select woid, material_no, qty, send_qty, res_stock_no, aim_stock_no, factory_id, status, sub_type, id from r_transfer_requisition_detail where woid='" + woid + "'",
                DbName)
        {
            _dt.Columns.Add("woid", typeof(string));
            _dt.Columns.Add("material_no", typeof(string));
            _dt.Columns.Add("qty", typeof(int));
            _dt.Columns.Add("send_qty", typeof(int));
            _dt.Columns.Add("res_stock_no", typeof(string));
            _dt.Columns.Add("aim_stock_no", typeof(string));
            _dt.Columns.Add("factory_id", typeof(string));
            _dt.Columns.Add("status", typeof(string));
            _dt.Columns.Add("sub_type", typeof(string));
            _dt.Columns.Add("id", typeof(int));
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
                                 reader.IsDBNull(9) ? 0 : reader.GetInt32(9));
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
