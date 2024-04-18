using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 客户的客服类型
    /// </summary>
    public enum ClientAdminType
    {
        /// <summary>
        /// 业务经理
        /// </summary>
        [Description("业务经理")]
        ServiceManager = 1,

        /// <summary>
        /// 跟单员
        /// </summary>
        [Description("跟单员")]
        Merchandiser = 2
    }   
}