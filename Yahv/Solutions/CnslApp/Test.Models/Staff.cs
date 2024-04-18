using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
    public class Staff
    {
        public void Enter()
        {
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            using (var r = LinqFactory<PvbCrmReponsitory>.Create())
            {


            }
        }
    }
}
