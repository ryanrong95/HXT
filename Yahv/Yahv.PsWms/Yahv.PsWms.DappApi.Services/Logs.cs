using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.DappApi.Services
{
    public class Logs
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
                writer.Write("Source：");
                writer.WriteLine(ex.Source);
                writer.WriteLine();
                writer.Close();
            }
        }

        public static void Log(string msg, Exception ex)
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
                writer.Write("msg:");
                writer.WriteLine(msg);
                writer.WriteLine();
                writer.Write("StackTrace：");
                writer.WriteLine(ex.StackTrace);
                writer.WriteLine();
                writer.Write("Source：");
                writer.WriteLine(ex.Source);
                writer.WriteLine();
                writer.Close();
            }
        }
    }
}
