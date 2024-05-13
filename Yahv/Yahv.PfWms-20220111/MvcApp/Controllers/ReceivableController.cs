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
using Wms.Services;

namespace MvcApp.Controllers
{
    public class ReceivableController : Controller
    {

        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("订单编号不能为空,请重新编辑")]
            OrderIsNull = 2,
            [Description("添加人信息不能为空,请重新编辑")]
            AdminIsNull = 3,
            [Description("程序出现异常,请联系管理员")]
            CatchException = 4,
            [Description("付款类型有误")]
            PayMethodError = 5,
            [Description("业务类型有误")]
            BusinessError = 6,
            [Description("付款类型有误")]
            CatalogError = 7,
            [Description("记账类型有误")]
            SubjectError = 8,
            [Description("记账类型有误")]
            AccountedError = 9,
        }
        Message message;
        // GET: CashFlowAccount
        public ActionResult Index(PayMethodTypes PayMethod = 0, BusinessTypes BusinessType = 0, CatalogTypes Catalog = 0, Subjects Subject = 0, bool IsSpecial = true, AccountedTypes AccountedType = 0)
        {
            try
            {
                Expression<Func<Receivables, bool>> exp = item => true;
                if (PayMethod > 0)
                {
                    if (!Enum.IsDefined(typeof(PayMethodTypes), PayMethod))
                    {
                        return Json(new { obj = (int)Message.PayMethodError, msg = Message.PayMethodError.GetDescription() });
                    }
                    exp = PredicateBuilder.And(exp, item => item.PayMethod.Equals(PayMethod));
                }
                if (BusinessType > 0)
                {
                    if (!Enum.IsDefined(typeof(BusinessTypes), BusinessType))
                    {
                        return Json(new { obj = (int)Message.BusinessError, msg = Message.BusinessError.GetDescription() });
                    }
                    exp = PredicateBuilder.And(exp, item => item.BusinessType.Equals(BusinessType));
                }
                if (Catalog > 0)
                {
                    if (!Enum.IsDefined(typeof(CatalogTypes), Catalog))
                    {
                        return Json(new { obj = (int)Message.CatalogError, msg = Message.CatalogError.GetDescription() });
                    }
                    exp = PredicateBuilder.And(exp, item => item.Catalog.Equals(Catalog));
                }
                if (Subject > 0)
                {
                    if (!Enum.IsDefined(typeof(Subjects), Subject))
                    {
                        return Json(new { obj = (int)Message.SubjectError, msg = Message.SubjectError.GetDescription() });
                    }
                    exp = PredicateBuilder.And(exp, item => item.Subject.Equals(Subject));
                }
                if (IsSpecial)
                {
                    exp = PredicateBuilder.And(exp, item => item.IsSpecial.Equals(IsSpecial));
                }
                if (AccountedType > 0)
                {
                    if (!Enum.IsDefined(typeof(AccountedTypes), AccountedType))
                    {
                        return Json(new { obj = (int)Message.AccountedError, msg = Message.AccountedError.GetDescription() });
                    }
                    exp = PredicateBuilder.And(exp, item => item.AccountedType.Equals(AccountedType));
                }
                return Json(new { obj = new Wms.Services.Views.ReceivablesView().Where(exp) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { obj = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Index(Receivables datas)
        {
            try
            {
                //保证数据的一致性
                if (string.IsNullOrWhiteSpace(datas.OrderID))
                {
                    message = Message.OrderIsNull;
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