

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.DappApi.Services.Views;

namespace Yahv.PsWms.DappApi.Controllers
{
    [Obsolete]
    /// <summary>
    /// 通知项中型号的对应文件
    /// </summary>    
    public class PartnumberFilesController : FilesController
    {
        /// <summary>
        /// 该ID为 NoticeItemID, 即通知项ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ActionResult Show(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Response.StatusCode = 500;

                return Json(new
                {
                    success = false,
                    data = "参数错误，ID(通知项ID)不能为Null或空字符串",
                }, JsonRequestBehavior.AllowGet);
            }

            using (var view = new PcFilesView())
            {
                var data = view;

                data = data.SearchByItemID(id);

                var result = data.ToMyArray();

                Response.StatusCode = 200;
                return Json(new
                {
                    success = true,
                    data = result,
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}