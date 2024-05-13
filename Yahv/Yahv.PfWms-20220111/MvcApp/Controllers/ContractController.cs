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
    public class ContractController : Controller
    {
        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("合同所有人不能为空,请重新编辑")]
            OwnerIsNull = 2,
            [Description("程序出现异常,请联系管理员")]
            CatchException = 3,
            [Description("合同类型有误")]
            TypeError = 4,
        }
        Message message;
        // GET: FileInfo
        public ActionResult Index(bool? IsCleared,string Code = null, ContractType Type = 0, DateTime? StartTime = null, DateTime? EndTime = null)
        {
            try
            {
                Expression<Func<Contracts, bool>> exp = item => true;
                if (!string.IsNullOrEmpty(Code))
                {
                    exp = PredicateBuilder.And(exp, item => item.Code == Code);
                }
                if (Type > 0)
                {
                    if (!Enum.IsDefined(typeof(ContractType), Type))
                    {
                        return Json(new { obj = (int)Message.TypeError, msg = Message.TypeError.GetDescription() });

                    }
                    exp = PredicateBuilder.And(exp, item => item.Type.Equals(Type));
                }
                if (StartTime != null && EndTime != null)
                {
                    if (StartTime.Value < EndTime.Value)
                    {
                        exp = PredicateBuilder.And(exp, item => item.StartTime <= StartTime);
                        exp = PredicateBuilder.And(exp, item => item.EndTime >= EndTime);
                    }
                    else
                    {
                        exp = PredicateBuilder.And(exp, item => item.StartTime <= EndTime);
                        exp = PredicateBuilder.And(exp, item => item.EndTime >= StartTime);
                    }
                }
                else if (StartTime != null || EndTime != null)
                {
                    if (StartTime == null)
                    {
                        exp = PredicateBuilder.And(exp, item => item.EndTime >= EndTime);
                    }
                    else
                    {
                        exp = PredicateBuilder.And(exp, item => item.StartTime <= StartTime);
                    }
                }
                if (IsCleared.HasValue)
                {
                    exp = PredicateBuilder.And(exp, item => item.IsCleared == IsCleared);
                }
                return Json(new { obj = new Wms.Services.Views.ContractsView().Where(exp) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { obj = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Index(Contracts datas)
        {
            try
            {
                //保证数据的一致性
                if (datas.OwnerID == null)
                {
                    message = Message.OwnerIsNull;
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