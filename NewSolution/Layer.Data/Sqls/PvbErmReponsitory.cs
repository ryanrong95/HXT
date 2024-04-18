using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class PvbErmReponsitory : LinqReponsitory<PvbErm.sqlDataContext>
    {
        public PvbErmReponsitory()
        {
        }

        public PvbErmReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {

        }
    }
}
