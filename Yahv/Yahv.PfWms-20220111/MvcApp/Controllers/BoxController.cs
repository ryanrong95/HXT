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
using WinApp;

namespace MvcApp.Controllers
{
    public class BoxController : Controller
    {
        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("你还没有设置当前库房")]
            WarehouseIsNull = 2,
            [Description("程序出现异常,请联系管理员")]
            CatchException = 3,
            [Description("状态类型有误")]
            StatusError = 4,
            [Description("所选日期不得小于当前日期")]
            LessDate = 4,
        }
        Message message;
        // GET: Box
        public ActionResult Index(string AdminID = null,string Code=null, BoxesStatus Status=0)
        {
            try
            {
                Expression<Func<Boxes, bool>> exp = item => item.Status != Wms.Services.Enums.BoxesStatus.Deleted && item.Status != Wms.Services.Enums.BoxesStatus.StopUsing;
                if (!string.IsNullOrWhiteSpace(AdminID))
                {
                    exp = PredicateBuilder.And(exp, item => item.AdminID == AdminID);
                }
                if (!string.IsNullOrWhiteSpace(Code))
                {
                    exp = PredicateBuilder.And(exp, item => item.Code.Contains(Code));
                }
                if (Status>0)
                {
                    if (!Enum.IsDefined(typeof(BoxesStatus), Status))
                    {
                        return Json(new { obj = (int)Message.StatusError, msg = Message.StatusError.GetDescription() });

                    }
                    exp = PredicateBuilder.And(exp, item => item.Status.Equals(Status));
                }
                return Json(new { obj = new Wms.Services.Views.BoxesView().Where(exp) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { obj = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Index(Boxes datas)
        {
            try
            {
                //保证数据的一致性
                if (!string.IsNullOrWhiteSpace(WareHouseManager.WareHouseID))
                {
                    message = Message.WarehouseIsNull;
                    return Json(new { obj = (int)message, msg = message.GetDescription() });
                }
                if (datas.AssignDate<DateTime.Now)
                {
                    message = Message.LessDate;
                    return Json(new { obj = (int)message, msg = message.GetDescription() });
                }
                datas.BoxesSuccess += Datas_BoxesSuccess;
                datas.EnterError+= Datas_EnterError;
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

        private void Datas_BoxesSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }
    }
}