using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 付汇申请明细
    /// </summary>
    public class PayExchangeApplyItem : ModelBase<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems, ScCustomsReponsitory>, IUnique, IPersist
    {
        public string PayExchangeApplyID { get; set; }

        public string OrderID { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// 报关总价
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 已申请付汇总价
        /// </summary>
        public decimal PaidExchangeAmount { get; set; }

        /// <summary>
        /// 应收货款总额
        /// </summary>
        public decimal ReceivableAmount { get; set; }

        /// <summary>
        /// 实收货款总额
        /// </summary>
        public decimal ReceivedAmount { get; set; }

        /// <summary>
        /// 申请状态
        /// </summary>
        public Enums.ApplyItemStatus ApplyStatus { get; set; }

        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>().Count(item => item.ID == this.ID);

            if (count == 0)
            {
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyItem);
                this.Reponsitory.Insert(this.ToLinq());
            }
            else
            {
                this.Reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
            }
        }
    }
}