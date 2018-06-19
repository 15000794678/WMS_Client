using System.Text.RegularExpressions;

namespace Phicomm_WMS.DataProcess
{
    public class DataDispose
    {
        public static bool CheckIsNumberic(string message)
        {
            Regex rex = new Regex(@"^\d+$");
            return rex.IsMatch(message);
        }
    }
}
