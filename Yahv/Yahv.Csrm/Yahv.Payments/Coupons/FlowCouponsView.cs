using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 优惠券流水视图
    /// </summary>
    public class FlowCouponsView : Yahv.Linq.UniqueView<FlowCoupon, PvbCrmReponsitory>
    {
        /// <summary>
        /// 优惠券的授予人
        /// </summary>
        /// <remarks>
        /// 应该是实实在在的企业信息对象
        /// </remarks>
        string payer;

        /// <summary>
        /// 优惠券的使用者
        /// </summary>
        string payee;

        public FlowCouponsView(string payer, string payee)
        {
            this.payee = payee;
            this.payer = payer;
        }

        internal FlowCouponsView(string payer, string payee, PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
            this.payee = payee;
            this.payer = payer;
        }

        protected override IQueryable<FlowCoupon> GetIQueryable()
        {
            return from flow in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.FlowCoupons>()
                   where flow.Payer == this.payer && flow.Payee == this.payee
                   select new FlowCoupon
                   {
                       ID = flow.ID,
                       Payer = flow.Payer,
                       Payee = flow.Payee,
                       CouponID = flow.CouponID,
                       Input = flow.Input,
                       Output = flow.Output,
                       Balance = flow.Balance,
                       CreateDate = flow.CreateDate,
                       AdminID = flow.AdminID,
                       UserID = flow.UserID,
                       Summary = flow.Summary
                   };
        }
    }
}
