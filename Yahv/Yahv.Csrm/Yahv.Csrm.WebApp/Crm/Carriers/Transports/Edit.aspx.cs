using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YaHv.Csrm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Web.Forms;

namespace Yahv.Csrm.WebApp.Crm.Carriers.Transports
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.VehicleType = ExtendsEnum.ToArray<VehicleType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                string carrierid = Request.QueryString["id"];
                string transportid = Request.QueryString["transportid"];
                if (!string.IsNullOrWhiteSpace(carrierid) && !string.IsNullOrWhiteSpace(transportid))
                {
                    var carrier = Erp.Current.Crm.Carriers[carrierid];
                    this.Model.Transports = carrier.Transports[transportid];
                }

            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var carrier = Erp.Current.Crm.Carriers[Request.QueryString["id"]];
            var transport = carrier.Transports[Request.QueryString["transportid"]] ?? new Transport();
            string carnumber1 = Request["CarNumber1"].Trim();
            string carnumber2 = Request["CarNumber2"].Trim();
            string weight = Request["Weight"].Trim();
            transport.Enterprise = carrier.Enterprise;
            transport.CarNumber1 = carnumber1;
            transport.CarNumber2 = carnumber2;
            transport.Weight = weight;
            var type = (VehicleType)int.Parse(Request["Type"]);
            transport.Type = type;
            transport.CreatorID = Erp.Current.ID;
            if (string.IsNullOrWhiteSpace(Request.QueryString["transportid"]) && carrier.Transports[transport.ID] != null)
            {
                Easyui.Reload("提示", "该车辆已存在!", Web.Controls.Easyui.Sign.Warning);
            }
            else
            {
                transport.EnterSuccess += Transport_EnterSuccess;
                transport.Enter();
            }
           
        }

        private void Transport_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Transport;
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
            model.Transport = new apiTransport
            {
                EnterpriseName = entity.Enterprise.Name,
                CarNumber2 = entity.CarNumber2,
                CarNumber1 = entity.CarNumber1,
                Weight = entity.Weight,
                Type = entity.Type,
                Status = 200,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Creator = entity.CreatorID,

            };
            model.Unify();
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}