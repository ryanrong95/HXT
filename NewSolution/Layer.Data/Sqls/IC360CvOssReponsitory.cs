using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class IC360CvOssReponsitory:LinqReponsitory<IC360CvOss.SqlDataContext>
    {
        public IC360CvOssReponsitory()
        {
        }

        public IC360CvOssReponsitory(bool isAutoSumit) : base(isAutoSumit)
        { 
        }
    }
}
