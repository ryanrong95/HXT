using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Services.Common
{
    public static class Tools
    {
        public static void Log(Exception ex)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applog", DateTime.Now.ToString("yyyyMMdd") + ".txt");

            var fileInfo = new FileInfo(path);
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            using (var stream = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                writer.Write('：');
                writer.Write(ex.Message);
                writer.WriteLine();
                writer.Write("StackTrace：");
                writer.WriteLine(ex.StackTrace);
                writer.WriteLine();
                writer.Close();
            }
        }
    }
}
