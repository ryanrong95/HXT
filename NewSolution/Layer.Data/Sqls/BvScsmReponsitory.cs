using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls
{
    public class BvScsmReponsitory : LinqReponsitory<BvScsm.SqlDataContext>
    {
        public BvScsmReponsitory()
        {
        }

        public BvScsmReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {
        }
    }
}