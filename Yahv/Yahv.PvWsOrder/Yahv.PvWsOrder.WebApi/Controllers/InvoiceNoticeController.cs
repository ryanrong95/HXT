using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.PvWsOrder.WebApi.Controllers
{
    public class InvoiceNoticeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        /// <summary>
        /// 深圳代仓储账单开票申请接口
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult SzInvoiceEnter(JPost obj)
        {
            try
            {
                Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
                {
                    MainID = string.Empty,
                    Operation = "开票申请接口",
                    Summary = obj.Json(),
                });
                var sznotice = obj.ToObject<SZInvoiceNotice>();

                using (var Reponsitory = new PvWsOrderReponsitory())
                {
                    var ID = Layers.Data.PKeySigner.Pick(PKeyType.invoiceNotice);
                    Reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.InvoiceNotices()
                    {
                        ID = ID,
                        ClientID = sznotice.ClientID,
                        Title = sznotice.Title,
                        TaxNumber = sznotice.TaxNumber,
                        RegAddress = sznotice.RegAddress,
                        Tel = sznotice.Tel,
                        BankName = sznotice.BankName,
                        BankAccount = sznotice.BankAccount,
                        PostAddress = sznotice.PostAddress,
                        PostRecipient = sznotice.PostRecipient,
                        PostTel = sznotice.PostTel,
                        AdminID = sznotice.AdminID,
                        DeliveryType = (int)sznotice.DeliveryType,

                        IsPersonal = false,
                        FromType = (int)Services.Enums.InvoiceFromType.SZStore,
                        Type = (int)InvoiceType.None,                       
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)Services.Enums.InvoiceEnum.UnInvoiced,
                    });

                    var itemID = Layers.Data.PKeySigner.Pick(PKeyType.invoiceNoticeItem);
                    Reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.InvoiceNoticeItems()
                    {
                        ID = itemID,
                        InvoiceNoticeID = ID,
                        BillID = sznotice.VourcherID,
                        UnitPrice = sznotice.Price,
                        Quantity = 1,
                        Amount = sznotice.Price,
                        Difference = sznotice.Difference,
                        Status = (int)GeneralStatus.Normal,
                        CreateDate = DateTime.Now,
                    });
                }
                //返回信息
                var json = new JMessage() { code = 200, success = true, data = "" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //错误日志记录
                Services.Logger.Error("NPC", "深圳开票通知接口" + ex.Message);
                var json = new JMessage() { code = 500, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
    }

    public class SZInvoiceNotice
    {
        public string ClientID { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxNumber { set; get; }
        /// <summary>
        /// 企业注册地址
        /// </summary>
        public string RegAddress { set; get; }
        /// <summary>
        /// 企业电话
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { set; get; }
        /// <summary>
        /// 收票地址
        /// </summary>
        public string PostAddress { get; set; }
        /// <summary>
        /// 收票人
        /// </summary>
        public string PostRecipient { set; get; }
        /// <summary>
        /// 收票人联系电话
        /// </summary>
        public string PostTel { set; get; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string AdminID { get; set; }
        /// <summary>
        /// 开票金额
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 开票差额
        /// </summary>
        public decimal Difference { get; set; }
        /// <summary>
        /// 账单ID
        /// </summary>
        public string VourcherID { get; set; }
        /// <summary>
        /// 邮寄类型
        /// </summary>
        public InvoiceDeliveryType DeliveryType { get; set; }

    }
}
