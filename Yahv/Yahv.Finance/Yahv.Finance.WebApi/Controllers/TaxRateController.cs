using System;
using System.Web.Mvc;
using Yahv.Finance.Services.Views;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.Finance.WebApi.Controllers
{
    /// <summary>
    /// 税率接口
    /// </summary>
    public class TaxRateController : ClientController
    {
        /// <summary>
        /// 获取税率
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTaxRates(string callback)
        {
            try
            {
                var json = new JSingle()
                {
                    data = TaxRatesAlls.Current.Json(),
                    success = true,
                    code = 200
                };

                return Jsonp(json, callback);
            }
            catch (Exception ex)
            {
                return Jsonp(new JMessage() { code = 500, success = false, data = ex.ToString() }, callback);
            }
        }
    }
}