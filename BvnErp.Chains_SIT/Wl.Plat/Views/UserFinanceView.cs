using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Wl.User.Plat.Models;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 付款记录
    /// </summary>
    public class ClientReceiptNoticesView : ReceiptNoticesView
    {

        IPlatUser user;

        public ClientReceiptNoticesView(IPlatUser user)
        {
            this.user = user;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.ReceiptNotice> GetIQueryable()
        {
            return from receipt in base.GetIQueryable()
                   where receipt.Client.ID == this.user.Client.ID
                   select receipt;
        }
    }

    /// <summary>
    /// 付款明细
    /// </summary>
    public class ClientOrderReceivedsView: OrderReceiptsAllsView
    {

        protected override IQueryable<Needs.Ccs.Services.Models.OrderReceipt> GetIQueryable()
        {
            return from received in base.GetIQueryable()
                   select received;
        }
    }
}
