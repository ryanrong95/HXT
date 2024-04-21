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

namespace Yahv.Csrm.WebApp.Srm.Granule
{
    public partial class Beneficiaries : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Supplier = new SuppliersRoll()[Request.QueryString["id"]];
                this.Model.Admin = new AdminsAllRoll()[Request["adminid"]];
                //币种
                this.Model.Currency = ExtendsEnum.ToArray(Currency.Unknown).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //支付方式
                this.Model.Methord = ExtendsEnum.ToArray<Methord>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            var mybenefits = Erp.Current.Srm.Admins[Request["adminid"]].Beneficiaries.Where(item => item.EnterpriseID == id).Select(item => item.ID).ToArray();
            var query = new TradingSuppliersRoll()[id].Beneficiaries.Where(Predicate());
            return this.Paging(query.OrderBy(item => item.CreateDate).ToArray().Select(item => new
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
                Admin = item.Creator.RealName,
                IsChecked = mybenefits.Contains(item.ID)
            }));
        }
        Expression<Func<TradingBeneficiary, bool>> Predicate()
        {
            Expression<Func<TradingBeneficiary, bool>> predicate = item => true;
            var name = Request["name"];
            var method = Request["method"];
            var currency = Request["currency"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Bank.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(method))
            {
                predicate = predicate.And(item => item.Methord == (Methord)int.Parse(method));
            }
            if (!string.IsNullOrWhiteSpace(currency))
            {
                predicate = predicate.And(item => item.Currency == (Currency)int.Parse(currency));
            }
            //if (!string.IsNullOrWhiteSpace(status))
            //{
            //    predicate = predicate.And(item => item.Status == (ApprovalStatus)int.Parse(status));
            //}
            return predicate;
        }

        /// <summary>
        /// 绑定
        /// </summary>
        protected JMessage Bind()
        {
            var benefitid = Request["benefitid"];
            var adminid = Request["adminid"];
            string supplierid = Request["supplierid"];
            if (string.IsNullOrWhiteSpace(benefitid) || string.IsNullOrWhiteSpace(adminid))
            {
                return new JMessage
                {
                    success = false,
                    code = 100,
                    data = "绑定失败"
                };
            }
            else
            {
                Erp.Current.Crm.Admins[adminid].Binding(supplierid, benefitid, MapsType.Beneficiary);
                return new JMessage
                {
                    success = true,
                    code = 200,
                    data = "绑定成功"
                };
            }

        }
        /// <summary>
        /// 解绑
        /// </summary>
        protected JMessage UnBind()
        {
            var benefitid = Request["benefitid"];
            var adminid = Request["adminid"];
            if (string.IsNullOrWhiteSpace(benefitid) || string.IsNullOrWhiteSpace(adminid))
            {
                return new JMessage
                {
                    success = false,
                    code = 100,
                    data = "绑定失败"
                };
            }
            else
            {
                Erp.Current.Crm.Admins[adminid].Unbind(benefitid, MapsType.Beneficiary);
                return new JMessage
                {
                    success = true,
                    code = 200,
                    data = "操作成功"
                };
            }

        }
    }
}