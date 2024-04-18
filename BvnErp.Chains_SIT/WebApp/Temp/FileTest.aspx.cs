using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Temp
{
    /// <summary>
    /// 用于临时测试，后期删除
    /// </summary>
    public partial class FileTest : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.Logs = new Needs.Ccs.Services.Views.OrderLogsView().Where(log => log.OrderID == "NL02020190420001").Json();
            this.Model.Files = new Needs.Ccs.Services.Views.OrderFilesView().Where(log => log.OrderID == "NL02020190420001").ToList()
                .Select(file => new
                {
                    file.ID,
                    file.Name,
                    file.FileFormat,
                    VirtualPath = file.Url,
                    Url = FileDirectory.Current.FileServerUrl + "/" + file.Url.ToUrl(),
                }).Json();

            List<string> test = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                test.Add("123.png".ReName());
            }
        }

        protected void dataFiles()
        {
            var files = new Needs.Ccs.Services.Views.OrderFilesView().Where(log => log.OrderID == "NL02020190420001").AsQueryable();

            Func<OrderFile, object> convert = file => new
            {
                file.ID,
                file.Name,
                file.FileFormat,
                VirtualPath = file.Url,
                Url = FileDirectory.Current.FileServerUrl + "/" + file.Url.ToUrl()
            };

            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count()
            }.Json());
        }

        protected void UploadFile()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadFile");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            //文件保存
                            string fileName = files[i].FileName.ReName();

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Order);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);

                            fileList.Add(new
                            {
                                Name = file.FileName,
                                FileFormat = file.ContentType,
                                VirtualPath = fileDic.VirtualPath,
                                Url = fileDic.FileUrl
                            });
                        }
                    }
                }

                Response.Write((new { success = true, data = fileList }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }
    }
}