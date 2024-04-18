using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Payments;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.PvData.WebApi.Controllers
{
    public class ExchangeRatesController : ClientController
    {

        /// <summary>
        /// 汇率计算
        /// </summary>
        /// <param name="from">来源币种</param>
        /// <param name="to">目标币种</param>
        /// <param name="price">价格</param>
        /// <param name="type">汇率类型</param>
        /// <returns></returns>
        public ActionResult Index(Currency from, Currency to, decimal? price, ExchangeType type = ExchangeType.Fixed)
        {
            var rate = ExchangeRates.Current[type][from, to];
            JSingle<object> entity;
            if (price.HasValue)
            {
                entity = new JSingle<object>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        price = (price.Value * rate).ToString("#0.0000000"),
                        priceNum = price * rate,
                        from = from,
                        to = to,
                        rate = rate
                    }
                };
            }
            else
            {
                entity = new JSingle<object>()
                {
                    code = 200,
                    success = true,
                    data = rate
                };
            }

            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 汇率获取
        /// </summary>
        public ActionResult Alls()
        {
            ExchangeType type = ExchangeType.TenAmChineseBank;
            type = ExchangeType.Fixed;

            var rates = ExchangeRates.Current[type].Alls;
            JSingle<object> entity;
            entity = new JSingle<object>()
            {
                code = 200,
                success = true,
                data = rates.Select(price => new
                {
                    from = price.From,
                    sfrom = price.From.GetCurrency().ChineseName,
                    to = price.To,
                    sto = price.To.GetCurrency().ChineseName,
                    rate = price.Value
                })
            };
            return Json(entity, JsonRequestBehavior.AllowGet);
        }


    }
}