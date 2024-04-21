using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Csrm.WebApi.Models;
using Yahv.Payments;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class CiiCloudController : Controller
    {
        // GET: CiiCloud
        public ActionResult Index()
        {
            return View();
        }
        #region 获取客户基本信息和工商信息
        [HttpGet]
        public ActionResult getClient(string id)
        {
            var client = new WsClientsRoll()[id];
            if (client == null)
            {
                return Json(new { success = false, data = "不存在" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ID = client.ID,
                    Name = client.Enterprise.Name,
                    Uscc = client.Enterprise.Uscc,
                    RegAddress = client.Enterprise.RegAddress,
                    Corporation = client.Enterprise.Corporation
                    //登录名，手机号，邮箱？
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 客户注册
        //客户是否存在
        //草稿是否存在，是否要完善客户基本信息和工商信息
        //客户和草稿都不存在，录入草稿

        #endregion

        #region 获取客户发票
        [HttpGet]
        public ActionResult getInvoice(string id)
        {
            var client = new WsClientsRoll()[id];
            if (client == null)
            {
                return Json(new { success = false, data = "客户不存在" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var invoice = client.Invoice;
                return Json(new
                {

                    ID = invoice.ID,
                    Title = client.Enterprise.Name,
                    TaxperNumber = invoice.TaxperNumber,
                    RegAddress = client.Enterprise.RegAddress,
                    Type = invoice.Type,
                    Bank = invoice.Bank,
                    BankAddress = invoice.BankAddress,
                    Account = invoice.Account,
                    DeliveryType = (int)invoice.DeliveryType,
                    ContactName = invoice.Name,
                    Tel = invoice.Tel,
                    Mobile = invoice.Mobile,
                    Email = invoice.Email,
                    Address = invoice.Address
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 添加/修改客户发票


        #endregion

        #region 获取客户收货地址
        [HttpGet]
        public ActionResult getConsignees(string id)
        {
            var client = new WsClientsRoll()[id];
            if (client == null)
            {
                return Json(new { success = false, data = "客户不存在" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(client.Consignees.Select(item => new
                {
                    item.ID,
                    ReceiveUnit = item.Title,
                    item.Place,
                    item.Address,
                    item.Name,
                    item.Tel,
                    item.Mobile,
                    item.Email
                }), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 添加/修改客户收货地址


        #endregion

        #region 删除客户收货地址


        #endregion

        #region 获取供应商
        [HttpGet]
        public ActionResult getSuppliers(string id)
        {
            var client = new WsClientsRoll()[id];
            if (client == null)
            {
                return Json(new { success = false, data = "客户不存在" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(client.nSuppliers.Where(item => item.Status == GeneralStatus.Normal).Select(item => new
                {
                    item.ID,
                    item.ChineseName,
                    item.EnglishName,
                    item.CHNabbreviation,
                    item.Enterprise.RegAddress,
                    item.Enterprise.Place
                }), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 添加/修改供应商

        #endregion

        #region 获取供应商联系人
        [HttpGet]
        public ActionResult getSupplierContacts(string clientid, string supplierid)
        {
            var client = new WsClientsRoll()[clientid];
            if (client == null)
            {
                return Json(new { success = false, data = "客户不存在" }, JsonRequestBehavior.AllowGet);
            }
            else if (client.nSuppliers[supplierid] == null)
            {
                return Json(new { success = false, data = "供应商不存在" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(client.nSuppliers[supplierid].nContacts.Where(item => item.Status == GeneralStatus.Normal).Select(item => new
                {
                    item.ID,
                    item.Name,
                    item.Tel,
                    item.Mobile,
                    item.Email,
                    item.QQ,
                    item.Status
                }), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 添加/修改供应商联系人

        #endregion

        #region 获取供应商提货地址
        [HttpGet]
        public ActionResult getSupplierConsignors(string clientid, string supplierid)
        {
            var client = new WsClientsRoll()[clientid];
            if (client == null)
            {
                return Json(new { success = false, data = "客户不存在" }, JsonRequestBehavior.AllowGet);
            }
            else if (client.nSuppliers[supplierid] == null)
            {
                return Json(new { success = false, data = "供应商不存在" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(client.nSuppliers[supplierid].nConsignors.Where(item => item.Status == GeneralStatus.Normal).Select(item => new
                {
                    item.ID,
                    item.Address,
                    item.Contact,
                    item.Tel,
                    item.Mobile,
                    item.Email,
                    item.Postzip
                }), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 添加/修改供应商提货地址

        #endregion

        #region 获取供应商银行
        [HttpGet]
        public ActionResult getSupplierPayees(string clientid, string supplierid)
        {
            var client = new WsClientsRoll()[clientid];
            if (client == null)
            {
                return Json(new { success = false, data = "客户不存在" }, JsonRequestBehavior.AllowGet);
            }
            else if (client.nSuppliers[supplierid] == null)
            {
                return Json(new { success = false, data = "供应商不存在" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(client.nSuppliers[supplierid].nPayees.Where(item => item.Status == GeneralStatus.Normal).Select(item => new
                {
                    item.ID,
                    item.Bank,
                    item.Account,
                    item.Place,
                    item.SwiftCode,
                    item.BankAddress,
                    item.Currency,
                    RealName = item.RealEnterprise.Name,
                    item.Contact,
                    item.Tel,
                    item.Mobile,
                    item.Email,
                }), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 添加/修改供应商银行

        #endregion

        #region 获取服务协议
        [HttpGet]
        public ActionResult getContract()
        {
            return View();
        }
        #endregion

        #region 添加服务协议

        #endregion
    }
}