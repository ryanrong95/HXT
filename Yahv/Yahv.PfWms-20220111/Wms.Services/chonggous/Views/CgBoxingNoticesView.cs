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

namespace Wms.Services.chonggous.Views
{
    /// <summary>
    /// 装箱通知视图
    /// </summary>
    public class CgBoxingNoticesView : CgNoticesView
    {
        public CgBoxingNoticesView()
        {

        }

        protected override IQueryable<CgNotice> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Type == CgNoticeType.Boxing);
        }

        /// <summary>
        /// output数据保存
        /// </summary>
        /// <param name="output">output数据</param>
        /// <returns></returns>
        private string EnterOutputOld(JToken output, string inputID, string adminID)
        {
            var outputid = output["ID"]?.Value<string>();
            if (string.IsNullOrWhiteSpace(outputid))
            {
                outputid = PKeySigner.Pick(PkeyType.Outputs);
            }

            if (!this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().Any(item => item.ID == outputid))
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Outputs
                {
                    ID = outputid,
                    InputID = inputID,
                    OrderID = output["OrderID"].Value<string>(),
                    TinyOrderID = output["TinyOrderID"]?.Value<string>(),
                    ItemID = output["ItemID"].Value<string>(),
                    OwnerID = adminID,
                    SalerID = output["SalerID"]?.Value<string>(),
                    PurchaserID = output["PurchaserID"]?.Value<string>(),
                    Currency = output["Currency"]?.Value<int>(),
                    Price = output["Price"]?.Value<decimal>(),
                    CreateDate = DateTime.Now,
                    ReviewerID = output["ReviewerID"]?.Value<string>(),
                    TrackerID = output["TrackerID"]?.Value<string>(),
                });
            }
            else
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Outputs>(new
                {
                    InputID = inputID,
                    OrderID = output["OrderID"].Value<string>(),
                    TinyOrderID = output["TinyOrderID"]?.Value<string>(),
                    ItemID = output["ItemID"].Value<string>(),
                    OwnerID = adminID,
                    SalerID = output["SalerID"]?.Value<string>(),
                    PurchaserID = output["PurchaserID"]?.Value<string>(),
                    Currency = output["Currency"]?.Value<int>(),
                    Price = output["Price"]?.Value<decimal>(),
                    ReviewerID = output["ReviewerID"]?.Value<string>(),
                    TrackerID = output["TrackerID"]?.Value<string>(),
                }, item => item.ID == outputid);
            }

            return outputid;
        }

        /// <summary>
        /// output保存
        /// </summary>
        /// <param name="output"></param>
        /// <param name="adminID"></param>
        /// <returns></returns>
        private string EnterOutput(JToken output, string adminID)
        {
            var outputid = output["ID"]?.Value<string>();
            if (string.IsNullOrWhiteSpace(outputid))
            {
                outputid = PKeySigner.Pick(PkeyType.Outputs);
            }

            if (!this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().Any(item => item.ID == outputid))
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Outputs
                {
                    ID = outputid,
                    InputID = output["InputID"].Value<string>(),
                    OrderID = output["OrderID"].Value<string>(),
                    TinyOrderID = output["TinyOrderID"]?.Value<string>(),
                    ItemID = output["ItemID"].Value<string>(),
                    OwnerID = adminID,
                    SalerID = output["SalerID"]?.Value<string>(),
                    PurchaserID = output["PurchaserID"]?.Value<string>(),
                    Currency = output["Currency"]?.Value<int>(),
                    Price = output["Price"]?.Value<decimal>(),
                    CreateDate = DateTime.Now,
                    ReviewerID = output["ReviewerID"]?.Value<string>(),
                    TrackerID = output["TrackerID"]?.Value<string>(),
                });
            }
            else
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Outputs>(new
                {
                    InputID = output["InputID"].Value<string>(),
                    OrderID = output["OrderID"].Value<string>(),
                    TinyOrderID = output["TinyOrderID"]?.Value<string>(),
                    ItemID = output["ItemID"].Value<string>(),
                    OwnerID = adminID,
                    SalerID = output["SalerID"]?.Value<string>(),
                    PurchaserID = output["PurchaserID"]?.Value<string>(),
                    Currency = output["Currency"]?.Value<int>(),
                    Price = output["Price"]?.Value<decimal>(),
                    ReviewerID = output["ReviewerID"]?.Value<string>(),
                    TrackerID = output["TrackerID"]?.Value<string>(),
                }, item => item.ID == outputid);
            }

            return outputid;
        }

        /// <summary>
        /// notice保存
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        private string EnterNotice(Layers.Data.Sqls.PvWms.Notices entity, PvWmsRepository repository)
        {
            string outputID = entity.OutputID;
            string noticeID = string.Empty;
            if (!repository.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Any(item => item.OutputID == outputID))
            {
                noticeID = PKeySigner.Pick(PkeyType.Notices);
                entity.ID = noticeID;
                entity.CreateDate = DateTime.Now;
                repository.Insert(entity);
            }
            else
            {
                repository.Update(entity, item => item.OutputID == outputID);
                noticeID = repository.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Single(item => item.OutputID == outputID).ID;
            }

            return noticeID;    
        }

        /// <summary>
        /// 装箱通知数据持久化
        /// </summary>
        /// <param name="Notices"></param>
        public void EnterBoxing(JToken waybill, JToken notices)
        {
            var waybillID = waybill["WaybillID"].Value<string>();
            var adminID = waybill["AdminID"]?.Value<string>();

            var source = (CgNoticeSource)waybill["Source"].Value<int>();
            var noticeType = (CgNoticeType)waybill["NoticeType"].Value<int>();

            if (source != CgNoticeSource.AgentCustomsFromStorage || noticeType != CgNoticeType.Boxing)
            {
                throw new Exception("装箱通知只用于转报关进项装箱!");
            }

            var storagesView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>();
            var isValid = true;

            // 判断装箱通知是否合理
            foreach (var notice in notices)
            {
                var storageID = notice["StorageID"].Value<string>();
                var storageEntity = storagesView.Single(item => item.ID == storageID);

                // 从Storage中减除对应的数量
                var quantity = notice["Quantity"].Value<decimal>();
                if (storageEntity.Quantity < quantity)
                {
                    isValid = false;
                    break;
                }
            }

            if (!isValid)
            {
                throw new Exception($"Notice中的数量大于库存的数量，不能正确生成装箱通知,请检查!");
            }

            #region 删除不需要的Notice 及归还库存, 现不需要此处逻辑
            /*
            var linq_notice = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                              where notice.WaybillID == waybillID && notice.Type == (int)CgNoticeType.Boxing && notice.Source == (int)CgNoticeSource.AgentCustomsFromStorage
                              select notice;

            var ienum_notices = linq_notice.ToArray();

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

            foreach (var notice in ienum_notices)
            {
                // 对应的流水库
                var flowStorage = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().
                    SingleOrDefault(item => item.Type == (int)CgStoragesType.Flows && item.WareHouseID == notice.WareHouseID && item.InputID == notice.InputID && item.Total == notice.Quantity);

                if (linq_declare.Any(item => item.StorageID == flowStorage.ID && item.Status >= (int)TinyOrderDeclareStatus.Declaring))
                {
                    throw new Exception($"重新装箱失败，{flowStorage.ID} 不能删除，该分拣入库的库存已经申报!");
                }

                var picking = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().SingleOrDefault(item => item.StorageID == flowStorage.ID);

                // 更新库存日志并删除对应的流水库
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_Storage>(new
                {
                    IsCurrent = false,
                }, item => item.IsCurrent == true && item.StorageID == flowStorage.ID);

                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Storages>(item => item.ID == flowStorage.ID);

                // 如果已经生成过Picking, 则把Picking用过的箱号删除
                if (picking != null)
                {
                    var boxCode = picking.BoxCode;

                    // 删除对应的Pickings
                    this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Pickings>(item => item.ID == picking.ID);

                    CgBoxManage.Current.Delete(boxCode);                    
                }

                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(item => item.StorageID == flowStorage.ID);                
                
                // 原库存
                var storage = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().SingleOrDefault(item => item.Type == (int)CgStoragesType.Stores && item.ID == notice.StorageID);

                // 更新流水库存--恢复原库存
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                {
                    Type = (int)CgStoragesType.Stores,
                    Quantity = storage.Quantity + notice.Quantity,
                }, item => item.Type == (int)CgStoragesType.Stores && item.ID == notice.StorageID);

                // 删除对应的Outputs
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Outputs>(item => item.ID == notice.OutputID);

                // 删除对应的Notices
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Notices>(item => item.ID == notice.ID);
            }
            */
            #endregion

            #region 保存正常的Notice
            foreach (var notice in notices)
            {
                var storageID = notice["StorageID"].Value<string>();
                var storageEntity = storagesView.Single(item => item.ID == storageID);// 效率低，建议用整体方式获取
                var output = notice["Output"];

                // 销项数据保存
                var outputID = EnterOutputOld(output, storageEntity.InputID, adminID);

                // 从Storage中减除对应的数量
                var noticeQuantity = notice["Quantity"].Value<decimal>();

                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                {
                    Quantity = storageEntity.Quantity - noticeQuantity,
                }, item => item.ID == storageID);

                var storageIDForFlow = PKeySigner.Pick(PkeyType.Storages);
                // 根据要求生成对应的流水库
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Storages
                {
                    ID = storageIDForFlow,
                    Type = (int)CgStoragesType.Flows, //OK!

                    WareHouseID = storageEntity.WareHouseID,
                    SortingID = storageEntity.SortingID, // 该值需要确认?
                    InputID = storageEntity.InputID,
                    ProductID = storageEntity.ProductID,
                    Total = noticeQuantity,
                    Quantity = noticeQuantity,
                    Origin = storageEntity.Origin,
                    IsLock = true, // 装箱的时候需要锁上是吗?
                    CreateDate = DateTime.Now,
                    Status = storageEntity.Status,
                    ShelveID = storageEntity.ShelveID,
                    Supplier = storageEntity.Supplier,
                    DateCode = storageEntity.DateCode,
                    Summary = outputID,
                });

                this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Storage
                {
                    AdminID = "Npc-Robot",
                    BoxCode = null,
                    CreateDate = DateTime.Now,
                    StorageID = storageIDForFlow,
                    IsCurrent = true,
                    Summary = null,
                    Weight = 0,
                    ID = Guid.NewGuid().ToString(),
                });                

                var noticeID = PKeySigner.Pick(PkeyType.Notices);
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Notices
                {
                    ID = noticeID,
                    Type = (int)CgNoticeType.Boxing,
                    WareHouseID = notice["WareHouseID"].Value<string>(),
                    WaybillID = waybillID,
                    InputID = storageEntity.InputID,
                    OutputID = outputID,
                    ProductID = storageEntity.ProductID,
                    Supplier = storageEntity.Supplier,
                    DateCode = storageEntity.DateCode,
                    Quantity = notice["Quantity"].Value<decimal>(),
                    Conditions = notice["Conditions"].Value<string>(),
                    CreateDate = DateTime.Now,
                    Status = (int)NoticesStatus.Waiting,
                    Source = (int)CgNoticeSource.AgentCustomsFromStorage,
                    Target = (int)NoticesTarget.Default,
                    //BoxCode = notice["BoxCode"]?.Value<string>(), //装箱通知中不涉及到, 只有在分拣入库的时候才用到
                    Weight = notice["Weight"]?.Value<decimal?>(),
                    NetWeight = notice["NetWeight"]?.Value<decimal?>(),
                    Volume = notice["Volume"]?.Value<decimal?>(),
                    ShelveID = notice["ShelveID"]?.Value<string>(),
                    //BoxingSpecs = notice["BoxingSpecs"]?.Value<int?>(), //装箱通知中不涉及到, 只有在分拣入库的时候才用到
                    Origin = storageEntity.Origin,
                    Summary = notice["Summary"]?.Value<string>(),
                    StorageID = notice["StorageID"].Value<string>(),
                });
            }
            #endregion
            
            // 更新运单到待拣货状态
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ExcuteStatus = (int)CgPickingExcuteStatus.Picking,
                }, item => item.ID == waybillID);
            }
        }

        /// <summary>
        /// 新的装箱通知
        /// </summary>
        /// <param name="waybill"></param>
        /// <param name="notices"></param>
        public void EnterBoxingNew(JToken waybill, JToken notices)
        {
            #region 检查装箱通知的合理性
            var waybillID = waybill["WaybillID"].Value<string>();
            var adminID = waybill["AdminID"]?.Value<string>();

            var source = (CgNoticeSource)waybill["Source"].Value<int>();
            var noticeType = (CgNoticeType)waybill["NoticeType"].Value<int>();

            if (source != CgNoticeSource.AgentCustomsFromStorage || noticeType != CgNoticeType.Boxing)
            {
                throw new Exception("装箱通知只用于转报关进项装箱!");
            }

            var isValid = true;

            // 判断装箱通知是否合理
            foreach (var notice in notices)
            {
                var storageID = notice["StorageID"].Value<string>();
                var storageEntity = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Single(item => item.ID == storageID);                                    

                // 从Storage中减除对应的数量
                var quantity = notice["Quantity"].Value<decimal>();
                if (storageEntity.Quantity < quantity)
                {
                    isValid = false;
                    break;
                }
            }

            if (!isValid)
            {
                throw new Exception($"Notice中的数量大于库存的数量，不能正确生成装箱通知,请检查!");
            }

            string[] currents_notices = new string[notices.Count()];
            string[] currents_outputs = new string[notices.Count()];

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
            #endregion

            #region 处理重新装箱通知
            int index = 0;
            foreach (var notice in notices)
            {
                var output = notice["Output"];
                var outputID = currents_outputs[index] = this.EnterOutput(output, adminID);
                var storageID = notice["StorageID"].Value<string>();
                var storage = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Single(item => item.ID == storageID);                                  

                string noticeID = this.EnterNotice(new Layers.Data.Sqls.PvWms.Notices
                {
                    #region 视图
                    Type = (int)CgNoticeType.Boxing,
                    WareHouseID = notice["WareHouseID"].Value<string>(),
                    WaybillID = waybillID,
                    InputID = output["InputID"].Value<string>(),
                    OutputID = outputID,
                    ProductID = storage.ProductID,
                    Supplier = storage.Supplier,
                    DateCode = storage.DateCode,
                    Quantity = notice["Quantity"].Value<decimal>(),
                    Conditions = notice["Conditions"].Value<string>(),
                    CreateDate = DateTime.Now,
                    Status = (int)NoticesStatus.Waiting,
                    Source = (int)CgNoticeSource.AgentCustomsFromStorage,
                    Target = (int)NoticesTarget.Default,                    
                    Weight = notice["Weight"]?.Value<decimal?>(),
                    NetWeight = notice["NetWeight"]?.Value<decimal?>(),
                    Volume = notice["Volume"]?.Value<decimal?>(),
                    ShelveID = notice["ShelveID"]?.Value<string>(),                    
                    Origin = storage.Origin,
                    Summary = notice["Summary"]?.Value<string>(),
                    StorageID = storageID,
                    #endregion
                }, this.Reponsitory);
                currents_notices[index++] = noticeID;

                // 需要把原有的拣货与重新入库的通知的关系匹配上
                if (this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Any(item => item.OutputID == outputID))
                {
                    this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Pickings>(new
                    {
                        NoticeID = noticeID,
                    }, item => item.OutputID == outputID);
                }                
            }
            #endregion

            #region 处理完重新装箱通知后，删除多余的通知，拣货，销项，及归还库存
            // 处理完重新装箱的通知后，重新获取装箱通知
            var linq_notices = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                               where notice.WaybillID == waybillID && notice.OutputID != null && notice.Type == (int)CgNoticeType.Boxing && notice.Source == (int)CgNoticeSource.AgentCustomsFromStorage
                               select new
                               {
                                   notice.ID,
                                   notice.OutputID,
                               };

            var ienum_notices = linq_notices.ToArray();

            var exists_noticeID = ienum_notices.Select(item => item.ID).Distinct();
            var exists_outputID = ienum_notices.Select(item => item.OutputID).Distinct();

            var deletes_noticeIDs = exists_noticeID.Where(item => !currents_notices.Contains(item));
            var delete_outputIDs = exists_outputID.Where(item => !currents_outputs.Contains(item));

            var storageViews = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                               join picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
                               on storage.ID equals picking.StorageID
                               join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                               on picking.NoticeID equals notice.ID
                               where deletes_noticeIDs.Contains(picking.NoticeID)
                               select new
                               {
                                   storeStorageID = notice.StorageID,
                                   notice.Quantity,
                                   PickingID = picking.ID,
                                   picking.OutputID,
                                   picking.NoticeID,
                                   flowStorageID = storage.ID,
                               };

            var ienum_storageViews = storageViews.ToArray();

            foreach (var view in ienum_storageViews)
            {
                if (linq_declare.Any(item => item.StorageID == view.flowStorageID && item.Status >= (int)TinyOrderDeclareStatus.Declaring))
                {
                    throw new Exception($"流水库{view.flowStorageID}已经申报，不能在重新装箱通知时进行删除!");
                }
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Storages>(item => item.ID == view.flowStorageID);
                var storeStorage = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Single(item => item.ID == view.storeStorageID);
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                {
                    Quantity = storeStorage.Quantity + view.Quantity,
                }, item => item.ID == view.storeStorageID);

                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Pickings>(item => item.ID == view.PickingID);
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Notices>(item => item.ID == view.NoticeID);
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Outputs>(item => item.ID == view.OutputID);
            }
            #endregion

            #region 转报关重新装箱通知更新后的后续处理
            var excuteStatus = CgCustomsStorageView.CheckAndUpdateWaybillStatus(waybillID, Npc.Robot.ToString(), this.Reponsitory);

            CgCustomsStorageView.CheckWaybillExcuteStatusAndDeclare(waybillID, Npc.Robot.ToString(), excuteStatus, this.Reponsitory);
            #endregion
        }
                
        /// <summary>
        /// 根据ItemID 删除对应的Notice, 库存等
        /// </summary>
        /// <param name="delete">JToken</param>
        public void Delete(JToken delete)
        {
            var itemIds = delete["ItemID"]?.Values<string>().ToArray() ?? new string[0];

            if (itemIds.Count() > 0)
            {
                var linq = from output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                           join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on output.InputID equals input.ID
                           join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on input.ID equals notice.InputID
                           where itemIds.Contains(output.ItemID) && notice.Type == (int)CgNoticeType.Boxing
                           select notice.ID;
                var ienum_noticeIds = linq.ToArray();

                // 删除对应的Pickings
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Pickings>(item => ienum_noticeIds.Contains(item.NoticeID));

                foreach (var noticeid in ienum_noticeIds)
                {
                    var notice = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().SingleOrDefault(item => item.ID == noticeid);

                    // 删除对应的流水库
                    this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Storages>(item => item.Type == (int)CgStoragesType.Flows && item.WareHouseID == notice.WareHouseID && item.InputID == notice.InputID);
                    var storage = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().SingleOrDefault(item => item.Type == (int)CgStoragesType.Stores && item.WareHouseID == notice.WareHouseID && item.InputID == notice.InputID);

                    // 更新流水库存--恢复原库存
                    this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                    {
                        Type = (int)CgStoragesType.Stores,
                        Quantity = storage.Quantity + notice.Quantity,
                    }, item => item.Type == (int)CgStoragesType.Stores && item.WareHouseID == notice.WareHouseID && item.InputID == notice.InputID);

                    // 删除对应的Notices
                    this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Notices>(item => item.ID == notice.ID);

                    // 删除对应的Outputs
                    this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Outputs>(item => item.ID == notice.OutputID);
                }
            }
        }
    }
}
