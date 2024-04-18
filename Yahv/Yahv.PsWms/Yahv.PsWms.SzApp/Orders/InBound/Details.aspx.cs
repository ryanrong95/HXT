using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.PsWms.SzMvc.Services.Views.Roll;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PsWms.SzApp.Orders.InBound
{
    public partial class Details : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var order = new SzMvc.Services.Views.Alls.OrdersAll().SingleOrDefault(t => t.ID == ID);
            var transport = new SzMvc.Services.Views.Origins.OrderTransportsOrigin().SingleOrDefault(t => t.ID == order.ConsignorID);
            var requires = new SzMvc.Services.Views.Origins.RequiresOrigin().Where(t => t.OrderID == ID);
            var files = new SzMvc.Services.Views.Origins.PcFilesOrigin().Where(t => t.MainID == ID);

            this.Model.orderData = new
            {
                Order = order,
                Transport = transport,
                Requires = requires,
                PickFile = files.Where(t => t.Type == SzMvc.Services.Enums.PsOrderFileType.Taking),
                DeliveryFile = files.Where(t => t.Type == SzMvc.Services.Enums.PsOrderFileType.InDelivery),
            };
        }

        protected object data()
        {
            string ID = Request.QueryString["ID"];
            var query = new SzMvc.Services.Views.Alls.OrderItemsAll().Where(t => t.OrderID == ID);

            return this.Paging(query, t => new
            {
                t.ID,
                t.CustomCode,
                t.Product.Partnumber,
                t.Product.Brand,
                t.Product.Package,
                t.Product.DateCode,
                StocktakingType = t.StocktakingType.GetDescription(),
                t.Mpq,
                t.PackageNumber,
                t.Total,
                t.Supplier,
            });
        }

        protected object fee()
        {
            string ID = Request.QueryString["ID"];
            var query = new SzMvc.Services.Views.Origins.PayeeLeftsOrigin().Where(t => t.FormID == ID);

            return this.Paging(query, t => new
            {
                t.ID,
                t.CutDateIndex,
                t.Subject,
                t.Total
            });
        }
    }
}