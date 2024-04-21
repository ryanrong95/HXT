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

namespace Yahv.Csrm.WebApp.Srm.Beneficiaries
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                init();
                this.Model.Entity = new EnterprisesRoll()[Request.QueryString["id"]];
                //来源
                EnterpriseType type = (EnterpriseType)int.Parse(Request["enterprisetype"]);
                this.Model.EnterpriseType = (int)type;
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
            string id = Request.QueryString["id"];

            var beneficiary = Erp.Current.Srm.Suppliers[id]?.Beneficiaries.Where(Predicate()).OrderBy(item => item.CreateDate);


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
                    item.BankCode
                })
            };
        }
        Expression<Func<TradingBeneficiary, bool>> Predicate()
        {
            Expression<Func<TradingBeneficiary, bool>> predicate = item => true;
            var name = Request["name"];
            var method = Request["method"];
            var currency = Request["currency"];
            var status = Request["status"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Bank.Contains(name));
            }
            ApprovalStatus approvalstatus;
            if (Enum.TryParse(status, out approvalstatus) && approvalstatus != 0)
            {
                predicate = predicate.And(item => item.Status == approvalstatus);
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
            var arry = Request["items"].Split(',');
            var suplliereid = Request.Form["supplierid"];
            if (Erp.Current.IsSuper)
            {
                var entity = new BeneficiariesRoll().Where(t => arry.Contains(t.ID));
                entity.Delete();
                foreach (var it in entity)
                {
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                               nameof(Yahv.Systematic.Srm),
                                              "BeneficiaryUpdate", "修改受益人状态(删除)：" + it.ID, "");

                }
            }
            else
            {
                var suppliers = Erp.Current.Srm.Suppliers[suplliereid].Beneficiaries.Where(item => arry.Contains(item.ID));
                foreach (var supplier in suppliers)
                {
                    supplier.Abandon();//删除关系
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                               nameof(Yahv.Systematic.Srm),
                                              "BeneficiaryDelete", "删除供应商：" + supplier.ID, "");

                }
            }


        }

        protected void Enable()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new BeneficiariesRoll().Where(t => arry.Contains(t.ID));
            entity.Enable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                        nameof(Yahv.Systematic.Srm),
                                       "BeneficiaryEnable", "启用受益人：" + it.ID, "");
            }
        }
        protected void Unable()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new BeneficiariesRoll().Where(t => arry.Contains(t.ID));
            entity.Unable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                          nameof(Yahv.Systematic.Srm),
                                         "BeneficiaryUnable", "停用受益人：" + it.ID, "");
            }
        }
    }
}