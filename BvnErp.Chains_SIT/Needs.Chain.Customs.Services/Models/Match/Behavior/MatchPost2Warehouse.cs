using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MatchPost2Warehouse
    {
        public List<Models.OrderItemAssitant> OrderItems { get; private set; }

        public Order OriginOrder { get; set; }
        public MatchPost2Warehouse(List<Models.OrderItemAssitant> orderItems, Order originOrder)
        {
            this.OrderItems = orderItems;
            this.OriginOrder = originOrder;
        }

        public void Post()
        {
            try
            {
                string orderID = OriginOrder.ID;
                string currency = OriginOrder.Currency;
                List<Post2WarehouseModel> PostData = new List<Post2WarehouseModel>();
                foreach (var item in this.OrderItems)
                {
                    if (item.InputID.Contains(','))
                    {
                        string[] inputids = item.InputID.Split(',');
                        foreach (var t in inputids)
                        {
                            Post2WarehouseModel post = new Post2WarehouseModel();
                            post.Unique = t;
                            post.ItemID = item.ID;
                            post.TinyOrderID = orderID;
                            post.Currency = currency;
                            post.Price = item.UnitPrice;
                            PostData.Add(post);
                        }
                    }
                    else
                    {
                        Post2WarehouseModel post = new Post2WarehouseModel();
                        post.Unique = item.InputID;
                        post.ItemID = item.ID;
                        post.TinyOrderID = orderID;
                        post.Currency = currency;
                        post.Price = item.UnitPrice;
                        PostData.Add(post);
                    }
                }

                var apisetting = new Needs.Ccs.Services.ApiSettings.PfWmsApiSetting();
                string address = apisetting.UpdateInput;
                if (PostData.FirstOrDefault().Unique.Substring(0, 1).ToLower().Equals("o"))
                {
                    address = apisetting.UpdateOutput;
                }
                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + address;

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    OrderID = OriginOrder.MainOrderID,
                    TinyOrderID = OriginOrder.ID,
                    Url = apiurl,
                    RequestContent = PostData.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                };
                apiLog.Enter();


                var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, PostData);
                apiLog.ResponseContent = result;
                apiLog.Enter();
            }
            catch (Exception ex)
            {
                ex.CcsLog("到货确认调用库房接口传当前跟单确认到货信息");
            }
        }
    }
}
