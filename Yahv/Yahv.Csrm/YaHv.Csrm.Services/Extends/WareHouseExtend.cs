using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Extends
{
   static public class WareHouseExtend
    {
        static public void Delete(this IEnumerable<Models.Origins.WareHouse> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.WareHouses>(new
                {
                    Status = GeneralStatus.Deleted
                }, item => arry.Contains(item.ID));
            }
        }
    }
}
