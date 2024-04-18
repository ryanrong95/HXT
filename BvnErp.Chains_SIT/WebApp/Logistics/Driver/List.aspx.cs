using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models.ApiModels;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebApp.App_Utils;

namespace WebApp.Logistics.Driver
{
    public partial class List : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void data()
        {
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Drivers
               .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            string mobile = Request.QueryString["Mobile"];
            string name = Request.QueryString["Name"];
            string CarrierName = Request.QueryString["CarrierName"];
            if (!string.IsNullOrEmpty(mobile))
            {
                list = list.Where(item => item.Mobile.Contains(mobile));
            }

            if (!string.IsNullOrEmpty(name))
            {
                list = list.Where(item => item.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(CarrierName))
            {
                list = list.Where(item => item.Carrier.Name.Contains(CarrierName));
            }
            Func<Needs.Ccs.Services.Models.Driver, object> convert = item => new
            {
                item.ID,
                item.Name,
                item.Mobile,
                item.License,
                CreateDate = item.CreateDate.ToShortDateString(),
                CarrierName = item.Carrier.Name,
                item.HSCode,
                item.HKMobile,
                item.DriverCardNo,
                item.PortElecNo,
                item.LaoPaoCode


            };
            this.Paging(list, convert);

        }

        /// <summary>
        /// 删除驾驶员
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Drivers[id];
            if (entity != null)
            {
                if (string.IsNullOrEmpty(URL))
                {
                    entity.AbandonSuccess += Driver_AbandonSuccess;
                    entity.Abandon();
                }
                else
                {
                    string requestUrl = URL + "/Carrier/Enter";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址
                    var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                    var dic = Common.GetCarrierInfo(entity.Carrier.CarrierType);
                    var carrier = new CarrierModel()
                    {
                        Carrier = new Needs.Ccs.Services.Models.ApiModels.Carrier
                        {
                            Code = entity.Carrier.Code,
                            Name = entity.Carrier.Name,
                            Status = 200,
                            Creator=ErmAdminID,
                            Type = (CRMCarrierType)dic.Values.ToArray()[0],
                            Place = dic.Keys.ToArray()[0],

                        },
                        Driver = new Needs.Ccs.Services.Models.ApiModels.Driver
                        {
                            EnterpriseName = entity.Carrier.Name,
                            Name = entity.Name,
                            Mobile = entity.Mobile,
                            IDCard = entity.License,
                            Status = 500,
                            CustomsCode = entity.HSCode,
                            CardCode = entity.DriverCardNo,
                            Mobile2 = entity.HKMobile,
                            PortCode = entity.PortElecNo,
                            LBPassword = entity.LaoPaoCode,
                            Creator=ErmAdminID
                        }
                    };
                    string apiCarrier = JsonConvert.SerializeObject(carrier);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiCarrier);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求CRM接口失败：" }).Json());
                        return;
                    }
                    entity.AbandonSuccess += Driver_AbandonSuccess;
                    entity.Abandon();
                }
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Driver_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
    }
}