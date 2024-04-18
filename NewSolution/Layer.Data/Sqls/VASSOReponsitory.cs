using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    /// <summary>
    /// 基本支持类
    /// </summary>

    public class VASSOReponsitory : LinqReponsitory<VASSO.SqlDataContext>
    {
        public VASSOReponsitory()
        {
        }

        public VASSOReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
