using System.Collections.Generic;
using System.Linq;

namespace Phicomm_WMS.DB
{
    /// <summary>
    /// 根据操作员要求将对应入库单指定分拣台的待拣选商品状态置为正在执行或将正在执行商品商品状态置为待拣选。
    /// </summary>
    public class UpateTransferRequsitionDetail : BaseDbUpdater
    {
        private List<string> _sqlList;

        public UpateTransferRequsitionDetail(Dictionary<string, object> dic1, Dictionary<string, object> dic2) :
            base("Update r_transfer_requisition_detail SET ", DbName)
        {
            foreach(KeyValuePair<string, object> obj in dic1)
            {
                Sql += " " + obj.Key + "='" + obj.Value + "',";
            }
            Sql = Sql.Substring(0, Sql.Length - 1);

            Sql += " Where ";
            foreach (KeyValuePair<string, object> obj in dic2)
            {
                Sql += " " + obj.Key + "='" + obj.Value + "' AND ";
            }
            Sql = Sql.Substring(0, Sql.Length - 5);
        }


        protected override List<string> ProcessSql(string sql)
        {
            _sqlList = new List<string> { sql };
            return _sqlList;
        }
    }
}