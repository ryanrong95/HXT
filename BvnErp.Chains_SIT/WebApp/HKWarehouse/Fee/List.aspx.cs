using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Fee
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            #region 旧视图

            /*
            string orderID = Request.QueryString["OrderID"];
            string entryNumber = Request.QueryString["ClientCode"];
            string clientName = Request.QueryString["ClientName"];

            var view = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremium;
            var data = view.Where(entryEntry => entryEntry.WarehouseType == WarehouseType.HongKong);
            if (!string.IsNullOrEmpty(orderID))
            {
                data = data.Where(item => item.OrderID.Contains(orderID));
            }

            if (!string.IsNullOrEmpty(entryNumber))
            {
                data = data.Where(item => item.Client.ClientCode == entryNumber);
            }

            if (!string.IsNullOrEmpty(clientName))
            {
                data = data.Where(item => item.Client.Company.Name.Contains(clientName));
            }

            data = data.OrderByDescending(item => item.CreateDate);

            Func<Needs.Ccs.Services.Models.OrderWhesPremium, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.OrderID,
                EntryNumber = item.Client.ClientCode,
                ClientName = item.Client.Company.Name,
                Currency = item.Currency,
                WarehousePremiumType = item.WarehousePremiumType.GetDescriptions(),
                PaymentType = item.WhsePaymentType.GetDescriptions(),
                item.UnitPrice,
                item.UnitName,
                item.Count,
                item.ApprovalPrice,
                CreateDate = item.CreateDate,
                AdminName = item.Creater.RealName,
                Approver = item.Approver?.RealName,
                Status = item.WarehousePremiumsStatus.GetDescription(),
            };
            data = data.OrderByDescending(t => t.CreateDate);
            this.Paging(data, convert);
            */

            #endregion

            #region 新视图

            string orderID = Request.QueryString["OrderID"];
            string entryNumber = Request.QueryString["ClientCode"];
            string clientName = Request.QueryString["ClientName"];

            var fees = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremiumsAll;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> expression = item => item.WarehouseType == WarehouseType.HongKong;

            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => item.OrderID.Contains(orderID.Trim());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(entryNumber))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(item => item.ClientCode == entryNumber.Trim()).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientName))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(item => item.Company.Name.Contains(clientName)).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }

            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            var feelist = fees.GetPageList(page, rows, expression, lambdas.ToArray());

            Response.Write(new
            {
                rows = feelist.Select(
                        item => new
                        {
                            ID = item.ID,
                            OrderID = item.OrderID,
                            EntryNumber = item.Client.ClientCode,
                            ClientName = item.Client.Company.Name,
                            Currency = item.Currency,
                            WarehousePremiumType = item.WarehousePremiumType.GetDescriptions(),
                            PaymentType = item.WhsePaymentType.GetDescriptions(),
                            item.UnitPrice,
                            item.UnitName,
                            item.Count,
                            item.ApprovalPrice,
                            CreateDate = item.CreateDate.ToShortDateString(),
                            AdminName = item.Creater.RealName,
                            Approver = item.Approver?.RealName,
                            Status = item.WarehousePremiumsStatus.GetDescription(),
                        }
                     ).ToArray(),
                total = feelist.Total,
            }.Json());

            #endregion
        }
    }
}