using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Http;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    /// <summary>
    /// 费用接口
    /// </summary>
    public class ChargesController : Controller
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        virtual public ActionResult List(string notictID)
        {
            //原则上是一个订单的应收与应付都获取并union(本地)展示




            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// json费用配置
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        virtual public ActionResult Options(string notictID)
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        virtual public ActionResult Enter(JPost jpost)
        {
            //判断收支

            return View();
        }

        virtual public ActionResult Delete(string id)
        {
            //判断收支

            return View();
        }
    }

    /// <summary>
    /// 通知费用接口
    /// </summary>
    public class NoticeChargesController : ChargesController
    {
        public override ActionResult List(string id)
        {
            var charges = new Services.Views.ChargesView().Where(t => t.NoticeID == id);

            return Json(charges, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public override ActionResult Enter(JPost jpost)
        {
            try
            {
                var args = new
                {
                    NoticeID = jpost["NoticeID"]?.Value<string>(),
                    FormID = jpost["FormID"]?.Value<string>(),
                    AdminID = jpost["AdminID"]?.Value<string>(),
                    Type = jpost["Type"]?.Value<string>(),
                    Source = jpost["Source"]?.Value<string>(),
                    PayerID = jpost["PayerID"]?.Value<string>(),
                    PayeeID = jpost["PayeeID"]?.Value<string>(),
                    TakerID = jpost["TakerID"]?.Value<string>(),
                    Conduct = jpost["Conduct"]?.Value<string>(),
                    Subject = jpost["Subject"]?.Value<string>(),
                    Currency = jpost["Currency"]?.Value<string>(),
                    Quantity = jpost["Quantity"]?.Value<int>(),
                    UnitPrice = jpost["UnitPrice"]?.Value<decimal>(),
                    Total = jpost["Total"]?.Value<decimal>(),
                };
                
                //期号
                var CutDateIndex = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0');
                if (args.Type == ChargeType.Income.GetDescription())
                {
                    var payeeLeft = new Services.Models.PayeeLeft();
                    payeeLeft.Source = (AccountSource)Enum.Parse(typeof(AccountSource), args.Source);
                    payeeLeft.PayerID = args.PayerID;
                    payeeLeft.PayeeID = args.PayeeID;
                    payeeLeft.Conduct = (Conduct)Enum.Parse(typeof(Conduct), args.Conduct);
                    payeeLeft.Subject = args.Subject;
                    payeeLeft.Currency = (Currency)Enum.Parse(typeof(Currency), args.Currency);
                    payeeLeft.Quantity = (int)args.Quantity;
                    payeeLeft.UnitPrice = (decimal)args.UnitPrice;
                    payeeLeft.Total = (decimal)args.Total;
                    payeeLeft.NoticeID = args.NoticeID;
                    payeeLeft.FormID = args.FormID;
                    payeeLeft.Unit = "个";
                    payeeLeft.AdminID = args.AdminID;
                    payeeLeft.CutDateIndex = int.Parse(CutDateIndex);
                    
                    payeeLeft.Enter();

                    #region 同步数据到管理端应收表

                    string url = Services.Enums.FromType.SynPayeeLeft.GetDescription();
                    var result = ApiHelper.Current.JPost<object>(url, payeeLeft);
                    
                    #endregion
                }
                else if (args.Type == ChargeType.Pay.GetDescription())
                {
                    var payerLeft = new Services.Models.PayerLeft();
                    payerLeft.Source = (AccountSource)Enum.Parse(typeof(AccountSource), args.Source);
                    payerLeft.PayerID = args.PayerID;
                    payerLeft.PayeeID = args.PayeeID;
                    payerLeft.TakerID = args.TakerID;
                    payerLeft.Conduct = (Conduct)Enum.Parse(typeof(Conduct), args.Conduct);
                    payerLeft.Subject = args.Subject;
                    payerLeft.Currency = (Currency)Enum.Parse(typeof(Currency), args.Currency);
                    payerLeft.Quantity = (int)args.Quantity;
                    payerLeft.UnitPrice = (decimal)args.UnitPrice;
                    payerLeft.Total = (decimal)args.Total;
                    payerLeft.NoticeID = args.NoticeID;
                    payerLeft.FormID = args.FormID;
                    payerLeft.Unit = "个";
                    payerLeft.AdminID = args.AdminID;
                    payerLeft.CutDateIndex = int.Parse(CutDateIndex);

                    payerLeft.Enter();
                }
                else
                {
                    throw new Exception("费用类型错误");
                }

                Response.StatusCode = 200;
                return Json(new { Success = true, Data = string.Empty });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { Success = false, Data = ex.Message, });
            }
        }

        [HttpPost]
        public override ActionResult Delete(string id)
        {
            try
            {
                var charge = new Services.Views.ChargesView().Single(t => t.ID == id);
                //判断收支
                if (charge.Type == Services.Enums.ChargeType.Income)
                {
                    var payee = new Services.Views.PayeeLeftsView().Single(t => t.ID == id);
                    payee.Abandon();
                }
                else
                {
                    var payer = new Services.Views.PayerLeftsView().Single(t => t.ID == id);
                    payer.Abandon();
                }
                Response.StatusCode = 200;
                return Json(new { Success = true, Data = string.Empty });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { Success = false, Data = ex.Message, });
            }
        }
    }
}