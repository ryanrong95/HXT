using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Underly;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{

    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]

   
    public class PayExchangeApplyController : ApiController
    {
        /// <summary>
        /// 付汇申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/payExchangeapplys")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage PayExchangeApply([System.Web.Http.FromBody]PayExchangeRequest model)
        {
            try
            {
                var json = new JMessage();
                if (model == null)
                {

                    json.code = 100;
                    json.success = false;
                    json.data = "参数为空!";
                }
                string SupplierEnglishName = model.SupplierEnglishName;
                string BankAddress = model.BankAddress;



                var Orders = model.UnPayExchangeOrders.Select(item => new Needs.Ccs.Services.Models.UnPayExchangeOrder
                {

                    ID = item.OrderID,
                    Currency = model.Currency,
                    DeclarePrice = item.DeclarePrice,
                    PaidExchangeAmount = item.PaidExchangeAmount.ToRound(2),
                    CurrentPaidAmount = item.CurrentPaidAmount.ToRound(2)
                });

                var user = new UsersView().FirstOrDefault(x => x.ID == model.UserID);

                AdminPayExchangeApply payExchangeApply = new AdminPayExchangeApply(Orders);
              
                payExchangeApply.ClientID = model.ClientID;
                payExchangeApply.Currency = model.Currency;
                payExchangeApply.ExchangeRateType = (ExchangeRateType)model.ExchangeRateType;
                payExchangeApply.ExchangeRate = model.ExchangeRate;
                payExchangeApply.User = user;
                payExchangeApply.SupplierName = model.SupplierName;
                payExchangeApply.SupplierAddress = model.SupplierAddress;
                payExchangeApply.SupplierEnglishName = SupplierEnglishName.Replace("&#39", "'");
                payExchangeApply.BankName = model.BankName;
                payExchangeApply.BankAddress = BankAddress.Replace("&#39", "'");
                payExchangeApply.BankAccount = model.BankAccount;
                payExchangeApply.SwiftCode = model.SwiftCode;
                payExchangeApply.OtherInfo = model.OtherInfo;
                payExchangeApply.Summary = model.Summary;
                payExchangeApply.ABA = model.ABA;
                payExchangeApply.IBAN = model.IBAN;
                payExchangeApply.IsAdvanceMoney = model.IsAdvanceMoney;
                if (model.ExpectPayDate != null)
                {
                    payExchangeApply.ExpectPayDate = model.ExpectPayDate;
                }
                payExchangeApply.SettlemenDate = DateTime.Now.AddDays(90);
                payExchangeApply.PaymentType = model.PaymentType;

                int? handlingFeePayerType = null;
                if (int.TryParse(model.HandlingFeePayerType, out var handlingFeePayerType_1))
                {
                    handlingFeePayerType = handlingFeePayerType_1;
                }
                payExchangeApply.HandlingFeePayerType = handlingFeePayerType;
                payExchangeApply.HandlingFee = model.HandlingFee;
                payExchangeApply.USDRate = model.USDRate;

                payExchangeApply.Enter();
                // 记录日志
                var logs = new PayExchangeLog()
                {
                    PayExchangeApplyID = payExchangeApply.ID,
                    User = user,
                    PayExchangeApplyStatus = PayExchangeApplyStatus.Auditing,
                    Summary = "用户[" + user?.Name + "]提交了付汇申请",
                };
                logs.Enter();
                json.code = 200;
                json.success = true;
                json.data = payExchangeApply.ID;
                return ApiResultModel.OutputResult(json);
            }
            catch (Exception ex)
            {

                var json = new JMessage() { code = 300, success = false, data = "操作失败，" + ex.Message };
                return ApiResultModel.OutputResult(json);
            }

        }

        /// <summary>
        /// 删除付汇申请
        /// </summary>
        /// <param name="ID">申请ID</param>
        /// <param name="UserID">用户ID</param>
        /// <returns></returns>
        [Route("api/deleteapply")]
        [HttpDelete]
        public HttpResponseMessage DeletePayExchange(string ID, string UserID)
        {
            try
            {

                var json = new JMessage();
                if (string.IsNullOrWhiteSpace(ID))
                {

                    json.code = 100;
                    json.success = false;
                    json.data = "参数ID为空!";
                }
                var user = new Needs.Ccs.Services.Views.UsersView()[UserID];
                var apply = new Needs.Ccs.Services.Views.UserPayExchangeAppliesView()[ID];
                apply.User = user;
                apply.Abandon();
                json.code = 200;
                json.success = true;
                json.data = "删除成功";
                return ApiResultModel.OutputResult(json);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = "操作失败，" + ex.Message };
                return ApiResultModel.OutputResult(json);
            }


        }
    }
}

