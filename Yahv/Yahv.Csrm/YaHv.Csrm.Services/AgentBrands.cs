using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YaHv.Csrm.Services
{
    class AgentBrands {

    }
    ///// <summary>
    ///// 品牌获取
    ///// </summary>
    ///// <remarks>
    ///// 优化复杂要求的响应速度
    ///// </remarks>
    //public class AgentBrands : IEnumerable<StandardBrands.Brand>
    //{
    //    Thread thread;
    //    Brand[] data;

    //    StandardBrands()
    //    {
    //        var action = new Action(() =>
    //        {
    //            using (var view = new ())
    //            {
    //                data = view.Select(item => new StandardBrands.Brand
    //                {
    //                    ID = item.ID,
    //                    ChineseName = item.ChineseName,
    //                    IsAgent = item.IsAgent,
    //                    Name = item.Name,
    //                    ShortName = item.ShortName
    //                }).ToArray();
    //            }
    //        });
    //        action();

    //        (thread = new Thread(() =>
    //        {
    //            while (true)
    //            {
    //                try
    //                {
    //                    action();
    //                }
    //                catch (Exception)
    //                {

    //                }
    //                finally
    //                {
    //                    Thread.Sleep(100);
    //                }
    //            }

    //        })).Start();
    //    }


    //    static object locker = new object();
    //    static StandardBrands current;
    //    /// <summary>
    //    /// 当前实例
    //    /// </summary>
    //    static public StandardBrands Current
    //    {
    //        get
    //        {
    //            if (current == null)
    //            {
    //                lock (locker)
    //                {
    //                    if (current == null)
    //                    {
    //                        current = new StandardBrands();
    //                    }
    //                }
    //            }
    //            return current;
    //        }
    //    }

    //    public IEnumerator<Brand> GetEnumerator()
    //    {
    //        return this.data.Select(item => item).GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this.GetEnumerator();
    //    }


    //    /// <summary>
    //    /// 品牌
    //    /// </summary>
    //    public class Brand
    //    {
    //        /// <summary>
    //        /// ID
    //        /// </summary>
    //        public string ID { get; set; }
    //        /// <summary>
    //        /// 名称
    //        /// </summary>
    //        public string Name { get; set; }
    //        /// <summary>
    //        /// 中文名称
    //        /// </summary>
    //        public string ChineseName { get; set; }
    //        /// <summary>
    //        /// 简称
    //        /// </summary>
    //        public string ShortName { get; set; }
    //        /// <summary>
    //        /// 是否代理品牌
    //        /// </summary>
    //        public bool IsAgent { get; set; }
    //    }
    //}
}
