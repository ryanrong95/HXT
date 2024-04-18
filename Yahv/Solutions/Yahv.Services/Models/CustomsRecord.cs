using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 清关记录
    /// </summary>
    public class CustomsRecord : Yahv.Linq.IUnique
    {
        public string ID { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }
        /// <summary>
        /// 清关费
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 到货时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime? ConfirmDate { get; set; }
    }
}
