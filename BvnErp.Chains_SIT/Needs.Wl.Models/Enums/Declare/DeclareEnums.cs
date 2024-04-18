using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 是否上传流水
    /// </summary>
    public enum UploadStatus
    {
        [Description("未上传")]
        NotUpload = 0,

        [Description("已上传")]
        Uploaded = 1,
    }

    /// <summary>
    /// 处理类型(复合形式)
    /// </summary>
    public enum HandledType
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        NoHandled = 0,

        /// <summary>
        /// 关税已处理
        /// </summary>
        [Description("关税")]
        Tariff = 1,

        /// <summary>
        /// 增值税已处理
        /// </summary>
        [Description("增值税")]
        AddedValueTax = 2,
    }
}
