//using Layers.Data;
//using Layers.Data.Sqls;
//using Layers.Data.Sqls.PvWms;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Transactions;
//using Wms.Services.Enums;
//using Wms.Services.Models;
//using Wms.Services.Views;
//using Yahv.Services.Extends;
//using Yahv.Services.Models;
//using Yahv.Underly;
//using Yahv.Underly.Enums;
//using Yahv.Underly.Erps;
//using Yahv.Usually;
//using Yahv.Utils.Converters.Contents;

//namespace Wms.Services
//{
//    public class StorageServices
//    {
//        public StorageServices()
//        {

//        }

//        IErpAdmin admin;
//        public StorageServices(IErpAdmin admin)
//        {
//            this.admin = admin;
//        }

//        public event SuccessHanlder Success;
//        public event ErrorHanlder Failed;
//        public event ErrorHanlder ShelveNotExist;
//        public event ErrorHanlder StoragesIsNull;
//        public void TempStorageEnter(TempStorageWaybill waybill)
//        {

//            List<CenterProduct> products = new List<CenterProduct>();

//            using (var rep = new PvWmsRepository())
//            {

//                if (waybill.Files != null)
//                {
//                    foreach (var file in waybill.Files)
//                    {
//                        if (string.IsNullOrEmpty(file.AdminID))
//                        {
//                            file.AdminID = admin.ID;
//                        }
//                    }
//                }

//                if (waybill.SummaryStorages != null)
//                {

//                    foreach (var storage in waybill.SummaryStorages)
//                    {
//                        storage.ID = PKeySigner.Pick(PkeyType.Storages);
//                        var soringID = PKeySigner.Pick(PkeyType.Sortings, null);
//                        storage.Sorting = new Sorting
//                        {
//                            ID = soringID,
//                            AdminID = admin.ID,
//                            CreateDate = DateTime.Now,
//                            Quantity = storage.Quantity,
//                        };
//                        storage.ProductID = "";
//                    }
//                }

//                if (waybill.ProductStorages != null)
//                {
//                    foreach (var storage in waybill.ProductStorages)
//                    {
//                        storage.ID = PKeySigner.Pick(PkeyType.Storages);
//                        var soringID = PKeySigner.Pick(PkeyType.Sortings, null);
//                        storage.Sorting = new Sorting
//                        {
//                            ID = soringID,
//                            AdminID = admin.ID,
//                            CreateDate = DateTime.Now,
//                            Quantity = storage.Quantity,
//                        };

//                        var product = new CenterProduct { PartNumber = storage.Product.PartNumber, Manufacturer = storage.Product.Manufacturer, PackageCase = storage.Product.PackageCase, Packaging = storage.Product.Packaging };

//                        var productd = rep.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>().Where(item => item.ID == product.ID).FirstOrDefault();

//                        if (productd == null)
//                        {
//                            products.Add(product);
//                        }

//                        storage.ProductID = product.ID;
//                    }
//                }

//                #region 运单编号为空则为添加
//                if (string.IsNullOrWhiteSpace(waybill.WaybillID))
//                {

//                    //发货人编号
//                    waybill.ConsignorID = Guid.NewGuid().ToString().MD5();

//                    //收货人编号
//                    waybill.ConsigneeID = Guid.NewGuid().ToString().MD5();

//                    //运单编号
//                    var waybillID = PKeySigner.Pick(PKeyType.Waybill);

//                    //入库单号
//                    var tempEnterCode= PKeySigner.Pick(PKeyType.TempEnterCode);

//                    using (var trans = new TransactionScope())
//                    {
//                        try
//                        {
//                            //rep.Command($"insert into ")

//                            //新增发货地(主要保存原产地)
//                            rep.Command($"insert into {nameof(WaybillsTopView)} ({nameof(WaybillsTopView.corID)},{nameof(WaybillsTopView.corCompany)},{nameof(WaybillsTopView.corPlace)},{nameof(WaybillsTopView.corAddress)},{nameof(WaybillsTopView.corContact)},{nameof(WaybillsTopView.corPhone)},{nameof(WaybillsTopView.corZipcode)},{nameof(WaybillsTopView.corEmail)},{nameof(WaybillsTopView.corCreateDate)}) values('{waybill.ConsignorID}','','{waybill.Consignor.Place}','','{waybill.Consignor.Contact}','{waybill.Consignor.Phone}','','','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");

//                            //新增收货地(主外键约束：不添加收货地信息无法添加运单信息)
//                            rep.Command($"insert into {nameof(WaybillsTopView)} ({nameof(WaybillsTopView.corID)},{nameof(WaybillsTopView.corCompany)},{nameof(WaybillsTopView.corPlace)},{nameof(WaybillsTopView.corAddress)},{nameof(WaybillsTopView.corContact)},{nameof(WaybillsTopView.corPhone)},{nameof(WaybillsTopView.corZipcode)},{nameof(WaybillsTopView.corEmail)},{nameof(WaybillsTopView.corCreateDate)}) values('{waybill.ConsigneeID}','','','','','','','','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");

//                            //新增运单
//                            rep.Command($"insert into {nameof(WaybillsTopView)} ({nameof(WaybillsTopView.wbID)},{nameof(WaybillsTopView.wbCode)},{nameof(WaybillsTopView.wbType)},{nameof(WaybillsTopView.wbCarrierID)},{nameof(WaybillsTopView.wbConsignorID)},{nameof(WaybillsTopView.wbConsigneeID)},{nameof(WaybillsTopView.wbFreightPayer)},{nameof(WaybillsTopView.wbCondition)},{nameof(WaybillsTopView.wbCreateDate)},{nameof(WaybillsTopView.wbModifyDate)},{nameof(WaybillsTopView.wbEnterCode)},{nameof(WaybillsTopView.wbStatus)},{nameof(WaybillsTopView.wbExcuteStatus)},{nameof(WaybillsTopView.wbSummary)},{nameof(WaybillsTopView.TempEnterCode)}) values('{waybillID}','{waybill.Code}','{(int)waybill.WaybillType}','{waybill.CarrierID}','{waybill.ConsignorID}','{waybill.ConsigneeID}','','','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{waybill.EnterCode}','{(int)GeneralStatus.Normal}','{(int)waybill.ExcuteStatus}','{waybill.Summary}','{tempEnterCode}')");

//                            //保存运单日志
//                            rep.Command($"update {nameof(Logs_WaybillsTopView)} set IsCurrent=0 where MainID='{waybillID}'");
//                            rep.Command($"insert into {nameof(Logs_WaybillsTopView)} values('{Guid.NewGuid().ToString().MD5()}','{waybillID}',{(int)WaybillStatusType.ExecutionStatus},{(int)waybill.ExcuteStatus},getdate(),'{admin.ID ?? ""}',1)");

//                            if (waybill.Files != null)
//                            {
//                                foreach (var item in waybill.Files)
//                                {
//                                    item.WaybillID = waybillID;
//                                }
//                                waybill.Files.Update(rep);
//                            }

//                            //保存产品信息
//                            foreach (var product in products)
//                            {
//                                try
//                                {
//                                    var sql = $"insert {nameof(ProductsTopView)} values('{product.ID}','{product.PartNumber}','{product.Manufacturer}','{product.PackageCase}','{product.Packaging}',getdate())";
//                                    rep.Command(sql);
//                                }
//                                catch
//                                { }
//                            }

//                            //支持产品存储和描述存储
//                            //1.支持描述存储：保存带Summary（描述）的分拣和库存信息
//                            if (waybill.SummaryStorages != null)
//                            {

//                                foreach (var storage in waybill.SummaryStorages)
//                                {
//                                    //保存分拣信息
//                                    rep.Insert(new Sortings
//                                    {
//                                        ID = storage.Sorting.ID,
//                                        NoticeID = null,
//                                        BoxCode = storage.Sorting.BoxCode,
//                                        Quantity = storage.Sorting.Quantity,
//                                        AdminID = storage.Sorting.AdminID,
//                                        CreateDate = DateTime.Now,
//                                        Weight = storage.Sorting.Weight,
//                                        Volume = storage.Sorting.Volume,
//                                        WaybillID = waybillID
//                                    });

//                                    //保存库存信息
//                                    rep.Insert(new Storages
//                                    {
//                                        ID = storage.ID,
//                                        Type = (int)StoragesType.StagingStock,
//                                        WareHouseID = storage.WareHouseID,
//                                        SortingID = storage.Sorting.ID,
//                                        InputID = "",
//                                        ProductID = storage.ProductID,
//                                        Quantity = storage.Quantity,
//                                        DateCode = "",
//                                        Supplier = "",
//                                        IsLock = false,
//                                        CreateDate = DateTime.Now,
//                                        Status = (int)Enums.StoragesStatus.Normal,
//                                        ShelveID = storage.ShelveID,
//                                        Origin = storage.Place/* ((int)storage.Place).ToString()*/,
//                                        Summary = storage.Summary
//                                    });
//                                }

//                            }


//                            //2.支持产品存储：保存带产品的分拣和库存信息
//                            if (waybill.ProductStorages != null)
//                            {
//                                foreach (var storage in waybill.ProductStorages)
//                                {
//                                    //保存分拣信息
//                                    rep.Insert(new Sortings
//                                    {
//                                        ID = storage.Sorting.ID,
//                                        NoticeID = null,
//                                        BoxCode = storage.Sorting.BoxCode,
//                                        Quantity = storage.Sorting.Quantity,
//                                        AdminID = storage.Sorting.AdminID,
//                                        CreateDate = DateTime.Now,
//                                        Weight = storage.Sorting.Weight,
//                                        Volume = storage.Sorting.Volume,
//                                        WaybillID = waybillID
//                                    });

//                                    //保存库存信息
//                                    rep.Insert(new Storages
//                                    {
//                                        ID = storage.ID,
//                                        Type = (int)StoragesType.StagingStock,
//                                        WareHouseID = storage.WareHouseID,
//                                        SortingID = storage.Sorting.ID,
//                                        InputID = "",
//                                        ProductID = storage.ProductID,
//                                        Quantity = storage.Quantity,
//                                        DateCode = storage.DateCode,
//                                        Supplier = "",
//                                        IsLock = false,
//                                        CreateDate = DateTime.Now,
//                                        Status = (int)Enums.StoragesStatus.Normal,
//                                        ShelveID = storage.ShelveID,
//                                        Origin = storage.Place/*((int)storage.Place).ToString()*/,
//                                        Summary = storage.Summary
//                                    });
//                                }
//                            }

//                            trans.Complete();
//                            if (this != null && this.Success != null)
//                            {
//                                this.Success(this, new SuccessEventArgs("Success!!"));
//                            }

//                        }
//                        catch (Exception ex)
//                        {
//                            if (this != null && this.Failed != null)
//                            {
//                                this.Failed(this, new ErrorEventArgs("Failed!!"));
//                            }
//                        }
//                        finally
//                        {
//                            trans.Dispose();
//                        }
//                    }
//                }
//                #endregion

//                #region 运单编号不为空则是修改

//                else
//                {
//                    //删除库存/分拣/文件信息，重新添加

//                    //获取所有分拣信息
//                    var sortings = new Yahv.Services.Views.SortingsView().Where(item => item.WaybillID == waybill.WaybillID);

//                    //获取所有库存信息
//                    var storages = new List<Storages>();
//                    foreach (var sorting in sortings)
//                    {
//                        var storage = rep.ReadTable<Storages>().Where(item=>item.SortingID==sorting.ID).FirstOrDefault();
//                        //new Views.StoragesView().Where(item => item.SortingID == sorting.ID).FirstOrDefault();
//                        storages.Add(storage);
//                    }

//                    //获取所有文件信息
//                    var files = new Yahv.Services.Views.CenterFilesTopView().Where(item => item.WaybillID == waybill.WaybillID);

//                    //获取所有分拣ID
//                    var sortingids = sortings.Select(item => item.ID).ToArray();
//                    //获取所有库存ID
//                    var storageids = storages.Select(item => item.ID).ToArray();

//                    using (var trans = new TransactionScope())
//                    {
//                        try
//                        {
//                            //先删除库存/分拣/文件信息
//                            rep.Delete<Layers.Data.Sqls.PvWms.Sortings>(item => sortingids.Contains(item.ID));
//                            rep.Delete<Layers.Data.Sqls.PvWms.Storages>(item => storageids.Contains(item.ID));

//                            rep.Command($"delete from {nameof(FilesDescriptionTopView)} where WaybillID='{waybill.WaybillID}'");


//                            //修改发货地
//                            rep.Command($"update {nameof(WaybillsTopView)} set {nameof(WaybillsTopView.corPlace)}='{waybill.Consignor.Place}',{nameof(WaybillsTopView.corContact)}='{waybill.Consignor.Contact}',{nameof(WaybillsTopView.corPhone)}='{waybill.Consignor.Phone}' where {nameof(WaybillsTopView.corID)}='{waybill.ConsignorID}'");

//                            //修改运单信息
//                            rep.Command($"update {nameof(WaybillsTopView)} set {nameof(WaybillsTopView.wbCode)}='{waybill.Code}',{nameof(WaybillsTopView.wbCarrierID)}='{waybill.CarrierID}',{nameof(WaybillsTopView.wbModifyDate)}='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',{nameof(WaybillsTopView.wbEnterCode)}='{waybill.EnterCode}',{nameof(WaybillsTopView.wbSummary)}='{waybill.Summary}' where {nameof(WaybillsTopView.wbID)}='{waybill.WaybillID}'");

//                            //保存运单日志
//                            rep.Command($"update {nameof(Logs_WaybillsTopView)} set IsCurrent=0 where MainID='{waybill.WaybillID}'");
//                            rep.Command($"insert into {nameof(Logs_WaybillsTopView)} values('{Guid.NewGuid().ToString().MD5()}','{waybill.WaybillID}',{(int)WaybillStatusType.ExecutionStatus},{(int)waybill.ExcuteStatus},getdate(),'{admin.ID ?? ""}',1)");

//                            if (waybill.Files != null)
//                            {
//                                foreach (var item in waybill.Files)
//                                {
//                                    item.WaybillID = waybill.WaybillID;
//                                }
//                                waybill.Files.Update(rep);
//                            }
//                            //保存产品信息
//                            foreach (var product in products)
//                            {
//                                try
//                                {
//                                    var sql = $"insert {nameof(ProductsTopView)} values('{product.ID}','{product.PartNumber}','{product.Manufacturer}','{product.PackageCase}','{product.Packaging}',getdate())";
//                                    rep.Command(sql);
//                                }
//                                catch
//                                { }
//                            }

//                            //支持产品存储和描述存储
//                            //1.支持描述存储：保存带Summary（描述）的分拣和库存信息
//                            if (waybill.SummaryStorages != null)
//                            {

//                                foreach (var storage in waybill.SummaryStorages)
//                                {
//                                    //保存分拣信息
//                                    rep.Insert(new Sortings
//                                    {
//                                        ID = storage.Sorting.ID,
//                                        NoticeID = null,
//                                        BoxCode = storage.Sorting.BoxCode,
//                                        Quantity = storage.Sorting.Quantity,
//                                        AdminID = storage.Sorting.AdminID,
//                                        CreateDate = DateTime.Now,
//                                        Weight = storage.Sorting.Weight,
//                                        Volume = storage.Sorting.Volume,
//                                        WaybillID = waybill.WaybillID
//                                    });

//                                    //保存库存信息
//                                    rep.Insert(new Storages
//                                    {
//                                        ID = storage.ID,
//                                        Type = (int)StoragesType.StagingStock,
//                                        WareHouseID = storage.WareHouseID,
//                                        SortingID = storage.Sorting.ID,
//                                        InputID = "",
//                                        ProductID = storage.ProductID,
//                                        Quantity = storage.Quantity,
//                                        DateCode = "",
//                                        Supplier = "",
//                                        IsLock = false,
//                                        CreateDate = DateTime.Now,
//                                        Status = (int)Enums.StoragesStatus.Normal,
//                                        ShelveID = storage.ShelveID,
//                                        Origin = storage.Place /*((int)storage.Place).ToString()*/,
//                                        Summary = storage.Summary
//                                    });
//                                }

//                            }


//                            //2.支持产品存储：保存带产品的分拣和库存信息
//                            if (waybill.ProductStorages != null)
//                            {
//                                foreach (var storage in waybill.ProductStorages)
//                                {
//                                    //保存分拣信息
//                                    rep.Insert(new Sortings
//                                    {
//                                        ID = storage.Sorting.ID,
//                                        NoticeID = null,
//                                        BoxCode = storage.Sorting.BoxCode,
//                                        Quantity = storage.Sorting.Quantity,
//                                        AdminID = storage.Sorting.AdminID,
//                                        CreateDate = DateTime.Now,
//                                        Weight = storage.Sorting.Weight,
//                                        Volume = storage.Sorting.Volume,
//                                        WaybillID = waybill.WaybillID
//                                    });

//                                    //保存库存信息
//                                    rep.Insert(new Storages
//                                    {
//                                        ID = storage.ID,
//                                        Type = (int)StoragesType.StagingStock,
//                                        WareHouseID = storage.WareHouseID,
//                                        SortingID = storage.Sorting.ID,
//                                        InputID = "",
//                                        ProductID = storage.ProductID,
//                                        Quantity = storage.Quantity,
//                                        DateCode = storage.DateCode,
//                                        Supplier = "",
//                                        IsLock = false,
//                                        CreateDate = DateTime.Now,
//                                        Status = (int)Enums.StoragesStatus.Normal,
//                                        ShelveID = storage.ShelveID,
//                                        Origin = storage.Place/*((int)storage.Place).ToString()*/,
//                                        Summary = storage.Summary
//                                    });
//                                }
//                            }

//                            trans.Complete();
//                            if (this != null && this.Success != null)
//                            {
//                                this.Success(this, new SuccessEventArgs("Success!!"));
//                            }
//                        }
//                        catch
//                        {

//                            if (this != null && this.Failed != null)
//                            {
//                                this.Failed(this, new ErrorEventArgs("Failed!!"));
//                            }
//                        }
//                        finally
//                        {
//                            trans.Dispose();
//                        }
//                    }



//                }


//                #endregion
//            }
//        }

//        public void ChangePosition(Models.Storage[] storages, string newShelveID)
//        {
//            using (var rep = new PvWmsRepository())
//            {
//                try
//                {
//                    var shelves = new Views.ShelvesView()[newShelveID];

//                    if (shelves == null)
//                    {
//                        if (this != null && this.ShelveNotExist != null)
//                        {
//                            this.ShelveNotExist(this, new ErrorEventArgs("Shelve is not exist!!"));
//                            return;
//                        }
//                    }
                 
//                    var storageIDs = storages.Select(item => item.ID).ToArray();
//                    rep.Update(new Layers.Data.Sqls.PvWms.Storages
//                    {
//                        ShelveID = newShelveID
//                    }, item => storageIDs.Contains(item.ID));

//                    if (this != null && this.Success != null)
//                    {
//                        this.Success(this, new SuccessEventArgs("Success"));
//                        return;
//                    }
//                }
//                catch
//                {
//                    if (this != null && this.Failed != null)
//                    {
//                        this.Failed(this, new ErrorEventArgs("Failed!!"));
//                        return;
//                    }

//                }

//            }
//        }


//        public void ChangePositionByShelveID(string oldShelveID, string newShelveID)
//        {
//            //using (var rep = new PvWmsRepository())
//            //{
//            //    try
//            //    {

//            //        var oldShelves= new Views.ShelvesView()[oldShelveID];

//            //        var newShelves = new Views.ShelvesView()[newShelveID];

//            //        if (oldShelves == null|| newShelves == null)
//            //        {
//            //            if (this != null && this.ShelveNotExist != null)
//            //            {
//            //                this.ShelveNotExist(this, new ErrorEventArgs("Shelve is not exist!!"));
//            //                return;
//            //            }
//            //        }

//            //        var storages = new StoragesView().Where(item => item.ShelveID == oldShelveID);
//            //        if (storages == null)
//            //        {
//            //            if (this != null && this.StoragesIsNull != null)
//            //            {
//            //                this.StoragesIsNull(this, new ErrorEventArgs("Shelve is not goods!!"));
//            //            }
//            //        }

//            //        var storageIDs = storages.Select(item => item.ID).ToArray();
//            //        rep.Update(new Layers.Data.Sqls.PvWms.Storages
//            //        {
//            //            ShelveID = newShelveID
//            //        }, item => storageIDs.Contains(item.ID));

//            //        if (this != null && this.Success != null)
//            //        {
//            //            this.Success(this, new SuccessEventArgs("Success"));
//            //            return;
//            //        }
//            //    }
//            //    catch
//            //    {
//            //        if (this != null && this.Failed != null)
//            //        {
//            //            this.Failed(this, new ErrorEventArgs("Failed!!"));
//            //            return;
//            //        }

//            //    }

//            //}
//        }

//        ///// <summary>
//        ///// 暂存运单显示
//        ///// </summary>
//        ///// <param name="warehouseID">库房ID，必填</param>
//        ///// <param name="excuteStatus">执行状态，必填</param>
//        ///// <param name="waybillID">运单号</param>
//        ///// <param name="carrierID">承运商编号</param>
//        ///// <param name="place">发货地</param>
//        ///// <param name="shelveID">库位</param>
//        ///// <param name="createDate">创建时间</param>
//        ///// <param name="pageindex">当前页码</param>
//        ///// <param name="pagesize">每页显示记录数</param>
//        ///// <returns></returns>
//        //public object GetWaybill(string warehouseID, string excuteStatus, string waybillID = null, string carrierID = null, string place = null, string shelveID = null, string createDate = null, int pageindex = 1,
//        // int pagesize = 20)
//        //{
//        //    using (PvWmsRepository repository = new PvWmsRepository())
//        //    {
//        //        var roll = new TempStoragesWayBillRoll(repository);
//        //        if (!string.IsNullOrWhiteSpace(warehouseID))
//        //        {
//        //            roll = roll.SearchByWarehouseID(warehouseID);
//        //        }

//        //        if (!string.IsNullOrWhiteSpace(excuteStatus))
//        //        {
//        //            roll = new TempStoragesWayBillRoll(repository, roll).SearchByStatus((TempStockExcuteStatus)(int.Parse(excuteStatus)));
//        //        }
//        //        if (!string.IsNullOrEmpty(waybillID))
//        //        {
//        //            roll = new TempStoragesWayBillRoll(repository, roll).SearchByID(waybillID);
//        //        }
//        //        if (!string.IsNullOrEmpty(carrierID))
//        //        {
//        //            roll = new TempStoragesWayBillRoll(repository, roll).SearchByCarrierID(carrierID);
//        //        }
//        //        if (!string.IsNullOrEmpty(place))
//        //        {
//        //            roll = new TempStoragesWayBillRoll(repository, roll).SearchByPlace(place);
//        //        }
//        //        if (!string.IsNullOrEmpty(shelveID))
//        //        {
//        //            roll = new TempStoragesWayBillRoll(repository, roll).SearchByShelveID(shelveID);
//        //        }
//        //        DateTime Date;

//        //        if (DateTime.TryParse(createDate, out Date))
//        //        {
//        //            roll = new TempStoragesWayBillRoll(repository, roll).SearchByDate(createDate);
//        //        }


//        //        roll = new TempStoragesWayBillRoll(repository, roll.Select(item => item).GroupBy(item => item.WaybillID).Select(item => item.First()));

//        //        return roll.ToPage(pageindex, pagesize);

//        //    }


//        //}


//        ///// <summary>
//        ///// 暂存运单详情
//        ///// </summary>
//        ///// <param name="warehouseID"></param>
//        ///// <param name="waybillid"></param>
//        ///// <returns></returns>
//        //public TempStorageWaybill WayBillDetail(string warehouseID, string waybillid)
//        //{
//        //    using (PvWmsRepository rep = new PvWmsRepository())
//        //    {
//        //        var roll = new TempStoragesWayBillRoll().SearchByWarehouseID(warehouseID);
//        //        var data = new TempStoragesWayBillRoll(rep, roll).SearchByID(waybillid).ToFillArray();

//        //        return data.FirstOrDefault();
//        //    }
//        //}
//    }
//}
