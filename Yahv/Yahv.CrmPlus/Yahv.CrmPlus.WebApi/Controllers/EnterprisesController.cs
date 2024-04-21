using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Yahv;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.CrmPlus.WebApi.Controllers
{
    public class EnterprisesController : ClientController
    {
        // GET: Enterprises
        public ActionResult Index()
        {
            return View();
        }
        #region 我的客户

        public class Result

        {
            public string Code
            {
                get; set;
            }

            public object Data
            {
                get; set;
            }

        }
        //
        /// <summary>
        /// 我的客户
        /// </summary>
        /// <param name="name">客户名称</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MyClients(string name, string callback)
        {
            try
            {
                Expression<Func<Client, bool>> predicate = item => (item.Name.Contains(name) || item.ID == name);
                if (Erp.Current == null)
                {
                    Response.StatusCode = 500;
                    if (string.IsNullOrWhiteSpace(callback))
                    {
                        return this.Json(new { Code = "200", summary = "无登录客户" }, JsonRequestBehavior.AllowGet);
                    }

                    return this.Jsonp(new Result { Code = "200", Data = null, }, callback);
                }
                else
                {
                    var all = Erp.Current.CrmPlus.MyClients.Where(predicate).Select(item => new Client
                    {
                        ID = item.ID,
                        Name = item.Name,
                        //Vip = item.Vip,
                        Grade = item.Grade,
                    }).OrderBy(item => item.Name).Take(20).ToArray();

                    if (string.IsNullOrWhiteSpace(callback))
                    {
                        return this.Json(new Result
                        {
                            Code = "200",
                            Data = all.Select(item => new
                            {
                                ID = item.ID,
                                Name = item.Name,
                                Grade = item.Grade
                            }).ToArray()
                        }, JsonRequestBehavior.AllowGet);
                    }

                    return this.Jsonp(new Result { Code = "200", Data = all }, callback);
                }

            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }


            //var clients = Erp.Current.CrmPlus.MyClients.Where(item => !item.IsDraft && item.Name.Contains(key)||item.ID==key);

            //return Json(clients.Select(item => new
            //{
            //    ID = item.ID,
            //    Name = item.Name,
            //}).Take(20), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 供应商
        /// <summary>
        /// 供应商
        /// </summary>
        /// <param name="clientid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Suppliers(string key)
        {
            var suppliers = new SuppliersRoll().Where(item => !item.IsDraft && item.Name.Contains(key) || item.ID == key);
            return Json(suppliers.Select(item => new
            {
                ID = item.ID,
                Name = item.Name,
                Grade = item.SupplierGrade
            }).Take(20), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 内部公司
        /// <summary>
        /// 内部公司
        /// </summary>
        /// <param name="name">内部公司名称</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Companies(string key)
        {
            var companies = new CompaniesRoll().Where(item => item.Name.Contains(key) || item.ID == key && item.CompanyStatus == DataStatus.Normal);
            return Json(companies.Select(item => new
            {
                ID = item.ID,
                Name = item.Name,
            }).Take(20), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 企业联系人
        /// <summary>
        /// 企业联系人
        /// </summary>
        /// <param name="clientid">企业ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Contacts(string enterpriseID)
        {
            //Pm 和FAE可以看到所有联系人
            if (Erp.Current.Role.ID == FixedRole.PM.GetFixedID() || Erp.Current.Role.ID == FixedRole.FAE.GetFixedID())
            {
                var contacts = new ContactsRoll(enterpriseID, RelationType.Trade);
                Response.StatusCode = 200;
                return Json(contacts.Select(item => new
                {
                    ID = item.ID,
                    Name = item.Name,
                    Mobile = item.Mobile,
                    Tel = item.Tel,
                    Email = item.Email
                }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var contacts = Erp.Current.CrmPlus.MyContacts.Where(item => item.EnterpriseID == enterpriseID && item.Status == Underly.DataStatus.Normal);
                Response.StatusCode = 200;
                return Json(contacts.Select(item => new
                {
                    ID = item.ID,
                    Name = item.Name,
                    Mobile = item.Mobile,
                    Tel = item.Tel,
                    Email = item.Email
                }), JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region 企业收件地址
        /// <summary>
        /// 企业收件地址
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Consignees(string enterpriseID)
        {
            var consignees = Erp.Current.CrmPlus.Addresses.Where(item => item.EnterpriseID == enterpriseID && item.AddressType == Underly.AddressType.Consignee && item.Status == Underly.AuditStatus.Normal);
            return Json(consignees.Select(item => new
            {
                ID = item.ID,
                Address = item.Context,
                Phone = item.Phone,
                Contact = item.Contact
            }), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 企业发票
        /// <summary>
        /// 企业发票
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Invoices(string enterpriseID)
        {
            var invoices = Erp.Current.CrmPlus.Invoices.Where(item => item.EnterpriseID == enterpriseID && item.Status == Underly.DataStatus.Normal);
            return Json(invoices.Select(item => new
            {
                ID = item.ID,
                item.Address,
                item.Bank,
                item.Account
            }), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 企业银行账号
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">账号类型</param>
        /// <param name="enterpriseID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BookAccount(string type, string enterpriseID)
        {
            BookAccountType accounttype;
            if (!Enum.TryParse(type, out accounttype))
            {
                throw new NullReferenceException();
            }
            var accounts = Erp.Current.CrmPlus.BookAccounts.Where(item => item.BookAccountType == accounttype);
            return Json(accounts.Select(item => new
            {
                item.ID,
                item.Account,
                item.Bank,
                item.BankCode,
                Methord = item.BookAccountMethord.GetDescription()
            }).Take(20), JsonRequestBehavior.AllowGet);
        }
        #endregion


    }



}