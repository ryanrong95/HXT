//using System;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Wms.Services.Models;
//using System.Linq.Expressions;
//using System.Diagnostics;
//using Wms.Services.Enums;
//using System.Configuration;
//using Yahv.Underly.Attributes;
//using Yahv.Underly;
//using Yahv.Utils;
//using Yahv.Utils.EventExtend;
//using Yahv.Usually;

//namespace MvcApp.Controllers
//{
//    public class FileUpLoadController : Controller
//    {
//        enum Message
//        {
//            [Description("上传成功")]
//            Success = 0,
//            [Description("上传失败")]
//            Fail = 1,
//        }
//        Message message;

//        [HttpPost]
//        public ActionResult Index(string key, string base64Code=null)
//        {
//            //自定义路径
//            string subdir = "";
//            key = key.ToLower();
//            if (key.Contains("waybill"))
//            {
//                subdir = "/waybill/" + DateTime.Now.ToString("yyyyMMdd") + "/" + key + "";
//            }
//            else if (key.Contains("stor"))
//            {
//                subdir = "/storage/" + DateTime.Now.ToString("yyyyMMdd") + "/" + key + "";
//            }
//            else
//            {
//                throw new NotSupportedException("尚未支持此功能的图片上传");
//            }
//            string url = FileServices.Save(base64Code, subdir);

//            new CenterFileDescription
//            {
//                Type = FileType.StoragesPic,
//                AdminID = "",
//                ClientID = "",
//                CustomName = "",
//                InputID = "",
//                StorageID = key.Contains("stor") ? key : "",
//                WaybillID = key.Contains("waybill") ? key : "",
//                Url = url
//            }.AddEvent("EnterSuccess", new SuccessHanlder(Datas_EnterSuccess))
//            .AddEvent("EnterError", new ErrorHanlder(Datas_EnterError)).Enter();
//            return Json(new { val = (int)message, msg = message.GetDescription(), url = url });
//        }

//        private void Datas_EnterError(object sender, ErrorEventArgs e)
//        {
//            message = Message.Fail;
//        }

//        private void Datas_EnterSuccess(object sender, SuccessEventArgs e)
//        {
//            message = Message.Success;
//        }

//        #region 废弃的文件上传

//        // GET: FileUpLoad
//        //[HttpPost]
//        //public ActionResult Index(/*int fileSize = 0*/)
//        //{

//        //    #region 废弃的文件上传
//        //    //try
//        //    //{
//        //    //    string filePath = string.Empty;
//        //    //    string drive = string.Empty;
//        //    //    string msg = Message.OverSize.GetDescription();
//        //    //    string returnMsg = string.Empty;
//        //    //    if (fileSize<1024)
//        //    //    {
//        //    //        msg = Message.OverSize.GetDescription();
//        //    //        returnMsg = msg.Replace("X", fileSize + "KB");
//        //    //    }
//        //    //    foreach (string item in Request.Files)
//        //    //    {
//        //    //        HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;
//        //    //        //获取文件后缀名
//        //    //        string FileExtension = Path.GetExtension(file.FileName).ToLower();
//        //    //        //判断上传的文件是否为空
//        //    //        if (file == null || file.ContentLength == 0)
//        //    //        {
//        //    //            return Json(new { obj = (int)Message.NoneFile, msg = Message.NoneFile.GetDescription() });
//        //    //        }
//        //    //        //获取文件大小
//        //    //        var ContentLength = file.ContentLength;
//        //    //        //所有的图片类型
//        //    //        string[] PictureType = { ".webp", ".bm", ".pcx", ".tif", ".gif", ".jpeg", ".tga", ".exif", ".fpx", ".svg", ".psd", ".cdr", ".pcd", ".dxf", ".ufo", ".eps", ".ai", ".png", ".hdri", ".raw", ".wmf", ".flic", ".emf", ".ico" };
//        //    //        //判断是否是图片
//        //    //        if (PictureType.Contains(FileExtension.ToLower()))
//        //    //        {
//        //    //            //图片文件上传限于5M
//        //    //            if (ContentLength > 5242880)
//        //    //            {
//        //    //                return Json(new { obj = (int)Message.OverSize, msg = Message.OverSize.GetDescription() });
//        //    //            }
//        //    //        }
//        //    //        //所有的视频类型
//        //    //        string[] VideoType = { ".mpeg", ".avi", ".mov", ".asf", ".wmv", ".navi", ".3gp", ".ra", ".rm", "ram", ".mkv", ".flv", ".f4v", ".rmvb", ".webm", ".hddvd", ".qsv" };
//        //    //        if (VideoType.Contains(FileExtension.ToLower()))
//        //    //        {
//        //    //            //视频文件上传限于20M
//        //    //            if (ContentLength > 20971520)
//        //    //            {
//        //    //                return Json(new { obj = (int)Message.OverSize, msg = Message.OverSize.GetDescription() });
//        //    //            }
//        //    //        }
//        //    //        //其他类型文件 如Word,Excel,Pdf
//        //    //        if (!PictureType.Contains(FileExtension) && !VideoType.Contains(FileExtension))
//        //    //        {
//        //    //            if (ContentLength > 1048576)
//        //    //            {
//        //    //                return Json(new { obj = (int)Message.OverSize, msg = Message.OverSize.GetDescription() });
//        //    //            }
//        //    //        }
//        //    //        string drivePath= AppDomain.CurrentDomain.BaseDirectory;
//        //    //        drive = drivePath.Substring(0, drivePath.IndexOf("\\")).ToLower();
//        //    //        string serverFilePath = ConfigurationManager.AppSettings["filePath"].ToString();
//        //    //        string path= drive + serverFilePath;
//        //    //        Guid guid = Guid.NewGuid();
//        //    //        Stream getInputStream = file.InputStream;
//        //    //        //判断文件夹是否存在
//        //    //        if (!Directory.Exists(path))
//        //    //        {
//        //    //            Directory.CreateDirectory(path);
//        //    //        }
//        //    //        string fileNewName = guid.ToString() + FileExtension;
//        //    //        //默认1次读取102400个字节
//        //    //        byte[] getBytes = new byte[102400];
//        //    //        int count = 0;
//        //    //        using (var stream = new FileStream(path + "/" + fileNewName, FileMode.OpenOrCreate))
//        //    //        {
//        //    //            while ((count = getInputStream.Read(getBytes, 0, 1024)) != 0)
//        //    //            {
//        //    //                stream.Write(getBytes, 0, count);
//        //    //            }
//        //    //        }
//        //    //        filePath = serverFilePath + "/" + fileNewName;
//        //    //        message = Message.Success;
//        //    //    }


//        //    //    return Json(new { obj = filePath, drive = drive, msg = message.GetDescription() });
//        //    //}
//        //    //catch
//        //    //{
//        //    //    message = Message.CatchException;
//        //    //    return Json(new { obj = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
//        //    //}
//        //    #endregion
//        //    return View();

//        //}

//        #endregion
//    }
//}