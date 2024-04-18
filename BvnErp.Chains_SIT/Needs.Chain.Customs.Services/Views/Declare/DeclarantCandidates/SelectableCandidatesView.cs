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
    public class SelectableCandidatesView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public SelectableCandidatesView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public SelectableCandidatesView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public List<DeclarantCandidate> GetUseCandidates(Enums.DeclarantCandidateType type)
        {
            var declarantCandidates = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarantCandidates>();
            var adminView = new AdminsTopView(this.Reponsitory);

            var candidates = from declarantCandidate in declarantCandidates
                             join admin in adminView on declarantCandidate.AdminID equals admin.ID into adminView2
                             from admin in adminView2.DefaultIfEmpty()
                             where declarantCandidate.Status == (int)Enums.Status.Normal
                                && declarantCandidate.Type == (int)type
                             select new DeclarantCandidate
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

            candidates = candidates.OrderBy(t => t.AdminName);

            return candidates.ToList();
        }

        public List<DeclarantCandidate> GetSelectableCandidates(Enums.DeclarantCandidateType type)
        {
            var declarantCandidates = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarantCandidates>();
            var adminView = new AdminsTopView(this.Reponsitory);

            string[] existAdminIDs = declarantCandidates.Where(t => t.Type == (int)type && t.Status == (int)Enums.Status.Normal).Select(t => t.AdminID).ToArray();

            var candidates = from declarantCandidate in declarantCandidates
                             join admin in adminView on declarantCandidate.AdminID equals admin.ID into adminView2
                             from admin in adminView2.DefaultIfEmpty()
                             where declarantCandidate.Status == (int)Enums.Status.Normal
                                && declarantCandidate.Type == (int)Enums.DeclarantCandidateType.Optional
                             select new DeclarantCandidate
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

            if (existAdminIDs != null && existAdminIDs.Any())
            {
                candidates = candidates.Where(t => !existAdminIDs.Contains(t.AdminID));
            }

            candidates = candidates.OrderBy(t => t.AdminName);

            return candidates.ToList();
        }
    }
}
