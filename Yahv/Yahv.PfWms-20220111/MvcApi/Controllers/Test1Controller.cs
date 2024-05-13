using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Wms.Services;
using Wms.Services.Models;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace MvcApi.Controllers
{
    public class Test1Controller : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            Wms.Services.chonggous.Views.LitTools.Current["陈翰"].Log(new
            {
                a = 1,
                b = 2
            }.Json());
            return Content("测试！~");

            if (true)
            {
                int kkk = 1;
                string kkk1 = "123";
                return View("asdfadf", kkk1);
            }
            else
            {
                int kkk = 1;
                string kkk1 = "123";
                return View("vbbr", kkk1);
            }

        }
    }
}