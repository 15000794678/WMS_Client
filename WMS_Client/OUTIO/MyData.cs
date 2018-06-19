using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Phicomm_WMS.OUTIO
{
    class MyData
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal,
            Byte[] retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
            StringBuilder retVal, int size, string filePath);

        private static string _iniPath = Environment.CurrentDirectory + "\\Config\\config.ini";

        private static string _user = string.Empty;
        private static string _password = string.Empty;

        private static int _stationId = 0; //站点编号，如，1，2
        private static int _stockNoType = 0;//工单类型:入库，退料，出库等
        private static string _stockSubType = string.Empty; //10003
        private static string _stockNo = string.Empty;//工单，如702003226

        //private static int _lastSelect = -1;

        public static LocInformation _lastArrive = new LocInformation();

        public enum PickWoType
        {
            Normal =1, //工单发料
            Delivery =2,//售出
            Reserve =3,//预留
            OutSource =4,//委外
            Transfer =5,//移库
            Super =6, //超领
            Max
        };

        public enum ReplenishWoType
        {
            Normal = 7, //工单入库
            Back = 8, //退料
            Max
        };

        public MyData()
        {
            _lastArrive.select = 0;
            _lastArrive.stockno = "";
            _lastArrive.locid = "";
            _lastArrive.kpno = "";
            _lastArrive.qty = "0";
            _lastArrive.allout = "0";
            _lastArrive.hoderid = "";
            _lastArrive.col = 0;
            _lastArrive.cnt = 0;
        }

        public static void Clear()
        {
            _lastArrive.select = 0;
            _lastArrive.stockno = "";
            _lastArrive.locid = "";
            _lastArrive.kpno = "";
            _lastArrive.qty = "0";
            _lastArrive.allout = "0";
            _lastArrive.hoderid = "";
            _lastArrive.col = 0;
            _lastArrive.cnt = 0;
        }

        public static string ReadValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, _iniPath);
            return temp.ToString();
        }

        public static void SetUser(string value)
        {
            _user = value;
        }
        
        public static string GetUser()
        {
            return _user;
        }

        public static void SetPassword(string value)
        {
            _password = value;
        }

        public static string GetPassword()
        {
            return _password;
        }
        public static void SetStationId(int value)
        {
            _stationId = value;
        }

        public static int GetStationId()
        {
            return _stationId;
        }

        public static void SetStockNoType(int value)
        {
            _stockNoType = value;
        }

        public static int GetStockNoType()
        {
            return _stockNoType;
        }

        public static void SetStockSubType(string value)
        {
            _stockSubType = value;
        }

        public static string GetStockSubType()
        {
            return _stockSubType;
        }

        public static void SetStockNo(string value)
        {
            _stockNo = value;
        }

        public static string GetStockNo()
        {
            return _stockNo;
        }
    }

    public class LocInformation
    {
        public int select { get; set; }
        public string stockno { get; set; }
        public string locid { get; set; }
        public string kpno { get; set; }
        public string qty { get; set; }
        public string allout { get; set; }
        public string hoderid { get; set; }
        public int col { get; set; } //货架列数
        public int cnt { get; set; } //储位里面剩下的盘数
    }

    public class CapacityType
    {
        public int current { get; set; }
        public int total { get; set; }
    }
}
