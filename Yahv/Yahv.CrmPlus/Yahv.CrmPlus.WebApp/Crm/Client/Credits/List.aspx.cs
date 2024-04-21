using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Credits
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.TakerID = Request.QueryString["TakerID"];
            }
        }
        protected object GetCredit()
        {
            string takerid = Request.QueryString["TakerID"];
            var query = new CreditStatisticsRoll()[CreditType.GrantingParty].Where(item => item.Taker.ID == takerid);
            return query.ToArray().Select(item => new
            {
                MakerID = item.Maker.ID,
                MakerName = item.Maker.Name,
                TakerName = item.Taker.Name,
                item.Total,
                item.Cost,
                Currency = item.Currency,
                CurrencyDes = item.Currency.GetDescription(),
                Surplus = item.Total + item.Cost
            });

        }
        protected object Settlement()
        {
            string takerid = Request.QueryString["TakerID"];
            var query = new ClientCreditsRoll().Where(item => item.TakerID == takerid);
            return query.ToArray().Select(item => new
            {
                item.ID,
                item.MakerID,
                MakerName = item.Maker.Name,
                TakerName = item.Taker.Name,
                ClearType = item.ClearType.GetDescription(),
                ClearDate = item.Months + "个月" + item.Days + "天后",
                item.IsAvailable,
            });

        }
    }
}