using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvRoute.Services.Models
{
    /// <summary>
    /// 对账联系人表
    /// </summary>
    public class BillContact:IUnique
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 账单编号
        /// </summary>
        public string BillID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单项编号
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 所占价值
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

    }
}
