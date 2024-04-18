using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Entry
{
    public partial class PackedBoxList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void PackedBoxData()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            string PackingDate = Request.QueryString["PackingDate"];
            string BoxIndex = Request.QueryString["BoxIndex"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(PackingDate))
            {
                PackingDate = PackingDate.Trim();

                DateTime dt;
                if (DateTime.TryParse(PackingDate, out dt))
                {
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.HKPackedBoxView.BoxListModel, bool>>)(t => t.PackingDate == dt));
                }
            }
            if (!string.IsNullOrEmpty(BoxIndex))
            {
                BoxIndex = BoxIndex.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.HKPackedBoxView.BoxListModel, bool>>)(t => t.BoxIndex == BoxIndex));
            }

            int total = 0;

            var listBoxList = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKPackedBoxView.GetBoxListModelResult(out total, page, rows, lamdas.ToArray()).ToList();

            Func<Needs.Ccs.Services.Views.HKPackedBoxView.BoxListModel, object> convert = item => new
            {
                PackingID = item.PackingID,
                PackingDate = item.PackingDate.ToString("yyyy-MM-dd"),
                BoxIndex = item.BoxIndex,
                StockCode = item.StockCode,
                OrderID = item.OrderID,
                SealedName = item.SealedName,
                SealedDate = item.SealedDate?.ToString(),
                ClientCode = item.ClientCode,
                ClientName = item.ClientName,
            };

            Response.Write(new
            {
                rows = listBoxList.Select(convert).ToArray(),
                total = total,
            }.Json());
        }

        protected void BoxDetail()
        {
            string PackingID = Request.QueryString["PackingID"];

            var listBoxDetail = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKPackedBoxView.GetBoxDetailModel(PackingID).ToList();

            Func<Needs.Ccs.Services.Views.HKPackedBoxView.BoxDetailModel, object> convert = item => new
            {
                Model = item.Model,
                Name = item.Name,
                Manufacturer = item.Manufacturer,
                Quantity = item.Quantity,
                Origin = item.Origin,
                GrossWeight = item.GrossWeight,
            };

            Response.Write(new
            {
                rows = listBoxDetail.Select(convert).ToArray(),
            }.Json());
        }
    }
}