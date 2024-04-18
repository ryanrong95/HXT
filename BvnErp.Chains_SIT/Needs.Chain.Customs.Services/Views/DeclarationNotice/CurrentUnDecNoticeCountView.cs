using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 查询当前各个制单员的未制单数量
    /// </summary>
    public class CurrentUnDecNoticeCountView
    {
        ScCustomsReponsitory _reponsitory { get; set; }

        public CurrentUnDecNoticeCountView()
        {
            this._reponsitory = new ScCustomsReponsitory();
        }

        public CurrentUnDecNoticeCountView(ScCustomsReponsitory reponsitory)
        {
            this._reponsitory = reponsitory;
        }

        public List<CurrentUnDecNoticeCountViewModel> GetCurrentUnDecNoticeCount()
        {
            var declarantCandidates = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarantCandidates>();
            //var orders = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var declarationNotices = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>();
            var adminsTopView2 = new AdminsTopView2(this._reponsitory);

            var declareCreators = (from declarantCandidate in declarantCandidates
                                   join adminsTop in adminsTopView2 on declarantCandidate.AdminID equals adminsTop.OriginID into adminsTopView2_2
                                   from adminsTop in adminsTopView2_2.DefaultIfEmpty()
                                   where declarantCandidate.Status == (int)Enums.Status.Normal
                                      && declarantCandidate.Type == (int)Enums.DeclarantCandidateType.DeclareCreator
                                   select new CurrentUnDecNoticeCountViewModel
                                   {
                                       AdminID = declarantCandidate.AdminID,
                                       AdminName = adminsTop != null ? adminsTop.RealName : "",
                                   }).ToList();

            if (declareCreators == null || !declareCreators.Any())
            {
                return new List<CurrentUnDecNoticeCountViewModel>();
            }

            string[] declareCreatorAdminIDs = declareCreators.Select(t => t.AdminID).ToArray();

            DateTime now = DateTime.Now;
            DateTime tom = DateTime.Now.AddDays(1);

            var theCounts = (from declarationNotice in declarationNotices
                                 //join order in orders on declarationNotice.OrderID equals order.ID
                                 //where declarationNotice.Status == (int)Enums.DeclareNoticeStatus.UnDec
                                 //   && declareCreatorAdminIDs.Contains(declarationNotice.CreateDeclareAdminID)
                                 //   && order.OrderStatus == (int)Enums.OrderStatus.QuoteConfirmed
                                 //   && order.Status == (int)Enums.Status.Normal
                             where declarationNotice.CreateDate >= new DateTime(now.Year, now.Month, now.Day)
                                && declarationNotice.CreateDate < new DateTime(tom.Year, tom.Month, tom.Day)
                                && declareCreatorAdminIDs.Contains(declarationNotice.CreateDeclareAdminID)
                             group declarationNotice by new { declarationNotice.CreateDeclareAdminID, } into g
                             select new CurrentUnDecNoticeCountViewModel
                             {
                                 AdminID = g.Key.CreateDeclareAdminID,
                                 UnDecNoticeCount = g.Count(),
                             }).ToList();

            var results = (from declareCreator in declareCreators
                           join theCount in theCounts on declareCreator.AdminID equals theCount.AdminID into theCounts2
                           from theCount in theCounts2.DefaultIfEmpty()
                           select new CurrentUnDecNoticeCountViewModel
                           {
                               AdminID = declareCreator.AdminID,
                               AdminName = declareCreator.AdminName,
                               UnDecNoticeCount = theCount != null ? theCount.UnDecNoticeCount : 0,
                           }).ToList();

            for (int i = 0; i < results.Count; i++)
            {
                results[i].SerialNo = i;
            }

            return results;
        }
    }

    public class CurrentUnDecNoticeCountViewModel
    {
        public string AdminID { get; set; } = string.Empty;

        public string AdminName { get; set; } = string.Empty;

        public int UnDecNoticeCount { get; set; }

        public int SerialNo { get; set; }
    }

}
