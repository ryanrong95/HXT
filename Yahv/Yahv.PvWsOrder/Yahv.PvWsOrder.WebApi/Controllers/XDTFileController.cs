using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PvWsOrder.WebApi.Models;
using Yahv.Underly;
using Layers.Data.Sqls;
using Yahv.Utils.Serializers;
using Newtonsoft.Json.Linq;
using Yahv.Web.Mvc;

namespace Yahv.PvWsOrder.WebApi.Controllers
{
    public class XDTFileController : Controller
    {
        /// <summary>
        /// 客户确认芯达通对接接口
        /// </summary>
        /// <param name="confirm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FileApprove(JPost obj)
        {
            using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
            {
                try
                {
                    var File = obj.ToObject<XDTFile>();
                    FileType filetype = (FileType)File.Type;

                    //文件改为审批通过
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                    {
                        Status = 300,
                    }, item => item.WsOrderID == File.OrderID && item.Type == (int)filetype);

                    //返回归类信息
                    var json = new JMessage() { code = 200, success = true, data = "提交成功" };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    //错误日志记录
                    Yahv.PvWsOrder.Services.Logger.Error("NPC", ex.Message);
                    var json = new JMessage() { code = 300, success = false, data = ex.Message };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}
