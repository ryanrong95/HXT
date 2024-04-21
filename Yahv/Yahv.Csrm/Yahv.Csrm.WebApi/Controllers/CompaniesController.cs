using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Web.Mvc;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class CompaniesController : ClientController
    {
        // GET: Companies
        public ActionResult Index(string callback)
        {
            var all = new CompaniesRoll().Where(item => item.CompanyStatus == ApprovalStatus.Normal).ToArray().Select(item => new Yahv.Services.Models.Company
            {
                ID = item.ID,
                Name = item.Enterprise.Name,
                Range = item.Range,
                Type = item.Type,
                //District = item.Enterprise.District,
                Status = item.CompanyStatus
            }).OrderBy(item => item.Name);

            return this.Jsonp(new Result { Code = "200", Data = all }, callback);
        }
        /// <summary>
        /// 内部公司根据公司名称搜索
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Search(string name, string callback)
        {
            Expression<Func<Company, bool>> predicate = item => item.CompanyStatus == ApprovalStatus.Normal && item.Enterprise.Name.Contains(name);
            var all = new CompaniesRoll().Where(predicate).ToArray().Select(item => new Yahv.Services.Models.Company
            {
                ID = item.ID,
                Name = item.Enterprise.Name,
                Range = item.Range,
                Type = item.Type,
                //District = item.Enterprise.District,
                Status = item.CompanyStatus
            }).OrderBy(item => item.Name).Take(20);

            if (string.IsNullOrWhiteSpace(callback))
            {
                return this.Json(new Result { Code = "200", Data = all }, JsonRequestBehavior.AllowGet);
            }

            return this.Jsonp(new Result { Code = "200", Data = all }, callback);
        }
        /// <summary>
        /// 内部公司受益人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Benneficiaries(string id, string callback)
        {
            try
            {
                var all = new CompaniesRoll()[id];
                if (all == null)
                {
                    return this.Jsonp(new Result { Code = "100", Data = null }, callback);
                }
                var entity = all.Beneficiaries.Where(item => item.Status == ApprovalStatus.Normal).ToArray().Select
                    (item => new Yahv.Services.Models.Beneficiary
                    {
                        ID = item.ID,
                        RealName = item.RealName,
                        Bank = item.Bank,
                        BankAddress = item.BankAddress,
                        Account = item.Account,
                        SwiftCode = item.SwiftCode,
                        Methord = item.Methord,
                        Currency = item.Currency,
                        District = item.District,
                        EnterpriseID = item.EnterpriseID,
                        InvoiceType = (InvoiceType)item.InvoiceType,
                        Name = item.Name,
                        Mobile = item.Mobile,
                        Tel = item.Tel,
                        Email = item.Email
                    });
                return this.Jsonp(new Result { Code = "200", Data = entity }, callback);
            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }


        /// <summary>
        /// 内部公司受益人
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Payee(string name, string callback)
        {
            //var all = new CompaniesRoll()[name];

            var company = new CompaniesRoll().SingleOrDefault(item => item.Enterprise.Name == name);

            if (company == null)
            {
                return this.Json(new Result { Code = "100", Data = new object[] { } }, JsonRequestBehavior.AllowGet);
            }

            var data = company.Beneficiaries.Where(item => item.Status == ApprovalStatus.Normal).ToArray().Select(item => new
            {
                ID = item.ID,
                RealName = item.RealName,
                Bank = item.Bank,
                BankAddress = item.BankAddress,
                Account = item.Account,
                SwiftCode = item.SwiftCode,
                Methord = item.Methord,
                Currency = item.Currency,
                District = item.District,


                MethordDes = item.Methord.GetDescription(),
                CurrencyDes = item.Currency.GetDescription(),
                DistrictDes = item.District.GetDescription(),

                EnterpriseID = item.EnterpriseID,
                InvoiceType = (InvoiceType)item.InvoiceType,
                Name = item.Name,
                Mobile = item.Mobile,
                Tel = item.Tel,
                Email = item.Email,
                BankAccount = $"{item.Bank} {item.Account}",
            });

            if (string.IsNullOrWhiteSpace(callback))
            {
                return this.Json(new Result { Code = "200", Data = data }, JsonRequestBehavior.AllowGet);
            }

            return this.Jsonp(new Result { Code = "200", Data = data }, callback);
        }
    }
}