using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Phicomm_WMS.DB
{
    /// <summary>
    /// 根据操作员要求将对应入库单指定分拣台的待拣选商品状态置为正在执行或将正在执行商品商品状态置为待拣选。
    /// </summary>
    public class UpateSapMaterialShipping : BaseDbUpdater
    {

        private DataTable _dt = new DataTable();
        private string _remark;
        private string _transactionno;
        private string _status;
        private string _upload;

        public UpateSapMaterialShipping(DataTable dt, string remark, string transactionno, string status, string upload) :
            base("Update r_sap_material_shipping ", DbName)
        {
            _dt = dt;
            _remark = remark;
            _transactionno = transactionno;
            _status = status;
            _upload = upload;
        }


        protected override List<string> ProcessSql(string sql)
        {
            List<string> sqlList = new List<string>();

            foreach (DataRow dr in _dt.Rows)
            {
                string ss = "update r_sap_material_shipping Set STATUS = '" + _status +
                            "', UPLOAD_FLAG = '" + _upload +
                            "', TRANSACTION_NO='" + _transactionno +
                            "', remark = '" + _remark +
                            "',UPLOAD_DATE='" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                            "' Where ID = " + dr["ID"].ToString();
                sqlList.Add(ss);
            }

            return sqlList;
        }
    }
}