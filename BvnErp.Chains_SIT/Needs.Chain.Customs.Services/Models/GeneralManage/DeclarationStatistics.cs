using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DeclarationStatistics : IUnique
    {
        /// <summary>
        /// 主键ID/业务员ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 业务员名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 报关总金额
        /// </summary>
        public decimal TotalDeclarePrice { get; set; }

        /// <summary>
        /// 主键ID/订单OrderID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// 下单时的会员补充协议
        /// </summary>
        internal ClientAgreement Agreement { get; set; }

        /// <summary>
        /// 报关货值
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public Enums.OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientName { get; set; }
        
    }
}