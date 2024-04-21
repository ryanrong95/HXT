using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments
{
    /// <summary>
    /// 优惠券
    /// </summary>
    public class CouponManager
    {
        static readonly object locker = new object();
        static CouponManager current;

        public static CouponManager Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        current = new CouponManager();
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="payer">优惠券的授予人ID</param>
        /// <param name="payee">优惠券的使用者ID</param>
        /// <returns></returns>
        public Coupons this[string payer, string payee]
        {
            get
            {
                return new Coupons(payer, payee);
            }
        }
    }
}
