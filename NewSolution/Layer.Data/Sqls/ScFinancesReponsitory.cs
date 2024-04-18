using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class ScFinancesReponsitory : LinqReponsitory<ScFinances.SqlDataContext>
    {
        public ScFinancesReponsitory()
        {
        }

        public ScFinancesReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
        
    }
}
