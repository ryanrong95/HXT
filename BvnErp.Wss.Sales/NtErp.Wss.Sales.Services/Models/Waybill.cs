using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models
{
    /// <summary>
    /// 运单
    /// </summary>
    public class Waybill
    {
        public Waybill()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string WaybillNumber { get; set; }
        /// <summary>
        /// 承运商
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// 运费支付方
        /// </summary>
        public FreightPayer Payer { get; set; }

        /// <summary>
        /// 实际执行运费
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public string Weight { get; set; }

        public int Count { get; set; }

        /// <summary>
        /// 体积大小
        /// </summary>
        public string Measurement { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string AdminID { get; set; }

        #endregion
    }
}
