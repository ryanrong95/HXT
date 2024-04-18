using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Web.Views
{
    class YahvAdminsToken : YahvAdminsView
    {
        string Token;

        public YahvAdminsToken(string token)
        {
            this.Token = token;
        }

        protected override IQueryable<Models.Admin> GetIQueryable()
        {
            return from token in this.Reponsitory.ReadTable<Layer.Data.Sqls.PvbErm.Tokens>()
                   join admin in base.GetIQueryable() on token.OutID equals admin.ID
                   where token.Token == this.Token
                   select admin;
        }
    }
}
