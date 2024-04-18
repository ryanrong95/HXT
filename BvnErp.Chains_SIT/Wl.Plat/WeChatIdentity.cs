using Layer.Data.Sqls;
using Needs.Utils;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat
{
    public class WeChatIdentity
    {
        /// <summary>
        /// 微信OpenID登录
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        public static IPlatUser OpenIDLogin(string openID)
        {
            if (string.IsNullOrEmpty(openID))
            {
                return null;
            }

            openID = openID.InputText();

            using (ScCustomsReponsitory reponsitory = new ScCustomsReponsitory())
            {
                var query = from user in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>()
                            where user.OpenID == openID && user.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                            select new Models.PlatUser
                            {
                                ID = user.ID,
                                ClientID = user.ClientID,
                                OpenID = user.OpenID,
                                Mobile = user.Mobile,
                                Email = user.Email,
                                UserName = user.Name,
                                RealName = user.RealName,
                                Password = user.Password,
                                IsMain = user.IsMain
                            };

                return query.FirstOrDefault();
            }
        }

        /// <summary>
        /// 微信登录成功后写入OPID
        /// </summary>
        /// <param name="user"></param>
        /// <param name="openID"></param>
        public static void CreateUserOpenID(IPlatUser user, string openID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Users>(new { OpenID = openID.InputText() }, item => item.ID == user.ID);
            }
        }
    }
}