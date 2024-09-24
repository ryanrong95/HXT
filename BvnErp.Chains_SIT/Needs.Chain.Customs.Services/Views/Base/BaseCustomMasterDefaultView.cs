using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseCustomMasterDefaultView : UniqueView<Models.BaseCustomMasterDefault, ScCustomsReponsitory>
    {
        public BaseCustomMasterDefaultView()
        {
        }

        internal BaseCustomMasterDefaultView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseCustomMasterDefault> GetIQueryable()
        {
            var masterView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCustomMaster>();
            var entryPortView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseEntryPort>();
            var orgView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseOrgCode>();

            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCustomMasterDefault>()
                   join master in masterView on entity.Code equals master.Code
                   join ieport in masterView on entity.IEPortCode equals ieport.Code
                   join entryPort in entryPortView on entity.EntyPortCode equals entryPort.Code
                   //join orgCode in orgView on entity.OrgCode equals orgCode.Code
                   //join vsaOrgCode in orgView on entity.VsaOrgCode equals vsaOrgCode.Code
                   //join inspOrgCode in orgView on entity.InspOrgCode equals inspOrgCode.Code
                   join purpOrgCode in masterView on entity.PurpOrgCode equals purpOrgCode.Code
                   select new Models.BaseCustomMasterDefault
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       CodeName = master.Name,
                       IEPortCode = entity.IEPortCode,
                       IEPortCodeName = ieport.Name,
                       EntyPortCode = entity.EntyPortCode,
                       EntyPortCodeName = entryPort.Name,
                       //OrgCode = entity.OrgCode,
                       //OrgCodeName = orgCode.Name,
                       //VsaOrgCode = entity.VsaOrgCode,
                       //VsaOrgCodeName = vsaOrgCode.Name,
                       //InspOrgCode = entity.InspOrgCode,
                       //InspOrgCodeName = inspOrgCode.Name,
                       PurpOrgCode = entity.PurpOrgCode,
                       PurpOrgCodeName = purpOrgCode.Name,
                       IsDefault = entity.IsDefault
                   };
        }
    }
}
