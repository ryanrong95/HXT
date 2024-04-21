using Yahv.Underly.Attributes;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 组织机构类型
    /// </summary>
    public enum LeagueType
    {
        /// <summary>
        /// 地域
        /// </summary>
        [Description("地域")]
        Area = 1,

        /// <summary>
        /// 公司
        /// </summary>
        [Description("公司")]
        Company = 2,

        /// <summary>
        /// 公司
        /// </summary>
        [Description("部门", "工作组", "Group")]
        Department = 3,

        /// <summary>
        /// 职位
        /// </summary>
        [Description("职位")]
        Position = 4,
    }
}