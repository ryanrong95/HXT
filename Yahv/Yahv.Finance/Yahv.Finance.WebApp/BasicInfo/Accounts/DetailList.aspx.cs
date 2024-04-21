using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.Accounts
{
    public partial class DetailList : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object getOrgin()
        {
            var accountId = Request.QueryString["AccountID"];
            var flows = Erp.Current.Finance.FlowAccounts.GetVouchersOrigin().Where(item => item.AccountID == accountId).ToArray();

            return this.Paging(flows.OrderBy(item => item.CreateDate), item => new
            {
                item.ID,
                LeftPrice = item.LeftPrice,
                item.RightPrice,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                Currency = item.Currency.GetDescription(),
                AccountMethord = item.AccountMethord.GetDescription(),
                item.Balance,
                item.CompanyName,
                item.Creator,
            });
        }

        protected object getStandard()
        {
            var accountId = Request.QueryString["AccountID"];
            var flows = Erp.Current.Finance.FlowAccounts.GetVouchersOrigin().Where(item => item.AccountID == accountId).ToArray();

            return this.Paging(flows.OrderBy(item => item.CreateDate), item => new
            {
                item.ID,
                LeftPrice = item.LeftPrice,
                item.RightPrice,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                Currency = item.Currency.GetDescription(),
                AccountMethord = item.AccountMethord.GetDescription(),
                item.Balance,
                item.CompanyName,
                item.Creator,
                item.Balance1,
                item.LeftPrice1,
                item.Rate,
                item.RightPrice1,
            });
        }
    }
}