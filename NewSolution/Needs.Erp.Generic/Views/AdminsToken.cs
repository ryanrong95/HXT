using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using System.Linq;

namespace Needs.Erp.Generic.Views
{
    public class AdminsToken : AdminsAlls
    {
        string token;
        protected internal AdminsToken(string token)
        {
            this.token = token;
        }

        protected override IQueryable<Models.Admin> GetIQueryable()
        {
            return from token in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.Tokens>()
                   join admin in base.GetIQueryable() on token.OutID equals admin.ID
                   where token.Token == this.token
                   select admin;
        }
    }
}
