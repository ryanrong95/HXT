using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ManifestsView : UniqueView<Models.Manifest, ScCustomsReponsitory>
    {
        public ManifestsView()
        {
        }

        internal ManifestsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Manifest> GetIQueryable()
        {
            return from manifest in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Manifests>()
                   select new Models.Manifest
                   {
                       ID = manifest.ID,
                       TrafMode = manifest.TrafMode,
                       CustomsCode = manifest.CustomsCode,
                       CarrierCode = manifest.CarrierCode,
                       TransAgentCode = manifest.TransAgentCode,
                       LoadingDate = manifest.LoadingDate,
                       LoadingLocationCode = manifest.LoadingLocationCode,
                       ArrivalDate = manifest.ArrivalDate,
                       CustomMaster = manifest.CustomMaster,
                       UnitCode = manifest.UnitCode,
                       MsgRepName = manifest.MsgRepName,
                       AdditionalInformation = manifest.AdditionalInformation,
                       CreateTime = manifest.CreateTime
                   };

        }
    }
}
