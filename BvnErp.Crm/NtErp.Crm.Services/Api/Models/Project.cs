using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Api.Models
{
    /// <summary>
    /// 销售机会实体类
    /// </summary>
    public class Project
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Project()
        {

        }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string Partnumber { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufaturer { get; set; }
        /// <summary>
        /// 品牌简称
        /// </summary>
        public string ManufaturerShortName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus Status { get; set; }
        /// <summary>
        /// 状态变更时间
        /// </summary>
        public DateTime StatusDate { get; set; }
        /// <summary>
        /// 询价信息
        /// </summary>
        public IEnumerable<EnquirySingle> Enquiries { get; set; }

        /// <summary>
        /// 状态变更申请信息
        /// </summary>
        public ApplySingle Apply { get; set; }
        /// <summary>
        /// 询价信息
        /// </summary>
        public class EnquirySingle
        {
            /// <summary>
            /// 客户ID
            /// </summary>
            public string ClientID { get; set; }

            /// <summary>
            /// 品牌
            /// </summary>
            public string Mf { get; set; }
            /// <summary>
            /// 品牌简称
            /// </summary>
            public string MfShortName { get; set; }
            /// <summary>
            /// 报备时间
            /// </summary>
            public DateTime ReportDate { get; set; }
        }

        /// <summary>
        /// 型号状态申请信息
        /// </summary>
        public class ApplySingle
        {
            /// <summary>
            /// 申请类型
            /// </summary>
            public ApplyType Type { get; set; }
            /// <summary>
            /// 更新时间
            /// </summary>
            public DateTime UpdateDate { get; set; }
        }
    }
}
