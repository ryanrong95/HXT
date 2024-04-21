using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.Invoices
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
                Dictionary<string, string> dic = new Dictionary<string, string>() { { "-100", "全部" } };
                this.Model.Type = dic.Concat(ExtendsEnum.ToDictionary<InvoiceType>()).Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value.ToString()
                });
                //来源
                EnterpriseType type = (EnterpriseType)int.Parse(Request["enterprisetype"]);
                this.Model.EnterpriseType = (int)type;
            }
        }
        protected object data()
        {
            string supplierid = Request.QueryString["id"];


            var invoices = Erp.Current.Srm.Suppliers[supplierid]?.Invoices.Where(Predicate()).OrderBy(item => item.CreateDate);
            return new
            {
                rows = invoices.ToArray().Select(item => new
                {
                    item.ID,
                    item.Bank,
                    item.BankAddress,
                    item.Account,
                    item.TaxperNumber,
                    item.Address,
                    District = item.District.GetDescription(),
                    Type = item.Type.GetDescription(),
                    item.Postzip,
                    ContactName = item.Name,
                    item.Tel,
                    item.Mobile,
                    item.Email,
                    item.Status,
                    StatusName = item.Status.GetDescription(),
                    Admin = item.Creator == null ? null : item.Creator.RealName
                })
            };
        }
        Expression<Func<TradingInvoice, bool>> Predicate()
        {
            Expression<Func<TradingInvoice, bool>> predicate = item => true;
            var name = Request["name"];
            var taxperNumber = Request["taxperNumber"];
            var type = Request["type"];
            var nature = Request["selNature"];
            var status = Request["status"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Bank.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(taxperNumber))
            {
                predicate = predicate.And(item => item.TaxperNumber.Contains(taxperNumber));
            }
            if (type != "-100")
            {
                predicate = predicate.And(item => item.Type == (InvoiceType)int.Parse(type));
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
            var supplierid = Request.Form["supplierid"];
            if (Erp.Current.IsSuper)
            {
                var entity = new SuppliersRoll()[supplierid].Invoices.Where(t => arry.Contains(t.ID));
                entity.Delete();//删除状态
                foreach (var it in entity)
                {
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                               nameof(Yahv.Systematic.Crm),
                                              "InvoiceUpdate", "修改供应商发票状态(删除)：" + it.ID, "");
                }
               
            }
            else {
                var invoices = Erp.Current.Srm.Suppliers[supplierid].Invoices;
                foreach (var invoice in invoices)
                {
                    invoice.Abandon();//删除关系
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                               nameof(Yahv.Systematic.Crm),
                                              "InvoiceDelete", "删除供应商发票：" + invoice.ID, "");
                }
            }
           
        }
        protected void Enable()
        {
            var arry = Request.Form["items"].Split(',');
            var id = Request.Form["supplierid"];
            var entity = new SuppliersRoll()[id].Invoices.Where(t => arry.Contains(t.ID));
            entity.Enable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "InvoiceEnable", "启用发票：" + it.ID, "");
            }
        }
        protected void Unable()
        {
            var arry = Request.Form["items"].Split(',');
            var id = Request.Form["supplierid"];
            var entity = new SuppliersRoll()[id].Invoices.Where(t => arry.Contains(t.ID));
            entity.Unable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "InvoiceUnable", "停用发票：" + it.ID, "");
            }
        }
    }
}