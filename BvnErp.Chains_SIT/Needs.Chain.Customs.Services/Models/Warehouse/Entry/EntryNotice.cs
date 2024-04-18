using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 入库通知
    /// </summary>
    public class EntryNotice : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }

        /// <summary>
        /// 客户订单
        /// </summary>
        public string OrderID { get; set; }
        public Interfaces.IOrder Order { get; set; }

        public string DecHeadID { get; set; }
        public virtual DecHead DecHead { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 分拣要求
        /// </summary>
        public virtual Enums.SortingRequire SortingRequire { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        public Enums.WarehouseType WarehouseType { get; set; }

        public Enums.EntryNoticeStatus EntryNoticeStatus { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #region 操作人

        internal Admin Operator;

        public void SetAdmin(Admin admin)
        {
            this.Operator = admin;
        }

        #endregion

        public EntryNotice()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
            this.EntryNoticeStatus = Enums.EntryNoticeStatus.UnBoxed;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public virtual void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>().Count(item => item.ID == this.ID);

                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice);
                        reponsitory.Insert(this.ToLinq());     
                    }
                    else
                    {
                        UpdateDate = DateTime.Now;
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
        public virtual void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Status = Enums.Status.Delete
                        }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess();
            }
            catch(Exception ex)
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