using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class CouponStatistic
    {
        /// <summary>
        /// 优惠券ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 优惠券编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 优惠券类型
        /// </summary>
        public CouponType Type { get; set; }

        /// <summary>
        /// 业务
        /// </summary>
        public string Conduct { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 定额优惠券的抵扣金额
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 一个订单最多可以使用该优惠券的数量
        /// </summary>
        public int? InOrderCount { get; set; }

        /// <summary>
        /// 优惠券的创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 优惠券的授予人ID，这里指我们的内部公司(创新恒远、芯达通、科睿等)
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 优惠券的使用者ID，即我们的客户
        /// </summary>
        public string Payee { get; set; }

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
    }
}
