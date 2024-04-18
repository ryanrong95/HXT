using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecNoticeVoyagesView : UniqueView<Models.DecNoticeVoyage, ScCustomsReponsitory>
    {
        public DecNoticeVoyagesView()
        {
        }

        internal DecNoticeVoyagesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecNoticeVoyage> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var declarationNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>();
            var voyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();

            return from decNoticeVoyage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>()
                   join declarationNotice in declarationNotices on decNoticeVoyage.DecNoticeID equals declarationNotice.ID
                   join voyage in voyages on decNoticeVoyage.VoyageID equals voyage.ID into voyages2
                   from voyage in voyages2.DefaultIfEmpty()
                   join admin in adminsView on decNoticeVoyage.AdminID equals admin.ID
                   where decNoticeVoyage.Status == (int)Enums.Status.Normal
                   select new Models.DecNoticeVoyage
                   {
                       ID = decNoticeVoyage.ID,
                       DeclarationNotice = new Models.DeclarationNotice { ID = declarationNotice.ID },
                       Voyage = new Models.Voyage { ID = voyage.ID, Type = (Enums.VoyageType)voyage.Type },
                       Admin = admin,
                       Status = (Enums.Status)decNoticeVoyage.Status,
                       CreateDate = decNoticeVoyage.CreateDate,
                       UpdateDate = decNoticeVoyage.UpdateDate,
                       Summary = decNoticeVoyage.Summary,
                   };
        }
    }
}
