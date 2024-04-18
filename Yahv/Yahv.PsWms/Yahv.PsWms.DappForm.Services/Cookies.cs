using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.DappForm.Services
{
    public class Cookies
    {
        static object locker = new object();
        static public void Cookie(object context)
        {
            ////没有实现
            //throw new Exception("6");

            lock (locker)
            {
                var jObject = new JObject();

                var cookie = jObject["cookies"] = new JObject();

                foreach (var item in context.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item =>
                {
                    return item.Trim().Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                }))
                {
                    cookie[item[0]] = item[1];
                }

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tcoc");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string CookiePath = Path.Combine(path, $"Ring.cookie");
                File.WriteAllText(CookiePath, jObject.ToString(), Encoding.UTF8);
            }
        }
    }
}
