using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 库房分拣
    /// </summary>
    public class HKSortingContext
    {
        private Order Order { get; set; }

        private EntryNotice EntryNotice { get; set; }

        /// <summary>
        /// 箱子
        /// </summary>
        public PackingModel Packing { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        private string WaybillCode { get; set; }

        public string Stock { get; set; }

        public string BoxIndex { get; set; }

        /// <summary>
        /// 分拣项
        /// </summary>
        public IEnumerable<SortingModel> Items;


        public Admin Admin { get; set; }

        public string EntryNoticeID { get; set; }

        /// <summary>
        /// 上架
        /// </summary>
        /// <param name="stock">库位号</param>
        /// <param name="boxIndex">箱号</param>
        public void ToShelve(string stock, string boxIndex)
        {
            this.Stock = stock;
            this.BoxIndex = boxIndex;
        }

        /// <summary>
        /// 装箱
        /// </summary>
        /// <param name="packing">Packing</param>
        public void SetPacking(PackingModel packing)
        {
            this.Packing = packing;
        }

        /// <summary>
        /// 运单
        /// </summary>
        /// <param name="waybillCode">waybill</param>
        public void SetWaybill(string waybillCode)
        {
            this.WaybillCode = waybillCode;
        }

        /// <summary>
        /// 装箱
        /// </summary>
        public void Pack()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //判断是否第一次装箱，如果是，发短信，给客户端发消息
                int sCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>().Where(t => t.OrderID == this.Packing.OrderID).Count();
                if (sCount == 0)
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        PushMsg pushMsg = new PushMsg((int)SpotName.ArriveHK, this.Packing.OrderID);
                        pushMsg.push();

                        var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
                        var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.UpdateOrderStatusToBoxed;

                        string[] mainOrderIDs = this.Packing.OrderID.Split('-');
                        var msg = new
                        {
                            MainID = mainOrderIDs[0],
                            AdminID = this.Packing.CenterAdminID
                        };

                        var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, msg);

                    });
                }

                //新增装箱结果Packing
                this.Packing.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Packing);
                reponsitory.Insert(this.Packing.ToLinq());

                HKStoreStorage storage = new HKStoreStorage();

                foreach (var model in Items)
                {
                    HKSorting sorting = new HKSorting();
                    //新增香港分拣结果
                    sorting.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Sorting);
                    sorting.BoxIndex = this.Packing.BoxIndex;
                    sorting.OrderItem = new OrderItem() { ID = model.OrderItemID };
                    sorting.EntryNoticeItemID = model.EntryNoticeItemID;
                    sorting.AdminID = this.Packing.AdminID;
                    sorting.OrderID = this.Packing.OrderID;
                    sorting.WrapType = this.Packing.WrapType;
                    //sorting.Product = new Product() { ID = model.ProductID };
                    sorting.Quantity = model.Quantity;
                    sorting.DecStatus = SortingDecStatus.No;
                    var gwet = (model.Quantity / this.Packing.Quantity * this.Packing.Weight).ToRound(2);
                    sorting.GrossWeight = gwet < ConstConfig.MinPackingGrossWeight ? ConstConfig.MinPackingGrossWeight : gwet;
                    sorting.NetWeight = (gwet * 0.7M).ToRound(2) < ConstConfig.MinPackingNetWeight ? ConstConfig.MinPackingNetWeight : (gwet * 0.7M).ToRound(2);
                    reponsitory.Insert(sorting.ToLinq());

                    ////上架
                    ////TODO:等陈瀚的框架完成后，可以直接使用storage.InStore();
                    //storage.ID = Needs.Overall.PKeySigner.Pick(PKeyType.StoreStorage);
                    //storage.OrderItemID = model.OrderItemID;
                    //storage.Sorting = sorting;
                    ////storage.Product = sorting.Product;
                    //storage.Quantity = sorting.Quantity;
                    //storage.BoxIndex = this.Packing.BoxIndex;
                    //storage.StockCode = this.Stock;
                    //storage.Purpose = StockPurpose.Declared;
                    //reponsitory.Insert(storage.ToLinq());

                    //添加装箱项明细
                    var packingItem = new PackingItem();
                    packingItem.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PackingItem);
                    packingItem.Sorting = sorting;
                    packingItem.PackingID = this.Packing.ID;
                    reponsitory.Insert(packingItem.ToLinq());

                    //新增国际快递项明细
                    if (string.IsNullOrEmpty(this.WaybillCode) == false)
                    {
                        OrderWaybillItem waybillItem = new OrderWaybillItem();

                        waybillItem.OrderWaybillID = this.WaybillCode;
                        waybillItem.Sorting = sorting;
                        reponsitory.Insert(waybillItem.ToLinq());
                        //waybillItem.Enter();
                    }

                }
                string Summry = "库房管理员[" + this.Admin.RealName + "]完成了箱号[" + this.Packing.BoxIndex + "]的装箱，重量：" + this.Packing.Weight + "KG。";

                //更新EntryNotice的UpDate 时间为封箱时间
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(new { UpdateDate = DateTime.Now }, t => t.OrderID == this.Packing.OrderID);

                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.EntryNoticeLogs
                {
                    ID = ChainsGuid.NewGuidUp(),
                    EntryNoticeID = this.EntryNoticeID,
                    AdminID = this.Admin.ID,
                    CreateDate = DateTime.Now,
                    Summary = Summry,
                });
            }
        }

    }

    /// <summary>
    /// 扩展分拣结果
    /// </summary>
    public class SortingModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string EntryNoticeItemID { get; set; }

        /// <summary>
        /// 进项
        /// </summary>
        public string OrderItemID { get; set; }

        //public string ProductID { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }
    }

    public class PackingModel : Packing
    {
        /// <summary>
        /// 装箱数量
        /// </summary>
        public decimal Quantity { get; set; }
        public string CenterAdminID { get; set; }
    }

    /// <summary>
    /// 扩展分拣结果，内单用
    /// </summary>
    public class InsideSortingModel : SortingModel
    {
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
    }
}