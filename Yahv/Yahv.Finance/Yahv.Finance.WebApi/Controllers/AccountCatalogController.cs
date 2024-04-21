using System.Web.Mvc;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Web.Mvc;

namespace Yahv.Finance.WebApi.Controllers
{
    /// <summary>
    /// 账款分类
    /// </summary>
    public class AccountCatalogController : ClientController
    {
        /// <summary>
        /// 获取收款供应链业务分类
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetReceipt(string callback = "")
        {
            var result = new JSingle<object>() { code = 200, success = true };
            using (var view = new AccountCatalogsRoll())
            {
                result.data = AccountCatalogsAlls.Current.ToObject(AccountCatalogType.Input.GetDescription());
            }
            return Jsonp(result, callback);
        }

        /// <summary>
        /// 获取付款供应链业务分类
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPay(string callback = "")
        {
            var result = new JSingle<object>() { code = 200, success = true };
            using (var view = new AccountCatalogsRoll())
            {
                result.data = AccountCatalogsAlls.Current.ToObject(AccountCatalogType.Output.GetDescription());
            }
            return Jsonp(result, callback);
        }
    }
}