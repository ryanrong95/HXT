using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Payments.Models.Origins
{
    /// <summary>
    /// 通知清单
    /// </summary>
    public class VoucherRecord : IUnique
    {
        #region 属性
        public string ID { get; internal set; }

        /// <summary>
        /// 财务通知ID
        /// </summary>
        public string VoucherID { get; set; }

        /// <summary>
        /// 收款银行（应与收益人相同）
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 银行、现金收付款手续的流水号
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {

        }
        #endregion
    }
}
