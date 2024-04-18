using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Fee.UnApproved
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }
        
        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            //费用类型
            this.Model.WarehousePremiumsStatus = "".Json();
            this.Model.WarehousePremiumsStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.WarehousePremiumType>()
                .Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 费用明细
        /// </summary>
        protected void data()
        {
            string OrderID = Request.QueryString["OrderID"];
            string Status = Request.QueryString["Status"];

            var Fees = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremium.Where(item=>item.WarehousePremiumsStatus== WarehousePremiumsStatus.Auditing).AsQueryable();
            if (!string.IsNullOrEmpty(OrderID))
            {
                Fees = Fees.Where(item => item.OrderID.Contains(OrderID));
            }
            if (!string.IsNullOrEmpty(Status))
            {
                Fees = Fees.Where(item => item.WarehousePremiumType == (WarehousePremiumType)int.Parse(Status));
            }

            Fees = Fees.OrderByDescending(item => item.CreateDate);

            Func<OrderWhesPremium, object> convert = fee => new
            {
                ID = fee.ID,
                OrderID = fee.OrderID,
                ClientName=fee.Client.Company.Name,
                Type = fee.WarehousePremiumType.GetDescription(),
                Currency = fee.Currency,
                Count = fee.Count,
                UnitPrice = fee.UnitPrice,
                ApprovalPrice = fee.Count* fee.UnitPrice,
                //Payer = fee.PayerType.GetDescription(),
                Creater= fee.Creater.RealName,
                CreateDate = fee.CreateDate.ToString("yyyy-MM-dd"),
            };
            Fees = Fees.OrderByDescending(t => t.CreateDate);
            this.Paging(Fees, convert);
        }
    }
}