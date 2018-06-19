using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Phicomm_WMS.DB
{
    public class BarcodeChecker : BaseDbQuery
    {
        private int _result;

        public BarcodeChecker(string barcode)
            : base("Select " + "F_BarcodeChecker('" + barcode + "')", DbName)
        {
            _result = 0;
        }

        protected override void ProcessResultSet(MySqlDataReader reader)
        {
            try
            {
                if (reader.Read())
                {
                    
                    _result = reader.IsDBNull(0) ? 0:reader.GetInt32(0);
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

        protected override List<string> ProcessSql(string sql)
        {
            List<string> sqlList = new List<string> { sql };
            return sqlList;
        }

        public int GetResult()
        {
            return _result;
        }
    }
}
