using Needs.Utils.Descriptions;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Needs.Utils.Linq;
using Wms.Services.Models;
using Needs.Linq;
using System.Linq.Expressions;
using System.IO;
using Microsoft.Office.Interop.Word;
using System.Diagnostics;
using Wms.Services.Enums;
using System.Configuration;

namespace MvcApp.Controllers
{
    public class FileInfoController : Controller
    {
        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("运单编号不能为空,请重新编辑")]
            WaybillIsNull = 2,
            [Description("库存编号不能为空,请重新编辑")]
            StorageIsNull = 3,
            [Description("程序出现异常,请联系管理员")]
            CatchException = 4,
            [Description("文件类型有误")]
            TypeError = 5,
            [Description("状态类型有误")]
            StatusError = 6,
        }
        Message message;
        // GET: FileInfo
        public ActionResult Index(string CustomName = null, FileTypes Type = 0, FileTypesStatus Status=0)
        {
            try
            {
                Expression<Func<FileInfos, bool>> exp = item => true;
                if (!string.IsNullOrWhiteSpace(CustomName))
                {
                    exp = PredicateBuilder.And(exp, item => item.CustomName.Contains(CustomName));
                }
                if (Type > 0)
                {
                    if (!Enum.IsDefined(typeof(FileTypes), Type))
                    {
                        return Json(new { obj = (int)Message.TypeError, msg = Message.TypeError.GetDescription() });
                    }
                    exp = PredicateBuilder.And(exp, item => item.Type.Equals(Type));
                }
                if (Status>0)
                {
                    if (!Enum.IsDefined(typeof(FileTypesStatus), Status))
                    {
                        return Json(new { obj = (int)Message.StatusError, msg = Message.StatusError.GetDescription() });
                    }
                    exp = PredicateBuilder.And(exp, item => item.Status.Equals(Status));
                }
                return Json(new { obj = new Wms.Services.Views.FileInfosView().Where(exp) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = Message.CatchException;
                return Json(new { obj = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Index(FileInfos datas)
        {
            try
            {
                //保证数据的一致性
                if (string.IsNullOrWhiteSpace(datas.WaybillID) )
                {
                    message = Message.WaybillIsNull;
                    return Json(new { obj = (int)message, msg = message.GetDescription() });
                }
                if (string.IsNullOrWhiteSpace(datas.StorageID))
                {
                    message = Message.StorageIsNull;
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

        #region 事件方法
        private void Datas_EnterSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }
        private void Datas_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            message = Message.Fail;
        }
        #endregion
    }
}