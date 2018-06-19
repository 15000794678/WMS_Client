using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LabelManager2;
using System.IO;
using System.Globalization;

namespace FrmBLL
{
   public class ClsCodeSoft7
    {

       public ClsCodeSoft7()
       {
       }
       ApplicationClass lbl = null;
       Document doc = null;
       int PrintQty = 0;
       public enum enumCounterBase
       {
           lppxBaseBinary = 2,
           lppxBaseOctal = 8,
           lppxBaseDecimal = 10,
           lppxBaseHexadecimal = 16,
           lppxBaseAlphabetic = 26,
           lppxBaseAlphaNumeric = 36,
           lppxBaseCustom = 255,
       }
       // enumCounterBase mFormula;
       /// <summary>
       /// 连接CodeSoft7
       /// </summary>
       public void OpenCodeSoft()
       {
           lbl = new ApplicationClass();
       }

       /// <summary>
       /// 检查模板路径是否存在
       /// </summary>
       /// <param name="FilePatch"></param>
       public void CheckFileExist(string FilePatch)
       {
           if (!File.Exists(FilePatch))
               throw new Exception(string.Format("Label File Not Found[{0}]", FilePatch));
       }

       /// <summary>
       /// 调用模板
       /// </summary>
       /// <param name="FilePatch"></param>
       /// <param name="ReadOnly"></param>
       /// <returns></returns>
       public string OpenLabelFile(string FilePatch, bool ReadOnly)
       {
           try
           {
               lbl.Documents.Open(FilePatch, ReadOnly);// 调用设计好的label文件
               doc = lbl.ActiveDocument;
               return "OK";
           }
           catch (Exception ex)
           {
               return ex.Message;
           }
       }
       /// <summary>
       /// 填充模板变量
       /// </summary>
       /// <param name="DicVariables"></param>
       public void Fill_Label_Variables(Dictionary<string, object> DicVariables)
       {
           foreach (KeyValuePair<string, object> _Variables in DicVariables)
           {
               try
               {                   
                   doc.Variables.FormVariables.Item(_Variables.Key).Value = _Variables.Value.ToString(); //给参数传值
                   SendLog(string.Format("Fill Variable[{0}]-->{1}", _Variables.Key, _Variables.Value.ToString()));
               }
               catch
               {
               }
           }

           if (DicVariables.Count==1)  //如果变量总数就一个,并且Key=TEST,则打印测试
            {
                if (DicVariables.ContainsKey("TEST"))
                {
                    for (int x = 0; x < doc.Variables.FormVariables.Count; x++)
                    {
                        doc.Variables.FormVariables.Item(x + 1).Value = "x".PadLeft(doc.Variables.FormVariables.Item(x + 1).Length, 'x');//doc.Variables.FormVariables.Item(x).Length
                    }
                }
            }

       }

       /// <summary>
       /// 清除模板变量默认值
       /// </summary>
       public void ClearVariables()
       {
           for (int i = 0; i < doc.Variables.FormVariables.Count; i++)
           {
               doc.Variables.FormVariables.Item(doc.Variables.FormVariables.Item(i + 1).Name).Value = "";
           }  
       }
       //private void SetFormula(string Key)
       //{
       //    doc.Variables.Counters.Item(Key).BaseType = mFormula;// LabelManager2.enumCounterBase.lppxBaseDecimal;
       //}

       /// <summary>
       /// 设置打印数量
       /// </summary>
       /// <param name="Num"></param>
       public void SetPrintNum(int Num)
       {
           PrintQty = Num;
       }
       /// <summary>
       /// 设置打印坐标
       /// </summary>
       /// <param name="PositionX"></param>
       /// <param name="PositionY"></param>
       public void SetPrintPosition(decimal PositionX, decimal PositionY)
       {
           doc.Format.MarginLeft =Convert.ToInt32( PositionX * 100);
           doc.Format.MarginTop = Convert.ToInt32(PositionY * 100);
       }
       /// <summary>
       /// 打印条码
       /// </summary>
       public void PrintLabel()
       {
           SendLog("PrintQty-->" + PrintQty.ToString());
           doc.PrintDocument(PrintQty);      
       }
       /// <summary>
       /// 退出CodeSoft
       /// </summary>
       public void QuitCodeSoft()
       {
           try
           {
               lbl.Quit();               
           }
           catch
           {
           }
       }

       public void SendLog(string StrLog)
       {
           deleevnet = new runEvent(SaveLog);
           deleevnet.BeginInvoke(StrLog, null, null);
       }
       delegate void runEvent(string StrLog);
       runEvent deleevnet;
       private void SaveLog(string StrLog)
       {
           #region 存储失败日志在服务器
           try
           {
               string TodayDate = System.DateTime.Now.ToString("yyyyMMdd");
               string FilePatch =Environment.CurrentDirectory+ "\\log";
               if (!Directory.Exists(FilePatch))
                   Directory.CreateDirectory(FilePatch);
               //如果文件a.txt存在就打开，不存在就新建 .append 是追加写 
               GregorianCalendar gc = new GregorianCalendar();
               // +gc.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
               FileStream fst = new FileStream(string.Format("{0}\\{1}.log", FilePatch, TodayDate), FileMode.Append);
               //写数据到a.txt格式 
               StreamWriter swt = new StreamWriter(fst, System.Text.Encoding.GetEncoding("utf-8"));
               swt.WriteLine(StrLog + "  Time:" + System.DateTime.Now.ToString());
               swt.Close();
               fst.Close();
           }
           catch
           {

           }
           #endregion
       }

    }
}
