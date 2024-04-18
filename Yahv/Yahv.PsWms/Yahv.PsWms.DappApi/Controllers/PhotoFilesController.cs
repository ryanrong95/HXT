using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.DappApi.Services.Views;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    /// <summary>
    /// 通知中的拍照文件， 仅包含类型为: 外观，单据的文件
    /// </summary>
    public class PhotoFilesController : FilesController
    {
        /// <summary>
        /// Get方式请求获取通知下面的所有外观图片，以及单据图片
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
                    data = "参数错误, ID(通知ID)不能为Null或空字符串",
                }, JsonRequestBehavior.AllowGet);
            }

            using (var view = new PcFilesView())
            {
                var data = view;

                data = data.SearchByID(id);

                Services.Enums.FileType[] types = new Services.Enums.FileType[] { Services.Enums.FileType.Exterior, Services.Enums.FileType.Form, Services.Enums.FileType.DriverSign, Services.Enums.FileType.CustomSign };

                data = data.SearchByType(types);

                var result = data.ToMyArray();

                Response.StatusCode = 200;

                return Json(new
                {
                    success = true,
                    data = result,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 利用Jpost方式，根据Type来返回对应的外观照片或者单据照片
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public override ActionResult Show(JPost jpost)
        {
            var arguments = new
            {
                id = jpost["ID"]?.Value<string>(),
                type = jpost["Type"]?.Value<int?>(),
            };

            if (string.IsNullOrEmpty(arguments.id))
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    data = "参数错误, ID(通知ID)不能为Null或空字符串",
                }, JsonRequestBehavior.DenyGet);
            }

            using (var view = new PcFilesView())
            {
                var data = view;
                data = data.SearchByID(arguments.id.Trim());

                if (arguments.type.HasValue)
                {
                    if (arguments.type.Value == 1)
                    {
                        data = data.SearchByType( new Services.Enums.FileType[] { Services.Enums.FileType.Exterior });
                    }

                    if (arguments.type.Value == 2)
                    {
                        data = data.SearchByType(new Services.Enums.FileType[] { Services.Enums.FileType.Form });
                    }

                    if (arguments.type.Value == 8)
                    {
                        data = data.SearchByType(new Services.Enums.FileType[] { Services.Enums.FileType.DriverSign });
                    }

                    if (arguments.type.Value == 9)
                    {
                        data = data.SearchByType(new Services.Enums.FileType[] { Services.Enums.FileType.CustomSign });
                    }
                    if (arguments.type.Value == 10)
                    {
                        data = data.SearchByType(new Services.Enums.FileType[] { Services.Enums.FileType.TrackingCode });
                    }                    
                }
                else
                {
                    Services.Enums.FileType[] types = new Services.Enums.FileType[] { Services.Enums.FileType.Exterior, Services.Enums.FileType.Form, Services.Enums.FileType.DriverSign, Services.Enums.FileType.CustomSign, Services.Enums.FileType.TrackingCode };
                    data = data.SearchByType(types);
                }                

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