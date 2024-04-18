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
    public class CvCSReponsitory : LinqReponsitory<CvCS.SqlDataContext>
    {
        public CvCSReponsitory()
        {
        }

        public CvCSReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
