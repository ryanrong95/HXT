using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class IC360BvSsoReponsitory : LinqReponsitory<IC360BvSso.SqlDataContext>
    {
        public IC360BvSsoReponsitory()
        {
        }

        public IC360BvSsoReponsitory(bool isAutoSumit) : base(isAutoSumit)
        { 
        }
    }
}
