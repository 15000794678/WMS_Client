using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Phicomm_WMS.DB;
using Phicomm_WMS.OUTIO;

namespace Phicomm_WMS.UI
{
    public partial class HolderFrm : Office2007Form
    {
        public HolderFrm()
        {
            InitializeComponent();
        }

        public void SetValue(string id, string current, string total)
        {
            label_Id.Text = id;
            label_cnt.Text = current.ToString() + " / " + total.ToString();                        
        }

        public void SetId(string id)
        {
            label_Id.Text = id;
        }

        public void SetCnt(string current, string total)
        {
            label_cnt.Text = current + " / " + total;
        }

        public void Select(Color cl)
        {
            this.BackColor = cl;
        }

        public void UnSelect()
        {
            this.BackColor = Color.FromArgb(194, 217, 247);
        }

        public void ShowHint(string msg, Color cl)
        {
            using (Phicomm_WMS.UI.MsgFrm mf = new Phicomm_WMS.UI.MsgFrm(msg, cl))
            {
                mf.BringToFront();
                mf.ShowDialog();
                mf.Dispose();
            }
        }

        private void HolderFrm_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                //MessageBox.Show("ID:" + label_Id.Text);
                if (string.IsNullOrEmpty(MyData.GetStockNo().Trim()))
                {
                    MessageBox.Show("请先填入入库单等信息");
                    return;
                }

                //根据周转箱查询储位信息
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("holder_id", label_Id.Text);
                dic.Add("stock_no", MyData.GetStockNo());
                SearchRHolderLocidRelationShip sl = new SearchRHolderLocidRelationShip(dic);
                sl.ExecuteQuery();
                DataTable dt = sl.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("在Holder表中查不到该周转箱信息", Color.Red);
                    return;
                }
                if (string.IsNullOrEmpty(dt.Rows[0]["loc_id"].ToString().Trim()))
                {
                    ShowHint("在Holder表中查不到该周转箱的储位信息", Color.Red);
                    return;
                }

                //根据储位查询物料条码信息
                dic.Clear();
                dic.Add("status", 0);
                dic.Add("loc_id", dt.Rows[0]["loc_id"].ToString().Trim());
                SearchRInventoryDetail sr = new SearchRInventoryDetail(dic);
                sr.ExecuteQuery();
                dt = sr.GetResult();
                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowHint("在R_Inventory_Detail表中查不到该储位信息", Color.Red);
                    return;
                }

                //显示
                string str = "";
                int cnt = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    str += dr["tr_sn"].ToString() + ",   ";
                    cnt++;
                }

                ShowHint("周转箱: " + label_Id.Text + "  中已装入 " + cnt.ToString() + " 盘物料，条码信息如下: \r\n" + str, Color.Red);
            }
            catch(Exception ex)
            {
                ShowHint(ex.Message, Color.Red);
            }
        }
    }
}
