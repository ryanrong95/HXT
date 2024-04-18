using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otr
{
    public interface IStandardProduct
    {
        string ID { get; set; }

        string Name { get; set; }

        /// <summary>
        /// 制造商
        /// </summary>
        string Manufactruer { get; set; }
        /// <summary>
        /// 封装
        /// </summary>
        string PackageCase { get; set; }
        /// <summary>
        /// 包装
        /// </summary>
        string Packaging { get; set; }
        /// <summary>
        /// 晶元批次
        /// </summary>
        string Batch { get; set; }
        /// <summary>
        /// 封装批次
        /// </summary>
        string DateCode { get; set; }
    }
}
