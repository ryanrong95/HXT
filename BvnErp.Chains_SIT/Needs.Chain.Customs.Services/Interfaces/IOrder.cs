using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Interfaces
{
    /// <summary>
    /// 订单
    /// </summary>
    public interface IOrder : IUnique
    {
        #region 属性

        /// <summary>
        /// 订单类型：内单、外单、Icgoo
        /// </summary>
        Enums.OrderType Type { get; set; }

        /// <summary>
        /// 下单时的跟单员
        /// </summary>
        string AdminID { get; set; }

        /// <summary>
        /// 平台用户
        /// </summary>
        string UserID { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        Client Client { get; set; }

        /// <summary>
        /// 下单时的会员补充协议
        /// </summary>
        ClientAgreement ClientAgreement { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        string Currency { get; set; }

        /// <summary>
        /// 报价时的海关税率
        /// </summary>
        decimal? CustomsExchangeRate { get; set; }

        /// <summary>
        /// 报价时的实时汇率
        /// </summary>
        decimal? RealExchangeRate { get; set; }

        /// <summary>
        /// 是否包车：Yes/No
        /// </summary>
        bool IsFullVehicle { get; set; }

        /// <summary>
        /// 是否垫款：Yes/No
        /// </summary>
        bool IsLoan { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        int? PackNo { get; set; }

        /// <summary>
        /// 包装种类
        /// </summary>
        string WarpType { get; set; }

        /// <summary>
        /// 报关总货值（外币）
        /// </summary>
        decimal DeclarePrice { get; set; }

        /// <summary>
        /// 订单的开票状态
        /// </summary>
        Enums.InvoiceStatus InvoiceStatus { get; set; }

        /// <summary>
        /// 已申请付汇金额
        /// </summary>
        decimal PaidExchangeAmount { get; set; }

        /// <summary>
        /// 是否挂起：Yes/No
        /// </summary>
        bool IsHangUp { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        Enums.OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        Enums.Status Status { get; set; }

        DateTime CreateDate { get; set; }

        DateTime UpdateDate { get; set; }

        string Summary { get; set; }

        string MainOrderID { get; set; }
        /// <summary>
        /// 对账单生成方式
        /// </summary>
        Enums.OrderBillType OrderBillType { get; set; }
        /// <summary>
        /// 订单是否报关，跟单匹配到货信息用
        /// </summary>
        Enums.DeclareFlagEnums DeclareFlag { get; set; }
        /// <summary>
        /// 订单的香港接货方式
        /// </summary>
        OrderConsignee OrderConsignee { get; set; }

        /// <summary>
        /// 订单的深圳交货方式
        /// </summary>
        OrderConsignor OrderConsignor { get; set; }

        /// <summary>
        /// 付汇供应商
        /// </summary>
        OrderPayExchangeSuppliers PayExchangeSuppliers { get; set; }

        /// <summary>
        /// 订单文件
        /// </summary>
        OrderFiles Files { get; set; }

        /// <summary>
        /// 主订单文件
        /// </summary>
        MainOrderFiles MainOrderFiles { get; set; }

        /// <summary>
        /// 订单日志
        /// </summary>
        OrderLogs Logs { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        OrderItems Items { get; set; }

        /// <summary>
        /// 订单附加费用
        /// </summary>
        OrderPremium[] Premiums { get; set; }

        /// <summary>
        /// 订单轨迹
        /// </summary>
        OrderTraces Traces { get; set; }

        decimal? CollectedAmount { get; set; }

        #endregion

        /// <summary>
        /// 订单挂起
        /// </summary>
        /// <param name="controlType"></param>
        void HangUp(Enums.OrderControlType controlType, string summary = null);
    }
}