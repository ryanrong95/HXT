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
    public class SuppliersController : ClientController
    {
        // GET: Suppliers
        [HttpGet]
        public ActionResult Index(string callback)
        {
            try
            {
                var all = new TradingSuppliersRoll().Where(item => item.SupplierStatus == ApprovalStatus.Normal).ToArray().Select(item => new
                {
                    ID = item.ID,
                    Name = item.Enterprise.Name,
                    AgentCompany = "addf",
                    DyjCode = item.DyjCode,
                    AreaType = item.AreaType,
                    InvoiceType = item.InvoiceType,
                    Nature = item.Nature,
                    TaxperNumber = item.TaxperNumber,
                    Type = item.Type,
                    District = item.Enterprise.District,
                    Grade = item.Grade,
                    IsFactory = item.IsFactory,
                    Status = item.SupplierStatus
                }).OrderBy(item => item.Name).Take(20);

                return this.Jsonp(new Result { Code = "200", Data = all }, callback);
            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }
        /// <summary>
        /// 供应商根据名称模糊搜索
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Search(string name, string callback)
        {
            try
            {
                Expression<Func<TradingSupplier, bool>> predicate = item => item.SupplierStatus == ApprovalStatus.Normal && item.Enterprise.Name.Contains(name);
                var all = new TradingSuppliersRoll().Where(predicate).Select(item => new
                {
                    ID = item.ID,
                    Name = item.Enterprise.Name,
                    AgentCompany = item.AgentCompany,
                    DyjCode = item.DyjCode,
                    AreaType = item.AreaType,
                    InvoiceType = item.InvoiceType,
                    Nature = item.Nature,
                    TaxperNumber = item.TaxperNumber,
                    Type = item.Type,
                    District = item.Enterprise.District,
                    Grade = item.Grade,
                    IsFactory = item.IsFactory,
                    Status = item.SupplierStatus
                }).OrderBy(item => item.Name).Take(20).ToArray();

                if (string.IsNullOrWhiteSpace(callback))
                {
                    return this.Json(new Result { Code = "200", Data = all }, JsonRequestBehavior.AllowGet);
                }

                return this.Jsonp(new Result { Code = "200", Data = all }, callback);
            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }
        /// <summary>
        /// 供应商受益人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Benneficiaries(string id, string callback)
        {
            try
            {
                var supplier = new TradingSuppliersRoll()[id];
                if (supplier == null)
                {
                    return this.Jsonp(new Result { Code = "100", Data = null }, callback);
                }

                var arry = supplier.Beneficiaries.Where(item => item.Status == ApprovalStatus.Normal).ToArray();

                var data = arry.Select
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


                if (string.IsNullOrWhiteSpace(callback))
                {
                    return this.Json(new Result { Code = "200", Data = data }, JsonRequestBehavior.AllowGet);
                }

                return this.Jsonp(new Result { Code = "200", Data = data }, callback);
            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }

        /// <summary>
        /// 供应商收款人
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Payee(string name, string callback)
        {
            try
            {
                var supplier = new TradingSuppliersRoll().SingleOrDefault(item => item.Enterprise.Name == name);
                if (supplier == null)
                {
                    return this.Jsonp(new Result { Code = "100", Data = null }, callback);
                }

                var arry = supplier.Beneficiaries.Where(item => item.Status == ApprovalStatus.Normal).ToArray();

                var data = arry.Select
                    (item => new //Yahv.Services.Models.Beneficiary
                    {
                        ID = item.ID,
                        RealName = item.RealName,
                        Bank = item.Bank,
                        BankAddress = item.BankAddress,
                        Account = item.Account,
                        SwiftCode = item.SwiftCode,
                        MethordDes = item.Methord.GetDescription(),
                        CurrencyDes = item.Currency.GetCurrency().ShortName,
                        DistrictDes = item.District.GetDescription(),
                        EnterpriseID = item.EnterpriseID,
                        //InvoiceType = (InvoiceType)item.InvoiceType,
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
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }

        #region  原厂供应商
        /// <summary>
        /// 原厂供应商
        /// </summary>
        /// <param name="key">供应商名称</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Factories(string key, string callback)
        {
            try
            {
                Expression<Func<TradingSupplier, bool>> predicate = item => item.SupplierStatus == ApprovalStatus.Normal && item.Enterprise.Name.Contains(key) && item.IsFactory;
                var data = new TradingSuppliersRoll().Where(predicate).Select(item => new
                {
                    ID = item.ID,
                    Name = item.Enterprise.Name,
                    Grade = item.Grade
                }).Take(20).ToArray();
                return this.Jsonp(new Result { Code = "200", Data = data }, callback);
            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }
        #endregion

        #region 货代供应商
        /// <summary>
        /// 货代供应商
        /// </summary>
        /// <param name="key">供应商名称</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Forwarders(string key, string callback)
        {
            try
            {
                Expression<Func<TradingSupplier, bool>> predicate = item => item.SupplierStatus == ApprovalStatus.Normal && item.Enterprise.Name.Contains(key) && item.IsForwarder;
                var all = new TradingSuppliersRoll().Where(predicate).Select(item => new
                {
                    ID = item.ID,
                    Name = item.Enterprise.Name,
                    Grade = item.Grade
                }).Take(20).ToArray();
                return this.Jsonp(new Result { Code = "200", Data = all }, callback);
            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }
        }
        #endregion

    }
}