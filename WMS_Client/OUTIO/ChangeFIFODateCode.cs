using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
//using RefWebService_BLL;
using System.Windows.Forms;
using System.Net;
using System.Management;
using System.IO;

namespace FrmBLL
{
    public class ChangeFIFODateCode
    {
        ChangeFIFODateCode()
        {
        }
        public static string dc_week = "";
        public static string fifo_datecode(string datecode)
        {
            string datefifo = "";
            switch (datecode.Length)
            {
                case 11:
                    datefifo = ChangeFIFODateCode.fifodc_ten(datecode);
                    break;
                case 10:
                    datefifo = ChangeFIFODateCode.fifodc_ten(datecode);
                    break;
                case 9:
                    datefifo = ChangeFIFODateCode.fifodc_nine(datecode);
                    break;
                case 8:
                    datefifo = ChangeFIFODateCode.fifodc_eight(datecode);
                    break;
                case 7:
                    datefifo = ChangeFIFODateCode.fifodc_seven(datecode);
                    break;
                case 6:
                    datefifo = ChangeFIFODateCode.fifodc_six(datecode);
                    break;
                case 5:
                    datefifo = ChangeFIFODateCode.fifodc_five(datecode);
                    break;
                case 4:
                    datefifo = ChangeFIFODateCode.fifodc_four(datecode);
                    break;
                case 3:
                    datefifo = ChangeFIFODateCode.fifodc_three(datecode);
                    break;
            }
            return datefifo;
        }
        public static string fifodc_ten(string datecode)
        {
            string[] sArray = null;
            string fifodc = "";
            if (datecode.Split('/').Length > 1)
            {
                sArray = datecode.Split('/');
            }
            if (datecode.Split('-').Length > 1)
            {
                sArray = datecode.Split('-');
            }
            if (datecode.Split('.').Length > 1)
            {
                sArray = datecode.Split('.');
            }
            if (sArray != null && sArray.Length > 1 && sArray.Length == 3)
            {
                for (int i = 0; i < sArray.Length; i++)
                {
                    if (publicfuntion.checkisnumber(sArray[i]) == false)
                    {
                        fifodc = fifo_mm(sArray[i].ToUpper());
                        if (!string.IsNullOrEmpty(fifodc))
                        {
                            if (sArray.Length == 3)
                            {
                                fifodc = sArray[0] + fifodc + sArray[2].PadLeft(2, '0');
                            }
                            else if (sArray.Length == 2)
                            {
                                fifodc = sArray[0] + fifodc + "30";
                            }
                            else
                            {
                                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            }
                        }
                        else
                        {
                            fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                        }
                        break;
                    }
                }
                if (string.IsNullOrEmpty(fifodc))
                {

                    if (sArray[0].Length == 2 && sArray[2].Length == 4 && Convert.ToInt32(sArray[0]) < 13)
                    {
                        fifodc = sArray[2] + sArray[0] + sArray[1];
                    }
                    else if (sArray[0].Length == 4 && sArray[1].Length == 2 && Convert.ToInt32(sArray[1]) < 13)
                    {
                        fifodc = sArray[0] + sArray[1] + sArray[2];
                    }
                    else if (sArray[0].Length == 2 && sArray[2].Length == 4 && Convert.ToInt32(sArray[1]) < 13)
                    {
                        fifodc = sArray[2] + sArray[1] + sArray[0];
                    }
                    else
                    {
                        fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                    }
                    if (Convert.ToInt32(fifodc) > Convert.ToInt32(System.DateTime.Now.ToString("yyyyMMdd")))
                    {
                        fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                    }
                }
            }
            else
            {
                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
            }
            return fifodc;
        }
        public static string fifodc_nine(string datecode)
        {
            string[] sArray = null;
            string fifodc = "";
            if (datecode.Split('/').Length > 1)
            {
                sArray = datecode.Split('/');
            }
            if (datecode.Split('-').Length > 1)
            {
                sArray = datecode.Split('-');
            }
            if (datecode.Split('.').Length > 1)
            {
                sArray = datecode.Split('.');
            }
            if (sArray != null && sArray.Length > 1)
            {
                for (int i = 0; i < sArray.Length; i++)
                {
                    if (publicfuntion.checkisnumber(sArray[i]) == false)
                    {
                        fifodc = fifo_mm(sArray[i].ToUpper());
                        if (!string.IsNullOrEmpty(fifodc))
                        {
                            if (sArray.Length == 3 && sArray[0].Length == 2)
                            {
                                fifodc = "20" + sArray[0] + fifodc + sArray[2];
                            }
                            else if (sArray.Length == 2)
                            {
                                fifodc = sArray[0] + fifodc + "30";
                            }
                            else
                            {
                                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            }
                        }
                        else
                        {
                            fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                        }
                        break;
                    }
                }
                if (string.IsNullOrEmpty(fifodc))
                {
                    switch (sArray[0].Length)
                    {
                        case 1:
                            if (sArray[2].Length == 4 && Convert.ToInt32(sArray[0]) < 13)
                            {
                                fifodc = sArray[2] + sArray[0].PadLeft(2, '0') + sArray[1].PadLeft(2, '0');
                            }
                            else
                            {
                                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            }
                            break;
                        case 2:
                            if (sArray[2].Length == 4 && Convert.ToInt32(sArray[0]) < 13)
                            {
                                fifodc = sArray[2] + sArray[0].PadLeft(2, '0') + sArray[1].PadLeft(2, '0');
                            }
                            else if (sArray[2].Length == 4 && Convert.ToInt32(sArray[1]) < 13)
                            {
                                fifodc = sArray[2] + sArray[1].PadLeft(2, '0') + sArray[0].PadLeft(2, '0');
                            }
                            else
                            {
                                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            }
                            break;
                        case 4:
                            if (Convert.ToInt32(sArray[1]) < 13)
                            {
                                fifodc = sArray[0] + sArray[1].PadLeft(2, '0') + sArray[2].PadLeft(2, '0');
                            }
                            else
                            {
                                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            }
                            break;
                        case 6:
                            if (sArray[0].Length == 6 && sArray[1].Length == 2)
                            {
                                fifodc = sArray[0] + sArray[1];
                            }
                            break;
                    }

                }
            }
            else
            {
                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
            }
            if (Convert.ToInt32(fifodc) > Convert.ToInt32(System.DateTime.Now.ToString("yyyyMMdd")))
            {
                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
            }
            return fifodc;
        }
        public static string fifodc_eight(string datecode)
        {
            string[] sArray = null;
            string fifodc = "";
            if (datecode.Split('/').Length > 1)
            {
                sArray = datecode.Split('/');
            }
            if (datecode.Split('-').Length > 1)
            {
                sArray = datecode.Split('-');
            }
            if (datecode.Split('.').Length > 1)
            {
                sArray = datecode.Split('.');
            }
            if (sArray != null && sArray.Length > 1)
            {
                for (int i = 0; i < sArray.Length; i++)
                {
                    if (publicfuntion.checkisnumber(sArray[i]) == false)
                    {
                        fifodc = fifo_mm(sArray[i].ToUpper());
                        if (!string.IsNullOrEmpty(fifodc))
                        {
                            if (sArray.Length == 3 && sArray[0].Length == 2)
                            {
                                fifodc = "20" + sArray[0] + fifodc + sArray[2].PadLeft(2, '0');
                            }
                            else if (sArray.Length == 2)
                            {
                                fifodc = sArray[0] + fifodc + "30";
                            }
                            else
                            {
                                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            }
                        }
                        else
                        {
                            fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                        }
                        if (Convert.ToInt32(fifodc) > Convert.ToInt32(System.DateTime.Now.ToString("yyyyMMdd")))
                        {
                            fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                        }
                        break;
                    }
                }
                if (string.IsNullOrEmpty(fifodc))
                {
                    switch (sArray.Length)
                    {
                        case 3:
                            if (sArray[2].Length == 4 && Convert.ToInt32(sArray[0]) < 13)
                            {
                                fifodc = sArray[2] + sArray[0].PadLeft(2, '0') + sArray[1].PadLeft(2, '0');
                            }
                            else if (sArray[0].Length == 4 && Convert.ToInt32(sArray[1]) < 13)
                            {
                                fifodc = sArray[0] + sArray[1].PadLeft(2, '0') + sArray[2].PadLeft(2, '0');
                            }
                            else if (sArray[2].Length == 4 && Convert.ToInt32(sArray[1]) < 13)
                            {
                                fifodc = sArray[2] + sArray[1].PadLeft(2, '0') + sArray[0].PadLeft(2, '0');
                            }
                            else if (sArray[0].Length == 2 && sArray[1].Length == 2 && sArray[2].Length == 2)
                            {
                                fifodc = "20" + sArray[0] + sArray[1] + sArray[2];
                            }
                            if (Convert.ToInt32(fifodc) > Convert.ToInt32(System.DateTime.Now.ToString("yyyyMMdd")))
                            {
                                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            }
                            break;
                        case 2:
                            if (sArray[0].Length == 6 && Convert.ToInt32(sArray[1]) < 32)
                            {
                                fifodc = sArray[0] + sArray[1].PadLeft(2, '0');
                            }
                            break;
                        default:
                            fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            break;
                    }
                    if (Convert.ToInt32(fifodc) > Convert.ToInt32(System.DateTime.Now.ToString("yyyyMMdd")))
                    {
                        fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                    }
                }
            }
            else
            {
                if (publicfuntion.checkisnumber(datecode) == false)
                {
                    fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                }
                else
                {
                    fifodc = datecode;
                }
                if (Convert.ToInt32(fifodc) > Convert.ToInt32(System.DateTime.Now.ToString("yyyyMMdd")))
                {
                    fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                }
            }
            return fifodc;
        }
        public static string fifo_mm(string mothe)
        {
            string fifo_m = "";
            switch (mothe)
            {
                case "JAN":
                    fifo_m = "01";
                    break;
                case "FEB":
                    fifo_m = "02";
                    break;
                case "MAR":
                    fifo_m = "03";
                    break;
                case "APR":
                    fifo_m = "04";
                    break;
                case "MAY":
                    fifo_m = "05";
                    break;
                case "JUN":
                    fifo_m = "06";
                    break;
                case "JUL":
                    fifo_m = "07";
                    break;
                case "AUG":
                    fifo_m = "08";
                    break;
                case "SEP":
                    fifo_m = "09";
                    break;
                case "OCT":
                    fifo_m = "10";
                    break;
                case "NOV":
                    fifo_m = "11";
                    break;
                case "DEC":
                    fifo_m = "12";
                    break;
            }
            return fifo_m;
        }
        public static string fifodc_seven(string datecode)
        {
            string[] sArray = null;
            string fifodc = "";
            if (datecode.Split('/').Length > 1)
            {
                sArray = datecode.Split('/');
            }
            if (datecode.Split('-').Length > 1)
            {
                sArray = datecode.Split('-');
            }
            if (datecode.Split('.').Length > 1)
            {
                sArray = datecode.Split('.');
            }
            if (sArray != null && sArray.Length > 1)
            {
                for (int i = 0; i < sArray.Length; i++)
                {
                    if (publicfuntion.checkisnumber(sArray[i]) == false)
                    {
                        fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                        break;
                    }
                }
                if (string.IsNullOrEmpty(fifodc))
                {
                    switch (sArray.Length)
                    {
                        case 3:
                            fifodc = "20" + sArray[0] + sArray[1].PadLeft(2, '0') + sArray[2].PadLeft(2, '0');
                            break;
                        case 2:
                            if (sArray[0].Length == 4 && sArray[0].Substring(0, 3) == "201")
                            {
                                dc_week = (Convert.ToInt32(sArray[1]) * 7).ToString();
                                if (Convert.ToInt32(dc_week) > 365)
                                {
                                    dc_week = "365";
                                }
                               // fifodc = refWcfMtr_Business.Instance.Getdate_dc(sArray[0] + dc_week);
                                fifodc = Getdate_dc(sArray[0],dc_week);
                            }
                            else if (sArray[1].Length == 4 && sArray[1].Substring(0, 3) == "201")
                            {
                                dc_week = (Convert.ToInt32(sArray[0]) * 7).ToString();
                                if (Convert.ToInt32(dc_week) > 365)
                                {
                                    dc_week = "365";
                                }
                               // fifodc = refWcfMtr_Business.Instance.Getdate_dc(sArray[1] + dc_week);
                                fifodc = Getdate_dc(sArray[1] ,dc_week);
                            }
                            else
                            {
                                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            }
                            break;
                        default:
                            fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            break;
                    }
                    if (Convert.ToInt32(fifodc) > Convert.ToInt32(System.DateTime.Now.ToString("yyyyMMdd")))
                    {
                        fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                    }
                }
            }
            else
            {
                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
            }
            return fifodc;
        }
        public static string fifodc_six(string datecode)
        {
            string[] sArray = null;
            string fifodc = "";
            if (datecode.Split('/').Length > 1)
            {
                sArray = datecode.Split('/');
            }
            if (datecode.Split('-').Length > 1)
            {
                sArray = datecode.Split('-');
            }
            if (datecode.Split('.').Length > 1)
            {
                sArray = datecode.Split('.');
            }
            if (sArray != null && sArray.Length > 1)
            {
                for (int i = 0; i < sArray.Length; i++)
                {
                    if (publicfuntion.checkisnumber(sArray[i]) == false)
                    {
                        fifodc = fifo_mm(sArray[i].ToUpper());
                        if (!string.IsNullOrEmpty(fifodc))
                        {
                            if (sArray.Length == 2 && sArray[0].Length == 2)
                            {
                                fifodc = "20" + sArray[0] + fifodc + "30";
                            }
                            else
                            {
                                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            }
                        }
                        else
                        {
                            fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                        }
                        break;
                    }
                }
                if (string.IsNullOrEmpty(fifodc))
                {
                    switch (sArray.Length)
                    {
                        case 3:
                            fifodc = "20" + sArray[0] + sArray[1].PadLeft(2, '0') + sArray[2].PadLeft(2, '0');
                            break;
                        default:
                            fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            break;
                    }
                }
            }
            else
            {
                if (publicfuntion.checkisnumber(datecode) == false)
                {
                    if (publicfuntion.checkisnumber(datecode.Substring(1, 4)) == false)
                    {
                        fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                    }
                    else
                    {
                        dc_week = (Convert.ToInt32(datecode.Substring(3, 2)) * 7).ToString();
                        if (Convert.ToInt32(dc_week) > 365)
                        {
                            dc_week = "365";
                        }
                      //  fifodc = refWcfMtr_Business.Instance.Getdate_dc("20" + datecode.Substring(1, 2) + dc_week);
                        fifodc = Getdate_dc("20" + datecode.Substring(1, 2) , dc_week);
                    }
                }
                else
                {
                    if (datecode.Substring(0, 4) == System.DateTime.Now.ToString("yyyy") || datecode.Substring(0, 3) == "201")
                    {
                        dc_week = (Convert.ToInt32(datecode.Substring(4, 2)) * 7).ToString();
                        if (Convert.ToInt32(dc_week) > 365)
                        {
                            dc_week = "365";
                        }
                       // fifodc = refWcfMtr_Business.Instance.Getdate_dc(datecode.Substring(0, 4) + dc_week);
                        fifodc = Getdate_dc(datecode.Substring(0, 4) , dc_week);
                    }
                    else if (datecode.Substring(2, 3) == "201")
                    {
                        dc_week = (Convert.ToInt32(datecode.Substring(0, 2)) * 7).ToString();
                        if (Convert.ToInt32(dc_week) > 365)
                        {
                            dc_week = "365";
                        }
                        fifodc = Getdate_dc(datecode.Substring(2, 4) , dc_week);
                      //  fifodc = refWcfMtr_Business.Instance.Getdate_dc(datecode.Substring(2, 4) + dc_week);
                    }
                    else
                    {
                        fifodc = "20" + datecode;
                    }
                }
            }
            if (Convert.ToInt32(fifodc) > Convert.ToInt32(System.DateTime.Now.ToString("yyyyMMdd")))
            {
                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
            }
            return fifodc;
        }
        public static string fifodc_five(string datecode)
        {
            string[] sArray = null;
            string fifodc = "";
            if (datecode.Split('/').Length > 1)
            {
                sArray = datecode.Split('/');
            }
            if (datecode.Split('-').Length > 1)
            {
                sArray = datecode.Split('-');
            }
            if (datecode.Split('.').Length > 1)
            {
                sArray = datecode.Split('.');
            }
            if (sArray != null && sArray.Length > 1)
            {
                for (int i = 0; i < sArray.Length; i++)
                {
                    if (publicfuntion.checkisnumber(sArray[i]) == false)
                    {
                        fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                        break;
                    }
                }
                if (string.IsNullOrEmpty(fifodc))
                {
                    switch (sArray.Length)
                    {
                        case 2:
                            dc_week = (Convert.ToInt32(sArray[1]) * 7).ToString();
                            if (Convert.ToInt32(dc_week) > 365)
                            {
                                dc_week = "365";
                            }
                           // fifodc = refWcfMtr_Business.Instance.Getdate_dc("20" + sArray[0] + dc_week);
                            fifodc = Getdate_dc("20" + sArray[0] , dc_week);
                            break;
                        default:
                            fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                            break;
                    }
                }
            }
            else
            {
                if (publicfuntion.checkisnumber(datecode) == false)
                {
                    if (publicfuntion.checkisnumber(datecode.Substring(1, 4)) == false)
                    {
                        if (publicfuntion.checkisnumber(datecode.Substring(0, 4)) == false)
                        {
                            fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                        }
                        else
                        {
                            dc_week = (Convert.ToInt32(datecode.Substring(2, 2)) * 7).ToString();
                            if (Convert.ToInt32(dc_week) > 365)
                            {
                                dc_week = "365";
                            }
                            //fifodc = refWcfMtr_Business.Instance.Getdate_dc("20" + datecode.Substring(0, 2) + dc_week);
                            fifodc = Getdate_dc("20" + datecode.Substring(0, 2) , dc_week);
                        }
                    }
                    else
                    {
                        dc_week = (Convert.ToInt32(datecode.Substring(3, 2)) * 7).ToString();
                        if (Convert.ToInt32(dc_week) > 365)
                        {
                            dc_week = "365";
                        }
                        //fifodc = refWcfMtr_Business.Instance.Getdate_dc("20" + datecode.Substring(1, 2) + dc_week);
                        fifodc = Getdate_dc("20" + datecode.Substring(1, 2) , dc_week);
                    }
                }
                else
                {
                    fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                }
            }
            if (Convert.ToInt32(fifodc) > Convert.ToInt32(System.DateTime.Now.ToString("yyyyMMdd")))
            {
                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
            }
            return fifodc;
        }
        public static string fifodc_four(string datecode)
        {
            string[] sArray = null;
            string fifodc = "";
            if (datecode.Split('/').Length > 1)
            {
                sArray = datecode.Split('/');
            }
            if (datecode.Split('-').Length > 1)
            {
                sArray = datecode.Split('-');
            }
            if (datecode.Split('.').Length > 1)
            {
                sArray = datecode.Split('.');
            }
            if (sArray != null && sArray.Length > 1)
            {
                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
            }
            else
            {
                if (publicfuntion.checkisnumber(datecode) == false)
                {
                    fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                }
                else
                {
                    dc_week = (Convert.ToInt32(datecode.Substring(2, 2)) * 7).ToString();
                    if (Convert.ToInt32(dc_week) > 365)
                    {
                        dc_week = "365";
                    }
                  //  fifodc = refWcfMtr_Business.Instance.Getdate_dc("20" + datecode.Substring(0, 2) + dc_week);
                    fifodc = Getdate_dc("20" + datecode.Substring(0, 2) , dc_week);
                }
            }
            if (Convert.ToInt32(fifodc) > Convert.ToInt32(System.DateTime.Now.ToString("yyyyMMdd")))
            {
                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
            }
            return fifodc;
        }
        public static string fifodc_three(string datecode)
        {
            string[] sArray = null;
            string fifodc = "";
            if (datecode.Split('/').Length > 1)
            {
                sArray = datecode.Split('/');
            }
            if (datecode.Split('-').Length > 1)
            {
                sArray = datecode.Split('-');
            }
            if (datecode.Split('.').Length > 1)
            {
                sArray = datecode.Split('.');
            }
            if (sArray != null && sArray.Length > 1)
            {
                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
            }
            else
            {
                if (publicfuntion.checkisnumber(datecode) == false)
                {
                    if (publicfuntion.checkisnumber(datecode.Substring(1, 2)) == true)
                    {
                        dc_week = (Convert.ToInt32(datecode.Substring(1, 2)) * 7).ToString();
                        if (Convert.ToInt32(dc_week) > 365)
                        {
                            dc_week = "365";
                        }
                       // fifodc = refWcfMtr_Business.Instance.Getdate_dc(System.DateTime.Now.ToString("yyyy") + dc_week);
                        fifodc = Getdate_dc(DateTime.Now.ToString("yyyy") ,dc_week);
                    }
                }
                else
                {
                    fifodc = System.DateTime.Now.ToString("yyyyMMdd");
                }
            }
            if (Convert.ToInt32(fifodc) > Convert.ToInt32(System.DateTime.Now.ToString("yyyyMMdd")))
            {
                fifodc = System.DateTime.Now.ToString("yyyyMMdd");
            }
            return fifodc;
        }

        private static string Getdate_dc(string Year,string Days )
        {
            DateTime dt = Convert.ToDateTime(string.Format("{0}-01-01", Year)).AddDays(Convert.ToInt32(Days) - 1);
           return   dt.ToString("yyyyMMdd");
        }
    }
}
