
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Model.SsoUsers
{
    /// <summary>
    /// 财产
    /// </summary>
    public class Asset
    {
        public Asset()
        {

        }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 货币符号
        /// </summary>
        public string Symbol
        {
            get
            {
                return this.Currency.GetTitle();
            }
        }

        /// <summary>
        /// 总计
        /// </summary>
        public decimal Total { get; set; }
        /// <summary>
        /// 已用
        /// </summary>
        public decimal Used { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance
        {
            get
            {
                return this.Total - this.Used;
            }
        }

    }
}
