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

namespace WebApp.HKWarehouse.Temporary
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
            string EntryNumber = Request.QueryString["EntryNumber"];
            string CompanyName = Request.QueryString["CompanyName"];
            string WaybillCode = Request.QueryString["WaybillCode"];
            string Status = Request.QueryString["Status"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var temporaryView = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Temporary;
            var data = temporaryView.Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            if (!string.IsNullOrEmpty(EntryNumber))
            {
                data = data.Where(item => item.EntryNumber == EntryNumber);
            }
            if (!string.IsNullOrEmpty(CompanyName))
            {
                data = data.Where(item => item.CompanyName.Contains(CompanyName));
            }
            if (!string.IsNullOrEmpty(WaybillCode))
            {
                data = data.Where(item => item.WaybillCode == WaybillCode);
            }
            if (!string.IsNullOrEmpty(Status))
            {
                data = data.Where(item => item.TemporaryStatus == (TemporaryStatus)int.Parse(Status));
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                var start = Convert.ToDateTime(StartDate);
                data = data.Where(item => item.EntryDate >= start);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var end = Convert.ToDateTime(EndDate).AddDays(1);
                data = data.Where(item => item.EntryDate < end);
            }

            data = data.OrderByDescending(item => item.CreateDate);

            Func<Needs.Ccs.Services.Models.Temporary, object> linq = item => new
            {
                ID = item.ID,
                EntryNumber = item.EntryNumber,
                CompanyName = item.CompanyName,
                ShelveNumber = item.ShelveNumber,
                WaybillCode = item.WaybillCode,
                PackNo = item.PackNo,
                EntryDate = item.EntryDate.ToString("yyyy-MM-dd"),
                Status = item.TemporaryStatus.GetDescription()
            };
            data = data.OrderByDescending(t => t.EntryDate);
            this.Paging(data, linq);
        }
    }
}