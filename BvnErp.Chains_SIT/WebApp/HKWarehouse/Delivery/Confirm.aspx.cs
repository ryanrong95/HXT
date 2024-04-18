using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Linq;

namespace WebApp.HKWarehouse.Delivery
{
    /// <summary>
    /// 提货通知单---香港库房
    /// </summary>
    public partial class Confirm : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadOrderData();
            }
        }

        /// <summary>
        /// 订单信息
        /// </summary>
        protected void LoadOrderData()
        {
            this.Model.deliveryNotice = "".Json();
            string id = Request.QueryString["ID"];
            if (!string.IsNullOrEmpty(id))
            {
                var deliveryNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.DeliveryNotice[id];
                if (deliveryNotice == null)
                {
                    return;
                }

                var order = deliveryNotice.Order;
                this.Model.OrderID = order.ID;

                var Files = order.MainOrderFiles.Where(x => x.FileType == FileType.DeliveryFiles && x.Status == Status.Normal).FirstOrDefault();
                this.Model.deliveryNotice = new
                {
                    OrderID = deliveryNotice.Order.ID,
                    Customer = deliveryNotice.Order.Client.Company.Name,
                    PickUpFiles = FileDirectory.Current.FileServerUrl + "/" + Files?.Url.ToUrl(),// 提货文件
                    FileName = Files?.Name==null?"":Files?.Name,
                    PackNo = deliveryNotice.Order.PackNo,
                    DeliveryCompany = deliveryNotice.DeliveryConsignees.Supplier,
                    DeliveryTime = deliveryNotice.DeliveryConsignees.PickUpDate.ToString("yyyy-MM-dd"),
                    ContactName = deliveryNotice.DeliveryConsignees.Contact,
                    PhoneNumber = deliveryNotice.DeliveryConsignees.Tel,
                    DeliveryAddress = deliveryNotice.DeliveryConsignees.Address,
                    NoticeStatus = deliveryNotice.DeliveryNoticeStatus,//提货状态
                }.Json();
            }
        }

        /// <summary>
        /// 产品列表数据加载
        /// </summary>
        protected void LoadProducts()
        {
            string orderID = Request.QueryString["orderID"];
            if (!string.IsNullOrEmpty(orderID))
            {
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders.Where(x => x.ID == orderID).FirstOrDefault();
                var data = order.Items;
                Func<OrderItem, object> convert = item => new
                {
                    ProductModel = item.Model,
                    ProductName = item.Category?.Name,//归类后的品名
                    Manufacturer = item.Manufacturer,
                    Quantity = item.Quantity,
                    Origin = item.Origin,
                    Weight = item.GrossWeight,
                };
                this.Paging(data, convert);
            }
        }

        /// <summary>
        /// 确认提货
        /// </summary>
        protected void ConfirmDelivery()
        {
            string id = Request.Form["ID"];
            var deliveryNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.DeliveryNotice[id];
            if (deliveryNotice != null)
            {
                deliveryNotice.EnterError += Delivey_EnterError;
                deliveryNotice.EnterSuccess += Delivey_EnterSuccess;

                deliveryNotice.CurrentAdmin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                deliveryNotice.Confirm();
            }
        }

        /// <summary>
        /// 确认成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delivey_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "确认成功" }).Json());
        }

        /// <summary>
        /// 失败触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delivey_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "确认失败" }).Json());
        }

        //提货操作日志记录
        protected void LoadDeliveryLogs()
        {
            string id = Request.Form["ID"];
            var noticeLog = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.DeliveryNoticeLog.Where(x => x.DeliveryNoticeID == id); 
            noticeLog = noticeLog.OrderByDescending(x => x.CreateDate);
            Func<DeliveryNoticeLog, object> convert = item => new
            {
                Summary = item.Summary,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
            Response.Write(new { rows = noticeLog.Select(convert).ToArray(), }.Json());
        }
    }
}