using System.Collections.Generic;

namespace Yahv.Erm.AttendService
{
    /// <summary>
    /// 单条数据格式
    /// </summary>
    public class JResult<T>
    {
        /// <summary>
        /// 错误编码
        /// </summary>
        public int errCod { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errMsg { get; set; }
        /// <summary>
        /// 单条数据
        /// </summary>
        public IEnumerable<T> data { get; set; }
    }
}