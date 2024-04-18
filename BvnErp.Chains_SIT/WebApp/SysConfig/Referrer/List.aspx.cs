using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.Referrer
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void data()
        {


            string name = Request.QueryString["Name"];
            var entity = Needs.Wl.Admin.Plat.AdminPlat.Referrers.Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                entity = entity.Where(item => item.Name.Contains(name));
            }
            Func<Needs.Ccs.Services.Models.Referrer, object> convert = item => new
            {
                item.ID,
                item.Name,
                CreatorName = item.CreatorName.RealName,
                item.Summary,
                CreateDate = item.CreateDate.ToShortDateString(),
                UpdateDate = item.UpdateDate
            };
            this.Paging(entity, convert);
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void DeleteReferrer()
        {
            try
            {
                string id = Request.Form["ID"];
                var entity = Needs.Wl.Admin.Plat.AdminPlat.Referrers[id];
                if (entity == null)
                {
                    Response.Write((new { success = false, message = "该引荐人已删除" }).Json());
                    return;
                }
                var isExsit = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView.Any(item => item.Referrer == entity.Name);
                if (isExsit)
                {
                    Response.Write((new { success = false, message = "该引荐人已使用，不能删除" }).Json());
                    return;
                }
                entity.Abandon();
                Response.Write((new { success = true, message = "删除成功！", ID = entity.ID }).Json());
            }
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = "删除成功:" + ex }).Json());
            }

        }
    }
}
