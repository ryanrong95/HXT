using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 订单附件的审核状态
    /// </summary>
    public enum OrderFileStatus
    {
        /// <summary>
        /// 未上传
        /// </summary>
        [Description("未上传")]
        NotUpload = 0,

        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        Auditing = 1,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        Audited = 2,
    }
}