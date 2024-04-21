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

namespace Yahv.Csrm.WebApp.Crm.Consignees
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
                //来源
                EnterpriseType type = (EnterpriseType)int.Parse(Request["enterprisetype"]);
                this.Model.EnterpriseType = type;
            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            var client = Erp.Current.Crm.Clients[id].Enterprise;
            ///传统贸易客户的到货地址数据
            var consignee = Erp.Current.Crm.MyConsignees[client].Where(Predicate()).OrderBy(item => item.CreateDate);

            return new
            {
                rows = consignee.ToArray().Select(item => new
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
                    Admin = item.Creator == null ? null : item.Creator.RealName,
                    item.Title,
                    item.PlateCode
                })
            };
        }
        Expression<Func<TradingConsignee, bool>> Predicate()
        {
            Expression<Func<TradingConsignee, bool>> predicate = item => true;
            ;
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
            string clientid = Request["clientid"];
            if (Erp.Current.IsSuper)
            {
                var consignees = new ConsigneesRoll().Where(t => arry.Contains(t.ID)).ToArray();
                consignees.Delete();
                foreach (var consignee in consignees)
                {
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                              nameof(Yahv.Systematic.Crm),
                                             "ConsigneeUpdate", "修改到货地址状态（删除），：" + consignee.ID, "");
                }
            }

            else
            {
                var client = Erp.Current.Crm.Clients[clientid].Enterprise;
                var consignees = Erp.Current.Crm.MyConsignees.Where(item => arry.Contains(item.ID));
                foreach (var it in consignees)
                {
                    it.Abandon();
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                               nameof(Yahv.Systematic.Crm),
                                              "ConsigneeDelete", "删除到货地址：" + it.ID + ",关系ID" + it.MapsID, "");
                }
            }




        }
        protected void Enable()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new ConsigneesRoll().Where(t => arry.Contains(t.ID)).ToArray();
            entity.Enable();
            //操作日志
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
            var entity = new ConsigneesRoll().Where(t => arry.Contains(t.ID)).ToArray();
            entity.Unable();
            //操作日志
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "ConsigneeUnable", "停用客户到货地址：" + it.ID, "");
            }
        }

    }
}