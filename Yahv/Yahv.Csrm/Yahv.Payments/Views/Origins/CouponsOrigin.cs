using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments.Views.Origins
{
    internal class CouponsOrigin : Yahv.Linq.UniqueView<Coupon, PvbCrmReponsitory>
    {
        internal CouponsOrigin()
        {
        }

        internal CouponsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Coupon> GetIQueryable()
        {
            return from coupon in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Coupons>()
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
                       CreatorID = coupon.CreatorID,
                       Status = (Underly.GeneralStatus)coupon.Status
                   };
        }
    }
}
