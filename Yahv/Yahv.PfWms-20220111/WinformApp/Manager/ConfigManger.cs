using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp
{
    public class ConfigManger
    {
        public static string ConfigPath
        {
            get
            {
                var path = System.AppContext.BaseDirectory + "\\config";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
    }
}
