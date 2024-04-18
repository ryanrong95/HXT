using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layers.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>
    public class PsWmsRepository : LinqReponsitory<PsWms.SqlDataContext>
    {
        public PsWmsRepository()
        {
        }

        public PsWmsRepository(bool isAutoSumit) : base(isAutoSumit)
        {
        }

        /// <summary>
        /// 工厂构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        /// <param name="isFactory">是否为工厂行数</param>
        public PsWmsRepository(bool isAutoSumit, bool isFactory) : base(isAutoSumit, isFactory)
        {

        }
    }
}
