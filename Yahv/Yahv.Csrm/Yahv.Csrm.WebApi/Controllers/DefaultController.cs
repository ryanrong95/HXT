using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Yahv.Csrm.WebApi.Controllers
{
    /// <summary>
    /// 描述
    /// </summary>

    public class DefaultController : ApiController
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.Route("Default/Get")]
        public string Get()
        {
            return "kong";
        }
    }
}