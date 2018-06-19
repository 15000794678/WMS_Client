using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System;

namespace Phicomm_WMS.DB
{
    /// <summary>
    /// 根据入库单筛选出对应的货品清单和信息
    /// </summary>
    public class SearchRInventoryId : BaseDbQuery
    {
        private DataTable _dt = null;

        public SearchRInventoryId(Dictionary<string, object> dic) :
            base("select material_no, qty, income_qty, material_desc, vender_name, stockno_type, sub_type, status, vender_no, inventory_id from r_inventory_id ", DbName)
        {
            _dt = new DataTable();
            _dt.Columns.Add("material_no", typeof(string));
            _dt.Columns.Add("qty", typeof(int));
            _dt.Columns.Add("income_qty", typeof(int));            
            _dt.Columns.Add("material_desc", typeof(string));
            _dt.Columns.Add("vender_name", typeof(string));
            _dt.Columns.Add("stockno_type", typeof(int));
            _dt.Columns.Add("sub_type", typeof(string));
            _dt.Columns.Add("status", typeof(string));
            _dt.Columns.Add("vender_no", typeof(string));
            _dt.Columns.Add("inventory_id", typeof(string));

            if (dic!=null && dic.Count>0)
            {
                Sql += " Where ";
                foreach(KeyValuePair<string, object> obj in dic)
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
                    DataRow dr = _dt.NewRow();
                    dr["material_no"] = reader.IsDBNull(0) ? "" : reader.GetString(0);
                    dr["qty"] = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                    dr["income_qty"] = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                    dr["material_desc"] = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    dr["vender_name"] = reader.IsDBNull(4) ? "" : reader.GetString(4);
                    dr["stockno_type"] = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                    dr["sub_type"] = reader.IsDBNull(6) ? "" : reader.GetString(6);
                    dr["status"] = reader.IsDBNull(7) ? "" : reader.GetString(7);
                    dr["vender_no"] = reader.IsDBNull(8) ? "" : reader.GetString(8);
                    dr["inventory_id"] = reader.IsDBNull(9) ? "" : reader.GetString(9);

                    _dt.Rows.Add(dr);
                }
            }
            catch(Exception ex)
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
