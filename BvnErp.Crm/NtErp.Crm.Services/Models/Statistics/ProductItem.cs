using Needs.Linq;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Statistics
{
    public class ProductItem : IUnique
    {
        #region 属性

        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 标准产品ID
        /// </summary>
        public string StandardID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// PM
        /// </summary>
        public string PMAdmin { get; set; }

        /// <summary>
        /// FAE
        /// </summary>
        public string FAEAdmin { get; set; }

        /// <summary>
        /// 销售
        /// </summary>
        public string SaleAdmin { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 申请
        /// </summary>
        public Apply Apply { get; set; }

        #endregion
    }

    public class Apply
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 申请类型
        /// </summary>
        public ApplyType Type { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}
