using Layers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PSLToSolrData.Import.Linqs
{
    public class PSLReponsitory : LinqReponsitory<Linqs.PSLDataContext>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public PSLReponsitory()
        {
        }

        /// <summary>
        /// 不建议使用，构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        public PSLReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {

        }
    }
}
