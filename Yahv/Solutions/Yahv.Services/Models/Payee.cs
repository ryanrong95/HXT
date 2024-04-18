using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>n
    /// 收款人
    /// </summary>
    public class Payee : IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string EnterpriseName { set; get; }
        /// <summary>
        /// 真实ID
        /// </summary>
        public string RealID { set; get; }
        /// <summary>
        /// 真实ID对应的企业名称
        /// </summary>
        public string RealName { set; get; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行账户
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 银行编码 (国际)
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 汇款方式
        /// </summary>
        public Methord Methord { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }


        #endregion
    }
}
