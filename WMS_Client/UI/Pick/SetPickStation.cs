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
    public partial class SetPickStation : Office2007Form
    {
        private Dictionary<string, string> _dic = new Dictionary<string, string>();

        public SetPickStation()
        {
            InitializeComponent();
        }

        private void SetPickStation_Load(object sender, EventArgs e)
        {            
            DataTable dt = GetFunctionIP();
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("从远程获取站点信息失败");
                this.Close();
                return;
            }

            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.RowCount = dt.Rows.Count;
            foreach(DataRow dr in dt.Rows)
            {
                if (dr["DeviceNickName"].ToString().Contains("出"))
                {
                    CheckBox cb = new CheckBox();
                    cb.Anchor = AnchorStyles.Right;
                    cb.Font = new Font("宋体", 14);
                    cb.Text = "站点" + dr["StationID"].ToString();                    
                    ComboBox cbb = new ComboBox();
                    cbb.Anchor = AnchorStyles.Left;
                    cbb.Font = new Font("宋体", 14);
                    cbb.DropDownStyle = ComboBoxStyle.DropDownList;
                    cbb.Items.Add("出A材");
                    cbb.Items.Add("不出A材");
                    if (dr["StationID"].ToString().Equals(MyData.GetStationId().ToString()))
                    {
                        cb.Checked = true;
                        //cb.Enabled = false;
                        cbb.SelectedIndex = 0;
                    }
                    else
                    {
                        cb.Checked = false;
                        cbb.SelectedIndex = 1;
                    }
                    tableLayoutPanel2.Controls.Add(cb);
                    tableLayoutPanel2.Controls.Add(cbb);
                }
            }
            button1.Focus();
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private int GetTaskCnt(string stationId)
        {
            try
            {

                SearchTaskCount st = new SearchTaskCount(2, int.Parse(stationId));
                st.ExecuteQuery();

                return st.GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetTaskCnt:" + ex.Message);
                return -1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {          
            _dic.Clear();

            string station = "";
            string type = "";
            foreach (object obj in tableLayoutPanel2.Controls)
            {
                //判断控件类型
                if ((obj as CheckBox)!=null)  //选中
                {
                    CheckBox cb = (CheckBox)obj;
                    if (cb.Checked)
                    {
                        station = cb.Text.Replace("站点", "").Trim();
                    }
                    else
                    {
                        station = "";
                    }
                }
                else if ((obj as ComboBox)!=null) //出A材
                {
                    ComboBox cbb = (ComboBox)obj;
                    if (!string.IsNullOrEmpty(station))  //站点选中
                    {
                        if (cbb.SelectedIndex==0)
                        {
                            type = "A";
                        }
                        else
                        {
                            type = "-";
                        }

                        if (_dic.ContainsKey(station))
                        {
                            _dic[station] = type;
                        }
                        else
                        {
                            _dic.Add(station, type);
                        }

                        station = "";
                        type = "";
                    }
                }
            }
            if (_dic.Count==0)
            {
                MessageBox.Show("请至少选择一个出库站点!");
                return;
            }

            bool result = false;
            foreach(KeyValuePair<string, string> kv in _dic)
            {
                int cnt = GetTaskCnt(kv.Key);  //查看其他站点是否有任务正在出库
                if (cnt==-1)
                {
                    return; //异常
                }
                if (cnt>0)
                {
                    MessageBox.Show("站点" + kv.Key + "有任务正在出库！");
                    return;
                }
                if (kv.Value.Equals("A"))
                {
                    result = true;
                    break;
                }
            }
            if (!result)
            {
                MessageBox.Show("请至少选择一个站点出A材！");
                return;
            }

            //设置其他站点状态
            foreach (KeyValuePair<string, string> kv in _dic)
            {
                try
                {
                    DBPCaller.DeinitProcess(int.Parse(kv.Key));
                    if (kv.Value.Contains("A"))
                    {
                        DBPCaller.InitPartialPickStation(MyData.GetUser(), int.Parse(kv.Key));
                    }
                    else
                    {
                        DBPCaller.InitPickProcess(MyData.GetUser(), int.Parse(kv.Key));
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }

            //返回
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
