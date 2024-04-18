using Layer.Data.Sqls;
using Layer.Data.Sqls.BvSso;
using  NtErp.Wss.Sales.Services.Models.SsoUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Web.Models
{
    public class User
    {
        public string ID { get; set; }
        public string UserName { get; set; }
    }

    public static class SsoLogon
    {

        internal const string CookieName = "ydxcyht_new_big_sso";
        static public User ByToken(string token, UserTokenType type)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }
            using (BvSsoReponsitory repository = new BvSsoReponsitory())
            {
                var first = repository.GetTable<UserTokens>()
                    .Where(item => item.Token == token && item.Type == (int)type && item.Status == (int)UserTokenStatus.Normal).OrderByDescending(item => item.CreateDate)
                    .Select(item => item.UserID).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(first))
                {
                    return null;
                }

                var user = repository.GetTable<Users>().SingleOrDefault(item => item.ID == first);
                   

                return new User {ID=user.ID,UserName=user.UserName };
                //return new SsoUsersDomain().SingleOrDefault(item => item.ID == first) as SsoUser;
            }
        }

        internal static User ByToken(object p, object userLogin)
        {
            throw new NotImplementedException();
        }
    }
        
}
