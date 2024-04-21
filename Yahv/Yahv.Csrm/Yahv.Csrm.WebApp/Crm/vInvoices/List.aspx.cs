using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.vInvoices
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.EnterpriseID = Request.QueryString["id"];
            }
        }
        protected object data()
        {
            string enterpirseid = Request.QueryString["id"];
            var query = new vInvoicesRoll(enterpirseid);
            return this.Paging(query.OrderBy(item => item.CreateDate).ToArray().Select(item => new
            {
                ID = item.ID,
                EnterpriseID = item.EnterpriseID,
                IsPersonal = item.IsPersonal,
                Type =item.Type.GetDescription(),
                Title = item.Title,
                TaxNumber = item.TaxNumber,
                RegAddress = item.RegAddress,
                Tel = item.Tel,
                BankName = item.BankName,
                BankAccount = item.BankAccount,
                PostAddress = item.PostAddress,
                PostRecipient = item.PostRecipient,
                PostTel = item.PostTel,
                item.PostZipCode,
                item.Status,
                DeliveryType = item.DeliveryType.GetDescription(),
                CreateDate = item.CreateDate,
                ModifyDate = item.CreateDate,
                CreatorID = item.CreatorID,
                IsDefault = item.IsDefault,
                RealName = item.CreatorRealName
            }));
        }
        bool delSuccess = false;
        protected bool del()
        {
            string id = Request["id"];
            var entity = new vInvoicesRoll()[id];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
            return delSuccess;
        }

        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            delSuccess = true;
        }
    }
}