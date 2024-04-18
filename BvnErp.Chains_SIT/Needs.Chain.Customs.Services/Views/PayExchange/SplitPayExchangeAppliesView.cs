using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class SplitPayExchangeAppliesView : QueryView<SplitPayExchangeAppliesViewModel, ScCustomsReponsitory>
    {
        public SplitPayExchangeAppliesView()
        {
        }

        protected SplitPayExchangeAppliesView(ScCustomsReponsitory reponsitory, IQueryable<SplitPayExchangeAppliesViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<SplitPayExchangeAppliesViewModel> GetIQueryable()
        {
            var payExchangeApplyItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();
            var payExchangeApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();
            var iQuery = from paymentApplyItem in payExchangeApplyItems
                         join payExchangeApply in payExchangeApplies on paymentApplyItem.PayExchangeApplyID equals payExchangeApply.ID
                         where paymentApplyItem.Status == (int)Enums.Status.Normal
                            && payExchangeApply.Status == (int)Enums.Status.Normal
                         select new SplitPayExchangeAppliesViewModel
                         {
                             PaymentApplyItemID = paymentApplyItem.ID,
                             ID = paymentApplyItem.PayExchangeApplyID,
                             CreateDate = paymentApplyItem.CreateDate,
                             ClientID = payExchangeApply.ClientID,
                             SupplierName = payExchangeApply.SupplierName,
                             PayExchangeApplyStatus = (Enums.PayExchangeApplyStatus)payExchangeApply.PayExchangeApplyStatus,
                             TotalAmount = paymentApplyItem.Amount,
                             Currency = payExchangeApply.Currency,
                             ExchangeRate = payExchangeApply.ExchangeRate,
                             OrderID = paymentApplyItem.OrderID,
                             SupplierEnglishName = payExchangeApply.SupplierEnglishName,
                             SupplierAddress = payExchangeApply.SupplierAddress,
                             BankAccount = payExchangeApply.BankAccount,
                             BankAddress = payExchangeApply.BankAddress,
                             BankName = payExchangeApply.BankName,
                             SwiftCode = payExchangeApply.SwiftCode,
                             ExchangeRateType = payExchangeApply.ExchangeRateType,
                         };
            return iQuery;
        }

    }
    public class SplitPayExchangeAppliesViewModel
    {
        /// <summary>
        /// PayExchangeApplyID
        /// </summary>
        public string ID { get; set; }

        public string PaymentApplyItemID { get; set; }
        /// <summary>
        /// PayExchangeApply 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 供应商英文名称
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 供应商地址
        /// </summary>
        public string SupplierAddress { get; set; }

        /// <summary>
        /// 付汇申请状态
        /// </summary>
        public Enums.PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }

        /// <summary>
        /// 付汇汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        ///预付汇申请金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }
        public string BankAccount { get; set; }
        public string BankAddress { get; set; }
        public string BankName { get; set; }
        public string SwiftCode { get; set; }
        public int ExchangeRateType { get; set; }
        public Enums.PaymentType PaymentType { get; set; }
        public DateTime? ExpectPayDate { get; set; }
        public string ABA { get; set; }
        public string IBAN { get; set; }

    }
}
