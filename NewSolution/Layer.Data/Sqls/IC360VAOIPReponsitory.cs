using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class IC360VAOIPReponsitory : LinqReponsitory<IC360Inquiry.SqlDataContext>
    {
        public IC360VAOIPReponsitory()
        {
        }

        public IC360VAOIPReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
