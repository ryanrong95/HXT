using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils;
using Needs.Ccs.Services.Models;

namespace WebApp.SZWarehouse.Finance.InBound
{
    public partial class Entry : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var id = Request.QueryString["ID"];
            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZEntryNotice[id];
            this.Model.AllData = new {
                ID = id,
                Date=notice.CreateDate.ToString("yyyy-MM-dd"),
                DecheadID=notice.DecHead.ID,
                InBound= notice.SZItems.Select(item => new
                {
                    GNo=item.DecList.GNo,
                    GName=item.DecList.GName,
                    GoodsModel = item.DecList.GoodsModel,
                    GUnit = item.DecList.GUnit,
                    GQty=item.DecList.GQty,
                    DeclPrice = item.DecList.DeclPrice,
                    Dectotal=item.DecList.DeclTotal,
                    Summary=item?.Summary,
                })
            }.Json();          
        }
        protected void ExportFiles()
        {
            try
            {
                string ID = Request.Form["ID"];
                var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZEntryNotice[ID];
                //2.返回PDF文件
                var files = apply.ToPDf();
                Response.Write((new { success = true, message = "导出成功", url = files }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }
    }
}