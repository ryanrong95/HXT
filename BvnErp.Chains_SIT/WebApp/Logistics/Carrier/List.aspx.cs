using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models.ApiModels;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Logistics.Carrier
{
    public partial class List : Uc.PageBase
    {
         private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.CarrierTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CarrierType>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();
        }

        protected void data()
        {
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.AsQueryable();
            list = list.Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            string code = Request.QueryString["Code"];
            string name = Request.QueryString["Name"];
            string carriertype = Request.QueryString["CarrierType"];

            if (!string.IsNullOrEmpty(code))
            {
                list = list.Where(item => item.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(name))
            {
                list = list.Where(item => item.Name.Contains(name));
            }

            // 获取承运商类型
            if (!string.IsNullOrEmpty(carriertype))
            {
                list = list.Where(x => x.CarrierType == (CarrierType)int.Parse(carriertype));

            }

            Func<Needs.Ccs.Services.Models.Carrier, object> convert = carrier => new
            {
                ID=carrier.ID,
                Code = carrier.Code,
                Name = carrier.Name,
                carrier.QueryMark,
                CarrierType = carrier.CarrierType.GetDescription(),
                ContactName = carrier.Contact?.Name,
                ContactMobile = carrier.Contact?.Mobile,
                Summary = carrier.Summary,
                CreateDate = carrier.CreateDate.ToShortDateString(),
                carrier.Address,
                carrier.Contact?.Fax
            };
            this.Paging(list,convert);
        }

        /// <summary>
        /// 删除承运商
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers[id];

            if (entity != null)
            {
                if (string.IsNullOrEmpty(URL))
                {
                    entity.AbandonSuccess += Carrier_AbandonSuccess;
                    entity.Abandon();
                }
                else {

                    string requestUrl = URL + "/Carrier/Enter";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址
                    var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                    var dic = Common.GetCarrierInfo(entity.CarrierType);
                    var carrier = new CarrierModel()
                    {
                        Carrier = new Needs.Ccs.Services.Models.ApiModels.Carrier
                        {
                            Code = entity.Code,
                            Name = entity.Name,
                            Summary = entity.Summary,
                            Status =500,
                            Creator=ErmAdminID,
                            Type = (CRMCarrierType)dic.Values.ToArray()[0],
                            Place = dic.Keys.ToArray()[0],
                        },
                    };
                    string apiCarrier = JsonConvert.SerializeObject(carrier);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiCarrier);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求CRM接口失败：" }).Json());
                        return;
                    }
                    entity.AbandonSuccess += Carrier_AbandonSuccess;
                    entity.Abandon();
                }
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Carrier_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
    }
}