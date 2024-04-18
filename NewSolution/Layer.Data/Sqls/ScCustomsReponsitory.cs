using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class ScCustomsReponsitory : LinqReponsitory<ScCustoms.SqlDataContext>
    {
        public ScCustomsReponsitory()
        {
        }

        public ScCustomsReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {

        }
    }
}
