using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Net;
using System.Net.Sockets;
using Phicomm_WMS.DB;
using Phicomm_WMS.OUTIO;

namespace WMS_Client.UI
{
    public partial class MFrm : Office2007Form
    {
        private string _ip = string.Empty;
        private DataTable _dt = null;

        public MFrm()
        {
            InitializeComponent();
        }

        private void MFrm_Load(object sender, EventArgs e)
        {
            using (LogInFrm frm = new LogInFrm())
            {
                if (frm.ShowDialog() == DialogResult.Cancel)
                {
                    this.Close();
                    return;
                }
            }

            InitSelect();
        }

        private string GetIpAddress()
        {
            List<string> listIp = new List<string>();
                
            IPAddress[] ipAddress = Dns.GetHostAddresses(Dns.GetHostName());
            List<IPAddress> ipAddressList = ipAddress.Where(ipa => ipa.AddressFamily == AddressFamily.InterNetwork).ToList();
            foreach (IPAddress ipaddress in ipAddressList)
            {
                return ipaddress.ToString();
            }

            return string.Empty;
        }

        private DataTable GetFunctionIP()
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();                

                SearchDevice sd = new SearchDevice(dic);
                sd.ExecuteQuery();

                return sd.GetResult();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void InitSelect()
        {
            _ip = GetIpAddress();
            if (string.IsNullOrEmpty(_ip))
            {
                return;
            }
            label_IP.Text = "当前IP：" + _ip;

            _dt = GetFunctionIP();
            if (_dt == null || _dt.Rows.Count==0)
            {
                return;
            }

            TreeNode node1 = new TreeNode();
            node1.Text = "功能列表";
            TreeNode defaultNode = null;
            foreach (DataRow dr in _dt.Rows)
            {
                if (int.Parse(dr["DeviceID"].ToString()) > 10000)
                {
                    if (dr["DeviceNickName"].ToString().Contains("充电"))
                    {
                        continue;
                    }

                    TreeNode node21 = new TreeNode();
                    node21.Text = dr["DeviceIP"] + " - " + dr["DeviceNickName"] + " - " + dr["StationID"].ToString();                  

                    if (dr["DeviceNickName"].ToString().Contains("出/入"))
                    {
                        TreeNode node31 = new TreeNode();
                        node31.Text = "入库";
                        TreeNode node32 = new TreeNode();
                        node32.Text = "出库";
                        node21.Nodes.Add(node31);
                        node21.Nodes.Add(node32);
                    }

                    node1.Nodes.Add(node21);

                    if (dr["DeviceIP"].Equals(_ip))
                    {
                        defaultNode = node21;
                    }
                }
            }

            TreeNode node22 = new TreeNode();
            node22.Text = "127.0.0.1 - 条码补印 - 0";
            node1.Nodes.Add(node22);

            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(node1);
            treeView1.ExpandAll();
            if (defaultNode != null)
            {
                treeView1.SelectedNode = defaultNode;
                treeView1.Focus();
            }
        }

        private void button_select_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeView1.SelectedNode == null)
                {
                    MessageBox.Show("请选择一项功能！");
                    return;
                }

                string str = treeView1.SelectedNode.Text;
                string str_child = string.Empty;
                string stationId = string.Empty;

                if (!str.Contains("-"))
                {
                    if (treeView1.SelectedNode.Parent!=null)
                    {                        
                        str = treeView1.SelectedNode.Parent.Text;
                        str_child = treeView1.SelectedNode.Text;
                    }
                    else
                    {
                        MessageBox.Show("请选择正确的功能!");
                        return;
                    }
                }
                else if (str.Contains("出/入"))
                {
                    MessageBox.Show("请选择该站点的子功能!");
                    return;
                }

                //获取站点编号
                stationId = str.Substring(str.IndexOf("-") + 1);
                stationId = stationId.Substring(stationId.IndexOf("-") + 1);
                MyData.SetStationId(int.Parse(stationId));
                MyData.SetStockNo("");
                MyData.SetStockNoType(0);
                MyData.SetStockSubType("");
                MyData.Clear();                   

                //选择功能
                if (str.Contains(_ip) || str.Contains("127.0.0.1") || str.Contains("打印点"))
                {
                    SelectUI(str, str_child);
                }
                else
                {//选择其他站点IP功能需要输入密码
                    using (PasswordFrm ifrm = new PasswordFrm())
                    {
                        if (ifrm.ShowDialog() == DialogResult.Cancel)
                        {
                            ifrm.Dispose();
                            return;
                        }
                        else
                        {
                            ifrm.Dispose();
                        }

                        SelectUI(str, str_child);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            InitSelect();    
               
            //测试分盘打印标签
            //using (SplitMaterial sm = new SplitMaterial())
            //{
            //    sm.SetLabel("100028180516000241", 100);
            //    sm.ShowDialog();
            //    sm.Dispose();
            //}
        }

        private void SelectUI(string str, string str_child)
        {
            Hide();
            if (str.Contains("分拣台"))
            {
                using (SelectionFrm sf = new SelectionFrm())
                {
                    sf.ShowDialog();
                    sf.Dispose();
                }
            }
            else if (str.Contains("打印点"))
            {
                using (Frm_PrintTrSn fp = new Frm_PrintTrSn())
                {
                    fp.ShowDialog();
                    fp.Dispose();
                }
            }
            else if (str.Contains("条码补印"))
            {
                using (Frm_Material_RePrint fr = new Frm_Material_RePrint())
                {
                    fr.ShowDialog();
                    fr.Dispose();
                }
            }
            else if (str.Contains("充电桩"))
            {
                //MessageBox.Show("充电桩");
            }
            else if (str.Contains("出/入"))
            {
                if (str_child.Contains("入"))
                {
                    using (ReplenishFrm rf = new ReplenishFrm())
                    {
                        rf.ShowDialog();
                        rf.Dispose();
                    }
                }
                else if (str_child.Contains("出"))
                {
                    using (PickFrm pf = new PickFrm())
                    {
                        pf.ShowDialog();
                        pf.Dispose();
                    }
                }                
            }
            Show();
        }
        
        private void MFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode!=Keys.Enter)
            {
                return;
            }

            button_select_Click(sender, e);
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            button_select_Click(sender, e);
        }
    }
}
