using Layer.Data.Sqls.ScCustoms;
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
    /// 出库通知Items
    /// </summary>
    [Serializable]
    public class ExitNoticeItem : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }

        public string ExitNoticeID { get; set; }

        /// <summary>
        /// 销售单、报关单明细
        /// </summary>
        public virtual DecList DecList { get; set; }

        /// <summary>
        /// 分拣结果(深圳出库使用)
        /// </summary>
        public virtual SZSorting Sorting { get; set; }

        public decimal Quantity { get; set; }

        /// <summary>
        /// 出库状态
        /// </summary>
        public ExitNoticeStatus ExitNoticeStatus { get; set; }

        public Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 出库通知类初始化
        /// </summary>
        public ExitNoticeItem()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
            this.ExitNoticeStatus = Enums.ExitNoticeStatus.UnExited;
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
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ExitNoticeItem);
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
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
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

    public class ExitNoticeItems : BaseItems<ExitNoticeItem>
    {
        internal ExitNoticeItems(IEnumerable<ExitNoticeItem> enums) : base(enums)
        {
        }

        internal ExitNoticeItems(IEnumerable<ExitNoticeItem> enums, Action<ExitNoticeItem> action) : base(enums, action)
        {
        }

        public override void Add(ExitNoticeItem item)
        {
            base.Add(item);
        }
    }
}