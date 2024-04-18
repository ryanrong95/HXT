using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 付款申请
    /// </summary>
    public class PaymentNoticeItem : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region

        public string ID { get; set; }

        public string PaymentNoticeID { get; set; }

        public string OrderID { get; set; }

        public Enums.FinanceFeeType PayFeeType { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate
        {
            get
            {
                using (var view = new Views.PaymentNoticesView())
                {
                    return view.FirstOrDefault(item => item.ID == this.PaymentNoticeID).ExchangeRate.GetValueOrDefault();
                }
            }
        }

        public Enums.PaymentNoticeStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        public PaymentNoticeItem()
        {
            this.Status = Enums.PaymentNoticeStatus.UnPay;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PaymentNoticeItem);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        this.UpdateDate = DateTime.Now;
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }
        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            ApplyStatus = Enums.PaymentNoticeStatus.Canceled,
                        }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess(); ;
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
            }
        }
        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }

    public class PaymentNoticeItems : BaseItems<PaymentNoticeItem>
    {
        internal PaymentNoticeItems(IEnumerable<PaymentNoticeItem> enums) : base(enums)
        {
        }

        internal PaymentNoticeItems(IEnumerable<PaymentNoticeItem> enums, Action<PaymentNoticeItem> action) : base(enums, action)
        {
        }

        public override void Add(PaymentNoticeItem item)
        {
            base.Add(item);
        }
    }
}
