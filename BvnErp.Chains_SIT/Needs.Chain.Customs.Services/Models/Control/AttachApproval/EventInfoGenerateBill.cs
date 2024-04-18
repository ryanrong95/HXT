using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class EventInfoGenerateBill
    {
        /// <summary>
        /// 
        /// </summary>
        public string ApplyAdminName { get; set; }

        /// <summary>
        /// 大订单号
        /// </summary>
        public string MainOrderID { get; set; }

        /// <summary>
        /// 小订单信息
        /// </summary>
        public List<OneTinyOrderInfo> TinyOrderInfos { get; set; }
    }

    /// <summary>
    /// 一个小订单信息
    /// </summary>
    public class OneTinyOrderInfo
    {
        /// <summary>
        /// 小订单号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 旧的海关汇率
        /// </summary>
        public decimal OldCustomsExchangeRate { get; set; }

        /// <summary>
        /// 旧的实时汇率
        /// </summary>
        public decimal OldRealExchangeRate { get; set; }

        /// <summary>
        /// 旧的代理费类型
        /// </summary>
        public int OldOrderBillTypeInt { get; set; }

        /// <summary>
        /// 旧的代理费单价
        /// </summary>
        public decimal OldAgencyFeeUnitPrice { get; set; }

        /// <summary>
        /// 新的海关汇率
        /// </summary>
        public decimal NewCustomsExchangeRate { get; set; }

        /// <summary>
        /// 新的实时汇率
        /// </summary>
        public decimal NewRealExchangeRate { get; set; }

        /// <summary>
        /// 新的代理费类型
        /// </summary>
        public int NewOrderBillTypeInt { get; set; }

        /// <summary>
        /// 新的代理费单价
        /// </summary>
        public decimal NewAgencyFeeUnitPrice { get; set; }
    }
}
