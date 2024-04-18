using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
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

namespace WebApp.Order.File
{
    /// <summary>
    /// 订单附件详情界面
    /// </summary>
    public partial class Display : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 初始化订单附件信息
        /// </summary>
        protected void dataOrderFiles2()
        {
            string id = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[id];
            var orderFiles = from file in order.Files
                             join item in order.Items on file.OrderItemID equals item.ID into items
                             from item in items.DefaultIfEmpty()
                             where file.FileType != FileType.OrderFeeFile &&
                             (file.FileType == FileType.AgentTrustInstrument || file.FileType == FileType.OrderBill || file.Status == Status.Normal)
                             select new
                             {
                                 file.ID,
                                 OrderID = file.OrderID,
                                 Name = item?.Category?.Name ?? item?.Name ?? "--",
                                 Manufacturer = item?.Manufacturer ?? "--",
                                 Model = item?.Model ?? "--",
                                 FileName = file.Name,
                                 FileType = file.FileType.GetDescription(),
                                 FileFormat = file.FileFormat,
                                 FileStatus = file.FileStatus.GetDescription(),
                                 Status = file.Status.GetDescription(),
                                 CreateDate = file.CreateDate.ToShortDateString(),
                                 Url = FileDirectory.Current.FileServerUrl + "/" + file.Url.ToUrl()
                             };

            Response.Write(new
            {
                rows = orderFiles.ToArray(),
                total = orderFiles.Count()
            }.Json());
        }


        protected void dataOrderFiles()
        {
            string id = Request.QueryString["ID"];
            var Orders = new Orders2View().OrderBy(item => item.ID).Where(item => item.MainOrderID == id).ToList();
           // var Orders = new Orders2View().OrderBy(item => item.ID).Where(item => item.MainOrderID == id && item.OrderStatus != OrderStatus.Canceled && item.OrderStatus != OrderStatus.Returned).ToList();
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[Orders.FirstOrDefault().ID];
            var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
            var orderFiles = from file in order?.MainOrderFiles
                             where
                             (file.FileType == FileType.OriginalInvoice || file.FileType == FileType.AgentTrustInstrument || file.FileType == FileType.OrderBill)&& (file.FileType != FileType.None) &&file.Status!=Status.Delete
                             select new
                             {
                                 file.ID,
                                 OrderID = file.MainOrderID,
                                 FileName = file.Name,
                                 FileType = file.FileType.GetDescription(),
                                 FileFormat = file.FileFormat,
                                 FileStatus = file.FileStatus.GetDescription(),
                                 Status = file.Status.GetDescription(),
                                 CreateDate = file.CreateDate.ToShortDateString(),
                                 Url = DateTime.Compare(file.CreateDate, t1) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + file.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + file.Url?.ToUrl(),
                                 //  Url = FileDirectory.Current.FileServerUrl + "/" + file.Url.ToUrl()
                             };

            Response.Write(new
            {
                rows = orderFiles.ToArray(),
                total = orderFiles.Count()
            }.Json());
        }
    }
}