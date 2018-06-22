using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class CreateCheckTask : BaseDbStoredProcedureCaller
    {
        private string _result = "Error";

        public CreateCheckTask(string checkNo, int type)
            : base(
                  DbName, "P_CreateCheckTask",
                  new MySqlParameter("@pCheckNo", MySqlDbType.String) { Value = checkNo },
                  new MySqlParameter("@pType", MySqlDbType.Int32) { Value = type })
        {

        }

        protected override void ProcessResult(MySqlDataReader reader)
        {
            try
            {
                if (reader.Read())
                {
                    _result = reader.GetString("Result");
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

        public string GetResult()
        {
            return _result;
        }
    }
}