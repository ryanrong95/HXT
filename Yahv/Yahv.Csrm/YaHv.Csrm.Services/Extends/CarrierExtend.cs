using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.Csrm.Services.Extends
{
    public static class CarrierExtend
    {
        /// <summary>
        /// 审批结果：正常，否决
        /// </summary>
        static public void Enable(this Models.Origins.Carrier entity)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Carriers>(new
                {
                    Status = Yahv.Underly.GeneralStatus.Normal
                }, item => item.ID == entity.ID);
                
            }
        }
        
    }
}
