using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat.Models
{
    /// <summary>
    /// 会员信息
    /// </summary>
    public partial interface IPlatUser : ILocalUser, Needs.Linq.IUnique
    {
        event LoginSuccessHanlder LoginSuccess;
        event LoginFailedHanlder LoginFailed;

        /// <summary>
        /// 用户名/邮箱/手机
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// 用户的真实姓名
        /// </summary>
        string RealName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        string Mobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        string ClientID { get; set; }

        /// <summary>
        /// 是否主账号
        /// </summary>
        bool IsMain { get; set; }

        bool IsValid { get; set; }

        /// <summary>
        /// 客户信息(Needs.Wl.Models)
        /// </summary>
        Needs.Wl.Models.Client Client { get; set; }

        void Login();

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPassWord"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        bool ChangePassword(string oldPassWord, string newPassword);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="password"></param>
        void ResetPassword(string password);

        /// <summary>
        /// 修改手机绑定
        /// </summary>
        /// <param name="newMobile">新手机号码</param>
        bool ChangeMobile(string newMobile);

        /// <summary>
        /// 修改邮箱绑定
        /// </summary>
        /// <param name="newEmail"></param>
        /// <returns></returns>
        bool ChangeEmail(string newEmail);

        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <param name="newUserName"></param>
        /// <returns></returns>
        bool ResetUserName(string newUserName);

        /// <summary>
        /// 登出
        /// </summary>
        void Logout();
    }
}