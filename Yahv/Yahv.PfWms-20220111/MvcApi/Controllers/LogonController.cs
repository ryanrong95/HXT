using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Yahv.Underly.Attributes;

namespace MvcApp.Controllers
{
    public class LogonController : Controller
    {
   

        // GET: Logon
        enum Message
        {
            [Description("登录成功！")]
            Success = 0,
            [Description("登录失败！")]
            Fail = 1,
            [Description("登录失败!")]
            Error
        }

        Message mes;
        object obj = null;
        [HttpPost]
        public ActionResult Index(string username,string password)
        {

            new Admin { UserName = username, Password = password }
            .AddEvent("LogonSuccess", new EventHandler<EventArgs>(Logon_Sucess))
            .AddEvent("LogonFail", new EventHandler<EventArgs>(Logon_Fail))
            .AddEvent("Error",new EventHandler<EventArgs>(Logon_Error))
            .Logon();

            return Json(new { val = (int)mes, msg = mes.GetDescription() ,obj= obj }, JsonRequestBehavior.AllowGet);

        }

        private void Logon_Error(object sender, EventArgs e)
        {
            mes = Message.Error;
        }

        private void Logon_Fail(object sender, EventArgs e)
        {
            mes = Message.Fail;
        }

        private void Logon_Sucess(object sender, EventArgs e)
        {
            mes = Message.Success;
            obj = sender;
        }
    }
}