using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Needs.Utils.Linq;
using Wms.Services.Models;
using Needs.Linq;
using System.Linq.Expressions;
using Wms.Services;

namespace MvcApp.Controllers
{
    public class CashFlowAccountController : Controller
    {
        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("订单编号不可为空,请重新编辑")]
            OrderIsNull = 2,
            [Description("程序出现异常,请联系管理员")]
            CatchException = 3,
            [Description("业务类型有误")]
            BusinessError = 4,
            [Description("记账类型有误")]
            SubjectError = 5,
        }
        Message message;
        // GET: CashFlowAccount
        public ActionResult Index(BusinessTypes BusinessType = 0, Subjects Subject = 0)
        {
            try
            {
                Expression<Func<CashFlowAccounts, bool>> exp = null;
                if (BusinessType > 0)
                {
                    if (!Enum.IsDefined(typeof(BusinessTypes), BusinessType))
                    {
                        return Json(new { obj = (int)Message.BusinessError, msg = Message.BusinessError.GetDescription() });
                    }
                    exp = PredicateBuilder.And(exp, item => item.BusinessType.Equals(BusinessType));
                }
                if (Subject > 0)
                {
                    if (!Enum.IsDefined(typeof(Subjects), Subject))
                    {
                        return Json(new { obj = (int)Message.SubjectError, msg = Message.SubjectError.GetDescription() });
                    }
                    exp = PredicateBuilder.And(exp, item => item.Subject.Equals(Subject));
                }
                return Json(new { obj = new Wms.Services.Views.CashFlowAccountsView().Where(exp) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { obj = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Index(CashFlowAccounts datas)
        {
            try
            {
                //保证数据的一致性
                if (string.IsNullOrWhiteSpace(datas.OrderID))
                {
                    message = Message.OrderIsNull;
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