using Needs.Underly.Attributes;
using Needs.Underly.Legals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
{
    /// <summary>
    /// 法定的
    /// </summary>


    [Obsolete("准备废弃")]
    public class Legally
    {
        Legally()
        {

        }

        /// <summary>
        /// 货币
        /// </summary>
        /// <param name="index">货币类型</param>
        /// <returns>货币信息</returns>
        public ICurrency this[Currency index]
        {
            get { return Legal<Currency, CurrenyAttribute>.Current[index]; }
        }

        /// <summary>
        /// 行政区
        /// </summary>
        /// <param name="index">地区类型</param>
        /// <returns>地区信息</returns>
        public IDistrict this[District index]
        {
            get { return Legal<District, DistrictAttribute>.Current[index]; }
        }

        static Legally currenct;
        static object locker = new object();
        static public Legally Current
        {
            get
            {
                if (currenct == null)
                {
                    lock (locker)
                    {
                        if (currenct == null)
                        {
                            currenct = new Legally();
                        }
                    }
                }
                return currenct;
            }
        }
    }
}
