using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Granule
{
    public partial class Invoices : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Client = new TradingClientsRoll()[Request.QueryString["id"]];
                //状态
                this.Model.Status = ExtendsEnum.ToArray(ApprovalStatus.Waitting, ApprovalStatus.Voted).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.Type = ExtendsEnum.ToArray(InvoiceType.Unkonwn).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            var query = new TradingClientsRoll()[id].Invoices.Where(Predicate());
            return this.Paging(query.OrderBy(item => item.CreateDate).ToArray().Select(item => new
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
                Admin = item.Creator.RealName
            }));
        }
        Expression<Func<TradingInvoice, bool>> Predicate()
        {
            Expression<Func<TradingInvoice, bool>> predicate = item => true;
            var name = Request["name"];
            var taxperNumber = Request["taxperNumber"];
            var type = Request["type"];
            var nature = Request["selNature"];
            //var status = Request["status"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Bank.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(taxperNumber))
            {
                predicate = predicate.And(item => item.TaxperNumber.Contains(taxperNumber));
            }
            if (!string.IsNullOrWhiteSpace(type))
            {
                predicate = predicate.And(item => item.Type == (InvoiceType)int.Parse(type));
            }
            //if (!string.IsNullOrWhiteSpace(status))
            //{
            //    predicate = predicate.And(item => item.Status == (ApprovalStatus)int.Parse(status));
            //}
            return predicate;
        }
    }
}