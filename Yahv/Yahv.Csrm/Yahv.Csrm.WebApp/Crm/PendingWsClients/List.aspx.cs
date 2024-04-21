using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.PendingWsClients
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            var query = Erp.Current.Whs.WsClients.Where(Predicate());
            return this.Paging(query.OrderBy(item => item.Enterprise.Name).ToArray().Select(item => new
            {
                item.ID,
                item.Enterprise.Name,
                item.Enterprise.AdminCode,
                item.Grade,
                item.Vip,
                item.Enterprise.District,
                item.WsClientStatus,
                StatusName = item.WsClientStatus.GetDescription(),
                Admin = item.Admin == null ? null : item.Admin.RealName,
                item.EnterCode,
                item.CustomsCode,
                item.Enterprise.Uscc,
                item.Enterprise.Corporation,
                item.Enterprise.RegAddress,
                item.CreateDate,
                item.UpdateDate
            }));
        }
        Expression<Func<WsClient, bool>> Predicate()
        {
            Expression<Func<WsClient, bool>> predicate = item => item.WsClientStatus == ApprovalStatus.Waitting; ;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }
            return predicate;
        }

        protected void del()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = Erp.Current.Whs.WsClients.Where(t => arry.Contains(t.ID));
        }
        protected void Approve()
        {
            var result = Request["status"];
            var ids = Request["id"].Split(',');
            var wsclients = Erp.Current.Whs.WsClients.Where(item => ids.Contains(item.ID));
            var Status = (ApprovalStatus)int.Parse(result);
            //wsclients.Approve(Status);
        }
    }
}