using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.PvData.WebApi.Controllers
{
    public class CurrenciesController : ClientController
    {
        /// <summary>
        /// 返回当前可用的全部币种
        /// </summary>
        /// <param name="business">业务来源</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(Businesss business = Businesss.RFQ)
        {
            try
            {
                ConcurrentDictionary<Currency, ICurrency> currencyDic = new ConcurrentDictionary<Currency, ICurrency>();

                foreach (Currency c in Enum.GetValues(typeof(Currency)))
                {
                    if (c == Currency.Unknown)
                        continue;

                    if (business == Businesss.RFQ && c > Currency.HKD)
                        continue;

                    currencyDic.TryAdd(c, c.GetCurrency());
                }

                return Json(new JSingle<object>()
                {
                    code = 200,
                    success = true,
                    data = currencyDic.Select(c => new
                    {
                        ID = c.Key,
                        Name = c.Value.ChineseName,
                        ShortName = c.Value.ShortName,
                        Symbol = c.Value.Symbol
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JSingle<string>() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
    }

    /// <summary>
    /// 业务来源
    /// </summary>
    public enum Businesss
    {
        [Description("询报价")]
        RFQ = 1,

        [Description("其他")]
        Other = 99
    }
}