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
    public class DriverController : ClientController
    {
        // GET: Driver
        public ActionResult Index([System.Web.Http.FromBody]Models.Driver model)
        {
            string code = "";
            #region 司机 
            if (model != null)
            {
                YaHv.Csrm.Services.Models.Origins.Driver driver = new YaHv.Csrm.Services.Models.Origins.Driver();
                //承运商是否存在
                var carriers = new YaHv.Csrm.Services.Views.Rolls.CarriersRoll().Where(item => item.Enterprise.Name == model.EnterpriseName);
                if (!carriers.Any())
                {
                    return eJson(new JMessage { code = 100, success = false, data = "承运商不存在" });
                }
                else
                {
                    var carrier = carriers.First();
                    code = carrier.ID;
                    driver.Enterprise = carrier.Enterprise;
                    driver.Name = model.Name;
                    driver.IDCard = model.IDCard;
                    driver.Mobile = string.IsNullOrWhiteSpace(model.Mobile) ? "" : model.Mobile;
                    driver.Mobile2 = model.Mobile2;
                    driver.CustomsCode = model.CustomsCode;
                    driver.PortCode = model.PortCode;
                    driver.LBPassword = model.LBPassword;
                    driver.CardCode = model.CardCode;
                    driver.IsChcd = model.IsChcd;//是否中港贸易
                    driver.CreatorID = string.IsNullOrWhiteSpace(model.Creator) ? "" : model.Creator;
                    //司机是否存在
                    if (carrier.Drivers[driver.ID] != null)
                    {
                        return eJson(new JMessage { code = 100, success = false, data = "重名" });
                    }
                    else
                    {
                        driver.Status = GeneralStatus.Normal;
                        driver.EnterSuccess += Driver_EnterSuccess; ;
                        driver.Enter();
                    }
                }


            }
            #endregion

            #region 异步调用芯达通接口
            var response = "";
            model.CarrierCode = code;
            Task t1 = new Task(() =>
            {
                try
                {
                    response = Commons.HttpPostRaw(Commons.UnifyApiUrl + "/Carriers/DriverEnter", model.Json());
                    //eJson(response);
                }
                catch (Exception ex)
                {
                    eJson(new JMessage { code = 400, success = false, data = "XDT接口调用失败" + ex });
                }

            });
            t1.Start();
            Task.WaitAll(t1);
            var result = t1;

            #endregion
            return eJson();
        }

        private void Driver_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as YaHv.Csrm.Services.Models.Origins.Driver;
            eJson(new JMessage { code = 200, success = true, data = entity.ID });
        }

        //private void Driver_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        //{
        //    eJson(new JMessage { code = 200, success = true, data = "Crm删除成功" });
        //}
    }
}