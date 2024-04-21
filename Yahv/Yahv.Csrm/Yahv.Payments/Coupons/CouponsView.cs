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
    /// 优惠券 管理视图
    /// </summary>
    public class CouponsView : Yahv.Linq.UniqueView<Coupon, PvbCrmReponsitory>
    {
        public CouponsView()
        {
        }

        internal CouponsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Coupon> GetIQueryable()
        {
            var adminsView = new Services.Views.AdminsAll<PvbCrmReponsitory>(this.Reponsitory);
            return from coupon in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Coupons>()
                   join creator in adminsView on coupon.CreatorID equals creator.ID
                   select new Coupon
                   {
                       ID = coupon.ID,
                       Name = coupon.Name,
                       Code = coupon.Code,
                       Type = (CouponType)coupon.Type,
                       Conduct = coupon.Conduct,
                       Catalog = coupon.Catalog,
                       Subject = coupon.Subject,
                       Currency = (Underly.Currency)coupon.Currency,
                       Price = coupon.Price,
                       InOrderCount = coupon.InOrderCount,
                       CreateDate = coupon.CreateDate,
                       CreatorID = creator.ID,
                       Creator = creator,
                       Status = (Underly.GeneralStatus)coupon.Status
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="conduct">业务</param>
        /// <param name="catalog">分类</param>
        /// <param name="subject">科目</param>
        /// <returns></returns>
        internal Coupon this[string conduct, string catalog, string subject]
        {
            get
            {
                return this.FirstOrDefault(c => c.Conduct == conduct && c.Catalog == catalog && c.Subject == subject);
            }
        }

        /// <summary>
        /// 保存一个优惠券
        /// </summary>
        /// <param name="o"></param>
        /// <remarks>
        /// 通过一个后台完成优惠券的制作
        /// </remarks>
        public void Enter(object o)
        {

        }

    }
}
