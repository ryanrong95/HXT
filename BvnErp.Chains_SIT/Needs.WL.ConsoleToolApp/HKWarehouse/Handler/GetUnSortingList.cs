
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Needs.WL.ConsoleToolApp
{
    public class GetUnSortingList
    {
        public List<UnSortingOrder> Orders { get; set; }

        public GetUnSortingList()
        {
            this.Orders = new List<UnSortingOrder>();
        }

        public void GetResult()
        {
            for(int i = 4; i < 5; i++)
            {
                this.Orders.Clear();
                var requestContent = new
                {
                    WaybillExcuteStatus = 10,
                    Source = 30,
                    WaybillType = 0,
                    PageSize = 20,
                    PageIndex = i,
                    WhID = "HK"
                };


                var apiurl = WarehouseAPI.ApiName + WarehouseAPI.UnSortingList;
                var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, requestContent);

                JObject jsonObject = (JObject)JToken.Parse(result);
                ResponseVo res = JsonConvert.DeserializeObject<ResponseVo>(jsonObject["obj"].ToString());

                foreach (var item in res.Data)
                {
                    JObject orderInfo = (JObject)JToken.Parse(Convert.ToString(item));
                    UnSortingOrder unSortingOrder = JsonConvert.DeserializeObject<UnSortingOrder>(orderInfo["Waybill"].ToString());
                    this.Orders.Add(unSortingOrder);
                }
            }
            
        }

        public void toExcel()
        {
            foreach (var order in this.Orders)
            {
                Console.WriteLine(order.OrderID);
            }
        }

        public void GenerateHistory()
        {
            using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var order in this.Orders)
                {
                    string orderID = order.OrderID + "-01";
                    try
                    {
                        Console.WriteLine(orderID + "开始");
                        var OrderInfo = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == orderID).FirstOrDefault();
                        if (OrderInfo == null)
                        {
                            continue;
                        }
                        string entryNoticeID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice);

                        reponsitory.Insert<Layer.Data.Sqls.ScCustoms.EntryNotices>(new Layer.Data.Sqls.ScCustoms.EntryNotices
                        {
                            ID = entryNoticeID,
                            OrderID = OrderInfo.ID,
                            WarehouseType = (int)WarehouseType.HongKong,
                            ClientCode = order.EnterCode,
                            SortingRequire = (int)SortingRequire.UnPacking,
                            EntryNoticeStatus = (int)EntryNoticeStatus.UnBoxed,
                            Status = (int)Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now
                        });

                        var OrderItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.OrderID == orderID);

                        foreach (var item in OrderItems)
                        {
                            reponsitory.Insert<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(new Layer.Data.Sqls.ScCustoms.EntryNoticeItems
                            {
                                ID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem),
                                EntryNoticeID = entryNoticeID,
                                OrderItemID = item.ID,
                                IsSpotCheck = false,
                                EntryNoticeStatus = (int)EntryNoticeStatus.UnBoxed,
                                Status = (int)Status.Normal,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now
                            });
                        }

                        if (!string.IsNullOrEmpty(order.Code))
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderConsignees>(new
                            {
                                CarrierID = order.CarrierID,                               
                                WayBillNo = order.Code,
                                UpdateDate = DateTime.Now
                            }, t => t.OrderID == orderID);
                        }
                        Console.WriteLine(orderID + "结束");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(orderID + "出错"+ex.ToString());
                        Console.ReadLine();
                    }                   
                }
            }
               
        }
        
    }
}
