using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layers.Data.Sqls
{
    public class PveSmsReponsitory : LinqReponsitory<PveSms.sqlDataContext>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public PveSmsReponsitory()
        {
        }

        /// <summary>
        /// 不建议使用，构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        public PveSmsReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }


        /// <summary>
        /// 工厂构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        /// <param name="isFactory">是否为工厂行数</param>
        public PveSmsReponsitory(bool isAutoSumit, bool isFactory) : base(isAutoSumit, isFactory)
        {

        }
    }
}
