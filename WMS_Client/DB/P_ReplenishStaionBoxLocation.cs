using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class ReplenishAtStationLoc : BaseDbStoredProcedureCaller
    {
        private DataTable _dt = new DataTable();

        public ReplenishAtStationLoc(int pickStationId) : base(DbName, "P_ReplenishStationBoxLocation",
            new MySqlParameter("@pStationID", MySqlDbType.Int32) { Value = pickStationId })
        {
            _dt.Columns.Add("StockNo", typeof(string));
            _dt.Columns.Add("PodId", typeof(int));
            _dt.Columns.Add("ShelfId", typeof(int));
            _dt.Columns.Add("BoxId", typeof(int));
            _dt.Columns.Add("MaterialId", typeof(string));
            _dt.Columns.Add("MaterialName", typeof(string));
            _dt.Columns.Add("BoxBarcode", typeof(string));
            _dt.Columns.Add("HoderId", typeof(int));
            _dt.Columns.Add("PodSide", typeof(int));

        }

        protected override void ProcessResult(MySqlDataReader reader)
        {
            try
            {
                while (reader.Read())
                {                    
                    _dt.Rows.Add(reader.IsDBNull(0)? "":reader.GetString(0),
                                 reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                 reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                 reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                 reader.IsDBNull(6) ? "" : reader.GetString(6),
                                 reader.IsDBNull(7) ? "" : reader.GetString(7),
                                 reader.IsDBNull(8) ? "" : reader.GetString(8),
                                 reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                                 reader.IsDBNull(10) ? 0 : reader.GetInt32(10));
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

        protected override void ProcessParms()
        {
            try
            {
                
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetResult()
        {
            return _dt;
        }
    }
}
