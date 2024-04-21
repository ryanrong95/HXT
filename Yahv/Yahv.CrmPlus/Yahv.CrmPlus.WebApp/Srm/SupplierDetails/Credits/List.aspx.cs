using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Credits
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.MakerID = Request.QueryString["MakerID"];
            }
        }
        protected object GetCredit()
        {
            string makerid = Request.QueryString["MakerID"];
            var query = new CreditStatisticsRoll()[CreditType.CreditReceiver].Where(item => item.Maker.ID == makerid);
            return query.ToArray().Select(item => new
            {
                MakerName1 = item.Maker.Name,
                TakerName1 = item.Taker.Name,
                item.Total,
                item.Cost,
                Currency = item.Currency,
                CurrencyDes = item.Currency.GetDescription(),
                Surplus = item.Total + item.Cost
            });

        }
        protected object Settlement()
        {
            string makerid = Request.QueryString["MakerID"];
            var query = new SupplierCreditsRoll().Where(item => item.MakerID == makerid);
            return query.ToArray().Select(item => new
            {
                item.ID,
                item.TakerID,
                MakerName = item.Maker.Name,
                TakerName = item.Taker.Name,
                ClearType = item.ClearType.GetDescription(),
                ClearDate = item.Months + "个月" + item.Days + "天后",
                item.IsAvailable,
            });

        }
    }
}