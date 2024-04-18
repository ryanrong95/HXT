using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWarehouse.Entry
{
    public partial class OnStockList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.OnStockListScVoyageID = Server.UrlDecode(Request.QueryString["OnStockListScVoyageID"]) ?? string.Empty;
            this.Model.OnStockListScCarrierName = Server.UrlDecode(Request.QueryString["OnStockListScCarrierName"]) ?? string.Empty;
        }

        protected void UnStockList()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            string VoyageID = Request.QueryString["VoyageID"];
            string CarrierName = Request.QueryString["CarrierName"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(VoyageID))
            {
                VoyageID = VoyageID.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZOnStockView.VoyageListModel, bool>>)(t => t.VoyageID.Contains(VoyageID)));
            }
            if (!string.IsNullOrEmpty(CarrierName))
            {
                CarrierName = CarrierName.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZOnStockView.VoyageListModel, bool>>)(t => t.CarrierName.Contains(CarrierName)));
            }


            int total = 0;

            var listVoyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOnStockView.GetVoyageListModel(out total, page, rows, lamdas.ToArray()).ToList();

            Func<Needs.Ccs.Services.Views.SZOnStockView.VoyageListModel, object> convert = item => new
            {
                VoyageID = item.VoyageID,
                CarrierName = item.CarrierName,
                HKLicense = item.HKLicense,
                TransportTime = item.TransportTime?.ToString("yyyy-MM-dd"),
                DriverName = item.DriverName,
                VoyageType = item.VoyageType.GetDescription(),
                AllBoxNum = item.AllBoxNum,
                StockedBoxNum = item.StockedBoxNum,
            };


            Response.Write(new
            {
                rows = listVoyage.Select(convert).ToArray(),
                total = total,
            }.Json());
        }
    }
}