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
    /// <summary>
    /// 暂存通知-已处理 
    /// 香港库房
    /// </summary>
    public partial class HandledList : Uc.PageBase
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
            var data = temporaryView.Where(item => item.Status == Status.Normal && item.TemporaryStatus == TemporaryStatus.Treated);
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

        /// <summary>
        /// 装箱完成
        /// </summary>
        /// <returns></returns>
        protected void Packing()
        {
            try
            {
                string id = Request.Form["ID"];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                var temporary = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Temporary.Where(item => item.ID == id).FirstOrDefault();
                temporary.TemporaryStatus = TemporaryStatus.Complete;
                temporary.SetOperator(admin);
                temporary.Enter();
                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch(Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            } 
        }
    }
}