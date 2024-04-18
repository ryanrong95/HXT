using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otr
{
    /// <summary>
    /// 库存类型
    /// </summary>
    public enum InventoryType
    {
        /// <summary>
        /// 现货
        /// </summary>
        Spots = 1,
        /// <summary>
        /// 期货
        /// </summary>
        Futures = 2,
    }

    /// <summary>
    /// 库存信息
    /// </summary>
    public interface IInventory
    {
        /// <summary>
        /// 库存类型
        /// </summary>
        InventoryType Type { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        string Origin { get; set; }

        string Supplier { get; set; }

        int Count { get; set; }

        //string DistrictCode { get; set; }
        //string DistrictName { get; set; }

        string District { get; set; }

        IEnumerable<IPricebreak> QuotedPrices { get; set; }
    }
}
