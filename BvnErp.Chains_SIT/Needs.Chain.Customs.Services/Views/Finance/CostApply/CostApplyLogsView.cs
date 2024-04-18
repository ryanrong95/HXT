using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class CostApplyLogsView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public CostApplyLogsView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public CostApplyLogsView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public List<CostApplyLogsViewModel> GetResults(string costApplyID)
        {
            var costApplyLogs = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.CostApplyLogs>();
            var admins = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();

            var results = from costApplyLog in costApplyLogs
                          join admin in admins on costApplyLog.AdminID equals admin.OriginID into admins2
                          from admin in admins2.DefaultIfEmpty()
                          where costApplyLog.CostApplyID == costApplyID
                          orderby costApplyLog.CreateDate ascending
                          select new CostApplyLogsViewModel
                          {
                              CostApplyLogID = costApplyLog.ID,
                              CostApplyID = costApplyLog.CostApplyID,
                              CreateDate = costApplyLog.CreateDate,
                              Summary = costApplyLog.Summary,
                              AdminName = admin.RealName,
                              Reason = costApplyLog.Reason,
                              CurrentCostStatus = (Enums.CostStatusEnum)costApplyLog.CurrentCostStatus,
                              NextCostStatus = (Enums.CostStatusEnum)costApplyLog.NextCostStatus,
                          };

            return results.ToList();
        }

    }

    public class CostApplyLogsViewModel
    {
        /// <summary>
        /// CostApplyLogID
        /// </summary>
        public string CostApplyLogID { get; set; } = string.Empty;

        /// <summary>
        /// CostApplyID
        /// </summary>
        public string CostApplyID { get; set; } = string.Empty;

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string AdminName { get; set; } = string.Empty;

        /// <summary>
        /// 理由
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// 当前费用申请状态
        /// </summary>
        public Enums.CostStatusEnum CurrentCostStatus { get; set; }

        /// <summary>
        /// 下一个费用申请状态
        /// </summary>
        public Enums.CostStatusEnum NextCostStatus { get; set; }
    }
}
