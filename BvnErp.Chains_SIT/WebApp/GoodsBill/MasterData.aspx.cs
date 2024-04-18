using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.GoodsBill
{
    public partial class MasterData : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string DecHeadID = Request.QueryString["DecHeadID"];
            string ContrNo = Request.QueryString["ContrNo"];
            string ClientNo = Request.QueryString["ClientNo"];
            string ClientName = Request.QueryString["ClientName"];
            string GName = Request.QueryString["GName"];
            string Model = Request.QueryString["Model"];
            string InDateFrom = Request.QueryString["InDateFrom"];
            string InDateTo = Request.QueryString["InDateTo"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            using (var query = new Needs.Ccs.Services.Views.GoodsBillMasterDataView())
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(DecHeadID))
                {
                    DecHeadID = DecHeadID.Trim();                  
                    view = view.SearchByEntryID(DecHeadID);
                }

                if (!string.IsNullOrWhiteSpace(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    view = view.SearchByContrNo(ContrNo);
                }

                if (!string.IsNullOrWhiteSpace(ClientNo))
                {
                    ClientNo = ClientNo.Trim();
                    view = view.SearchByClientNo(ClientNo);
                }

                if (!string.IsNullOrWhiteSpace(ClientName))
                {
                    ClientName = ClientName.Trim();
                    view = view.SearchByClientName(ClientName);
                }

                if (!string.IsNullOrWhiteSpace(Model))
                {
                    Model = Model.Trim();
                    view = view.SearchByModel(Model);
                }

                if (!string.IsNullOrWhiteSpace(GName))
                {
                    GName = GName.Trim();
                    view = view.SearchByGName(GName);
                }

                if (!string.IsNullOrWhiteSpace(InDateFrom))
                {
                    InDateFrom = InDateFrom.Trim();
                    DateTime dtFrom = Convert.ToDateTime(InDateFrom);
                    view = view.SearchByFrom(dtFrom);
                }


                if (!string.IsNullOrWhiteSpace(InDateTo))
                {
                    InDateTo = InDateTo.Trim();
                    DateTime dtTo = Convert.ToDateTime(InDateTo).AddDays(1);
                    view = view.SearchByTo(dtTo);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}