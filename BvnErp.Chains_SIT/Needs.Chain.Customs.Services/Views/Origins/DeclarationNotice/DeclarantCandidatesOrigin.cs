using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class DeclarantCandidatesOrigin : UniqueView<Models.DeclarantCandidate, ScCustomsReponsitory>
    {
        public DeclarantCandidatesOrigin()
        {
        }

        public DeclarantCandidatesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DeclarantCandidate> GetIQueryable()
        {
            return from declarantCandidate in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarantCandidates>()
                   select new Models.DeclarantCandidate
                   {
                       ID = declarantCandidate.ID,
                       AdminID = declarantCandidate.AdminID,
                       Type = (Enums.DeclarantCandidateType)declarantCandidate.Type,
                       Status = (Enums.Status)declarantCandidate.Status,
                       CreateTime = declarantCandidate.CreateTime,
                       UpdateTime = declarantCandidate.UpdateTime,
                       Summary = declarantCandidate.Summary,
                   };
        }
    }
}
