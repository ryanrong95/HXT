using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly.CrmPlus
{
    public enum StractureType
    {
        /// <summary>
        /// 地区
        /// </summary>
        [Description("地区")]
        [Fixed("")]
        Region = 1,

        /// <summary>
        /// 公司
        /// </summary>
        [Description("公司")]
        [Fixed("icon-company")]
        Company = 2,

        /// <summary>
        /// 部门
        /// </summary>
        [Description("部门")]
        [Fixed("icon-department")]
        Department = 3,

        /// <summary>
        /// 组
        /// </summary>
        [Description("组")]
        [Fixed("icon-group")]
        Group = 4,

        /// <summary>
        /// 职务
        /// </summary>
        [Description("职务")]
        [Fixed("icon-position")]
        Position = 5,
    }
}
