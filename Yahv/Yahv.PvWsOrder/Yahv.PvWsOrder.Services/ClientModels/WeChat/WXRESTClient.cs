using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class WXRESTClient
    {
        public static string Get(string url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader readStream = new StreamReader(response.GetResponseStream(), encoding: Encoding.UTF8))
            {
                return readStream.ReadToEnd();
            }

        }

        public static string Post(string url, string data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader readStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return readStream.ReadToEnd();
            }

        }
    }
}
