using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class ScPurchasesReponsitory : LinqReponsitory<ScPurchases.SqlDataContext>
    {
        public ScPurchasesReponsitory()
        {
        }

        public ScPurchasesReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
