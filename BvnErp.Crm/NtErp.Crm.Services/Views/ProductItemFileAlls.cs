using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    public class ProductItemFileAlls : UniqueView<ProductItemFile, BvCrmReponsitory>
    {
        internal ProductItemFileAlls()
        {

        }

        public ProductItemFileAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ProductItemFile> GetIQueryable()
        {
            return from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItemFiles>()
                   select new ProductItemFile
                   {
                       ID = file.ID,
                       ProductItemID = file.ProductItemID,
                       SubID = file.SubID,
                       Type = (FileType)file.Type,
                       Name = file.Name,
                       Url = file.Url,
                       Status = (Status)file.Status,
                       AdminID = file.ID,
                       CreateDate = DateTime.Now,
                   };
        }
    }
}
