using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.View.Origins
{
    internal class CustomMasterDefaultOrigin : UniqueView<Needs.Cbs.Services.Model.Origins.CustomMasterDefault, ScCustomsReponsitory>
    {
        internal CustomMasterDefaultOrigin()
        {
        }

        internal CustomMasterDefaultOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Needs.Cbs.Services.Model.Origins.CustomMasterDefault> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsMasterDefaults>()
                   select new Needs.Cbs.Services.Model.Origins.CustomMasterDefault
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
