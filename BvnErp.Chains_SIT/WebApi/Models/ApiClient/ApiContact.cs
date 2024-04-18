using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 联系人
    /// </summary>
    public class ApiContact
    {
        /// <summary>
        /// 联系人类型(1 Online线上、2 Offline线下、3 Sales销售、4 Pruchaser 采购)
        /// </summary>
       // public int Type { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系人电话
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
        /// 传真
        /// </summary>
        public string Fax { get; set; }
        /// <summary>
        /// l状态(nomal,abandon)
        /// </summary>
     //   public int Status { get; set; }
        /// <summary>
        ///添加人
        /// </summary>
       // public string CreatorID { get; set; }

    }
}