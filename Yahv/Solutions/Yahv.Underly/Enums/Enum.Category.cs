using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 组织机构分类
    /// </summary>
    public enum Category
    {
        /// <summary>
        /// 工作关系
        /// </summary>
        [Description("工作关系")]
        Work = 1,

        /// <summary>
        /// 员工所在城市
        /// </summary>
        [Description("员工所在城市")]
        StaffInCity = 2,

        /// <summary>
        /// 询报价区域管理
        /// </summary>
        [Description("询报价区域管理")]
        RFQ = 3
    }
}
