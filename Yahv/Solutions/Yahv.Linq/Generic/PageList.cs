using System.Collections;
using System.Collections.Generic;

namespace Yahv.Linq.Generic
{

    /// <summary>
    /// 分页对象
    /// </summary>
    /// <typeparam name="T">指定类型</typeparam>
    public class PageList<T> : IEnumerable<T>
    {
        /// <summary>
        /// 单页返回条数
        /// </summary>
        public int PageSize { get; private set; }
        /// <summary>
        /// 指定页码
        /// </summary>
        public int PageIndex { get; private set; }
        /// <summary>
        /// 数据总数
        /// </summary>
        public int Total { get; private set; }

        IEnumerable<T> data;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="pageIndex">指定页码</param>
        /// <param name="pageSize">单页返回条数</param>
        /// <param name="data">数据</param>
        /// <param name="total">数据总数</param>
        public PageList(int pageIndex, int pageSize, IEnumerable<T> data, int total)
        {
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.data = data;
            this.Total = total;
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>支持在泛型集合上进行简单迭代</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
