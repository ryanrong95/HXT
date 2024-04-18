using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.Merchandiser
{
    /// <summary>
    /// 文件上传展示界面
    /// 用于上传3C认证或原产地证明
    /// </summary>
    public partial class FileDisplay : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化管控数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id];

            this.Model.ControlData = new
            {
                control.ID,
                OrderID = control.Order.ID,
                ClientName = control.Order.Client.Company.Name,
                ClientRank = control.Order.Client.ClientRank,
                DeclarePrice = control.Order.DeclarePrice.ToRound(2).ToString("0.00"),
                Currency = control.Order.Currency,
                Merchandiser = control.Order.Client.Merchandiser.RealName,
                ControlType = control.ControlType.GetDescription()
            }.Json();
        }

        /// <summary>
        /// 初始化管控产品列表
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];
            var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id];
            IEnumerable<OrderFile> files = new List<OrderFile>();
            if (control.ControlType == OrderControlType.CCC)
            {
                files = control.Files.Where(file => file.FileType == FileType.CCC);
            }
            else if (control.ControlType == OrderControlType.OriginCertificate)
            {
                files = control.Files.Where(file => file.FileType == FileType.OriginCertificate);
            }

            Func<OrderControlItem, object> convert = item => new
            {
                item.ID,
                item.OrderID,
                OrderItemID = item.OrderItem.ID,
                ControlTypeValue = item.ControlType,
                ControlType = item.ControlType.GetDescription(),
                item.OrderItem.Category.Name,
                item.OrderItem.Model,
                item.OrderItem.Manufacturer,
                item.OrderItem.Category.HSCode,
                item.OrderItem.Quantity,
                UnitPrice = item.OrderItem.UnitPrice.ToString("0.0000"),
                TotalPrice = item.OrderItem.TotalPrice.ToRound(2).ToString("0.00"),
                item.OrderItem.Origin,
                Declarant = item.OrderItem.Category.ClassifySecondOperator?.RealName,
                FileID = files.Where(file => file.OrderItemID == item.OrderItem.ID).FirstOrDefault()?.ID,
                Url = FileDirectory.Current.FileServerUrl + "/" + files.Where(file => file.OrderItemID == item.OrderItem.ID).FirstOrDefault()?.Url.ToUrl()
            };

            Response.Write(new
            {
                rows = control.Items.Select(convert).ToList(),
                total = control.Items.Count()
            }.Json());
        }

        /// <summary>
        /// 上传3C认证或原产地证明
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                var orderID = Request.Form["OrderID"];
                var orderItemID = Request.Form["OrderItemID"];
                var fileType = Request.Form["FileType"];
                var fileID = Request.Form["FileID"];
                var rowNum = int.Parse(Request.Form["RowNum"]);
                var file = Request.Files.GetMultiple("uploadFile")[rowNum];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                //文件保存
                string fileName = file.FileName.ReName();

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Order);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);

                OrderFile orderFile = new OrderFile();
                orderFile.ID = fileID;
                orderFile.OrderID = orderID;
                orderFile.OrderItemID = orderItemID;
                orderFile.Admin = admin;
                orderFile.Name = file.FileName;
                orderFile.FileType = (FileType)Enum.Parse(typeof(FileType), fileType);
                orderFile.FileFormat = file.ContentType;
                orderFile.Url = fileDic.VirtualPath;
                orderFile.FileStatus = OrderFileStatus.Audited;
                orderFile.EnterSuccess += File_UploadSuccess;
                orderFile.Enter();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 确认文件上传完成，审批通过
        /// </summary>
        protected void Confirm()
        {
            try
            {
                string id = Request.Form["ID"];
                var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                control.SetAdmin(admin);
                control.Confirmed += Control_ConfirmSuccess;
                control.Confirm();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 文件上传成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void File_UploadSuccess(object sender, SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "上传成功！" }).Json());
        }

        /// <summary>
        /// 审批成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_ConfirmSuccess(object sender, OrderControledEventArgs e)
        {
            NoticeLog noticeLog = new NoticeLog();
            noticeLog.MainID = e.OrderControl.Order.ID;
            switch (e.OrderControl.ControlType)
            {
                case OrderControlType.CCC:
                    noticeLog.NoticeType = SendNoticeType.CCC;
                    break;

                case OrderControlType.OriginCertificate:
                    noticeLog.NoticeType = SendNoticeType.OriginCertificate;
                    break;
            }
            noticeLog.Readed = true;
            noticeLog.SendNotice();
            Response.Write((new { success = true, message = "审批成功！" }).Json());
        }
    }
}