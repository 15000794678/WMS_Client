using System;
using System.Collections.Generic;
using System.Text;
//using Entity;
using System.Data;
//using RefWebService_BLL;
using System.Windows.Forms;
using System.Net;
using System.Management;
using System.IO;

namespace FrmBLL
{
    public class publicfuntion
    {
        publicfuntion()
        {
        }

        public static DataTable DataTableToSort(DataTable dt, string Colnums)
        {
            DataView dv = dt.DefaultView;
            dv.Sort = string.Format("{0} ASC", Colnums);
            return dv.ToTable();
        }

        public static DataTable DataTableDistinct(DataTable dt, List<string> LsColnum)
        {
            DataView dataView = dt.DefaultView;
            DataTable dataTableDistinct = dataView.ToTable(true, LsColnum.ToArray());//注：其中ToTable（）
            return dataTableDistinct;
        }

   

        /// <summary>
        /// 返回一个新的datatable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable getNewTable(DataTable dt, string sql)
        {
            try
            {
                DataTable mydt = new DataTable();
                mydt = dt.Clone();
                DataRow[] arrDr = dt.Select(sql);
                for (int i = 0; i < arrDr.Length; i++)
                {
                    mydt.ImportRow((DataRow)arrDr[i]);
                }
                return mydt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 将datagridview内容转换为datatable
        /// </summary>
        /// <param name="dgv"></param>
        /// <returns></returns>
        public static DataTable GetDgvToTable(DataGridView dgv)
        {
            DataTable dt = new DataTable();
            for (int count = 0; count < dgv.Columns.Count; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                dt.Columns.Add(dc);
            }
            for (int count = 0; count < dgv.Rows.Count; count++)
            {
                DataRow dr = dt.NewRow();
                for (int countsub = 0; countsub < dgv.Columns.Count; countsub++)
                {
                    dr[countsub] = Convert.ToString(dgv.Rows[count].Cells[countsub].Value);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static string mGetString(string temp, int i)
        {
            int x = i - 1;
            string s = temp.Trim();
            string sMsg = "";
            string[] sArray = s.Split('|');
            if ((sArray.Length - 1) != 4)
            {
                sMsg = "";
            }
            else
            {
                sMsg = sArray[x];
            }

            return sMsg;
        }



        private static string getstring(string[] arr, int icount)
        {
            string str = "";
            for (int x = 0; x < arr.Length; x++)
            {
                str += arr[x] + ",";
                if (x != 0 && 0 == (x % icount))
                    str += "|";
                if (x + 1 == arr.Length)
                    str += "|";
            }
            return str;
        }


        //public static void AddProgInfo(Dictionary<string, object> proginfo, IList<IDictionary<string, object>> lsfunls)
        //{
        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add("PROGID", proginfo["PROGID"]);
        //    dic.Add("FUNID", "SHOWFROM");
        //    dic.Add("FUNDESC", "显示程序");
        //    dic.Add("FUNNAME", "显示程序");

        //    lsfunls.Add(dic);
        //    for (int i = 0; i < lsfunls.Count; i++)
        //    {
        //        lsfunls[i]["PROGID"] = proginfo["PROGID"];
        //    }
        //    RefWebService_BLL.refWebB_User_Info.Instance.AddProgInfo(FrmBLL.ReleaseData.DictionaryToJson(proginfo));
        //    RefWebService_BLL.refWebB_User_Info.Instance.AddFunctionList(FrmBLL.ReleaseData.ListDictionaryToJson(lsfunls));
        //}

        //public static void AddFunctionList(IList<IDictionary<string, object>> lsfunls)
        //{
        //    RefWebService_BLL.refWebB_User_Info.Instance.AddFunctionList(FrmBLL.ReleaseData.ListDictionaryToJson(lsfunls));
        //}
        public static IList<IDictionary<string, object>> GetFromCtls(System.Windows.Forms.Control ctl, ref  IList<IDictionary<string, object>> lsfunls)
        {
            if (ctl is System.Windows.Forms.Button || ctl is DevComponents.DotNetBar.ButtonX)
            {
                if (!string.IsNullOrEmpty(ctl.Text))
                {
                    //lsfunls.Add(new WebServices.tUserInfo.tFunctionList()
                    //{
                    //    funId = ctl.Name,
                    //    funname = ctl.Text,
                    //    fundesc = ctl.Text,
                    //});
                }
            }
            else
            {
                for (int i = 0; i < ctl.Controls.Count; i++)
                {
                    lsfunls = GetFromCtls(ctl.Controls[i], ref lsfunls);
                }
            }
            return lsfunls;
        }
        /// <summary>
        /// 获取控件名,值
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="Ctrl"></param>
        public static void SerializeControl(IDictionary<string, object> dic, Control Ctrl)
        {
            foreach (Control ctrl in Ctrl.Controls)
            {
                switch (ctrl.GetType().Name)
                {
                    case "TextBox":
                    case "TextBoxX":
                    case "ComboBox":
                    case "ComboBoxEx":
                        {
                            dic.Add(ctrl.Name.Substring(4, ctrl.Name.Length - 4).ToUpper(), ctrl.Text);
                            break;
                        }
                    case "CheckBox":
                        {
                            dic.Add(ctrl.Name.Substring(4, ctrl.Name.Length - 4).ToUpper(), (ctrl as CheckBox).Checked);
                            break;
                        }
                    case "CheckBoxX":
                        {
                            dic.Add(ctrl.Name.Substring(4, ctrl.Name.Length - 4).ToUpper(), (ctrl as DevComponents.DotNetBar.Controls.CheckBoxX).Checked);
                            break;
                        }
                }
                if (ctrl.Controls.Count > 0)
                    SerializeControl(dic, ctrl);
            }
        }


        /// <summary>
        /// 获取选中行
        /// </summary>
        /// <param name="PartNumber"></param>
        public static void SelectDataGridViewRows(string Data, System.Windows.Forms.DataGridView _dgv, int columnName)
        {
            for (int i = 0; i < _dgv.RowCount; i++)
            {
                if (_dgv[columnName, i].Value.ToString() == Data)
                {
                    _dgv.Rows[i].Selected = true;
                    _dgv.FirstDisplayedScrollingRowIndex = i;
                }
                else
                    _dgv.Rows[i].Selected = false;
            }
        }

        //public static string InserSystemLog(string User, string PrgName, string ActionType, string ActionDesc)
        //{

        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add("USERID", User);
        //    dic.Add("PRG_NAME", PrgName);
        //    dic.Add("ACTION_TYPE", ActionType);
        //    dic.Add("ACTION_DESC", ActionDesc);
        //    dic.Add("RECDATE", System.DateTime.Now);
        //    return refWebRecodeSystemLog.Instance.InsertSystemLog(ReleaseData.DictionaryToJson(dic));
        //}



        /// <summary>
        /// 36进制加10进制计算(计算6进制数据的相加结果)
        /// </summary>
        /// <param name="val">原36进制数</param>
        /// <param name="num">加数</param>
        /// <returns>返回相加后的数</returns>
        public static string F36STR(string val, int num)
        {
            List<int> ls = new List<int>();
            int _num = num;
            val = val.ToUpper();
            char[] t_arychr = new char[val.Length];
            t_arychr = val.ToCharArray();

            if (num > 36)
            {
                ls.Add(36);
                while ((_num % 36) > 36)
                {
                    ls.Add(_num = _num % 36);

                }
                ls.Add(_num % 36);
            }
            else
            {
                ls.Add(num);
            }
            foreach (int ix in ls)
            {
                t_arychr[t_arychr.Length - 1] = (char)(byte)(((int)t_arychr[t_arychr.Length - 1]) + ix);
                for (int i = t_arychr.Length - 1; i > 0; i--)
                {
                    if ((int)t_arychr[i] > 90)
                    {
                        t_arychr[i] = (char)(byte)48;
                        t_arychr[i - 1] = (char)(byte)(((int)t_arychr[i - 1] + 1));
                    }
                    if (i == 0)
                    {
                        if ((int)t_arychr[0] > 90)
                            throw new Exception("溢出");
                    }
                }
            }
            return new string(t_arychr);
        }
        private static char[] rDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        /// <summary>
        /// 将指定基数的数字的 System.String 表示形式转换为等效的 64 位有符号整数。
        /// </summary>
        /// <param name="value">包含数字的 System.String。</param>
        /// <param name="fromBase">value 中数字的基数，它必须是[2,36]</param>
        /// <returns>等效于 value 中的数字的 64 位有符号整数。- 或 - 如果 value 为 null，则为零。</returns>
        public static long x2h(string value, int fromBase)
        {
            value = value.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return 0L;
            }

            string sDigits = new string(rDigits, 0, fromBase);
            long result = 0;
            //value = reverse(value).ToUpper(); 1
            value = value.ToUpper();// 2
            for (int i = 0; i < value.Length; i++)
            {
                if (!sDigits.Contains(value[i].ToString()))
                {
                    throw new ArgumentException(string.Format("The argument \"{0}\" is not in {1} system.", value[i], fromBase));
                }
                else
                {
                    try
                    {
                        //result += (long)Math.Pow(fromBase, i) * getcharindex(rDigits, value[i]); 1
                        result += (long)Math.Pow(fromBase, i) * getcharindex(rDigits, value[value.Length - i - 1]);//   2
                    }
                    catch
                    {
                        throw new OverflowException("运算溢出.");
                    }
                }
            }

            return result;
        }

        private static int getcharindex(char[] arr, char value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == value)
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// long转化为toBase进制
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toBase"></param>
        /// <returns></returns>
        public static string h2x(long value, int toBase)
        {
            int digitIndex = 0;
            long longPositive = Math.Abs(value);
            int radix = toBase;
            char[] outDigits = new char[63];

            for (digitIndex = 0; digitIndex <= 64; digitIndex++)
            {
                if (longPositive == 0) { break; }

                outDigits[outDigits.Length - digitIndex - 1] =
                    rDigits[longPositive % radix];
                longPositive /= radix;
            }

            return new string(outDigits, outDigits.Length - digitIndex, digitIndex);
        }

        /// <summary>
        /// 任意进制转换,将fromBase进制表示的value转换为toBase进制
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromBase"></param>
        /// <param name="toBase"></param>
        /// <returns></returns>
        public static string X2X(string value, int fromBase, int toBase)
        {
            if (string.IsNullOrEmpty(value.Trim()))
            {
                return string.Empty;
            }

            if (fromBase < 2 || fromBase > 36)
            {
                throw new ArgumentException(String.Format("The fromBase radix \"{0}\" is not in the range 2..36.", fromBase));
            }

            if (toBase < 2 || toBase > 36)
            {
                throw new ArgumentException(String.Format("The toBase radix \"{0}\" is not in the range 2..36.", toBase));
            }

            long m = x2h(value, fromBase);
            string r = h2x(m, toBase);
            return r;
        }


        /// <summary>
        /// 存入记事本
        /// </summary>
        /// <param name="Path"></param>
        public static void SaveTxtLog(string path, System.Windows.Forms.ListBox Lbx)
        {

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string names = path + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";
            System.IO.StreamWriter writer = new System.IO.StreamWriter(names);
            int i = Lbx.Items.Count;
            for (int j = 0; j < i; j++)
            {
                writer.WriteLine(Lbx.Items[j].ToString());
            }
            writer.Close();
        }

        //public static void WriteLog(string StrLog)
        //{
        //    #region 存储失败日志在服务器
        //    try
        //    {
        //        //如果文件a.txt存在就打开，不存在就新建 .append 是追加写 
        //        string Patch = System.Windows.Forms.Application.StartupPath + "\\log";
        //        if (!File.Exists(Patch + "\\log.txt"))
        //            Directory.CreateDirectory(Patch);
        //        FileStream fst = new FileStream(Patch + "\\log.txt", FileMode.Append);
        //        //写数据到a.txt格式 
        //        StreamWriter swt = new StreamWriter(fst, System.Text.Encoding.GetEncoding("utf-8"));
        //        swt.WriteLine(StrLog + "  :Time" + DateTime.Now.ToString());
        //        swt.Close();
        //        fst.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("写入Log失败:" + ex.Message + ",\r\n请及时联系SFIS人员");
        //    }
        //    #endregion
        //}

        public static DataTable LoadTxt(string[] files, string machine, out string err)
        {
            err = string.Empty;
            DataTable dtTemp = new DataTable();
            dtTemp.Columns.Add("smtsoftname", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("pcbside", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("LineId", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("stationId", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("kpnumber", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("feeder", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("loction", System.Type.GetType("System.String"));

            int yyyy = 0;
            try
            {

                foreach (string item in files)
                {
                    Dictionary<string, kpdelet> entity = new Dictionary<string, kpdelet>();
                    string[] arrTxt = File.ReadAllLines(item);
                    string mcName = string.Empty;
                    //用来缓存上一个Table和下一个Table比较
                    string tableTemp = string.Empty;
                    int tableIndex = 0;
                    string stationid = string.Empty;
                    string kpnumber = string.Empty;
                    string lor = string.Empty;
                    string feeder = string.Empty;
                    string loc = string.Empty;
                    string smtsoftname = string.Empty;
                    string pcbside = string.Empty;

                    string userTotal = string.Empty;

                    string stationTemp = string.Empty;
                    int num = 0;
                    int kpnumberleng = 0;
                    bool flag = false;

                    int ix = 0;
                    yyyy = 0;
                    for (int i = 0; i < arrTxt.Length; i++)
                    {
                        ix = i;
                        if (arrTxt[0].IndexOf("Lot") == -1)
                            throw new Exception("选择的文件格式不正确,不是SMT程式文件,请重新选择");
                        #region xxx读取标题
                        switch (arrTxt[i].Split(':')[0].Trim())
                        {
                            case "MC":
                                //flag = false;
                                //stationTemp = string.Empty;
                                mcName = arrTxt[i].Split(':')[1];
                                break;
                            case "Table":
                                flag = false;
                                if (tableTemp.Trim().ToUpper() != arrTxt[i].Split(':')[1].Replace('=', ' ').Trim().ToUpper())
                                {
                                    tableTemp = arrTxt[i].Split(':')[1].Replace('=', ' ').Trim();
                                    stationTemp = string.Empty;
                                    tableIndex += 1;// arrTxt[i].Split(':')[1].Replace('=', ' ').Trim();
                                }
                                break;
                            case "Lot":
                                //flag = false;
                                //stationTemp = string.Empty;
                                smtsoftname = arrTxt[i].Split(':')[1];
                                string s = arrTxt[i].Split('-')[arrTxt[i].Split('-').Length - 1].Substring(0, 1);
                                pcbside = s != "T" && s != "B" ? "T" : s;
                                break;
                            case "TrayLayout":
                                flag = false;
                                stationTemp = string.Empty;
                                break;
                            default:
                                break;
                        }
                        #endregion
                        if (arrTxt[i].IndexOf("TrayLayout") != -1)
                        {
                            flag = false;
                            stationTemp = string.Empty;
                            continue;
                        }
                        if (!string.IsNullOrEmpty(arrTxt[i].Trim()) &&
                            arrTxt[i].Trim().Length > "Address".Length &&
                            arrTxt[i].Trim().Substring(0, "Address".Length) == "Address" &&
                            arrTxt[i].Trim().IndexOf("Parts") != -1)
                        {
                            flag = true;
                            num = arrTxt[i].IndexOf("Number");
                            kpnumberleng = arrTxt[i].IndexOf("L/R");
                            if (num < 1)
                                throw new Exception("SMT程式格式错误,请修正..");
                            continue;
                        }
                        if (arrTxt[i].IndexOf("CM101") != -1)
                        {

                        }
                        if (flag)
                        {
                            if (string.IsNullOrEmpty(arrTxt[i].Trim()))   //2013-3-18修改
                                continue;
                            if (arrTxt[i].Trim().Length < 8)
                                continue;
                            if (arrTxt[i].Trim().Substring(0, arrTxt[i].Trim().Length).IndexOf("Lot") != -1)
                                continue;
                            if (arrTxt[i].Trim().Substring(0, arrTxt[i].Trim().Length).IndexOf("MC") != -1)
                                continue;
                            if (arrTxt[i].Trim().Substring(0, 4).IndexOf("***") != -1)
                                continue;
                            kpdelet kd = new kpdelet();
                            try
                            {
                                bool br = false;
                                while (arrTxt[i].Substring(0, 7).Trim() == "-")
                                {
                                    loc += string.IsNullOrEmpty(arrTxt[i]) ? string.Empty : arrTxt[i].Substring(num + 3, arrTxt[i].Length - (num + 3)).Trim();
                                    i++;
                                    while (string.IsNullOrEmpty(arrTxt[i]))
                                    {
                                        i++;
                                        if (i == arrTxt.Length)
                                        {
                                            br = true;
                                            break;
                                        }
                                    }
                                    if (br)
                                        break;
                                    entity[stationTemp].loc = loc;
                                }
                            }
                            catch (Exception ex)
                            {
                                yyyy = i;
                                throw new Exception(string.Format("文件第[{0}]行解析错误:\n{1}", yyyy, ex.Message));
                            }
                            #region 解决料号信息被截断
                            if (arrTxt[i].Trim().Length < 8)
                                continue;
                            if (arrTxt[i].Trim().Substring(0, arrTxt[i].Trim().Length).IndexOf("Lot") != -1)
                                continue;
                            if (arrTxt[i].Trim().Substring(0, arrTxt[i].Trim().Length).IndexOf("MC") != -1)
                                continue;
                            if (arrTxt[i].Trim().Substring(0, arrTxt[i].Trim().Length).IndexOf("Feeder Setup") != -1)
                                continue;
                            #endregion
                            if (arrTxt[i].IndexOf("TrayLayout") != -1)
                            {
                                flag = false;
                                stationTemp = string.Empty;
                                continue;
                            }
                            if (string.IsNullOrEmpty(arrTxt[i].Substring(0, 7).Trim()))
                            {

                            }
                            else
                            {
                                stationid = arrTxt[i].Substring(0, 7).Trim();
                            }
                            if (arrTxt[i].Trim().Length < 9)
                                continue;
                            if (arrTxt[i].Substring(7, 11).Trim().IndexOf("*****") != -1)
                            {
                                if (!string.IsNullOrEmpty(arrTxt[i].Substring(0, 7).Trim()))
                                {
                                    stationid = arrTxt[i].Substring(0, 7).Trim();
                                }
                                continue;
                            }


                            if (arrTxt[i].Substring(kpnumberleng - 1, 3).Trim() == "-")  //(arrTxt[i].Substring(22, 10).Trim() == "-")
                            {
                                lor = "A";
                            }
                            else
                            {
                                lor = arrTxt[i].Substring(kpnumberleng - 1, 3).Trim();//arrTxt[i].Substring(22, 10).Trim()
                            }
                            if (kpnumberleng < 15)
                                kpnumber = arrTxt[i].Substring(7, 10).Trim();
                            else
                                kpnumber = arrTxt[i].Substring(7, kpnumberleng - 8).Trim().Split(' ')[0];

                            feeder = arrTxt[i].Substring(kpnumberleng + 3, 20);
                            string feederTemp = string.Empty;
                            if (feeder.IndexOf("Wide") != -1)
                            {
                                feederTemp = feeder.Substring(feeder.IndexOf("Wide") + 4, 2);
                                if (feeder.IndexOf("Forward") != -1)
                                {
                                    feederTemp += "*" + feeder.Substring(feeder.IndexOf("Forward") + 7, 2);
                                }
                            }
                            else
                            {
                                feederTemp = arrTxt[i].Substring(31, 20).Trim();
                            }

                            feeder = feederTemp;

                            string aaa = string.Empty;
                            if (arrTxt[i].Length < (num + 3))
                            {
                                //当碰到用量为0时跳出
                                if (arrTxt[i].Length > num)
                                {
                                    if ((aaa = arrTxt[i].Substring(num - 1, arrTxt[i].Length - (num - 1)).Trim()) == "0")
                                        continue;
                                }
                            }
                            else
                            {
                                //当碰到用量为0时跳出
                                if (arrTxt[i].Substring(num - 1, 3).Trim() == "0")
                                    continue;
                                loc = arrTxt[i].Substring(num + 4, arrTxt[i].Length - (num + 4)).Trim();
                            }
                            stationTemp = string.Format("{0}{1}{2}",
                               tableIndex.ToString("D2"),
                               int.Parse(stationid).ToString("D2"),
                               lor);
                            if (!string.IsNullOrEmpty(loc))
                            {
                                string[] arr = loc.Split(',');
                                Dictionary<string, int> dis = new Dictionary<string, int>();
                                foreach (string str1 in arr)
                                {

                                    try
                                    {
                                        dis.Add(str1, 1);
                                    }
                                    catch
                                    {
                                        err += string.Format("警告!!,器件[{1}]=> 位置 [{0}] 有重复,请检查..\n", str1, kpnumber);
                                    }
                                }
                            }

                            kd.stationId = stationTemp;
                            kd.partsName = kpnumber;
                            kd.lor = lor;
                            kd.freeder = feeder;
                            kd.loc = loc;
                            kd.mcname = mcName;
                            kd.tableIndex = tableIndex.ToString();
                            kd.pcbside = pcbside;
                            kd.smtsoftname = smtsoftname;

                            entity.Add(stationTemp, kd);
                        }
                        yyyy = i;
                    }

                    foreach (kpdelet kpd in entity.Values)
                    {
                        string str = "R";
                        if (IsOdd(int.Parse(kpd.stationId.Substring(0, 2))))
                        {
                            str = "L";
                        }
                        dtTemp.Rows.Add(kpd.smtsoftname, kpd.pcbside, machine + "S" + str, kpd.stationId, kpd.partsName, kpd.freeder, kpd.loc);
                    }
                }

                return dtTemp;
            }
            catch (Exception ex)
            {
                err = ex.Message;// ex.Message + yyyy.ToString();
                return null;
            }
        }

        public static DataTable LoadCSV(string[] files, string machine, out string Err)
        {
            Err = string.Empty;
            DataTable dtTemp = new DataTable();
            dtTemp.Columns.Add("smtsoftname", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("pcbside", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("LineId", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("stationId", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("kpnumber", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("feeder", System.Type.GetType("System.String"));
            dtTemp.Columns.Add("loction", System.Type.GetType("System.String"));


            foreach (string item in files)
            {
                string[] stationTemp;
                string strLine = "";
                string[] strArr;
                string stationid = string.Empty;
                string kpnumber = string.Empty;
                string lor = string.Empty;
                string feeder = string.Empty;
                string loc = string.Empty;
                string smtsoftname = string.Empty;
                string pcbside = string.Empty;

                StreamReader sr = new StreamReader(item);
                while (strLine != null)
                {
                    strLine = sr.ReadLine();
                    if (strLine != null && strLine.Length > 0)
                    {
                        strLine = strLine.Trim();
                        if (strLine.Contains("line"))
                        {
                            smtsoftname = System.IO.Path.GetFileNameWithoutExtension(item);
                            string strname = smtsoftname.Split('(')[0];
                            string s = strname.Split('-')[strname.Split('-').Length - 1];
                            // pcbside = strname.Split('-')[strname.Split('-').Length - 1];
                            pcbside = s != "T" && s != "B" ? "T" : s;

                        }
                        else
                        {
                            strArr = strLine.Split(',');
                            kpnumber = strArr[3];
                            feeder = strArr[5];
                            stationTemp = strArr[2].Split('-');
                            lor = stationTemp[0];
                            stationid = stationTemp[0] + strArr[0].Replace('#', '0') + stationTemp[1];
                            for (int i = 8; i < strArr.Length; i++)
                            {
                                if (i != strArr.Length - 1)
                                    loc += strArr[i] + ',';
                                else
                                    loc += strArr[i];
                            }
                            string str = "L";
                            if (IsRightOrLeft(lor))
                            {
                                str = "R";
                            }
                            dtTemp.Rows.Add(smtsoftname, pcbside, machine + "S" + str, stationid, kpnumber, feeder, loc);
                            loc = "";
                        }
                    }
                }
            }
            return dtTemp;

        }

        public static bool IsRightOrLeft(string str)
        {
            if (str == "H")

                return true;
            else
                return false;

        }
        /// <summary>
        /// 判断句子中是否含有中文
        /// </summary>
        /// <param >字符串</param>
        public static bool WordsIScn(string words)
        {
            string TmmP;
            for (int i = 0; i < words.Length; i++)
            {
                TmmP = words.Substring(i, 1);
                byte[] sarr = System.Text.Encoding.GetEncoding("gb2312").GetBytes(TmmP);
                if (sarr.Length == 2)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是奇数还是偶数
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool IsOdd(int i)
        {

            return Convert.ToBoolean(i % 2);
        }

        #region 获取本机电脑相关信息

        /// <summary> 
        /// 获取主机名 
        /// </summary> 
        /// <returns></returns> 
        public static string HostName
        {
            get
            {
                string hostname = Dns.GetHostName();
                return hostname;
            }
        }
        /// <summary> 
        /// 获取IP地址 
        /// </summary> 
        /// <returns></returns> 
        public static List<string> GetIPList()
        {
            List<string> ipList = new List<string>();
            IPAddress[] addressList = Dns.GetHostEntry(HostName).AddressList;
            for (int i = 0; i < addressList.Length; i++)
            {
                ipList.Add(addressList[i].ToString());
            }
            return ipList;
        }
  

     
        #endregion
        public static string MaterialStatus(string status)
        {
            //状态：0 在库 ;1 备料待接收;2 接收备料;3 待上线;4 待退料;5 移库中;6 售出;7 已使用;8已下线;9盘点完成;10 盘点出库

            string _Status = string.Empty;
            switch (Convert.ToInt32(status))
            {
                case 0:
                    _Status = "在库";
                    break;
                case 1:
                    _Status = "备料待接收";
                    break;
                case 2:
                    _Status = "接收备料";
                    break;
                case 3:
                    _Status = "待上线";
                    break;
                case 4:
                    _Status = "待退料";
                    break;
                case 5:
                    _Status = "移库中";
                    break;
                case 6:
                    _Status = "售出";
                    break;
                case 7:
                    _Status = "已使用";
                    break;
                case 8:
                    _Status = "已下线";
                    break;
                case 9:
                    _Status = "盘点完成";
                    break;
                case 10:
                    _Status = "盘点出库";
                    break;
                case 11:
                    _Status = "待检验";
                    break;
                case 12:
                    _Status = "待入库";
                    break;
                case 13:
                    _Status = "不良退库";
                    break;
                case 14:
                    _Status = "入烤箱";
                    break;
                case 15:
                    _Status = "出烤箱";
                    break;
                default:
                    _Status = "ERROR";
                    break;
            }
            return _Status;
        }

        //public static string ChktEditing(string FUNNAME, string PRJ, string USERID, string USERNAME)
        //{
        //    List<string> LsIp = new List<string>();
        //    LsIp = GetIPList();
        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add("FUNNAME", FUNNAME);
        //    dic.Add("PRJ", PRJ);
        //    dic.Add("USERID", USERID);
        //    dic.Add("USERNAME", USERNAME);
        //    dic.Add("PC_NAME", HostName + " IP[" + ((LsIp.Count > 1) ? LsIp[1] : LsIp[0])+"]");
        //    dic.Add("MAC_ADDRESS", getMacList()[0]);
        //    return RefWebService_BLL.refwebtEditing.Instance.ChktEditing(FrmBLL.ReleaseData.DictionaryToJson(dic));
        //}

        /// <summary>
        /// DataTableIsToDicList
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static IList<IDictionary<string, object>> DataTableIsToDicList(DataTable dt, IDictionary<string, object> dic = null)
        {
            IList<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                IDictionary<string, object> dct = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dic == null || dic.ContainsKey(dc.ColumnName))
                    {
                        dct.Add(dc.ColumnName, dr[dc.ColumnName].ToString());
                    }
                }
                list.Add(dct);
            }
            return list;
        }


        public static string GetWOtype(string sapwotype)
        {
            string sType = string.Empty;
            switch (sapwotype)
            {
                case "P701":
                    sType = "Normal";
                    break;
                case "P702":
                    sType = "Pilot Run";
                    break;
                case "P703":
                    sType = "Rework";
                    break;
                case "P704":
                    sType = "RMA";
                    break;
                case "P705":
                    sType = "PACK";
                    break;
                case "P706":
                    sType = "Try Run";
                    break;
                case "P707":
                    sType = "Tools";
                    break;
                case "P708":
                    sType = "Rework";
                    break;
                case "P709":
                    sType = "New Product";
                    break;
                case "P710":
                    sType = "Subcontract";
                    break;
                case "P711":
                    sType = "Normal";
                    break;
                default:
                    sType = "Other";
                    break;
            }
            return sType;
        }

        public static void UpdateFieldName(DataGridView dgv)
        {
            foreach (DataGridViewColumn dgvc in dgv.Columns)
            {
                dgvc.HeaderText = DB_Field(dgvc.Name);
            }
        }

        /// <summary>
        /// 根据DataTable第一行值填充控件
        /// </summary>
        /// <param name="FillCtrl"></param>
        /// <param name="dt"></param>
        public static void Fill_Control(Control FillCtrl, DataTable _dt)
        {
            foreach (Control ctrl in FillCtrl.Controls)
            {
                foreach (DataColumn dc in _dt.Columns)
                {
                    if (dc.ColumnName == ctrl.Name.Substring(4, ctrl.Name.Length - 4).ToUpper())
                    {
                        switch (ctrl.GetType().Name)
                        {
                            case "TextBox":
                                {
                                    if (_dt.Rows.Count > 0)
                                        ctrl.Text = _dt.Rows[0][ctrl.Name.Substring(4, ctrl.Name.Length - 4).ToUpper()].ToString();
                                    else
                                        ctrl.Text = string.Empty;
                                    break;
                                }
                            case "TextBoxX":
                                {
                                    if (_dt.Rows.Count > 0)
                                        ctrl.Text = _dt.Rows[0][ctrl.Name.Substring(4, ctrl.Name.Length - 4).ToUpper()].ToString();
                                    else
                                        ctrl.Text = string.Empty;
                                    break;
                                }
                            case "ComboBoxEx":
                                {
                                    if (_dt.Rows.Count > 0)
                                        ctrl.Text = _dt.Rows[0][ctrl.Name.Substring(4, ctrl.Name.Length - 4).ToUpper()].ToString();
                                    else
                                        ctrl.Text = string.Empty;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public static string DB_Field(string FieldName)
        {
            string _FieldName = FieldName;
            switch (FieldName)
            {
                case "ROUTGROUPID":
                    _FieldName = "途程代码";
                    break;
                case "ROUTGROUPDESC":
                    _FieldName = "途程描述";
                    break;
                case "WOID":
                    _FieldName = "工单";
                    break;
                case "PARTNUMBER":
                    _FieldName = "产品料号";
                    break;
                case "POID":
                    _FieldName = "订单编号";
                    break;
                case "QTY":
                    _FieldName = "数量";
                    break;
                case "WOSTATE":
                    _FieldName = "工单状态";
                    break;
                case "USERID":
                    _FieldName = "员工工号";
                    break;
                case "BOMVER":
                    _FieldName = "BOM版本";
                    break;
                case "INPUTGROUP":
                    _FieldName = "投入途程";
                    break;
                case "OUTPUTGROUP":
                    _FieldName = "产出途程";
                    break;
                case "WOTYPE":
                    _FieldName = "工单类型";
                    break;
                case "SAPWOTYPE":
                    _FieldName = "SAP工单类型";
                    break;
                case "RECDATE":
                    _FieldName = "时间";
                    break;
                case "PVER":
                    _FieldName = "版本";
                    break;
                case "BOMNUMBER":
                    _FieldName = "BOM编号";
                    break;
                case "INPUTQTY":
                    _FieldName = "投入数量";
                    break;
                case "OUTPUTQTY":
                    _FieldName = "产出数量";
                    break;
                case "SCRAPQTY":
                    _FieldName = "报废数量";
                    break;
                case "CPWD":
                    _FieldName = "密码类型";
                    break;
                case "PRODUCTNAME":
                    _FieldName = "产品名称";
                    break;
                case "WO_CLOSE_TIME":
                    _FieldName = "工单关闭时间";
                    break;
                case "SW_VER":
                    _FieldName = "软件版本";
                    break;
                case "FW_VER":
                    _FieldName = "硬件版本";
                    break;
                case "NAL_PREFIX":
                    _FieldName = "网标前缀";
                    break;
                case "CHECK_NO":
                    _FieldName = "检查编号";
                    break;
                case "ESN":
                    _FieldName = "唯一序号";
                    break;
                case "VERSIONCODE":
                    _FieldName = "版本";
                    break;
                case "LOCSTATION":
                    _FieldName = "当前途程";
                    break;
                case "STATIONNAME":
                    _FieldName = "当前站名";
                    break;
                case "WIPSTATION":
                    _FieldName = "下一途程";
                    break;
                case "NEXTSTATION":
                    _FieldName = "优先途程";
                    break;
                case "ERRFLAG":
                    _FieldName = "错误标记";
                    break;
                case "SCRAPFLAG":
                    _FieldName = "报废标记";
                    break;
                case "CARTONNUMBER":
                    _FieldName = "产品箱号";
                    break;
                case "MCARTONNUMBER":
                    _FieldName = "客户箱号";
                    break;
                case "PALLETNUMBER":
                    _FieldName = "栈板号";
                    break;
                case "MPALLETNUMBER":
                    _FieldName = "客户栈板";
                    break;
                case "LINE":
                    _FieldName = "线别";
                    break;
                case "SECTIONNAME":
                    _FieldName = "制程段";
                    break;
                case "STORENUMBER":
                    _FieldName = "入库单号";
                    break;
                case "WEIGHTQTY":
                    _FieldName = "重量";
                    break;
                case "QA_NO":
                    _FieldName = "品质检验编号";
                    break;
                case "QA_RESULT":
                    _FieldName = "品质检查结果";
                    break;
                case "ATE_STATION_NO":
                    _FieldName = "测试机台编号";
                    break;
                case "IN_LINE_TIME":
                    _FieldName = "上线时间";
                    break;
                case "REWORKNO":
                    _FieldName = "退回编号";
                    break;
                case "SNTYPE":
                    _FieldName = "条码类型";
                    break;
                case "SNVAL":
                    _FieldName = "条码值";
                    break;
                case "STATION":
                    _FieldName = "途程";
                    break;
                case "SNPREFIX":
                    _FieldName = "条码前缀";
                    break;
                case "SNPOSTFIX":
                    _FieldName = "条码后缀";
                    break;
                case "SNSTART":
                    _FieldName = "条码开始";
                    break;
                case "SNEND":
                    _FieldName = "条码结束";
                    break;
                case "SNLENG":
                    _FieldName = "条码长度";
                    break;
                case "USENUM":
                    _FieldName = "用量";
                    break;
                case "LINEID":
                    _FieldName = "线体";
                    break;
                case "CLEAR_SERIAL_TYPE":
                    _FieldName = "清除序号类型";
                    break;
                case "LOC":
                    _FieldName = "储位";
                    break;
                case "FACTORYID":
                    _FieldName = "工厂";
                    break;
            }
            return _FieldName;
        }

        public static string Material_Status(string Flag)
        {
            int C_Flag = Convert.ToInt32(Flag);
            string Msg = string.Empty;
            switch (C_Flag)
            {
                case 0: Msg = "在库";
                    break;
                case 1: Msg = "备料待接收";
                    break;
                case 2: Msg = "接收备料";
                    break;
                case 3: Msg = "待上线";
                    break;
                case 4: Msg = "待退料";
                    break;
                case 5: Msg = "移库中";
                    break;
                case 6: Msg = "售出";
                    break;
                case 7: Msg = "已使用";
                    break;
                case 8: Msg = "已下线";
                    break;
                case 9: Msg = "盘点完成";
                    break;
                case 10: Msg = "盘点出库";
                    break;
                case 11: Msg = "待IQC检验";
                    break;
                case 12: Msg = "待入库";
                    break;
                case 13: Msg = "IQC检验不良";
                    break;
                case 14: Msg = "烘烤中";
                    break;
                case 15: Msg = "烘烤完成";
                    break;
                default:
                    Msg = "物料状态没有定义";
                    break;
            }
            return Msg;
        }
        public static bool checkisnumber(string datecode)
        {
            try
            {
                int m = 0;
                m = int.Parse(datecode);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void WriteLog(string StrLog)
        {
            #region 存储失败日志在服务器
            try
            {
                //如果文件a.txt存在就打开，不存在就新建 .append 是追加写 
                string Patch = System.Environment.CurrentDirectory + "\\log";
                string LogFile = "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!File.Exists(Patch + LogFile))
                    Directory.CreateDirectory(Patch);
                FileStream fst = new FileStream(Patch + LogFile, FileMode.Append);
                //写数据到a.txt格式 
                StreamWriter swt = new StreamWriter(fst, System.Text.Encoding.GetEncoding("utf-8"));
                swt.WriteLine(StrLog + "  Time" + DateTime.Now.ToString());
                swt.Close();
                fst.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("写入Log失败:" + ex.Message + ",\r\n请及时联系SFIS人员");
            }
            #endregion
        }

        private static Dictionary<string, string> msg_trsn_LocId = new Dictionary<string, string>();
        //public static string Get_Tr_Sn_LocId(string KP_NO,string PLANT)
        //{
        //    Dictionary<string, object> mst = new Dictionary<string, object>();
        //    mst.Add("KP_NO", KP_NO);
        //    mst.Add("STOCK_ID", "1001");
        //    mst.Add("PLANT", PLANT);

        //    if (!msg_trsn_LocId.ContainsKey(mst["KP_NO"].ToString()))        
        //        msg_trsn_LocId.Add(mst["KP_NO"].ToString(), refWebtR_Tr_Sn.Instance.Get_Tr_Sn_LocId( ReleaseData.DictionaryToJson(mst)));
        //    return msg_trsn_LocId[mst["KP_NO"].ToString()];
        //}


        /// <summary>
        /// 获取字符串的字节数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int Get_String_ByteLength(string str)
        {
            return System.Text.Encoding.Default.GetBytes(str).Length;
        }


        /// <summary>
        /// 获取联璧表基础信息(STATUS,是否激活 PTP_CHANEL 渠道标记)
        /// </summary>
        /// <param name="PartNumber"></param>
        /// <returns></returns>
        //public static Dictionary<string, object> Get_Lianbi_Info(string PartNumber)
        //{
        //    Dictionary<string, object> dic_Msg = new Dictionary<string, object>();
        //    dic_Msg.Add("STATUS", "N");
        //    dic_Msg.Add("PTP_CHANEL", 0);
        //    Dictionary<string, object> mst = new Dictionary<string, object>();
        //    mst.Add("PARTNUMBER", PartNumber);
        //    DataTable dt = FrmBLL.ReleaseData.arrByteToDataTable(refWebB_LianBi_Info.Instance.Get_Lianbi_Info(FrmBLL.ReleaseData.DictionaryToJson(mst), "STATUS,PTP_CHANEL"));
        //    if (dt.Rows.Count > 0)
        //    {
        //        dic_Msg["STATUS"] = dt.Rows[0]["STATUS"].ToString();
        //        dic_Msg["PTP_CHANEL"] = Convert.ToInt32(dt.Rows[0]["PTP_CHANEL"].ToString());
        //    }
        //    return dic_Msg;
        //}
        /// <summary>
        /// 获取发货渠道 1联璧 2华夏
        /// </summary>
        /// <param name="Flag"></param>
        /// <returns></returns>
        public static string Get_PTP_CHANEL(int Flag)
        {
            string C_PTP_CHANEL = "非理财";
            switch (Flag)
            {
                case 1:
                    C_PTP_CHANEL = "联璧";
                    break;
                case 2:
                    C_PTP_CHANEL = "华夏";
                    break;
                default:
                    C_PTP_CHANEL = "非理财";
                    break;
            }

            return C_PTP_CHANEL;

        }
        //public static void Insert_Z_Whs_Lianbi_Result(string LianBi_Sn, int PTP_CHANNEL,string OrderNo,string Msg)
        //{
        //    IDictionary<string, object> dic_Resule = new Dictionary<string, object>();
        //    dic_Resule.Add("OPERATION", 0);
        //    dic_Resule.Add("LIANBI_SN", LianBi_Sn);
        //    if (Msg.Length > 255)
        //        Msg = Msg.Substring(0, 250);
        //    dic_Resule.Add("ALLOT_RESULT", Msg);
        //    dic_Resule.Add("PTP_CHANEL", PTP_CHANNEL);
        //    dic_Resule.Add("ORDER_NO", OrderNo);
        //    refWebtZ_Whs_Lianbi_Result.Instance.Insert_Z_Whs_Lianbi_Result(FrmBLL.ReleaseData.DictionaryToJson(dic_Resule));
        //}
        /// <summary>  
        /// 根据GUID获取19位的唯一数字序列  
        /// </summary>  
        /// <returns></returns>  
        public static long GuidToLongID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// 以字节数据截取字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string cutSubstring(string s, int length)
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(s);
            int n = 0;  //  表示当前的字节数
            int i = 0;  //  要截取的字节数
            for (; i < bytes.GetLength(0) && n < length; i++)
            {
                //  偶数位置，如0、2、4等，为UCS2编码中两个字节的第一个字节
                if (i % 2 == 0)
                {
                    n++;      //  在UCS2第一个字节时n加1
                }
                else
                {
                    //  当UCS2编码的第二个字节大于0时，该UCS2字符为汉字，一个汉字算两个字节
                    if (bytes[i] > 0)
                    {
                        n++;
                    }
                }
            }
            //  如果i为奇数时，处理成偶数
            if (i % 2 == 1)
            {
                //  该UCS2字符是汉字时，去掉这个截一半的汉字
                if (bytes[i] > 0)
                    i = i - 1;
                //  该UCS2字符是字母或数字，则保留该字符
                else
                    i = i + 1;
            }
            return System.Text.Encoding.Unicode.GetString(bytes, 0, i);
        }

 

        public class kpdelet
        {
            public string mcname { get; set; }
            public string tableIndex { get; set; }
            public string stationId { get; set; }
            public string partsName { get; set; }
            public string lor { get; set; }
            public string freeder { get; set; }
            public string loc { get; set; }
            public string pcbside { get; set; }
            public string smtsoftname { get; set; }
        }

        public class cSerialnumberRule
        {
            public string sFacCode;
            public string sYear;
            public string sMonth;
            public string sDay;
            public string sWoType;
            public string sNumber;
        }
    }
}
