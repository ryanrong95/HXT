using System.Collections.Generic;

namespace Yahv.Underly
{
    /// <summary>
    /// 数据列表格式
    /// </summary>
    public class JList<T>
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
        /// 数据列表
        /// </summary>
        public IEnumerable<T> data { get; set; }
    }
}
