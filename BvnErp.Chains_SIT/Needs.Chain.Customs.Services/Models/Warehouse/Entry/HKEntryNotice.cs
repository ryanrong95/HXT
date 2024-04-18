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
    /// 香港库房的入库通知
    /// </summary>
    public class HKEntryNotice : EntryNotice
    {
        /// <summary>
        /// 分拣要求
        /// </summary>
        public override SortingRequire SortingRequire
        {
            get
            {
                return base.SortingRequire;
            }
            set
            {
                base.SortingRequire = value;
            }
        }

        public IEnumerable<HKEntryNoticeItem> HKItems { get; set; }

        /// <summary>
        /// 香港库房封箱事件
        /// </summary>
        public event SealedHanlder Sealed;


        public HKEntryNotice() : base()
        {
            base.WarehouseType = WarehouseType.HongKong;
            this.Sealed += EntryNotice_Sealed;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Abandon()
        {
            base.Abandon();
        }

        /// <summary>
        /// 更新状态（未装箱、已装箱）
        /// </summary>
        public void UpdateItemsStatus()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新明细项的状态
                foreach (var item in HKItems)
                {
                    if (item.RelQuantity == 0M)
                    {
                        if (item.EntryNoticeStatus == EntryNoticeStatus.UnBoxed)
                        {
                            item.EntryNoticeStatus = EntryNoticeStatus.Boxed;
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(
                                new { EntryNoticeStatus = Enums.EntryNoticeStatus.Boxed }, t => t.ID == item.ID);
                        }
                    }
                    else
                    {
                        if (item.EntryNoticeStatus == Enums.EntryNoticeStatus.Boxed)
                        {
                            item.EntryNoticeStatus = EntryNoticeStatus.UnBoxed;
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(
                                new { EntryNoticeStatus = Enums.EntryNoticeStatus.UnBoxed }, t => t.ID == item.ID);
                        }
                    }
                }
                //更新自己的状态
                int count = HKItems.Count(item => item.EntryNoticeStatus == Enums.EntryNoticeStatus.UnBoxed);
                if (count == 0)
                {
                    if (this.EntryNoticeStatus == Enums.EntryNoticeStatus.UnBoxed)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(
                                new { EntryNoticeStatus = Enums.EntryNoticeStatus.Boxed }, item => item.ID == this.ID);
                    }
                }
                else
                {
                    if (this.EntryNoticeStatus == Enums.EntryNoticeStatus.Boxed)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(
                                new { EntryNoticeStatus = Enums.EntryNoticeStatus.UnBoxed }, item => item.ID == this.ID);
                    }
                }
            }
        }

        /// <summary>
        /// 分拣异常
        /// </summary>
        public void AbnormalSorting()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //TODO:分拣异常 入库通知自身的处理方式是什么？待确认
                //需要需求确认，分拣异常后订单是否要重新生成入通知？

                this.Log(this.Operator, "库房管理员[" + this.Operator.RealName + "]做了分拣异常。");
                this.Order.HangUp(Enums.OrderControlType.SortingAbnomaly, this.Summary);
            }
        }

        /// <summary>
        /// 封箱
        /// </summary>
        /// <param name="SourceOperation">正常默认是香港库房封箱，如果传过来是SplitOrder，跟单拆分，则记的Log有所差异</param>
        public void Seal(string SourceOperation = "HK")
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新入库通知状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        EntryNoticeStatus = EntryNoticeStatus.Sealed
                    }, item => item.ID == this.ID);
            }

            this.OnSealed(SourceOperation);
        }

        /// <summary>
        /// 封箱
        /// </summary>
        public virtual void OnSealed(string SourceOperation)
        {
            if (this != null && this.Sealed != null)
            {
                this.Sealed(this, new SealedEventArgs(this.Order.ID, SourceOperation));
            }
        }

        /// <summary>
        /// 封箱并生成报关通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryNotice_Sealed(object sender, SealedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var sortings = new SortingsView(reponsitory).Where(item => item.OrderID == e.OrderID && item.DecStatus == SortingDecStatus.No);

                GenerateCreator generateCreator = new GenerateCreator();
                string creatorID = generateCreator.getCreator();
                //生成报关通知
                var entity = new Layer.Data.Sqls.ScCustoms.DeclarationNotices();
                entity.ID = Needs.Overall.PKeySigner.Pick(PKeyType.DeclareNotice);
                entity.OrderID = e.OrderID;
                entity.AdminID = this.Order.AdminID;
                entity.Status = (int)Enums.DeclareNoticeStatus.UnDec;
                entity.CreateDate = DateTime.Now;
                entity.UpdateDate = DateTime.Now;
                entity.CreateDeclareAdminID = creatorID;
                reponsitory.Insert(entity);

                //生成报关通知项
                foreach (var item in sortings.ToList())
                {
                    DeclarationNoticeItem entityItem = new DeclarationNoticeItem();
                    entityItem.ID = ChainsGuid.NewGuidUp();
                    entityItem.DeclarationNoticeID = entity.ID;
                    entityItem.Sorting = item;
                    reponsitory.Insert(entityItem.ToLinq());
                }

                //更新装箱结果状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(new { PackingStatus = PackingStatus.Sealed }, t => t.OrderID == e.OrderID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new { DecStatus = SortingDecStatus.Yes }, t => t.OrderID == e.OrderID);

                reponsitory.Submit();
            }

            orderLog(e.SourceOperation);
        }

        private void orderLog(string SourceOperation)
        {
            if (SourceOperation.ToLower() == "splitorder")
            {
                this.Order.Log(this.Operator, "跟单员[" + this.Operator.RealName + "]拆分报关，系统自动封箱。");
                this.Order.Trace(this.Operator, OrderTraceStep.HKProcessing, "拆分报关，系统自动封箱");
                this.Log(this.Operator, "跟单员[" + this.Operator.RealName + "]拆分报关，系统自动封箱。");
            }
            else
            {
                this.Order.Log(this.Operator, "库房管理员[" + this.Operator.RealName + "]分拣完成，等待报关。");
                this.Order.Trace(this.Operator, OrderTraceStep.HKProcessing, "香港库房已分拣封箱完成");
                this.Log(this.Operator, "库房管理员[" + this.Operator.RealName + "]完成了封箱。");
            }

          
        }

    }
}