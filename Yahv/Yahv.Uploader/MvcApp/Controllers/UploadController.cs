using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApp.Models;
using Yahv.Underly;
using Uploader.Services.Models;
using Uploader.Services.Views;
using System.Configuration;

namespace MvcApp.Controllers
{
    public class UploadController : Controller
    {
        //override 
        public ActionResult Index()
        {
            return Json(new
            {
                success = true,
                code = 200,
                message = "File Upload Runnign!"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Index(FormCollection forms)
        {
            var query = Request.QueryString;
            var list = new List<Yahv.Services.Models.UploadResult>();
            string topID = FilesDescriptionView.GenID();

            using (FilesDescriptionView view = new FilesDescriptionView())
            {
                int counter = 0;
                DateTime now = DateTime.Now;

                //数位
                var digit = Request.Files.Count.ToString().Length;

                foreach (string key in Request.Files.AllKeys)
                {
                    var file = Request.Files[key];

                    string fileName;


                    if (Request.Files.Count == 1)
                    {
                        fileName = string.Concat(topID, Path.GetExtension(file.FileName));
                    }
                    else
                    {
                        fileName = string.Concat(topID, "-", (counter++).ToString().PadLeft(digit, '0'), Path.GetExtension(file.FileName));
                    }

                    var message = new FileMessage
                    {
                        WsOrderID = query["WsOrderID"],
                        LsOrderID = query["LsOrderID"],
                        ApplicationID = query["ApplicationID"],
                        AdminID = query["AdminID"],
                        ClientID = query["ClientID"],
                        InputID = query["InputID"],
                        NoticeID = query["NoticeID"],
                        CustomName = query["CustomName"] ?? file.FileName,
                        StorageID = query["StorageID"],
                        Type = int.Parse(query["Type"] ?? "0"),
                        Url = string.Join("/", now.Year.ToString(),
                            now.Month.ToString(),
                            now.Day.ToString(),
                            fileName),
                        WaybillID = query["WaybillID"],
                        PayID = query["PayID"],
                        StaffID = query["StaffID"],
                        ErmApplicationID = query["ErmApplicationID"],
                        ShipID=query["ShipID"]
                    };

                    if (query["Sender"] == "WsCamera")
                    {
                        message.CustomName = fileName;
                    }

                    view.Add(message);

                    DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["SavePath"]);
                    string fullFileName = Path.Combine(di.FullName,
                        now.Year.ToString(),
                        now.Month.ToString(),
                        now.Day.ToString(),
                        fileName);

                    FileInfo fi = new FileInfo(fullFileName);

                    if (!fi.Directory.Exists)
                    {
                        fi.Directory.Create();
                    }

                    file.SaveAs(fi.FullName);

                    list.Add(new Yahv.Services.Models.UploadResult
                    {
                        FileID = topID,
                        FileName = message.CustomName,
                        SessionID = query["SessionID"],
                        //Url = string.Concat(Request.Url.Scheme, "://", Request.Url.Host, Request.Url.IsDefaultPort? "" :(":"+Request.Url.Port)  ,"/" + message.Url),
                        Url = string.Concat(Request.Url.Scheme, "://", Request.Url.Authority, "/" + message.Url),
                    });
                }
            }

            return Json(list.ToArray());
        }

        [HttpPost]
        public ActionResult DeleteFile(string fileID)
        {
            try
            {
                var view = new FilesDescriptionView();

                view.DeleteFile(fileID);
                return Json(new
                {
                    Code = 200,
                    Success = true,
                    Data = "调用成功"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Code = 400,
                    Success = false,
                    Data = "调用失败，" + ex.Message
                });
            }

        }

        //[HttpPost]
        //public ActionResult DeleteFiles(string[] fileIDs)
        //{
        //    try
        //    {
        //        var view = new FilesDescriptionView();

        //        view.DeleteFiles(fileIDs);
        //        return Json(new
        //        {
        //            Code = 200,
        //            Success = true,
        //            Data = "调用成功"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            Code = 400,
        //            Success = false,
        //            Data = "调用失败，" + ex.Message
        //        });
        //    }

        //}
    }
}