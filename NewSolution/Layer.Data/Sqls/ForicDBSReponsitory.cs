using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class ForicDBSReponsitory : LinqReponsitory<foricDBS.foricDBSDataContext>
    {
        public ForicDBSReponsitory()
        {
        }

        public ForicDBSReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {

        }
    }
}
