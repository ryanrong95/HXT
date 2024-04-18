using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Needs.Wl.Models.Views
{
    public class UserTokensView : View<Models.UserToken, ScCustomsReponsitory>
    {
        public UserTokensView()
        {
        }

        internal UserTokensView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.UserToken> GetIQueryable()
        {
            return from user in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UserTokens>()
                   select new Models.UserToken
                   {
                       ID = user.ID,
                       UserID = user.UserID,
                       IP = user.IP,
                       Token = user.Token,
                       CreateDate = user.CreateDate.Value
                   };
        }
    }
}