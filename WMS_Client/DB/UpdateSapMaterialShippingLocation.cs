using System.Collections.Generic;
using System.Linq;

namespace Phicomm_WMS.DB
{
    /// <summary>
    /// 根据操作员要求将对应入库单指定分拣台的待拣选商品状态置为正在执行或将正在执行商品商品状态置为待拣选。
    /// </summary>
    public class UpateSapMaterialShippingLocation : BaseDbUpdater
    {
        List<string> _sqlList = null;

        public UpateSapMaterialShippingLocation(string stockno, string moveplant, string moveloc) :
            base("Update r_sap_material_shipping Set MOVE_PLANT='" + moveplant + "', MOVE_STLOC='" + moveloc + "' Where SHIPPING_NO='" + stockno + "' And DEB_CRED='OUT'", DbName)
        {

        }


        protected override List<string> ProcessSql(string sql)
        {
            _sqlList = new List<string>() { sql };

            return _sqlList;
        }
    }
}