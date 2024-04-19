using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Views
{
    public class VendersView : Needs.Linq.UniqueView<Models.Vender, Layer.Data.Sqls.BvnVrsReponsitory> 
    {
        protected override IQueryable<Models.Vender> GetIQueryable()
        {
            CompaniesView cpmpanyview = new CompaniesView(this.Reponsitory);
            return from entity in this.Reponsitory.GetTable<Layer.Data.Sqls.BvnVrs.Companies>()
                   join company in cpmpanyview on entity.ID equals company.ID
                   //where entity.Status == (int)Enums.Status.Nomal
                   select new Models.Vender
                   {
                       ID = company.ID,
                       Name = company.Name,
                       RegisteredCapital = company.RegisteredCapital,
                       CorporateRepresentative = company.CorporateRepresentative,
                       Type = (Enums.ComapnyType)entity.Type,
                       //Grade = (Enums.Grade)entity.Grade,
                       //Status = (Enums.Status)entity.Status,
                       //Properties = entity.Properties,
                       Address = company.Address,
                   };
        }        
    }  
}
