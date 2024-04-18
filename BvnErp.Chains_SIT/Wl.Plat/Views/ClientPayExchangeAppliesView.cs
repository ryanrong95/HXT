using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Wl.User.Plat.Models;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 客户的付汇申请
    /// TODO:已经作废，请使用：MyPayExchangeAppliesView
    /// </summary>
    public class ClientPayExchangeAppliesView : Needs.Ccs.Services.Views.UserPayExchangeAppliesView
    {
        IPlatUser User;

        internal ClientPayExchangeAppliesView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.UserPayExchangeApply> GetIQueryable()
        {
            if (this.User.IsMain)
            {
                return from entity in base.GetIQueryable()
                       where entity.ClientID == this.User.Client.ID && entity.Status == Needs.Ccs.Services.Enums.Status.Normal
                       select entity;
            }
            else
            {
                return from entity in base.GetIQueryable()
                       where entity.User.ID == this.User.ID && entity.Status == Needs.Ccs.Services.Enums.Status.Normal
                       select entity;
            }
        }
    }
}