using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class PickAtStationLoc : BaseDbStoredProcedureCaller
    {
        private DataTable _dt = new DataTable();

        public PickAtStationLoc(int pickStationId) : base(DbName, "P_PickStationBoxLocation",
            new MySqlParameter("@pStationID", MySqlDbType.Int32) { Value = pickStationId })
        {
            _dt.Columns.Add("StockNo", typeof(string));
            _dt.Columns.Add("ShowText", typeof(string));
            _dt.Columns.Add("PodId", typeof(int));
            _dt.Columns.Add("PodSide", typeof(int));
            _dt.Columns.Add("ShelfId", typeof(int));
            _dt.Columns.Add("BoxBarcode", typeof(string));
            _dt.Columns.Add("BoxId", typeof(int));
            _dt.Columns.Add("MaterialId", typeof(string));
            _dt.Columns.Add("MaterialName", typeof(string));
            _dt.Columns.Add("Qty", typeof(int));
            _dt.Columns.Add("AllOut", typeof(int));
        }

        protected override void ProcessResult(MySqlDataReader reader)
        {
            try
            {                
                while (reader.Read())
                {
                    DataRow dr = _dt.NewRow();

                    dr["StockNo"] = reader.GetString(0);
                    dr["ShowText"] = reader.GetString(2);
                    dr["PodId"] = reader.GetInt32(3);
                    dr["PodSide"] = reader.GetInt32(4);
                    dr["ShelfId"] = reader.GetInt32(5);
                    dr["BoxBarcode"] = reader.GetString(6);
                    dr["BoxId"] = reader.GetInt32(7);
                    dr["MaterialId"] = reader.GetString(8);
                    dr["MaterialName"] = reader.GetString(9);
                    dr["Qty"] = reader.GetInt32(10).ToString().Trim();
                    dr["AllOut"] = reader.GetInt32(11).ToString().Trim();

                    _dt.Rows.Add(dr);
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
