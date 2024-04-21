using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp
{
    public static class RemoteManger
    {
        public static string Read(string url)
        {
            using (var webClient = new System.Net.WebClient() )
            {
                webClient.Encoding = Encoding.UTF8;
                return webClient.DownloadString(url);
            }
        }
    }
}
