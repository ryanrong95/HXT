using System;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Prm.Companies
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                ///Yahv.Erp.Current.Crm.Clients[clientid]
                var entity = new YaHv.Csrm.Services.Views.Rolls.CompaniesRoll()[id];
                //下拉绑定
                //1.类型
                selType.DataSource = ExtendsEnum.ToDictionary<CompanyType>();
                selType.DataTextField = "Value";
                selType.DataValueField = "Key";
                selType.DataBind();
                //2.范围
                selRange.DataSource = ExtendsEnum.ToDictionary<AreaType>();
                selRange.DataTextField = "Value";
                selRange.DataValueField = "Key";
                selRange.DataBind();
                if (entity != null)
                {
                    this.selRange.Value = ((int)entity?.Range).ToString();
                    this.selType.Value = ((int)entity?.Type).ToString();
                    this.Model = new
                    {
                        ID = entity?.ID,
                        AdminCode = entity?.Enterprise.AdminCode,
                        Name = entity?.Enterprise.Name,
                        District = entity?.Enterprise.District,
                        Type = entity?.Type,
                        entity.Enterprise.Uscc,
                        entity.Enterprise.Corporation,
                        entity.Enterprise.RegAddress

                    };
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var entity = new YaHv.Csrm.Services.Views.Rolls.CompaniesRoll()[id] ?? new Company();
            entity.Type = (CompanyType)int.Parse(selType.Value);
            entity.Range = (AreaType)int.Parse(selRange.Value);
            string corporation = Request["Corporation"].Trim();
            string regAddress = Request["RegAddress"].Trim();
            string uscc = Request["Uscc"].Trim();
            entity.Enterprise = new Enterprise
            {
                Name = Request.Form["Name"].Trim(),
                AdminCode = Request.Form["AdminCode"].Trim(),
                District = Request.Form["District"].Trim(),
                Corporation = corporation,
                RegAddress = regAddress,
                Uscc = uscc
            };
            if (string.IsNullOrEmpty(id))
            {
                entity.CreatorID = Yahv.Erp.Current.ID;
                entity.StatusUnnormal += Company_StatusUnnormal;
            }
            entity.EnterSuccess += Company_EnterSuccess;
            entity.Enter();

        }
        private void Company_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Company;
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Plat),
                                        "新增内部公司", "新增内部公司：" + entity.Enterprise.Name, "");
            }
            else
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Plat),
                                        "修改内部公司信息", "修改内部公司信息：" + entity.Enterprise.Name, "");
            }
            //Easyui.Alert("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info, true, Web.Controls.Easyui.Method.Window);
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

        private void Company_StatusUnnormal(object sender, Usually.ErrorEventArgs e)
        {
            var entity = sender as Company;
            Easyui.Reload("提示", "该公司已存在，状态：" + entity.CompanyStatus.GetDescription(), Yahv.Web.Controls.Easyui.Sign.Warning);
        }
    }
}