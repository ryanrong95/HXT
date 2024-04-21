using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 应付
    /// </summary>
    public class Payable
    {
        #region 属性
        /// <summary>
        /// id  
        /// </summary>
        public string ID { get; internal set; }

        /// <summary>
        /// 收款人 客户(公司)(clientid)
        /// </summary>
        public string Payee { get; set; }
        /// <summary>
        /// 收款人账户
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 付款人
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 付款人账户
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string Business { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// 科目类型
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 币种 发生币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额 发生金额
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 所属订单id (mainid)
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 创建时间 消费时间
        /// </summary>
        public DateTime CreateDate { get; internal set; }

        /// <summary>
        /// 添加人  npc，实际的人员 
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summay { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 匿名付款人
        /// </summary>
        public string PayerAnonymous { get; set; }

        /// <summary>
        /// 匿名收款人
        /// </summary>
        public string PayeeAnonymous { get; set; }

        /// <summary>
        /// 账单ID
        /// </summary>
        public string VoucherID { get; set; }
        #endregion
    }
}
