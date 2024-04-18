using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{

    public class IcgooFinanceReceipt
    {
        /// <summary>
        /// 付款人 固定”北京创新在线电子产品销售有限公司杭州分公司”
        /// </summary>
        public string Payer { get; set; }
        /// <summary>
        /// 付款流水号
        /// </summary>
        public string SeqNo { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime ReceiptDate { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal Amount { get; set; }
    }


    public class VerificationInfo
    {
        /// <summary>
        /// 是否全部核销
        /// </summary>
        public bool IsAllVerification { get; set; }
        public List<OrderFeeInfo> OrderFeeInfos { get; set; }

    }

    public class OrderFeeInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 货款 OrderReceipts里面，Type为货值(1) 值是 负的
        /// </summary>
        public decimal GoodsValue { get; set; }
        /// <summary>
        /// 关税 OrderReceipts里面，Type为关税(2) 值是 负的
        /// </summary>
        public decimal Tariff { get; set; }
        /// <summary>
        /// 增值税 OrderReceipts里面，Type为增值税(3) 值是 负的
        /// </summary>
        public decimal AddedVauleTax { get; set; }
        /// <summary>
        /// 代理费 OrderReceipts里面，Type为代理费(4) 值是 负的
        /// </summary>
        public decimal AgencyFee { get; set; }
        /// <summary>
        /// 杂费 OrderReceipts里面，Type为杂费(5) 值是 负的
        /// </summary>
        public decimal Incidental { get; set; }
        /// <summary>
        /// 美金权益(已申请付汇的金额，还是所有外币金额) 先不提供给他们
        /// </summary>
        //public decimal DollarEquity { get; set; }
        public decimal ExchangeRate { get; set; }


        public List<OrderItemInfo> OrderItemInfos { get; set; }
    }

    public class OrderItemInfo
    {
        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 产品唯一值
        /// </summary>
        public string ProductUnionCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }
    }

    public class EntrustPayExchange
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplyID { get; set; }
        /// <summary>
        /// 供应商中文名称
        /// </summary>
        public string SupplierChnName { get; set; }
        /// <summary>
        /// 供应商英文名称
        /// </summary>
        public string SupplierEngName { get; set; }
        /// <summary>
        /// 供应商银行名称英文
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 供应商银行地址英文
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 供应商银行账户
        /// </summary>
        public string BankAccount { get; set; }
        /// <summary>
        /// 供应商银行代码
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 付汇金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 付汇币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 期望付款日期
        /// </summary>
        public DateTime ExpectPayDate { get; set; }
    }

    public class SummaryVerificationInfo
    {
        /// <summary>
        /// Icgoo订单号
        /// </summary>
        public string IcgooOrder { get; set; }
        /// <summary>
        /// 是否核销
        /// </summary>
        public bool IsAllVerified { get; set; }
    }
}
