using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.App_Utils
{
    /// <summary>
    /// 日志 Service
    /// </summary>
    public class FileLogService
    {
        private string fileLogPath = ConfigurationManager.AppSettings["FileLogPath"];
        FileStream fs = null;
        StreamWriter sw = null;

        public FileLogService(
            Needs.Ccs.Services.Enums.BalanceQueueBusinessType businessType,
            string businessID)
        {
            string fileFullPath = fileLogPath + @"\" + businessType.ToString() + @"\" + DateTime.Now.ToString("yyyyMM")
                + @"\" + businessType.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + businessID + ".txt";

            string path = Path.GetDirectoryName(fileFullPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            fs = new FileStream(fileFullPath, FileMode.OpenOrCreate);
            fs.Position = fs.Length;
            sw = new StreamWriter(fs);
        }

        public void Write(string text)
        {
            //开始写入
            sw.WriteLine(text);
            //清空缓冲区
            sw.Flush();
        }

        public void Close()
        {
            //关闭流
            sw.Close();
            fs.Close();
        }
    }
}
