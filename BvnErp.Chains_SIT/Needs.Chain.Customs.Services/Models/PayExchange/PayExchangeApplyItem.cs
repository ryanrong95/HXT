using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 付汇申请明细
    /// </summary>
    public class PayExchangeApplyItem : IUnique, IPersist
    {
        public string ID { get; set; }

        public string PayExchangeApplyID { get; set; }

        public string OrderID { get; set; }
        public string NewOrderID { get; set; }

        public decimal Amount { get; set; }
        public decimal OldAmount { get; set; }
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 报关总价
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 已申请付汇总价
        /// </summary>
        public decimal PaidExchangeAmount { get; set; }

        /// <summary>
        /// 应收货款总额
        /// </summary>
        public decimal ReceivableAmount { get; set; }

        /// <summary>
        /// 实收货款总额
        /// </summary>
        public decimal ReceivedAmount { get; set; }

        public string RealName { get; set; }

        public string UserID { get; set; }

        public string DyjIDs { get; set; }
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }
        public void PidEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var Summary = "";
                int Count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PrePayExchangeApplyLogs>().Count(item => item.OrderID == this.OrderID && item.PayExchangeApplyID == this.ID);
                if (Count == 0)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new { Amount = Amount }, item => item.PayExchangeApplyID == this.ID && item.OrderID == this.OrderID);
                    Summary = "用户[" + RealName + "]提交了匹配金额[" + this.Amount + "]";
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new { Amount = PaidExchangeAmount + Amount }, item => item.PayExchangeApplyID == this.ID && item.OrderID == this.OrderID);

                    Summary = "用户[" + RealName + "]提交了匹配金额[" + (PaidExchangeAmount + this.Amount) + "]";
                }
                //更新订单的已付汇金额（可能申请付汇详细表里有多笔记录（订单编号相同），不能直接取对应详细表里的已申请金额）
                var curOrder = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(item => item.ID == this.OrderID).FirstOrDefault();

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                    new { PaidExchangeAmount = curOrder.PaidExchangeAmount + Amount }, item => item.ID == this.OrderID);

                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PrePayExchangeApplyLogs>(new Layer.Data.Sqls.ScCustoms.PrePayExchangeApplyLogs
                {
                    ID = Guid.NewGuid().ToString("N"),
                    PayExchangeApplyID = this.ID,
                    OrderID = this.OrderID,
                    AdminID = null,
                    UserID = UserID,
                    CreateDate = DateTime.Now,
                    Summary = Summary,
                });
            }
        }
        public void PreItemEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var prePayment = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PrePayExchangeApplies>().FirstOrDefault(item => item.PayExchangeApplyID == PayExchangeApplyID && item.OrderID == this.OrderID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PrePayExchangeApplies>(new { Amount = prePayment.Amount - Amount }, item => item.PayExchangeApplyID == PayExchangeApplyID && item.OrderID == this.OrderID);


            }
        }
        public void NewPayEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>().Count(item => item.OrderID == this.OrderID);
                //if (count == 0)
                //{
                    var payExchangeApplies = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>().FirstOrDefault(item => item.ID == this.ID);
                    this.PayExchangeApplyID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApply);
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new Layer.Data.Sqls.ScCustoms.PayExchangeApplies
                    {
                        ID = this.PayExchangeApplyID,
                        AdminID = payExchangeApplies.AdminID,
                        UserID = payExchangeApplies.UserID,
                        ClientID = payExchangeApplies.ClientID,
                        SupplierName = payExchangeApplies.SupplierName,
                        SupplierEnglishName = payExchangeApplies.SupplierEnglishName,
                        SupplierAddress = payExchangeApplies.SupplierAddress,
                        BankAccount = payExchangeApplies.BankAccount,
                        BankAddress = payExchangeApplies.BankAddress,
                        BankName = payExchangeApplies.BankName,
                        SwiftCode = payExchangeApplies.SwiftCode,
                        ExchangeRateType = (int)payExchangeApplies.ExchangeRateType,
                        Currency = payExchangeApplies.Currency,
                        ExchangeRate = payExchangeApplies.ExchangeRate,
                        PaymentType = (int)payExchangeApplies.PaymentType,
                        ExpectPayDate = payExchangeApplies.ExpectPayDate,
                        SettlemenDate = payExchangeApplies.SettlemenDate,
                        OtherInfo = payExchangeApplies.OtherInfo,
                        PayExchangeApplyStatus = (int)payExchangeApplies.PayExchangeApplyStatus,
                        Status = (int)payExchangeApplies.Status,
                        CreateDate = payExchangeApplies.CreateDate,
                        UpdateDate = DateTime.Now,
                        Summary = payExchangeApplies.Summary,
                        ABA = payExchangeApplies.ABA,
                        IBAN = payExchangeApplies.IBAN
                    });

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyItem),
                        PayExchangeApplyID = this.PayExchangeApplyID,
                        OrderID = this.OrderID,
                        Amount = this.Amount,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        ApplyStatus = (int)Enums.ApplyItemStatus.Appling,
                    });
                    //更新订单的已付汇金额
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                        new { PaidExchangeAmount = PaidExchangeAmount + this.Amount }, item => item.ID == this.OrderID);

                    var prePayment = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PrePayExchangeApplies>().FirstOrDefault(item => item.PayExchangeApplyID == this.ID && item.OrderID == NewOrderID);

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PrePayExchangeApplies>(
                       new { Amount = prePayment.Amount - this.Amount }, item => item.OrderID == prePayment.OrderID && item.PayExchangeApplyID == this.ID);

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PrePayExchangeApplyLogs>(new Layer.Data.Sqls.ScCustoms.PrePayExchangeApplyLogs
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        PayExchangeApplyID = this.ID,
                        OrderID = this.OrderID,
                        AdminID = null,
                        UserID = UserID,
                        CreateDate = DateTime.Now,
                        Summary = "用户[" + RealName + "]提交了匹配金额[" + this.Amount + "]",
                    });
               // }
                //reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new { PayExchangeApplyID = PayExchangeApplyID, Amount = Amount }, item => item.PayExchangeApplyID == this.ID);

            }
        }
    }

    public class PayExchangeApplyItems : BaseItems<PayExchangeApplyItem>
    {
        internal PayExchangeApplyItems(IEnumerable<PayExchangeApplyItem> enums) : base(enums)
        {
        }

        internal PayExchangeApplyItems(IEnumerable<PayExchangeApplyItem> enums, Action<PayExchangeApplyItem> action) : base(enums, action)
        {
        }

        public override void Add(PayExchangeApplyItem item)
        {
            base.Add(item);
        }
    }
}