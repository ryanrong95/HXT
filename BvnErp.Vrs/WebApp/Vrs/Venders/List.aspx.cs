using Needs.Erp;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Vrs.Venders
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            string type = Request.QueryString["param"];
            IQueryable<NtErp.Vrs.Services.Models.Vender> data = ErpPlot.Current.Publishs.MyVenders;
            if (!string.IsNullOrWhiteSpace(type))
            {
                data = data.Where(item => item.Type == (NtErp.Vrs.Services.Enums.ComapnyType)Convert.ToInt32(type));
            }
            Response.Paging(data,item=>new {
                ID=item.ID,
                CompanyID=item.CompanyID,
                Name=item.Name,
                Grade=item.Grade.GetDescription(),
                Status = item.Status.GetDescription(),
                Type = item.Type.GetDescription(),
                Address =item.Address,
                CorporateRepresentative=item.CorporateRepresentative,
                RegisteredCapital=item.RegisteredCapital
            });
            Response.End();
        }
        protected void del()
        {
            string id = Request.Form["id"];
            var entity = ErpPlot.Current.Publishs.MyVenders[id] ?? new NtErp.Vrs.Services.Models.Vender();
            //entity.AbandonSuccess += AbandonSuccess;
            //entity.Abandon();
           // entity.Status = NtErp.Vrs.Services.Enums.Status.Limited;
            entity.AbandonVenderSuccess += AbandonSuccess;
            entity.Abandon();
        }
        private void AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write(new { success = true }.Json());
        }

        protected void editStatus()
        {
            string id = Request.Form["id"];
            int status = int.Parse(Request.Form["status"]);
            var entity = ErpPlot.Current.Publishs.MyVenders[id] ?? new NtErp.Vrs.Services.Models.Vender();
            entity.Status = (NtErp.Vrs.Services.Enums.Status)status;
            entity.EnterVenderSuccess += EnterVenderSuccess;
            entity.Enter();
        }
        private void EnterVenderSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write(new { success = true }.Json());
        }
    }
}