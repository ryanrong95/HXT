namespace Yahv.Web.Controls.Easyui
{
    /// <summary>
    /// 标题
    /// </summary>
    public enum AutoSign
    {

        None = 0,

        /// <summary>
        /// 信息：info,
        /// </summary>
        Info,

        /// <summary>
        /// 错误：error,
        /// </summary>
        Error,

        /// <summary>
        /// 成功：success,
        /// </summary>
        Success,
        /// <summary>
        /// 警告：warning
        /// </summary>
        Warning
    }

    public class AutoAlert : JlEasyuiControl
    {
        public string Context { get; set; }

        public AutoSign Sign { get; set; }
    }
}
