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

namespace Yahv.Csrm.WebApp.Crm.Carriers.Beneficiaries
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                init();
                this.Model.CarrierID = Request.QueryString["id"];

            }
        }
        void init()
        {
            //状态
            Dictionary<string, string> statusdic = new Dictionary<string, string>() { { "0", "全部" } };
            statusdic.Add(ApprovalStatus.Normal.ToString(), ApprovalStatus.Normal.GetDescription());
            statusdic.Add(ApprovalStatus.Black.ToString(), ApprovalStatus.Black.GetDescription());
            statusdic.Add(ApprovalStatus.Closed.ToString(), ApprovalStatus.Closed.GetDescription());
            statusdic.Add(ApprovalStatus.Deleted.ToString(), ApprovalStatus.Deleted.GetDescription());
            //状态
            this.Model.Status = statusdic.Select(item => new
            {
                value = item.Key,
                text = item.Value
            });
            Dictionary<string, string> dic = new Dictionary<string, string>() { { "-100", "全部" } };
            //币种
            this.Model.Currency = dic.Concat(ExtendsEnum.ToDictionary<Currency>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
            //支付方式
            this.Model.Methord = dic.Concat(ExtendsEnum.ToDictionary<Methord>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
        }
        protected object data()
        {
            string carrierid = Request.QueryString["id"];
            string supplierid = Request.QueryString["supplierid"];
            var beneficiary = Erp.Current.Crm.Carriers[carrierid].Beneficiaries.Where(Predicate());
            return new
            {
                rows = beneficiary.ToArray().Select(item => new
                {
                    item.ID,
                    item.RealName,
                    item.Bank,
                    item.BankAddress,
                    item.SwiftCode,
                    item.Account,
                    District = item.District.GetDescription(),
                    Currency = item.Currency.GetDescription(),
                    Methord = item.Methord.GetDescription(),
                    Name = item.Name,
                    item.Mobile,
                    item.Tel,
                    item.Status,
                    StatusName = item.Status.GetDescription(),
                    TaxType = item.InvoiceType.GetDescription(),
                    Admin = item.Creator == null ? null : item.Creator.RealName,
                })
            };
        }
        Expression<Func<Beneficiary, bool>> Predicate()
        {
            Expression<Func<Beneficiary, bool>> predicate = item => true;
            var name = Request["name"];
            var method = Request["method"];
            var currency = Request["currency"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Bank.Contains(name));
            }

            if (method != "-100")
            {
                predicate = predicate.And(item => item.Methord == (Methord)int.Parse(method));
            }
            if (currency != "-100")
            {
                predicate = predicate.And(item => item.Currency == (Currency)int.Parse(currency));
            }

            return predicate;
        }
        protected void del()
        {
            var id = Request["benefitid"];
            string carrierid = Request["carrierid"];
            var beneficiary = Erp.Current.Crm.Carriers[carrierid].Beneficiaries[id];
            beneficiary.Abandon();

        }
        
    }
}