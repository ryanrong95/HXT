using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class SiteUser : Linq.IUnique
    {
        #region 属性
        public string ID { set; get; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { set; get; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        //public string From { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { set; get; }
        /// <summary>
        /// 微信
        /// </summary>
        public string Wx { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string EnterpriseName { set; get; }
        /// <summary>
        /// 是否主账号
        /// </summary>
        public bool IsMain { set; get; }
        /// <summary>
        /// 账号状态
        /// </summary>
        public ApprovalStatus Status { set; get; }

        #endregion
    }
}
