using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using YaHv.Csrm.Services.Extends;

namespace Yahv.Csrm.WebApp.Crm.Payers
{
    public partial class List : BasePage
    {
        bool result = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ID = Request.QueryString["id"];
            }
        }
        protected object data()
        {
            var query = Erp.Current.Whs.MyWsClients[Request.QueryString["id"]].Payers;
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
                Place = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Place).GetOrigin()?.ChineseName : null,
            });
        }

        protected bool del()
        {
            var entity = Erp.Current.Whs.MyWsClients[Request["id"]].Payers[Request["payerid"]];
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
            string clientid = Request.Form["clientid"];
            var entity = Erp.Current.Whs.MyWsClients[clientid].Payers[id];
            if (entity.Status == GeneralStatus.Deleted)
            {
                entity.Enable();
            }
        }
    }
}