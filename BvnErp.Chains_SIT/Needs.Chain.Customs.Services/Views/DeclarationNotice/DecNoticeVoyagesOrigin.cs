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
    public class DecNoticeVoyagesOrigin : UniqueView<DecNoticeVoyage, ScCustomsReponsitory>
    {
        protected override IQueryable<DecNoticeVoyage> GetIQueryable()
        {        
            return from decNoticeVoyage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>()                 
                   where decNoticeVoyage.Status == (int)Enums.Status.Normal
                   select new Models.DecNoticeVoyage
                   {
                       ID = decNoticeVoyage.ID,
                       DeclarationNotice = new Models.DeclarationNotice { ID = decNoticeVoyage.DecNoticeID },
                       Voyage = new Models.Voyage { ID = decNoticeVoyage.VoyageID },
                       Admin = new Admin { ID= decNoticeVoyage.AdminID},
                       Status = (Enums.Status)decNoticeVoyage.Status,
                       CreateDate = decNoticeVoyage.CreateDate,
                       UpdateDate = decNoticeVoyage.UpdateDate,
                       Summary = decNoticeVoyage.Summary,
                   };
        }
    }
}
