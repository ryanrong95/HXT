using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.Carriers.Drivers
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var carrier = Erp.Current.Crm.Carriers[Request.QueryString["id"]];
                string driverid = Request.QueryString["driverid"];
                this.Model.Driver = carrier.Drivers[driverid];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var carrier = Erp.Current.Crm.Carriers[Request.QueryString["id"]];
            string driverid = Request.QueryString["driverid"];
            Driver entity = carrier.Drivers[driverid] ?? new Driver();
            if (string.IsNullOrWhiteSpace(driverid))
            {
                entity.Enterprise = carrier.Enterprise;
                entity.CreatorID = Erp.Current.ID;
            }
            entity.Name = Request["Name"].Trim();
            entity.IDCard = Request["IDCard"].Trim();
            entity.Mobile = Request["Mobile1"].Trim();

            entity.Mobile2 = Request["Mobile2"].Trim();
            entity.CardCode = Request["CardCode"].Trim();
            entity.CustomsCode = Request["CustomsCode"].Trim();
            entity.PortCode = Request["PortCode"].Trim();
            entity.LBPassword = Request["LBPassword"].Trim();
            entity.IsChcd = Request["IsChcd"] != null;
            if (string.IsNullOrWhiteSpace(driverid) && carrier.Drivers[entity.ID] != null)
            {
                Easyui.Reload("提示", "已存在重名司机!", Web.Controls.Easyui.Sign.Warning);
            }
            else
            {
                entity.EnterSuccess += Entity_EnterSuccess;
                entity.Enter();
            }

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Driver;
            var model = new CarrierModel();
            var carrier = Erp.Current.Crm.Carriers[entity.Enterprise.ID];
            model.Carrier = new apiCarrier
            {
                Name = carrier.Enterprise.Name,
                Code = carrier.Code,
                Summary = carrier.Summary,
                Status = 200,
                CarrierType = Commons.ConvertType(carrier.Type, carrier.Enterprise.Place),
                Creator = entity.CreatorID
            };
            model.Driver = new apiDriver
            {
                EnterpriseName = entity.Enterprise.Name,
                Name = entity.Name,
                CustomsCode = entity.CustomsCode,//海关编码
                CardCode = entity.CardCode,//司机卡号
                Mobile2 = entity.Mobile2,//临时手机号
                Mobile = entity.Mobile,//大陆手机号，必填，
                LBPassword = entity.LBPassword,
                IDCard = entity.IDCard,
                PortCode = entity.PortCode,
                Status = 200,
                IsChcd = entity.IsChcd  //是否中港贸易
            };
            model.Unify();
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}