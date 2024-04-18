using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class UnExpectedOrderItem:IUnique
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public decimal Qty { get; set; }
        /// <summary>
        /// 批号
        /// </summary>
        public string Batch { get; set; }
        public string Origin { get; set; }
        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxIndex { get; set; }
        public decimal GrossWeight { get; set; }
        /// <summary>
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }
        /// <summary>
        /// 异常原因
        /// </summary>
        public string UnExpectedReason { get; set; }
        /// <summary>
        /// 是否已匹配
        /// </summary>
        public bool IsMapped { get; set; }

        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public UnExpectedOrderItem()
        {
            this.IsMapped = false;
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }
        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.UnExpectedOrderItem
                    {
                        ID = this.ID,
                        OrderID = this.OrderID,
                        Model = this.Model,
                        Brand = this.Brand,
                        Qty = this.Qty,
                        Origin = this.Origin,
                        Batch = this.Batch,
                        BoxIndex = this.BoxIndex,
                        GrossWeight = this.GrossWeight,
                        Volume = this.Volume,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        UnexpectedReason = this.UnExpectedReason,
                        Summary = this.Summary,
                        IsMapped = this.IsMapped
                    });
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
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

    }
}
