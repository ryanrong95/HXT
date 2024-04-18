using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.AdvanceMoney
{
    public partial class AdvanceRecord : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }
        protected void LoadData()
        {
            this.Model.AdvanceMoneyApply = "".Json();

            string clientID = Request.QueryString["ClientID"];
            var advanceMoneyApply = new Needs.Ccs.Services.Views.AdvanceMoneyApplyView1().Where(t => t.Status == Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective && t.ClientID == clientID).FirstOrDefault();

            if (advanceMoneyApply != null)
            {
                //申请信息
                this.Model.AdvanceMoneyApply = new
                {
                    ClientCode = advanceMoneyApply.ClientCode,
                    ClientName = advanceMoneyApply.ClientName,
                    ClientID = advanceMoneyApply.ClientID,
                    Amount = advanceMoneyApply.Amount,
                    AmountUsed = advanceMoneyApply.AmountUsed,
                    LimitDays = advanceMoneyApply.LimitDays,
                    InterestRate = Convert.ToDouble(advanceMoneyApply.InterestRate).ToString() + "%",
                }.Json();
            }

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string clientID = Request.QueryString["ClientID"];
            using (var query = new Needs.Ccs.Services.Views.AdvanceMoneyRecordsView())
            {
                var view = query;
                if (!string.IsNullOrEmpty(clientID))
                {
                    view = view.SearchByClientID(clientID);
                }
                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}
