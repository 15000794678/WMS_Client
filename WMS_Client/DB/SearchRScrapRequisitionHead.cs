using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Phicomm_WMS.DB
{
    class SearchRScrapRequisitionHead : BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchRScrapRequisitionHead(string woid)
            : base(
                "Select cnd from r_scrap_requisition_head where woid='" + woid + "'",
                DbName)
        {
            _dt.Columns.Add("cnd", typeof(string));            
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
                    _dt.Rows.Add(reader.IsDBNull(0) ? "" : reader.GetString(0)
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
