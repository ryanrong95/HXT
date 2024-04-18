using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Plats.Services.Models.Rolls
{
    public class Staff
    {
        public void Enter()
        {
            using (var r = LinqFactory<PvbCrmReponsitory>.Create())
            {


            }
            var r1 = new Layers.Data.Sqls.PvbCrmReponsitory();
            Linq.LinqContext.Current.Dispose();
        }
    }
}
