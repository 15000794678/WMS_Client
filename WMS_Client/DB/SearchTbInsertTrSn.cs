using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class SearchTbInsertTrSn : BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchTbInsertTrSn(string stockNo)
            : base(
                "Select Tr_Sn, StockNo_Type, Stock_No, ID from tb_insert_trsn where Stock_No='" + stockNo + "' AND UPLOAD='N'",
                DbName)
        {
            _dt.Columns.Add("Tr_Sn", typeof(string));
            _dt.Columns.Add("StockNo_Type", typeof(int));
            _dt.Columns.Add("Stock_No", typeof(string));
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
                                 reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                 reader.IsDBNull(2) ? "" : reader.GetString(2),
                                 reader.IsDBNull(3) ? 0 : reader.GetInt32(3));
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
