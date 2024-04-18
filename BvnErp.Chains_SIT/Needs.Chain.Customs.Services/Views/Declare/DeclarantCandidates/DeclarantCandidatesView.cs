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
    /// <summary>
    /// 候选报关员（制单人、发单人、核对人）视图
    /// </summary>
    public class DeclarantCandidatesView : UniqueView<Models.DeclarantCandidate, ScCustomsReponsitory>
    {
        protected override IQueryable<DeclarantCandidate> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);

            return from declarantCandidate in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarantCandidates>()
                   join admin in adminView on declarantCandidate.AdminID equals admin.ID into adminView2
                   from admin in adminView2.DefaultIfEmpty()
                   where declarantCandidate.Status == (int)Enums.Status.Normal
                   select new Models.DeclarantCandidate
                   {
                       ID = declarantCandidate.ID,
                       AdminID = declarantCandidate.AdminID,
                       Type = (Enums.DeclarantCandidateType)declarantCandidate.Type,
                       Status = (Enums.Status)declarantCandidate.Status,
                       CreateTime = declarantCandidate.CreateTime,
                       UpdateTime = declarantCandidate.UpdateTime,
                       Summary = declarantCandidate.Summary,
                       AdminName = admin != null ? admin.RealName : "",
                   };
        }
    }
}
