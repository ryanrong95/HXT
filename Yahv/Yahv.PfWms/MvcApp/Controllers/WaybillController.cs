using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.EventExtend;
using Needs.Utils.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;

namespace MvcApp.Controllers
{
    public class WaybillController : Controller
    {

        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail= 1,
        }

        Message message;

        // GET: Waybill
        public ActionResult Index(string id=null)
        {
            Expression<Func<Waybills, bool>> exp = item => item.Status != Wms.Services.Enums.WaybillsStatus.Deleted;

            if (!string.IsNullOrWhiteSpace(id))
            {
                exp = PredicateBuilder.And(exp, item => item.ID == id);
            }

            return Json(new { obj = new Wms.Services.Views.WaybillsView().Where(exp) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        /// <summary>
        /// 运单添加与修改(id有值是修改，code有值是添加（根据类型判断）)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Waybills datas)
        {
            try
            {
                datas.AddEvent("WaybillSuccess", new SuccessHandler(Datas_WaybillSuccess))
                              .AddEvent("WaybillFailed", new ErrorHandler(Datas_WaybillFailed))
                              .Enter();
                return Json(new { val = (int)message, msg = message.GetDescription() });
            }
            catch 
            {
                return Json(new { val = (int)Message.Fail, msg = Message.Fail.GetDescription() });
            }
        }

        private void Datas_WaybillFailed(object sender, ErrorEventArgs e)
        {
            message = Message.Fail;
        }

        private void Datas_WaybillSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }
    }
}