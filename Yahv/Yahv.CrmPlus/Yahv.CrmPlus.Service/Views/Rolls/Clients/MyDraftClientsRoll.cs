using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    
    /// <summary>
    /// 注册
    /// </summary>
    public class MyDraftClientsRoll : MyDraftClientsOrigin
    {

        IErpAdmin Admin;
        public MyDraftClientsRoll() { }
        public MyDraftClientsRoll(IErpAdmin admin)
        {
            this.Admin = admin;
        }
        protected override IQueryable<Models.Origins.Client> GetIQueryable()
        {
            if (Admin.IsSuper)
            {
                var ids = new RelationsOrigin().Where(x => x.Status != Underly.AuditStatus.Closed && x.Status != Underly.AuditStatus.Black).Select(x => x.ClientID).ToArray();
                return base.GetIQueryable().Where(x => ids.Contains(x.ID));

            }
            var IDs = new MyRelationRoll(Admin).Where(x => x.Status != Underly.AuditStatus.Closed && x.Status != Underly.AuditStatus.Black).Select(x => x.ClientID).ToArray();
            return base.GetIQueryable().Where(item=>IDs.Contains(item.ID)); ;
        }

    }

   
}
