using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class IC360BvOrdersReponsitory : LinqReponsitory<IC360BvOrders.SqlDataContext>
    {
        public IC360BvOrdersReponsitory()
        {
        }

        public IC360BvOrdersReponsitory(bool isAutoSumit) : base(isAutoSumit)
        { 
        }
    }
}
