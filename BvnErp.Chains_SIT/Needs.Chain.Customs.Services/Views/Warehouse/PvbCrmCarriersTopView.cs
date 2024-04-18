using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PvbCrmCarriersTopView : UniqueView<Models.PvbCrmCarrierViewModel, ScCustomsReponsitory>
    {
        public PvbCrmCarriersTopView()
        {

        }

        public PvbCrmCarriersTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PvbCrmCarrierViewModel> GetIQueryable()
        {
            var result = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvbCrmCarriersTopView>()
                         select new Models.PvbCrmCarrierViewModel
                         {
                            ID = c.ID,
                            Name = c.Name,
                            Code = c.Code,
                            Place = c.Place,
                            Type = (Models.PvbCarrierType)c.Type,
                            IsInternational = c.IsInternational,
                            CreateDate = c.CreateDate,
                            UpdateDate = c.UpdateDate
                         };

            return result;
        }
    }
}
