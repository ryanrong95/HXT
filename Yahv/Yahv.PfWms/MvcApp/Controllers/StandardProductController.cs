using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Needs.Utils.Linq;
using Wms.Services.Models;
using Needs.Linq;

namespace MvcApp.Controllers
{
    public class StandardProductController : Controller
    {
        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("制造商数据不能为空,请重新编辑")]
            ManufacturerIsNull = 2,
            [Description("产品型号数据不能为空,请重新编辑")]
            PartNumberIsNull = 3,
            [Description("产品包装数据不能为空,请重新编辑")]
            PackingIsNull = 4,
            [Description("产品分装数据不能为空,请重新编辑")]
            PackageCaseIsNull = 5,
            [Description("程序出现异常,请联系管理员")]
            CatchException = 6,
        }
        Message message;
        // GET: StandardProduct
        public ActionResult Index()
        {
            try
            {
                return Json(new { obj = new Wms.Services.Views.StandardProductsView().ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { obj = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Index(StandardProducts datas)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(datas.Manufacturer))
                {
                    message = Message.ManufacturerIsNull;
                    return Json(new { obj = (int)message, msg = message.GetDescription() });
                }
                if (string.IsNullOrWhiteSpace(datas.PartNumber))
                {
                    message = Message.PartNumberIsNull;
                    return Json(new { obj = (int)message, msg = message.GetDescription() });
                }
                if (string.IsNullOrWhiteSpace(datas.Packing))
                {
                    message = Message.PackingIsNull;
                    return Json(new { obj = (int)message, msg = message.GetDescription() });
                }
                if (string.IsNullOrWhiteSpace(datas.PackageCase))
                {
                    message = Message.PackageCaseIsNull;
                    return Json(new { obj = (int)message, msg = message.GetDescription() });
                }
                datas.EnterSuccess += Datas_EnterSuccess;
                datas.EnterError += Datas_EnterError;
                datas.Enter();
                return Json(new { obj = (int)message, msg = message.GetDescription() });
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { obj = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
        }

        private void Datas_EnterError(object sender, ErrorEventArgs e)
        {
            message = Message.Fail;
        }

        private void Datas_EnterSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }
    }
}