using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

namespace FrmBLL
{
    public class ReleaseData
    {

        public static DataTable arrByteToDataTable(byte[] zipBuffer)
        {
            if (zipBuffer == null || zipBuffer.Length < 1)
                return null;
            byte[] buffer = UnZipClass.Decompress(zipBuffer);
            BinaryFormatter ser = new BinaryFormatter();
            DataSetSurrogate dss = ser.Deserialize(new MemoryStream(buffer)) as DataSetSurrogate;
            DataSet dataSet = dss.ConvertToDataSet();

            return dataSet.Tables[0];
        }

        public static DataSet arrByteToDataSet(byte[] zipBuffer)
        {
            if (zipBuffer == null || zipBuffer.Length < 1)
                return null;
            byte[] buffer = UnZipClass.Decompress(zipBuffer);
            BinaryFormatter ser = new BinaryFormatter();
            DataSetSurrogate dss = ser.Deserialize(new MemoryStream(buffer)) as DataSetSurrogate;
            DataSet dataSet = dss.ConvertToDataSet();

            return dataSet;
        }
        public static IDictionary<string, object> JsonToDictionary(string JosnStr)
        {
            return (IDictionary<string, object>)JsonConvert.DeserializeObject(JosnStr, typeof(IDictionary<string, object>));
        }
        public static string DictionaryToJson(IDictionary<string, object> Dic)
        {
            return JsonConvert.SerializeObject(Dic);
        }
        public static string ObjectToJson(Object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static string ListDictionaryToJson(IList<IDictionary<string, object>> LsDic)
        {
            return JsonConvert.SerializeObject(LsDic);
        }
        public static IList<IDictionary<string, object>> JsonToListDictionary(string JosnStr)
        {
            return (IList<IDictionary<string, object>>)JsonConvert.DeserializeObject(JosnStr, typeof(IList<IDictionary<string, object>>));
        }
    }
    public static class UnZipClass
    {
        public static byte[] Decompress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(data);
                Stream zipStream = null;
                zipStream = new GZipStream(ms, CompressionMode.Decompress);
                byte[] dc_data = null;
                dc_data = ExtractBytesFromStream(zipStream, data.Length);
                return dc_data;
            }
            catch
            {
                return null;
            }
        }
        public static byte[] ExtractBytesFromStream(Stream zipStream, int dataBlock)
        {
            byte[] data = null;
            int totalBytesRead = 0;
            try
            {
                while (true)
                {
                    Array.Resize(ref data, totalBytesRead + dataBlock + 1);
                    int bytesRead = zipStream.Read(data, totalBytesRead, dataBlock);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    totalBytesRead += bytesRead;
                }
                Array.Resize(ref data, totalBytesRead);
                return data;
            }
            catch
            {
                return null;
            }
        }
    }

}
