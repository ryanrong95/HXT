using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
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
    /// 北京总部（HQ：headquarters）管控审批查询界面
    /// </summary>
    public partial class CCCList : Uc.PageBase
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
        /// 初始化管控审批数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];

            var controls = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyHQControls.Where(t=>t.ControlType== OrderControlType.CCC).AsQueryable();
            if (!string.IsNullOrEmpty(orderID))
            {
                controls = controls.Where(item => item.Order.ID.Contains(orderID.Trim()));
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                controls = controls.Where(item => item.Order.Client.ClientCode.Contains(clientCode.Trim()));
            }

            Func<Needs.Ccs.Services.Models.HQControl, object> convert = control => new
            {
                control.ID,
                OrderID = control.Order.ID,
                ClientCode = control.Order.Client.ClientCode,
                ClientName = control.Order.Client.Company.Name,
                ClientRank = control.Order.Client.ClientRank,
                ControlTypeValue = control.ControlType,
                ControlType = control.ControlType.GetDescription(),
                DeclarePrice = control.Order.DeclarePrice.ToRound(2).ToString("0.00"),
                Currency = control.Order.Currency,
                Merchandiser = control.Order.Client.Merchandiser.RealName,
                OrderDate = control.Order.CreateDate.ToShortDateString()
            };

            this.Paging(controls, convert);
        }
    }
}