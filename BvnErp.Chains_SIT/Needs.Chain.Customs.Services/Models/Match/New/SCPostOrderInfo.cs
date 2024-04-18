using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 同步订单信息
    /// </summary>
    public class SCPostOrderInfo : SCHandler
    {
        public string CurrentMainOrderID { get; set; }
        public SCPostOrderInfo(Order currentOrder)
        {
            CurrentOrder = currentOrder;
            string[] orderID = currentOrder.ID.Split('-');
            CurrentMainOrderID = orderID[0];
        }
        public override void handleRequest()
        {
            try
            {

                List<Needs.Ccs.Services.Views.OrderItemChanges> listOrderItemChanges = new List<Needs.Ccs.Services.Views.OrderItemChanges>();

                //var cgDeliveryView = new Needs.Ccs.Services.Views.CgDeliveriesTopViewOrigin().
                //   Where(item => item.MainOrderID == this.CurrentMainOrderID && item.OrderItemID != null).OrderBy(item => item.CaseNo).ToList();

                var OrderIDs = new Views.Orders2View().Where(item => item.MainOrderID == this.CurrentMainOrderID).Select(item => item.ID).ToList();
                foreach (var orderId in OrderIDs)
                {
                    var order = new Views.Orders2View().Where(t => t.ID == orderId).FirstOrDefault();
                    foreach (var item in order.Items)
                    {
                        //var deliveryInfo = cgDeliveryView.Where(t => t.OrderItemID == item.ID).FirstOrDefault();
                        listOrderItemChanges.Add(new Needs.Ccs.Services.Views.OrderItemChanges()
                        {
                            OrderItemID = item.ID,
                            InputID = "",
                            CustomName = item.Name,
                            Product = new Needs.Ccs.Services.Views.CenterProduct()
                            {
                                PartNumber = item.Model,
                                Manufacturer = item.Manufacturer,
                            },
                            Origin = item.Origin,
                            DateCode = item.Batch,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            TotalPrice = item.TotalPrice,
                            Unit = item.Unit,
                            TinyOrderID = orderId,
                        });
                    }
                }

                Post2AgentWarehouse(listOrderItemChanges);

                if (next != null)
                {
                    next.handleRequest();
                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("到货拆分接口中调用代仓储接口传当前跟单确认到货信息,CurrentOrder:"+ CurrentOrder.ID);
            }
        }

        private void Post2AgentWarehouse(List<Needs.Ccs.Services.Views.OrderItemChanges> listOrderItemChanges)
        {
            Needs.Ccs.Services.Views.CurrentOrderInfo currentOrderInfo = new Needs.Ccs.Services.Views.CurrentOrderInfo()
            {
                OrderID = CurrentMainOrderID,
                Currency = CurrentOrder.Currency,
                Confirmed = false,
                items = listOrderItemChanges,
                OriginOrderItemIDs = new List<string>(),
        };

            var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
            var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SubmitChanged;

            Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
            {
                ID = Guid.NewGuid().ToString("N"),
                OrderID = CurrentMainOrderID,
                TinyOrderID = CurrentOrder.ID,
                Url = apiurl,
                RequestContent = currentOrderInfo.Json(),
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            };
            apiLog.Enter();

            var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, currentOrderInfo);
            apiLog.ResponseContent = result;
            apiLog.Enter();
        }
    }
}
