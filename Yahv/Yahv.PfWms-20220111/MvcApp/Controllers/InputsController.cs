using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Wms.Services.Views;

namespace MvcApp.Controllers
{
    public class InputsController : Controller
    {
        #region 自定义枚举
        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("传入数据有误")]
            DataWrong = 1,
            [Description("编辑数据出错")]
            EditWrong = 2,
            [Description("系统出现异常")]
            CatchException = 3,
            [Description("数据为空")]
            DataIsNull=4
        }
        #endregion

        #region 私有变量
        Message message;
        #endregion

        #region 公共接口
        // GET: Inputs
        public ActionResult Index(string id = null, string code = null, string supplier = null, string original = null, DateTime? beginDate = null, DateTime? endDate = null, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                Expression<Func<Inputs, bool>> exp = item => true;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    exp.And(item => item.ID == id);
                    return Json(new { data = new InputsView().Where(exp) });
                }

                if (!string.IsNullOrWhiteSpace(code))
                {
                    exp.And(item => item.Code.ToUpper().Contains(code.ToUpper()));
                }

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    exp.And(item => item.Supplier.ToUpper().Contains(supplier.ToUpper()));
                }

                if (!string.IsNullOrWhiteSpace(original))
                {
                    exp.And(item => item.Origin.ToUpper().Contains(original.ToUpper()));
                }

                //确保beginDate小于endDate
                if (beginDate != null && endDate != null)
                {
                    if (beginDate.Value < endDate.Value)
                    {
                        DateTime? dt = beginDate;
                        beginDate = endDate;
                        endDate = dt;
                    }
                }

                if (beginDate != null)
                {
                    exp.And(item => item.CreateDate >= beginDate.Value);
                }

                if (endDate != null)
                {
                    exp.And(item => item.CreateDate <= endDate.Value);
                }

                var data = new InputsView();
                if (data != null)
                {
                    return Json(new { obj = data.Where(exp) }, JsonRequestBehavior.AllowGet);
                }

                message = Message.DataIsNull;
                return Json(new { val = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { val = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Index(Inputs datas)
        {
            try
            {
                //保证数据正确及完整性
                if (!ModelState.IsValid)
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var modelstate = ModelState[key];
                        if (modelstate.Errors.Any())
                        {
                            return Json(new {val=(int)Message.DataWrong, msg = modelstate.Errors.FirstOrDefault().ErrorMessage });
                        }
                    }
                }

                datas.InputSuccess += Datas_InputSuccess;
                datas.EnterError += Datas_EnterError;
                datas.Enter();
                return Json(new { val = (int)message, msg = message.GetDescription() });
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { val = (int)message, msg = message.GetDescription() });
            }
        }
        #endregion

        #region 私有方法
        private void Datas_EnterError(object sender, ErrorEventArgs e)
        {
            message = Message.EditWrong;
        }

        private void Datas_InputSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }
        #endregion
    }
}