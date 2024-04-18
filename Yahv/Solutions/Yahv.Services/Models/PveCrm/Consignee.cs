using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.Services.Models.PveCrm
{
    public class Consignee : IUnique
    {
        public Consignee()
        {

        }

        #region 属性
        /// <summary>
        /// 唯一标识ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        ///名称，子库房名称
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// DYJCode
        /// </summary>
        public string DyjCode { get; set; }
        /// <summary>
        /// 地区，，以Place为准
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        /// 联系人类型
        /// </summary>
        //public ContactType Type { get; set; }
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
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public Underly.AuditStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string AdminID { get; set; }
        /// <summary>
        /// 国家/地区，以Place为准
        /// </summary>
        public string Place { set; get; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 企业
        /// </summary>
        public Enterprise Enterprise { get; set; }
        /// <summary>
        /// 地区描述
        /// </summary>
        public string DistrictDesc { get; set; }
        #endregion
    }
}
