using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 装箱结果
    /// </summary>
    public class Packing : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }

        public string OrderID { get; set; }

        public string AdminID { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxIndex { get; set; }

        /// <summary>
        /// 日期(箱号上的日期)
        /// </summary>
        public DateTime PackingDate { get; set; }

        /// <summary>
        /// 整箱重量
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 包装类型
        /// </summary>
        public string WrapType { get; set; }

        /// <summary>
        /// 状态：未封箱、已封箱
        /// </summary>
        public Enums.PackingStatus PackingStatus { get; set; }

        public string PackingStatusDes
        {
            get
            {
                return this.PackingStatus.GetDescription();
            }
        }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 分拣明细
        /// </summary>
        PackingItems items;
        public PackingItems Items
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.PackingItemsView())
                    {
                        var query = view.Where(item => item.PackingID == this.ID);
                        this.Items = new PackingItems(query);
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

                this.items = new PackingItems(value, new Action<PackingItem>(delegate (PackingItem item)
                {
                    item.PackingID = this.ID;
                }));
            }
        }

        public Packing()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
            this.PackingStatus = Enums.PackingStatus.UnSealed;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Packing);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 去持久化
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Status = Enums.Status.Delete
                        }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        /// <summary>
        /// 删除装箱以及其相关数据
        /// </summary>
        public void Delete(Needs.Ccs.Services.Models.Admin deleteOperationAdmin, string deleteEntryNoticeID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //查询装箱结果项
                var packingItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>()
                    .Where(item => item.PackingID == this.ID);
                foreach (var packingItem in packingItems)
                {
                    //删除装箱结果项
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PackingItems>(
                        new
                        {
                            Status = Enums.Status.Delete
                        }, item => item.ID == packingItem.ID);
                    //删除分拣结果
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Status = Enums.Status.Delete
                        }, item => item.ID == packingItem.SortingID);
                    //删除库存结果
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.StoreStorages>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Status = Enums.Status.Delete
                        }, item => item.SortingID == packingItem.SortingID);
                    //删除OrderWaybillItem
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>(t => t.SortingID == packingItem.SortingID);
                }
                //删除装箱结果
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Status = Enums.Status.Delete
                        }, item => item.ID == this.ID);


                //检查OrderWaybills是否还有项目
                // 如果没有项，则说明上面把OrderWaybillItems删光了，那OrderWaybill也删除
                var orderWayBillInfos = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybills>().Where(t => t.OrderID == this.OrderID).ToList();
                foreach(var item in orderWayBillInfos)
                {
                    int itemCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>().Where(t => t.OrderWaybillID == item.ID).Count();
                    if (itemCount == 0)
                    {
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderWaybills>(t => t.ID == item.ID);
                    }                        
                }

                //插入取消装箱日志
                string Summry = "库房管理员[" + deleteOperationAdmin.RealName + "]取消了箱号[" + this.BoxIndex + "]的装箱。";
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.EntryNoticeLogs
                {
                    ID = ChainsGuid.NewGuidUp(),
                    EntryNoticeID = deleteEntryNoticeID,
                    AdminID = deleteOperationAdmin.ID,
                    CreateDate = DateTime.Now,
                    Summary = Summry,
                });

            }
        }

        /// <summary>
        /// 取消封箱
        /// </summary>
        public void CancelSealed()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //查询对应的报关通知,并取消报关通知
                var packingItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>()
                    .Where(item => item.PackingID == this.ID && item.Status == (int)Enums.Status.Normal).FirstOrDefault();
                var declarNoticeItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>()
                    .Where(item => item.SortingID == packingItem.SortingID && item.Status != (int)Enums.DeclareNoticeItemStatus.Cancel).FirstOrDefault();
                var declarNoitce = new Views.DeclarationNoticesView(reponsitory)
                    .Where(item => item.ID == declarNoticeItem.DeclarationNoticeID).FirstOrDefault();

                bool flag = declarNoitce.CancelDeclaration();
                if (flag == false)
                {
                    throw new Exception("已报关成功，不能取消封箱！");
                }
                else
                {
                    var sortingPackingView = new Views.SortingPackingsView(reponsitory);
                    foreach (var Item in declarNoitce.Items)
                    {
                        var sortingPakcing = sortingPackingView.Where(t => t.ID == Item.Sorting.ID).FirstOrDefault();
                        //变更所有已报关Sorting的状态
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            DecStatus = Enums.SortingDecStatus.No,
                        }, t => t.ID == Item.Sorting.ID);
                        //变更所有已报关Packing的状态
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            PackingStatus = Enums.PackingStatus.UnSealed
                        }, t => t.ID == sortingPakcing.Packing.ID);
                    }

                    //变更入库通知状态
                    //var entryNotice = new Views.HKEntryNoticeView(reponsitory)
                    //    .Where(t => t.Order.ID == this.OrderID).FirstOrDefault();
                    var entryNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>().
                                      Where(t => t.OrderID == this.OrderID && 
                                                 t.WarehouseType == (int)Enums.WarehouseType.HongKong && 
                                                 t.Status == (int)Enums.Status.Normal).FirstOrDefault();

                    if (entryNotice.EntryNoticeStatus == (int)Enums.EntryNoticeStatus.Sealed)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            EntryNoticeStatus = Enums.EntryNoticeStatus.Boxed,
                        }, t => t.ID == entryNotice.ID);
                    }
                }
            }
        }

        /// <summary>
        /// 更改箱号
        /// </summary>
        public void ChangeBoxIndex()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update(new Layer.Data.Sqls.ScCustoms.Packings
                {
                    BoxIndex = this.BoxIndex,
                    PackingDate = this.PackingDate
                }, item => item.ID == this.ID);

                foreach (var item in this.Items)
                {
                    item.Sorting.ChangeBoxIndex(this.BoxIndex);
                }
            }
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}