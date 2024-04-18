using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.FileServices.Models;
using Yahv.Underly;

namespace Yahv.FileServices.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return null;
        }

        // GET: Home
        [HttpPost]
        [Route("upload/{key}")]
        public ActionResult Index(string key)
        {

            var config = UploadConfig.Configs.Where(item => item.Id == key || item.Name == key).FirstOrDefault();
            var typesizes = config.TypeSizes.Select(item => item.FileType);

            if (config == null)
            {
                return Json(new JMessage { success = false, code = 300, data = "不支持！" }, JsonRequestBehavior.AllowGet); ;
            }


            var keys = System.Web.HttpContext.Current.Request.Files.AllKeys;
            if (keys.Length > 0)
            {

                foreach (var item in keys)
                {

                    HttpPostedFile hpfile = System.Web.HttpContext.Current.Request.Files[item];

                    var fileExt = Path.GetExtension(hpfile.FileName);

                    if (!typesizes.Contains(fileExt))
                    {
                        return Json(new JMessage { success = false,code=310,data="文件类型错误！" }, JsonRequestBehavior.AllowGet); 
                    }

                    decimal fileSize = hpfile.ContentLength;
                    var sysfileSize = config.TypeSizes.Where(tem => tem.FileType == fileExt).First().FileSize;
                    if (fileSize > sysfileSize)
                    {
                        return Json(new JMessage { success = false, code = 320, data = "文件太大了！不能大于（"+sysfileSize+")k"}, JsonRequestBehavior.AllowGet); ;
                    }

                    var datePath = DateTime.Now.ToString("yyyy/MM/dd");

                    var fileUrl = string.Concat(UploadConfig.FileUploadUrl, config.Path, "/", datePath, "/");
                    var filePath = string.Concat(UploadConfig.FileUploadPath, config.Path, @"\", datePath.Replace("/", @"\"), @"\");
                    string fullPath = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }

                    var originName = Path.GetFileNameWithoutExtension(hpfile.FileName);
                    var fileName = Guid.NewGuid();
                    fileUrl = string.Concat(fileUrl, fileName, fileExt);
                    filePath = string.Concat(filePath, fileName, fileExt);

                    hpfile.SaveAs(filePath);

              
                    return Json(new JSingle<object>{ success = true, code = 200, data = new { name = originName, url = fileUrl } }, JsonRequestBehavior.AllowGet); ;

                }
            }
            else
            {
              
            }



            return Json(new JMessage { success = false, code = 400, data = "错误！" }, JsonRequestBehavior.AllowGet); ;

        }
    }
}