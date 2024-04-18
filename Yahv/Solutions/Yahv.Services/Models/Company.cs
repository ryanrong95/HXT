using System.Collections;
using System.Collections.Generic;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 内部公司
    /// </summary>
    public class Company : Enterprise
    {
        public Company()
        {

        }

        #region 属性

        /// <summary>
        /// 公司
        /// </summary>
        public CompanyType Type { set; get; }
        /// <summary>
        /// 区域
        /// </summary>
        public AreaType Range { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }

        /// <summary>
        /// 联系人
        /// </summary>
        public IEnumerable<Contact> Contacts { get; set; }

        #endregion

        #region 扩展属性
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
        /// 区域描述
        /// </summary>
        public string RangeDes
        {
            get
            {
                return this.Range.GetDescription();
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
