using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.App_Utils;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class ClientController : BaseController
    {
        #region 页面

        /// <summary>
        /// 开票信息
        /// </summary>
        /// <returns></returns>
        public ActionResult InvoiceInfo() { return View(); }

        #endregion

        /// <summary>
        /// 获取开票信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMyInvoice()
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var returnModel = repository.ReadTable<Layers.Data.Sqls.PsOrder.Invoices>()
                                .Where(t => t.ClientID == theClientID)
                                .Select(item => new GetMyInvoiceReturnModel
                                {
                                    InvoiceID = item.ID,
                                    InvoiceName = item.Name,
                                    TaxNumber = item.TaxNumber,
                                    RegAddress = item.RegAddress,
                                    CompanyTel = item.Tel,
                                    BankName = item.BankName,
                                    BankAccount = item.BankAccount,
                                    DeliveryTypeInt = item.DeliveryType.ToString(),
                                    //DeliveryTypeName = 
                                    RevAddress = item.RevAddress,
                                    //RevAddressArray = 
                                    //RevAddressDetail = 
                                    Contact = item.Contact,
                                    Phone = item.Phone,
                                    Email = item.Email,
                                }).FirstOrDefault();

                if (returnModel != null)
                {
                    returnModel.DeliveryTypeName = ((Underly.InvoiceDeliveryType)(Convert.ToInt32(returnModel.DeliveryTypeInt))).GetDescription();
                    returnModel.RevAddressArray = returnModel.RevAddress?.ToAddress();
                    returnModel.RevAddressDetail = returnModel.RevAddress?.ToDetailAddress();
                }

                return Json(new { type = "success", msg = "", result = returnModel }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 开票信息提交
        /// </summary>
        /// <returns></returns>
        public JsonResult InvoiceInfoSubmit(InvoiceInfoSubmitModel model)
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            try
            {
                string addressValue = string.Join(" ", model.RevAddressArray?.Concat(new string[] { model.RevAddressDetail.Trim() }) ?? Array.Empty<string>());

                var theInvoice = new Layers.Data.Sqls.PsOrder.Invoices
                {
                    ClientID = theClientID,
                    Name = model.InvoiceName,
                    TaxNumber = model.TaxNumber,
                    RegAddress = model.RegAddress,
                    Tel = model.CompanyTel,
                    BankName = model.BankName,
                    BankAccount = model.BankAccount,
                    DeliveryType = model.DeliveryTypeInt,
                    RevAddress = addressValue,
                    Contact = model.Contact,
                    Phone = model.Phone,
                    Email = model.Email,
                };

                if (string.IsNullOrWhiteSpace(model.InvoiceID))
                {
                    using (PsOrderRepository repository = new PsOrderRepository())
                    {
                        theInvoice.ID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.Invoice);
                        theInvoice.CreateDate = DateTime.Now;
                        theInvoice.ModifyDate = DateTime.Now;
                        repository.Insert(theInvoice);
                    }
                }
                else
                {
                    using (PsOrderRepository repository = new PsOrderRepository())
                    {
                        theInvoice.ModifyDate = DateTime.Now;
                        repository.Update<Layers.Data.Sqls.PsOrder.Invoices>(new
                        {
                            ClientID = theClientID,
                            Name = model.InvoiceName,
                            TaxNumber = model.TaxNumber,
                            RegAddress = model.RegAddress,
                            Tel = model.CompanyTel,
                            BankName = model.BankName,
                            BankAccount = model.BankAccount,
                            DeliveryType = model.DeliveryTypeInt,
                            RevAddress = addressValue,
                            Contact = model.Contact,
                            Phone = model.Phone,
                            Email = model.Email,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                        }, item => item.ID == model.InvoiceID);
                    }
                }

                return Json(new { type = "success", msg = "提交成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}