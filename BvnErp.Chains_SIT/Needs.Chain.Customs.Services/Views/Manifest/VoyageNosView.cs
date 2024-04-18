using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 获取有效航次号列表-报关单录单用
    /// </summary>
    public class VoyageNosView : UniqueView<Models.Voyage, ScCustomsReponsitory>
    {
        public VoyageNosView()
        {
        }

        internal VoyageNosView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Voyage> GetIQueryable()
        {
            var carriersView = new CarriersView(this.Reponsitory);
            var orderVoyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>();

            return from voyage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>()
                   join carrier in carriersView on voyage.CarrierCode equals carrier.Code into carriers
                   from carrier in carriers.DefaultIfEmpty()
                   where (carrier.CarrierType == Enums.CarrierType.InteLogistics || carrier == null)
                   && voyage.Status == (int)Enums.Status.Normal
                   && voyage.CutStatus == (int)Enums.CutStatus.UnCutting
                   //&& !orderVoyagesView.Any(s => s.VoyageID == voyage.ID)
                   select new Models.Voyage
                   {
                       ID = voyage.ID,
                      
                   };          
        }
    }
}
