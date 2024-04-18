using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views.Origins;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Rolls.Supplier
{
    public  class SupplierRoll : UniqueView<Models.SupplierModel, ScCustomsReponsitory>
    {

        public  SupplierRoll()
        {

        }

        public  SupplierRoll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }



        protected override IQueryable<Models.SupplierModel> GetIQueryable()
        {
            // var ids = new ClientsRoll().Where(x => x.Status == Enums.Status.Normal).Select(x=>x.ID).ToArray();
            var ClientView = new ClientsRoll(this.Reponsitory).Where(x => x.Status == Enums.Status.Normal);
            var suplierView = new ClientSuppliersOrigin(this.Reponsitory).Where(x => x.Status == Enums.Status.Normal);
            return from entity in suplierView
                   join client in ClientView on entity.ClientID equals client.ID
                   select new Models.SupplierModel
                   {
                       ID=entity.ID,
                       Name=entity.Name,
                       ChineseName=entity.ChineseName,
                       Place=entity.Place,
                       SupplierGrade= entity.SupplierGrade,
                       ClientID=entity.ClientID,
                       ClientName=client.Company.Name,
                       ClientCode=client.ClientCode,
                       CreateDate=entity.CreateDate,
                       Summary = entity.Summary
                   };

        }
    }
}
