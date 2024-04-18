using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.Headquarters
{
    /// <summary>
    /// 北京总部（HQ：headquarters）审批记录查询界面
    /// </summary>
    public partial class Record : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            //this.Model.ClientData = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Select(item => new { item.ID, item.Company.Name }).Json();
            this.Model.ControlType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.OrderControlType>().Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 初始化审批记录数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string controlType = Request.QueryString["ControlType"];

            var records = new Needs.Ccs.Services.Views.HQControlRecordsView().AsQueryable();
            if (!string.IsNullOrEmpty(orderID))
            {
                records = records.Where(item => item.OrderID.Contains(orderID.Trim()));
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                records = records.Where(item => item.Client.ClientCode.Contains(clientCode.Trim()));
            }
            if (!string.IsNullOrEmpty(controlType))
            {
                var conType = int.Parse(controlType);
                records = records.Where(item => (int)item.ControlType == conType);
            }

            Func<Needs.Ccs.Services.Models.OrderControlRecord, object> convert = record => new
            {
                record.ID,
                record.OrderID,
                record.Client.ClientCode,
                ClientName = record.Client.Company.Name,
                record.OrderItem?.Category.Name,
                record.OrderItem?.Model,
                record.OrderItem?.Manufacturer,
                record.OrderItem?.Category.HSCode,
                record.OrderItem?.Origin,
                ControlType = record.ControlType.GetDescription(),
                AuditResult = record.ControlStatus.GetDescription(),
                AdminName = string.IsNullOrEmpty(record.Admin.ByName) ? record.Admin.RealName : record.Admin.ByName,
                AuditDate = record.UpdateDate.ToString().Replace("T", " "),
                ApproveSummary = record.ApproveSummary
            };

            this.Paging(records, convert);
        }
    }
}