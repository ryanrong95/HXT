using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.Finance.WebApi.Controllers
{
    /// <summary>
    /// 基础字典
    /// </summary>
    public class BasicInfoController : ClientController
    {
        /// <summary>
        /// 收款方式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetReceiptMethord(string callback = "")
        {
            var result = new JList<dynamic>()
            {
                code = 200,
                success = true,
                data = ExtendsEnum.ToDictionary<PaymentMethord>()
                    .Select(item => new { ID = item.Key, Name = item.Value }).ToArray()
            };
            return Jsonp(result, callback);
        }
    }
}