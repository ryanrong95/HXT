using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class CustomsBaseSystemReponsitory : LinqReponsitory<Customs.SqlDataContext>
    {
        public CustomsBaseSystemReponsitory()
        {
        }

        public CustomsBaseSystemReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {

        }
    }
}
