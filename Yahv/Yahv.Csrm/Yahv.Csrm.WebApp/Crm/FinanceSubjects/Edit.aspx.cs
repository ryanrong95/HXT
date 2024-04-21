using System;
using System.Linq;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.FinanceSubjects
{
    public partial class Edit : BasePage
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
                var league = new SubjectsRoll().FirstOrDefault(item => item.ID == Request.QueryString["id"]);

                if (league != null)
                {
                    this.Model = new
                    {
                        ID = Request.QueryString["id"],
                        FatherID = league.FatherID,
                        Name = league.Name,
                        Type = league.Type,
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
            var result = ExtendsEnum.ToDictionary<YaHv.Csrm.Services.SubjectType>();

            return result.Select(item => new
            {
                id = int.Parse(item.Key),
                name = item.Value.ToString(),
            });

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            _Subject model;

            model = new _Subject()
            {
                ID = Request["ID"].Trim(),
                Name = Request.Form["Name"].Trim(),
                Type = (YaHv.Csrm.Services.SubjectType)Enum.Parse(typeof(YaHv.Csrm.Services.SubjectType), Request.Form["Type"]),
                Status = Status.Normal,
                FatherID = Request["FatherId"]
            };

            var subjects = Erp.Current.Crm.FinanceSubjects;
            if (subjects.Any(item => item.Name == model.Name && item.ID != model.ID))
            {
                Easyui.Alert("操作失败", "财务科目名称不能重复!", Sign.Error);
                return;
            }

            new _Subject().Subs.Add(model);

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Crm), "财务科目",
                    $"保存", model.Json());

            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);

        }
    }
}