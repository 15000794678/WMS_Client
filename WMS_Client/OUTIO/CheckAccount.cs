using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMS_Client.CheckAccoutServiceIService;


namespace Phicomm_WMS.OUTIO
{
    class CheckAccount
    {
        public static bool CheckUser(string user, string password)
        {
            try
            {
                CheckAccountServiceIService service = new CheckAccountServiceIService();
                
                service.Credentials = new System.Net.NetworkCredential("doubi.liu", "12345678");

                service.checkAccount(user, password);
                
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
