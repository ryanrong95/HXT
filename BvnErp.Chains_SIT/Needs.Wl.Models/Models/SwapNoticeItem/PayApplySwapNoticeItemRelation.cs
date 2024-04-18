using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// PayExchangeApplyItems 和 SwapNoticeItems 关系表
    /// </summary>
    public class PayApplySwapNoticeItemRelation : IUnique, IPersist
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// PayExchangeApplyItemID
        /// </summary>
        public string PayExchangeApplyItemID { get; set; } = string.Empty;

        /// <summary>
        /// SwapNoticeItemID
        /// </summary>
        public string SwapNoticeItemID { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// UpdateDate
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; }

        //public event SuccessHanlder AbandonSuccess;
        //public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        //public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayApplySwapNoticeItemRelation>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayApplySwapNoticeItemRelation>(new Layer.Data.Sqls.ScCustoms.PayApplySwapNoticeItemRelation()
                    {
                        ID = this.ID,
                        PayExchangeApplyItemID = this.PayExchangeApplyItemID,
                        SwapNoticeItemID = this.SwapNoticeItemID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayApplySwapNoticeItemRelation>(new
                    {
                        PayExchangeApplyItemID = this.PayExchangeApplyItemID,
                        SwapNoticeItemID = this.SwapNoticeItemID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
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
    }
}
