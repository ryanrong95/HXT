using System;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.Cities
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        private void InitData()
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                var league = new LeaguesRoll().FirstOrDefault(item => item.ID == Request.QueryString["id"]);

                if (league != null)
                {
                    this.Model = new
                    {
                        ID = Request.QueryString["id"],
                        FatherID = league.FatherID,
                        Name = league.Name,
                        Type = league.Type,
                        RoleID = league.RoleID,
                    };
                }
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            League model;

            model = new League()
            {
                Name = Request.Form["Name"].Trim(),
                Type = LeagueType.Area,
                Category = Category.StaffInCity,
                Status = Status.Normal,
                FatherID = Request["FatherId"]
            };

            new League().Subs.Add(model);
            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工城市",
                    $"添加子节点", model.Json());
            //Easyui.Reload("操作提示", "操作成功!", Web.Controls.Easyui.Sign.None);
            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);

        }
    }
}