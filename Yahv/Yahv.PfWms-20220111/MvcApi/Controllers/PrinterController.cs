using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Services.Models;
using Yahv.Services.Views;

namespace MvcApi.Controllers
{
    public class PrinterController : Controller
    {
        // GET: Printer
        [HttpPost]
        public ActionResult Print(FaceOrder entity)
        {
            using (FaceOrdersTopView orderView = new FaceOrdersTopView())
            {
                orderView.Enter(entity);
            }

            return Json(new
            {
                Success = true,
                Message = "成功录入",
            });
        }

        /// <summary>
        /// 获取面单数据
        /// </summary>
        /// <returns>面单数据</returns>
        [HttpGet]
        public ActionResult GetFaceSheet()
        {
            try
            {

                string code = Request.QueryString["code"];

                using (var faceOrderView = new FaceOrdersTopView())
                {

                    var linq = from faceOrder in faceOrderView
                               where faceOrder.Code == code
                               select new
                               {
                                   ID = faceOrder.ID,
                                   SendJson = faceOrder.SendJson,
                                   Html = faceOrder.Html,
                                   Source = (int)faceOrder.Source
                               };

                    var entity = linq.SingleOrDefault();
                    return Json(entity, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw;
            }


        }
    }
}