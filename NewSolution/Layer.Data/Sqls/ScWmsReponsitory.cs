using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class ScWmsReponsitory : LinqReponsitory<ScWms.SqlDataContext>
    {
        public ScWmsReponsitory()
        {
        }

        public ScWmsReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
        
    }
}
