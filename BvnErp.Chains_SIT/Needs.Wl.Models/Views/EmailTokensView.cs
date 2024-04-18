using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class EmailTokensView : View<Needs.Wl.Models.EmailToken, ScCustomsReponsitory>
    {
        public EmailTokensView()
        {

        }

        protected override IQueryable<EmailToken> GetIQueryable()
        {
            return from token in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EmailTokens>()
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
