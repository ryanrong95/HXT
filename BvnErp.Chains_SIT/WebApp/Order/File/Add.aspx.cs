using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.File
{
    public partial class Add : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 保存附件
        /// </summary>
        protected void SaveFile()
        {
            try
            {
                //前台数据
                var orderID = Request.Form["OrderID"];
                var type = (FileType)Enum.Parse(typeof(FileType), Request.Form["Type"]);
                var file = Request.Files["File"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                //订单附件
                if (file != null && file.ContentLength != 0)
                {
                    //文件保存
                    string fileName = file.FileName.ReName();
                    FileDirectory fileDic = new FileDirectory(fileName);
                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Order);
                    fileDic.CreateDataDirectory();
                    file.SaveAs(fileDic.FilePath);

                    //先删除之前上传的对账单或委托书
                    if (type == FileType.OrderBill || type == FileType.AgentTrustInstrument || type == FileType.DeliveryFiles)
                    {
                        var origFiles = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderFiles.Where(f => f.OrderID == orderID && f.FileType == type && f.Status == Status.Normal);
                        foreach (var origFile in origFiles)
                            origFile.Abandon();
                    }

                    //var orderfile = new Needs.Ccs.Services.Models.MainOrderFile()
                    //{
                    //    MainOrderID = orderID,
                    //    Admin = admin,
                    //    Name = file.FileName,
                    //    FileType = type,
                    //    FileFormat = file.ContentType,
                    //    Url = fileDic.VirtualPath,
                    //    FileStatus = OrderFileStatus.Audited
                    //};

                    //orderfile.Enter();

                    #region 本地文件同步中心文件库

                    var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                    var dic = new { CustomName = file.FileName, WsOrderID = orderID, AdminID = ErmAdminID };

                    var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.Declaration;

                    if (type == FileType.OrderBill)
                    {
                        centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.OrderBill;
                    }
                    if (type == FileType.OriginalInvoice)
                    {
                        centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.Invoice;
                    }
                    if (type == FileType.DeliveryFiles)
                    {
                        centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.SalesContract;
                    }

                    //本地文件上传到服务器
                    var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(fileDic.FilePath, centerType, dic);

                    #endregion
                }

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}