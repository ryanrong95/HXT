using Layers.Data.Sqls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Notice
{
    public class StorageInNoticeService
    {
        /// <summary>
        /// 订单
        /// </summary>
        public Layers.Data.Sqls.PsOrder.Orders Order { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public Layers.Data.Sqls.PsOrder.OrderItems[] OrderItems { get; set; }

        /// <summary>
        /// 产品信息
        /// </summary>
        public Layers.Data.Sqls.PsOrder.Products[] Products { get; set; }

        /// <summary>
        /// 货运信息
        /// </summary>
        public Layers.Data.Sqls.PsOrder.OrderTransports OrderTransport { get; set; }

        /// <summary>
        /// 特殊要求
        /// </summary>
        public Layers.Data.Sqls.PsOrder.Requires[] Requires { get; set; }

        /// <summary>
        /// 装箱单文件
        /// </summary>
        public Layers.Data.Sqls.PsOrder.PcFiles[] Files { get; set; }

        /// <summary>
        /// Json 字符串 请求
        /// </summary>
        public string StrJsonReq { get; set; }

        /// <summary>
        /// Json 字符串 接收
        /// </summary>
        public string StrJsonRes { get; set; }

        public string StrJsonFileReq { get; set; }

        public string StrJsonFileRes { get; set; }

        /// <summary>
        /// 产生 Json
        /// </summary>
        public string GenerateJson(string trackerID)
        {
            string fileUrlPrefix = ConfigurationManager.AppSettings["FileUrlPrefix"];
            string WarehouseID = ConfigurationManager.AppSettings["WarehouseID"];

            var items = this.OrderItems.Select(item => new StorageNoticeModel.Item
            {
                Product = this.Products.Where(t => t.ID == item.ProductID).Select(p => new StorageNoticeModel.Product
                {
                    Partnumber = p.Partnumber,
                    Brand = p.Brand,
                    Package = p.Package,
                    DateCode = p.DateCode,
                    Mpq = p.Mpq?.ToString(),
                    Moq = p.Moq?.ToString(),
                }).FirstOrDefault(),
                ID = item.NoticeItemID,
                Source = Convert.ToString(((int)NoticeSource.Tracker)),
                CustomCode = item.CustomCode,
                StocktakingType = item.StocktakingType,
                Mpq = item.Mpq,
                PackageNumber = item.PackageNumber,
                Total = item.Total,
                Currency = (int)Currency.CNY,
                ClientID = this.Order.ClientID,
                FormID = item.OrderID,
                FormItemID = item.ID,
            }).ToArray();

            var requires = this.Requires.Select(item => new StorageNoticeModel.Require
            {
                OrderID = item.OrderID,
                OrderTransportID = item.OrderTransportID,
                Name = item.Name,
                Contents = item.Content,
            }).ToArray();

            //var packingFiles = this.Files.Select(item => new StorageNoticeModel.File
            //{
            //    Type = item.Type.ToString(),
            //    CustomName = item.CustomName,
            //    Url = string.Join(@"/", fileUrlPrefix, item.Url),
            //    SiteuserID = item.SiteuserID,
            //}).ToArray();

            var consignor = new StorageNoticeModel.Transport
            {
                TransportMode = this.OrderTransport.TransportMode.ToString(),
                Carrier = this.OrderTransport.Carrier,
                WaybillCode = this.OrderTransport.WaybillCode,
                ExpressPayer = this.OrderTransport.ExpressPayer,
                TakingTime = this.OrderTransport.TakingTime?.ToString("yyyy-MM-dd"),
                Address = this.OrderTransport.Address,
                Contact = this.OrderTransport.Contact,
                Phone = this.OrderTransport.Phone,
            };

            string noticeID = null;
            if (this.OrderItems != null && this.OrderItems.Length > 0)
            {
                noticeID = this.OrderItems[0].NoticeID;
            }

            var storageNoticeModel = new StorageNoticeModel
            {
                ID = noticeID,
                WarehouseID = WarehouseID,
                ClientID = this.Order.ClientID,
                CompanyID = this.Order.CompanyID,
                NoticeType = Convert.ToString((int)NoticeType.Inbound),
                FormID = this.Order.ID,
                TrackerID = trackerID,
                Items = items,
                Requires = requires,
                //Files = packingFiles,
                Consignor = consignor,
            };

            this.StrJsonReq = JsonConvert.SerializeObject(storageNoticeModel);
            return this.StrJsonReq;
        }

        #region 新的 Json

        public string GenerateJsonNew(string trackerID)
        {
            string WarehouseID = ConfigurationManager.AppSettings["WarehouseID"];

            var items = this.OrderItems.Select(item => new StorageNoticeModelNew.Item
            {
                Product = this.Products.Where(t => t.ID == item.ProductID).Select(p => new StorageNoticeModelNew.Product
                {
                    Partnumber = p.Partnumber,
                    Brand = p.Brand,
                    Package = p.Package,
                    DateCode = p.DateCode,
                    Mpq = p.Mpq ?? 0,
                    Moq = p.Moq ?? 0,
                }).FirstOrDefault(),
                OrderItemID = item.ID,
                Source = Convert.ToString(((int)NoticeSource.Tracker)),
                InputID = item.InputID,
                CustomCode = item.CustomCode,
                StocktakingType = item.StocktakingType,
                Mpq = item.Mpq,
                PackageNumber = item.PackageNumber,
                Total = item.Total,
                Currency = (int)Currency.CNY,
            }).ToArray();

            var requires = this.Requires.Select(item => new StorageNoticeModelNew.Require
            {
                Name = item.Name,
                Contents = item.Content,
                Type = 1, //应接收端要求, 要么给1, 要么给 null
            }).ToArray();

            var consignor = new StorageNoticeModelNew.Transport
            {
                TransportMode = this.OrderTransport.TransportMode,
                Carrier = this.OrderTransport.Carrier,
                WaybillCode = this.OrderTransport.WaybillCode,
                ExpressPayer = this.OrderTransport.ExpressPayer,
                TakingTime = this.OrderTransport.TakingTime?.ToString("yyyy-MM-dd"),
                Address = this.OrderTransport.Address,
                Contact = this.OrderTransport.Contact,
                Phone = this.OrderTransport.Phone,
            };

            var storageNoticeModel = new StorageNoticeModelNew
            {
                OrderID = this.Order.ID,
                WarehouseID = WarehouseID,
                ClientID = this.Order.ClientID,
                CompanyID = this.Order.CompanyID,
                NoticeType = (int)NoticeType.Inbound,
                TrackerID = trackerID,
                Items = items,
                Requires = requires,
                Consignor = consignor,
            };

            this.StrJsonReq = JsonConvert.SerializeObject(storageNoticeModel);
            return this.StrJsonReq;
        }

        public string GenerateJsonFile()
        {
            string fileUrlPrefix = ConfigurationManager.AppSettings["FileUrlPrefix"];

            var packingFiles = this.Files.Select(item => new StorageNoticeFileModel.File
            {
                Type = item.Type.ToString(),
                CustomName = item.CustomName,
                Url = string.Join(@"/", fileUrlPrefix, item.Url),
                SiteuserID = item.SiteuserID,
            }).ToArray();

            var uploadList = new List<StorageNoticeFileModel.UploadItem>();
            uploadList.Add(new StorageNoticeFileModel.UploadItem
            {
                MainID = this.Order.ID,
                Files = packingFiles,
            });

            StorageNoticeFileModel fileModel = new StorageNoticeFileModel
            {
                Upload = uploadList.ToArray(),
            };

            this.StrJsonFileReq = JsonConvert.SerializeObject(fileModel);
            return this.StrJsonFileReq;
        }

        #endregion


        /// <summary>
        /// 发送通知
        /// </summary>
        public void SendNotice(string orderID)
        {
            string warehouseUrl = ConfigurationManager.AppSettings["WarehouseUrl"];
            string url = string.Join(@"/", warehouseUrl, "InNotices/Notice");

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                repository.Insert(new Layers.Data.Sqls.PsOrder.Logs
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ActionName = LogAction.NewStorageInSendNotice.GetDescription(),
                    MainID = orderID,
                    Url = url,
                    Content = this.StrJsonReq,
                    CreateDate = DateTime.Now,
                });
            }

            string apiResult = string.Empty;
            using (WebClient client = new WebClient { Encoding = Encoding.UTF8 })
            {
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Accept", "application/json");
                client.Headers.Add("User-Agent", "POST");
                apiResult = client.UploadString(url, "POST", this.StrJsonReq);
                this.StrJsonRes = apiResult;
            }

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                repository.Insert(new Layers.Data.Sqls.PsOrder.Logs
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ActionName = LogAction.NewStorageInRevNotice.GetDescription(),
                    MainID = orderID,
                    Url = url,
                    Content = apiResult,
                    CreateDate = DateTime.Now,
                });
            }
        }

        /// <summary>
        /// 发送文件信息
        /// </summary>
        public void SendFileInfo(string orderID)
        {
            string warehouseUrl = ConfigurationManager.AppSettings["WarehouseUrl"];
            string url = string.Join(@"/", warehouseUrl, "Files/upload");

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                repository.Insert(new Layers.Data.Sqls.PsOrder.Logs
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ActionName = LogAction.SendFileInfoInInNoticeReq.GetDescription(),
                    MainID = orderID,
                    Url = url,
                    Content = this.StrJsonFileReq,
                    CreateDate = DateTime.Now,
                });
            }

            string apiResult = string.Empty;
            using (WebClient client = new WebClient { Encoding = Encoding.UTF8 })
            {
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Accept", "application/json");
                client.Headers.Add("User-Agent", "POST");
                apiResult = client.UploadString(url, "POST", this.StrJsonFileReq);
                this.StrJsonFileRes = apiResult;
            }

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                repository.Insert(new Layers.Data.Sqls.PsOrder.Logs
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ActionName = LogAction.SendFileInfoInInNoticeRes.GetDescription(),
                    MainID = orderID,
                    Url = url,
                    Content = apiResult,
                    CreateDate = DateTime.Now,
                });
            }

        }
    }
}
