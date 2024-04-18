using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// 与大赢家交互费用申请的接口
    /// </summary>
    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CostApplyController : ApiController
    {
        /// <summary>
        /// 总公司审批不通过，被调用接口，更新状态
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/costapply/headofficeapproverefuse")]
        public HttpResponseMessage HeadOfficeApproveRefuse([FromBody]ApiCostApplyParamModel.HeadOfficeApproveRefuseRequest param)
        {
            string newBatchID = Guid.NewGuid().ToString("N");
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                new Needs.Ccs.Services.Models.CostApplyApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = newBatchID,
                    Url = "HeadOfficeApproveRefuse",
                    RequestContent = JsonConvert.SerializeObject(param),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                }.Enter();

                var costApiHandler = new Needs.Ccs.Services.Models.CostApiHandler(param.BillNo, newBatchID);
                string rtnMsg = string.Empty;
                bool result = costApiHandler.HeadOfficeApproveRefuse(param.Summary, out rtnMsg);

                if (!result)
                {
                    mo.code = "-1";
                    mo.desc = rtnMsg;
                }
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }

            new Needs.Ccs.Services.Models.CostApplyApiLog()
            {
                ID = Guid.NewGuid().ToString("N"),
                BatchID = newBatchID,
                Url = "HeadOfficeApproveOk",
                ResponseContent = JsonConvert.SerializeObject(mo),
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            }.Enter();

            return ApiResultModel.OutputResult(mo);
        }

        /// <summary>
        /// 总公司审批通过后，付款成功，被调用接口，更新状态，并获取付款凭证
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/costapply/paymentsuccessnotice")]
        public HttpResponseMessage PaymentSuccessNotice([FromBody]ApiCostApplyParamModel.PaymentSuccessNoticeRequest param)
        {
            string newBatchID = Guid.NewGuid().ToString("N");
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {
                new Needs.Ccs.Services.Models.CostApplyApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = newBatchID,
                    Url = "PaymentSuccessNotice",
                    RequestContent = JsonConvert.SerializeObject(param),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                }.Enter();

                var costApiHandler = new Needs.Ccs.Services.Models.CostApiHandler(param.BillNo, newBatchID);
                string rtnMsg = string.Empty;
                bool result = costApiHandler.PaymentSuccessNotice(param.SeqNo, param.BankAccount, param.PaymentVoucherUrl, out rtnMsg);

                if (!result)
                {
                    mo.code = "-1";
                    mo.desc = rtnMsg;
                }
            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }

            new Needs.Ccs.Services.Models.CostApplyApiLog()
            {
                ID = Guid.NewGuid().ToString("N"),
                BatchID = newBatchID,
                Url = "PaymentSuccessNotice",
                ResponseContent = JsonConvert.SerializeObject(mo),
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            }.Enter();

            return ApiResultModel.OutputResult(mo);
        }

        /// <summary>
        /// 总公司审批通过后，付款失败，被调用接口，更新状态
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/costapply/paymentfailnotice")]
        public HttpResponseMessage PaymentFailNotice([FromBody]ApiCostApplyParamModel.PaymentFailNoticeRequest param)
        {
            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {

            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }
            return ApiResultModel.OutputResult(mo);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/costapply/testjson")]
        public HttpResponseMessage TestJson(string token)
        {
            //Collection<CookieHeaderValue> cookieValues = Request.Headers.GetCookies();

            


            ApiResultModel.BaseMeg mo = new ApiResultModel.BaseMeg();
            try
            {

            }
            catch (Exception ex)
            {
                mo.code = "-1";
                mo.desc = ex.Message;
            }
            //return ApiResultModel.OutputResult(mo);
            
            string r = "showData(" + JsonConvert.SerializeObject(mo) + ");";

            return new HttpResponseMessage { Content = new StringContent(r, Encoding.GetEncoding("UTF-8"), "application/json") };
        }

    }
}