namespace Yahv.Underly
{
    /// <summary>
    /// 单条数据格式
    /// </summary>
    public class JSingle<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 标识值
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 单条数据
        /// </summary>
        public T data { get; set; }
    }

    /// <summary>
    /// 单条数据格式
    /// </summary>
    public class JSingle : JSingle<object>
    {

    }
}
