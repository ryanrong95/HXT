using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebApp.HKWarehouse.Delivery
{
    /// <summary>
    /// 提货通知
    /// 待提货列表界面
    /// </summary>
    public partial class UnDeliveredList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        #region 主列表数据加载
        protected void data()
        {
            string OrderID = Request.QueryString["OrderId"];
            string CustomerCode = Request.QueryString["CustomerCode"];

            var deliveryNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.DeliveryNotice
                .Where(x => x.DeliveryNoticeStatus == DeliveryNoticeStatus.UnDelivery).AsQueryable();
            // 订单编号查询
            if (!string.IsNullOrEmpty(OrderID))
            {
                deliveryNotices = deliveryNotices.Where(x => x.Order.ID.Contains(OrderID));
            }
            //  入仓号
            if (!string.IsNullOrEmpty(CustomerCode))
            {
                deliveryNotices = deliveryNotices.Where(x => x.Order.Client.ClientCode.Contains(CustomerCode));
            }

            deliveryNotices = deliveryNotices.OrderByDescending(x => x.CreateDate);

            Func<DeliveryNotice, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.Order.ID,//订单编号
                CustomerCode = item.Order.Client.ClientCode,//入仓号
                Customer = item.Order.Client.Company.Name,//客户
                DeliveryCompany = item.DeliveryConsignees.Supplier,//提货公司
                DeliveryTime = item.DeliveryConsignees.PickUpDate.ToString("yyyy-MM-dd"),//提货时间
                DeliveryAddress = item.DeliveryConsignees.Address,//提货地址
                Merchandiser = item.Admin.RealName,//跟单员
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                NoticeStatus = item.DeliveryNoticeStatus.GetDescription(),//提货状态
            };
            this.Paging(deliveryNotices, convert);
        }
        #endregion
    }
}