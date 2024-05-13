using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.chonggous.Models;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Wms.Services.chonggous.Views
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class CgOutNoticesView : CgNoticesView
    {
        public CgOutNoticesView()
        {

        }

        protected override IQueryable<CgNotice> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Type == CgNoticeType.Out);
        }

        /// <summary>
        /// output数据保存
        /// </summary>
        /// <param name="output">output数据</param>
        /// <returns></returns>
        private string OnEnterOutput(JToken output)
        {
            var outputid = output["ID"].Value<string>();
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
                    OwnerID = output["OwnerID"].Value<string>(),
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
                    OwnerID = output["OwnerID"].Value<string>(),
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
        /// notice数据保存
        /// </summary>
        /// <param name="Notice">通知数据</param>
        /// <param name="OutputID">出项ID</param>
        /// <param name="StorageID">流水库ID</param>
        private void OnEnterNotice(JToken Notice, string OutputID, Layers.Data.Sqls.PvWms.Storages CurrentStorage, string waybillID)
        {
            var noticeid = Notice["ID"]?.Value<string>();
            if (string.IsNullOrWhiteSpace(noticeid))
            {
                noticeid = PKeySigner.Pick(PkeyType.Notices);
            }
            //判断Notice数据是否存在
            if (!this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Any(item => item.ID == noticeid))
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Notices
                {
                    ID = noticeid,
                    Type = Notice["Type"].Value<int>(),
                    WareHouseID = Notice["WareHouseID"].Value<string>(),
                    // WaybillID = Notice["WaybillID"].Value<string>(),
                    WaybillID = waybillID,
                    InputID = Notice["InputID"]?.Value<string>(),
                    OutputID = OutputID,
                    ProductID = Notice["ProductID"].Value<string>(),
                    Supplier = Notice["Supplier"]?.Value<string>(),
                    DateCode = Notice["DateCode"]?.Value<string>(),
                    Quantity = Notice["Quantity"].Value<decimal>(),
                    Conditions = Notice["Conditions"].Value<string>() == null ? "" : Notice["Conditions"].Value<string>(),
                    CreateDate = DateTime.Now,
                    Status = (int)(NoticesStatus.Waiting),
                    Source = Notice["Source"].Value<int>(),
                    Target = Notice["Target"].Value<int>(),
                    BoxCode = Notice["BoxCode"]?.Value<string>(),
                    Weight = Notice["Weight"]?.Value<decimal?>(),
                    NetWeight = Notice["NetWeight"]?.Value<decimal?>(),
                    Volume = Notice["Volume"]?.Value<decimal?>(),
                    ShelveID = CurrentStorage.ShelveID,
                    BoxingSpecs = Notice["BoxingSpecs"]?.Value<int?>(),
                    Origin = Notice["Origin"]?.Value<string>(),
                    StorageID = Notice["StorageID"].Value<string>(),
                    CustomsName = Notice["CustomsName"]?.Value<string>(),
                });
            }
            else
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Notices>(new
                {
                    ID = noticeid,
                    Type = Notice["Type"].Value<int>(),
                    WareHouseID = Notice["WareHouseID"].Value<string>(),
                    WaybillID = waybillID,
                    InputID = Notice["InputID"]?.Value<string>(),
                    OutputID = OutputID,
                    ProductID = Notice["InputID"].Value<string>(),
                    Supplier = Notice["Supplier"]?.Value<string>(),
                    DateCode = Notice["DateCode"]?.Value<string>(),
                    Quantity = Notice["Quantity"].Value<decimal>(),
                    Conditions = Notice["Conditions"].Value<string>() == null ? "" : Notice["Conditions"].Value<string>(),
                    CreateDate = DateTime.Now,
                    Status = (int)(NoticesStatus.Waiting),
                    Source = Notice["Source"].Value<int>(),
                    Target = Notice["Target"].Value<int>(),
                    BoxCode = Notice["BoxCode"]?.Value<string>(),
                    Weight = Notice["Weight"]?.Value<decimal?>(),
                    NetWeight = Notice["NetWeight"]?.Value<decimal?>(),
                    Volume = Notice["Volume"]?.Value<decimal?>(),
                    ShelveID = Notice["ShelveID"]?.Value<string>(),
                    BoxingSpecs = Notice["BoxingSpecs"]?.Value<int?>(),
                    Origin = Notice["Origin"]?.Value<string>(),
                    StorageID = Notice["StorageID"].Value<string>(),
                    CustomsName = Notice["CustomsName"]?.Value<string>(),
                }, item => item.ID == noticeid);
            }
        }

        /// <summary>
        /// 运单数据持久化
        /// </summary>
        /// <param name="Waybill">运单数据</param>
        /// <param name="repository">数据库连接实例</param>
        /// <param name="ConsigneeID">收货人ID</param>
        /// <returns></returns>
        private string OnWaybillEnter(JToken Waybill, PvCenterReponsitory repository, string ConsigneeID = null)
        {
            var waybillId = Waybill["WaybillID"]?.Value<string>();
            var carrierName = Waybill["CarrierName"]?.Value<string>();
            var summary = Waybill["Summary"]?.Value<string>();
            string carrierId = "";
            //如果是深圳库房就去承运商名称
            if (!string.IsNullOrEmpty(carrierName))
            {
                var carrier = new Wms.Services.chonggous.Views.CarriersTopView().FirstOrDefault(item => item.Name == carrierName);
                carrierId = carrier.ID;
            }
            if (string.IsNullOrWhiteSpace(waybillId))
            {
                waybillId = PKeySigner.Pick(PkeyType.Waybills);
                repository.Insert(new Layers.Data.Sqls.PvCenter.Waybills
                {
                    ID = waybillId,
                    FatherID = Waybill["FatherID"]?.Value<string>(),
                    Code = Waybill["Code"]?.Value<string>(),
                    Type = Waybill["WaybillType"].Value<int>(),
                    Subcodes = Waybill["Subcodes"]?.Value<string>(),
                    CarrierID = carrierId != "" ? carrierId : null,
                    ConsignorID = Waybill["ConsignorID"]?.Value<string>(),
                    ConsigneeID = ConsigneeID,
                    FreightPayer = Waybill["FreightPayer"].Value<int>(),
                    TotalParts = Waybill["TotalParts"]?.Value<int?>(),
                    TotalWeight = Waybill["TotalWeight"]?.Value<decimal>(),
                    TotalVolume = Waybill["TotalVolume"]?.Value<decimal?>(),
                    CarrierAccount = Waybill["CarrierAccount"]?.Value<string>(),
                    VoyageNumber = Waybill["VoyageNumber"]?.Value<string>(),
                    Condition = Waybill["Condition"] == null ? "" : Waybill["Condition"].Value<string>(),
                    CreateDate = DateTime.Now,
                    EnterCode = Waybill["EnterCode"].Value<string>() == null ? "" : Waybill["EnterCode"].Value<string>(),
                    Status = (int)GeneralStatus.Normal,
                    CreatorID = Waybill["FatherID"].Value<string>(),
                    TransferID = Waybill["FatherID"]?.Value<string>(),
                    IsClearance = Waybill["IsClearance"]?.Value<bool>(),
                    Packaging = Waybill["Packaging"]?.Value<string>(),
                    Supplier = Waybill["Supplier"]?.Value<string>(),
                    ExcuteStatus = Waybill["ExcuteStatus"]?.Value<int>(),
                    CuttingOrderStatus = Waybill["CuttingOrderStatus"]?.Value<int?>(),
                    ConfirmReceiptStatus = Waybill["ConfirmReceiptStatus"]?.Value<int?>(),
                    AppointTime = Waybill["AppointTime"]?.Value<DateTime>(),
                    OrderID = Waybill["OrderID"]?.Value<string>(),
                    Source = Waybill["Source"]?.Value<int?>(),
                    NoticeType = Waybill["NoticeType"]?.Value<int?>(),
                    Summary = summary,
                    TempEnterCode = Waybill["TempEnterCode"]?.Value<string>(),
                });
            }
            else
            {
                repository.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    Type = Waybill["Type"].Value<int>(),
                    Supplier = Waybill["Supplier"]?.Value<string>(),
                    Summary = summary,
                }, item => item.ID == waybillId);
            }

            return waybillId;
        }

        private void OnWaybillConsignorEnter(JToken Express, PvCenterReponsitory repository, string ConsignorID, string WaybillID, WaybillType wayBillType)
        {

            repository.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
            {
                ConsignorID = ConsignorID,
            }, item => item.ID == WaybillID);
            if (wayBillType == WaybillType.LocalExpress || wayBillType == WaybillType.InternationalExpress)
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PvCenter.WayExpress>().Any(item => item.ID == WaybillID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvCenter.WayExpress
                    {
                        ID = WaybillID,
                        ExType = Express["ExType"]?.Value<int>(),
                        ExPayType = Express["ExPayType"]?.Value<int>(),
                        ThirdPartyCardNo = Express["ThirdPartyCardNo"]?.Value<string>(),
                    });
                }
                else
                {
                    ///快递方式
                    repository.Update<Layers.Data.Sqls.PvCenter.WayExpress>(new
                    {
                        ExType = Express["ExType"]?.Value<int>(),
                        ExPayType = Express["ExPayType"]?.Value<int>(),
                        ThirdPartyCardNo = Express["ThirdPartyCardsNo"]?.Value<string>(),
                    }, item => item.ID == WaybillID);

                }

            }

        }
        /// <summary>
        /// 联系人数据持久化
        /// </summary>
        /// <param name="Wayparter">联系人数据</param>
        /// <param name="reponsitory">数据库连接实例</param>
        /// <returns></returns>
        private string OnWayParterEnter(JToken Wayparter, PvCenterReponsitory reponsitory)
        {
            var WayParter = new Layers.Data.Sqls.PvCenter.WayParters()
            {
                Company = Wayparter["Company"].Value<string>(),
                Place = Wayparter["Place"].Value<string>(),
                Address = Wayparter["Address"].Value<string>(),
                Contact = Wayparter["Contact"].Value<string>(),
                Phone = Wayparter["Phone"].Value<string>(),
                Zipcode = Wayparter["Zipcode"]?.Value<string>(),
                Email = Wayparter["Email"]?.Value<string>(),
                CreateDate = DateTime.Now,
                IDType = Wayparter["IDType"]?.Value<int>(),
                IDNumber = Wayparter["IDNumber"]?.Value<string>(),
            };
            //主键MD5
            WayParter.ID = string.Concat(WayParter.Company, WayParter.Place, WayParter.Address, WayParter.Contact,
                WayParter.Phone, WayParter.Zipcode, WayParter.Email).MD5();

            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Any(item => item.ID == WayParter.ID))
            {
                reponsitory.Insert(WayParter);
            }
            else
            {
                reponsitory.Update(WayParter, item => item.ID == WayParter.ID);
            }

            return WayParter.ID;
        }

        /// <summary>
        /// 送货上门信息持久化
        /// </summary>
        /// <param name="WayLoading">送货上门</param>
        /// <param name="WaybillID">主键ID</param>
        /// <param name="reponsitory">数据库连接实例</param>
        /// <returns></returns>
        private void OnWayLoadingEnter(JToken WayLoading, string WaybillID, PvCenterReponsitory reponsitory)
        {
            var drivername = WayLoading["Driver"]?.Value<string>();
            var carNumner = WayLoading["CarNumber1"]?.Value<string>();
            //咱没考虑司机重名的问题
            var driver = new Yahv.Services.Views.DriversTopView().FirstOrDefault(item => item.Name == drivername);
            var car = new Wms.Services.Views.TransportTopView().FirstOrDefault(item => item.CarNumber1 == carNumner);
            if (driver == null)
            {
                throw new NotImplementedException("司机信息未和芯达通同步");
            }
            if (car == null)
            {
                throw new NotImplementedException("运输信息未和芯达通同步");
            }

            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayLoadings>().Any(item => item.ID == WaybillID))
            {

                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayLoadings
                {
                    ID = WaybillID,
                    TakingDate = WayLoading["TakingDate"].Value<DateTime>(),
                    TakingAddress = WayLoading["TakingAddress"].Value<string>(),
                    TakingPhone = WayLoading["TakingPhone"].Value<string>(),
                    CarNumber1 = carNumner,
                    Driver = drivername,
                    Carload = WayLoading["Carload"]?.Value<int?>(),
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    CreatorID = WayLoading["CreatorID"].Value<string>() ?? "",
                    ExcuteStatus = (int)CgLoadingExcuteStauts.Waiting,
                    Carrier = driver.EnterpriseID
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.WayLoadings>(new
                {
                    TakingDate = WayLoading["TakingDate"].Value<DateTime>(),
                    TakingAddress = WayLoading["TakingAddress"].Value<string>(),
                    TakingPhone = WayLoading["TakingPhone"].Value<string>(),
                    CarNumber1 = WayLoading["CarNumber1"]?.Value<string>(),
                    Driver = WayLoading["Driver"]?.Value<string>(),
                    Carload = WayLoading["Carload"]?.Value<int?>(),
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    CreatorID = WayLoading["CreatorID"].Value<string>() ?? "",
                    ExcuteStatus = (int)CgLoadingExcuteStauts.Waiting,
                    Carrier = driver.EnterpriseID
                }, item => item.ID == WaybillID);
            }
        }

        /// <summary>
        /// 如果InputID对应的Input不存在时, 生成对应inputID的Input,
        /// </summary>
        /// <param name="inputID">要生成的Input的inputID</param>
        /// <param name="storageInputID">原Storage中的InputID</param>
        private string OnInputEnter(JToken Notice, string storageInputID)
        {
            var inputID = Notice["InputID"]?.Value<string>();
            if (!this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Any(item => item.ID == inputID))
            {
                return storageInputID;
            }
            return inputID;
        }

        /// <summary>
        /// 流水库持久化
        /// </summary>
        /// <param name="Notice">通知数据</param>
        /// <param name="storage">库存库数据</param>
        /// <returns></returns>
        private void OnStorageEnter(JToken Notice, Layers.Data.Sqls.PvWms.Storages storage)
        {
            if (storage.Type == (int)CgStoragesType.Flows)
            {
                return;
            }

            //生成流水库数据,修改库存库数据
            this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
            {
                Quantity = storage.Quantity - Notice["Quantity"].Value<int>(),
            }, item => item.ID == storage.ID);

            //如果InputID对应的Input不存在, 则生成对应的Input
            var inputID = OnInputEnter(Notice, storage.InputID);

            #region 插入对应的库存流水库

            this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Storages
            {
                ID = PKeySigner.Pick(PkeyType.Storages),
                Type = (int)CgStoragesType.Flows,
                WareHouseID = Notice["WareHouseID"]?.Value<string>(),
                SortingID = storage.SortingID,
                InputID = inputID,
                ProductID = Notice["ProductID"].Value<string>(),
                Total = Notice["Quantity"].Value<decimal>(),
                Quantity = Notice["Quantity"].Value<decimal>(),
                Origin = storage.Origin,
                IsLock = false,
                CreateDate = DateTime.Now,
                Status = (int)GeneralStatus.Normal,
                //ShelveID = Notice["ShelveID"]?.Value<string>(),
                //Supplier = Notice["Supplier"]?.Value<string>(),
                //DateCode = Notice["DateCode"]?.Value<string>(),
                // 修改使得新生成的流水库的对应货架, 供应商, 批次号能拿到以前库存的对应数据
                ShelveID = storage.ShelveID,
                Supplier = storage.Supplier,
                DateCode = storage.DateCode,
            });

            #endregion
        }

        /// <summary>
        /// 取消订单时,恢复原库存数据
        /// </summary>
        /// <param name="Notice"></param>
        /// <param name="storage">这个是通知的库存</param>
        private void OnStorageCancel(CgNotice notice, /*decimal quantity, string warehouseId,*/
            Layers.Data.Sqls.PvWms.Storages storage)
        {
            //查看是否 有出库？
            //notice.Quantity, notice.WareHouseID,

            if (notice.Type == CgNoticeType.Out)
            {
                //现在有picking 就算是分拣，因此只能这样做
                if (this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Any(item => item.NoticeID == notice.ID))
                {
                    throw new NotImplementedException("未实现已出库的取消！");
                }

                //假定是装箱通知，同时还完成部分报关（其实就是出库）
                if (this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Any(item => item.OutputID == notice.OutputID))
                {
                    throw new NotImplementedException("未实现已出库的取消！");
                }
            }

            // 注意下面的删除顺序不能改变,否则违反数据库约束


            if (storage.Type == (int)CgStoragesType.Flows)
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                {
                    Quantity = storage.Quantity + notice.Quantity,
                }, item => item.ID == storage.ID);
            }

            //修改库存库数据,回复原库存的数量

            var pickings = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().
               Where(item => item.NoticeID == notice.ID).ToArray();

            if (storage.Type == (int)CgStoragesType.Stores)
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                {
                    Quantity = storage.Quantity + pickings.Sum(item => item.Quantity),
                }, item => item.ID == notice.StorageID);

                var pstoragesID = pickings.Select(item => item.StorageID);

                //// 删除CurrentStorage对应的流水库存,
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Storages>(item => pstoragesID.Contains(item.ID));
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Pickings>(item => item.NoticeID == notice.ID);

            }
        }

        /// <summary>
        /// Waybill数据持久化逻辑
        /// </summary>
        /// <param name="Waybill"></param>
        public string WaybillEnter(JToken Waybill)
        {
            //运单数据处理逻辑
            using (var repository = new PvCenterReponsitory())
            {
                var waybillId = Waybill["WaybillID"]?.Value<string>();
                //var orderRequirement = Waybill["OrderRequirement"].Value<string>();
                var orderRequirement = Waybill["Requirement"]?.ToObject<OrderRequirement[]>();
                var checkRequirement = Waybill["CheckRequirement"]?.ToObject<CheckRequirement>();

                if (string.IsNullOrWhiteSpace(waybillId))
                {
                    var consignee = Waybill["Consignee"];
                    // 添加收件人
                    var ConsigneeID = this.OnWayParterEnter(consignee, repository);
                    waybillId = this.OnWaybillEnter(Waybill, repository, ConsigneeID);
                    //添加发货人信息
                    var consignor = Waybill["Consignor"];
                    var waybillType = Waybill["WaybillType"].Value<int>();
                    var Express = Waybill["Express"];
                    if (!string.IsNullOrEmpty(consignor.ToString()))
                    {
                        var ConsignorID = this.OnWayParterEnter(consignor, repository);
                        this.OnWaybillConsignorEnter(Express, repository, ConsignorID, waybillId, (WaybillType)waybillType);
                    }
                    //WayLoading 存的是承运信息 
                    var wayloading = Waybill["WayLoading"];
                    if (!string.IsNullOrEmpty(wayloading.ToString()))
                    {
                        this.OnWayLoadingEnter(wayloading, waybillId, repository);
                    }
                }
                else
                {
                    // 当订单要求不为空时，存入订单的特殊要求, 主要针对代发货进行保存, 代转运的出库通知是自动生成的所以走不到此逻辑
                    if (orderRequirement != null && orderRequirement.Count() > 0)
                    {
                        bool exist = repository.ReadTable<Layers.Data.Sqls.PvCenter.WayRequirements>().Any(item => item.ID == waybillId);

                        if (exist)
                        {
                            repository.Update<Layers.Data.Sqls.PvCenter.WayRequirements>(new
                            {
                                OrderRequirement = orderRequirement.Json(),
                            }, item => item.ID == waybillId);
                        }
                        else
                        {
                            repository.Insert(new Layers.Data.Sqls.PvCenter.WayRequirements
                            {
                                ID = waybillId,
                                OrderRequirement = orderRequirement.Json(),
                            });
                        }
                    }

                    if (checkRequirement != null)
                    {
                        bool exist = repository.ReadTable<Layers.Data.Sqls.PvCenter.WayRequirements>().Any(item => item.ID == waybillId);
                        if (checkRequirement.ApplicationID != null || checkRequirement.DelivaryOpportunity != null || checkRequirement.IsPayCharge != null)
                        {
                            if (exist)
                            {
                                repository.Update<Layers.Data.Sqls.PvCenter.WayRequirements>(new
                                {
                                    CheckRequirement = checkRequirement.Json(),
                                    DelivaryOpportunity = (int)checkRequirement.DelivaryOpportunity,
                                }, item => item.ID == waybillId);
                            }
                            else
                            {
                                repository.Insert(new Layers.Data.Sqls.PvCenter.WayRequirements
                                {
                                    ID = waybillId,
                                    DelivaryOpportunity = (int)checkRequirement.DelivaryOpportunity,
                                    CheckRequirement = checkRequirement.Json(),
                                });
                            }
                        }
                    }

                    this.OnWaybillEnter(Waybill, repository);
                }
                
                return waybillId;
            }
        }

        /// <summary>
        /// 出库通知数据持久化逻辑
        /// </summary>
        /// <param name="Notices">通知数据</param>
        public void OutNoticeEnter(JToken Notices, string waybillID)
        {
            var checkers = Notices.Select(item => new
            {
                StorageID = item["StorageID"].Value<string>(),
                Quantity = item["Quantity"].Value<decimal>()
            });

            var storagesID = checkers.Select(item => item.StorageID);
            var storages = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(item => storagesID.Contains(item.ID)).ToArray();

            if (checkers.Any(nchecker =>
            {
                var storage = storages.SingleOrDefault(s => s.ID == nchecker.StorageID);
                if (storage == null)
                {
                    return true;
                }
                return storage.Quantity < nchecker.Quantity;
            }))
            {
                throw new Exception("库存不足!");
            }


            foreach (var notice in Notices)
            {
                //校验库存是否充足
                var storage = storages.SingleOrDefault(item => item.ID == notice["StorageID"].Value<string>());
                this.OnStorageEnter(notice, storage);

                var output = notice["Output"];

                //出项数据持久化
                var outputid = this.OnEnterOutput(output);

                //通知数据持久化
                this.OnEnterNotice(notice, outputid, storage, waybillID);
            }
        }

        /// <summary>
        /// 取消订单时,数据的持久化
        /// </summary>
        /// <param name="Notices"></param>
        public void OutNoticeCancel(string waybillId)
        {
            var noticeView = this.IQueryable.Cast<CgNotice>().Where(item => item.WaybillID == waybillId);
            var ienum_notices = noticeView.ToArray();

            foreach (var notice in ienum_notices)
            {
                var storage = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().SingleOrDefault(item => item.ID == notice.StorageID);

                this.OnStorageCancel(notice, storage);





                // 删除订单时,删除通知对应的Picking, 删除对应的出库通知                
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Notices>(item => item.ID == notice.ID);

                // 取消订单时删除Notice中对应的Output
                this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Outputs>(item => item.ID == notice.OutputID);
            }

            // 最后删除对应的运单
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                pvcenterReponsitory.Delete<Layers.Data.Sqls.PvCenter.WayCharges>(item => item.ID == waybillId);
                pvcenterReponsitory.Delete<Layers.Data.Sqls.PvCenter.WayChcd>(item => item.ID == waybillId);
                pvcenterReponsitory.Delete<Layers.Data.Sqls.PvCenter.WayLoadings>(item => item.ID == waybillId);
                pvcenterReponsitory.Delete<Layers.Data.Sqls.PvCenter.WayExpress>(item => item.ID == waybillId);
                pvcenterReponsitory.Delete<Layers.Data.Sqls.PvCenter.Waybills>(item => item.ID == waybillId);
            }
        }


        #region  深圳库房出库

        /// <summary>
        /// Waybill数据持久化逻辑
        /// </summary>
        /// <param name="Waybill"></param>
        public string SZWaybillEnter(JToken Waybill)
        {
            //运单数据处理逻辑
            using (var repository = new PvCenterReponsitory())
            {
                var waybillId = Waybill["WaybillID"]?.Value<string>();
                var waybillType = Waybill["WaybillType"].Value<int>();
                var type = (WaybillType)waybillType;

                var consignee = Waybill["Consignee"];
                switch (type)
                {
                    case WaybillType.PickUp:
                        // 提货地址
                        var ConsigneeIDpickUp = this.OnWayParterEnter(consignee, repository);
                        waybillId = this.SZOnWaybillEnter(Waybill, repository, ConsigneeIDpickUp);
                        break;
                    case WaybillType.DeliveryToWarehouse:
                        // 添加收件人
                        var ConsigneeID = this.OnWayParterEnter(consignee, repository);
                        //发货人
                        var consignor = Waybill["Consignor"];
                        var wayloading = Waybill["WayLoading"];
                        var ConsignorID = this.OnWayParterEnter(consignor, repository);
                        waybillId = this.SZOnWaybillEnter(Waybill, repository, ConsigneeID, ConsignorID);
                        //WayLoading 存的是承运信息 
                        this.SZOnWayLoadingEnter(wayloading, waybillId, repository);
                        break;
                    case WaybillType.LocalExpress:
                        // 添加收件人
                        var expConsigneeID = this.OnWayParterEnter(consignee, repository);
                        //发货人
                        var Express = Waybill["Express"];
                        var expconsignor = Waybill["Consignor"];
                        var expwayloading = Waybill["WayLoading"];
                        var expConsignorID = this.OnWayParterEnter(expconsignor, repository);
                        waybillId = this.SZOnWaybillEnter(Waybill, repository, expConsigneeID, expConsignorID);
                        this.SZOnExpressEnter(Express,waybillId, repository);
                        break;

                    default:
                        break;
                }
                return waybillId;
            }
        }


      /// <summary>
      /// 
      /// </summary>
      /// <param name="Waybill"></param>
      /// <param name="repository"></param>
      /// <param name="ConsigneeID">收件信息ID</param>
      /// <param name="ConsinorID">发货信息ID</param>
      /// <returns></returns>
        private string SZOnWaybillEnter(JToken Waybill, PvCenterReponsitory repository, string ConsigneeID = null, string ConsinorID = null)
        {
            var waybillId = Waybill["WaybillID"]?.Value<string>();
            var carrierName = Waybill["CarrierName"]?.Value<string>();
            string carrierId = "";
            //深圳库房根据承运商名称获取承运ID
            if (!string.IsNullOrEmpty(carrierName))
            {
                var carrier = new Wms.Services.chonggous.Views.CarriersTopView().FirstOrDefault(item => item.Name == carrierName);
                carrierId = carrier.ID;
            }
            if (string.IsNullOrWhiteSpace(waybillId))
            {
                waybillId = PKeySigner.Pick(PkeyType.Waybills);
                repository.Insert(new Layers.Data.Sqls.PvCenter.Waybills
                {
                    ID = waybillId,
                    FatherID = Waybill["FatherID"]?.Value<string>(),
                    Code = Waybill["Code"]?.Value<string>(),
                    Type = Waybill["WaybillType"].Value<int>(),
                    Subcodes = Waybill["Subcodes"]?.Value<string>(),
                    CarrierID = carrierId != "" ? carrierId : null,
                    ConsignorID = ConsinorID,
                    ConsigneeID = ConsigneeID,
                    FreightPayer = Waybill["FreightPayer"].Value<int>(),
                    TotalParts = Waybill["TotalParts"]?.Value<int?>(),
                    TotalWeight = Waybill["TotalWeight"]?.Value<decimal>(),
                    TotalVolume = Waybill["TotalVolume"]?.Value<decimal?>(),
                    CarrierAccount = Waybill["CarrierAccount"]?.Value<string>(),
                    VoyageNumber = Waybill["VoyageNumber"]?.Value<string>(),
                    Condition = Waybill["Condition"] == null ? "" : Waybill["Condition"].Value<string>(),
                    CreateDate = DateTime.Now,
                    EnterCode = Waybill["EnterCode"].Value<string>() == null ? "" : Waybill["EnterCode"].Value<string>(),
                    Status = (int)GeneralStatus.Normal,
                    CreatorID = Waybill["FatherID"].Value<string>(),
                    TransferID = Waybill["FatherID"]?.Value<string>(),
                    IsClearance = Waybill["IsClearance"]?.Value<bool>(),
                    Packaging = Waybill["Packaging"]?.Value<string>(),
                    Supplier = Waybill["Supplier"]?.Value<string>(),
                    ExcuteStatus = Waybill["ExcuteStatus"]?.Value<int>(),
                    CuttingOrderStatus = Waybill["CuttingOrderStatus"]?.Value<int?>(),
                    ConfirmReceiptStatus = Waybill["ConfirmReceiptStatus"]?.Value<int?>(),
                    AppointTime = Waybill["AppointTime"]?.Value<DateTime>(),
                    OrderID = Waybill["OrderID"]?.Value<string>(),
                    Source = Waybill["Source"]?.Value<int?>(),
                    NoticeType = Waybill["NoticeType"]?.Value<int?>(),
                    TempEnterCode = Waybill["TempEnterCode"]?.Value<string>(),
                });
            }
            else
            {
                repository.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    Type = Waybill["Type"].Value<int>(),
                    Supplier = Waybill["Supplier"]?.Value<string>(),
                }, item => item.ID == waybillId);


            }

            return waybillId;
        }

        /// <summary>
        /// 送货上门信息持久化
        /// </summary>
        /// <param name="WayLoading">送货上门</param>
        /// <param name="WaybillID">主键ID</param>
        /// <param name="reponsitory">数据库连接实例</param>
        /// <returns></returns>
        private void SZOnWayLoadingEnter(JToken WayLoading, string WaybillID, PvCenterReponsitory reponsitory)
        {
            var drivername = WayLoading["Driver"]?.Value<string>();
            var carNumner = WayLoading["CarNumber1"]?.Value<string>();
            //咱没考虑司机重名的问题
            var driver = new Yahv.Services.Views.DriversTopView().FirstOrDefault(item => item.Name == drivername);
            var car = new Wms.Services.Views.TransportTopView().FirstOrDefault(item => item.CarNumber1 == carNumner);
            if (driver == null)
            {
                throw new NotImplementedException("司机信息未和芯达通同步");
            }
            if (car == null)
            {
                throw new NotImplementedException("运输信息未和芯达通同步");
            }

            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayLoadings>().Any(item => item.ID == WaybillID))
            {

                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayLoadings
                {
                    ID = WaybillID,
                    TakingDate = WayLoading["TakingDate"].Value<DateTime>(),
                    TakingAddress = WayLoading["TakingAddress"].Value<string>(),
                    TakingPhone = WayLoading["TakingPhone"].Value<string>(),
                    CarNumber1 = carNumner,
                    Driver = drivername,
                    Carload = WayLoading["Carload"]?.Value<int?>(),
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    CreatorID = WayLoading["CreatorID"].Value<string>() ?? "",
                    ExcuteStatus = (int)CgLoadingExcuteStauts.Waiting,
                    Carrier = driver.EnterpriseID
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.WayLoadings>(new
                {
                    TakingDate = WayLoading["TakingDate"].Value<DateTime>(),
                    TakingAddress = WayLoading["TakingAddress"].Value<string>(),
                    TakingPhone = WayLoading["TakingPhone"].Value<string>(),
                    CarNumber1 = WayLoading["CarNumber1"]?.Value<string>(),
                    Driver = WayLoading["Driver"]?.Value<string>(),
                    Carload = WayLoading["Carload"]?.Value<int?>(),
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    CreatorID = WayLoading["CreatorID"].Value<string>() ?? "",
                    ExcuteStatus = (int)CgLoadingExcuteStauts.Waiting,
                    Carrier = driver.EnterpriseID
                }, item => item.ID == WaybillID);
            }
        }

        /// <summary>
        /// 快递类型
        /// </summary>
        /// <param name="Express"></param>
        /// <param name="WaybillID"></param>
        /// <param name="repository"></param>
        private void SZOnExpressEnter(JToken Express, string WaybillID, PvCenterReponsitory repository)
        {
            if (!repository.ReadTable<Layers.Data.Sqls.PvCenter.WayExpress>().Any(item => item.ID == WaybillID))
            {
                repository.Insert(new Layers.Data.Sqls.PvCenter.WayExpress
                {
                    ID = WaybillID,
                    ExType = Express["ExType"]?.Value<int>(),
                    ExPayType = Express["ExPayType"]?.Value<int>(),
                    ThirdPartyCardNo = Express["ThirdPartyCardNo"]?.Value<string>(),
                });
            }
            else
            {
                ///快递方式
                repository.Update<Layers.Data.Sqls.PvCenter.WayExpress>(new
                {
                    ExType = Express["ExType"]?.Value<int>(),
                    ExPayType = Express["ExPayType"]?.Value<int>(),
                    ThirdPartyCardNo = Express["ThirdPartyCardNo"]?.Value<string>(),
                }, item => item.ID == WaybillID);

            }


        }
        #endregion

    }
}
