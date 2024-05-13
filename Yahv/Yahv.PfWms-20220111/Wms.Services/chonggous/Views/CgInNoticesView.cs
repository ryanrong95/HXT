using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.chonggous.Models;
using Wms.Services.Enums;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Wms.Services.chonggous.Views
{
    public class CgInNoticesView : CgNoticesView
    {
        public CgInNoticesView()
        {

        }

        protected override IQueryable<CgNotice> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Type == CgNoticeType.Enter);
        }

        /// <summary>
        /// 保存Input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        string EnterInput(JToken input, string productId, PvWmsRepository repository)
        {
            var inputId = input["ID"].Value<string>();
            var inputView = repository.ReadTable<Layers.Data.Sqls.PvWms.Inputs>();
            if (inputView.Any(item => item.ID == inputId))
            {
                repository.Update<Layers.Data.Sqls.PvWms.Inputs>(new
                {
                    ID = inputId,
                    Code = inputId,
                    OrderID = input["OrderID"]?.Value<string>(),
                    TinyOrderID = input["TinyOrderID"]?.Value<string>(),
                    ItemID = input["ItemID"]?.Value<string>(),
                    ProductID = productId,
                    ClientID = input["ClientID"]?.Value<string>(),
                    PayeeID = input["PayeeID"]?.Value<string>(),
                    ThirdID = input["ThirdID"]?.Value<string>(),
                    TrackerID = input["TrackerID"]?.Value<string>(),
                    SalerID = input["SalerID"]?.Value<string>(),
                    PurchaserID = input["PurchaserID"]?.Value<string>(),
                    Currency = input["Currency"]?.Value<int>(),
                    UnitPrice = input["UnitPrice"]?.Value<decimal>(),
                    CreateDate = DateTime.Now,
                }, item => item.ID == inputId);
            }
            else
            {
                if (string.IsNullOrEmpty(inputId))
                {
                    inputId = PKeySigner.Pick(PkeyType.Inputs);
                }

                repository.Insert(new Layers.Data.Sqls.PvWms.Inputs
                {
                    ID = inputId,
                    Code = inputId,
                    OrderID = input["OrderID"]?.Value<string>(),
                    TinyOrderID = input["TinyOrderID"]?.Value<string>(),
                    ItemID = input["ItemID"]?.Value<string>(),
                    ProductID = productId,
                    ClientID = input["ClientID"]?.Value<string>(),
                    PayeeID = input["PayeeID"]?.Value<string>(),
                    ThirdID = input["ThirdID"]?.Value<string>(),
                    TrackerID = input["TrackerID"]?.Value<string>(),
                    SalerID = input["SalerID"]?.Value<string>(),
                    PurchaserID = input["PurchaserID"]?.Value<string>(),
                    Currency = input["Currency"]?.Value<int>(),
                    UnitPrice = input["UnitPrice"]?.Value<decimal>(),
                    CreateDate = DateTime.Now,
                });
            }

            return inputId;
        }

        /// <summary>
        /// 保存Product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        string EnterProduct(JToken product)
        {
            string ID = string.Concat(product["PartNumber"]?.Value<string>(), product["Manufacturer"]?.Value<string>(), product["PackageCase"]?.Value<string>(), product["Packaging"]?.Value<string>()).MD5();

            using (var reponsitory = new PvDataReponsitory())
            {
                if (reponsitory.ReadTable<Layers.Data.Sqls.PvData.Products>().All(item => item.ID != ID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvData.Products
                    {
                        ID = ID,
                        PartNumber = product["PartNumber"]?.Value<string>(),
                        Manufacturer = product["Manufacturer"]?.Value<string>(),
                        PackageCase = product["PackageCase"]?.Value<string>(),
                        Packaging = product["Packaging"]?.Value<string>(),
                        CreateDate = DateTime.Now,
                    });
                }
            }
            return ID;
        }

        string EnterNotice(Layers.Data.Sqls.PvWms.Notices entity, PvWmsRepository repository)
        {
            #region 保存Notice
            string inputID = entity.InputID;
            string noticeID = string.Empty;
            if (!repository.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Any(item => item.InputID == inputID))
            {
                noticeID = PKeySigner.Pick(PkeyType.Notices);
                entity.ID = noticeID;
                entity.CreateDate = DateTime.Now;
                repository.Insert(entity);
            }
            else
            {
                repository.Update(entity, item => item.InputID == inputID && item.OutputID == null);
                noticeID = repository.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Single(item => item.InputID == inputID && item.OutputID == null).ID;
            }
            #endregion

            return noticeID;
        }

        /// <summary>
        /// 修改更新Waybill状态
        /// </summary>
        /// <param name="waybill"></param>
        public void EnterWaybill(JToken waybill)
        {
            var waybillID = waybill["WaybillID"].Value<string>();
            //var orderRequirement = waybill["OrderRequirement"].Value<string>();
            var orderRequirement = waybill["Requirement"]?.ToObject<OrderRequirement[]>();
            var checkRequirement = waybill["CheckRequirement"]?.ToObject<CheckRequirement>();
            // 处理更新Waybill
            using (var repository = new PvCenterReponsitory())
            {
                var waybillInstance = repository.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Single(item => item.ID == waybillID);
                var transferID = waybillInstance.TransferID;
                var orderID = waybillInstance.OrderID;

                repository.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    Status = 200,
                }, item => item.OrderID == orderID);

                repository.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    //ID = waybill["WaybillID"].Value<string>(),
                    Type = waybill["Type"].Value<int>(),
                    Supplier = waybill["Supplier"].Value<string>(),
                }, item => item.ID == waybillID);

                var waybillItem = repository.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Single(item => item.ID == waybill["WaybillID"].Value<string>());
                repository.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                {
                    WaybillID = waybillItem.ID,
                }, item => item.WsOrderID == waybillItem.OrderID);

                // 当订单要求不为空时，存入订单的特殊要求
                if (orderRequirement != null && orderRequirement.Count() > 0 && waybillInstance.Source == (int)CgNoticeSource.Transfer)
                {
                    // 转运入库,出库都要保存其特殊要求
                    EnterWayOrderRequirements(repository, orderRequirement, waybillID);
                    EnterWayOrderRequirements(repository, orderRequirement, transferID);
                }

                if (checkRequirement != null && (waybillInstance.Source == (int)CgNoticeSource.Transfer || waybillInstance.Source == (int)CgNoticeSource.AgentEnter))
                {
                    EnterWayCheckRequirements(repository, checkRequirement, waybillID);

                    if (transferID != null)
                    {
                        EnterWayCheckRequirements(repository, checkRequirement, transferID);
                    }
                }
            }
        }

        // 对代转运运单序列化其,
        public void EnterWayOrderRequirements(PvCenterReponsitory repository, OrderRequirement[] orderRequirement, string waybillID)
        {
            bool exist = repository.ReadTable<Layers.Data.Sqls.PvCenter.WayRequirements>().Any(item => item.ID == waybillID);

            if (exist)
            {
                repository.Update<Layers.Data.Sqls.PvCenter.WayRequirements>(new
                {
                    OrderRequirement = orderRequirement.Json(),
                }, item => item.ID == waybillID);
            }
            else
            {
                repository.Insert(new Layers.Data.Sqls.PvCenter.WayRequirements
                {
                    ID = waybillID,
                    OrderRequirement = orderRequirement.Json(),
                });
            }
        }

        // 订单要求保存
        public void EnterWayCheckRequirements(PvCenterReponsitory repository, CheckRequirement checkRequirement, string waybillID)
        {
            bool exist = repository.ReadTable<Layers.Data.Sqls.PvCenter.WayRequirements>().Any(item => item.ID == waybillID);

            if (checkRequirement.ApplicationID != null || checkRequirement.DelivaryOpportunity != null || checkRequirement.IsPayCharge != null)
            {
                if (exist)
                {
                    repository.Update<Layers.Data.Sqls.PvCenter.WayRequirements>(new
                    {
                        CheckRequirement = checkRequirement.Json(),
                        IsPayCharge = checkRequirement.IsPayCharge,
                        DelivaryOpportunity = (int?)checkRequirement.DelivaryOpportunity
                    }, item => item.ID == waybillID);
                }
                else
                {
                    repository.Insert(new Layers.Data.Sqls.PvCenter.WayRequirements
                    {
                        ID = waybillID,
                        IsPayCharge = checkRequirement.IsPayCharge,
                        DelivaryOpportunity = (int?)checkRequirement.DelivaryOpportunity,
                        CheckRequirement = checkRequirement.Json(),
                    });
                }
            }
        }

        /// <summary>
        /// Notice Enter 保存
        /// </summary>
        /// <param name="jobject"></param>
        public void EnterOld(JToken notices, string waybillID)
        {

            string[] currents = new string[notices.Count()];
            int index = 0;
            foreach (var notice in notices)
            {
                var product = notice["Product"];
                var productId = this.EnterProduct(product);

                var input = notice["Input"];
                //var inputID = currents[index++] = this.EnterInput(input, productId, this.Reponsitory);
                var inputID = this.EnterInput(input, productId, this.Reponsitory);

                string noticeID = this.EnterNotice(new Layers.Data.Sqls.PvWms.Notices
                {
                    #region 视图
                    Type = notice["Type"].Value<int>(),
                    WareHouseID = notice["WareHouseID"]?.Value<string>(),
                    WaybillID = notice["WaybillID"]?.Value<string>(),
                    InputID = inputID,
                    OutputID = notice["OutputID"]?.Value<string>(),
                    ProductID = productId,
                    DateCode = notice["DateCode"]?.Value<string>(),
                    Quantity = notice["Quantity"].Value<decimal>(),
                    Conditions = notice["Conditions"]?.Value<string>(),
                    Source = notice["Source"].Value<int>(),
                    Target = notice["Target"].Value<int>(),
                    Weight = notice["Weight"]?.Value<decimal?>(),
                    NetWeight = notice["NetWeight"]?.Value<decimal?>(),
                    Volume = notice["Volume"]?.Value<decimal?>(),
                    BoxCode = notice["BoxCode"]?.Value<string>(),
                    ShelveID = notice["ShelveID"]?.Value<string>(),
                    BoxingSpecs = notice["BoxingSpecs"]?.Value<int?>(),
                    Origin = notice["Origin"]?.Value<string>(),
                    CustomsName = notice["CustomsName"]?.Value<string>(),
                    #endregion
                }, this.Reponsitory);
                currents[index++] = noticeID;
            }

            //处理好了 通知后，一定是重新拿
            //这里多说一点：如果在库通知需要处理库存，就是依据通知生成流水库库存数据。并在原有库存中扣除notice的数量。
            //如果原有库存数量<通知数量 做throw  处理


            var linq_notices = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                               where notice.WaybillID == waybillID
                               select new
                               {
                                   notice.ID,
                                   notice.InputID
                               };

            var ienum_notices = linq_notices.ToArray();

            var exsits_noticeID = ienum_notices.Select(item => item.ID);

            var deletes = exsits_noticeID.Where(item => !currents.Contains(item));

            var storageIDs = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                             join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                             where deletes.Contains(sorting.NoticeID)
                             select storage.ID;
            var ienum_storageIDs = storageIDs.ToArray();

            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Storages>(item => ienum_storageIDs.Contains(item.ID));
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Sortings>(item => deletes.Contains(item.NoticeID));
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Notices>(item => deletes.Contains(item.ID));

            #region 检查更新Waybill执行状态
            /*
            //检查者
            Func<string, CgSortingExcuteStatus> StatusChecher = new Func<string, CgSortingExcuteStatus>(delegate (string wbID)
            {
                using (var pvcenterReponsitory = new PvCenterReponsitory())
                using (var pvwmsReponsitory = new PvWmsRepository())
                {
                    CgSortingExcuteStatus excuteStatus;
                    //历史分拣
                    var linq_sorteds = from sorting in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                       join _waybill in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>() on sorting.WaybillID equals _waybill.wbID
                                       where _waybill.wbID == wbID || _waybill.wbFatherID == wbID
                                       select new
                                       {
                                           sorting.NoticeID,
                                           sorting.Quantity
                                       };
                    var ienums_sortings = linq_sorteds.ToArray();

                    //当前的全部通知
                    var linq_noticeschecker = from notice in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                              where notice.WaybillID == wbID
                                              select new
                                              {
                                                  notice.ID,
                                                  notice.Quantity
                                              };
                    var ienums_notices = linq_noticeschecker.ToArray();


                    if (ienums_sortings.Any(item => item.NoticeID == null))
                    {
                        excuteStatus = CgSortingExcuteStatus.Anomalous;
                    }
                    else
                    {
                        var linq_merger = from notice in ienums_notices
                                          join sorting in ienums_sortings on notice.ID equals sorting.NoticeID into _sortings
                                          select new
                                          {
                                              NoticeID = notice.ID,
                                              noticeQuantity = notice.Quantity,
                                              sortedQuantity = _sortings.Sum(item => item.Quantity)
                                          };
                        if (linq_merger.All(item => item.sortedQuantity == 0))
                        {
                            excuteStatus = CgSortingExcuteStatus.Sorting;
                            return excuteStatus;
                        }

                        if (linq_merger.Any(item => item.noticeQuantity < item.sortedQuantity))
                        {
                            excuteStatus = CgSortingExcuteStatus.Anomalous;
                        }
                        else if (linq_merger.Any(item => item.noticeQuantity != item.sortedQuantity))
                        {
                            excuteStatus = CgSortingExcuteStatus.PartStocked;
                        }
                        else
                        {
                            excuteStatus = CgSortingExcuteStatus.Completed;
                        }

                    }

                    return excuteStatus;
                }
            });

            //重新更新状态, 如果满足入库完成，到货异常处理后的通知，就更改运单为入库完成。
            var status = StatusChecher(waybillID);
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ExcuteStatus = (int)status,
                }, item => item.ID == waybillID);
            }
            */
            #endregion

            // 重新入库通知后运单状态更新及后续处理
            UpdateNoticeHandle(waybillID);
        }

        /// <summary>
        /// 入库通知保存 Notice Enter 保存
        /// </summary>
        /// <param name="notices"></param>
        /// <param name="waybillID"></param>
        public void Enter(JToken notices, string waybillID)
        {

            Enter_chenhanxiugai(notices, waybillID);
            return;

            string[] currents_notices = new string[notices.Count()];
            string[] currents_inputs = new string[notices.Count()];

            var orderID = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Single(item => item.wbID == waybillID).OrderID;
            //建议把已经申报的判断逻辑提前到本地
            var linq_declare = from declare in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>()
                               join declareitem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>() on declare.TinyOrderID equals declareitem.TinyOrderID
                               select new
                               {
                                   declare.TinyOrderID,
                                   declare.Status,
                                   declareitem.OrderItemID,
                                   declareitem.StorageID,
                                   declareitem.Quantity,
                                   declareitem.BoxCode,
                               };

            int index = 0;
            foreach (var notice in notices)
            {
                var product = notice["Product"];
                var productId = this.EnterProduct(product);

                var input = notice["Input"];

                var inputID = currents_inputs[index] = this.EnterInput(input, productId, this.Reponsitory);

                string noticeID = this.EnterNotice(new Layers.Data.Sqls.PvWms.Notices
                {
                    #region 视图
                    Type = notice["Type"].Value<int>(),
                    WareHouseID = notice["WareHouseID"]?.Value<string>(),
                    WaybillID = notice["WaybillID"]?.Value<string>(),
                    InputID = inputID,
                    OutputID = notice["OutputID"]?.Value<string>(),
                    ProductID = productId,
                    DateCode = notice["DateCode"]?.Value<string>(),
                    Quantity = notice["Quantity"].Value<decimal>(),
                    Conditions = notice["Conditions"]?.Value<string>(),
                    Source = notice["Source"].Value<int>(),
                    Target = notice["Target"].Value<int>(),
                    Weight = notice["Weight"]?.Value<decimal?>(),
                    NetWeight = notice["NetWeight"]?.Value<decimal?>(),
                    Volume = notice["Volume"]?.Value<decimal?>(),
                    BoxCode = notice["BoxCode"]?.Value<string>(),
                    ShelveID = notice["ShelveID"]?.Value<string>(),
                    BoxingSpecs = notice["BoxingSpecs"]?.Value<int?>(),
                    Origin = notice["Origin"]?.Value<string>(),
                    CustomsName = notice["CustomsName"]?.Value<string>(),
                    #endregion
                }, this.Reponsitory);
                currents_notices[index++] = noticeID;

                // 后先需要把原有的分拣与重入库的通知的关系先配合上
                if (this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Any(item => item.InputID == inputID))
                {
                    this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Sortings>(new
                    {
                        NoticeID = noticeID,
                    }, item => item.InputID == inputID);
                }
                else
                {
                    decimal quantity = notice["Quantity"].Value<decimal>();
                    string partnumber = notice["Product"]["PartNumber"].Value<string>();
                    string manufacture = notice["Product"]["Manufacturer"].Value<string>();
                    string itemid = notice["Input"]["ItemID"].Value<string>();
                    //var orderID = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().
                    //Single(item => item.wbID == waybillID).OrderID;

                    var deliveryView = from delivery in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgDeliveriesTopView>()
                                       where delivery.OrderID == orderID
                                       select new
                                       {
                                           delivery.SortingID,
                                           delivery.ID,
                                           delivery.Type,
                                           delivery.Total,
                                           delivery.Quantity,
                                           delivery.InputID,
                                           delivery.ItemID,
                                           delivery.PartNumber,
                                           delivery.Manufacturer,
                                       };
                    var deliveryStorage = deliveryView.SingleOrDefault(item => item.Total == quantity && item.ItemID == itemid && item.PartNumber == partnumber && item.Manufacturer == manufacture);

                    if (deliveryStorage != null)
                    {
                        this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Sortings>(new
                        {
                            NoticeID = noticeID,
                            InputID = inputID,
                        }, item => item.ID == deliveryStorage.SortingID);

                        this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                        {
                            InputID = inputID,
                        }, item => item.ID == deliveryStorage.ID);
                    }

                }
            }

            //处理好了 通知后，一定是重新拿

            var linq_notices = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                               where notice.WaybillID == waybillID && notice.OutputID == null
                               select new
                               {
                                   notice.ID,
                                   notice.InputID,
                               };

            var ienum_notices = linq_notices.ToArray();

            var exsits_noticeID = ienum_notices.Select(item => item.ID).Distinct();
            var exsits_inputID = ienum_notices.Select(item => item.InputID).Distinct();

            var deletes_notices = exsits_noticeID.Where(item => !currents_notices.Contains(item));
            var deletes_inputIDs = exsits_inputID.Where(item => !currents_inputs.Contains(item));

            var storageIDs = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                             join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                             where deletes_notices.Contains(sorting.NoticeID)
                             select storage.ID;
            var ienum_storagesID = storageIDs.ToArray();


            if (linq_declare.Any(item => ienum_storagesID.Contains(item.StorageID) && item.Status >= (int)TinyOrderDeclareStatus.Declaring))
            {
                throw new Exception($"库存{string.Join(",", ienum_storagesID)}已经进行申报,不能做通知变更");
            }

            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Storages>(item => ienum_storagesID.Contains(item.ID));
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Sortings>(item => deletes_notices.Contains(item.NoticeID));
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Notices>(item => deletes_notices.Contains(item.ID));
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Inputs>(item => deletes_inputIDs.Contains(item.ID));

            string url = Wms.Services.FromType.NoticeDelivery.GetDescription();

            Yahv.Services.Models.StoreChange sc = new Yahv.Services.Models.StoreChange();
            sc.List.Add(new Yahv.Services.Models.ChangeItem
            {
                orderid = orderID
            });

            Yahv.Utils.Http.ApiHelper.Current.JPost(url, sc);

            // 重新入库通知后运单状态更新及后续处理
            UpdateNoticeHandle(waybillID);
        }
        /// <summary>
        /// 入库通知保存 Notice Enter 保存
        /// </summary>
        /// <param name="notices"></param>
        /// <param name="waybillID"></param>
        public void Enter_chenhanxiugai(JToken notices, string waybillID)
        {
            string[] currents_notices = new string[notices.Count()];
            string[] currents_inputs = new string[notices.Count()];
            JToken[] currents_jnotices = new JToken[notices.Count()];

            //建议把已经申报的判断逻辑提前到本地
            var linq_declare = from declare in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>()
                               join declareitem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>() on declare.TinyOrderID equals declareitem.TinyOrderID
                               select new
                               {
                                   declare.TinyOrderID,
                                   declare.Status,
                                   //declareitem.OrderItemID,
                                   declareitem.StorageID,
                                   //declareitem.Quantity,
                                   //declareitem.BoxCode,
                               };

            int index = 0;

            //获取订单ID
            var orderID = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().
                Single(item => item.wbID == waybillID).OrderID;

            //获取到货信息
            var deliveryView = from delivery in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgDeliveriesTopView>()
                               where delivery.OrderID == orderID
                               select new
                               {
                                   delivery.SortingID,
                                   delivery.ID,
                                   delivery.Type,
                                   delivery.Total,
                                   delivery.Quantity,
                                   delivery.InputID,
                                   delivery.ItemID,
                                   delivery.PartNumber,
                                   delivery.Manufacturer,
                               };

            var deliveries = deliveryView.ToArray();

            foreach (var notice in notices)
            {
                var product = notice["Product"];
                var productId = this.EnterProduct(product);

                var input = notice["Input"];

                var inputID = currents_inputs[index] = this.EnterInput(input, productId, this.Reponsitory);

                string noticeID = this.EnterNotice(new Layers.Data.Sqls.PvWms.Notices
                {
                    #region 视图
                    Type = notice["Type"].Value<int>(),
                    WareHouseID = notice["WareHouseID"]?.Value<string>(),
                    WaybillID = notice["WaybillID"]?.Value<string>(),
                    InputID = inputID,
                    OutputID = notice["OutputID"]?.Value<string>(),
                    ProductID = productId,
                    DateCode = notice["DateCode"]?.Value<string>(),
                    Quantity = notice["Quantity"].Value<decimal>(),
                    Conditions = notice["Conditions"]?.Value<string>(),
                    Source = notice["Source"].Value<int>(),
                    Target = notice["Target"].Value<int>(),
                    Weight = notice["Weight"]?.Value<decimal?>(),
                    NetWeight = notice["NetWeight"]?.Value<decimal?>(),
                    Volume = notice["Volume"]?.Value<decimal?>(),
                    BoxCode = notice["BoxCode"]?.Value<string>(),
                    ShelveID = notice["ShelveID"]?.Value<string>(),
                    BoxingSpecs = notice["BoxingSpecs"]?.Value<int?>(),
                    Origin = notice["Origin"]?.Value<string>(),
                    CustomsName = notice["CustomsName"]?.Value<string>(),
                    #endregion
                }, this.Reponsitory);
                currents_jnotices[index] = notice;
                currents_notices[index++] = noticeID;

                // 后先需要把原有的分拣与重入库的通知的关系先配合上

                //修改
            }

            var sortings = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().
                Where(item => currents_inputs.Contains(item.InputID)).Select(item => new
                {
                    item.ID,
                    item.InputID,
                }).ToArray();

            for (int reffer = 0; reffer < currents_inputs.Length; reffer++)
            {
                string inputID = currents_inputs[reffer];
                string noticeID = currents_notices[reffer];
                var notice = currents_jnotices[reffer];

                if (sortings.Any(item => item.InputID == inputID))
                {
                    this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Sortings>(new
                    {
                        NoticeID = noticeID,
                    }, item => item.InputID == inputID);
                }
                else
                {
                    decimal quantity = notice["Quantity"].Value<decimal>();
                    string partnumber = notice["Product"]["PartNumber"].Value<string>();
                    string manufacture = notice["Product"]["Manufacturer"].Value<string>();
                    string itemid = notice["Input"]["ItemID"].Value<string>();

                    var deliveryStorage = deliveries.SingleOrDefault(item => item.Total == quantity && item.ItemID == itemid && item.PartNumber == partnumber && item.Manufacturer == manufacture);

                    if (deliveryStorage != null)
                    {
                        this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Sortings>(new
                        {
                            NoticeID = noticeID,
                            InputID = inputID,
                        }, item => item.ID == deliveryStorage.SortingID);

                        this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                        {
                            InputID = inputID,
                        }, item => item.ID == deliveryStorage.ID);
                    }
                }
            }

            //处理好了 通知后，一定是重新拿

            var linq_notices = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                               where notice.WaybillID == waybillID && notice.OutputID == null
                               select new
                               {
                                   notice.ID,
                                   notice.InputID,
                               };

            var ienum_notices = linq_notices.ToArray();

            var exsits_noticeID = ienum_notices.Select(item => item.ID).Distinct();
            var exsits_inputID = ienum_notices.Select(item => item.InputID).Distinct();

            var deletes_notices = exsits_noticeID.Where(item => !currents_notices.Contains(item));
            var deletes_inputIDs = exsits_inputID.Where(item => !currents_inputs.Contains(item));

            var storageIDs = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                             join sorting in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                             where deletes_notices.Contains(sorting.NoticeID)
                             select storage.ID;
            var ienum_storagesID = storageIDs.ToArray();


            if (linq_declare.Any(item => ienum_storagesID.Contains(item.StorageID) && item.Status >= (int)TinyOrderDeclareStatus.Declaring))
            {
                throw new Exception($"库存{string.Join(",", ienum_storagesID)}已经进行申报,不能做通知变更");
            }

            var ienum_declareitemstorageIDs = linq_declare.Where(item => item.Status < (int)TinyOrderDeclareStatus.Declaring && item.TinyOrderID.Contains(orderID)).Select(item => item.StorageID).ToArray();
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(item => ienum_declareitemstorageIDs.Contains(item.StorageID));

            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Storages>(item => ienum_storagesID.Contains(item.ID));
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Sortings>(item => deletes_notices.Contains(item.NoticeID));
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Notices>(item => deletes_notices.Contains(item.ID));
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Inputs>(item => deletes_inputIDs.Contains(item.ID));

            string url = Wms.Services.FromType.NoticeDelivery.GetDescription();

            Yahv.Services.Models.StoreChange sc = new Yahv.Services.Models.StoreChange();
            sc.List.Add(new Yahv.Services.Models.ChangeItem
            {
                orderid = orderID
            });

            Yahv.Utils.Http.ApiHelper.Current.JPost(url, sc);

            // 重新入库通知后运单状态更新及后续处理
            UpdateNoticeHandle(waybillID);
        }

        /// <summary>
        /// 入库分拣异常处理后重新进行入库通知更新后的处理
        /// </summary>
        /// <param name="waybillID"></param>
        static public void UpdateNoticeHandle(string waybillID)
        {
            CgSortingExcuteStatus excuteStatus;
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            using (var pvwmsReponsitory = new PvWmsRepository())
            {

                var waybill = pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Single(item => item.wbID == waybillID);
                //历史分拣
                var linq_sorteds = from storage in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                   join sorting in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                                   join input in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on storage.InputID equals input.ID
                                   join _waybill in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>() on sorting.WaybillID equals _waybill.wbID
                                   where _waybill.wbID == waybillID || _waybill.wbFatherID == waybillID
                                   select new
                                   {
                                       #region 视图
                                       sorting.ID,
                                       sorting.NoticeID,
                                       sorting.Quantity,
                                       sorting.BoxCode,
                                       sorting.AdminID,
                                       StorageID = storage.ID,
                                       sorting.Weight,
                                       sorting.CreateDate,
                                       sorting.NetWeight,
                                       sorting.Volume,
                                       storage.ProductID,
                                       storage.DateCode,
                                       storage.Origin,
                                       storage.ShelveID,
                                       StorageType = storage.Type,
                                       input,
                                       #endregion
                                   };
                var ienums_sortings = linq_sorteds.ToArray();
                //当前的全部通知
                var linq_notices = from notice in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                   join input in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                                   on notice.InputID equals input.ID
                                   where notice.WaybillID == waybillID && notice.OutputID == null
                                   select new
                                   {
                                       #region 视图

                                       notice.ID,
                                       notice.Type,
                                       notice.WareHouseID,
                                       notice.WaybillID,
                                       notice.InputID,
                                       notice.OutputID,
                                       notice.ProductID,
                                       notice.Quantity,
                                       notice.DateCode,
                                       notice.Origin,
                                       notice.Weight,
                                       notice.NetWeight,
                                       notice.Volume,
                                       notice.Source,
                                       notice.Target,
                                       notice.BoxCode,
                                       notice.BoxingSpecs,
                                       notice.ShelveID,
                                       notice.Conditions,
                                       notice.Supplier,
                                       notice.Summary,
                                       notice.StorageID,
                                       notice.Status,
                                       notice.CreateDate,
                                       notice.CustomsName,
                                       input

                                       #endregion
                                   };
                var ienums_notices = linq_notices.ToArray().Select(notice => new
                {
                    #region 视图

                    notice.ID,
                    notice.Type,
                    notice.WareHouseID,
                    notice.WaybillID,
                    notice.InputID,
                    notice.OutputID,
                    notice.ProductID,
                    notice.Quantity,
                    notice.DateCode,
                    notice.Origin,
                    notice.Weight,
                    notice.NetWeight,
                    notice.Volume,
                    notice.Source,
                    notice.Target,
                    notice.BoxCode,
                    notice.BoxingSpecs,
                    notice.ShelveID,
                    Conditions = notice.Conditions.JsonTo<NoticeCondition>(),
                    notice.Supplier,
                    notice.Summary,
                    notice.StorageID,
                    notice.Status,
                    notice.CreateDate,
                    notice.CustomsName,
                    notice.input,

                    #endregion
                });

                // 更新已处理过的代仓储的无通知入库的库存类型为Store
                if (ienums_notices.Any(item => item.WareHouseID.StartsWith("HK") && item.Type == (int)CgNoticeType.Enter && item.Source == (int)CgNoticeSource.AgentEnter))
                {
                    var storageIDs = from sorting in ienums_sortings
                                     join _notice in ienums_notices on sorting.NoticeID equals _notice.ID
                                     where sorting.StorageType == (int)CgStoragesType.Unknown && sorting.NoticeID != null
                                     select sorting.StorageID;
                    var ienum_storageIDs = storageIDs.ToArray();

                    pvwmsReponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                    {
                        Type = (int)CgStoragesType.Stores,
                    }, item => ienum_storageIDs.Contains(item.ID));
                }

                #region 检测运单的执行状态
                if (ienums_sortings.Any(item => item.NoticeID == null))
                {
                    excuteStatus = CgSortingExcuteStatus.Anomalous;
                }
                else
                {
                    var linq_merger = from notice in ienums_notices
                                      join sorting in ienums_sortings on notice.ID equals sorting.NoticeID into _sortings
                                      select new
                                      {
                                          NoticeID = notice.ID,
                                          noticeQuantity = notice.Quantity,
                                          sortedQuantity = _sortings.Sum(item => item.Quantity)
                                      };
                    if (linq_merger.All(item => item.sortedQuantity == 0))
                    {
                        excuteStatus = CgSortingExcuteStatus.Sorting;
                    }
                    else if (linq_merger.Any(item => item.noticeQuantity < item.sortedQuantity))
                    {
                        excuteStatus = CgSortingExcuteStatus.Anomalous;
                    }
                    else if (linq_merger.Any(item => item.noticeQuantity != item.sortedQuantity))
                    {
                        excuteStatus = CgSortingExcuteStatus.PartStocked;
                    }
                    else
                    {
                        excuteStatus = CgSortingExcuteStatus.Completed;
                    }

                }
                #endregion

                //与荣家商议，代报关流程中。是否去申报与分拣是否异常无关
                if (excuteStatus != CgSortingExcuteStatus.Sorting/* & excuteStatus != CgSortingExcuteStatus.Anomalous*/)
                {
                    if (waybill.Source == (int)CgNoticeSource.AgentBreakCustoms)
                    {
                        #region 报关处理
                        // 当前重新启用IsDeclared标识 --取消后面的注释， 当前根据封箱功能的新要求, 不再根据IsDeclared去生成申报日志及申报项日志
                        // 取消后面的注释 -- 所以此时也不用去更改订单的Logs_PvWsOrder状态了
                        var notices = ienums_notices;
                        string adminid = Npc.Robot.Obtain();
                        bool isDeclare = false;
                        if (excuteStatus == CgSortingExcuteStatus.PartStocked || excuteStatus == CgSortingExcuteStatus.Anomalous)
                        {
                            notices = ienums_notices.Where(item => item.Conditions.IsDeclared);
                            adminid = ienums_sortings.OrderByDescending(item => item.CreateDate).
                                First().AdminID;

                            isDeclare = true;
                        }
                        else if (excuteStatus == CgSortingExcuteStatus.Completed)
                        {
                            notices = ienums_notices;

                            adminid = ienums_sortings.OrderByDescending(item => item.CreateDate).
                                First().AdminID;

                            isDeclare = true;
                        }
                        else
                        {
                            isDeclare = false;
                        }

                        if (string.IsNullOrWhiteSpace(adminid))
                        {
                            throw new ArgumentNullException("adminid", "内部参数异常！");
                        }

                        if (isDeclare)
                        {
                            foreach (var notice in notices)
                            {
                                var sortings = ienums_sortings.Where(item => item.NoticeID == notice.ID).ToArray();

                                if (sortings.Count() == 0)
                                    continue;
                                foreach (var sorting in sortings)
                                {
                                    if (!string.IsNullOrWhiteSpace(sorting?.NoticeID))
                                    {
                                        Declaring(notice.input.TinyOrderID, waybill.wbEnterCode, sorting.AdminID, pvwmsReponsitory);
                                        DeclaringItem(notice.OutputID, sorting.Quantity, notice.input.TinyOrderID, sorting.input.ItemID, sorting.BoxCode, waybill.wbEnterCode, sorting.AdminID, sorting.StorageID, sorting.Weight, pvwmsReponsitory);
                                    }

                                    pvwmsReponsitory.Update<Layers.Data.Sqls.PvWms.Sortings>(new
                                    {
                                        Summary = string.Empty,
                                    }, item => item.ID == sorting.ID);
                                }
                            }
                        }
                        if ((excuteStatus == CgSortingExcuteStatus.PartStocked && ienums_notices.Any(item => item.Conditions.IsDeclared))
                            || excuteStatus == CgSortingExcuteStatus.Completed)
                        {
                            UpdateOrderStatus((CgNoticeSource)waybill.Source, waybill.OrderID, adminid);

                        }

                        #endregion
                    }

                    // 代收货完成时的处理
                    if (waybill.Source == (int)CgNoticeSource.AgentEnter && excuteStatus == CgSortingExcuteStatus.Completed)
                    {
                        // 更新订单日志
                        UpdateOrderStatus((CgNoticeSource)waybill.Source, waybill.OrderID, Npc.Robot.Obtain());
                    }

                    // 代转运完成入库时的处理
                    if (waybill.Source == (int)CgNoticeSource.Transfer && excuteStatus == CgSortingExcuteStatus.Completed)
                    {
                        #region 代转运处理
                        var exsit = pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().
                            Any(notice => notice.WaybillID == waybill.wbTransferID && notice.Type == (int)CgNoticeType.Out);

                        if (!exsit)
                        {
                            var enums_notice = from storage in ienums_sortings
                                               join notice in ienums_notices on storage.NoticeID equals notice.ID
                                               select new
                                               {
                                                   #region 视图
                                                   notice.WareHouseID,
                                                   storage.StorageID,                                                   
                                                   notice.InputID,
                                                   notice.WaybillID,
                                                   notice.Supplier,
                                                   storage.NoticeID,
                                                   storage.BoxCode,
                                                   storage.Weight,
                                                   storage.NetWeight,
                                                   storage.Volume,
                                                   notice,
                                                   notice.input,
                                                   storage,
                                                   storage.ProductID,
                                                   storage.DateCode,
                                                   storage.Origin,
                                                   storage.Quantity,
                                                   storage.ShelveID

                                                   #endregion
                                               };
                            //代转运完成入库时, 进行生成Output,以及出库通知
                            var ienum_linq = enums_notice.ToArray();
                            var storages = ienum_linq.Where(item => item.WaybillID == waybillID);
                            var transferID = waybill.wbTransferID;

                            foreach (var storage in storages)
                            {
                                var outputId = PKeySigner.Pick(PkeyType.Outputs);
                                var outNoticeID = PKeySigner.Pick(PkeyType.Notices);

                                pvwmsReponsitory.Insert(new Layers.Data.Sqls.PvWms.Outputs
                                {
                                    ID = outputId,
                                    InputID = storage.storage.input.ID,
                                    OrderID = storage.storage.input.OrderID,
                                    TinyOrderID = storage.storage.input.TinyOrderID,
                                    ItemID = storage.storage.input.ItemID,
                                    OwnerID = storage.storage.input.ClientID,
                                    SalerID = storage.storage.input.SalerID,
                                    CustomerServiceID = null, // 跟单员 转运出库销项跟单员填什么
                                    PurchaserID = storage.storage.input.PurchaserID,
                                    Currency = storage.storage.input.Currency,
                                    Price = storage.storage.input.UnitPrice,
                                    CreateDate = DateTime.Now,
                                    TrackerID = storage.storage.input.TrackerID
                                });

                                //在分拣录入之后就会发出出库通知
                                pvwmsReponsitory.Insert(new Layers.Data.Sqls.PvWms.Notices
                                {
                                    ID = outNoticeID,
                                    Type = (int)CgNoticeType.Out,
                                    WareHouseID = storage.WareHouseID,
                                    WaybillID = waybill.wbTransferID,
                                    InputID = storage.storage.input.ID,
                                    OutputID = outputId,
                                    ProductID = storage.storage.ProductID,
                                    Supplier = storage.Supplier,
                                    DateCode = storage.storage.DateCode,
                                    Origin = storage.storage.Origin,
                                    Quantity = storage.storage.Quantity,
                                    Conditions = storage.notice.Conditions.Json(),
                                    CreateDate = DateTime.Now,
                                    Status = (int)NoticesStatus.Waiting,  // 需要再确认，转运出库时的通知状态
                                    Source = (int)NoticeSource.Transfer,
                                    Target = (int)NoticesTarget.Default,
                                    StorageID = storage.StorageID,
                                    BoxCode = storage.BoxCode,
                                    Weight = storage.Weight,
                                    NetWeight = storage.NetWeight,
                                    Volume = storage.Volume,
                                    ShelveID = storage.storage.ShelveID,
                                });

                            }
                        }

                        // 更新订单日志
                        UpdateOrderStatus((CgNoticeSource)waybill.Source, waybill.OrderID, Npc.Robot.Obtain());
                        #endregion
                    }
                }

                // 最后去更新运单执行状态
                pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ExcuteStatus = (int)excuteStatus,
                }, item => item.ID == waybillID);
            }

        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="jobject"></param>
        public void Delete(JToken delete)
        {
            // 删除Notice

            var itemIds = ((JArray)delete["ItemID"])?.Select(item => item.Value<string>()).ToArray() ?? new string[0];
            var inputIds = ((JArray)delete["InputID"])?.Select(item => item.Value<string>()).ToArray() ?? new string[0];
            var noticeIds = ((JArray)delete["NoticeID"])?.Select(item => item.Value<string>()).ToArray() ?? new string[0];
            if (inputIds.Length == 0 && noticeIds.Length == 0 && itemIds.Length == 0)
            {
                return;
            }
            using (var reponsitory = new PvWmsRepository())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvWms.Notices>(item =>
                       noticeIds.Contains(item.ID) || inputIds.Contains(item.InputID));

                //reponsitory.Delete<Layers.Data.Sqls.PvWms.Inputs>(item => inputIds.Contains(item.ID));

                if (itemIds.Length > 0)
                {
                    var linq = from input in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                               join notice in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on input.ID equals notice.InputID
                               where itemIds.Contains(input.ItemID)
                               select notice.ID;
                    var arry = linq.ToArray();

                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Notices>(item => arry.Contains(item.ID));
                }
            }
        }

        /// <summary>
        /// 更新订单主状态
        /// </summary>
        /// <param name="source"></param>
        /// <param name="waybillorderid"></param>
        /// <param name="adminID"></param>
        public static void UpdateOrderStatus(CgNoticeSource source, string waybillorderid, string adminID)
        {
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                if (source == CgNoticeSource.AgentEnter || source == CgNoticeSource.Transfer)
                {
                    //保存订单日志, 非代报关业务订单的状态为已收货
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.Type == 1 && item.MainID == waybillorderid);

                    var orderID = waybillorderid;
                    pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = orderID,
                        Type = 1, //订单主状态，  
                        Status = (int)CgOrderStatus.已交货,//建议询问小辉
                        CreateDate = DateTime.Now,
                        CreatorID = adminID,
                        IsCurrent = true,
                    });

                    if (source == CgNoticeSource.AgentEnter)
                    {
                        //保存订单日志, 代收货业务订单的支付状态为待确认
                        pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                        {
                            IsCurrent = false,
                        }, item => item.Type == (int)OrderStatusType.PaymentStatus && item.MainID == waybillorderid);

                        pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                        {
                            ID = Guid.NewGuid().ToString(),
                            MainID = orderID,
                            Type = (int)OrderStatusType.PaymentStatus, //订单支付状态，  
                            Status = (int)OrderPaymentStatus.Confirm,
                            CreateDate = DateTime.Now,
                            CreatorID = adminID,
                            IsCurrent = true,
                        });
                    }
                }
                else if (source == CgNoticeSource.AgentBreakCustoms)
                {
                    //保存订单日志, 非代报关业务订单的状态为已收货
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.Type == 1 && item.MainID == waybillorderid);

                    var orderID = waybillorderid;
                    pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = orderID,
                        Type = 1, //订单主状态，  
                        Status = (int)CgOrderStatus.待报关,//建议询问小辉
                        CreateDate = DateTime.Now,
                        CreatorID = adminID,
                        IsCurrent = true,
                    });
                }

            }
        }

        /// <summary>
        /// 申报
        /// </summary>
        /// <param name="tinyOrderID">小订单</param>        
        /// <param name="enterCode">入仓号</param>
        /// <param name="adminID">装箱人</param>
        /// <param name="reponsitory"></param>
        public static void Declaring(string tinyOrderID, string enterCode, string adminID, PvWmsRepository reponsitory)
        {
            bool existLogDelcare = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>().
                Any(item => item.TinyOrderID == tinyOrderID);
            if (!existLogDelcare)
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Declare
                {
                    ID = PKeySigner.Pick(PkeyType.LogsDeclare),
                    TinyOrderID = tinyOrderID,
                    EnterCode = enterCode,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Status = (int)TinyOrderDeclareStatus.Boxed,
                    AdminID = adminID,
                });
            }
        }

        /// <summary>
        /// 申报项目
        /// </summary>
        /// <param name="outputID">销项(可空)</param>
        /// <param name="quantity">申报数量</param>
        /// <param name="tinyOrderID">小订单ID</param>
        /// <param name="itemID">订单项ID</param>
        /// <param name="boxCode"></param>
        /// <param name="enterCode"></param>
        /// <param name="adminID"></param>
        /// <param name="storageID"></param>
        /// <param name="weight"></param>
        /// <param name="reponsitory"></param>
        public static void DeclaringItem(string outputID, decimal quantity,
         string tinyOrderID,
         string itemID,
         string boxCode,
         string enterCode,
         string adminID,
         string storageID,
         decimal? weight, PvWmsRepository reponsitory)
        {
            bool existLogDeclareItem = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>().Any(item => item.TinyOrderID == tinyOrderID && item.OrderItemID == itemID
                && item.StorageID == storageID);
            if (!existLogDeclareItem)
            {
                var storage = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().SingleOrDefault(item => item.ID == storageID);
                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_DeclareItem
                {
                    ID = PKeySigner.Pick(PkeyType.LogsDeclareItem),
                    TinyOrderID = tinyOrderID,
                    OrderItemID = itemID,
                    StorageID = storageID,
                    Quantity = quantity,
                    AdminID = adminID,
                    OutputID = outputID,
                    BoxCode = boxCode,
                    Weight = weight,
                    Status = 0,
                });
            }
        }
    }
}