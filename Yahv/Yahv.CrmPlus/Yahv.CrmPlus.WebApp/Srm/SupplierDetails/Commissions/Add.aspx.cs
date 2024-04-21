using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Commissions
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string supplierid = Request.QueryString["id"];
            CommissionType type = (CommissionType)int.Parse(Request.Form["Type"]);
            string method = Request.Form["Methord"];
            decimal msp = decimal.Parse(Request.Form["Msp"]);
            decimal radio = decimal.Parse(Request.Form["Radio"]);
            Currency currecny = (Currency)int.Parse(Request.Form["Currency"]);
            Commission entity = new Commission();
            entity.SupplierID = supplierid;
            entity.Type = type;
            if (type == CommissionType.Discount)
            {
                entity.Methord = CommissionMethod.None;
            }
            else
            {
                entity.Methord = (CommissionMethod)int.Parse(method);
            }
            entity.Msp = msp;
            entity.Radio = radio;
            entity.Currency = currecny;
            entity.CreatorID = Erp.Current.ID;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Commission;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增返款信息:{entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}