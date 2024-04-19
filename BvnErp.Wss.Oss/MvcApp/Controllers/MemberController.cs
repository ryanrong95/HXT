using NtErp.Wss.Oss.Services;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    /// <summary>
    /// 用户个人中心首页信息
    /// </summary>
    public class UserIndex
    {
        public string UserName { get; set; }

        public Account[] Cash { get; set; }
        public Account[] Credit { get; set; }
        public int PayingCount { get; set; }
        public int CompletedCount { get; set; }
    }
    public class MemberController : UserController
    {
        NtErp.Wss.Oss.Services.Models.ClientTop GetClient()
        {
            using (var view = new NtErp.Wss.Oss.Services.Views.ClientsTopView())
            {
                return view[this.Client.ID];
            }
        }
        // GET: Member
        /// <summary>
        /// 会员信息
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberInfor()
        {
            var Infor = new UserIndex
            {
                UserName= this.GetClient().UserName,
               Cash= this.GetClient().GetBalances(UserAccountType.Cash),
               Credit=this.GetClient().GetBalances(UserAccountType.Credit),
               PayingCount= this.GetClient().GetPayingCount(),
               CompletedCount= this.GetClient().GetCompletedCount()
            };
            return Json(Infor, JsonRequestBehavior.AllowGet);
        }
    }
}