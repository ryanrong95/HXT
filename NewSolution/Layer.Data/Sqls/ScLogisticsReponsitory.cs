using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class ScLogisticsReponsitory : LinqReponsitory<ScLogistics.SqlDataContext>
    {
        public ScLogisticsReponsitory()
        {
        }

        public ScLogisticsReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
        
    }
}
