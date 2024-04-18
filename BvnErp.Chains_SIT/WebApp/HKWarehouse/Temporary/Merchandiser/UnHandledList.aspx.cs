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

namespace WebApp.HKWarehouse.Temporary.Merchandiser
{
    /// <summary>
    /// 未处理暂存记录-跟单
    /// </summary>
    public partial class UnHandledList : Uc.PageBase
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

            var temporaryView = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Temporary;
            var data = temporaryView.Where(item => item.Status == Status.Normal && item.TemporaryStatus == TemporaryStatus.Untreated);
            if (!string.IsNullOrEmpty(EntryNumber))
            {
                data = data.Where(item => item.EntryNumber == EntryNumber);
            }
            if (!string.IsNullOrEmpty(CompanyName))
            {
                data = data.Where(item => item.CompanyName.Contains(CompanyName));
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
            this.Paging(data, linq);
        }
    }
}