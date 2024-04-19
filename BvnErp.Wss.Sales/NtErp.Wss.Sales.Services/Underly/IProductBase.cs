using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 实际product运营规范
    /// </summary>
    public interface IProductBase : IStandardProduct
    {
        /// <summary>
        /// 分销商标识
        /// </summary>
        string DistributorSign { get; set; }

        /// <summary>
        /// 封装
        /// </summary>
        /// <example>
        /// Tray
        /// reel
        /// Tape 
        /// tube
        /// </example>
        string PackageCase { get; set; }

        /// <summary>
        /// 包装
        /// </summary>
        string Packaging { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        int Count { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        string Batch { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        string Origin { get; set; }

        /// <summary>
        /// 最小起订量
        /// </summary>
        int Moq { get; set; }

        /// <summary>
        /// 最小金额限制
        /// </summary>
        decimal Mal { get; set; }

        /// <summary>
        /// 库存地
        /// </summary>
        District District { get; set; }

        /// <summary>
        /// 交货期
        /// </summary>
        string Leadtime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        string Summary { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        DateTime UpdateDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        SelfStatus Status { get; set; }

        /// <summary>
        /// 阶梯价
        /// </summary>
        Products.Prices.BasePricebreaks Prices { get; set; }

        string Supplier { get; set; }
    }
}
