using NtErp.Wss.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Needs.Web;
using NtErp.Wss.Services.Models;

namespace MvcApp.Controllers
{
    public class ResponseMessage
    {
        public ResponseMessage()
        {

        }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; } = "";
    }
    public class BomsController :Needs.Web.Mvc.ClientController
    {   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Contact"></param>
        /// <param name="Email"></param>
        /// <param name="file"></param>
        /// <returns>-1:文件过大;-2格式不正确</returns>
        [HttpPost]
        public ActionResult BomsUpload(string Contact, string Email)
        {

            ResponseMessage result = new ResponseMessage() { Success = false };
            var file=Request.Files["filename"];
            string saveLogoPath = "";
            var BomPath = file.FileName;
            Random rd = new Random();
               var  i= rd.Next();
            var  NewFileName = Current.ID + DateTime.Now.ToString("yyyyMMddHHmmss") + i + Path.GetExtension(BomPath);
          
            if (file.ContentLength > 1 * 1024 * 1024)
            {
                result.Message = "-1";
            }
            else if (Path.GetExtension(BomPath)!=".xls"&& Path.GetExtension(BomPath) != ".xlsx")
            {
                result.Message = "-2";
            }
            else
            {

                if (file != null && !string.IsNullOrEmpty(NewFileName) && NewFileName != "")
                {
                    Needs.Utils.NetFile netfile = new Needs.Utils.NetFile(NewFileName, Needs.Utils.NetFileType.NewBom);
                    var paths = netfile.Save(file.InputStream);
                    saveLogoPath = paths[1];
                }

                new NtErp.Wss.Services.Models.Boms() {ClientID = Current.ID,Contact=Contact,Uri=saveLogoPath,Email=Email }.Enter();
                result.Success = true;
                result.Message = "0";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBoms()
        {

            return Json(this.Paging(new BomsView().Where(item=>item.ClientID==Current.ID)), JsonRequestBehavior.AllowGet);
            
        }



        public ActionResult BomDel(string ID)
        {
            //var ID = Request.Files["ID"];

            ResponseMessage result = new ResponseMessage() { Success = false};
            result.Message = "-1";
            try
            {
                new Boms() { ID = ID,ClientID=Current.ID }.Abandon();
                result.Success = true;
                result.Message = "1";
                
                
            }
            catch (Exception)
            {
                
            }


            return Json(result, JsonRequestBehavior.AllowGet);


        }

    }
}