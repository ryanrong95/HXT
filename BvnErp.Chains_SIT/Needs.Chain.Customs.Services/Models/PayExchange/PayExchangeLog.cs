using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PayExchangeLog : IUnique, IPersist
    {
        public string ID { get; set; }

        public string PayExchangeApplyID { get; set; }

        /// <summary>
        /// 后台管理员
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 平台用户
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// 付汇申请状态
        /// </summary>
        public Enums.PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }

        public PayExchangeLog()
        {
            this.CreateDate = DateTime.Now;
        }

        public void Enter()
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
        internal PayExchangeLogs(IEnumerable<PayExchangeLog> enums) : base(enums)
        {
        }

        internal PayExchangeLogs(IEnumerable<PayExchangeLog> enums, Action<PayExchangeLog> action) : base(enums, action)
        {
        }
    }
}