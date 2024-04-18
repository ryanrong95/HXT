using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 订单应收
    /// </summary>
    public class OrderReceivables : ModelBase<Layer.Data.Sqls.ScCustoms.OrderReceipts, ScCustomsReponsitory>, IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 属性

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 费用类型
        /// 科目
        /// </summary>
        public Needs.Wl.Models.Enums.OrderFeeType FeeType { get; set; }

        /// <summary>
        /// 费用名称
        /// </summary>
        public string FeeName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 收款人/跟单员
        /// </summary>
        public Needs.Wl.Models.Admin Admin { get; set; }

        #endregion

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public OrderReceivables()
        {
            this.Status = (int)Needs.Wl.Models.Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public override void Enter()
        {

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
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
        public override void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(new { Status = Needs.Wl.Models.Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }
    }
}