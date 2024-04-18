using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class UploadedTaxFlow : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        private void load()
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ContrNo = Request.QueryString["ContrNo"];
            string EntryId = Request.QueryString["EntryId"];
            string OrderID = Request.QueryString["OrderID"];
            string OwnerName = Request.QueryString["OwnerName"];
            string StrDDateBegin = Request.QueryString["StrDDateBegin"];
            string StrDDateEnd = Request.QueryString["StrDDateEnd"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(ContrNo))
            {
                ContrNo = ContrNo.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UploadedTaxFlowListView.UploadedTaxFlowListModel, bool>>)(t => t.ContrNo.Contains(ContrNo)));
            }
            if (!string.IsNullOrEmpty(EntryId))
            {
                EntryId = EntryId.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UploadedTaxFlowListView.UploadedTaxFlowListModel, bool>>)(t => t.EntryId.Contains(EntryId)));
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UploadedTaxFlowListView.UploadedTaxFlowListModel, bool>>)(t => t.OrderID.Contains(OrderID)));
            }
            if (!string.IsNullOrEmpty(OwnerName))
            {
                OwnerName = OwnerName.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UploadedTaxFlowListView.UploadedTaxFlowListModel, bool>>)(t => t.OwnerName.Contains(OwnerName)));
            }
            if (!string.IsNullOrEmpty(StrDDateBegin))
            {
                DateTime dt;
                if (DateTime.TryParse(StrDDateBegin,out dt))
                {
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UploadedTaxFlowListView.UploadedTaxFlowListModel, bool>>)(t => t.DDate >= dt));
                }
            }
            if (!string.IsNullOrEmpty(StrDDateEnd))
            {
                DateTime dt;
                if (DateTime.TryParse(StrDDateEnd, out dt))
                {
                    dt = dt.AddDays(1);
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UploadedTaxFlowListView.UploadedTaxFlowListModel, bool>>)(t => t.DDate < dt));
                }
            }

            int count = 0;
            var uploadedTaxFlowList = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.UploadedTaxFlowListView.GetResult(out count, page, rows, lamdas.ToArray());

            Func<Needs.Ccs.Services.Views.UploadedTaxFlowListView.UploadedTaxFlowListModel, object> convert = uploadedTaxFlow => new
            {
                ContrNo = uploadedTaxFlow.ContrNo,
                EntryId = uploadedTaxFlow.EntryId,
                DDate = uploadedTaxFlow.DDate?.ToString("yyyy-MM-dd"),
                TariffTaxNumber = uploadedTaxFlow.TariffTaxNumber,
                TariffAmount = uploadedTaxFlow.TariffAmount,
                ExciseTaxNumber = uploadedTaxFlow.ExciseTaxNumber,
                ExciseTaxAmount = uploadedTaxFlow.ExciseTaxAmount,
                AddedValueTaxTaxNumber = uploadedTaxFlow.AddedValueTaxTaxNumber,
                AddedValueTaxAmount = uploadedTaxFlow.AddedValueTaxAmount,
                OrderID = uploadedTaxFlow.OrderID,
                OwnerName = uploadedTaxFlow.OwnerName,
            };

            Response.Write(new
            {
                rows = uploadedTaxFlowList.Select(convert).ToArray(),
                total = count,
            }.Json());
        }





    }
}