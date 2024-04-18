using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DollarEquityApply : IUnique
    {
        #region
        public string ID { get; set; }
        public string ApplyID { get; set; }
        public string ClientID { get; set; }
        public string SupplierChnName { get; set; }
        public string SupplierEngName { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string BankAccount { get; set; }
        public string SwiftCode { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public bool IsPaid { get; set; }       
        public DateTime ExpectDate { get; set; }
        public string SeqNo { get; set; }
        public Enums.PaymentType PayType { get; set; }
        public string FinanceVaultID { get; set; }
        public string FinanceAccountID { get; set; }
        public string PayerID { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Summary { get; set; }
        #endregion

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public DollarEquityApply()
        {
            this.Status = Status.Normal;
            this.CreateDate = DateTime.Now;            
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DollarEquityApplies>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DollarEquityApplies
                    {
                        ID = this.ID,
                        ApplyID = this.ApplyID,
                        ClientID = this.ClientID,
                        SupplierChnName = this.SupplierChnName,
                        SupplierEngName = this.SupplierEngName,
                        BankName = this.BankName,
                        BankAccount = this.BankAccount,
                        BankAddress = this.BankAddress,
                        SwiftCode = this.SwiftCode,
                        Amount = this.Amount,
                        Currency = this.Currency,
                        IsPaid = this.IsPaid,                    
                        ExpectDate = this.ExpectDate,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                }

                this.OnEnter();
            }
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 确认收款
        /// </summary>
        public void ConfirmPayment()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DollarEquityApplies>(new
                {
                    SeqNo = this.SeqNo,
                    PayType = (int)this.PayType,
                    FinanceVaultID = this.FinanceVaultID,
                    FinanceAccountID = this.FinanceAccountID,
                }, item => item.ID == this.ID);
            }
        }

    }
}
