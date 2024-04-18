using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 入库通知Item
    /// </summary>
    public class EntryNoticeItem : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }

        public string EntryNoticeID { get; set; }

        /// <summary>
        /// 香港库房 进项
        /// </summary>
        public string OrderItemID { get; set; }
        public virtual OrderItem OrderItem { get; set; }

        /// <summary>
        /// 深圳库房 进项
        /// </summary>
        public string DecListID { get; set; }
        public virtual DecList DecList { get; set; }

        /// <summary>
        /// 是否抽检
        /// </summary>
        public virtual bool IsSportCheck { get; set; }

        public EntryNoticeStatus EntryNoticeStatus { get; set; }

        public Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 入库通知项类初始化
        /// </summary>
        public EntryNoticeItem()
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
        /// 数据持久化
        /// </summary>
        public virtual void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem);
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
        public virtual void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
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

    public class EntryNoticeItems : BaseItems<EntryNoticeItem>
    {
        internal EntryNoticeItems(IEnumerable<EntryNoticeItem> enums) : base(enums)
        {
        }

        internal EntryNoticeItems(IEnumerable<EntryNoticeItem> enums, Action<EntryNoticeItem> action) : base(enums, action)
        {
        }

        public override void Add(EntryNoticeItem item)
        {
            base.Add(item);
        }
    }
}