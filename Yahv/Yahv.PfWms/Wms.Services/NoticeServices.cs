using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq.Extends;
using Yahv.Services.Models;
using Yahv.Services.Enums;
using Wms.Services.Models;
using Layers.Data;
using Yahv.Underly.Erps;
using System.Transactions;
using Yahv.Utils.Serializers;
using Yahv.Usually;
using Yahv.Underly;

namespace Wms.Services
{
    public class NoticeServices
    {

        public event SuccessHanlder Success;
        public event ErrorHanlder Failed;


        public NoticeServices()
        {

        }

        IErpAdmin admin;
        public NoticeServices(IErpAdmin admin)
        {
            this.admin = admin;
        }

        /// <summary>
        /// 获得通知
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetNotices(string warehouseID, string key, int pageIndex = 1, int pageSize = 10)
        {
            using (var rep = new PvWmsRepository())
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return null;
                }
                else
                {
                    List<CenterProduct> product = new List<CenterProduct>();

                    //通过箱号和当前库房获得入库通知
                    var box = new Views.BoxesView()[key];
                    if (box != null)
                    {
                        var noticesByBoxCode = new Views.PDANoticeRoll().Where(item => item.BoxCode == box.Code && item.WareHouseID == warehouseID && item.Type==CgNoticeType.Enter && item.Status != NoticesStatus.Closed);

                        if (noticesByBoxCode.Where(item => item.Status == NoticesStatus.Waiting).Count() <= 0)
                        {
                            return null;
                        }

                        //var waybillIDs = noticesByBoxCode.Select(item => item.WaybillID).ToArray();


                        if (noticesByBoxCode.Count() > 0)
                        {
                            return noticesByBoxCode.Paging(pageIndex, pageSize);
                        }
                    }

                    //通过运输批次号和当前库房获得入库通知
                    var wayChcd = rep.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Where(item => item.chcdLotNumber == key);

                    if (wayChcd.Count() > 0)
                    {
                        var waybillID = wayChcd.FirstOrDefault().chcdID;
                        var noticesByLotNumber = new Views.PDANoticeRoll().Where(item => item.WaybillID == waybillID && item.WareHouseID == warehouseID && item.Type == CgNoticeType.Enter && item.Status != NoticesStatus.Closed);

                        if (noticesByLotNumber.Where(item => item.Status == NoticesStatus.Waiting).Count() <= 0)
                        {
                            return null;
                        }
                        if (noticesByLotNumber.Count() > 0)
                        {
                            return noticesByLotNumber.Paging(pageIndex, pageSize);
                        }
                        else return null;
                    }
                }
            }

            return null;
        }

        public void Enter(string warehouseID,string shelveID,string key)
        {
            string[] waybillIDs = null;
            PDANotices[] notices = null;

            #region 第一步：取数据
            using (var rep = new PvWmsRepository())
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    if (this != null && this.Failed != null)
                    {
                        this.Failed(this, new ErrorEventArgs("Failed!!"));
                    }
                }
                else
                {
                    List<CenterProduct> product = new List<CenterProduct>();

                    //通过箱号和当前库房获得入库通知
                    var box = new Views.BoxesView()[key];

                    //通过运输批次号和当前库房获得入库通知
                    var wayChcd = rep.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Where(item => item.chcdLotNumber == key);
                    if (box != null)
                    {
                        notices = new Views.PDANoticeRoll().Where(item => item.BoxCode == box.Code && item.WareHouseID == warehouseID && item.Type ==CgNoticeType.Enter && item.Status != NoticesStatus.Closed).ToArray();
                    }
                    else
                    {
                        if (wayChcd.Count() > 0)
                        {
                            var waybillID = wayChcd.FirstOrDefault().chcdID;
                            notices = new Views.PDANoticeRoll().Where(item => item.WaybillID == waybillID && item.WareHouseID == warehouseID && item.Type == CgNoticeType.Enter && item.Status != NoticesStatus.Closed).ToArray();
                        }
                    }
                }
            }
            #endregion

            #region 第二步：补充PDANotices数据
            if (notices != null)
            {

                waybillIDs = notices.Select(item => item.WaybillID).Distinct().ToArray();
                foreach (var notice in notices)
                {
                    //notice.ID = PKeySigner.Pick(PkeyType.Notices);

                    var sortingID = PKeySigner.Pick(PkeyType.Sortings);
                    notice.Sorting = new Sorting
                    {
                        ID = sortingID,
                        NoticeID = notice.ID,
                        BoxCode = null,
                        Quantity = notice.Quantity,
                        AdminID = admin.ID,
                        CreateDate = DateTime.Now,
                        Weight = notice.Weight,
                        Volume = notice.Volume,
                    };

                    var storageID = PKeySigner.Pick(PkeyType.Storages);
                    notice.Storage = new Wms.Services.Models.Storage
                    {
                        ID = storageID,
                        Type = Yahv.Underly.StoragesType.Inventory,
                        WareHouseID = warehouseID,
                        SortingID = sortingID,
                        InputID = notice.InputID,
                        ProductID = notice.Product.ID,
                        Quantity = notice.Quantity,
                        NoticeID = notice.ID,
                        Supplier = notice.Supplier,
                        DateCode = notice.DateCode,
                        IsLock = false,
                        CreateDate = DateTime.Now,
                        Status = Enums.StoragesStatus.Normal,
                        ShelveID = shelveID,
                        Forms = new Form[]
                              {
                                    new Form{
                                        ID =PKeySigner.Pick(PkeyType.StoragesForm, null)
                                        ,NoticeID=notice.ID,
                                        StorageID=storageID,
                                        Quantity=notice.Quantity,
                                        Status=FormStatus.Facted
                                    }
                              }
                    };

                }
            }
            #endregion

            #region 第三步：给对应的Waybill，Notice，Sorting，Input，Forms存数据
            using (var rep = new PvWmsRepository())
            {
                using (var trans = new TransactionScope())
                {
                    try
                    {
                        if (notices != null)
                        {
                            foreach (var waybillID in waybillIDs)
                            {
                                //更新运单信息
                                rep.Command($"update {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView)} set {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView.wbExcuteStatus)}={(int)SortingExcuteStatus.Stocked} where {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView.wbID)}='{waybillID}'");
                            }

                            foreach (var notice in notices)
                            {

                                rep.Update(new Layers.Data.Sqls.PvWms.Notices
                                {
                                    Status = (int)NoticesStatus.Completed,
                                }, item => item.ID == notice.ID);

                                //保存分拣信息
                                rep.Insert(new Layers.Data.Sqls.PvWms.Sortings
                                {
                                    ID = notice.Sorting.ID,
                                    NoticeID = notice.ID,
                                    BoxCode = notice.Sorting.BoxCode,//boxCode目前 取值null
                                    Quantity = notice.Sorting.Quantity,
                                    AdminID = "",
                                    CreateDate = DateTime.Now,
                                    Weight = notice.Sorting.Weight,
                                    Volume = notice.Sorting.Volume,
                                    WaybillID = notice.WaybillID
                                });

                                //保存库存信息
                                rep.Insert(new Layers.Data.Sqls.PvWms.Storages
                                {
                                    ID = notice.Storage.ID,
                                    Type = (int)notice.Storage.Type,
                                    WareHouseID = notice.Storage.WareHouseID,
                                    SortingID = notice.Storage.SortingID,
                                    InputID = notice.Storage.InputID,
                                    ProductID = notice.Storage.ProductID,
                                    Quantity = notice.Storage.Quantity,
                                    DateCode = notice.DateCode,
                                    Supplier = notice.Supplier,
                                    IsLock = false,
                                    CreateDate = DateTime.Now,
                                    Status = (int)Enums.StoragesStatus.Normal,
                                    ShelveID = notice.Storage.ShelveID,
                                    Origin = notice.Input == null ? null : notice.Input.OriginID
                                });

                                if (notice.Storage.Forms != null)
                                {
                                    foreach (var form in notice.Storage.Forms)
                                    {
                                        rep.Insert(new Layers.Data.Sqls.PvWms.Forms
                                        {
                                            ID = form.ID,
                                            NoticeID = form.ID,
                                            StorageID = form.StorageID,
                                            Quantity = form.Quantity,
                                            Status = (int)FormStatus.Facted

                                        });
                                    }
                                }

                            }

                            trans.Complete();
                            if (this != null && this.Success != null)
                            {
                                this.Success(this, new SuccessEventArgs("Success"));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this != null && this.Failed != null)
                        {
                            this.Failed(this, new ErrorEventArgs("Failed!!"));
                        }
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }
            #endregion

        }

        public void Enter(string warehouseID, string shelveID, PDANotices[] notices)
        {
            string[] waybillIDs = null;
            if (notices != null)
            {

                waybillIDs = notices.Select(item => item.WaybillID).Distinct().ToArray();
                foreach (var notice in notices)
                {
                    //notice.ID = PKeySigner.Pick(PkeyType.Notices);

                    var sortingID = PKeySigner.Pick(PkeyType.Sortings);
                    notice.Sorting = new Sorting
                    {
                        ID = sortingID,
                        NoticeID = notice.ID,
                        BoxCode = null,
                        Quantity = notice.Quantity,
                        AdminID = admin.ID,
                        CreateDate = DateTime.Now,
                        Weight = notice.Weight,
                        Volume = notice.Volume,
                    };

                    var storageID = PKeySigner.Pick(PkeyType.Storages);
                    notice.Storage = new Wms.Services.Models.Storage
                    {
                        ID = storageID,
                        Type = Yahv.Underly.StoragesType.Inventory,
                        WareHouseID = warehouseID,
                        SortingID = sortingID,
                        InputID = notice.InputID,
                        ProductID = notice.Product.ID,
                        Quantity = notice.Quantity,
                        NoticeID = notice.ID,
                        Supplier = notice.Supplier,
                        DateCode = notice.DateCode,
                        IsLock = false,
                        CreateDate = DateTime.Now,
                        Status = Enums.StoragesStatus.Normal,
                        ShelveID = shelveID,
                        Forms = new Form[]
                              {
                                    new Form{
                                        ID =PKeySigner.Pick(PkeyType.StoragesForm, null)
                                        ,NoticeID=notice.ID,
                                        StorageID=storageID,
                                        Quantity=notice.Quantity,
                                        Status=FormStatus.Facted
                                    }
                              }
                    };

                }
            }
            using (var rep = new PvWmsRepository())
            {
                using (var trans = new TransactionScope())
                {
                    try
                    {


                        if (notices != null)
                        {
                            foreach (var waybillID in waybillIDs)
                            {
                                //更新运单信息
                                rep.Command($"update {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView)} set {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView.wbExcuteStatus)}={(int)SortingExcuteStatus.Stocked} where {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView.wbID)}='{waybillID}'");
                            }

                            foreach (var notice in notices)
                            {
                                //rep.Insert(new Layers.Data.Sqls.PvWms.Notices
                                //{
                                //    ID = notice.ID,
                                //    Type = (int)NoticeType.TransferEnter,//???
                                //    WareHouseID = notice.WareHouseID,
                                //    WaybillID = notice.WaybillID,
                                //    InputID = notice.InputID,
                                //    OutputID = null,
                                //    ProductID = notice.ProductID,
                                //    Supplier = notice.Supplier,
                                //    DateCode = notice.DateCode,
                                //    Quantity = notice.Quantity,
                                //    Conditions = notice.Conditions.Json(),
                                //    CreateDate = DateTime.Now,
                                //    Status = (int)NoticesStatus.Completed,
                                //    Source = (int)NoticeSource.AgentEnter,
                                //    Target = (int)notice.Target,
                                //    BoxCode = null,
                                //    Weight = notice.Weight,
                                //    Volume = notice.Volume,
                                //    ShelveID = shelveID,
                                //});

                                rep.Update(new Layers.Data.Sqls.PvWms.Notices
                                {
                                    Status = (int)NoticesStatus.Completed,
                                }, item => item.ID == notice.ID);

                                //保存分拣信息
                                rep.Insert(new Layers.Data.Sqls.PvWms.Sortings
                                {
                                    ID = notice.Sorting.ID,
                                    NoticeID = notice.ID,
                                    BoxCode = notice.Sorting.BoxCode,//boxCode目前 取值null
                                    Quantity = notice.Sorting.Quantity,
                                    AdminID = "",
                                    CreateDate = DateTime.Now,
                                    Weight = notice.Sorting.Weight,
                                    Volume = notice.Sorting.Volume,
                                    WaybillID = notice.WaybillID
                                });

                                //保存库存信息
                                rep.Insert(new Layers.Data.Sqls.PvWms.Storages
                                {
                                    ID = notice.Storage.ID,
                                    Type = (int)notice.Storage.Type,
                                    WareHouseID = notice.Storage.WareHouseID,
                                    SortingID = notice.Storage.SortingID,
                                    InputID = notice.Storage.InputID,
                                    ProductID = notice.Storage.ProductID,
                                    Quantity = notice.Storage.Quantity,
                                    DateCode = notice.DateCode,
                                    Supplier = notice.Supplier,
                                    IsLock = false,
                                    CreateDate = DateTime.Now,
                                    Status = (int)Enums.StoragesStatus.Normal,
                                    ShelveID = notice.Storage.ShelveID,
                                    Origin = notice.Input == null ? null : notice.Input.OriginID
                                });

                                if (notice.Storage.Forms != null)
                                {
                                    foreach (var form in notice.Storage.Forms)
                                    {
                                        rep.Insert(new Layers.Data.Sqls.PvWms.Forms
                                        {
                                            ID = form.ID,
                                            NoticeID = form.ID,
                                            StorageID = form.StorageID,
                                            Quantity = form.Quantity,
                                            Status = (int)FormStatus.Facted

                                        });
                                    }
                                }

                            }

                            trans.Complete();
                            if (this != null && this.Success != null)
                            {
                                this.Success(this, new SuccessEventArgs("Success"));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this != null && this.Failed != null)
                        {
                            this.Failed(this, new ErrorEventArgs("Failed!!"));
                        }
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }
        }

    }

}
