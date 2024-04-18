using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{   
    /// <summary>
    /// 应付费用接口
    /// </summary>
    public class PayerChargesController : ChargesController
    {
        public override ActionResult Options(string orderID)
        {
            throw new NotImplementedException();
        }
    }
}