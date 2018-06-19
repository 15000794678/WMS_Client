using System.Collections.Generic;
using System.Linq;

namespace Phicomm_WMS.DB
{
    /// <summary>
    /// 根据操作员要求将对应入库单指定分拣台的待拣选商品状态置为正在执行或将正在执行商品商品状态置为待拣选。
    /// </summary>
    public class UpateReserveRequisitionDetail : BaseDbUpdater
    {
        private List<string> _sqlList;

        public UpateReserveRequisitionDetail(int ID) :
            base("Update r_reserve_requisition_detail SET STATUS='1' where ID='" + ID.ToString() + "'", DbName)
        {

        }

        protected override List<string> ProcessSql(string sql)
        {
            _sqlList = new List<string> { sql };
            return _sqlList;
        }
    }
}