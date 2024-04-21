using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using Yahv.Linq.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Rings;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.WmsAdmins
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ID = Request.QueryString["id"];
            }
        }

        /// <summary>
        /// 加载树形菜单
        /// </summary>
        /// <returns></returns>
        protected string tree()
        {
            var district = Alls.Current.Warehouses.Where(item => item.WareHouseStatus == ApprovalStatus.Normal).ToList();
            var wareHouseIds_selected = Alls.Current.Admins.GetWareHouseIds(Request.QueryString["id"]);

            var ss = new[]
            {
                new
                {
                    id = "1",
                    text = "全球",
                    children = (from dis in district
                        group dis by new {dis.EnterpriseID, dis.WarehouseName}
                        into g
                        select new
                        {
                            id = g.Key.EnterpriseID,
                            text = g.Key.WarehouseName,
                            attributes = new {fatherId = g.Key.EnterpriseID},
                            children = (from d in district
                                where d.EnterpriseID == g.Key.EnterpriseID
                                select new
                                {
                                    id = d.ID,
                                    text = d.Title,
                                    attributes = new {fatherId = g.Key.EnterpriseID},
                                    @checked = wareHouseIds_selected.Contains(d.ID),
                                })
                        }).ToArray(),
                }
            }.Json();

            return new[]
            {
                    new
                    {
                        id = "1",
                        text = "全球",
                        children = (from dis in district
                                    group dis by new {dis.EnterpriseID,dis.WarehouseName} into g
                                    select new
                                    {
                                        id = g.Key.EnterpriseID,
                                        text = g.Key.WarehouseName,
                                        attributes=new {fatherId= g.Key.EnterpriseID},
                                        children=(from d in district
                                                  where d.EnterpriseID ==g.Key.EnterpriseID
                                                  select new
                                                  {
                                                      id=d.ID,
                                                      text=d.Title,
                                                      attributes=new {fatherId= g.Key.EnterpriseID},
                                                      @checked=wareHouseIds_selected.Contains(d.ID),
                                                  })
                                    }).ToArray(),
                    }
}.Json();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var adminid = Request.QueryString["id"];
                var ids = this.hidids.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(item => new
                {
                    id = item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[1],
                    fatherId = item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[0],
                }).Json();



                Alls.Current.Admins.BindingWareHouse(adminid, ids);

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "管理员管理", $"给【{adminid}】分配库房", ids.Json());
            }
            catch (Exception ex)
            {
                Easyui.Alert("操作提示", ex.Message, Web.Controls.Easyui.Sign.Error);
            }

            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}