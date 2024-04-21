using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Yahv.Erm.Fingerprints.Services
{
    public class GeckoHelper
    {
        /// <summary>
        /// 获取全部打印机名称
        /// </summary>
        /// <returns></returns>
        [GeckoFuntion]
        static public string[] GetAllPrinterNames()
        {
            string[] notIn = "发送至 OneNote 2010,Microsoft XPS Document Writer,Microsoft Print to PDF,Fax".Split(',');
            var view = PrinterSettings.InstalledPrinters.Cast<string>();
            return view.Where(item => !notIn.Contains(item)).ToArray();
        }

        /// <summary>
        /// 获取日期
        /// </summary>
        /// <returns></returns>
        [GeckoFuntion]
        static public string GetDate()
        {
            return TimeServer.Current.GetDate();
        }


        /// <summary>
        /// 获取日期时间
        /// </summary>
        /// <returns></returns>
        [GeckoFuntion]
        static public string GetDateTime()
        {
            return TimeServer.Current.GetDateTime();
        }
    }
}


