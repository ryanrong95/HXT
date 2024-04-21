using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    public class MyCoupon
    {
        /// <summary>
        /// 唯一码
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
        /// 优惠券类型：定额、据实
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
        /// 币种，默认CNY
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 价值，定额优惠券的抵扣金额
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 一个订单最多可以使用该优惠券的数量
        /// </summary>
        public int? InOrderCount { get; set; }

        /// <summary>
        /// 收
        /// </summary>
        public int Input { get; set; }

        /// <summary>
        /// 支
        /// </summary>
        public int Output { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        public int Balance { get; set; }
    }

    public class MyCouponsView : Linq.QueryView<MyCoupon, PvbCrmReponsitory>
    {
        /// <summary>
        /// 优惠券的授予人
        /// </summary>
        /// <remarks>
        /// 应该是实实在在的企业信息对象
        /// </remarks>
        protected string payer;

        /// <summary>
        /// 优惠券的使用者
        /// </summary>
        protected string payee;

        public MyCouponsView(string payer, string payee)
        {
            this.payee = payee;
            this.payer = payer;

            //名称需要从数据库中获取一下，给客户返回
        }

        protected override IQueryable<MyCoupon> GetIQueryable()
        {
            /*
            var linq = from flow in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.FlowCoupons>()
                       where flow.Payer == this.payer && flow.Payee == this.payee
                       group flow by flow.CouponID into groups
                       select new
                       {
                           CouponID = groups.Key,
                           Input = groups.Sum(item => item.Input),
                           Output = groups.Sum(item => item.Output)
                       };

            return from item in linq
                   join coupon in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Coupons>() on item.CouponID equals coupon.ID
                   select new MyCoupon
                   {
                       ID = coupon.ID,
                       Name = coupon.Name,
                       Code = coupon.Code,
                       Type = (CouponType)coupon.Type,
                       Conduct = coupon.Conduct,
                       Catalog = coupon.Catalog,
                       Subject = coupon.Subject,
                       Currency = (Currency)coupon.Currency,
                       Price = coupon.Price,
                       InOrderCount = coupon.InOrderCount,
                       Input = item.Input,
                       Output = item.Output,
                       Balance = item.Input - item.Output,
                   };
            */

            var linq = new Yahv.Services.Views.CouponStatisticsTopView<PvbCrmReponsitory>(this.Reponsitory);

            return from entity in linq
                   where entity.Payer == this.payer && entity.Payee == this.payee
                   select new MyCoupon
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Code = entity.Code,
                       Type = entity.Type,
                       Conduct = entity.Conduct,
                       Catalog = entity.Catalog,
                       Subject = entity.Subject,
                       Currency = entity.Currency,
                       Price = entity.Price,
                       InOrderCount = entity.InOrderCount,
                       Input = entity.Input,
                       Output = entity.Output,
                       Balance = entity.Input - entity.Output,
                   };
        }
    }
}
