using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AdvanceRecordModel : IUnique
    {
        #region 属性

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 垫资ID
        /// </summary>
        public string AdvanceID { get; set; }

        /// <summary>
        /// 付汇申请ID
        /// </summary>
        public string PayExchangeID { get; set; }

        /// <summary>
        /// 垫款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 还款金额
        /// </summary>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// 垫款日期
        /// </summary>
        public DateTime AdvanceTime { get; set; }
        /// <summary>
        /// 垫款期限
        /// </summary>
        public int LimitDays { get; set; }

        /// <summary>
        /// 利率
        /// </summary>
        public Decimal InterestRate { get; set; }

        /// <summary>
        /// 当前利息
        /// </summary>
        public Decimal InterestAmount { get; set; }

        /// <summary>
        /// 逾期利息
        /// </summary>
        public Decimal OverdueInterestAmount { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Enums.Status Status { get; set; }
        public string AdminID { get; set; }

        public DateTime CreateDate { get; set; }
        public decimal AmountUsed { get; set; }
        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion
        public virtual void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var payExchangeApplyItems = new Needs.Ccs.Services.Views.PayExchangeApplieItemsView().Where(item => item.PayExchangeApplyID == this.PayExchangeID).ToList();
                var advanceMoneyApply = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>().Where(t => t.ClientID == this.ClientID && t.Status == (int)Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective).FirstOrDefault();
                if (advanceMoneyApply != null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>(new
                    {
                        UpdateDate = DateTime.Now,
                        AmountUsed = this.AmountUsed + advanceMoneyApply.AmountUsed
                    }, t => t.ClientID == this.ClientID && t.Status == (int)Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective);
                    int count = 0;
                    decimal payAmount = 0;
                    foreach (var item in payExchangeApplyItems)
                    {

                        var payFatherID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>().Where(t => t.ID == this.PayExchangeID && t.FatherID != null).FirstOrDefault();

                        if (payFatherID != null)
                        {

                            item.PayExchangeApplyID = payFatherID.FatherID;
                            count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>().Where(t => t.PayExchangeID == item.PayExchangeApplyID && t.ClientID == this.ClientID && t.OrderID == item.OrderID).Count();
                        }
                        var payExchangeRate = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>().Where(t => t.ID == this.PayExchangeID && t.ClientID == this.ClientID).FirstOrDefault();
                        payAmount = (payExchangeRate.ExchangeRate * item.Amount).ToRound(2);
                        if (count == 0)
                        {
                            reponsitory.Insert<Layer.Data.Sqls.ScCustoms.AdvanceRecords>(new Layer.Data.Sqls.ScCustoms.AdvanceRecords
                            {
                                ID = Guid.NewGuid().ToString("N"),//this.ID,
                                ClientID = this.ClientID,
                                OrderID = item.OrderID,
                                AdvanceID = advanceMoneyApply.ID,
                                PayExchangeID = item.PayExchangeApplyID,
                                Amount = payAmount,//this.AmountUsed,//item.ReceivableAmount,
                                PaidAmount = 0,//item.ReceivedAmount,
                                AdvanceTime = DateTime.Now,
                                LimitDays = advanceMoneyApply.LimitDays,
                                InterestRate = advanceMoneyApply.InterestRate,
                                InterestAmount = 0, //advanceMoneyApply.InterestRate * advanceMoneyApply.LimitDays * (item.ReceivableAmount - item.ReceivedAmount),
                                OverdueInterestAmount = 0,//advanceMoneyApply.OverdueInterestRate * (item.ReceivableAmount - item.ReceivedAmount) * (DateTime.Now - advanceMoneyApply.CreateDate).Days,
                                Status = (int)Needs.Ccs.Services.Enums.Status.Normal,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                Summary = Summary,
                            });
                        }
                        else
                        {
                            var advanceRecordId = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>().Where(t => t.PayExchangeID == item.PayExchangeApplyID && t.ClientID == this.ClientID && t.OrderID == item.OrderID).FirstOrDefault();
                            if (advanceRecordId != null)
                            {
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceRecords>(new
                                {
                                    UpdateDate = DateTime.Now,
                                    Amount = payAmount + advanceRecordId.Amount//this.AmountUsed + advanceRecordId.Amount
                                }, t => t.ID == advanceRecordId.ID);
                            }
                        }
                    }

                }
            }
        }
        public virtual void Update()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var advanceMoneyApply = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>().Where(t => t.ClientID == this.ClientID && t.Status == (int)Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective).FirstOrDefault();
                if (advanceMoneyApply != null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>(new
                    {
                        UpdateDate = DateTime.Now,
                        AmountUsed = advanceMoneyApply.AmountUsed - this.AmountUsed
                    }, t => t.ClientID == this.ClientID && t.Status == (int)Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective);
                }
                var advanceRecords = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>().Where(t => t.ClientID == this.ClientID && t.AdvanceID == advanceMoneyApply.ID && t.OrderID == this.OrderID && t.PayExchangeID == this.PayExchangeID && t.Status == (int)Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();
                if (advanceRecords != null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceRecords>(new
                    {
                        UpdateDate = DateTime.Now,
                        PaidAmount = this.AmountUsed + advanceRecords.PaidAmount
                    }, t => t.ClientID == this.ClientID && t.AdvanceID == advanceMoneyApply.ID && t.OrderID == this.OrderID && t.PayExchangeID == this.PayExchangeID);
                }
            }
        }

        public virtual void AdvanceRecordUpdate()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var advanceMoneyApply = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>().Where(t => t.ClientID == this.ClientID && t.Status == (int)Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective).FirstOrDefault();
                if (advanceMoneyApply != null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>(new
                    {
                        UpdateDate = DateTime.Now,
                        AmountUsed = advanceMoneyApply.AmountUsed - this.AmountUsed
                    }, t => t.ClientID == this.ClientID && t.Status == (int)Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective);
                }
                var advanceRecords = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>().Where(t => t.ClientID == this.ClientID && t.AdvanceID == advanceMoneyApply.ID && t.OrderID == this.OrderID && t.PayExchangeID == this.PayExchangeID && t.Status == (int)Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();
                if (advanceRecords != null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceRecords>(new
                    {
                        UpdateDate = DateTime.Now,
                        PaidAmount = this.AmountUsed + advanceRecords.PaidAmount
                    }, t => t.ClientID == this.ClientID && t.AdvanceID == advanceMoneyApply.ID && t.OrderID == this.OrderID && t.PayExchangeID == this.PayExchangeID);
                }
            }
        }
    }
}
