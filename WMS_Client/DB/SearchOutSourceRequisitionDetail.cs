using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class SearchOutSourceRequisitionDetail : BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchOutSourceRequisitionDetail(string woid)
            : base(
                "Select ID, LIFNR, woid, material_no, kpesc, PLANT, STORE_LOC, qty, send_qty, sub_type, STATUS From r_outsource_requisition_detail Where woid='" + woid + "'",
                DbName)
        {
            _dt.Columns.Add("ID", typeof(int));
            _dt.Columns.Add("LIFNR", typeof(string)); //供应商代码
            _dt.Columns.Add("woid", typeof(string));
            _dt.Columns.Add("material_no", typeof(string));
            _dt.Columns.Add("kpesc", typeof(string));
            _dt.Columns.Add("PLANT", typeof(string));
            _dt.Columns.Add("STORE_LOC", typeof(string));
            _dt.Columns.Add("qty", typeof(int));
            _dt.Columns.Add("send_qty", typeof(int));
            _dt.Columns.Add("sub_type", typeof(string));
            _dt.Columns.Add("STATUS", typeof(string));
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
                                 reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                 reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
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
