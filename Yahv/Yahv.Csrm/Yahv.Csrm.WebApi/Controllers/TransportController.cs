using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Yahv.Services;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class TransportController : ClientController
    {
        // GET: Transport
        public ActionResult Index([System.Web.Http.FromBody]Models.Transport model)
        {
            string code = "";
            #region 车辆信息
            if (model != null)
            {

                YaHv.Csrm.Services.Models.Origins.Transport transport = new YaHv.Csrm.Services.Models.Origins.Transport();
                //承运商是否存在
                var carriers = new YaHv.Csrm.Services.Views.Rolls.CarriersRoll().Where(item => item.Enterprise.Name == model.EnterpriseName);
                if (!carriers.Any())
                {
                    return eJson(new JMessage { code = 100, success = false, data = "承运商不存在" });
                }
                else
                {
                    var carrier = carriers.First();
                    code = carrier.Code;
                    transport.Enterprise = carrier.Enterprise;
                    transport.Type = model.Type;
                    transport.CarNumber1 = string.IsNullOrWhiteSpace(model.CarNumber1) ? "" : model.CarNumber1;
                    transport.CarNumber2 = model.CarNumber2;
                    transport.Weight = model.Weight;
                    if (carrier.Transports[transport.ID] != null)
                    {
                        return eJson(new JMessage { code = 300, success = true, data = "重名" });
                    }
                    else
                    {
                        transport.Status = GeneralStatus.Normal;
                        transport.CreatorID = model.Creator;
                        transport.EnterSuccess += Transport_EnterSuccess;
                        transport.Enter();
                    }
                }


            }
            #endregion

            #region 异步调用芯达通接口
            model.CarrierCode = code;
            var response = "";
            Task t1 = new Task(() =>
            {
                try
                {
                    response = Commons.HttpPostRaw(Commons.UnifyApiUrl + "/Carriers/TransportEnter", model.Json());
                    //eJson(response);
                }
                catch (Exception ex)
                {
                    eJson(new JMessage { code = 400, success = false, data = "XDT接口调用失败" + ex });
                }

            });
            t1.Start();
            Task.WaitAll(t1);
            #endregion
            return eJson();
        }
        private void Transport_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as YaHv.Csrm.Services.Models.Origins.Transport;
            eJson(new JMessage { code = 200, success = true, data = entity.ID });
        }

        private void Transport_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = "Transport删除成功" });
        }
    }
}