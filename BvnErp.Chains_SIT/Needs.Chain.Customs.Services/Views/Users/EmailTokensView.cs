using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class EmailTokensView : UniqueView<Models.EmailToken, ScCustomsReponsitory>
    {
        public EmailTokensView()
        {

        }

        protected override IQueryable<EmailToken> GetIQueryable()
        {
            return from token in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EmailTokens>()
                   select new EmailToken
                   {
                       ID = token.ID,
                       Email = token.Email,
                       Token = token.Token,
                       CreateDate = token.CreateDate,
                   };
        }
    }
}
