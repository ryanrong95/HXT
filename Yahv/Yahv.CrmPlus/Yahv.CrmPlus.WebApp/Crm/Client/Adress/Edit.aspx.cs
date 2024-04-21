using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Adress
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var address = Request.Form["Address"];
            var name = Request.Form["Name"];
            var phone = Request.Form["Phone"];
            var content = Request.Form["Context"];
            var postZip = Request.Form["PostZip"];
            var id = Request.QueryString["enterpriseid"];
            var district = Request.Form["District"];
            var entity = new Address();
            var addresstype = Request.Form["AddressType"];
             entity.Contact = name.Trim();
            entity.Phone = phone.Trim();
            entity.Context = content.Trim();
            entity.PostZip = postZip.Trim();
            entity.AddressType = (AddressType)int.Parse(addresstype);
            entity.CreatorID = Erp.Current.ID;
            entity.RelationType = Underly.RelationType.Trade;
            entity.EnterpriseID = id.Trim();
            entity.District = district;
            entity.EnterError += Entity_EnterError;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }



        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Address;
            var applytask = new ApplyTask();
            applytask.MainID = entity.ID;
            applytask.MainType = Underly.MainType.Clients;
            applytask.ApplierID = entity.CreatorID;
            applytask.ApplyTaskType = Underly.ApplyTaskType.ClientAddress;
            applytask.Status = Underly.ApplyStatus.Waiting;
            applytask.Enter();


            LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增地址信息:{ entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        private void Entity_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("保存失败!", Web.Controls.Easyui.AutoSign.Error);
        }
    }
}