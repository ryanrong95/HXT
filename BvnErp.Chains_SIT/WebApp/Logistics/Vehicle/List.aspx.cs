using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
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

namespace WebApp.Logistics.Vehicle
{
    public partial class List : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            this.Model.CarrierData = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.Select(item => new
            {
                item.ID,
                item.Code,
                item.CarrierType,
                item.Name
            }).Where(x => x.CarrierType == CarrierType.DomesticLogistics || x.CarrierType == CarrierType.InteLogistics).OrderBy(x => x.Code).Json();

            this.Model.VehicleType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.VehicleType>().Select(item => new { item.Key, item.Value }).Json();
        }

        protected void data()
        {
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Vehicles.AsQueryable()
                 .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            string Carrier = Request.QueryString["Carrier"];
            string license = Request.QueryString["License"];
            string vehicleType = Request.QueryString["VehicleType"];

            //承运商
            if (!string.IsNullOrEmpty(Carrier))
            {
                list = list.Where(x => x.Carrier.Name.Contains(Carrier));
            }
            //车牌号
            if (!string.IsNullOrEmpty(license))
            {
                list = list.Where(item => item.License.Contains(license));
            }
            // 车辆类型
            if (!string.IsNullOrEmpty(vehicleType))
            {
                list = list.Where(x => x.VehicleType == (VehicleType)int.Parse(vehicleType));

            }
            Func<Needs.Ccs.Services.Models.Vehicle, object> convert = item => new
            {
                ID = item.ID,
                VehicleType = item.VehicleType.GetDescription(),
                CarrierName = item.Carrier.Name,
                License = item.License,
                HKLicense = item.HKLicense,
                CreateDate = item.CreateDate.ToShortDateString(),
                item.Weight,
                item.Size
            };
            this.Paging(list, convert);

        }

        /// <summary>
        /// 删除驾驶员
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Vehicles[id];

            if (entity != null)
            {
                if (!string.IsNullOrEmpty(URL))
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
                            Status = (int)entity.Carrier.Status,
                            Creator = ErmAdminID,
                            Type = (CRMCarrierType)dic.Values.ToArray()[0],
                            Place = dic.Keys.ToArray()[0],
                        },
                        Transport = new Needs.Ccs.Services.Models.ApiModels.Transport
                        {
                            EnterpriseName = entity.Carrier.Name,
                            Type = entity.VehicleType,
                            CarNumber1 = entity.License,
                            CarNumber2 = entity.HKLicense,
                            Weight = entity.Weight,
                            Status = 500,
                            Creator = ErmAdminID
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
                else
                {
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