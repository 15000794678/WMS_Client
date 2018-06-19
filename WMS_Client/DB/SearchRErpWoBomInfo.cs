using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Phicomm_WMS.DB
{
    public class SearchErpWoBomInfo : BaseDbQuery
    {
        private DataTable _dt = new DataTable();

        public SearchErpWoBomInfo(Dictionary<string, object> dic)
            : base(
                "Select woid, material_no,qty,send_qty,factory,loc,move_type,kpdesc,sub_type,rel_requireid, rel_projectid, createtime from r_erp_wo_bom_info",
                DbName)
        {
            _dt.Columns.Add("WOID", typeof(string));
            _dt.Columns.Add("KP_NO", typeof(string));
            _dt.Columns.Add("QTY", typeof(int));
            _dt.Columns.Add("Send_QTy", typeof(int));
            _dt.Columns.Add("FromFactory", typeof(string));
            _dt.Columns.Add("FromLoc", typeof(string));
            _dt.Columns.Add("MoveType", typeof(string));
            _dt.Columns.Add("ToFactory", typeof(string));//NA
            _dt.Columns.Add("ToLoc", typeof(string));//NA            
            _dt.Columns.Add("KPDESC", typeof(string));
            _dt.Columns.Add("SubType", typeof(string)); 
            _dt.Columns.Add("StockNoType", typeof(int)); //默认为1
            _dt.Columns.Add("Rel_Requireid", typeof(string));
            _dt.Columns.Add("Rel_ProjectId", typeof(string));
            _dt.Columns.Add("CreateTime", typeof(string));

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
                    DataRow dr = _dt.NewRow();
                    dr["WOID"] = reader.IsDBNull(0)? "" : reader.GetString(0);
                    dr["KP_NO"] = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    dr["QTY"] = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                    dr["Send_QTY"] = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                    dr["FromFactory"] = reader.IsDBNull(4) ? "" : reader.GetString(4);
                    dr["FromLoc"] = reader.IsDBNull(5) ? "" : reader.GetString(5);
                    dr["MoveType"] = reader.IsDBNull(6) ? "" : reader.GetString(6);
                    dr["ToFactory"] = "NA";
                    dr["ToLoc"] = "NA";
                    dr["KPDESC"] = reader.IsDBNull(7) ? "" : reader.GetString(7);
                    dr["SubType"] = reader.IsDBNull(8) ? "" : reader.GetString(8);
                    dr["StockNoType"] = 1;
                    dr["Rel_Requireid"] = reader.IsDBNull(9) ? "" : reader.GetString(9);
                    dr["Rel_ProjectId"] = reader.IsDBNull(10) ? "" : reader.GetString(10);
                    dr["CreateTime"] = reader.IsDBNull(11) ? "" : reader.GetString(11);

                    _dt.Rows.Add(dr);
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
