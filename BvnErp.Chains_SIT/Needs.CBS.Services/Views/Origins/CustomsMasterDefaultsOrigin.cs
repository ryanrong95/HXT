using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Views.Origins
{
    /// <summary>
    /// 申报地默认关联视图
    /// </summary>
    internal class CustomsMasterDefaultsOrigin : UniqueView<Models.Origins.CustomsMasterDefault, ScCustomsReponsitory>
    {
        internal CustomsMasterDefaultsOrigin()
        {
        }

        internal CustomsMasterDefaultsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Origins.CustomsMasterDefault> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsMasterDefaults>()
                   select new Models.Origins.CustomsMasterDefault
                   {
                       ID = cert.ID,
                       Code = cert.Code,
                       EntyPortCode = cert.EntyPortCode,
                       IEPortCode = cert.IEPortCode,
                       InspOrgCode = cert.InspOrgCode,
                       OrgCode = cert.OrgCode,
                       PurpOrgCode = cert.PurpOrgCode,
                       VsaOrgCode = cert.VsaOrgCode,
                   };
        }
    }
}
