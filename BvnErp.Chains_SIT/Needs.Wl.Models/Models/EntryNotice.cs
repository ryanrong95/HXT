using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 入库通知
    /// </summary>
    public class EntryNotice : ModelBase<Layer.Data.Sqls.ScCustoms.EntryNotices, ScCustomsReponsitory>, IUnique, IPersist, IFulError, IFulSuccess
    {
        /// <summary>
        /// 客户订单
        /// </summary>
        public string OrderID { get; set; }

        public string DecHeadID { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 分拣要求
        /// </summary>
        public virtual Needs.Wl.Models.Enums.SortingRequire SortingRequire { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        public Needs.Wl.Models.Enums.WarehouseType WarehouseType { get; set; }

        public Needs.Wl.Models.Enums.EntryNoticeStatus EntryNoticeStatus { get; set; }

        public Needs.Wl.Models.Admin Admin { get; set; }


        public EntryNotice()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = (int)Needs.Wl.Models.Enums.Status.Normal;
            this.EntryNoticeStatus = Needs.Wl.Models.Enums.EntryNoticeStatus.UnBoxed;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public override void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>().Count(item => item.ID == this.ID);

                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice);
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.EntryNotices
                        {
                            ID = this.ID,
                            OrderID = this.OrderID,
                            DecHeadID = this.DecHeadID,
                            WarehouseType = (int)this.WarehouseType,
                            ClientCode = this.ClientCode,
                            SortingRequire = (int)this.SortingRequire,
                            EntryNoticeStatus = (int)this.EntryNoticeStatus,
                            Status = (int)this.Status,
                            CreateDate = DateTime.Now,
                            UpdateDate = this.UpdateDate,
                            Summary = this.Summary
                        });
                    }
                    else
                    {
                        reponsitory.Update(new Layer.Data.Sqls.ScCustoms.EntryNotices
                        {
                            ID = this.ID,
                            OrderID = this.OrderID,
                            DecHeadID = this.DecHeadID,
                            WarehouseType = (int)this.WarehouseType,
                            ClientCode = this.ClientCode,
                            SortingRequire = (int)this.SortingRequire,
                            EntryNoticeStatus = (int)this.EntryNoticeStatus,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = DateTime.Now,
                            Summary = this.Summary
                        }, item => item.ID == this.ID);
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
        public override void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Status = Needs.Wl.Models.Enums.Status.Delete
                        }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess();
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
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