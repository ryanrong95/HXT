using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class MyClientsRoll : Linq.UniqueView<Client, PvdCrmReponsitory>
    {
        IErpAdmin admin;
        public MyClientsRoll(IErpAdmin Admin)
        {
            this.admin = Admin;
        }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected MyClientsRoll(PvdCrmReponsitory reponsitory, IQueryable<Client> iQueryable) : base(reponsitory, iQueryable)
        {

        }
        protected override IQueryable<Client> GetIQueryable()
        {
            //排除公海客户
            var publicIDs = new ConductsOrigin().Where(x => x.IsPublic == true).Select(x => x.EnterpriseID).ToArray();
         
            if (admin.IsSuper)
            {
                var ids = new RelationsOrigin().Where(x => x.Status != Underly.AuditStatus.Closed && x.Status != Underly.AuditStatus.Black).Select(x=>x.ClientID).Distinct().ToArray();
                var newid = ids.Except(publicIDs).ToArray();
                return new MyClientsOrigin(this.Reponsitory).Where(x=> newid.Contains(x.ID));

            }
            var IDs = new MyRelationRoll(admin).Where(x=>x.Status!=Underly.AuditStatus.Closed && x.Status != Underly.AuditStatus.Black).Select(x=>x.ClientID).ToArray();
            var newids = IDs.Except(publicIDs).ToArray();
            return new MyClientsOrigin(this.Reponsitory).Where(item => newids.Contains(item.ID));
        }
    }
}
