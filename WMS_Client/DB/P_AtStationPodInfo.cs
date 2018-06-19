using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    /// <summary>
    /// 获取当前拣选站执行入库任务的货架信息
    /// </summary>
    public class AtStationPod : BaseDbStoredProcedureCaller
    {
        private DataTable _dt = new DataTable();

        public AtStationPod(int pickStationId)
            : base(DbName, "P_AtStationPodInfo",
                new MySqlParameter("@pStationID", MySqlDbType.Int32) { Value = pickStationId })
        {
            
            _dt.Columns.Add("PodId", typeof(int));
            _dt.Columns.Add("PodName", typeof(string));
            //_dt.Columns.Add("Linked", typeof(int));            
            _dt.Columns.Add("Row", typeof(int));
            _dt.Columns.Add("Column", typeof(int));
        }

        protected override void ProcessResult(MySqlDataReader reader)
        {
            try
            {
                if (reader.Read())
                {
                    DataRow dr = _dt.NewRow();

                    dr["PodId"] = reader.IsDBNull(1)? 0:reader.GetInt32(1);
                    dr["PodName"] = reader.IsDBNull(2)? "0":reader.GetString(2);
                    dr["Row"] = reader.IsDBNull(3)? 0:reader.GetInt32(3);
                    dr["Column"] = reader.IsDBNull(4)? 0:reader.GetInt32(4);

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
