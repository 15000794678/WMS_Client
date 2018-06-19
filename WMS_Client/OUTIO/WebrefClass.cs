using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS_Client.WMS_WebServices;

namespace RefWebService_BLL
{
    //public class refWebZ_Shopex_Sales
    //{
    //    private static WebServices.Z_Shopex_Sales.Z_Shopex_Sales instance;

    //    public static WebServices.Z_Shopex_Sales.Z_Shopex_Sales Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.Z_Shopex_Sales.Z_Shopex_Sales();
    //            return instance;
    //        }
    //    }
    //    static refWebZ_Shopex_Sales()
    //    {
    //        instance = new WebServices.Z_Shopex_Sales.Z_Shopex_Sales();
    //    }
    //}
    //public class refWebB_User_Info
    //{
    //    private static WebServices.b_user_info.b_user_info instance;

    //    public static WebServices.b_user_info.b_user_info Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.b_user_info.b_user_info();
    //            return instance;
    //        }
    //    }
    //    static refWebB_User_Info()
    //    {
    //        instance = new WebServices.b_user_info.b_user_info();
    //    }
    //}
    //public class refWebZ_Whs_Tracking
    //{
    //    private static WebServices.Z_Whs_Tracking.Z_Whs_Tracking instance;

    //    public static WebServices.Z_Whs_Tracking.Z_Whs_Tracking Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.Z_Whs_Tracking.Z_Whs_Tracking();
    //            return instance;
    //        }
    //    }
    //    static refWebZ_Whs_Tracking()
    //    {
    //        instance = new WebServices.Z_Whs_Tracking.Z_Whs_Tracking();
    //    }
    //}
    //public class refWebB_LianBi_Info
    //{
    //    private static WebServices.B_LianBi_Info.B_LianBi_Info instance;

    //    public static WebServices.B_LianBi_Info.B_LianBi_Info Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.B_LianBi_Info.B_LianBi_Info();
    //            return instance;
    //        }
    //    }
    //    static refWebB_LianBi_Info()
    //    {
    //        instance = new WebServices.B_LianBi_Info.B_LianBi_Info();
    //    }
    //}
    //public class refWebZ_Whs_Keypart
    //{
    //    private static WebServices.Z_WHS_KEYPART.Z_WHS_KEYPART instance;

    //    public static WebServices.Z_WHS_KEYPART.Z_WHS_KEYPART Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.Z_WHS_KEYPART.Z_WHS_KEYPART();
    //            return instance;
    //        }
    //    }
    //    static refWebZ_Whs_Keypart()
    //    {
    //        instance = new WebServices.Z_WHS_KEYPART.Z_WHS_KEYPART();
    //    }
    //}
    //public class refWebT_System_Log
    //{
    //    private static WebServices.T_System_Log.T_System_Log instance;

    //    public static WebServices.T_System_Log.T_System_Log Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.T_System_Log.T_System_Log();
    //            return instance;
    //        }
    //    }
    //    static refWebT_System_Log()
    //    {
    //        instance = new WebServices.T_System_Log.T_System_Log();
    //    }
    //}
    //public class refWebB_Parameter_Ini
    //{
    //    private static WebServices.B_Parameter_Ini.B_Parameter_Ini instance;

    //    public static WebServices.B_Parameter_Ini.B_Parameter_Ini Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.B_Parameter_Ini.B_Parameter_Ini();
    //            return instance;
    //        }
    //    }
    //    static refWebB_Parameter_Ini()
    //    {
    //        instance = new WebServices.B_Parameter_Ini.B_Parameter_Ini();
    //    }
    //}
    //public class refWebCheck_Version
    //{
    //    private static WebServices.Check_Version.Check_Version instance;

    //    public static WebServices.Check_Version.Check_Version Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.Check_Version.Check_Version();
    //            return instance;
    //        }
    //    }
    //    static refWebCheck_Version()
    //    {
    //        instance = new WebServices.Check_Version.Check_Version();
    //    }
    //}
    //public class refWebDomain_Ldap
    //{
    //    private static WebServices.Domain_Ldap.Domain_Ldap instance;

    //    public static WebServices.Domain_Ldap.Domain_Ldap Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.Domain_Ldap.Domain_Ldap();
    //            return instance;
    //        }
    //    }
    //    static refWebDomain_Ldap()
    //    {
    //        instance = new WebServices.Domain_Ldap.Domain_Ldap();
    //    }
    //}

    //public class refWebtR_Sap_Po
    //{
    //    private static WebServices.R_Sap_Po.R_Sap_Po instance;

    //    public static WebServices.R_Sap_Po.R_Sap_Po Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.R_Sap_Po.R_Sap_Po();
    //            return instance;
    //        }
    //    }
    //    static refWebtR_Sap_Po()
    //    {
    //        instance = new WebServices.R_Sap_Po.R_Sap_Po();
    //    }
    //}
    //public class refWebSapConnector
    //{
    //    private static WebServices.SapConnector.SapConnector instance;

    //    public static WebServices.SapConnector.SapConnector Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.SapConnector.SapConnector();
    //            return instance;
    //        }
    //    }
    //    static refWebSapConnector()
    //    {
    //        instance = new WebServices.SapConnector.SapConnector();
    //    }

    //}
    public class refWebtR_Tr_Sn
    {
        private static R_Tr_Sn instance;

        public static R_Tr_Sn Instance
        {
            get
            {
                if (instance == null)
                    instance = new R_Tr_Sn();
                return instance;
            }
        }
        static refWebtR_Tr_Sn()
        {
            instance = new R_Tr_Sn();
        }

    }
    //public class refWebtR_Part_Maps
    //{
    //    private static WebServices.R_Part_Maps.R_Part_Maps instance;

    //    public static WebServices.R_Part_Maps.R_Part_Maps Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.R_Part_Maps.R_Part_Maps();
    //            return instance;
    //        }
    //    }
    //    static refWebtR_Part_Maps()
    //    {
    //        instance = new WebServices.R_Part_Maps.R_Part_Maps();
    //    }

    //}
    //public class refWebtB_Wms_Factory
    //{
    //    private static WebServices.B_Wms_Factory.B_Wms_Factory instance;

    //    public static WebServices.B_Wms_Factory.B_Wms_Factory Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.B_Wms_Factory.B_Wms_Factory();
    //            return instance;
    //        }
    //    }
    //    static refWebtB_Wms_Factory()
    //    {
    //        instance = new WebServices.B_Wms_Factory.B_Wms_Factory();
    //    }

    //}
    //public class refWebtR_Loction_Info
    //{
    //    private static WebServices.R_Loction_Info.R_Loction_Info instance;

    //    public static WebServices.R_Loction_Info.R_Loction_Info Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.R_Loction_Info.R_Loction_Info();
    //            return instance;
    //        }
    //    }
    //    static refWebtR_Loction_Info()
    //    {
    //        instance = new WebServices.R_Loction_Info.R_Loction_Info();
    //    }

    //}
    //public class refWebtR_Stock_Info
    //{
    //    private static WebServices.R_Stock_Info.R_Stock_Info instance;

    //    public static WebServices.R_Stock_Info.R_Stock_Info Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.R_Stock_Info.R_Stock_Info();
    //            return instance;
    //        }
    //    }
    //    static refWebtR_Stock_Info()
    //    {
    //        instance = new WebServices.R_Stock_Info.R_Stock_Info();
    //    }

    //}

    //public class refWebtB_Storehouse_Info
    //{
    //    private static WebServices.B_Storehouse_Info.B_Storehouse_Info instance;

    //    public static WebServices.B_Storehouse_Info.B_Storehouse_Info Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.B_Storehouse_Info.B_Storehouse_Info();
    //            return instance;
    //        }
    //    }
    //    static refWebtB_Storehouse_Info()
    //    {
    //        instance = new WebServices.B_Storehouse_Info.B_Storehouse_Info();
    //    }

    //}
    //public class refWebtZ_Whs_SAP_ShipNotice
    //{
    //    private static WebServices.Z_Whs_SAP_ShipNotice.Z_Whs_SAP_ShipNotice instance;

    //    public static WebServices.Z_Whs_SAP_ShipNotice.Z_Whs_SAP_ShipNotice Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.Z_Whs_SAP_ShipNotice.Z_Whs_SAP_ShipNotice();
    //            return instance;
    //        }
    //    }
    //    static refWebtZ_Whs_SAP_ShipNotice()
    //    {
    //        instance = new WebServices.Z_Whs_SAP_ShipNotice.Z_Whs_SAP_ShipNotice();
    //    }

    //}
    //public class refWebtZ_Whs_Sap_Backflush
    //{
    //    private static WebServices.Z_Whs_Sap_Backflush.Z_Whs_Sap_Backflush instance;

    //    public static WebServices.Z_Whs_Sap_Backflush.Z_Whs_Sap_Backflush Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.Z_Whs_Sap_Backflush.Z_Whs_Sap_Backflush();
    //            return instance;
    //        }
    //    }
    //    static refWebtZ_Whs_Sap_Backflush()
    //    {
    //        instance = new WebServices.Z_Whs_Sap_Backflush.Z_Whs_Sap_Backflush();
    //    }

    //}
    //public class refWebtZ_Whs_Lianbi_Result
    //{
    //    private static WebServices.Z_Whs_Lianbi_Result.Z_Whs_Lianbi_Result instance;

    //    public static WebServices.Z_Whs_Lianbi_Result.Z_Whs_Lianbi_Result Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.Z_Whs_Lianbi_Result.Z_Whs_Lianbi_Result();
    //            return instance;
    //        }
    //    }
    //    static refWebtZ_Whs_Lianbi_Result()
    //    {
    //        instance = new WebServices.Z_Whs_Lianbi_Result.Z_Whs_Lianbi_Result();
    //    }

    //}

    //public class refWebtZ_Whs_Lianbi_Info
    //{
    //    private static WebServices.Z_Whs_Lianbi_Info.Z_Whs_Lianbi_Info instance;

    //    public static WebServices.Z_Whs_Lianbi_Info.Z_Whs_Lianbi_Info Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //                instance = new WebServices.Z_Whs_Lianbi_Info.Z_Whs_Lianbi_Info();
    //            return instance;
    //        }
    //    }
    //    static refWebtZ_Whs_Lianbi_Info()
    //    {
    //        instance = new WebServices.Z_Whs_Lianbi_Info.Z_Whs_Lianbi_Info();
    //    }

    //}
}
