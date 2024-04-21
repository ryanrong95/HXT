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
    public class SortingsController : Controller
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
            CatchException = 3
        }
        #endregion

        #region 私有变量
        Message message;
        #endregion

        #region 接口方法
        // GET: SortingsByID
        public ActionResult Index(string id = null, string boxingcode = null, DateTime? beginDate = null, DateTime? endDate = null, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
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

                Expression<Func<Sortings, bool>> exp = item => true;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    exp.And(item => item.ID == id);
                }

                if (!string.IsNullOrWhiteSpace(boxingcode))
                {
                    exp.And(item => item.BoxingCode.ToUpper().Contains(boxingcode.ToUpper()));
                }

                if (beginDate != null)
                {
                    exp.And(item => item.CreateDate >= beginDate.Value);
                }

                if (endDate != null)
                {
                    exp.And(item => item.CreateDate <= endDate.Value);
                }

                return Json(new { obj = new SortingsView().Where(exp) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { val = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Index(Sortings datas)
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
                            return Json(new { val = (int)Message.DataWrong, msg = modelstate.Errors.FirstOrDefault().ErrorMessage });
                        }
                    }
                }

                datas.SortingSuccess += Datas_SortingSuccess;
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
        private void Datas_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            message = Message.EditWrong;
        }

        private void Datas_SortingSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            message = Message.Success;
        }
        #endregion
    }
}