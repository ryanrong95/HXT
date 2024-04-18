using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class RefundApply : IUnique
    {
        public string ID { get; set; }
        /// <summary>
        /// 客户信息
        /// </summary>
        public Client Client { get; set; }
        /// <summary>
        /// 退款账号信息
        /// </summary>
        public FinanceAccount PayeeAccount { get; set; }
        public string PayeeAccountID { get; set; }
        /// <summary>
        /// 收款信息
        /// </summary>
        public FinanceReceipt ReceiptInfo { get; set; }
        public string FinanceReceiptID { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 币制
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal? ExchangeRate { get; set; }
        /// <summary>
        /// 退款状态
        /// </summary>
        public Enums.RefundApplyStatus ApplyStatus { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public Admin Applicant { get; set; }
        /// <summary>
        /// 付款人
        /// </summary>
        public Admin Payer { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public RefundApply()
        {
            this.Currency = "CNY";
            this.ExchangeRate = 1;
            this.ApplyStatus = Enums.RefundApplyStatus.Applied;
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.RefundApply>().Count(item => item.ID == this.ID);
                if (count == 0)
                {                   
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.RefundApply
                    {
                        ID = this.ID,
                        ClientID = this.Client.ID,
                        FinanceReceiptID = this.ReceiptInfo.ID,
                        Amount = this.Amount,
                        Currency = this.Currency,
                        ExchangeRate = this.ExchangeRate,
                        AdminID = this.Applicant.ID,
                        ApplyStatus = (int)this.ApplyStatus,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.RefundApply>(
                        new
                        {
                           
                            Status = (int)this.Status,
                            UpdateDate = this.UpdateDate,
                            Summary = this.Summary
                        }, item => item.ID == this.ID);
                }
            }
        }

       
        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="ApplyStatus"></param>
        public void Approve(Enums.RefundApplyStatus ApplyStatus)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.RefundApply>(
                        new
                        {
                            ApplyStatus = (int)ApplyStatus,
                            UpdateDate = DateTime.Now
                        }, item => item.ID == this.ID);
            }
        }

        public void UpdateAccount()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.RefundApply>(
                        new
                        {
                            PayeeAccountID = this.PayeeAccountID,
                            UpdateDate = DateTime.Now
                        }, item => item.ID == this.ID);
            }
        }
    }
}
