using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// Part Adapter
    /// </summary>
    /// <example>
    /// 临时写法
    /// </example>
    public sealed class PartAdapter
    {
        /// <summary>
        /// 地区
        /// </summary>
        public Translators.Districts Districts { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Languages Languages { get; private set; }

        /// <summary>
        /// order逻辑翻译
        /// </summary>
        public object Order { get; private set; }

        PartAdapter()
        {
            this.Districts = new Translators.Districts();
            this.Languages = new Languages();
        }


        static PartAdapter runtime;
        static object lockcurrent = new object();

        public static PartAdapter Current
        {
            get
            {
                if (runtime == null)
                {
                    lock (lockcurrent)
                    {
                        if (runtime == null)
                        {
                            runtime = new PartAdapter();
                        }
                    }
                }

                return runtime;
            }
        }


    }
}
