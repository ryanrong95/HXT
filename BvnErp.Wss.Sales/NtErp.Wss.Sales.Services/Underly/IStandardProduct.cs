using NtErp.Wss.Sales.Services.Underly.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    public interface IStandardProduct : IUnique, INaming
    {
        /// <summary>
        /// b1b 标识
        /// </summary>
        string B1bSign { get; set; }

        /// <summary>
        /// 制造商
        /// </summary>
        string Manufacturer { get; set; }

        /// <summary>
        /// 暂时进行综合扩展
        /// </summary>
        Catalogs Catalogs { get; set; }

        /// <summary>
        /// 禁运 （暂时放在这里）
        /// </summary>
        Embargos Embargos { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        string Weight { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        Attachments Attachments { get; set; }
    }
}
