using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Linq;

namespace Wms.Services.Views
{
    public class WayPartersView : UniqueView<WayParters, PvWmsRepository>
    {
        protected override IQueryable<WayParters> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.WaybillsTopView>()
                   select new WayParters
                   {
                       ID = entity.wbID,
                       Company = entity.corCompany,
                       Place = entity.corPlace,
                       Address = entity.corPlace,
                       Contact = entity.corContact,
                       Phone = entity.corPhone,
                       Zipcode = entity.corZipcode,
                       Email = entity.corEmail,
                       CreateDate = entity.corCreateDate
                   };
        }
    }
}
