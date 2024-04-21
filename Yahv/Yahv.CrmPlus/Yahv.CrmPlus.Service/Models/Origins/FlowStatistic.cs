using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace YaHv.CrmPlus.Services.Models.Origins
{


    public class CreditStatistic
    {
        public CreditType CreditType { set; get; }
        /// <summary>
        /// 授信方
        /// </summary>
        public Enterprise Maker { set; get; }

        /// <summary>
        /// 接收方
        /// </summary>
        public Enterprise Taker { set; get; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { set; get; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal Total { set; get; }
        /// <summary>
        /// 花费
        /// </summary>
        public decimal Cost { set; get; }
        ///// <summary>
        ///// 流水
        ///// </summary>
        //public IEnumerable<FlowCredit> FlowCredit { set; get; }

    }
}
