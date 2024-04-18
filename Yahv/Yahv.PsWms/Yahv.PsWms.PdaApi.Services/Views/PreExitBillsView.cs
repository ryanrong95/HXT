using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.PdaApi.Services.Extends;
using Yahv.PsWms.PdaApi.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Http;

namespace Yahv.PsWms.PdaApi.Services.Views
{
    /// <summary>
    /// 预出库单视图
    /// </summary>
    public class PreExitBillsView : NoticesView
    {
        public PreExitBillsView()
        {
        }

        protected PreExitBillsView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected PreExitBillsView(PsWmsRepository reponsitory, IQueryable<Notice> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        public override object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<Notice>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            #region 补充完整对象

            var ienum_iquery = iquery.ToArray();

            //客户信息
            var clientIDs = ienum_iquery.Select(item => item.ClientID).Distinct();
            var clientsView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
                              where clientIDs.Contains(client.ID)
                              select new
                              {
                                  client.ID,
                                  client.Name
                              };
            var ienum_clients = clientsView.ToArray();

            //产品信息
            var noticeIDs = ienum_iquery.Select(item => item.ID);
            var productsView = from item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>()
                               join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>() on item.ProductID equals product.ID
                               where noticeIDs.Contains(item.NoticeID)
                               select new
                               {
                                   item.ID,
                                   item.NoticeID,
                                   item.Mpq,
                                   item.PackageNumber,
                                   item.Total,
                                   product.Partnumber,
                                   product.Brand
                               };
            var ienum_products = productsView.ToArray();

            var linq = from notice in ienum_iquery
                       join client in ienum_clients on notice.ClientID equals client.ID
                       join product in ienum_products on notice.ID equals product.NoticeID into products
                       select new
                       {
                           notice.ID,
                           ClientName = client.Name,
                           notice.FormID,
                           TotalCount = products.Count(),

                           Products = products.Select(item => new
                           {
                               item.Partnumber,
                               item.Brand,
                               item.Mpq,
                               item.PackageNumber,
                               item.Total
                           }).ToArray()
                       };

            #endregion

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                return new
                {
                    Total = total,
                    Size = pageSize,
                    Index = pageIndex,
                    Data = linq.ToArray(),
                };
            }
            else
            {
                return linq.ToArray();
            }
        }

        #region 搜索方法
        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PreExitBillsView SearchByID(string id)
        {
            var noticesView = this.IQueryable.Cast<Notice>();
            var linq = from notice in noticesView
                       where notice.ID == id
                       select notice;

            var view = new PreExitBillsView(this.Reponsitory, linq);
            return view;
        }
        #endregion

        /// <summary>
        /// 扫码复核
        /// </summary>
        /// <param name="code">预出库单号</param>
        public void Review(string code)
        {
            var notice = this.Single(item => item.ID == code);
            if (notice.Status != Enums.NoticeStatus.Reviewing)
                throw new Exception($"当前出库通知状态为【{notice.Status.GetDescription()}】，不能进行预出库单复核");

            //复核完成，将通知状态更新为“等待包装”
            this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Notices>(new
            {
                Status = (int)Enums.NoticeStatus.Packing,
                ModifyDate = DateTime.Now,
            }, item => item.ID == code);
        }

        /// <summary>
        /// 扫码出库
        /// </summary>
        /// <param name="code">预出库单号</param>
        /// <param name="reviewerID">复核人</param>
        public void Outbound(string code, string reviewerID)
        {
            #region 完成出库
            var notice = this.Single(item => item.ID == code);
            if (notice.Status != Enums.NoticeStatus.Packing)
                throw new Exception($"当前出库通知状态为【{notice.Status.GetDescription()}】，不能出库");

            //出库完成，将通知状态更新为“等待收货”
            Reponsitory.Update<Layers.Data.Sqls.PsWms.Notices>(new
            {
                Status = (int)Enums.NoticeStatus.Arrivaling,
                ModifyDate = DateTime.Now,
            }, item => item.ID == code);
            #endregion

            #region 修改订单状态
            var orderTask = Task.Run(() =>
            {
                var apisetting = new ApiSettings.SzApiSetting();
                var url = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.ChangeOrderStatus;

                var data = new
                {
                    OrderID = notice.FormID,
                    OrderStatus = (int)Enums.OrderStatus.Closed,
                };
                var result = ApiHelper.Current.JPost<object>(url, data);
            });
            #endregion

            #region 通知项
            var noticeItemsView = from item in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>()
                                  where item.NoticeID == code
                                  select new
                                  {
                                      item.ID,
                                      item.NoticeID,
                                      item.ProductID,
                                      item.InputID,
                                      item.Mpq,
                                      item.PackageNumber,
                                      item.Total,
                                      item.UnitPrice,
                                      item.FormItemID,
                                      item.StorageID,
                                      item.ShelveID,
                                      item.Exception
                                  };
            var ienum_noticeItems = noticeItemsView.ToArray();
            #endregion

            #region 扣减库存
            //并行减库存
            /*
            Parallel.For(0, ienum_storages.Length, index =>
            {
                var storageID = ienum_storages[index].ID;
                var noticeItems = ienum_noticeItems.Where(item => item.StorageID == storageID).ToArray();
                int packageNumber = ienum_storages[index].PackageNumber - noticeItems.Sum(item => item.PackageNumber);
                int total = ienum_storages[index].Total - noticeItems.Sum(item => item.Total);

                using (var reponsitory = new PsWmsRepository())
                {
                    reponsitory.Update<Layers.Data.Sqls.PsWms.Storages>(new
                    {
                        PackageNumber = packageNumber,
                        Total = total,
                        ModifyDate = DateTime.Now,
                    }, item => item.ID == storageID);
                }
            });
            */

            //异步减库存
            var storageTask = Task.Run(() =>
            {
                using (var reponsitory = new PsWmsRepository())
                {
                    //库存信息
                    var storageIDs = ienum_noticeItems.Select(item => item.StorageID).Distinct();
                    var storagesView = from storage in reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Storages>()
                                       where storageIDs.Contains(storage.ID)
                                       select new
                                       {
                                           storage.ID,
                                           storage.PackageNumber,
                                           storage.Total
                                       };
                    var ienum_storages = storagesView.ToArray();

                    var tempStorages = new List<TempStorage>();
                    foreach (var storage in ienum_storages)
                    {
                        var noticeItems = ienum_noticeItems.Where(item => item.StorageID == storage.ID).ToArray();
                        int packageNumber = storage.PackageNumber - noticeItems.Sum(item => item.PackageNumber);
                        int total = storage.Total - noticeItems.Sum(item => item.Total);

                        tempStorages.Add(new TempStorage
                        {
                            ID = storage.ID,
                            PackageNumber = packageNumber,
                            Total = total,
                            ModifyDate = DateTime.Now
                        });
                    }

                    using (var conn = reponsitory.CreateConnection())
                    {
                        conn.BulkUpdateByTempTable(tempStorages);
                    }
                }
            });


            #endregion

            #region 出库报告
            //生成出库报告
            var reportID = Layers.Data.PKeySigner.Pick(PKeyType.Report);
            Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Reports()
            {
                ID = reportID,
                ReportType = (int)Enums.ReportType.Outbound,
                ReviewDateTime = DateTime.Now,
                ReviewerID = reviewerID,
                Status = (int)GeneralStatus.Normal,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                FormID = notice.FormID
            });

            //生成出库报告项
            var reportItemIDs = Layers.Data.PKeySigner.Series(PKeyType.ReportItem, ienum_noticeItems.Count())
                                .OrderBy(item => item).ToArray();
            var reportItems = new Layers.Data.Sqls.PsWms.ReportItems[ienum_noticeItems.Count()];
            for (int index = 0; index < reportItems.Length; index++)
            {
                var noticeItem = ienum_noticeItems[index];
                reportItems[index] = new Layers.Data.Sqls.PsWms.ReportItems()
                {
                    ID = reportItemIDs[index],
                    ReportID = reportID,
                    NoticeID = notice.ID,
                    NoticeItemID = noticeItem.ID,
                    ProductID = noticeItem.ProductID,
                    InputID = noticeItem.InputID,
                    NoticeMpq = noticeItem.Mpq,
                    NoticePackageNumber = noticeItem.PackageNumber,
                    NoticeTotal = noticeItem.Total,
                    StorageMpq = noticeItem.Mpq,
                    StoragePackageNumber = noticeItem.PackageNumber,
                    StorageTotal = noticeItem.Total,
                    AdminID = reviewerID,
                    FormID = notice.FormID,
                    FormItemID = noticeItem.FormItemID,
                    OutCurrency = (int)Currency.CNY,
                    OutUnitPrice = noticeItem.UnitPrice,
                    ClientID = notice.ClientID,
                    ShelveID = noticeItem.ShelveID,
                    CreateDate = DateTime.Now,
                    Exception = noticeItem.Exception,
                    ReviewDate = DateTime.Now,
                    ReviewerID = reviewerID
                };
            }

            using (var conn = Reponsitory.CreateConnection())
            {
                conn.BulkInsert(reportItems);
            }
            #endregion

            orderTask.Wait();
            storageTask.Wait();
        }
    }
}
