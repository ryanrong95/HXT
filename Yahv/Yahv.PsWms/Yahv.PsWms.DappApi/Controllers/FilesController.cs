using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.PsWms.DappApi.Services.Views;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    /// <summary>
    /// 图片接口
    /// </summary>
    public class FilesController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            //filterContext.HttpContext.Response.StatusCode = 501;
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ActionResult Show(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 400,
                    data = "参数id不能为null或空字符串"
                }, JsonRequestBehavior.AllowGet);
            }

            using (var view = new PcFilesView())
            {
                var data = view;
                data = data.SearchByMainID(id);
                var results = data.ToArray();
                Response.StatusCode = 200;

                return Json(new
                {
                    success = true,
                    data = results
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据Jpost参数返回请求的文件
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult Show(JPost jpost)
        {
            var arguments = new
            {
                mainID = jpost["MainID"].Value<string>(),
                id = jpost["ID"]?.Value<string>(),
                type = jpost["Type"]?.Value<int?>(),
            };

            using (var view = new PcFilesView())
            {
                var data = view;
                if (string.IsNullOrEmpty(arguments.mainID))
                {
                    Response.StatusCode = 500;
                    return Json(new
                    {
                        success = false,
                        data = "参数错误,MainID不能为空"
                    }, JsonRequestBehavior.DenyGet);
                }

                data = data.SearchByMainID(arguments.mainID);

                if (!string.IsNullOrEmpty(arguments.id))
                {
                    data = data.SearchByFileID(arguments.id);
                }

                if (arguments.type.HasValue)
                {
                    data = data.SearchByType((Services.Enums.FileType)arguments.type.Value);
                }

                var result = data.ToMyArray();
                Response.StatusCode = 200;
                return Json(new
                {
                    success = true,
                    data = result
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 文件删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Response.StatusCode = 500;

                return Json(new
                {
                    success = false,
                    data = "参数不正确, ID不能为Null或空字符串!"
                }, JsonRequestBehavior.DenyGet);
            }

            try
            {
                using (var view = new PcFilesView())
                {
                    view.DeleteFile(id);
                }

                Response.StatusCode = 200;
                return Json(new
                {
                    success = true,
                    data = "文件删除成功",
                }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;

                return Json(new
                {
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 删除多个文件
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Deletes(JPost jpost)
        {
            var ids = jpost["IDs"]?.Values<string>();

            if (ids.Count() == 0)
            {
                Response.StatusCode = 500;

                return Json(new
                {
                    success = false,
                    data = "参数不正确, IDs不能为Null或空字符串!"
                }, JsonRequestBehavior.DenyGet);
            }

            try
            {
                string[] idArr = ids.ToArray();
                using (var view = new PcFilesView())
                {
                    view.DeleteFiles(idArr);
                }

                Response.StatusCode = 200;
                return Json(new
                {
                    success = true,
                    data = "文件删除成功",
                }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;

                return Json(new
                {
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload()
        {
            string prexPath = ConfigurationManager.AppSettings["PrexPath"];
            string urlPath = ConfigurationManager.AppSettings["PrexUrl"];
            List<UploadFileResult> uploadResultList = new List<UploadFileResult>();

            using (var pcFilesView = new PcFilesView())
            {
                List<PcFile> pcFileList = new List<PcFile>();

                if (Request.Files.Count > 0)
                {
                    #region 处理上传的内容是文件的情况

                    var adminID = Request.QueryString["AdminID"];
                    var siteUserID = Request.QueryString["SiteuserID"];
                    var fileType = (Services.Enums.FileType)Enum.Parse(typeof(Services.Enums.FileType), Request.QueryString["Type"]);
                    var mainID = Request.QueryString["MainID"];

                    DirectoryInfo di = new DirectoryInfo(Path.Combine(prexPath, DateTime.Now.ToString("yyyyMMdd")));
                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    //处理多个 文件, 并保存
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var uploadFile = Request.Files[i];
                        var uploadFileName = uploadFile.FileName;
                        var uploadExtension = Path.GetExtension(uploadFileName);

                        var fileID = pcFilesView.GenerateFileID();
                        var newFileName = string.Concat(fileID, uploadExtension);
                        var newFullFileName = Path.Combine(di.FullName, newFileName);

                        // 对象化, 然后进行保存
                        var tempFile = new PcFile
                        {
                            ID = fileID,
                            MainID = mainID,
                            Type = fileType,
                            CustomName = uploadFileName,
                            Url = string.Concat(DateTime.Now.ToString("yyyyMMdd"), "/", newFileName),
                            CreateDate = DateTime.Now,
                            AdminID = adminID,
                            SiteuserID = siteUserID,
                        };
                        pcFileList.Add(tempFile);

                        FileInfo fi = new FileInfo(newFullFileName);

                        if (!fi.Exists)
                        {
                            uploadFile.SaveAs(newFullFileName);
                        }

                        uploadResultList.Add(new UploadFileResult
                        {
                            ID = tempFile.ID,
                            CustomName = tempFile.CustomName,
                            Url = string.Concat(urlPath.TrimEnd('/'), "/", tempFile.Url),
                        });
                    }

                    pcFilesView.Enter(pcFileList.ToArray());

                    Response.StatusCode = 200;
                    return Json(new
                    {
                        success = true,
                        data = uploadResultList.ToArray(),
                    }, JsonRequestBehavior.DenyGet);

                    #endregion
                }
                else
                {
                    #region 非异步上传处理

                    var jpost = new JPost();
                    var jobjects = jpost["Upload"];
                    var mainIds = jobjects.Select(item => item["MainID"]).Values<string>().ToArray();

                    var filesView = new PcFilesView().Where(item => mainIds.Contains(item.MainID));

                    foreach (var jobject in jobjects)
                    {
                        using (WebClient client = new WebClient())
                        {
                            var MainID = jobject["MainID"].Value<string>();
                            var files = jobject["Files"];
                            foreach (var fileItem in files)
                            {
                                Uri uri = new Uri(fileItem["Url"].Value<string>().Trim());
                                string fileName = VirtualPathUtility.GetFileName(uri.AbsolutePath);
                                string newUrlPath = uri.AbsolutePath.TrimStart('/');

                                var file = filesView.FirstOrDefault(item => item.MainID == MainID && item.Url == newUrlPath);
                                if (file == null)
                                {
                                    string newFilePath = uri.AbsolutePath.TrimStart('/').Replace('/', '\\');
                                    FileInfo fi = new FileInfo(Path.Combine(prexPath, newFilePath));
                                    if (!fi.Directory.Exists)
                                    {
                                        fi.Directory.Create();
                                    }

                                    client.DownloadFile(uri, fi.FullName);


                                    var fileID = pcFilesView.GenerateFileID();

                                    // 对象化, 然后进行保存
                                    var tempFile = new PcFile
                                    {
                                        ID = fileID,
                                        MainID = MainID,
                                        Type = (Services.Enums.FileType)fileItem["Type"].Value<int>(),
                                        CustomName = fileItem["CustomName"].Value<string>(),
                                        Url = newUrlPath,
                                        CreateDate = DateTime.Now,
                                        AdminID = fileItem["AdminID"].Value<string>(),
                                        SiteuserID = fileItem["SiteuserID"].Value<string>(),
                                    };
                                    pcFileList.Add(tempFile);

                                    uploadResultList.Add(new UploadFileResult
                                    {
                                        ID = tempFile.ID,
                                        CustomName = tempFile.CustomName,
                                        Url = string.Concat(urlPath.TrimEnd('/'), "/", newUrlPath),
                                    });
                                }
                                else
                                {
                                    uploadResultList.Add(new UploadFileResult
                                    {
                                        ID = file.ID,
                                        CustomName = file.CustomName,
                                        Url = string.Concat(urlPath.TrimEnd('/'), "/", file.Url),
                                    });
                                }
                            }
                        }
                    }

                    pcFilesView.Enter(pcFileList.ToArray());

                    Response.StatusCode = 200;
                    return Json(new
                    {
                        success = true,
                        data = uploadResultList.ToArray(),
                    }, JsonRequestBehavior.DenyGet);

                    #endregion
                }
            }
        }

    }

}