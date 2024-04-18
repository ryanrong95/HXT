using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
  
    public class BomsReponsitory : LinqReponsitory<Boms.SqlDataContext>
    {
        public BomsReponsitory()
        {
        }

        public BomsReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}
