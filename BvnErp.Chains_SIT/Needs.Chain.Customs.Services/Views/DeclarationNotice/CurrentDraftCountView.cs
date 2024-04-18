using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 查询当前各个发单员的草稿数量
    /// </summary>
    public class CurrentDraftCountView
    {
        ScCustomsReponsitory _reponsitory { get; set; }

        public CurrentDraftCountView()
        {
            this._reponsitory = new ScCustomsReponsitory();
        }

        public CurrentDraftCountView(ScCustomsReponsitory reponsitory)
        {
            this._reponsitory = reponsitory;
        }

        public List<CurrentDraftCountViewModel> GetCurrentDraftCount()
        {
            var declarantCandidates = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarantCandidates>();
            var adminsTopView2 = new AdminsTopView2(this._reponsitory);
            var decHeads = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orders = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var customSubmiters = (from declarantCandidate in declarantCandidates
                                   join adminsTop in adminsTopView2 on declarantCandidate.AdminID equals adminsTop.OriginID into adminsTopView2_2
                                   from adminsTop in adminsTopView2_2.DefaultIfEmpty()
                                   where declarantCandidate.Status == (int)Enums.Status.Normal
                                      && declarantCandidate.Type == (int)Enums.DeclarantCandidateType.CustomSubmiter
                                   select new CurrentDraftCountViewModel
                                   {
                                       AdminID = declarantCandidate.AdminID,
                                       AdminName = adminsTop != null ? adminsTop.RealName : "",
                                   }).ToList();

            if (customSubmiters == null || !customSubmiters.Any())
            {
                return new List<CurrentDraftCountViewModel>();
            }

            string[] customSubmiterAdminIDs = customSubmiters.Select(t => t.AdminID).ToArray();

            var theCounts = (from decHead in decHeads
                             join order in orders on decHead.OrderID equals order.ID
                             where decHead.IEDate == DateTime.Now.ToString("yyyyMMdd")   //decHead.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Draft)
                                && order.Status == (int)Enums.Status.Normal
                             group decHead by new { decHead.SubmitCustomAdminID, } into g
                             select new CurrentDraftCountViewModel
                             {
                                 AdminID = g.Key.SubmitCustomAdminID,
                                 DraftCount = g.Count(),
                             }).ToList();

            var results = (from customSubmiter in customSubmiters
                           join theCount in theCounts on customSubmiter.AdminID equals theCount.AdminID into theCounts2
                           from theCount in theCounts2.DefaultIfEmpty()
                           select new CurrentDraftCountViewModel
                           {
                               AdminID = customSubmiter.AdminID,
                               AdminName = customSubmiter.AdminName,
                               DraftCount = theCount != null ? theCount.DraftCount : 0,
                           }).ToList();

            for (int i = 0; i < results.Count; i++)
            {
                results[i].SerialNo = i;
            }

            return results;
        }
    }

    public class CurrentDraftCountViewModel
    {
        public string AdminID { get; set; } = string.Empty;

        public string AdminName { get; set; } = string.Empty;

        public int DraftCount { get; set; }

        public int SerialNo { get; set; }
    }

}
