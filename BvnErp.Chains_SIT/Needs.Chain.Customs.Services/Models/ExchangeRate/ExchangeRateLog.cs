using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 币种汇率
    /// </summary>
    public class ExchangeRateLog : IUnique, IPersist
    {
        public string ID { get; set; }

        public string ExchangeRateID { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 后台管理员
        /// </summary>
        public Admin Admin { get; set; }

        public ExchangeRateLog()
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
}