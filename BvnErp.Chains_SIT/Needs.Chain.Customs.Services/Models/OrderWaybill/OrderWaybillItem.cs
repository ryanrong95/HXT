using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单的国际运单项
    /// </summary>
    public class OrderWaybillItem : IUnique, IPersist, IFulError, IFulSuccess
    {
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Sorting.ID + this.OrderWaybillID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OrderWaybillID { get; set; }

        /// <summary>
        /// 到货信息
        /// </summary>
        public Sorting Sorting { get; set; }

        public OrderWaybillItem()
        {

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
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
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
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
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

    public class OrderWaybillItems : BaseItems<OrderWaybillItem>
    {
        internal OrderWaybillItems(IEnumerable<OrderWaybillItem> enums) : base(enums)
        {
        }

        internal OrderWaybillItems(IEnumerable<OrderWaybillItem> enums, Action<OrderWaybillItem> action) : base(enums, action)
        {
        }

        public override void Add(OrderWaybillItem item)
        {
            base.Add(item);
        }
    }
}