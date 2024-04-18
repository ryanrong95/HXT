using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services.Models
{
    public class Consignor : Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 供应商企业ID
        /// </summary>
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 供应商企业
        /// </summary>
        public Enterprise Enterprise { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { set; get; }
        /// <summary>
        /// 名称，主要用于库房的子库房名称
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 大赢家Code
        /// </summary>
        public string DyjCode { set; get; }
        /// <summary>
        /// 省
        /// </summary>
        //public string Province { set; get; }
        /// <summary>
        /// 市
        /// </summary>
       // public string City { set; get; }
        /// <summary>
        /// 地
        /// </summary>
       // public string Land { set; get; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        /// 联系人类型
        /// </summary>
       // public ContactType Type { get; set; }
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
        public Underly.ApprovalStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>

        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }

        public string AdminID { get; set; }
        #endregion
    }
}
