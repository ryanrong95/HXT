using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Http;
using Yahv.Utils.Serializers;

namespace Wms.Services.chonggous.Views
{
    public class CgTempStocksView : QueryView<object, PvWmsRepository>
    {
        #region 构造函数
        public CgTempStocksView()
        {

        }

        protected CgTempStocksView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected CgTempStocksView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        #endregion

        protected override IQueryable<object> GetIQueryable()
        {            
            var waybillViews = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>(); 

            var waybillIDsView = from sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                 join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                 on sorting.ID equals storage.SortingID
                                 where storage.Type == (int)CgStoragesType.Staging
                                 select sorting.WaybillID;

            var waybillIDs = waybillIDsView.Distinct().OrderByDescending(item => item);

            var linqs = from waybill in waybillViews
                        join waybillid in waybillIDs on waybill.wbID equals waybillid
                        orderby waybill.wbID descending
                        select new MyWaybill
                        {
                            ID = waybill.wbID,
                            Code = waybill.wbCode,
                            EnterCode = waybill.wbEnterCode,
                            Supplier = waybill.wbSupplier,
                            ExcuteStatus = (CgTempStockExcuteStatus)waybill.wbExcuteStatus,
                            Type = (WaybillType)waybill.wbType,
                            CarrierID = waybill.wbCarrierID,
                            Consignor = new WayParter
                            {
                                ID = waybill.corID,
                                Company = waybill.corCompany,
                                Place = waybill.corPlace,
                                Address = waybill.corAddress,
                                Contact = waybill.corContact,
                                Phone = waybill.corPhone,
                                Zipcode = waybill.corZipcode,
                                Email = waybill.corEmail,
                                CreateDate = waybill.corCreateDate,
                                IDType = (IDType?)waybill.corIDType,
                                IDNumber = waybill.corIDNumber,
                            },
                            ConsigneeID = waybill.wbConsigneeID,
                            //Place = (Origin?)Enum.Parse(typeof(Origin?), waybill.corPlace),
                            FreightPayer = (WaybillPayer)waybill.wbFreightPayer,
                            OrderID = waybill.OrderID,
                            Source = (CgNoticeSource)waybill.Source,
                            NoticeType = (CgNoticeType)waybill.NoticeType,
                            CreateDate = waybill.wbCreateDate,
                            Summary = waybill.wbSummary,
                            TempEnterCode = waybill.TempEnterCode,
                        };
            return linqs;
        }

        /// <summary>
        /// 补全数据
        /// </summary>
        /// <returns></returns>
        public object[] ToMyArray()
        {
            return this.ToMyPage() as object[];
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<MyWaybill>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            // 获取符合条件的WaybillID
            var ienum_myWaybill = iquery.ToArray();
            var waybillIds = ienum_myWaybill.Select(item => item.ID).Distinct();

            #region 文件处理
            var filesView = from file in new Yahv.Services.Views.CenterFilesTopView()
                            where waybillIds.Contains(file.WaybillID)
                            select new CenterFileDescription
                            {
                                ID = file.ID,
                                WaybillID = file.WaybillID,
                                CustomName = file.CustomName,
                                Type = file.Type,
                                Url = CenterFile.Web + file.Url,
                                AdminID = file.AdminID,
                                Status = file.Status,
                            };

            var files = filesView.ToArray();
            #endregion            

            var sortingView = from sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                              join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                              on sorting.ID equals storage.SortingID
                              join _product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>()
                              on storage.ProductID equals _product.ID into products
                              from product in products.DefaultIfEmpty()
                              where storage.Type == (int)CgStoragesType.Staging && waybillIds.Contains(sorting.WaybillID)
                              select new
                              {
                                  sorting.ID,
                                  sorting.BoxCode,
                                  sorting.NoticeID,
                                  sorting.Quantity,
                                  sorting.WaybillID,
                                  sorting.AdminID,
                                  sorting.CreateDate,
                                  sorting.Weight,
                                  sorting.NetWeight,
                                  sorting.Volume,
                                  sorting.InputID,
                                  sorting.Summary,
                                  storage = new
                                  {
                                      storage.ID,
                                      Type = (CgStoragesType)storage.Type,
                                      storage.SortingID,
                                      storage.InputID,
                                      storage.Origin,
                                      storage.ProductID,
                                      Product = product == null ? null : new
                                      {
                                          PartNumber = product.PartNumber,
                                          Manufacturer = product.Manufacturer,
                                          PackageCase = product.PackageCase,
                                          Packing = product.Packaging,
                                      },
                                      storage.DateCode,
                                      storage.CreateDate,
                                      storage.ModifyDate,
                                      storage.ShelveID,
                                      storage.WareHouseID,
                                      storage.Total,
                                      storage.Quantity,
                                      storage.Supplier,
                                      storage.Summary,
                                  }
                              };

            var ienum_sortings = sortingView.ToArray();

            var clientView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>();
            var carriersTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>();

            var linq = from waybill in ienum_myWaybill
                       join sorting in ienum_sortings
                       on waybill.ID equals sorting.WaybillID into sortings
                       join _client in clientView on waybill.EnterCode equals _client.EnterCode into clients
                       from client in clients.DefaultIfEmpty()
                       join _carrier in carriersTopView on waybill.CarrierID equals _carrier.ID into carriers
                       from carrier in carriers.DefaultIfEmpty()
                       select new
                       {
                           Waybill = new
                           {
                               ID = waybill.ID,
                               Code = waybill.Code,
                               EnterCode = waybill.EnterCode,
                               ClientName = client?.Name,
                               Supplier = waybill.Supplier,
                               ExcuteStatus = waybill.ExcuteStatus,
                               Type = waybill.Type,
                               CarrierID = waybill.CarrierID,
                               CarrierName = carrier?.Name,
                               Consignor = waybill.Consignor,
                               ConsigneeID = waybill.ConsigneeID,
                               Place = waybill.Place,
                               FreightPayer = waybill.FreightPayer,
                               OrderID = waybill.OrderID,
                               Source = waybill.Source,
                               NoticeType = waybill.NoticeType,
                               CreateDate = waybill.CreateDate,
                               Summary = waybill.Summary,
                               TempEnterCode = waybill.TempEnterCode,
                               Files = files.Where(file => file.WaybillID == waybill.ID),
                           },
                           Sortings = sortings.ToArray(),
                       };
            var linq_ienum = linq.ToArray();            

            var linq_fenye = from item in linq_ienum                             
                             select new
                             {
                                 Waybill = new
                                 {
                                     ID = item.Waybill.ID,
                                     Code = item.Waybill.Code,
                                     EnterCode = item.Waybill.EnterCode,
                                     ClientName = item.Waybill.ClientName,
                                     Supplier = item.Waybill.Supplier,
                                     ExcuteStatus = item.Waybill.ExcuteStatus,
                                     Type = item.Waybill.Type,
                                     TypeDes = item.Waybill.Type.GetDescription(),
                                     CarrierID = item.Waybill.CarrierID,
                                     CarrierName = item.Waybill.CarrierName,
                                     Consignor = item.Waybill.Consignor,
                                     ConsigneeID = item.Waybill.ConsigneeID,
                                     Place = item.Waybill.Place,
                                     FreightPayer = item.Waybill.FreightPayer,
                                     OrderID = item.Waybill.OrderID,
                                     Source = item.Waybill.Source,
                                     NoticeType = item.Waybill.NoticeType,
                                     CreateDate = item.Waybill.CreateDate,
                                     Summary = item.Waybill.Summary,
                                     TempEnterCode = item.Waybill.TempEnterCode,
                                     Files = item.Waybill.Files,
                                     ShelveID = item.Sortings?.Select(sort => sort.storage.ShelveID)?.FirstOrDefault(),
                                     MinTempCreateDate = item.Sortings.Min(sort => sort.storage.CreateDate),
                                     MaxTempModifyDate = item.Sortings.All(sort => sort.Quantity == 0) ? (item.Sortings.All(sort => sort.storage.ModifyDate.HasValue) ? item.Sortings.Max(sort => sort.storage.ModifyDate.Value) : DateTime.Now) : DateTime.Now,
                                 },
                                 Sortings = item.Sortings,
                             };

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                var fenyeResult = linq_fenye.Select(item => new
                {
                    Waybill = new
                    {
                        ID = item.Waybill.ID,
                        Code = item.Waybill.Code,
                        EnterCode = item.Waybill.EnterCode,
                        ClientName = item.Waybill.ClientName,
                        Supplier = item.Waybill.Supplier,
                        ExcuteStatus = item.Waybill.ExcuteStatus,
                        Type = item.Waybill.Type,
                        TypeDes = item.Waybill.TypeDes,
                        CarrierID = item.Waybill.CarrierID,
                        CarrierName = item.Waybill.CarrierName,
                        Consignor = item.Waybill.Consignor,
                        ConsigneeID = item.Waybill.ConsigneeID,
                        Place = item.Waybill.Place,
                        FreightPayer = item.Waybill.FreightPayer,
                        OrderID = item.Waybill.OrderID,
                        Source = item.Waybill.Source,
                        NoticeType = item.Waybill.NoticeType,
                        CreateDate = item.Waybill.CreateDate,
                        Summary = item.Waybill.Summary,
                        TempEnterCode = item.Waybill.TempEnterCode,
                        Files = item.Waybill.Files,
                        ShelveID = item.Waybill.ShelveID,
                        TempDays = (new DateTime(item.Waybill.MaxTempModifyDate.Year, item.Waybill.MaxTempModifyDate.Month, item.Waybill.MaxTempModifyDate.Day) - new DateTime(item.Waybill.MinTempCreateDate.Year, item.Waybill.MinTempCreateDate.Month, item.Waybill.MinTempCreateDate.Day)).Days,
                    },
                });

                return new
                {
                    Total = total,
                    Size = pageSize ?? 20,
                    Index = pageIndex ?? 1,
                    Data = fenyeResult.ToArray(),
                };
            }

            var results = linq_fenye.Select(item => new
            {
                Waybill = new
                {
                    ID = item.Waybill.ID,
                    Code = item.Waybill.Code,
                    EnterCode = item.Waybill.EnterCode,
                    ClientName = item.Waybill.ClientName,
                    Supplier = item.Waybill.Supplier,
                    ExcuteStatus = item.Waybill.ExcuteStatus,
                    Type = item.Waybill.Type,
                    TypeDes = item.Waybill.Type.GetDescription(),
                    CarrierID = item.Waybill.CarrierID,
                    CarrierName = item.Waybill.CarrierName,
                    Consignor = item.Waybill.Consignor,
                    ConsigneeID = item.Waybill.ConsigneeID,
                    Place = item.Waybill.Place,
                    FreightPayer = item.Waybill.FreightPayer,
                    OrderID = item.Waybill.OrderID,
                    Source = item.Waybill.Source,
                    NoticeType = item.Waybill.NoticeType,
                    CreateDate = item.Waybill.CreateDate,
                    Summary = item.Waybill.Summary,
                    TempEnterCode = item.Waybill.TempEnterCode,
                    Files = item.Waybill.Files,
                    ShelveID = item.Waybill.ShelveID,
                    TempDays = (new DateTime(item.Waybill.MaxTempModifyDate.Year, item.Waybill.MaxTempModifyDate.Month, item.Waybill.MaxTempModifyDate.Day) - new DateTime(item.Waybill.MinTempCreateDate.Year, item.Waybill.MinTempCreateDate.Month, item.Waybill.MinTempCreateDate.Day)).Days,
                },
                SummaryStorages = item.Sortings.Where(sorting => String.IsNullOrEmpty(sorting.storage.ProductID) == true)?.Select(sorting => new
                {
                    StorageID = sorting.storage.ID,
                    Type = (CgStoragesType)sorting.storage.Type,
                    sorting.storage.SortingID,
                    sorting.storage.InputID,
                    sorting.storage.Origin,
                    sorting.storage.ProductID,
                    Product = sorting.storage.Product == null ? null : new
                    {
                        PartNumber = sorting.storage.Product.PartNumber,
                        Manufacturer = sorting.storage.Product.Manufacturer,
                        PackageCase = sorting.storage.Product.PackageCase,
                        Packing = sorting.storage.Product.Packing,
                    },
                    sorting.storage.DateCode,
                    sorting.storage.CreateDate,
                    sorting.storage.ShelveID,
                    sorting.storage.WareHouseID,
                    sorting.storage.Total,
                    sorting.storage.Quantity,
                    sorting.storage.Supplier,
                    sorting.storage.Summary,
                }),
                ProductStorages = item.Sortings.Where(sorting => String.IsNullOrEmpty(sorting.storage.ProductID) == false)?.Select(sorting => new
                {
                    StorageID = sorting.storage.ID,
                    Type = (CgStoragesType)sorting.storage.Type,
                    sorting.storage.SortingID,
                    sorting.storage.InputID,
                    sorting.storage.Origin,
                    sorting.storage.ProductID,
                    Product = sorting.storage.Product == null ? null : new
                    {
                        PartNumber = sorting.storage.Product.PartNumber,
                        Manufacturer = sorting.storage.Product.Manufacturer,
                        PackageCase = sorting.storage.Product.PackageCase,
                        Packing = sorting.storage.Product.Packing,
                    },
                    sorting.storage.DateCode,
                    sorting.storage.CreateDate,
                    sorting.storage.ShelveID,
                    sorting.storage.WareHouseID,
                    sorting.storage.Total,
                    sorting.storage.Quantity,
                    sorting.storage.Supplier,
                    sorting.storage.Summary,
                })
            });            

            if (!pageIndex.HasValue && !pageSize.HasValue)
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = results.ToArray(),
            };
        }

        /// <summary>
        /// 入库保存
        /// </summary>
        /// <param name="jobject"></param>
        public void Enter(JObject jobject)
        {
            var waybill = jobject["Waybill"];
            var adminID = jobject["AdminID"].Value<string>();
            var productStorages = jobject["ProductStorages"];
            var summaryStorages = jobject["SummaryStorages"];
            var delete = jobject["Delete"];

            var waybillID = waybill["ID"]?.Value<string>();

            var waybillView = Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>();
            int? freightPayer = waybill["FreightPayer"].Value<int?>();
            var consignorID = waybill["ConsignorID"].Value<string>();
            var warehouseID = waybill["WarehouseID"].Value<string>();
            var shelveID = waybill["ShelveID"].Value<string>();
            var supplier = waybill["Supplier"].Value<string>();
            var place = waybill["ConsignorPlace"].Value<string>();
            var contact = waybill["Contact"]?.Value<string>();
            var phone = waybill["Phone"].Value<string>();

            using (var reponsitory = new PvWmsRepository())
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            using (var pvdataReponsitory = new PvDataReponsitory())
            {
                // 新增保存
                if (!waybillView.Any(item => item.wbID == waybillID))
                {
                    #region 新增保存Waybill
                    if (string.IsNullOrEmpty(waybillID))
                    {
                        waybillID = PKeySigner.Pick(PkeyType.Waybills);
                    }
                    contact = contact == null ? "" : contact;
                    consignorID = string.Concat(supplier, place, "", contact, phone, "", "").MD5();

                    if (!pvcenterReponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Any(item => item.ID == consignorID))
                    {
                        // 新增发货人
                        pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayParters
                        {
                            ID = consignorID,
                            Company = supplier,
                            Place = place,
                            Address = "",
                            Contact = contact,
                            Phone = phone,
                            Zipcode = "",
                            Email = "",
                            CreateDate = DateTime.Now,
                            IDType = null,
                            IDNumber = null,
                        });
                    }

                    // 香港库房收货人---默认
                    string consigneeID = "B32EF70C088270664E184DA7BC2DFA06";
                    if (!pvcenterReponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Any(item => item.ID == consigneeID))
                    {
                        // 新增收货人
                        pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayParters
                        {
                            ID = consigneeID,
                            Company = "香港库房",
                            Place = "HKG",
                            Address = "",
                            Contact = "",
                            Phone = "",
                            Zipcode = "",
                            Email = "",
                            CreateDate = DateTime.Now,
                            IDType = 1,
                            IDNumber = null,
                        });
                    }

                    pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Waybills
                    {
                        ID = waybillID,
                        Code = waybill["Code"]?.Value<string>(),
                        Type = waybill["Type"].Value<int>(),
                        ConsignorID = consignorID,
                        ConsigneeID = consigneeID,
                        FreightPayer = freightPayer.HasValue ? freightPayer.Value : (int)WaybillPayer.Consignor,
                        Condition = new NoticeCondition().Json(),
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        EnterCode = waybill["EnterCode"].Value<string>(),
                        Status = (int)GeneralStatus.Normal,
                        CreatorID = adminID,
                        ModifierID = adminID,
                        ExcuteStatus = (int)CgTempStockExcuteStatus.TempStock,
                        Summary = waybill["Summary"].Value<string>(),
                        Supplier = supplier,
                        OrderID = waybill["OrderID"].Value<string>(),
                        CarrierID = waybill["CarrierID"].Value<string>(),
                        Source = (int)CgNoticeSource.AgentEnter,
                        NoticeType = (int)CgNoticeType.Enter,
                        TempEnterCode = PKeySigner.Pick(PKeyType.TempEnterCode),
                    });

                    var files = waybill["Files"];

                    if (files.Count() > 0)
                    {
                        List<string> fileids = new List<string>();
                        foreach (var file in files)
                        {
                            fileids.Add(file["ID"].Value<string>());
                        }

                        pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                        {
                            WaybillID = waybillID,
                            WsOrderID = waybill["OrderID"].Value<string>(),
                        }, item => fileids.Contains(item.ID));
                    }
                    #endregion

                    #region 新增Sorting及Storage

                    #region 描述库存, 根据新要求,去掉, 重新启用用于PDA使用

                    if (summaryStorages.Count() > 0)
                    {
                        foreach (var storage in summaryStorages)
                        {
                            var summary = storage["Summary"].Value<string>();
                            var sortingID = PKeySigner.Pick(PkeyType.Sortings);
                            reponsitory.Insert(new Layers.Data.Sqls.PvWms.Sortings
                            {
                                ID = sortingID,
                                WaybillID = waybillID,
                                Quantity = 0,
                                AdminID = adminID,
                                CreateDate = DateTime.Now,
                            });

                            reponsitory.Insert(new Layers.Data.Sqls.PvWms.Storages
                            {
                                ID = PKeySigner.Pick(PkeyType.Storages),
                                Type = (int)CgStoragesType.Staging, //暂存库,
                                WareHouseID = warehouseID,
                                SortingID = sortingID,
                                ProductID = "",
                                Total = 0,
                                Quantity = 0,
                                CreateDate = DateTime.Now,
                                Status = (int)GeneralStatus.Normal,
                                ShelveID = shelveID,
                                Supplier = supplier,
                                Summary = summary,

                            });
                        }
                    }

                    #endregion

                    #region 产品库存
                    if (productStorages.Count() > 0)
                    {
                        foreach (var product in productStorages)
                        {
                            var sortingID = PKeySigner.Pick(PkeyType.Sortings);
                            var quantity = product["Quantity"].Value<decimal>();
                            var origin = product["Origin"].Value<string>();
                            var datecode = product["DateCode"].Value<string>();

                            // 保存对应的Product                           
                            string productid = EnterProduct(product["Product"]);

                            reponsitory.Insert(new Layers.Data.Sqls.PvWms.Sortings
                            {
                                ID = sortingID,
                                WaybillID = waybillID,
                                Quantity = quantity,
                                AdminID = adminID,
                                CreateDate = DateTime.Now,
                            });

                            reponsitory.Insert(new Layers.Data.Sqls.PvWms.Storages
                            {
                                ID = PKeySigner.Pick(PkeyType.Storages),
                                Type = (int)CgStoragesType.Staging, //暂存库,
                                WareHouseID = warehouseID,
                                SortingID = sortingID,
                                Total = quantity,
                                Quantity = quantity,
                                ProductID = productid,
                                CreateDate = DateTime.Now,
                                Status = (int)GeneralStatus.Normal,
                                ShelveID = shelveID,
                                Supplier = supplier,
                                Origin = origin,
                                DateCode = datecode,
                            });
                        }
                    }
                    #endregion

                    #endregion
                }
                else
                {
                    #region 修改Waybill
                    contact = contact == null ? "" : contact;
                    consignorID = string.Concat(supplier, place, "", contact, phone, "", "").MD5();

                    if (!pvcenterReponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Any(item => item.ID == consignorID))
                    {
                        // 新增发货人
                        pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayParters
                        {
                            ID = consignorID,
                            Company = supplier,
                            Place = place,
                            Address = "",
                            Contact = contact,
                            Phone = phone,
                            Zipcode = "",
                            Email = "",
                            CreateDate = DateTime.Now,
                            IDType = null,
                            IDNumber = null,
                        });
                    }

                    // 修改
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                    {
                        ID = waybillID,
                        Code = waybill["Code"]?.Value<string>(),
                        Type = waybill["Type"].Value<int>(),
                        ConsignorID = consignorID,
                        FreightPayer = freightPayer.HasValue ? freightPayer.Value : (int)WaybillPayer.Consignor,
                        ModifyDate = DateTime.Now,
                        EnterCode = waybill["EnterCode"].Value<string>(),
                        Status = (int)GeneralStatus.Normal,
                        ModifierID = adminID,
                        ExcuteStatus = (int)CgTempStockExcuteStatus.TempStock,
                        Summary = waybill["Summary"].Value<string>(),
                        Supplier = waybill["Supplier"].Value<string>(),
                        OrderID = waybill["OrderID"].Value<string>(),
                        CarrierID = waybill["CarrierID"].Value<string>(),
                        Source = (int)CgNoticeSource.AgentEnter,
                        NoticeType = (int)CgNoticeType.Enter,
                    }, item => item.ID == waybillID);

                    var files = waybill["Files"];

                    if (files.Count() > 0)
                    {
                        List<string> fileids = new List<string>();
                        foreach (var file in files)
                        {
                            fileids.Add(file["ID"].Value<string>());
                        }

                        pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                        {
                            WaybillID = waybillID,
                            WsOrderID = waybill["OrderID"].Value<string>(),
                        }, item => fileids.Contains(item.ID));
                    }
                    #endregion

                    #region 修改Sorting及Storage

                    #region 描述库存,根据新要求去掉, 重新启用用于PDA

                    if (summaryStorages.Count() > 0)
                    {
                        foreach (var storage in summaryStorages)
                        {
                            var summary = storage["Summary"].Value<string>();
                            var sortingID = storage["SortingID"].Value<string>();
                            var storageID = storage["StorageID"].Value<string>();

                            if (string.IsNullOrEmpty(sortingID))
                            {
                                sortingID = PKeySigner.Pick(PkeyType.Sortings);
                                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Sortings
                                {
                                    ID = sortingID,
                                    WaybillID = waybillID,
                                    Quantity = 0,
                                    AdminID = adminID,
                                    CreateDate = DateTime.Now,
                                });
                            }

                            if (string.IsNullOrEmpty(storageID))
                            {
                                storageID = PKeySigner.Pick(PkeyType.Storages);
                                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Storages
                                {
                                    ID = storageID,
                                    Type = (int)CgStoragesType.Staging, //暂存库,
                                    WareHouseID = warehouseID,
                                    SortingID = sortingID,
                                    ProductID = "",
                                    Quantity = 0,
                                    CreateDate = DateTime.Now,
                                    Status = (int)GeneralStatus.Normal,
                                    ShelveID = shelveID,
                                    Supplier = supplier,
                                    Summary = summary,
                                });
                            }
                            else
                            {
                                reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                                {
                                    ShelveID = shelveID,
                                    Summary = summary,
                                    Supplier = supplier,
                                }, item => item.ID == storageID);
                            }

                        }
                    }

                    #endregion

                    #region 产品库存
                    if (productStorages.Count() > 0)
                    {
                        foreach (var product in productStorages)
                        {
                            var sortingID = product["SortingID"].Value<string>();
                            var storageID = product["StorageID"].Value<string>();
                            var quantity = product["Quantity"].Value<decimal>();
                            var datecode = product["DateCode"].Value<string>();
                            var origin = product["Origin"].Value<string>();

                            // 保存对应的Product                           
                            string productid = EnterProduct(product["Product"]);

                            if (string.IsNullOrEmpty(sortingID))
                            {
                                sortingID = PKeySigner.Pick(PkeyType.Sortings);
                                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Sortings
                                {
                                    ID = sortingID,
                                    WaybillID = waybillID,
                                    Quantity = quantity,
                                    AdminID = adminID,
                                    CreateDate = DateTime.Now,
                                });
                            }

                            if (string.IsNullOrEmpty(storageID))
                            {
                                storageID = PKeySigner.Pick(PkeyType.Storages);
                                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Storages
                                {
                                    ID = PKeySigner.Pick(PkeyType.Storages),
                                    Type = (int)CgStoragesType.Staging, //暂存库,
                                    WareHouseID = warehouseID,
                                    SortingID = sortingID,
                                    ProductID = productid,
                                    Total = quantity,
                                    Quantity = quantity,
                                    CreateDate = DateTime.Now,
                                    Status = (int)GeneralStatus.Normal,
                                    ShelveID = shelveID,
                                    Supplier = supplier,
                                    DateCode = datecode,
                                    Origin = origin,
                                });
                            }
                            else
                            {
                                reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                                {
                                    ShelveID = shelveID,
                                    Quantity = quantity,
                                    ProductID = productid,
                                    Supplier = supplier,
                                    DateCode = datecode,
                                    Origin = origin,
                                    ModifyDate = DateTime.Now,
                                }, item => item.ID == storageID);
                            }
                        }
                    }
                    #endregion

                    #region 删除无用的Sorting， Storage

                    #endregion
                    if (delete.Count() > 0)
                    {
                        foreach (var deleteItem in delete)
                        {
                            var sortingID = deleteItem["SortingID"].Value<string>();
                            var storageID = deleteItem["StorageID"].Value<string>();
                            reponsitory.Delete<Layers.Data.Sqls.PvWms.Storages>(item => item.ID == storageID);
                            reponsitory.Delete<Layers.Data.Sqls.PvWms.Sortings>(item => item.ID == sortingID);
                        }
                    }
                    #endregion
                }
            }

        }

        string EnterProduct(JToken product)
        {
            string partNumber = product["PartNumber"]?.Value<string>();
            string manufacturer = product["Manufacturer"]?.Value<string>();
            string packageCase = product["PackageCase"]?.Value<string>();
            string packaging = product["Packaging"]?.Value<string>();
            string productid = string.Concat(partNumber, manufacturer, packageCase, packaging).MD5();

            using (var reponsitory = new PvDataReponsitory())
            {
                var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvData.Products>().Any(item => item.ID == productid);
                if (!exist)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvData.Products
                    {
                        ID = productid,
                        PartNumber = partNumber,
                        Manufacturer = manufacturer,
                        PackageCase = packageCase,
                        Packaging = packaging,
                        CreateDate = DateTime.Now,
                    });
                }
            }

            return productid;
        }

        /// <summary>
        /// 获取库房
        /// </summary>
        /// <returns></returns>
        public object GetWarehouseInfos()
        {
            var results = new List<object>();
            results.Add(new
            {
                ID = ConfigurationManager.AppSettings["TempWarehouseIDForHK"],
                Name = ConfigurationManager.AppSettings["TempWarehouseNameForHK"]
            });

            return results.ToArray();
        }

        /// <summary>
        /// 返回WaybillID
        /// </summary>
        /// <returns></returns>
        public string GetWaybillID()
        {
            return PKeySigner.Pick(PkeyType.Waybills);
        }

        #region 搜索方法

        string wareHouseID;
        /// <summary>
        /// 根据库房ID搜索
        /// </summary>
        /// <param name="wareHouseID"></param>
        /// <returns></returns>
        public CgTempStocksView SearchByWareHouseID(string wareHouseID)
        {
            this.wareHouseID = wareHouseID;

            var waybillView = this.IQueryable.Cast<MyWaybill>();

            if (string.IsNullOrWhiteSpace(this.wareHouseID))
            {
                return new CgTempStocksView(this.Reponsitory, waybillView);
            }

            var linq_waybillIDs = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                  join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                  on storage.SortingID equals sorting.ID
                                  join waybill in waybillView on sorting.WaybillID equals waybill.ID
                                  where storage.WareHouseID.Contains(wareHouseID) && storage.Type == (int)CgStoragesType.Staging                                  
                                  select waybill.ID;
            var linq_ids = linq_waybillIDs.Distinct();

            var linq = from waybill in waybillView
                       join id in linq_ids on waybill.ID equals id
                       orderby waybill.ID descending
                       select waybill;
            var view = new CgTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID
            };
            return view;
        }

        /// <summary>
        /// 根据运单号搜索已有暂存运单
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public CgTempStocksView SearchByWaybillCode(string code)
        {
            var waybillView = IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.Code == code
                       select waybill;
            var view = new CgTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据运单ID搜索已有暂存运单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CgTempStocksView SearchByWaybillID(string id)
        {
            var waybillView = IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.ID == id
                       select waybill;
            var view = new CgTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据承运商搜索已有暂存运单
        /// </summary>
        /// <param name="carrierID"></param>
        /// <returns></returns>
        public CgTempStocksView SearchByCarrier(string carrierID)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.CarrierID == carrierID
                       select waybill;

            var view = new CgTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据库位ID来搜索已有运单
        /// </summary>
        /// <param name="shelveID"></param>
        /// <returns></returns>
        public CgTempStocksView SearchByShelveID(string shelveID)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq_waybillIDs = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                  join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                  on storage.SortingID equals sorting.ID
                                  join waybill in waybillView on sorting.WaybillID equals waybill.ID
                                  where storage.ShelveID == shelveID
                                  select waybill.ID;
            var linq_ids = linq_waybillIDs.Distinct().Take(500);

            var linq = from waybill in waybillView
                       join id in linq_ids on waybill.ID equals id
                       orderby waybill.ID descending
                       select waybill;
            var view = new CgTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID
            };
            return view;
        }

        /// <summary>
        /// 根据运单录入时间搜索已有暂存运单
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public CgTempStocksView SearchByCreateTime(DateTime start, DateTime end)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.CreateDate >= start && waybill.CreateDate < end.AddDays(1)
                       select waybill;
            var view = new CgTempStocksView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据型号搜索已有暂存运单
        /// </summary>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        public CgTempStocksView SearchByPartNumber(string partNumber)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on waybill.ID equals sorting.WaybillID
                       join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on sorting.ID equals storage.SortingID
                       join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
                       where storage.Type == (int)CgStoragesType.Staging && product.PartNumber == partNumber
                       select waybill;
            var view = new CgTempStocksView(this.Reponsitory, linq.Distinct())
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }
        #endregion

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">压缩后保存图片地址</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="sfsc">是否是第一次调用</param>
        /// <returns></returns>
        public bool CompressImage(string sFile, string dFile, int flag = 90, int size = 150, bool sfsc = true)
        {
            //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
            FileInfo firstFileInfo = new FileInfo(sFile);
            if (sfsc == true && firstFileInfo.Length < size * 1024)
            {
                firstFileInfo.CopyTo(dFile);
                //firstFileInfo.Delete();
                return true;
            }
            Image iSource = Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;
            int dHeight = iSource.Height / 2;
            int dWidth = iSource.Width / 2;
            int sW = 0, sH = 0;
            //按比例缩放
            Size tem_size = new Size(iSource.Width, iSource.Height);
            if (tem_size.Width > dHeight || tem_size.Width > dWidth)
            {
                if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }

            Bitmap ob = new Bitmap(dWidth, dHeight);
            Graphics g = Graphics.FromImage(ob);

            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

            g.Dispose();

            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;

            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                    FileInfo fi = new FileInfo(dFile);
                    if (fi.Length > 1024 * size)
                    {
                        flag = flag - 10;
                        CompressImage(sFile, dFile, flag, size, false);
                    }
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }


        }

        public string UploadFile(string sFile, string adminID, string WaybillID)
        {
            //string address = $"{UploadUrl}?WaybillID={WaybillID}&AdminID={adminID}&Type=8000";
            string uploadUrl = CenterFile.Web + "api/Upload";
            string address = $"{uploadUrl}?WaybillID={WaybillID}&AdminID={adminID}&Type=8000";
            using (WebClient client = new WebClient())
            {
                var responseStr = client.UploadFile(new Uri(address), "POST", sFile);
                var result = System.Text.Encoding.ASCII.GetString(responseStr);
                return result;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public string DeleteFile(string fileID)
        {
            string uploadUrl = CenterFile.Web + "api/Upload";
            string url = $"{uploadUrl}/DeleteFile?FileID={fileID}";
            var result = ApiHelper.Current.JPost(url);
            return result;
        }

        #region Helper Class

        private class MyWaybill
        {
            public string ID { get; set; }
            public string Code { get; set; } // 运单号
            public string EnterCode { get; set; }
            public string ClientName { get; set; }
            public string Supplier { get; set; }
            public CgTempStockExcuteStatus ExcuteStatus { get; set; }
            public WaybillType Type { get; set; }
            public string CarrierID { get; set; }
            public string CarrierName { get; set; }
            public WayParter Consignor { get; set; }
            public string ConsigneeID { get; set; }
            public Origin? Place { get; set; }
            public WaybillPayer? FreightPayer { get; set; } // 运费支付人, 如果没填怎么处理
            public string OrderID { get; set; }
            public CgNoticeSource Source { get; set; }
            public CgNoticeType NoticeType { get; set; }
            public DateTime CreateDate { get; set; }
            public string Summary { get; set; }
            public string TempEnterCode { get; set; }
            public string[] Files { get; set; }
        }

        public class FileDescrition
        {
            public string SessionID { get; set; }
            public string FileID { get; set; }
            public string Url { get; set; }
            public string FileName { get; set; }
        }
        #endregion
    }
    
}
