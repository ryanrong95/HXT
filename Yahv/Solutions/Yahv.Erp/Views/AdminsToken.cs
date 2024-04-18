
using Yahv.Models;
using System.Linq;

namespace Yahv.Views
{
    class AdminsToken : AdminsAlls
    {
        string token;
        protected internal AdminsToken(string token)
        {
            this.token = token;
        }

        protected override IQueryable<ErpAdmin> GetIQueryable()
        {
            return from token in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Tokens>()
                   join admin in base.GetIQueryable() on token.OutID equals admin.ID
                   where token.Token == this.token
                   select admin;
        }
    }
}
