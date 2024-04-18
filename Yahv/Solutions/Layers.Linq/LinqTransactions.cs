using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layers.Linq
{
    /// <summary>
    /// Linq 事物帮助类
    /// </summary>
    /// <remarks>
    /// 按照很多个人员的要求：增加本封装。
    /// 有什么问题联系陈翰
    /// </remarks>
    public class LinqTransactions : IDisposable
    {
        /// <summary>
        /// 指定连接的事务锁定行为
        /// </summary>
        public IsolationLevel IsolationLevel { get; internal set; }

        List<DbTransaction> list;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="transaction">事物</param>
        /// <param name="transactions">联合事物</param>
        internal LinqTransactions(DbTransaction transaction, IEnumerable<DbTransaction> transactions = null)
        {
            this.list = new List<DbTransaction>(5);
            this.list.Add(transaction);
            if (transactions != null)
            {
                list.AddRange(transactions);
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            this.list.ForEach(item => item.Commit());
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void Rollback()
        {
            this.list.ForEach(item => item.Rollback());
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            this.list.ForEach(item => item.Dispose());
        }
    }
}
