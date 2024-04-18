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
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWareHouse.Exit
{
    /// <summary>
    /// 已出库报关订单
    /// 深圳库房
    /// </summary>
    public partial class Exited : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            this.Model.ExitType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ExitType>()
                .Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 已出库通知列表
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            //查询条件
            string ExitNoticeID = Request.QueryString["ExitNoticeID"];
            string OrderID = Request.QueryString["OrderID"];
            string CustomerCode = Request.QueryString["EntryNumber"];
            string ExitType = Request.QueryString["ExitType"];
            string ClientName = Request.QueryString["ClientName"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(ExitNoticeID))
            {
                ExitNoticeID = ExitNoticeID.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZExitedListView.SZExitedListModel, bool>>)(t => t.ExitNoticeID.Contains(ExitNoticeID)));
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZExitedListView.SZExitedListModel, bool>>)(t => t.OrderID.Contains(OrderID)));
            }
            if (!string.IsNullOrEmpty(ExitType))
            {
                ExitType = ExitType.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZExitedListView.SZExitedListModel, bool>>)(t => t.ExitType == (ExitType)int.Parse(ExitType)));
            }
            if (!string.IsNullOrEmpty(CustomerCode))
            {
                CustomerCode = CustomerCode.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZExitedListView.SZExitedListModel, bool>>)(t => t.ClientCode.Contains(CustomerCode)));
            }
            if (!string.IsNullOrEmpty(ClientName))
            {
                ClientName = ClientName.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZExitedListView.SZExitedListModel, bool>>)(t => t.ClientName.Contains(ClientName)));
            }

            int totalCount = 0;
            var exitNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitedListView.GetResult(out totalCount, page, rows, lamdas.ToArray());

            // 返回数据列表
            Func<Needs.Ccs.Services.Views.SZExitedListView.SZExitedListModel, object> convert = item => new
            {
                ID = item.ExitNoticeID,
                OrderID = item.OrderID,//订单编号
                ClientCode = item.ClientCode,
                ClientName = item.ClientName,//客户
                PackNo = item.PackNo,
                AdminName = item.AdminName,//制单人
                ExitType = item.ExitType.GetDescription(),
                CreateDate = item.CreateDate,
                NoticeStatus = item.ExitNoticeStatus.GetDescription(),
                IsHasReceiptConfirmationFile = item.IsHasReceiptConfirmationFile,
                OutStockTime = item.OutStockTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            };

            Response.Write(new
            {
                rows = exitNotices.Select(convert).ToArray(),
                total = totalCount,
            }.Json());
        }

        ///// <summary>
        ///// 上传客户收货确认单
        ///// </summary>
        //protected void UploadReceiptConfirmFile()
        //{
        //    try
        //    {
        //        string exitNoticeID = Request.Form["exitnoticeid"];

        //        List<dynamic> fileList = new List<dynamic>();
        //        IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("upload-receipt-confirm-file");
        //        if (files.Count > 0)
        //        {
        //            var validTypes = new List<string>() { ".jpg", ".bmp", ".jpeg", ".gif", ".png", };  //".pdf"
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                string ext = Path.GetExtension(files[i].FileName);
        //                if (!validTypes.Contains(ext.ToLower()))
        //                {
        //                    Response.Write((new { success = false, message = "上传的原始PI只能是图片(jpg、bmp、jpeg、gif、png)格式！" }).Json()); //或pdf
        //                    return;
        //                }

        //                //处理附件
        //                HttpPostedFile file = files[i];
        //                if (file.ContentLength != 0)
        //                {
        //                    //文件保存
        //                    string fileName = files[i].FileName.ReName();

        //                    //创建文件目录
        //                    FileDirectory fileDic = new FileDirectory(fileName);
        //                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Warehouse);
        //                    fileDic.CreateDataDirectory();
        //                    file.SaveAs(fileDic.FilePath);

        //                    //插入数据 ExitNoticeFiles 表
        //                    Needs.Wl.Warehouse.Services.Models.ExitNoticeFile exitNoticeFile = new Needs.Wl.Warehouse.Services.Models.ExitNoticeFile()
        //                    {
        //                        ID = Guid.NewGuid().ToString("N"),
        //                        ExitNoticeID = exitNoticeID,
        //                        AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
        //                        Name = file.FileName,
        //                        FileType = Needs.Wl.Models.Enums.FileType.ReceiptConfirmationFile,
        //                        FileFormat = file.ContentType,
        //                        URL = fileDic.VirtualPath,
        //                        Status = Needs.Wl.Models.Enums.Status.Normal,
        //                        CreateDate = DateTime.Now,
        //                    };
        //                    exitNoticeFile.InsertUniqueFileForOneExitNotice(exitNoticeID);

        //                    fileList.Add(new
        //                    {
        //                        Name = file.FileName,
        //                        FileType = FileType.ReceiptConfirmationFile.GetDescription(),
        //                        FileFormat = file.ContentType,
        //                        VirtualPath = fileDic.VirtualPath,
        //                        Url = fileDic.FileUrl
        //                    });
        //                }
        //            }
        //        }

        //        if (fileList.Count() == 0)
        //        {
        //            Response.Write((new { success = false, message = "服务器未保存文件！", }).Json());
        //        }
        //        else
        //        {
        //            Response.Write((new { success = true, data = fileList }).Json());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
        //    }
        //}

        ///// <summary>
        ///// 根据 ExitNoticeID 获取客户确认单文件信息
        ///// </summary>
        //protected void GetReceiptConfirmFileInfo()
        //{
        //    string exitNoticeID = Request.Form["ExitNoticeID"];

        //    var exitNoticeFile = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExitNoticeFilesView
        //        .Where(t => t.ExitNoticeID == exitNoticeID
        //                 && t.FileType == Needs.Wl.Models.Enums.FileType.ReceiptConfirmationFile
        //                 && t.Status == Needs.Wl.Models.Enums.Status.Normal)
        //        .OrderByDescending(t => t.CreateDate)
        //        .FirstOrDefault();

        //    if (exitNoticeFile == null)
        //    {
        //        Response.Write((new { success = false, message = "该出库通知无对应的客户确认单文件！", }).Json());
        //    }
        //    else
        //    {
        //        exitNoticeFile.URL = ConfigurationManager.AppSettings["FileServerUrl"] + @"/" + exitNoticeFile.URL.Replace(@"\", @"/");

        //        Response.Write((new { success = true, message = "", exitNoticeFile = exitNoticeFile, }).Json());
        //    }
        //}


    }
}