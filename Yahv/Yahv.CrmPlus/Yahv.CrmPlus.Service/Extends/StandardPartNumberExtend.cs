using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;

namespace Yahv.CrmPlus.Service.Extends
{
    public static class StandardPartNumberExtend
    {
        static public void Enable(this StandardPartNumber entity)
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardPartNumbers>().Any(item => item.ID == entity.ID))
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.StandardPartNumbers>(new
                    {
                        Status = (int)Underly.DataStatus.Normal
                    }, item => item.ID == entity.ID);

                }
            }
        }
    }
}
