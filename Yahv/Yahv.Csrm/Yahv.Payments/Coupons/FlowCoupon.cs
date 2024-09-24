using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments
{
    /// <summary>
    /// 优惠券流水
    /// </summary>
    public class FlowCoupon : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 优惠券的授予人ID，这里指我们的内部公司(创新恒远、华芯通、科睿等)
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 优惠券的使用者ID，即我们的客户
        /// </summary>
        public string Payee { get; set; }

        /// <summary>
        /// 优惠券ID
        /// </summary>
        public string CouponID { get; set; }

        /// <summary>
        /// 收：我们分配给客户该优惠券的数量
        /// </summary>
        public int Input { get; set; }

        /// <summary>
        /// 支：客户使用该优惠券的数量
        /// </summary>
        public int Output { get; set; }

        /// <summary>
        /// 余：收- 支
        /// </summary>
        public int Balance { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 管理员
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 会员
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 摘要备注，主要是管理员分配优惠券时使用
        /// </summary>
        public string Summary { get; set; }

        #endregion

        internal FlowCoupon()
        {
            this.CreateDate = DateTime.Now;
        }

        #region 持久化

        public void Enter()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
