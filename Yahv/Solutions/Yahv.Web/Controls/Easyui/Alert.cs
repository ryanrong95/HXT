namespace Yahv.Web.Controls.Easyui
{
    public enum Sign
    {
        None = 0,
        Info,
        Error,
        Question,
        Warning,
    }

    /// <summary>
    /// 弹出样式
    /// </summary>
    public enum Method
    {
        None = 0,
        Window = 1,
        Dialog = 2
    }

    /// <summary>
    /// alert对象消息
    /// </summary>
    public class Alert : EasyuiControl
    {
        public string Title { get; set; }

        public string Context { get; set; }

        public Sign Sign { get; set; }

        public bool isClose { get; set; }
        public Method Method { get; set; }
    }
}
