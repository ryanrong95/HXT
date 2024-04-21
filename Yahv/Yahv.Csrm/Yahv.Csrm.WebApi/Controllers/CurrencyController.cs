using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class CurrencyController : ClientController
    {
        // GET: Currency
        [HttpGet]
        public ActionResult Index(string callback)
        {
            var currency = ExtendsEnum.ToArray<Currency>().Where(item => item != Currency.Unknown).Select(item => new
            {
                ID = item,
                Name = item.GetDescription(),
                ShortName = item.GetCurrency().ShortName,
                Symbol = item.GetCurrency().Symbol,
            });
            return this.Jsonp(currency, callback);

        }


    }
}