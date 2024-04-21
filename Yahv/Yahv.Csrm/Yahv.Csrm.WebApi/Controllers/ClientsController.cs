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
    public class Result
    {
        public string Code { set; get; }
        public object Data { set; get; }
    }
    public class ClientsController : ClientController
    {
        // GET: Clients
        [HttpGet]
        public ActionResult Index(string callback)
        {
            try
            {
                if (Erp.Current == null)
                {
                    return this.Jsonp(new Result { Code = "200", Data = null }, callback);
                }
                else
                {
                    var all = Erp.Current.Crm.MyClients.Where(item => item.ClientStatus == ApprovalStatus.Normal).ToArray().Select(item => new Yahv.Services.Models.Client
                    {
                        ID = item.ID,
                        Name = item.Enterprise.Name,
                        // District = item.Enterprise.District,
                        DyjCode = item.DyjCode,
                        AreaType = item.AreaType,
                        TaxperNumber = item.TaxperNumber,
                        Vip = (VIPLevel)item.Vip,
                        Status = item.ClientStatus,
                        Nature = item.Nature,
                        Grade = item.Grade
                    }).OrderBy(item => item.Name).Take(20);

                    return this.Jsonp(new Result { Code = "200", Data = all }, callback);
                }
            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }
        /// <summary>
        ///  客户根据公司名称搜索
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns> 
        [HttpGet]
        public ActionResult Getbyid(string id, string callback)
        {
            try
            {
                var single = new ClientsRoll().SingleOrDefault(item => item.Enterprise.ID == id);
                var client = new Yahv.Services.Models.Client
                {
                    ID = single.ID,
                    Name = single.Enterprise.Name,
                    // District = single.Enterprise.District,
                    DyjCode = single.DyjCode,
                    AreaType = single.AreaType,
                    TaxperNumber = single.TaxperNumber,
                    Vip = (VIPLevel)single.Vip,
                    Status = single.ClientStatus,
                    Nature = single.Nature,
                    Grade = single.Grade
                };
                return this.Jsonp(new Result { Code = "200", Data = client }, callback);
            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }
        /// <summary>
        ///  客户根据公司名称搜索
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Search(string name, string callback)
        {
            try
            {
                Expression<Func<TradingClient, bool>> predicate = item => item.ClientStatus == ApprovalStatus.Normal && item.Enterprise.Name.Contains(name);
                if (Erp.Current == null)
                {
                    Response.StatusCode = 500;
                    if (string.IsNullOrWhiteSpace(callback))
                    {
                        return this.Json(new { Code = "200", summary = "无登录客户" }, JsonRequestBehavior.AllowGet);
                    }

                    return this.Jsonp(new Result { Code = "200", Data = null }, callback);
                }
                else
                {
                    var all = Erp.Current.Crm.MyClients.Where(predicate).Select(item => new Yahv.Services.Models.Client
                    {
                        ID = item.ID,
                        Name = item.Enterprise.Name,
                        //District = item.Enterprise.District,
                        DyjCode = item.DyjCode,
                        AreaType = item.AreaType,
                        TaxperNumber = item.TaxperNumber,
                        Vip = item.Vip,
                        Status = item.ClientStatus,
                        Nature = item.Nature,
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
                                //District = item.District,
                                DyjCode = item.DyjCode,
                                AreaType = item.AreaType,
                                AreaTypeDes = item.AreaType.GetDescription(),
                                TaxperNumber = item.TaxperNumber,
                                Vip = item.Vip,
                                Status = item.Status,
                                Nature = item.Nature,
                                NatureDes = item.Nature.GetDescription(),
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

        }
        /// <summary>
        /// 客户联系人信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Contacts(string id, string callback)
        {
            try
            {
                var client = new TradingClientsRoll()[id].Enterprise;
                if (Erp.Current == null)
                {
                    return this.Jsonp(new Result { Code = "200", Data = null }, callback);
                }
                else if (client == null)
                {
                    return this.Jsonp(new Result { Code = "100", Data = null }, callback);
                }
                else
                {
                    var contacts = Erp.Current.Crm.MyContacts[client].Where(item => item.Status == Status.Normal).OrderBy(item => item.Name).ToArray().Select(item => new Yahv.Services.Models.Contact
                    {
                        ID = item.ID,
                        Name = item.Name,
                        Mobile = item.Mobile,
                        Tel = item.Tel,
                        Email = item.Email,
                        EnterpriseID = item.EnterpriseID,
                        Enterprise = new Yahv.Services.Models.Enterprise
                        {
                            Name = item.Enterprise.Name,
                            //District = item.Enterprise.District
                        },
                        Type = item.Type,
                        Status = (ApprovalStatus)item.Status
                    }).Take(20);

                    if (string.IsNullOrWhiteSpace(callback))
                    {
                        return this.Json(new Result
                        {
                            Code = "200",
                            Data = contacts.Select(item => new
                            {
                                ID = item.ID,
                                Name = item.Name,
                                Mobile = item.Mobile,
                                Tel = item.Tel,
                                Email = item.Email,
                                EnterpriseID = item.EnterpriseID,
                                EnterpriseName = item.Enterprise?.Name,
                                Type = item.Type,
                                TypeDes = item.Type.GetDescription(),
                                Status = item.Status
                            })
                        }, JsonRequestBehavior.AllowGet);
                    }

                    return this.Jsonp(new Result { Code = "200", Data = contacts }, callback);
                }

            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }
        /// <summary>
        /// 客户联系人搜索
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchContacts(string id, string name, string callback)
        {
            try
            {
                var client = new TradingClientsRoll()[id];
                if (Erp.Current == null)
                {
                    return this.Jsonp(new Result { Code = "200", Data = null }, callback);
                }
                else if (client == null)
                {
                    return this.Jsonp(new Result { Code = "100", Data = null }, callback);
                }
                else
                {
                    Expression<Func<TradingContact, bool>> predicate = item => item.Status == Status.Normal && (item.Name.Contains(name) || item.Tel.Contains(name) || item.Mobile.Contains(name));
                    var contacts = Erp.Current.Crm.MyContacts[client.Enterprise].Where(predicate).OrderBy(item => item.Name).ToArray().Select(item => new Yahv.Services.Models.Contact
                    {
                        ID = item.ID,
                        Name = item.Name,
                        Mobile = item.Mobile,
                        Tel = item.Tel,
                        Email = item.Email,
                        EnterpriseID = item.EnterpriseID,
                        Enterprise = new Yahv.Services.Models.Enterprise
                        {
                            Name = item.Enterprise.Name,
                            // District = item.Enterprise.District
                        },
                        Type = item.Type,
                        Status = (ApprovalStatus)item.Status
                    }).Take(20);
                    return this.Jsonp(new Result { Code = "200", Data = contacts }, callback);
                }

            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }
        /// <summary>
        /// 到货地址
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Consignees(string id, string callback)
        {
            try
            {
                var client = new TradingClientsRoll()[id];
                if (Erp.Current == null)
                {
                    return this.Jsonp(new Result { Code = "200", Data = null }, callback);
                }
                else if (client == null)
                {
                    return this.Jsonp(new Result { Code = "100", Data = null }, callback);
                }
                else
                {
                    var consignees = Erp.Current.Crm.MyConsignees[client.Enterprise].Where(item => item.Status == ApprovalStatus.Normal || item.Status == ApprovalStatus.Waitting).OrderBy(item => item.Name).ToArray().Select(item => new Yahv.Services.Models.Consignee
                    {
                        ID = item.ID,
                        EnterpriseID = item.Enterprise.ID,
                        Enterprise = new Yahv.Services.Models.Enterprise
                        {
                            Name = item.Enterprise.Name,
                            //District = item.Enterprise.District
                        },
                        Address = item.Address,
                        District = item.District,
                        DyjCode = item.DyjCode,
                        Name = item.Name,
                        Postzip = item.Postzip,
                        Mobile = item.Mobile,
                        Tel = item.Tel,
                        Email = item.Email,
                    });

                    if (string.IsNullOrWhiteSpace(callback))
                    {
                        return this.Json(new Result
                        {
                            Code = "200",
                            Data = consignees.Select(item => new
                            {
                                ID = item.ID,
                                EnterpriseID = item.Enterprise.ID,
                                EnterpriseName = item.Enterprise.Name,
                                Address = item.Address,
                                District = item.District,
                                DyjCode = item.DyjCode,
                                Name = item.Name,
                                Postzip = item.Postzip,
                                Mobile = item.Mobile,
                                Tel = item.Tel,
                                Email = item.Email,
                            })
                        }, JsonRequestBehavior.AllowGet);
                    }

                    return this.Jsonp(new Result { Code = "200", Data = consignees }, callback);
                }

            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }
        /// <summary>
        /// 发票
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Invoices(string id, string callback)
        {
            try
            {
                var client = new TradingClientsRoll()[id];
                if (client == null)
                {
                    return this.Jsonp(new Result { Code = "100", Data = null }, callback);
                }
                var invoices = client.Invoices.Where(item => item.Status == ApprovalStatus.Normal).
                    OrderBy(item => item.CreateDate).ToArray().Select(item => new Yahv.Services.Models.Invoice
                    {
                        ID = item.ID,
                        EnterpriseID = item.EnterpriseID,
                        Enterprise = new Yahv.Services.Models.Enterprise
                        {
                            Name = item.Enterprise.Name,
                            //District = item.Enterprise.District
                        },
                        Type = item.Type,
                        Account = item.Account,
                        Bank = item.Bank,
                        BankAddress = item.BankAddress,
                        TaxperNumber = item.TaxperNumber,
                        Name = item.Name,
                        Mobile = item.Mobile,
                        Tel = item.Tel,
                        Address = item.Address,
                        Postzip = item.Postzip,
                        District = item.District,
                        Email = item.Email,
                        Status = item.Status
                    });

                if (true)
                {
                    return this.Json(new Result
                    {
                        Code = "200",
                        Data = invoices.Select(item => new
                        {
                            ID = item.ID,
                            EnterpriseID = item.EnterpriseID,
                            EnterpriseName = item.Enterprise.Name,
                            Type = item.Type,
                            Account = item.Account,
                            Bank = item.Bank,
                            BankAddress = item.BankAddress,
                            TaxperNumber = item.TaxperNumber,
                            Name = item.Name,
                            Mobile = item.Mobile,
                            Tel = item.Tel,
                            Address = item.Address,
                            Postzip = item.Postzip,
                            District = item.District,
                            Email = item.Email,
                            Status = item.Status
                        })
                    }, JsonRequestBehavior.AllowGet);
                }


                return this.Jsonp(new Result { Code = "200", Data = invoices }, callback);
            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }
    }
}