using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layers.Data.Sqls
{
    public class PveStandardReponsitory : LinqReponsitory<PveStandard.SqlDataContext>
    { /// <summary>
      /// 默认构造器
      /// </summary>
        public PveStandardReponsitory()
        {
        }

        /// <summary>
        /// 不建议使用，构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        public PveStandardReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }


        /// <summary>
        /// 工厂构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        /// <param name="isFactory">是否为工厂行数</param>
        public PveStandardReponsitory(bool isAutoSumit, bool isFactory) : base(isAutoSumit, isFactory)
        {

        }
    }

}
