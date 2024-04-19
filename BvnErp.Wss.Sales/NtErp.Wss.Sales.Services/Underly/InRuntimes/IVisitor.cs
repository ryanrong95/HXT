using Needs.Underly.Translators;
using NtErp.Wss.Sales.Services.Underly.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.InRuntimes
{
    /// <summary>
    /// 访问者接口
    /// (同时也是前端任意端管控的类型)
    /// </summary>
    public interface IVisitor
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        string SessionID { get; }

        /// <summary>
        /// 交货地
        /// </summary>
        District Delivery { get; }

        /// <summary>
        /// 显示信息
        /// </summary>
        Displayer Displayer { get; }

        /// <summary>
        /// 交易币种
        /// </summary>
        Currency Transaction { get; }

        /// <summary>
        /// 报价币种
        /// </summary>
        Currency Quotation { get; }

        /// <summary>
        /// 语言信息
        /// </summary>
        Language Language { get; }
    }
}
