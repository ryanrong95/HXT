using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YaHv.DyjData.Services
{
    /// <summary>
    /// 大赢家标准品牌
    /// </summary>
    public class DyjManufacturer
    {
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 代理品牌
        /// </summary>
        public bool IsAgent { get; set; }


        /// <summary>
        /// 比较器
        /// </summary>
        internal class Comparer : IComparer<DyjManufacturer>
        {
            CaseInsensitiveComparer caseiComp = new CaseInsensitiveComparer();
            public int Compare(DyjManufacturer x, DyjManufacturer y)
            {
                string xExt = x.Name;
                string yExt = y.Name;

                return caseiComp.Compare(xExt, yExt);
                //if (vExt == 0)
                //{
                //    return caseiComp.Compare(x, y);
                //}
                //else
                //{
                //    return vExt;
                //}
            }
        }
    }

    /// <summary>
    /// 大赢家品牌视图
    /// </summary>
    public class DyjManufacturerView : IEnumerable<DyjManufacturer>
    {
        DateTime lastWriteTime;
        SortedSet<DyjManufacturer> data;

        DyjManufacturerView()
        {
            this.lastWriteTime = DateTime.Now;
            this.Read();
            new Thread(delegate ()
            {
                while (true)
                {
                    try
                    {
                        this.Read();
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        Thread.Sleep(1000);
                    }
                }

            }).Start();
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>名称索引的结果</returns>
        public IEnumerable<DyjManufacturer> this[string index]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(index))
                {
                    return this;
                }
                return this.data.Where(item => item.Name.IndexOf(index, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        /// <summary>
        /// 读取
        /// </summary>
        void Read()
        {
            FileInfo fInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "dyj.manufacturers.txt"));

            if (fInfo.LastWriteTime != this.lastWriteTime)
            {
                this.lastWriteTime = DateTime.Now;

                using (var file = fInfo.OpenText())
                {

                    HashSet<string> hset = new HashSet<string>();
                    while (true)
                    {
                        string line = file.ReadLine();
                        if (line == null)
                        {
                            break;
                        }
                        line = line.Trim();
                        if (line.Length == 0)
                        {
                            continue;
                        }
                        hset.Add(line);
                    }
                    this.data = new SortedSet<DyjManufacturer>(hset.Select(item => new DyjManufacturer
                    {
                        Name = item,
                    }), new DyjManufacturer.Comparer());
                }
            }
        }

        public IEnumerator<DyjManufacturer> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        static object locker = new object();
        static DyjManufacturerView current;
        static public DyjManufacturerView Current
        {
            get
            {
                if (true)
                {
                    lock (locker)
                    {
                        if (true)
                        {
                            current = new DyjManufacturerView();
                        }
                    }
                }
                return current;
            }
        }
    }
}
