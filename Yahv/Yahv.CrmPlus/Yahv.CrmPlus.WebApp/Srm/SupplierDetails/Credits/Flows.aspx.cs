using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;
using YaHv.CrmPlus.Services.Models.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Credits
{
    public partial class Flows : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            var query = new FlowCreditsRoll().Where(Predicate());
            return query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.Subject,
                item.Catalog,
                Price = Math.Abs(item.Price),
                Currency = item.Currency.GetDescription(),
                RealName = item.Creator?.RealName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        Expression<Func<FlowCredit, bool>> Predicate()
        {
            string makerid = Request.QueryString["MakerID"];
            string takerid = Request.QueryString["TakerID"];
            string currency = Request.QueryString["Currency"];
            bool iscredit = bool.Parse(Request.QueryString["isCredit"]);
            Currency cr = Currency.Unknown;
            Enum.TryParse(currency, out cr);
            Expression<Func<FlowCredit, bool>> predicate = item => item.Type==CreditType.CreditReceiver;

            if (!string.IsNullOrWhiteSpace(makerid))
            {
                predicate = predicate.And(item => item.MakerID == makerid);
            }
            if (!string.IsNullOrWhiteSpace(takerid))
            {
                predicate = predicate.And(item => item.TakerID == takerid);
            }
            if (cr != Currency.Unknown)
            {
                predicate = predicate.And(item => item.Currency == cr);
            }
            predicate = iscredit ? predicate.And(item => item.Price > 0) : predicate.And(item => item.Price < 0);
            return predicate;
        }
    }
}