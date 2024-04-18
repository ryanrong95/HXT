using Needs.Wl.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    public class AdminsToken : Erp.Generic.Views.AdminsToken
    {
        internal AdminsToken(string token) : base(token)
        {
        }
    }

    /// <summary>
    /// Yahv框架
    /// </summary>
    public class YahvAdminsToken : YahvAdminAlls
    {
        string Token;

        public YahvAdminsToken(string token)
        {
            this.Token = token;
        }

        protected override IQueryable<Needs.Wl.Admin.Plat.Models.Admin> GetIQueryable()
        {
            var linq = from token in this.Reponsitory.ReadTable<Layer.Data.Sqls.PvbErm.Tokens>()
                       join admin in base.GetIQueryable() on token.OutID equals admin.ID
                       where token.Token == this.Token
                       select new Needs.Wl.Admin.Plat.Models.Admin
                       {
                           ID = admin.OriginID, //admin.ID,
                           RealName = admin.RealName,
                           UserName = admin.UserName,
                           ErmAdminID = admin.ID
                       };

            return linq;
        }
    }
}
