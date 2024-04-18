using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class ScSalesReponsitory : LinqReponsitory<ScSales.SqlDataContext>
    {
        public ScSalesReponsitory()
        {
        }

        public ScSalesReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
        
    }
}
