using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.Carriers
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    this.Model.Entity = Erp.Current.Crm.Carriers[Request.QueryString["id"]];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                this.Model.CarrierType = ExtendsEnum.ToArray<CarrierType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var entity = Erp.Current.Crm.Carriers[id] ?? new Carrier();

            string admincode = Request["AdminCode"].Trim();
            string corporation = Request["Corporation"].Trim();
            string regAddress = Request["RegAddress"].Trim();
            string uscc = Request["Uscc"].Trim();
            string summary = Request["Summary"];
            string code = Request.Form["Code"];//简称

            entity.Summary = summary;
            entity.Code = code;
            entity.Icon = hidurl.Value;
            entity.Type = (CarrierType)int.Parse(Request["CarrierType"]);
            entity.IsInternational = Request["IsInternational"] != null;
            entity.Enterprise = new Enterprise
            {
                Name = Request.Form["Name"].Trim(),
                AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode,
                Corporation = corporation,
                RegAddress = regAddress,
                Uscc = uscc,
                Place = Request["Place"]
            };
            if (string.IsNullOrEmpty(id))
            {
                //录入人
                entity.CreatorID = Yahv.Erp.Current.ID;
                entity.NameReapt += Entity_NameReapt; ;
            }
            entity.EnterSuccess += Entity_EnterSuccess; ;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Carrier;
            ///仅物流或快递可同步，物流快递不同步
            if (entity.Type != CarrierType.Both)
            {
                var model = new CarrierModel();
                model.Carrier = new apiCarrier
                {
                    Name = entity.Enterprise.Name,
                    Code = entity.Code,
                    Summary = entity.Summary,
                    Status = 200,
                    CarrierType = Commons.ConvertType(entity.Type, entity.Enterprise.Place),
                    Creator = entity.CreatorID
                };
                model.Unify();
            }

            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

        private void Entity_NameReapt(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Alert("提示", "承运商已存在", Web.Controls.Easyui.Sign.Info);
        }
    }
}