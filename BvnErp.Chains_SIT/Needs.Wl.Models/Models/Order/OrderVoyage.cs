using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    [Serializable]
    public class OrderVoyage : IUnique, IPersist
    {
        private string _id;

        public string ID
        {
            get { return this._id ?? string.Concat(this.Order.ID, this.Type).MD5(); }
            set { _id = value; }
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// 订单特殊类型
        /// </summary>
        public Enums.OrderSpecialType Type { get; set; }

        public Enums.Status Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        public OrderVoyage()
        {
            this.Status = Enums.Status.Normal;
            this.CreateTime = DateTime.Now;
            this.UpdateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderVoyages
                    {
                        ID = this.ID,
                        OrderID = this.Order.ID,
                        Type = (int)this.Type,
                        Status = (int)this.Status,
                        CreateDate = this.CreateTime,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderVoyages
                    {
                        ID = this.ID,
                        OrderID = this.Order.ID,
                        Type = (int)this.Type,
                        Status = (int)this.Status,
                        CreateDate = this.CreateTime,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }
            this.OnEnterSuccess();
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder DeleteSuccess;

        public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderVoyages>(item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        public void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }
    }
}
