using Needs.Cbs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Models.Origins
{
    /// <summary>
    /// 币种汇率变更日志
    /// </summary>
    public class ExchangeRateLog : IUnique, IPersist
    {
        #region 属性

        public string ID { get; set; }

        public string ExchangeRateID { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        public ExchangeRateLog()
        {
            this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.ID = ChainsGuid.NewGuidUp();
                reponsitory.Insert(new Layer.Data.Sqls.Customs.ExchangeRateLogs
                {
                    ID = this.ID,
                    ExchangeRateID = this.ExchangeRateID,
                    Rate = this.Rate,
                    CreateDate = this.CreateDate,
                    Summary = this.Summary
                });
            }
        }
    }
}