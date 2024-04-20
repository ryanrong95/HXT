using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Services.Views
{
    public class BomsView : Needs.Linq.UniqueView<Models.Boms, Layer.Data.Sqls.BomsReponsitory>
    {
        public BomsView()
        {

        }
        protected override IQueryable<Models.Boms> GetIQueryable()
        {
          

            return from bom in this.Reponsitory.ReadTable<Layer.Data.Sqls.Boms.Boms>()
                       //join client in ClientTopView on entity.Boms.ClientID equals client.ID
                       orderby bom.CreateDate descending
                   select new Models.Boms
                   {
                       ID = bom.ID,
                       CreateDate = bom.CreateDate,
                       ClientID = bom.ClientID,
                       Uri = bom.Uri,
                       Contact = bom.Contact,
                       Email = bom.Email,
                   };
        }
    }
}