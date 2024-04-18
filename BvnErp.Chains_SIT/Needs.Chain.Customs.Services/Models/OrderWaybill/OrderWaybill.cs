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
    /// 订单的国际运单
    /// </summary>
    [Serializable]
    public class OrderWaybill : IUnique, IPersist, IFulError, IFulSuccess
    {
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.OrderID, this.WaybillCode).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public Carrier Carrier { get; set; }

        /// <summary>
        /// 运单编号
        /// </summary>
        public string WaybillCode { get; set; }

        /// <summary>
        /// 到港日期
        /// </summary>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// 添加人、管理员
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 香港申报状态
        /// </summary>
        public Enums.HKDeclareStatus HKDeclareStatus { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 国际快递项明细项
        /// </summary>
        OrderWaybillItems items;
        public OrderWaybillItems Items
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.OrderWaybillItemView())
                    {
                        var query = view.Where(item => item.OrderWaybillID == this.ID);
                        this.Items = new OrderWaybillItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.items = new OrderWaybillItems(value, delegate (OrderWaybillItem item)
               {
                   item.OrderWaybillID = this.ID;
               });
            }
        }
        
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        /// <returns></returns>
        public decimal? TotalCount { get; set; }

        /// <summary>
        /// 总价格
        /// </summary>
        public decimal? TotalPrice { get; set; }

        public OrderWaybill()
        {
            this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
            this.HKDeclareStatus = Enums.HKDeclareStatus.UnDeclare;
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
                foreach(var item in this.Items)
                {
                    item.Enter();
                }

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybills>().Count(item => item.ID == this.ID||item.WaybillCode==this.WaybillCode);
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
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderWaybills>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        /// <summary>
        /// 清关
        /// </summary>
        public void Clear()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderWaybills>(new { HKDeclareStatus = Enums.HKDeclareStatus.Declared }, item => item.ID == this.ID);
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

    [Serializable]
    public class OrderWaybills : BaseItems<OrderWaybill>
    {
        internal OrderWaybills(IEnumerable<OrderWaybill> enums) : base(enums)
        {
        }

        internal OrderWaybills(IEnumerable<OrderWaybill> enums, Action<OrderWaybill> action) : base(enums, action)
        {
        }

        public override void Add(OrderWaybill item)
        {
            base.Add(item);
        }
    }
}