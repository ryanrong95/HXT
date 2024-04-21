using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.Erm.WebApi.Controllers
{
    /// <summary>
    /// 审批流
    /// </summary>
    public class VoteFlowsController : ClientController
    {
        /// <summary>
        /// 初始化审批流基础数据
        /// </summary>
        /// <param name="id">审批流id</param>
        /// <returns></returns>
        [HttpPost]
        public string Init()
        {
            string id = Request.Form["id"];
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                Services.VoteFlowManager.Current.Init();
            });

            return "ok";
        }
    }
}