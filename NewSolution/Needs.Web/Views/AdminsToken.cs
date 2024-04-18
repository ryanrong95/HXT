using Needs.Web.Models;
using System;
using System.Linq;

namespace Needs.Web.Views
{
    [Obsolete("这里要重新开发")]
    class AdminsToken : AdminsView
    {
        string token;
        internal AdminsToken(string token)
        {
            this.token = token;
        }

        protected override IQueryable<Admin> GetIQueryable()
        {
            return from token in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvTester.Tokens>()
                   join admin in base.GetIQueryable() on token.OutID equals admin.ID
                   where token.Token == this.token
                   select admin;
        }
    }
}
