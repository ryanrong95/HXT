using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Models
{
    /// <summary>
    /// 收支列表
    /// </summary>
    public class PaymentRecord
    {
        public RecordModel[] Records { get; set; }
        public CurrentModel[] Currents { get; set; }

        /// <summary>
        /// 记录实体
        /// </summary>
        public class Record
        {
            /// <summary>
            /// 业务
            /// </summary>
            public string Conduct { get; set; }

            /// <summary>
            /// 分类
            /// </summary>
            public string Catalog { get; set; }

            /// <summary>
            /// 科目
            /// </summary>
            public string Subject { get; set; }

            /// <summary>
            /// 录入时间
            /// </summary>
            public string CreateDate { get; set; }

            /// <summary>
            /// 录入人
            /// </summary>
            public string Creator { get; set; }

            /// <summary>
            /// 承运商（收款人） or 客户（付款人）
            /// </summary>
            public string TargetName { get; set; }

            /// <summary>
            /// 快递单号
            /// </summary>
            public string TrackingNumber { get; set; }

            /// <summary>
            /// 描述
            /// </summary>
            public string Descirption { get; set; }

            /// <summary>
            /// 数量
            /// </summary>
            public int? Quantity { get; set; }
        }

        /// <summary>
        /// 记录
        /// </summary>
        public class RecordModel : Record
        {
            /// <summary>
            /// 计价金额
            /// </summary>
            /// <remarks>币种+金额</remarks>
            public string RecordPrice { get; set; }

            /// <summary>
            /// 结算金额
            /// </summary>
            /// <remarks>币种+金额</remarks>
            public string SettlePrice { get; set; }
        }

        /// <summary>
        /// 现金
        /// </summary>
        public class CurrentModel : Record
        {
            /// <summary>
            /// 结算金额
            /// </summary>
            public string Price { get; set; }
        }
    }
}