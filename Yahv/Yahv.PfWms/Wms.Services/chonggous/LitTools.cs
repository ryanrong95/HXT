using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.chonggous.Views
{
    /// <summary>
    /// 临时使用记录日志
    /// </summary>
    public class LitTools
    {
        static object llocker = new object();

        LitTools() : this("unions")
        {

        }

        string name;
        LitTools(string name)
        {

            this.name = name;
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="lines"></param>
        public void Log(params string[] lines)
        {

#if TEST
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.name + ".txt");
            lock (llocker)
            {
                File.AppendAllText(path, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), Encoding.UTF8);
                File.AppendAllLines(path, lines.Select(item => item + "\r\n"), Encoding.UTF8);
                File.AppendAllText(path, "\r\n");
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LitTools this[string index]
        {
            get { return new LitTools(index); }
        }

        static LitTools current;
        static object locker = new object();

        /// <summary>
        /// 访问单例
        /// </summary>
        static public LitTools Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new LitTools();
                        }
                    }
                }
                return current;
            }
        }
    }
}
