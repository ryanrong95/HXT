using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 香港库房的出库通知
    /// </summary>
    [Serializable]
    public class HKExitNotice : ExitNotice
    {
        public override DecHead DecHead
        {
            get
            {
                return base.DecHead;
            }

            set
            {
                base.DecHead = value;
            }
        }

        public OutputWayBill OutputWayBill { get; set; }

        /// <summary>
        /// 香港出库通知明细项
        /// </summary>
        HKExitNoticeItems items;
        public HKExitNoticeItems HKItems
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.HKExitNoticeItemView())
                    {
                        var query = view.Where(item => item.ExitNoticeID == this.ID);
                        this.HKItems = new HKExitNoticeItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.items = new HKExitNoticeItems(value, new Action<HKExitNoticeItem>(delegate (HKExitNoticeItem item)
                {
                    item.ExitNoticeID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 当订单香港出库时发生
        /// </summary>
        public event WarehouseExitedEventHanlder HKExited;

        public HKExitNotice() : base()
        {
            base.WarehouseType = WarehouseType.HongKong;
            base.ExitType = ExitType.Delivery;

            this.HKExited += Warehouse_Exited;
        }

        /// <summary>
        /// 香港出库
        /// </summary>
        public void OutStock()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新NoticeItems表状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        ExitNoticeStatus = ExitNoticeStatus.Exited
                    }, item => item.ExitNoticeID == this.ID);
                //更新ExitNotice主表的状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        ExitNoticeStatus = ExitNoticeStatus.Exited
                    }, item => item.ID == this.ID);

                //生成深圳仓库的入库通知
                SZEntryNotice entryNotice = new SZEntryNotice();
                entryNotice.ID = "SZ" + Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice);
                entryNotice.Order = this.Order;
                entryNotice.DecHead = this.DecHead;
                entryNotice.ClientCode = this.Order.Client.ClientCode;
                entryNotice.EntryNoticeStatus = EntryNoticeStatus.UnBoxed;
                reponsitory.Insert(entryNotice.ToLinq());

                //生成深圳入库通知项
                var EntryNoticeItems = this.HKItems.Select(item => new
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem),
                    ItemID = item.ID,
                    EntryNoticeID = entryNotice.ID,
                    DecListID = item.DecList?.ID,
                    IsSpotCheck = false,
                }).ToArray();
                //插入数据库
                reponsitory.Insert(EntryNoticeItems.Select(item => new Layer.Data.Sqls.ScCustoms.EntryNoticeItems()
                {
                    ID = item.ID,
                    EntryNoticeID = item.EntryNoticeID,
                    DecListID = item.DecListID,
                    IsSpotCheck = item.IsSpotCheck,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Status = (int)Status.Normal,
                    EntryNoticeStatus = (int)EntryNoticeStatus.UnBoxed,
                }).ToArray());

                //生成深圳分拣结果
                Layer.Data.Sqls.ScCustoms.Sortings[] SZSortings = this.HKItems.Select(item => new Layer.Data.Sqls.ScCustoms.Sortings()
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.Sorting),
                    OrderID = this.Order.ID,
                    OrderItemID = item.DecList.DeclarationNoticeItem.Sorting.OrderItem.ID,
                    EntryNoticeItemID = EntryNoticeItems.SingleOrDefault(EntryItem => EntryItem.ItemID == item.ID).ID,
                    //ProductID = item.DecList.DeclarationNoticeItem.Sorting.Product.ID,
                    Quantity = item.DecList.GQty,
                    BoxIndex = item.DecList.CaseNo,
                    NetWeight = item.DecList.NetWt == null ? 0M : (decimal)item.DecList.NetWt,
                    GrossWeight = item.DecList.GrossWt == null ? 0M : (decimal)item.DecList.GrossWt,
                    DecStatus = (int)SortingDecStatus.Yes,
                    AdminID = this.Admin.ID,
                    WrapType = this.DecHead.WrapType,
                    WarehouseType = (int)WarehouseType.ShenZhen,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Status = (int)Status.Normal,
                }).ToArray();
                reponsitory.Insert(SZSortings);

                var storages = new HKStoreStorageView(reponsitory);
                var linq = (from storage in storages.AsEnumerable()
                            join item in this.HKItems on storage.Sorting.ID equals item.DecList.DeclarationNoticeItem.Sorting.ID
                            select new
                            {
                                Quantity = storage.Quantity - item.Quantity,
                                storage.ID,
                            }).ToArray();
                //删除库存数量为0的库存记录
                var deleteids = linq.Where(item => item.Quantity == 0M).Select(item => item.ID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.StoreStorages>(new
                {
                    Status = Enums.Status.Delete
                }, t => deleteids.Contains(t.ID));
                //更新库存数量不为0的库存记录
                var updateStorages = linq.Where(item => item.Quantity != 0M);
                foreach (var stogage in updateStorages)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.StoreStorages>(new
                    {
                        Quantity = stogage.Quantity,
                        UpdateDate = DateTime.Now,
                    }, t => t.ID == stogage.ID);
                }
            }

            this.OnHKExited();
        }

        virtual public void OnHKExited()
        {
            if (this != null && this.HKExited != null)
            {
                this.HKExited(this, new WarehouseExitedEventArgs(this));
            }
        }

        private void Warehouse_Exited(object sender, WarehouseExitedEventArgs e)
        {
            var order = e.ExitNotice.Order;
            var admin = e.ExitNotice.Admin;
            order.Trace(admin, OrderTraceStep.InTransit, "您的订单由【香港库房】送往【深圳库房】");
        }
    }
}