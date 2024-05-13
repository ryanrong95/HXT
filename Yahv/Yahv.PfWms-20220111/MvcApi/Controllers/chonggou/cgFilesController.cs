using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous;
using Yahv.Web.Mvc;

namespace MvcApi.Controllers.chonggou
{
    public class cgFilesController : Controller
    {
        /// <summary>
        /// 删除文件单数
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(JPost jpost)
        {
            var id = jpost["id"].Value<string>();
            FilesManage file = new FilesManage();
            file.DeleteFile(id);

            return Json(new
            {
                Success = true,
                Data = string.Empty
            });

            //FilesManage file = new FilesManage();
            //var data = file.DeleteFile(id);

            //return Json(data);

        }

        ///// <summary>
        ///// 删除文件复数
        ///// </summary>
        ///// <param name="fileIDs"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult DeleteFiles(string[] fileIDs)
        //{
        //    FilesManage file = new FilesManage();
        //    file.DeleteFiles(fileIDs);

        //    return Json(new
        //    {
        //        Success = true,
        //        Data = string.Empty
        //    });

        //    //FilesManage file = new FilesManage();
        //    //var data = file.DeleteFile(id);

        //    //return Json(data);

        //}
    }
}