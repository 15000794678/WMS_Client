using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class SearchSapMaterialShipping : BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchSapMaterialShipping(Dictionary<string, object> dic)
            : base(
                "Select SHIPPING_NO, material_no, PLANT, STORE_LOC, MOVE_PLANT, MOVE_STLOC,SHIP_QTY,ID, MOVE_TYPE, UPLOAD_FLAG, TRANSACTION_NO, REMARK, SAP_QTY from r_sap_material_shipping", DbName)
        {
            _dt.Columns.Add("SHIPPING_NO", typeof(string)); //0
            _dt.Columns.Add("material_no", typeof(string)); //1
            _dt.Columns.Add("SAP_QTY", typeof(int));//12
            _dt.Columns.Add("SHIP_QTY", typeof(int)); //6
            _dt.Columns.Add("PLANT", typeof(string)); //2
            _dt.Columns.Add("STORE_LOC", typeof(string));//3
            _dt.Columns.Add("MOVE_TYPE", typeof(string));//8
            _dt.Columns.Add("MOVE_PLANT", typeof(string));//4
            _dt.Columns.Add("MOVE_STLOC", typeof(string)); //5      
            _dt.Columns.Add("UPLOAD_FLAG", typeof(string));//9
            _dt.Columns.Add("TRANSACTION_NO", typeof(string));//10
            _dt.Columns.Add("REMARK", typeof(string));//11
            _dt.Columns.Add("ID", typeof(int));//7


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
            List<string> sqlList = new List<string> { sql };
            return sqlList;
        }

        protected override void ProcessResultSet(MySqlDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    _dt.Rows.Add(
                            reader.IsDBNull(0) ? "" : reader.GetString(0),
                            reader.IsDBNull(1) ? "" : reader.GetString(1),
                            reader.IsDBNull(12) ? 0 : reader.GetInt32(12),
                            reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                            reader.IsDBNull(2) ? "" : reader.GetString(2),
                            reader.IsDBNull(3) ? "" : reader.GetString(3),
                             reader.IsDBNull(8) ? "" : reader.GetString(8),
                            reader.IsDBNull(4) ? "" : reader.GetString(4),
                            reader.IsDBNull(5) ? "" : reader.GetString(5),                                         
                            reader.IsDBNull(9) ? "" : reader.GetString(9),
                            reader.IsDBNull(10) ? "" : reader.GetString(10),
                            reader.IsDBNull(11) ? "" : reader.GetString(11),
                            reader.IsDBNull(7) ? 0 : reader.GetInt32(7)
                        );
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
