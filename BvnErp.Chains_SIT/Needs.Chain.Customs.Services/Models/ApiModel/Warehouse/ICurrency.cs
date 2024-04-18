using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.ApiModels.Warehouse
{
    /// <summary>
    /// 货币信息接口
    /// </summary>
    public interface ICurrency
    {
        /// <summary>
        /// 短名称
        /// </summary>
        string ShortName { get; }

        /// <summary>
        /// 符号
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// 短符号
        /// </summary>
        string ShortSymbol { get; }
    }
}
