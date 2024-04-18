using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// SwapNotices、OrderReceipts 使用关联关系
    /// </summary>
    public class SwapNoticeReceiptUse : IUnique, IPersistence, IFulError, IFulSuccess
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// SourceID
        /// </summary>
        public string SourceID { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public Enums.ReceiptUseType Type { get; set; }

        /// <summary>
        /// OrderReceiptID
        /// </summary>
        public string OrderReceiptID { get; set; }

        /// <summary>
        /// OrderReceiptAmount
        /// </summary>
        public decimal OrderReceiptAmount { get; set; }

        /// <summary>
        /// SwapNoticeID
        /// </summary>
        public string SwapNoticeID { get; set; }

        /// <summary>
        /// SwapNoticeAmountCNY
        /// </summary>
        public decimal? SwapNoticeAmountCNY { get; set; }

        /// <summary>
        /// 数据状态(200,400)
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>(new Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses
                    {
                        ID = this.ID,
                        SourceID = this.SourceID,
                        Type = (int)this.Type,
                        OrderReceiptID = this.OrderReceiptID,
                        OrderReceiptAmount = this.OrderReceiptAmount,
                        SwapNoticeID = this.SwapNoticeID,
                        SwapNoticeAmountCNY = this.SwapNoticeAmountCNY,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>(new
                    {
                        SourceID = this.SourceID,
                        Type = (int)this.Type,
                        OrderReceiptID = this.OrderReceiptID,
                        OrderReceiptAmount = this.OrderReceiptAmount,
                        SwapNoticeID = this.SwapNoticeID,
                        SwapNoticeAmountCNY = this.SwapNoticeAmountCNY,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>(new
                {
                    Status = Enums.Status.Delete,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
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
