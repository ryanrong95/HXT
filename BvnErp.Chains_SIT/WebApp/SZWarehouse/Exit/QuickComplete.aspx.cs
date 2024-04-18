using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWarehouse.Exit
{
    public partial class QuickComplete : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void QueryExitNotice()
        {
            try
            {
                string ExitNoticeID = Request["ExitNoticeID"];
                if (!string.IsNullOrEmpty(ExitNoticeID))
                {
                    ExitNoticeID = ExitNoticeID.Trim();
                }

                var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExitNoticesOrigin
                    .Where(t => t.ID == ExitNoticeID
                             && t.WarehouseType == Needs.Ccs.Services.Enums.WarehouseType.ShenZhen
                             && t.Status == Needs.Ccs.Services.Enums.Status.Normal)
                    .FirstOrDefault();

                if (exitNotice == null)
                {
                    Response.Write((new { success = false, message = "未找到该单号信息", }).Json());
                    return;
                }

                var exitNoticeFile = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExitNoticeFilesView
                    .Where(t => t.ExitNoticeID == ExitNoticeID && t.FileType == Needs.Wl.Models.Enums.FileType.ReceiptConfirmationFile && t.Status == Needs.Wl.Models.Enums.Status.Normal)
                    .FirstOrDefault();

                Response.Write((new { success = true, message = string.Empty, exitNoticeInfo = new
                {
                    ExitNoticeID = exitNotice.ID,
                    ExitType = exitNotice.ExitType.GetDescription(),
                    CreateDate = exitNotice.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsFileUploaded = exitNoticeFile != null ? true : false,
                    FileName = exitNoticeFile != null ? exitNoticeFile.Name : string.Empty,
                    FileUrl = exitNoticeFile != null ? ConfigurationManager.AppSettings["FileServerUrl"] + @"/" + exitNoticeFile.URL.Replace(@"\", @"/") : string.Empty,
                    ExitNoticeStatus = exitNotice.ExitNoticeStatus,
                    ExitNoticeStatusDisplay = exitNotice.ExitNoticeStatus.GetDescription(),
                }, }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误，" + ex.Message, }).Json());
            }
        }

        /// <summary>
        /// 上传客户收货确认单
        /// </summary>
        protected void UploadReceiptConfirmFile()
        {
            try
            {
                string exitNoticeID = Request.Form["exitnoticeid"];

                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("upload-receipt-confirm-file");
                if (files.Count > 0)
                {
                    var validTypes = new List<string>() { ".jpg", ".bmp", ".jpeg", ".gif", ".png", };  //".pdf"
                    for (int i = 0; i < files.Count; i++)
                    {
                        string ext = Path.GetExtension(files[i].FileName);
                        if (!validTypes.Contains(ext.ToLower()))
                        {
                            Response.Write((new { success = false, message = "上传的原始PI只能是图片(jpg、bmp、jpeg、gif、png)格式！" }).Json()); //或pdf
                            return;
                        }

                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            //文件保存
                            string fileName = files[i].FileName.ReName();

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Warehouse);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);

                            //插入数据 ExitNoticeFiles 表
                            //Needs.Wl.Warehouse.Services.Models.ExitNoticeFile exitNoticeFile = new Needs.Wl.Warehouse.Services.Models.ExitNoticeFile()
                            //{
                            //    ID = Guid.NewGuid().ToString("N"),
                            //    ExitNoticeID = exitNoticeID,
                            //    AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                            //    Name = file.FileName,
                            //    FileType = Needs.Wl.Models.Enums.FileType.ReceiptConfirmationFile,
                            //    FileFormat = file.ContentType,
                            //    URL = fileDic.VirtualPath,
                            //    Status = Needs.Wl.Models.Enums.Status.Normal,
                            //    CreateDate = DateTime.Now,
                            //};
                            //exitNoticeFile.InsertUniqueFileForOneExitNotice(exitNoticeID);

                            Needs.Ccs.Services.Models.ExitNoticeFile exitNoticeFile = new Needs.Ccs.Services.Models.ExitNoticeFile()
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                ExitNoticeID = exitNoticeID,
                                AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                                Admin = new Admin() { ID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID },
                                Name = file.FileName,
                                FileType = Needs.Ccs.Services.Enums.FileType.ReceiptConfirmationFile,
                                FileFormat = file.ContentType,
                                URL = fileDic.VirtualPath,
                                Status = Needs.Ccs.Services.Enums.Status.Normal,
                                CreateDate = DateTime.Now,
                            };
                            SZExitInsertFileHandler handler = new SZExitInsertFileHandler(exitNoticeFile, exitNoticeID, Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                            handler.Execute();

                            fileList.Add(new
                            {
                                Name = file.FileName,
                                FileType = FileType.ReceiptConfirmationFile.GetDescription(),
                                FileFormat = file.ContentType,
                                VirtualPath = fileDic.VirtualPath,
                                Url = fileDic.FileUrl
                            });
                        }
                    }
                }

                if (fileList.Count() == 0)
                {
                    Response.Write((new { success = false, message = "服务器未保存文件！", }).Json());
                }
                else
                {
                    Response.Write((new { success = true, data = fileList }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }


    }
}