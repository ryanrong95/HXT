using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layers.Data.Sqls
{
    public class PveCrmReponsitory : LinqReponsitory<PveCrm.sqlDataContext>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public PveCrmReponsitory()
        {
        }

        /// <summary>
        /// 不建议使用，构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        public PveCrmReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }


        /// <summary>
        /// 工厂构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        /// <param name="isFactory">是否为工厂行数</param>
        public PveCrmReponsitory(bool isAutoSumit, bool isFactory) : base(isAutoSumit, isFactory)
        {

        }
    }
}
