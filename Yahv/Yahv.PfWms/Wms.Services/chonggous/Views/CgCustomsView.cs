//using Layers.Data;
//using Layers.Data.Sqls;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Wms.Services.Enums;
//using Yahv.Linq;
//using Yahv.Services.Enums;
//using Yahv.Services.Models;
//using Yahv.Underly;
//using Yahv.Utils.Converters.Contents;
//using Yahv.Utils.Serializers;
////using static Wms.Services.chonggous.Journals;

//namespace Wms.Services.chonggous.Views
//{
//    /// <summary>
//    /// 报关视图(代报关，转报关)
//    /// </summary>

//    [Obsolete("再商议")]
//    public class CgCustomsView : QueryView<object, PvWmsRepository>
//    {
//        #region 构造函数
//        public CgCustomsView()
//        {
//        }

//        protected CgCustomsView(PvWmsRepository reponsitory)
//        {
//        }

//        protected CgCustomsView(PvWmsRepository reponsitory, IQueryable<object> iQueryable)
//        {
//        }
//        #endregion

//        protected override IQueryable<object> GetIQueryable()
//        {
//            var clientView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>();
//            var waybillViews = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>();
//            var carriersTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>();

//            var linqs = from waybill in waybillViews
//                        join _client in clientView on waybill.wbEnterCode equals _client.EnterCode into clients
//                        from client in clients.DefaultIfEmpty()
//                        join _carrier in carriersTopView on waybill.wbCarrierID equals _carrier.ID into carriers
//                        from carrier in carriers.DefaultIfEmpty()
//                        orderby waybill.wbCreateDate descending
//                        select new MyWaybill
//                        {
//                            ID = waybill.wbID,
//                            CreateDate = waybill.wbCreateDate,
//                            EnterCode = waybill.wbEnterCode,
//                            ClientName = client == null ? null : client.Name,
//                            Supplier = waybill.wbSupplier,
//                            ExcuteStatus = (Yahv.Underly.CgSortingExcuteStatus)waybill.wbExcuteStatus,
//                            Type = (WaybillType)waybill.wbType,
//                            Code = waybill.wbCode,
//                            CarrierName = carrier.Name,
//                            CarrierID = waybill.wbCarrierID,
//                            ConsignorID = waybill.wbConsignorID,
//                            Place = (Origin)Enum.Parse(typeof(Origin), waybill.corPlace),
//                            ConsignorPlace = waybill.corPlace,
//                            OrderID = waybill.OrderID,
//                            Source = (CgNoticeSource)waybill.Source,
//                            NoticeType = (CgNoticeType)waybill.NoticeType,
//                            AppointTime = waybill.AppointTime,
//                            TransferID = waybill.wbTransferID,
//                            Driver = waybill.wldDriver,
//                            CarNumber1 = waybill.wldCarNumber1,
//                            LoadingExcuteStatus = (CgLoadingExcuteStauts?)waybill.loadExcuteStatus,
//                        };
//            return linqs;
//        }

//        /// <summary>
//        /// 补全数据
//        /// </summary>
//        /// <returns></returns>
//        public object[] ToMyArray()
//        {
//            return this.ToMyPage() as object[];
//        }

//        /// <summary>
//        /// 分页方法
//        /// </summary>
//        /// <param name="pageIndex"></param>
//        /// <param name="pageSize"></param>
//        /// <returns></returns>
//        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
//        {
//            var iquery = this.IQueryable.Cast<MyWaybill>();
//            int total = iquery.Count();

//            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
//            {
//                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
//            }

//            // 获取符合条件的ID            
//            var ienum_myWaybill = iquery.ToArray().OrderBy(item => item.Type).ThenByDescending(item => item.CreateDate);
//            var waybillIds = ienum_myWaybill.Select(item => item.ID).Distinct();

//            var productView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>();

//            var noticeView = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
//                             join product in productView on notice.ProductID equals product.ID
//                             where waybillIds.Contains(notice.WaybillID)
//                             select new
//                             {
//                                 Product = new
//                                 {
//                                     PartNumber = product.PartNumber,
//                                     Manufacturer = product.Manufacturer,
//                                     PackageCase = product.PackageCase,
//                                     Packaging = product.Packaging,
//                                 },
//                                 notice.ID,
//                                 notice.WaybillID,
//                                 notice.InputID,
//                                 notice.DateCode,
//                                 notice.Quantity,
//                                 notice.Origin,
//                                 Conditions = notice.Conditions,
//                                 Source = (NoticeSource)notice.Source,
//                                 Type = (CgNoticeType)notice.Type,
//                                 notice.BoxCode,
//                                 notice.Weight,
//                                 notice.NetWeight,
//                                 notice.Volume,
//                                 notice.ShelveID,
//                                 notice.Summary,
//                             };

//            var ienum_notices = noticeView.ToArray();
//            var inputIds = ienum_notices.Select(item => item.InputID).Distinct();
//            var noticeIds = ienum_notices.Select(item => item.ID).Distinct();

//            #region 文件处理

//            var filesView = from file in new Yahv.Services.Views.CenterFilesTopView()
//                            where waybillIds.Contains(file.WaybillID)
//                                || noticeIds.Contains(file.NoticeID)
//                            select new CenterFileDescription
//                            {
//                                ID = file.ID,
//                                WaybillID = file.WaybillID,
//                                NoticeID = file.NoticeID,
//                                StorageID = file.StorageID,
//                                CustomName = file.CustomName,
//                                Type = file.Type,
//                                Url = CenterFile.Web + file.Url,
//                                CreateDate = file.CreateDate,
//                                ClientID = file.ClientID,
//                                AdminID = file.AdminID,
//                                InputID = file.InputID,
//                                Status = file.Status,
//                            };
//            var files = filesView.ToArray();

//            #endregion

//            var sortingView = from sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
//                              join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
//                              on sorting.InputID equals input.ID
//                              join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
//                              on sorting.ID equals storage.SortingID
//                              join product in productView on storage.ProductID equals product.ID
//                              where noticeIds.Contains(sorting.NoticeID)
//                              select new
//                              {
//                                  sorting.ID,
//                                  sorting.BoxCode,
//                                  sorting.Quantity,

//                                  sorting.AdminID,
//                                  sorting.CreateDate,
//                                  sorting.Weight,
//                                  sorting.NetWeight,
//                                  sorting.Volume,
//                                  sorting.InputID,
//                                  sorting.Summary,
//                                  input = new
//                                  {
//                                      ID = input.ID,
//                                      input.OrderID,
//                                      input.TinyOrderID,
//                                      input.ItemID,
//                                  },
//                                  Product = new
//                                  {
//                                      PartNumber = product.PartNumber,
//                                      Manufacturer = product.Manufacturer,
//                                      PackageCase = product.PackageCase,
//                                      Packaging = product.Packaging,
//                                  },
//                              };

//            var ienum_sortings = sortingView.ToArray();

//            var sortingWithNoNoticeView = from sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
//                                          join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
//                                          on sorting.InputID equals input.ID
//                                          join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
//                                          on sorting.ID equals storage.SortingID
//                                          join product in productView on storage.ProductID equals product.ID
//                                          where waybillIds.Contains(sorting.WaybillID) || sorting.NoticeID == null
//                                          select new
//                                          {
//                                              sorting.ID,
//                                              sorting.BoxCode,
//                                              sorting.NoticeID,
//                                              sorting.Quantity,
//                                              sorting.WaybillID,
//                                              sorting.AdminID,
//                                              sorting.CreateDate,
//                                              sorting.Weight,
//                                              sorting.NetWeight,
//                                              sorting.Volume,
//                                              sorting.InputID,
//                                              sorting.Summary,
//                                              input = new
//                                              {
//                                                  ID = input.ID,
//                                                  input.OrderID,
//                                                  input.TinyOrderID,
//                                                  input.ItemID,
//                                              },
//                                              Product = new
//                                              {
//                                                  PartNumber = product.PartNumber,
//                                                  Manufacturer = product.Manufacturer,
//                                                  PackageCase = product.PackageCase,
//                                                  Packaging = product.Packaging,
//                                              },
//                                          };

//            var ienum_sortingswithnonotice = sortingWithNoNoticeView.ToArray();

//            var ienum_noticesorting = from notice in ienum_notices
//                                      join sorting in ienum_sortings on notice.InputID equals sorting.InputID into sortings
//                                      select new
//                                      {
//                                          Product = notice.Product,
//                                          notice.ID,
//                                          notice.WaybillID,
//                                          notice.InputID,
//                                          notice.DateCode,
//                                          notice.Type,
//                                          notice.Quantity,
//                                          notice.Conditions,
//                                          Source = notice.Source,
//                                          notice.BoxCode,
//                                          notice.Weight,
//                                          notice.NetWeight,
//                                          notice.Volume,
//                                          notice.ShelveID,
//                                          notice.Summary,
//                                          Origin = (Origin)Enum.Parse(typeof(Origin), notice.Origin ?? nameof(Origin.Unknown)),
//                                          Sortings = sortings.ToArray(),
//                                      };

//            var linq = from waybill in ienum_myWaybill
//                       join notice in ienum_noticesorting on waybill.ID equals notice.WaybillID into notices
//                       join sorting in ienum_sortingswithnonotice on waybill.ID equals sorting.WaybillID into sortings
//                       select new
//                       {
//                           Waybill = new
//                           {
//                               ID = waybill.ID,
//                               CreateDate = waybill.CreateDate,
//                               EnterCode = waybill.EnterCode,
//                               ClientName = waybill.ClientName,
//                               Supplier = waybill.Supplier,
//                               ExcuteStatus = waybill.ExcuteStatus,
//                               ExcuteStatusDes = waybill.ExcuteStatus.GetDescription(),
//                               Type = waybill.Type,
//                               TypeDes = waybill.Type.GetDescription(),
//                               Code = waybill.Code,
//                               CarrierID = waybill.CarrierID,
//                               CarrierName = waybill.CarrierName,
//                               ConsignorID = waybill.ConsignorID,
//                               ConsignorPlace = waybill.ConsignorPlace,
//                               waybill.Place,
//                               Source = waybill.Source,
//                               OrderID = waybill.OrderID,
//                               NoticeType = waybill.NoticeType,
//                               Driver = waybill.Driver,
//                               CarNumber1 = waybill.CarNumber1,
//                               AppointTime = waybill.AppointTime,
//                               TransferID = waybill.TransferID,
//                               LoadingExcuteStatus = waybill.LoadingExcuteStatus,
//                           },
//                           Notice = notices.ToArray(),
//                           Sorting = sortings.ToArray(),
//                       };

//            // 为了计算并添加LQuantity
//            var results = linq.Select(item => new
//            {
//                Waybill = new
//                {
//                    ID = item.Waybill.ID,
//                    CreateDate = item.Waybill.CreateDate,
//                    EnterCode = item.Waybill.EnterCode,
//                    ClientName = item.Waybill.ClientName,
//                    Supplier = item.Waybill.Supplier,
//                    ExcuteStatus = item.Waybill.ExcuteStatus,
//                    ExcuteStatusDes = item.Waybill.ExcuteStatus.GetDescription(),
//                    Type = item.Waybill.Type,
//                    TypeDes = item.Waybill.Type.GetDescription(),
//                    Code = item.Waybill.Code,
//                    CarrierID = item.Waybill.CarrierID,
//                    CarrierName = item.Waybill.CarrierName,
//                    ConsignorID = item.Waybill.ConsignorID,
//                    ConsignorPlace = item.Waybill.ConsignorPlace,
//                    ConsignorPlaceDes = item.Waybill.Place.GetDescription(),
//                    ConsignorPlaceText = item.Waybill.ConsignorPlace + " " + item.Waybill.Place.GetDescription(),
//                    ConsignorPlaceID = ((int)item.Waybill.Place).ToString(),
//                    Source = item.Waybill.Source,
//                    SourceDes = item.Waybill.Source.GetDescription(),
//                    NoticeType = item.Waybill.NoticeType,
//                    NoticeTypeDes = item.Waybill.NoticeType.GetDescription(),
//                    Driver = item.Waybill.Driver,
//                    CarNumber1 = item.Waybill.CarNumber1,
//                    AppointTime = item.Waybill.AppointTime,
//                    TransferID = item.Waybill.TransferID,
//                    LoadingExcuteStatus = item.Waybill.LoadingExcuteStatus,
//                    OrderID = item.Waybill.OrderID ?? string.Join(",", item.Notice.SelectMany(notice => notice.Sortings)
//                        .Select(sorting => sorting.input.OrderID).Distinct()),
//                    Files = files,
//                },
//                Notice = item.Notice.Select(notice => new
//                {
//                    notice.ID,
//                    NoticeID = notice.ID,
//                    notice.Product,
//                    notice.WaybillID,
//                    notice.InputID,
//                    notice.DateCode,
//                    notice.Quantity,
//                    ArrivedQuantity = notice.Sortings.Sum(s => s.Quantity),
//                    LeftQuantity = notice.Quantity - notice.Sortings.Sum(s => s.Quantity),
//                    CurrentQuantity = notice.Quantity - notice.Sortings.Sum(s => s.Quantity),
//                    _disabled = (notice.Quantity - notice.Sortings.Sum(s => s.Quantity)) == 0 ? true : false,
//                    Conditions = notice.Conditions.JsonTo<NoticeCondition>(),
//                    Source = notice.Source,
//                    notice.BoxCode,
//                    notice.Weight,
//                    notice.NetWeight,
//                    notice.Volume,
//                    notice.ShelveID,
//                    notice.Type,
//                    origintext = notice.Origin.ToString() + " " + notice.Origin.GetDescription(),
//                    originID = ((int)notice.Origin).ToString(),
//                    originDes = notice.Origin.GetDescription(),
//                    notice.Summary,
//                    SortingID = string.Empty,
//                    Files = files.Where(file => file.InputID == notice.InputID).ToArray(),
//                    Sortings = notice.Sortings.Select(sorting => new
//                    {
//                        sorting.ID,
//                        sorting.BoxCode,
//                        sorting.Quantity,
//                        sorting.AdminID,
//                        sorting.CreateDate,
//                        sorting.Weight,
//                        sorting.NetWeight,
//                        sorting.Volume,
//                        sorting.InputID,
//                        input = sorting.input,
//                        Product = sorting.Product,
//                        sorting.Summary,
//                    }),
//                }),
//                Sortings = item.Sorting.Select(sorting => new
//                {
//                    sorting.WaybillID,
//                    sorting.NoticeID, // null
//                    sorting.BoxCode,
//                    sorting.Quantity,
//                    sorting.AdminID,
//                    sorting.CreateDate,
//                    sorting.Weight,
//                    sorting.NetWeight,
//                    sorting.Volume,
//                    sorting.InputID,
//                    sorting.input,
//                    sorting.Product,
//                    sorting.Summary,
//                })
//            });

//            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
//            {
//                return results.Select(item =>
//                {
//                    object o = item;
//                    return o;
//                }).ToArray();
//            }

//            return new
//            {
//                Total = total,
//                Size = pageSize ?? 20,
//                Index = pageIndex ?? 1,
//                Data = results.ToArray(),
//            };
//        }

//        /// <summary>
//        /// 报关处理
//        /// </summary>
//        /// <param name="jobject"></param>
//        [Obsolete("")]
//        public void Enter(JObject jobject)
//        {
//            var waybill = jobject["Waybill"];
//            var sortings = jobject["Sortings"];
//            var waybillorderid = waybill["OrderID"].Value<string>();

//            // bool updateOrder = false; // 是否发生异常入库,需要更新Waybill对应的订单
//            string waybillID = waybill["ID"].Value<string>();
//            string adminID = jobject["AdminID"].Value<string>();

//            // 当前运单的执行状态
//            var waybillExcuteStatus = (CgSortingExcuteStatus)waybill["ExcuteStatus"].Value<int>();

//            // 获取必要的信息
//            var noticeView = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
//                             join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on notice.ProductID equals product.ID
//                             join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID
//                             where notice.WaybillID == waybillID
//                             select new
//                             {
//                                 input.OrderID,
//                                 input.TinyOrderID,
//                                 input.ItemID,
//                                 InputID = input.ID,
//                                 input.ClientID,
//                                 input.SalerID,
//                                 input,
//                                 notice,
//                                 product = product,
//                                 notice.WareHouseID,
//                                 NoticeID = notice.ID,
//                                 notice.Supplier,
//                                 notice.Conditions,
//                                 notice.BoxCode,
//                                 notice.Quantity,
//                             };

//            var ienum_notices = noticeView.ToArray();


//            var source = (CgNoticeSource)waybill["Source"].Value<int>();
//            var noticeType = (CgNoticeType)waybill["NoticeType"].Value<int>();

//            #region 代报关处理
//            if (noticeType == CgNoticeType.Enter && (source == CgNoticeSource.AgentBreakCustoms))
//            {

//            }
//            #endregion

//            #region 转报关处理
//            if (noticeType == CgNoticeType.Boxing && source == CgNoticeSource.AgentCustomsFromStorage)
//            {
//                //处理保存 waybill
//                EnterWaybill(waybill, adminID, null);

//                using (var reponsitory = new PvWmsRepository())
//                {
//                    var storageView = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>();

//                    foreach (var sorting in sortings)
//                    {
//                        #region 处理Sorting
//                        string productID = EnterProduct(sorting["Product"]);
//                        string noticeID = sorting["NoticeID"].Value<string>();
//                        var noticeEntity = ienum_notices.Where(item => item.NoticeID == noticeID).First();
//                        var inputID = noticeEntity.InputID;
//                        var boxcode = sorting["BoxCode"].Value<string>();

//                        var sortingId = sorting["SortingID"]?.Value<string>();
//                        sortingId = EnterSorting(sorting, sortingId, inputID, noticeID, adminID, waybillID, reponsitory);
//                        #endregion

//                        // 查找出来在生成Boxing通知时的流水库StorageID
//                        var storageWithFlow = storageView.Single(item => item.InputID == inputID && item.Type == (int)CgStoragesType.Flows && item.Quantity == noticeEntity.Quantity);

//                        #region 新建Picking
//                        reponsitory.Insert(new Layers.Data.Sqls.PvWms.Pickings
//                        {
//                            ID = PKeySigner.Pick(PkeyType.Pickings),
//                            StorageID = storageWithFlow.ID, //生成的流水库的storageID
//                            NoticeID = noticeID,
//                            OutputID = noticeEntity.notice.OutputID, //生成的OutputID
//                            BoxCode = boxcode,
//                            Quantity = noticeEntity.Quantity,
//                            AdminID = adminID,
//                            CreateDate = DateTime.Now,
//                            Weight = sorting["Weight"].Value<decimal>(),
//                            NetWeight = sorting["NetWeight"].Value<decimal>(),
//                            Volume = sorting["Volume"].Value<decimal>(),
//                            Summary = sorting["Summary"].Value<string>(),
//                        });
//                        #endregion

//                        #region 更新流水库存
//                        #endregion

//                        #region 发送申报日志
//                        // 箱号检查,不能为空
//                        if (!string.IsNullOrEmpty(boxcode))
//                        {
//                            // 以小订单为基础
//                            bool existLogDeclare = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>().Any(item => item.TinyOrderID == noticeEntity.TinyOrderID);
//                            if (!existLogDeclare)
//                            {
//                                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Declare
//                                {
//                                    ID = PKeySigner.Pick(PkeyType.LogsDeclare),
//                                    TinyOrderID = noticeEntity.TinyOrderID,
//                                    //WaybillID = waybillID,
//                                    EnterCode = waybill["EnterCode"].Value<string>(),
//                                    //BoxCode = boxcode,
//                                    CreateDate = DateTime.Now,
//                                    UpdateDate = DateTime.Now,
//                                    Status = (int)TinyOrderDeclareStatus.Boxed,
//                                });
//                            }

//                            // 以小订单TinyOrderID与库存信息为基础, 需要与陈经理确认
//                            bool existLogDeclareItem = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>().Any(item => item.OrderItemID == noticeEntity.input.ItemID && item.StorageID == noticeEntity.notice.StorageID);
//                            if (!existLogDeclareItem)
//                            {
//                                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_DeclareItem
//                                {
//                                    ID = PKeySigner.Pick(PkeyType.LogsDeclareItem),
//                                    TinyOrderID = noticeEntity.input.TinyOrderID,
//                                    OrderItemID = noticeEntity.input.ItemID,
//                                    StorageID = storageWithFlow.ID, //noticeEntity.notice.StorageID,
//                                    Quantity = sorting["Quantity"].Value<int>(),
//                                    AdminID = adminID,
//                                    OutputID = noticeEntity.notice.OutputID,// 装箱通知中的OutputID,
//                                });
//                                // 如果已经存在的话，需要更新吗，怎样更新对应的数量
//                            }
//                        }
//                        #endregion

//                        // 添加转报关操作日志
//                        //Log(waybillID, waybillorderid, noticeEntity.TinyOrderID, noticeEntity.ItemID, sortingId, null, storageWithFlow.ID, $"{source.GetDescription()} {noticeType.GetDescription()} 操作成功, 订单 {waybillExcuteStatus.GetDescription()}", adminID);

//                    }
//                }
//            }
//            #endregion
//        }

//        #region Helper Method

//        /// <summary>
//        /// 专门用于修改waybill 信息
//        /// </summary>
//        void EnterWaybill(JToken waybill, string adminID, CgSortingExcuteStatus? excuteStatus)
//        {
//            using (var reponsitory = new PvCenterReponsitory())
//            {
//                if (excuteStatus == null)
//                {
//                    excuteStatus = (CgSortingExcuteStatus)(waybill["ExcuteStatus"]?.Value<int>() ?? (int)CgSortingExcuteStatus.PartStocked);
//                }


//                //处理 waybill
//                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
//                {
//                    ExcuteStatus = (int)excuteStatus,
//                    Summary = waybill["Summary"]?.Value<string>(),
//                    Code = waybill["Code"]?.Value<string>(),
//                    CarrierID = waybill["CarrierID"]?.Value<string>(),
//                    ModifyDate = DateTime.Now,
//                    ModifierID = adminID,
//                    TransferID = waybill["TransferID"]?.Value<string>(),
//                }, item => item.ID == waybill["ID"].Value<string>());

//                reponsitory.Update<Layers.Data.Sqls.PvCenter.WayChcd>(new
//                {
//                    Driver = waybill["Driver"]?.Value<string>(),
//                    CarrNumber1 = waybill["CarNumber1"]?.Value<string>(),
//                }, item => item.ID == waybill["ID"].Value<string>());

//                //更新ConsignorPlace value 
//                reponsitory.Update<Layers.Data.Sqls.PvCenter.WayParters>(new
//                {
//                    Place = waybill["ConsignorPlace"].Value<string>(),
//                }, item => item.ID == waybill["ConsignorID"].Value<string>());

//                // 更新Files信息
//                var files = waybill["Files"].Values<string>();
//                if (files.Count() > 0)
//                {
//                    reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
//                    {
//                        WaybillID = waybill["ID"].Value<string>(),
//                        WsOrderID = waybill["OrderID"].Value<string>(),
//                    }, item => files.Contains(item.ID));
//                }

//            }
//        }

//        /// <summary>
//        /// 保存产品信息
//        /// </summary>
//        /// <param name="product"></param>
//        /// <returns></returns>
//        string EnterProduct(JToken product)
//        {
//            string partNumber = product["PartNumber"]?.Value<string>();
//            string manufacturer = product["Manufacturer"]?.Value<string>();
//            string packageCase = product["PackageCase"]?.Value<string>();
//            string packaging = product["Packaging"]?.Value<string>();
//            string productid = string.Concat(partNumber, manufacturer, packageCase, packaging).MD5();

//            using (var reponsitory = new PvDataReponsitory())
//            {
//                var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvData.Products>().Any(item => item.ID == productid);
//                if (!exist)
//                {
//                    reponsitory.Insert(new Layers.Data.Sqls.PvData.Products
//                    {
//                        ID = productid,
//                        PartNumber = partNumber,
//                        Manufacturer = manufacturer,
//                        PackageCase = packageCase,
//                        Packaging = packaging,
//                        CreateDate = DateTime.Now,
//                    });
//                }
//            }

//            return productid;
//        }

//        /// <summary>
//        /// 保存Sorting信息
//        /// </summary>
//        /// <param name="sortings"></param>
//        /// <param name="reponsitory"></param>
//        /// <returns></returns>
//        string EnterSorting(JToken sorting, string sortingId, string inputId, string noticeID, string adminID, string waybillID, PvWmsRepository reponsitory)
//        {
//            bool existSorting = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Any(item => item.ID == sortingId);
//            if (!existSorting)
//            {
//                if (string.IsNullOrEmpty(sortingId))
//                {
//                    sortingId = PKeySigner.Pick(PkeyType.Sortings);
//                }

//                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Sortings
//                {
//                    ID = sortingId,
//                    BoxCode = sorting["BoxCode"]?.Value<string>(),
//                    Quantity = sorting["Quantity"].Value<decimal>(),
//                    AdminID = adminID,
//                    CreateDate = DateTime.Now,
//                    Weight = sorting["Weight"].Value<decimal>(),
//                    NetWeight = sorting["NetWeight"].Value<decimal>(),
//                    Volume = sorting["Volume"].Value<decimal>(),
//                    InputID = inputId,
//                    NoticeID = noticeID,
//                    WaybillID = waybillID,
//                    Summary = sorting["Summary"]?.Value<string>(),
//                });
//            }
//            else
//            {
//                reponsitory.Update<Layers.Data.Sqls.PvWms.Sortings>(new
//                {
//                    BoxCode = sorting["BoxCode"]?.Value<string>(),
//                    Quantity = sorting["Quantity"].Value<decimal>(),
//                    AdminID = sorting["AdminID"].Value<string>(),
//                    CreateDate = DateTime.Now,
//                    Weight = sorting["Weight"].Value<decimal>(),
//                    NetWeight = sorting["NetWeight"].Value<decimal>(),
//                    Volume = sorting["Volume"].Value<decimal>(),
//                    InputID = inputId,
//                    NoticeID = noticeID,
//                    WaybillID = waybillID,
//                    Summary = sorting["Summary"]?.Value<string>(),
//                }, item => item.ID == sortingId);
//            }

//            return sortingId;
//        }


//        /// <summary>
//        /// 添加操作日志
//        /// </summary>
//        /// <param name="waybillID"></param>
//        /// <param name="orderID"></param>
//        /// <param name="tinyOrderID"></param>
//        /// <param name="orderItemID"></param>
//        /// <param name="sortingID"></param>
//        /// <param name="pickingID"></param>
//        /// <param name="storageID"></param>
//        /// <param name="context"></param>
//        /// <param name="adminID"></param>
//        public void Log(string waybillID, string orderID, string tinyOrderID, string orderItemID, string sortingID, string pickingID, string storageID, string context, string adminID)
//        {
//            //var log = new EnterOperating
//            //{
//            //    WaybillID = waybillID,
//            //    OrderID = orderID,
//            //    TinyOrderID = tinyOrderID,
//            //    OrderItemID = orderItemID,
//            //    SortingID = sortingID,
//            //    PickingID = pickingID,
//            //    StorageID = storageID,
//            //    Context = context,
//            //    AdminID = adminID,
//            //};

//            //Journals.Current.Write(log);
//        }

//        #endregion

//        #region Helper Class
//        private class MyWaybill
//        {
//            public string ID { get; set; }
//            public DateTime CreateDate { get; set; }
//            public string EnterCode { get; set; }
//            public string ClientName { get; set; }
//            public string Supplier { get; set; }
//            public Yahv.Underly.CgSortingExcuteStatus ExcuteStatus { get; set; }
//            public Yahv.Underly.WaybillType Type { get; set; }
//            public string Code { get; set; }
//            public string CarrierID { get; set; }
//            public string CarrierName { get; set; }
//            public string ConsignorID { get; set; }
//            public string ConsignorPlace { get; set; }
//            public Origin Place { get; set; }
//            public string OrderID { get; set; }
//            public CgNoticeSource Source { get; set; }
//            public CgNoticeType NoticeType { get; set; }
//            public DateTime? AppointTime { get; set; }
//            public string TransferID { get; set; }
//            public string Driver { get; set; }
//            public string CarNumber1 { get; set; }
//            public CgLoadingExcuteStauts? LoadingExcuteStatus { get; set; }
//            /// <summary>
//            /// 
//            /// </summary>
//            /// <remarks>
//            /// 经过商议暂时增加，等与库房前端对通后，我们可以没有这个传递与处理了。
//            /// </remarks>
//            public string[] Files { get; set; }
//        }
//        #endregion
//    }
//}
