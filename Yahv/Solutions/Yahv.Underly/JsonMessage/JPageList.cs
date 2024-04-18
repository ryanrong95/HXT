using System.Collections.Generic;

namespace Yahv.Underly
{
    /// <summary>
    /// 数据列表分页格式
    /// </summary>
    public class JPageList<T>
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
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }
        /// <summary>
        /// 每页数据条数
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 数据总条数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 数据列表
        /// </summary>
        public IEnumerable<T> data { get; set; }
    }
}
