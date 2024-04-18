using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    enum Responser
    {
        [Description("成功！")]
        OK = 200
    }

    public class LetsController : Needs.FrontEnd.ClientController
    {
        public ActionResult Index()
        {
            var msg = new Needs.FrontEnd.JsonMessage<Responser>
            {
                Status = Responser.OK
            };

            var model = new { name = "chenhan", age = 5000000 };


            //return eContent("/Feds/Index.shtml", new
            //{
            //    name = "chenhan",
            //    age = 5000000
            //});

            return eContent("/Feds/Index.shtml", msg);
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            var msg = new Needs.FrontEnd.JsonMessage<Responser>
            {
                Status = Responser.OK
            };

            var model = new { name = "chenhan", age = 5000000 };


            return eContent("/Feds/Index.shtml", new
            {
                name = "chenhan",
                age = 5000000
            });

            return eContent("/Feds/Index.shtml", msg);
        }


        public ActionResult Explain()
        {
            return Json(new
            {
                OK = 200
            }, JsonRequestBehavior.AllowGet);
        }
    }
}