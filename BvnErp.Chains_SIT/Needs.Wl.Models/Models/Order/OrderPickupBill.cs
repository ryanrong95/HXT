using Needs.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 订单的提货单
    /// </summary>
    public class OrderPickupBill : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 提货单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int PackNo { get; set; }

        /// <summary>
        /// 提货人姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 提货人电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public Enums.IDType IDType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string IDNumber { get; set; }
    }
}