using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    /// <summary>
    /// 根据ip获取当前设备的名称和类型
    /// </summary>
    public class SearchDevice:BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchDevice(Dictionary<string, object> dic)
            : base(
                "Select DeviceID, DeviceIP, DeviceTypeID, DeviceSerial, DeviceNickName, StationID, IsActive, HolderSequence " +
                "From device",
                DbName)
        {
            _dt.Columns.Add("DeviceIP", typeof(string));
            _dt.Columns.Add("DeviceNickName", typeof(string));
            _dt.Columns.Add("StationID", typeof(int));
            _dt.Columns.Add("DeviceTypeID", typeof(int));
            _dt.Columns.Add("DeviceID", typeof(string));
            _dt.Columns.Add("DeviceSerial", typeof(string));

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
            List<string> sqlList = new List<string> {sql};

            return sqlList;
        }

        protected override void ProcessResultSet(MySqlDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = _dt.NewRow();
                    dr["DeviceIP"] = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    dr["DeviceNickName"] = reader.IsDBNull(4) ? "" : reader.GetString(4);
                    dr["StationID"] = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                    dr["DeviceTypeID"] = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                    dr["DeviceID"] = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    dr["DeviceSerial"] = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    _dt.Rows.Add(dr);
                }
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
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
