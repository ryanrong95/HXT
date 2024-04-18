using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// SwapNoticeItem
    /// </summary>
    [Serializable]
    public class SwapNoticeItem : IUnique, IPersist
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// SwapNoticeID
        /// </summary>
        public string SwapNoticeID { get; set; } = string.Empty;

        /// <summary>
        /// DecHeadID
        /// </summary>
        public string DecHeadID { get; set; } = string.Empty;

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 本次换汇金额、单次换汇金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// 自定义金额
        /// </summary>
        public decimal CustomizeAmount { get; set; }

        public event SuccessHanlder AbandonSuccess;
        //public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        //public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>(new Layer.Data.Sqls.ScCustoms.SwapNoticeItems()
                    {
                        ID = this.ID,
                        SwapNoticeID = this.SwapNoticeID,
                        DecHeadID = this.DecHeadID,
                        CreateDate = this.CreateDate,
                        Amount = this.Amount,
                        Status = (int)this.Status,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>(new
                    {
                        SwapNoticeID = this.SwapNoticeID,
                        DecHeadID = this.DecHeadID,
                        CreateDate = this.CreateDate,
                        Amount = this.Amount,
                        Status = (int)this.Status,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandon();
        }

        virtual protected void OnAbandon()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }
    }
}
