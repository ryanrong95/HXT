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
    /// 通知中的文件， 不包含类型为: 外观，单据的文件
    /// </summary>
    public class NoticeFilesController : FilesController
    {        
        public override ActionResult Show(string id)
        {
            //可以根据 NoticeID or FormID
            //或是 [noticeItemID or FormItemID,] -- 这个没有用的忽略


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

                Services.Enums.FileType[] types = new Services.Enums.FileType[] { Services.Enums.FileType.Label, Services.Enums.FileType.InDelivery, Services.Enums.FileType.OutDelivery, Services.Enums.FileType.Bill, Services.Enums.FileType.Taking, Services.Enums.FileType.DriverSign, Services.Enums.FileType.CustomSign}; 

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
    }
}