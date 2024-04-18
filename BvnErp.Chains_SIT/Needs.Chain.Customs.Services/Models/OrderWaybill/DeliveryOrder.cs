using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DeliveryOrder : IUnique
    {
        /// <summary>
        /// 运单号
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 出库类型
        /// </summary>
        public WaybillType WayBillType { get; set; }
        /// <summary>
        /// 件数
        /// </summary>
        public int? Quantity { get; set; }
        public string OrderID { get; set; }
        /// <summary>
        /// 运单执行状态
        /// </summary>
        public CgPickingExcuteStatus IsModify { get; set; }
        public DateTime? AppointTime { get; set; }
        public DateTime CreateDate { get; set; }
        public string Code { get; set; }
        public CgPickingExcuteStatus ConfirmReceiptStatus { get; set; }

        public string coeAddress { get; set; }
        public string coePlace { get; set; }
        public string coeContact { get; set; }
        public string coePhone { get; set; }
        public string coeIDNumber { get; set; }
        public DateTime? Expr1 { get; set; }

        /// <summary>
        /// 快递方式
        /// </summary>
        public int? ExpressTy { get; set; }

        /// <summary>
        /// 付费方式
        /// </summary>
        public Enums.PayType? ExpressPayType { get; set; }
        public DateTime? wldTakingDate { get; set; }
        public string wldTakingAddress { get; set; }
        public string wldTakingContact { get; set; }
        public string wldTakingPhone { get; set; }
        public string wldCarNumber1 { get; set; }
        public string wldDriver { get; set; }
        public string CarrierName { get; set; }
        public string CompanyName { get; set; }
        public string ClientCode { get; set; }
        public Admin Admin { get; set; }

        public string Url { get; set; }

        DeliveryOrderItems items;
        public DeliveryOrderItems Items
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.DeliveryOrderItemsView())
                    {
                        var query = view.Where(item => item.DeliveryOrderID == this.ID );
                        this.Items = new DeliveryOrderItems(query);
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

                this.items = new DeliveryOrderItems(value, new Action<DeliveryOrderItem>(delegate (DeliveryOrderItem item)
                {
                    item.DeliveryOrderID = this.ID;
                }));
            }
        }
    }
}
