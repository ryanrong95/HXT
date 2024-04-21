using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Prm.Payers
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ID = Request.QueryString["id"];
            }
        }
        protected object data()
        {
            var query = new CompaniesRoll()[Request.QueryString["id"]].Payers;
            return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
            {
                item.ID,
                RealName = item.RealEnterprise?.Name,
                Bank = item.Bank,
                BankAddress = item.BankAddress,
                Currency = item.Currency.GetDescription(),
                Methord = item.Methord.GetDescription(),
                Account = item.Account,
                SwiftCode = item.SwiftCode,
                Contact = item.Contact,
                Mobile = item.Mobile,
                Tel = item.Tel,
                Email = item.Email,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = item.Status,
                StatusName = item.Status.GetDescription(),
                Place = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Place).GetOrigin()?.ChineseName : null
            });
        }
        bool result = false;
        protected bool del()
        {
            var entity = new CompaniesRoll()[Request["id"]].Payers[Request["payerid"]];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
            return result;
        }

        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            result = true;
        }
        protected void enable()
        {
            var id = Request.Form["id"];
            string companyid = Request.Form["companyid"];
            var entity = Erp.Current.Crm.Companies[companyid].Payers[id];
            if (entity.Status == GeneralStatus.Deleted)
            {
                entity.Enable();
            }
        }
    }
}