using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layers.Data.Sqls
{
    /// <summary>
    /// 2020/06/16版本
    /// </summary>
    public class PveCrm2Reponsitory : LinqReponsitory<PveCrm2.sqlDataContext>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public PveCrm2Reponsitory()
        {
        }

        /// <summary>
        /// 不建议使用，构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        public PveCrm2Reponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }


        /// <summary>
        /// 工厂构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        /// <param name="isFactory">是否为工厂行数</param>
        public PveCrm2Reponsitory(bool isAutoSumit, bool isFactory) : base(isAutoSumit, isFactory)
        {

        }
    }
}
