using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// SwapNotice
    /// </summary>
    [Serializable]
    public class SwapNotice : IUnique, IPersist
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string AdminID { get; set; } = string.Empty;

        /// <summary>
        /// 换汇银行
        /// </summary>
        public string BankName { get; set; } = string.Empty;

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 境外发货人
        /// </summary>
        public string ConsignorCode { get; set; } = string.Empty;

        /// <summary>
        /// 换汇总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 换汇汇率
        /// </summary>
        public decimal? ExchangeRate { get; set; }

        /// <summary>
        /// 换汇所用RMB
        /// </summary>
        public decimal? TotalAmountCNY { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Enums.SwapStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// 换出账户 ID
        /// </summary>
        public string OutFinanceAccountID { get; set; } = string.Empty;

        /// <summary>
        /// 换人账户 ID
        /// </summary>
        public string InFinanceAccountID { get; set; } = string.Empty;

        /// <summary>
        /// 中间账户 ID
        /// </summary>
        public string MidFinanceAccountID { get; set; } = string.Empty;

        /// <summary>
        /// 手续费
        /// </summary>
        public decimal? Poundage { get; set; }

        public event SuccessHanlder AbandonSuccess;
        //public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        //public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.SwapNotices>(new Layer.Data.Sqls.ScCustoms.SwapNotices()
                    {
                        ID = this.ID,
                        AdminID = this.AdminID,
                        BankName = this.BankName,
                        Currency = this.Currency,
                        TotalAmount = this.TotalAmount,
                        ExchangeRate = this.ExchangeRate,
                        TotalAmountCNY = this.TotalAmountCNY,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        OutFinanceAccountID = this.OutFinanceAccountID,
                        InFinanceAccountID = this.InFinanceAccountID,
                        MidFinanceAccountID = this.MidFinanceAccountID,
                        Poundage = this.Poundage,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(new
                    {
                        AdminID = this.AdminID,
                        BankName = this.BankName,
                        Currency = this.Currency,
                        TotalAmount = this.TotalAmount,
                        ExchangeRate = this.ExchangeRate,
                        TotalAmountCNY = this.TotalAmountCNY,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        OutFinanceAccountID = this.OutFinanceAccountID,
                        InFinanceAccountID = this.InFinanceAccountID,
                        MidFinanceAccountID = this.MidFinanceAccountID,
                        Poundage = this.Poundage,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandon();
        }

        virtual protected void OnAbandon()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }
    }
}
