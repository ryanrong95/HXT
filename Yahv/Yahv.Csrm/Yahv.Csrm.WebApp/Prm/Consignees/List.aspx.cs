using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Prm.Consignees
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Entity = new EnterprisesRoll()[Request.QueryString["id"]];
                Dictionary<string, string> statuslist = new Dictionary<string, string>() { { "0", "全部" } };
                statuslist.Add(ApprovalStatus.Normal.ToString(), ApprovalStatus.Normal.GetDescription());
                statuslist.Add(ApprovalStatus.Black.ToString(), ApprovalStatus.Black.GetDescription());
                statuslist.Add(ApprovalStatus.Closed.ToString(), ApprovalStatus.Closed.GetDescription());
                statuslist.Add(ApprovalStatus.Deleted.ToString(), ApprovalStatus.Deleted.GetDescription());
                //状态
                this.Model.Status = statuslist.Select(item => new
                {
                    value = item.Key,
                    text = item.Value
                });
            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            var query = new CompaniesRoll()[id].Consignees.Where(Predicate());
            return new
            {
                rows = query.OrderBy(item => item.CreateDate).ToArray().Select(item => new
                {
                    item.ID,
                    item.Address,
                    District = item.District.GetDescription(),
                    item.Postzip,
                    item.DyjCode,
                    ContactName = item.Name,
                    item.Tel,
                    item.Mobile,
                    item.Email,
                    item.Status,
                    StatusName = item.Status.GetDescription(),
                    EnterpriseID = item.EnterpriseID,
                    Admin = item.Creator == null ? null : item.Creator.RealName
                })
            };
        }
        Expression<Func<Consignee, bool>> Predicate()
        {
            Expression<Func<Consignee, bool>> predicate = item => true;
            var address = Request["address"];
            var contactname = Request["contactname"];
            var tel = Request["tel"];
            var status = Request["status"];
            if (!string.IsNullOrWhiteSpace(address))
            {
                predicate = predicate.And(item => item.Address.Contains(address));
            }
            if (!string.IsNullOrWhiteSpace(contactname))
            {
                predicate = predicate.And(item => item.Name.Contains(contactname));
            }
            if (!string.IsNullOrWhiteSpace(tel))
            {
                predicate = predicate.And(item => item.Tel.Contains(tel) || item.Mobile.Contains(tel));
            }
            ApprovalStatus approvalstatus;
            if (Enum.TryParse(status, out approvalstatus) && approvalstatus != 0)
            {
                predicate = predicate.And(item => item.Status == approvalstatus);
            }
            return predicate;
        }
        protected void del()
        {
            var arry = Request["items"].Split(',');
            var entity = new ConsigneesRoll().Where(t => arry.Contains(t.ID));
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "ConsigneeDelete", "删除到货地址：" + it.ID, "");
            }
            entity.Delete();
        }
        protected void Enable()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new ConsigneesRoll().Where(t => arry.Contains(t.ID));
            entity.Enable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "ConsigneeEnable", "启用到货地址：" + it.ID, "");
            }
        }
        protected void Unable()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new ConsigneesRoll().Where(t => arry.Contains(t.ID));
            entity.Unable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "ConsigneeUnable", "停用到货地址：" + it.ID, "");
            }
        }

    }
}