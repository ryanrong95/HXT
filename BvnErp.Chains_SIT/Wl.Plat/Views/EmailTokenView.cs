using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat.Views
{
    public class EmailTokenView : View<Needs.Wl.Models.EmailToken, ScCustomsReponsitory>
    {
        private string Token;

        public EmailTokenView(string token)
        {
            this.Token = token;
        }

        protected override IQueryable<Needs.Wl.Models.EmailToken> GetIQueryable()
        {
            return from token in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EmailTokens>()
                   where token.Token == this.Token
                   select new Needs.Wl.Models.EmailToken
                   {
                       ID = token.ID,
                       Email = token.Email,
                       Token = token.Token,
                       CreateDate = token.CreateDate
                   };
        }
    }
}
