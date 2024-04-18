using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    /// <summary>
    /// 会员信息
    /// </summary>
    public class User : Yahv.Linq.IUnique
    {
        public User()
        {
        }

        #region 自定义属性
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; internal set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; internal set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; internal set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; internal set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; internal set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; internal set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; internal set; }

        /// <summary>
        /// QQ账号
        /// </summary>
        public string QQ { get; internal set; }

        /// <summary>
        /// 微信
        /// </summary>
        public string Wx { get; internal set; }

        /// <summary>
        ///是否为主账号
        /// </summary>
        public bool IsMain { get; internal set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; internal set; }

        /// <summary>
        /// 停启用状态
        /// </summary>
        public Enums.UserStatus Status { get; internal set; }
        #endregion
    }
}
