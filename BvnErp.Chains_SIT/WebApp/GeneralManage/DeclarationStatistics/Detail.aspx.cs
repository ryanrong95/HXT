using Needs.Ccs.Services;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.GeneralManage.DeclarationStatistics
{
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        protected void LoadData()
        {
        }

        protected void data()
        {
            var SaleManId = Request.QueryString["ID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string Currency = Request.QueryString["Currency"];
            var from = DateTime.Parse(StartDate);
            var to = DateTime.Parse(EndDate);
            var declarationStatistics = new Needs.Ccs.Services.Views.DeclarationStatisticsView().Where(x => x.OrderDate >= from
            && x.OrderDate < to.AddDays(1) && x.ID == SaleManId && x.Currency == Currency).ToArray();

            Func<Needs.Ccs.Services.Models.DeclarationStatistics, object> convert = t => new
            {
                ClientName = t.ClientName,
                OrderID = t.OrderID,
                DeclarePrice = t.DeclarePrice.ToRound(2).ToString("0.00"),
                OrderDate = t.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                OrderStatus = t.OrderStatus.GetDescription(),
                Currency = t.Currency,

            };
            Response.Write(new { rows = declarationStatistics.OrderByDescending(t => t.OrderDate).Select(convert).ToArray() }.Json());
        }
    }
}