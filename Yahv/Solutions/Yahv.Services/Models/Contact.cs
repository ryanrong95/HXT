using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 联系人
    /// </summary>
    public class Contact : Linq.IUnique
    {
        public Contact()
        {

        }
        #region 属性
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 公司唯一标识号
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 联系人类型
        /// </summary>
        public ContactType Type { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 企业
        /// </summary>
        public Enterprise Enterprise { get; set; }
        /// <summary>
        /// 类型描述
        /// </summary>
        public string TypeDes
        {
            get
            {
                return this.Type.GetDescription();
            }
        }
        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatusDes
        {
            get
            {
                return this.Status.GetDescription();
            }
        }

        #endregion
    }
}
