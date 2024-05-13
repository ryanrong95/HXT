using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Needs.Utils.Linq;
using Wms.Services.Models;
using Needs.Linq;
using System.Linq.Expressions;
using Wms.Services.Enums;

namespace MvcApp.Controllers
{
    public class PaymentController : Controller
    {

        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("应付款项编号不能为空,请重新编辑")]
            PayableIsNull = 2,
            [Description("添加人信息不能为空,请重新编辑")]
            AdminIsNull = 3,
            [Description("程序出现异常,请联系管理员")]
            CatchException = 4,
            [Description("账户类型有误")]
            AcountError = 5,
        }
        Message message;
        // GET: CashFlowAccount
        public ActionResult Index(AcountTypes AcountType=0)
        {
            try
            {
                Expression<Func<Payments, bool>> exp = item => true;
                if (AcountType > 0)
                {
                    if (!Enum.IsDefined(typeof(AcountTypes), AcountType))
                    {
                        return Json(new { obj = (int)Message.AcountError, msg = Message.AcountError.GetDescription() });
                    }
                    exp = PredicateBuilder.And(exp, item => item.AcountType.Equals(AcountType));
                }
                return Json(new { obj = new Wms.Services.Views.PaymentsView().ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { obj = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Index(Payments datas)
        {
            try
            {
                //保证数据的一致性
                if (string.IsNullOrWhiteSpace(datas.PayableID))
                {
                    message = Message.PayableIsNull;
                    return Json(new { obj = (int)message, msg = message.GetDescription() });
                }
                if (string.IsNullOrWhiteSpace(datas.AdminID))
                {
                    message = Message.AdminIsNull;
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