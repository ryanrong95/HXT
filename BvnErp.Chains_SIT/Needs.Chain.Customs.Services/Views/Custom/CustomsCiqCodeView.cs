using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class CustomsCiqCodeView : UniqueView<Models.CustomsCiqCode, ScCustomsReponsitory>
    {
        public CustomsCiqCodeView()
        {
        }

        internal CustomsCiqCodeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<CustomsCiqCode> GetIQueryable()
        {
            return from ciq in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsCiqCodes>()
                   where ciq.Status == (int)Enums.Status.Normal
                   select new Models.CustomsCiqCode
                   {
                       ID = ciq.ID,
                       Name = ciq.Name,
                       Category = ciq.Category,
                       InspectionCode = ciq.InspectionCode,
                       Status = (Enums.Status)ciq.Status,
                       CreateDate = ciq.CreateDate,
                       UpdateDate = ciq.UpdateDate,
                       Summary = ciq.Summary
                   };
        }
    }
}
