using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.Merchandiser
{
    /// <summary>
    /// 跟单员审批记录查询界面
    /// </summary>
    public partial class Record : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //LoadComboBoxData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.ClientData = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Select(item => new { item.ID, item.Company.Name }).Json();
        }

        /// <summary>
        /// 初始化审批记录数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];

            var records = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControlRecords.AsQueryable();
            if (!string.IsNullOrEmpty(orderID))
            {
                records = records.Where(item => item.OrderID.Contains(orderID.Trim()));
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                records = records.Where(item => item.Client.ClientCode.Contains(clientCode.Trim()));
            }

            Func<Needs.Ccs.Services.Models.OrderControlRecord, object> convert = record => new
            {
                record.ID,
                record.OrderID,
                record.Client.ClientCode,
                ClientName = record.Client.Company.Name,
                Name = record.OrderItem?.Category?.Name ?? record.OrderItem?.Name,
                record.OrderItem?.Model,
                record.OrderItem?.Manufacturer,
                record.OrderItem?.Category?.HSCode,
                record.OrderItem?.Origin,
                ControlTypeValue = record.ControlType,
                ControlType = record.ControlType.GetDescription(),
                record.ControlStatus,
                AuditResult = record.ControlStatus.GetDescription(),
                AdminName = record.Admin.RealName,
                AuditDate = record.UpdateDate.ToString().Replace("T", " "),
                ApproveSummary = record.ApproveSummary
            };

            this.Paging(records, convert);
        }
    }
}