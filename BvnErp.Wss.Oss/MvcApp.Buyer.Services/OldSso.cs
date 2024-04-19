using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services
{
    /// <summary>
    /// 用户令牌状态 字段说明：100 未使用 200 已使用
    /// </summary>
    public enum UserTokenStatus
    {
        /// <summary>
        /// 未使用
        /// </summary>
        NonUse = 100,
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 200,
    }

    /// <summary>
    /// 用户令牌类型 1 激活  2 找回密码 3 用户登入
    /// </summary>
    public enum UserTokenType
    {
        /// <summary>
        /// 激活 (基本)
        /// </summary>
        Activing = 100,
        /// <summary>
        /// 找回密码
        /// </summary>
        Password = 200,
        /// <summary>
        /// 用户登入
        /// </summary>
        UserLogin = 300,
        /// <summary>
        /// 邮箱确认
        /// </summary>
        EmailActiving = 110
    }

    /// <summary>
    /// 临时使用
    /// </summary>

    [Obsolete("临时使用")]
    public class OldSso
    {
        internal const string CookieName = "ydxcyht_new_big_sso";
        const string session_name = "tj85j94jgj";
        public const string session_cartid = session_name + "_cartid_new";

        static public Models.ClientTop Current
        {
            get
            {
                return ByToken(Needs.Utils.Http.Cookies.Current[CookieName]);
            }
        }

        /// <summary>
        /// 获取 StartUser 对象
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>StartUser 对象</returns>
        static Models.ClientTop ByToken(string token, UserTokenType type = UserTokenType.UserLogin)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }
            using (BvSsoReponsitory repository = new BvSsoReponsitory())
            {
                var first = repository.GetTable<Layer.Data.Sqls.BvSso.UserTokens>()
                    .Where(item => item.Token == token && item.Type == (int)type).OrderByDescending(item => item.CreateDate)
                    .Select(item => item.UserID).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(first))
                {
                    return null;
                }

                var linq = from top in repository.ReadTable<Layer.Data.Sqls.BvSso.Users>()
                           select new Models.ClientTop
                           {
                               ID = top.ID,
                               UserName = top.UserName,
                               Email = top.Email,
                               Mobile = top.Mobile,
                           };

                return linq.SingleOrDefault(item => item.ID == first);
            }
        }
    }
}
