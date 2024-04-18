using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    public class PayExchangeApplyLogsView : UniqueView<PayExchangeLog, ScCustomReponsitory>
    {
        private PayExchangeApplyLogsView()
        {

        }

        public PayExchangeApplyLogsView(ScCustomReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<PayExchangeLog> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PayExchangeApplyLogs>()
                   orderby entity.CreateDate descending
                   select new PayExchangeLog
                   {
                       ID = entity.ID,
                       PayExchangeApplyID = entity.PayExchangeApplyID,
                       PayExchangeApplyStatus = (PayExchangeApplyStatus)entity.PayExchangeApplyStatus,
                       AdminID = entity.AdminID,
                       UserID = entity.UserID,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate
                   };
        }
    }

    public class PayExchangeLog : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 付汇申请ID
        /// </summary>
        public string PayExchangeApplyID { get; set; }

        /// <summary>
        /// 后台管理员
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 付汇申请状态
        /// </summary>
        public PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
    }
}
