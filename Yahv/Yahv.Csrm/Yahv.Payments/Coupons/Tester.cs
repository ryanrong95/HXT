using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Serializers;

namespace Yahv.Payments
{
    class Tester
    {
        public Tester()
        {
            //优惠券可用数量
            var balance = CouponManager.Current["payer", "payee"]["couponID"].Balance;
            Console.WriteLine(balance);

            //分配优惠券
            CouponManager.Current["payer", "payee"]["couponID"].Grant(1, "", "admin001");
            CouponManager.Current["payer", "payee"]["conduct", "couponName"].Grant(1, "", "admin001");

            //CouponManager.Current["payer", "payee"]["couponID"].For("receivableID").Cost(2, adminID: "");
            //CouponManager.Current["payer", "payee"]["couponID"].For("receivableID").Cost(2, userID: "");
            //CouponManager.Current["payer", "payee"]["conduct", "couponName"].For("receivableID").Cost(2, adminID: "");
            //CouponManager.Current["payer", "payee"]["conduct", "couponName"].For(new Receivable()).Cost(2, userID: "");

            //会员中心确认账单，使用优惠券抵扣应收时调用
            CouponManager.Current["payer", "payee"].Pay("UserID", new UsedMap[5]);

            //管理端确认账单，使用优惠券抵扣应收时调用
            CouponManager.Current["payer", "payee"].Confirm("AdminID", new UsedMap[5]);

            //实现目标
            //CouponManager.Current["payer", "payee"].For(new Receivable(), new Receivable(), new Receivable()).Cost(2, userID: "");

            //平台公司分配给客户的优惠券
            CouponManager.Current["payer", "payee"].ToArray();
            CouponManager.Current["payer", "payee"].Select(item => new
            {
                item.ID, // 主键
                item.Name, // 优惠券名称
                item.Code, // 优惠券编码
                item.Type, // 优惠券类型 ： 定额、据实
                item.Conduct, // 业务
                item.Catalog, // 分类
                item.Subject, // 科目
                item.Currency, // 币种
                item.Price, // 价格：定额优惠券抵扣的金额
                item.InOrderCount, // 
                item.Input, // 收
                item.Output, // 支
                item.Balance // 剩余数量/可用余额
            });
            CouponManager.Current["payer", "payee"].Select(item => new
            {
                value = item.ID,
                text = item.Name,
            });
        }
    }
}
