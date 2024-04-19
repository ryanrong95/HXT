using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.View
{
    public class ConsigneesView : UniqueView<Models.Consignee, BvSsoReponsitory>
    {
        public ConsigneesView()
        {

        }
        internal ConsigneesView(BvSsoReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Consignee> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.BvSso.Consignees>()
                       select new Models.Consignee
                       {
                           ID = entity.ID,
                           UserID = entity.UserID,
                           Country = entity.Country,
                           FirstName = entity.FirstName,
                           LastName = entity.LastName,
                           Contact = entity.Contact,
                           Company = entity.Company,
                           Tel = entity.Tel,
                           Email = entity.Email,
                           Zipcode = entity.Zipcode,
                           Address = entity.Address,
                       };

            return linq;
        }

    }

}
