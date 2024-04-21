using System;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.Leagues
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
                        EnterpriseID = league.EnterpriseID,
                    };
                }
            }
        }


        /// <summary>
        /// 组织类型
        /// </summary>
        /// <returns>组织类型列表</returns>
        protected object selects_type()
        {
            var result = ExtendsEnum.ToDictionary<LeagueType>();

            return result.Select(item => new
            {
                id = int.Parse(item.Key),
                name = item.Value.ToString(),
            });

        }

        /// <summary>
        /// 角色
        /// </summary>
        /// <returns></returns>
        protected object roles()
        {
            return Alls.Current.Roles.Select(item => new
            {
                id = item.ID,
                name = item.Name
            });
        }

        /// <summary>
        /// 内部公司
        /// </summary>
        /// <returns></returns>
        protected object enterprises()
        {
            return Alls.Current.Companies.Select(item => new
            {
                id = item.ID,
                name = item.Name
            });
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            League model;

            model = new League()
            {
                Name = Request.Form["Name"].Trim(),
                Type = (LeagueType)Enum.Parse(typeof(LeagueType), Request.Form["Type"]),
                Category = Category.Work,
                Status = Status.Normal,
                FatherID = Request["FatherId"]
            };

            //只有职位的时候，才可以给角色赋值
            if (model.Type == LeagueType.Position)
            {
                model.RoleID = Request.Form["RoleID"].Trim();
            }

            if (model.Type == LeagueType.Company)
            {
                model.EnterpriseID = Request.Form["EnterpriseID"].Trim();
            }

            new League().Subs.Add(model);

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "组织结构",
                    $"保存", model.Json());

            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);

        }
    }
}