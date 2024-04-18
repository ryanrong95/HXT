using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Collections.Generic;

namespace Needs.Wl.Models
{
    public class PayExchangeLog : ModelBase<Layer.Data.Sqls.ScCustoms.PayExchangeApplyLogs, ScCustomsReponsitory>, IUnique, IPersist
    {
        public string PayExchangeApplyID { get; set; }

        /// <summary>
        /// 后台管理员
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 平台用户
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 付汇申请状态
        /// </summary>
        public Enums.PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }

        public PayExchangeLog()
        {
            this.CreateDate = DateTime.Now;
        }

        public override void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyLog);
                reponsitory.Insert(this.ToLinq());
            }
        }
    }

    public class PayExchangeLogs : BaseItems<PayExchangeLog>
    {
        public PayExchangeLogs(IEnumerable<PayExchangeLog> enums) : base(enums)
        {
        }

        internal PayExchangeLogs(IEnumerable<PayExchangeLog> enums, Action<PayExchangeLog> action) : base(enums, action)
        {
        }
    }
}