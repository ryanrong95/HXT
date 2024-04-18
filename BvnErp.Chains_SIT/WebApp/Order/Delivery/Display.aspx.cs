using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Delivery
{
    /// <summary>
    /// 跟单员生成深圳库房出库通知
    /// </summary>
    public partial class Display : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var order = new Orders2View().Where(item => item.MainOrderID == id && item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Declared).FirstOrDefault();

            this.Model.OrderData = new
            {
                ID = order.MainOrderID,
                OrderID = order.ID,
                ClientID = order.ClientID,
                CreateDate = order.CreateDate.ToShortDateString()
            }.Json();
        }

        /// <summary>
        /// 初始化分拣信息
        /// </summary>
        protected void dataSortings()
        {
            string id = Request.QueryString["ID"];


            //var Orders = new Orders2View().Where(item => item.MainOrderID == id && item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Declared);
            //var OrderIDS = Orders.Select(item => item.ID).ToList();
            //var sortings = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZSorting.Where(item => OrderIDS.Contains(item.OrderID)).OrderBy(item => item.BoxIndex).AsQueryable(); 

            var sortings = new SZCenterSortingsViewForDisplay().Where(t => t.OrderID == id).OrderBy(t => t.BoxIndex).ToList();

            Func<Needs.Ccs.Services.Models.SZSorting, object> convert = sorting => new
            {
                sorting.ID,
                sorting.BoxIndex,
                sorting.DateBoxIndex,
                //sorting.OrderItem.Category.Name,
                sorting.OrderItem.Name,
                sorting.OrderItem.Manufacturer,
                sorting.OrderItem.Model,
                sorting.OrderItem.Origin,
                sorting.Quantity,
                DeliveriedQuantity = sorting.DeliveriedQuantity
            };

            Response.Write(new
            {
                rows = sortings.Select(convert).ToArray(),
                total = sortings.Count()
            }.Json());
        }

        /// <summary>
        /// 初始化送货信息 老方法
        /// </summary>
        protected void dataDeliveries1()
        {
            string id = Request.QueryString["ID"];
            var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZMianExitNoticeView.Where(item => item.Order.ID == id).AsQueryable();

            Func<Needs.Ccs.Services.Models.SZExitNotice, object> convert = notice => new
            {
                notice.ID,
                DeliveryCode = notice.ExitDeliver.Code,
                DeliveryDate = notice.ExitDeliver.DeliverDate.ToShortDateString(),
                DeliveryType = notice.ExitType.GetDescription(),
                DeliveryQuantity = notice.SZItems.Select(item => item.Quantity).Sum(),
                PackNo = notice.ExitDeliver.PackNo,
                DeliveryStatus = notice.DeliveryStatus,
                ExitNoticeStatus = notice.ExitNoticeStatus,
                notice.IsPrint
            };

            Response.Write(new
            {
                rows = notices.Select(convert).ToArray(),
                total = notices.Count()
            }.Json());
        }

        /// <summary>
        /// 初始化送货信息 新方法，通过接口查询
        /// </summary>
        protected void dataDeliveries()
        {
            string id = Request.QueryString["ID"];

            var exitNotices = new CenterSZExitNoticeView().Where(t => t.Order.ID == id).AsQueryable();

            Func<Needs.Ccs.Services.Models.SZExitNotice, object> convert = notice => new
            {
                notice.ID,
                //DeliveryCode = notice.WaybillID,
                DeliveryDate = notice.CreateDate.ToShortDateString(),
                DeliveryType = notice.CenterExitType.GetDescription(),
                //DeliveryQuantity = notice.Notices.Select(item => item.Quantity).Sum(),
                PackNo = notice.CenterPackNo,
                CenterExeStatus = notice.CenterExeStatus,
                CenterExeStatusName = notice.CenterExeStatus.GetDescription(),
                //ExitNoticeStatus = notice.Status,
                notice.IsPrint
            };

            Response.Write(new
            {
                rows = exitNotices.Select(convert).ToArray(),
                total = exitNotices.Count()
            }.Json());
        }

        /// <summary>
        /// 删除送货信息
        /// </summary>
        protected void Delete()
        {
            try
            {
                string id = Request.Form["ID"];

                var apisetting = new Needs.Ccs.Services.ApiSettings.PfWmsApiSetting();
                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SZExitNoticeDelete;
                var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl + "?id=" + id);
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Needs.Underly.JMessage>(result);
                if (!message.success)
                {
                    Response.Write((new { success = false, message = "删除失败!" + message.data }).Json());
                }
                else
                {
                    Response.Write((new { success = true, message = "删除成功！" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Notice_AbandonSuccess(object sender, SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "删除成功！", ID = e.Object }).Json());
        }

        /// <summary>
        /// 欠款情况列表数据
        /// </summary>
        protected void DebtsData()
        {
            string id = Request.QueryString["ID"];
            var view = new Needs.Ccs.Services.Views.CgXdtCreditsStatisticsView();

            var clientName = view.GetClientNameByMainOrderID(id);
            var debts = view.GetCgXdtCreditsStatisticsForDebtTip(clientName);

            if (debts != null && debts.Any())
            {
                for (int i = 0; i < debts.Count; i++)
                {
                    bool isOverdue = view.GetIsOverdue(debts[i].PayerID, debts[i].PayeeID, debts[i].Business, debts[i].CurrencyInt);
                    debts[i].IsOverdue = isOverdue;
                }
            }

            Func<Needs.Ccs.Services.Views.CgXdtCreditsStatisticsForDebtTipModel, object> convert = item => new
            {
                ClientName = item.PayerName,
                Currency = ShowCurrency(item.CurrencyInt),
                TotalSum = item.TotalSum,
                CostSum = item.CostSum,
                IsOverDue = item.IsOverdue ? "是" : "否",
            };

            Response.Write(new
            {
                rows = debts.Select(convert).ToArray(),

            }.Json());
        }

        private string ShowCurrency(int currencyInt)
        {
            string currencyStr = string.Empty;

            switch (currencyInt)
            {
                case 1:
                    currencyStr = "CNY";
                    break;
                default:
                    break;
            }

            return currencyStr;
        }

        protected void isCanDelivery()
        {
            
            string MainOrderID = Request.Form["MainOrderID"];
            string OrderID = Request.Form["OrderID"];
            string ClientID = Request.Form["ClientID"];
            bool isOverDuePayment = false;
            string getOrderId = "";
            UnHangUpCheck unHangUpCheck = new UnHangUpCheck(MainOrderID,OrderID);
            bool isExceedLimit = unHangUpCheck.IsExceedLimit(ClientID);

            isOverDuePayment = unHangUpCheck.isOverDuePayment(ClientID);
            if (isOverDuePayment)
            {
                getOrderId = unHangUpCheck.GetOrderID(ClientID);
            }
            Response.Write((new { success = isExceedLimit, OverDuePayment = isOverDuePayment, GetOrderId = getOrderId }).Json());
        }

    }
}