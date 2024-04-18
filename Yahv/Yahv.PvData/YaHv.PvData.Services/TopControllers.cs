using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Npoi;

namespace YaHv.PvData.Services
{
    public class MyClass
    {
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 目标
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 是否交货
        /// </summary>
        public bool IsDelivery { get; set; }

        /// <summary>
        /// 确认时间
        /// </summary>
        /// <remarks>
        /// 数值日期型
        /// </remarks>
        public int? ConfirmedDate { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 索引金额交货
        /// </summary>
        /// <param name="price">金额</param>
        /// <returns>返回管控信息</returns>
        public bool _IsDelivery(decimal? price = null)
        {
            throw new NotImplementedException("返回数据");
        }
    }

    public class MyClasses : IEnumerable<MyClass>
    {
        /// <summary>
        /// 数据
        /// </summary>
        IEnumerable<MyClass> data;

        internal MyClasses(IEnumerable<MyClass> data)
        {
            this.data = data;
        }

        /// <summary>
        /// 索引来源目标数据
        /// </summary>
        /// <param name="source">来源</param>
        /// <param name="target">目标</param>
        /// <returns>返回管控信息</returns>
        public MyClass this[string source, string target]
        {
            get
            {
                return this.SingleOrDefault(item => item.Source == source && item.Target == target);
            }
        }

        public IEnumerator<MyClass> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// eccn只是美国管制中的做法之一，理论上可能会有任意个。
    /// 例如：美国管制
    /// 例如：香港管制
    /// 具体在应用中如何使用，是有一个限制就不做还是如何。这个可以未来听需求，如果在无需求下工作可以先做到 都管制
    /// </remarks>
    public class EccnControllers
    {
        /// <summary>
        /// 设置需要监视的目录
        /// </summary>
        string MonitoringPath { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"); } }

        /// <summary>
        /// 设置需要监视的文件
        /// </summary>
        string MonitoringFile { get { return "eccn.xlsx"; } }

        /// <summary>
        /// 管制数据
        /// </summary>
        Dictionary<string, MyClasses> data;

        /// <summary>
        /// 文件监控
        /// </summary>
        FileSystemWatcher watcher;

        internal EccnControllers()
        {
            this.data = this.InitData();

            #region 文件监控
            this.watcher = new FileSystemWatcher()
            {
                Path = this.MonitoringPath,
                IncludeSubdirectories = false,
                Filter = this.MonitoringFile,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                EnableRaisingEvents = true
            };

            //文件发生修改，重新加载数据
            this.watcher.Changed += (sender, e) => { this.data = this.InitData(); };
            #endregion
        }

        /// <summary>
        /// 返回初始化数据
        /// </summary>
        /// <returns></returns>
        /// <remarks>读取数据库或是类似xls等进行初始化</remarks>
        protected virtual Dictionary<string, MyClasses> InitData()
        {
            var data = new Dictionary<string, MyClasses>();

            //读取Excel
            string file = Path.Combine(this.MonitoringPath, this.MonitoringFile);
            IWorkbook workbook = ExcelFactory.CreateByTemplate(file);
            var npoi = new NPOIHelper(workbook);
            var dt = npoi.ExcelToDataTable(false);
            var rows = dt.Rows;

            //初始化数据字典
            foreach (System.Data.DataRow row in rows)
            {
                var key = row[0].ToString();
                if (data.ContainsKey(key))
                    continue;

                int? confirmedDate = row[4] as int?;
                string summary = row[5].ToString();

                var list = new List<MyClass>
                {
                    //美国-中国
                    new MyClass()
                    {
                        Source = "美国",
                        Target = "中国",
                        ConfirmedDate = confirmedDate,
                        Summary = summary
                    },
                    //美国-香港
                    new MyClass()
                    {
                        Source = "美国",
                        Target = "香港",
                        ConfirmedDate = confirmedDate,
                        Summary = summary
                    }
                };

                data.Add(key, new MyClasses(list));
            }

            return data;
        }

        /// <summary>
        /// 字典索引
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>限制集合</returns>
        public MyClasses this[string index]
        {
            get
            {
                if (this.data.ContainsKey(index))
                    return this.data[index];
                else
                    return new MyClasses(new List<MyClass>());
            }
        }

        static object locker = new object();
        static EccnControllers interior;

        static internal EccnControllers Interior
        {
            get
            {
                if (interior == null)
                {
                    lock (locker)
                    {
                        if (interior == null)
                        {
                            interior = new EccnControllers();
                        }
                    }
                }
                return interior;
            }
        }
    }

    /// <summary>
    /// 香港管制
    /// </summary>
    public class HkControllers : EccnControllers
    {
        protected override Dictionary<string, MyClasses> InitData()
        {
            return null;
        }
    }

    /// <summary>
    /// 管控
    /// </summary>
    public class TopControllers
    {
        TopControllers()
        {
            throw new NotSupportedException("不支持如此调用！");
        }

        /// <summary>
        /// 美国管制
        /// </summary>
        static public EccnControllers Eccn
        {
            get
            {
                return EccnControllers.Interior;
            }
        }

        /*
        由于欧盟与美国是联盟，因此目前Eccn就是欧盟的。可能在不断的将来就会独立
        香港属于自由贸易港（纽伦港），属于自由贸易器。管理一定是独立的，中国也有自贸区。但是目前世界上不承认，因为没有按照自由贸易通用要求开放。
        因此，未来只有两个方向需要开发。
        国家，自贸区
        联盟
        */
    }


    class tester
    {
        public tester()
        {
            bool isDelivery1 = TopControllers.Eccn["3A001.a.7"]["美国", "中国"].IsDelivery;
            bool isDelivery2 = TopControllers.Eccn["3A001.a.7"]["美国", "中国"]._IsDelivery();

            foreach (var item in TopControllers.Eccn["3A001.a.7"])
            {

            }
        }
    }

}
