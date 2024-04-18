using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models.ApiModels;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebApp.App_Utils;

namespace WebApp.Logistics.Vehicle
{
    public partial class Edit : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化基本信息
        /// </summary>
        protected void LoadData()
        {
            string driverID = Request.QueryString["ID"];

            //this.Model.CarrierData = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.Where(x => x.CarrierType == CarrierType.DomesticLogistics || x.CarrierType == CarrierType.InteLogistics).Select(item => new
            //{
            //    item.ID,
            //    item.Code,
            //    item.CarrierType,
            //    item.Name
            //}).OrderBy(x => x.Code).Json();
            this.Model.CarrierData = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.Where(x => (x.CarrierType == CarrierType.DomesticLogistics || x.CarrierType == CarrierType.InteLogistics) && x.Status == Status.Normal).Select(item => new
            {
                value = item.ID,
                text = item.Name,
                Type = item.CarrierType
            }).Json();
            if (!string.IsNullOrWhiteSpace(driverID))
            {
                var vehicle = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Vehicles[driverID];

                this.Model.VehiclesInfo = new
                {
                    vehicle.ID,
                    CarrierID = vehicle.Carrier.ID,
                    Type = vehicle.Carrier.CarrierType,
                    vehicle.License,
                    vehicle.VehicleType,
                    vehicle.HKLicense,
                    vehicle.Weight,
                    vehicle.Size
                }.Json();
            }
            else
            {
                this.Model.VehiclesInfo = new { }.Json();
            }
            this.Model.VehicleType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.VehicleType>().Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save()
        {
            string id = Request.Form["ID"];
            var carrerid = Request.Form["CarrierID"];
            var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Vehicles[id] ?? new Needs.Ccs.Services.Models.Vehicle();
            var carrer = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers[carrerid] ?? new Needs.Ccs.Services.Models.Carrier();
            var vehicleType = Request.Form["VehicleType"];
            var weight = Request.Form["Weight"];
            var size = Request.Form["Size"];
            entity.Carrier = carrer;
            entity.License = Request.Form["License"];
            entity.VehicleType = (VehicleType)int.Parse(vehicleType);
            entity.HKLicense = Request.Form["HKLicense"];
            entity.Weight = weight;
            entity.Size = size;
            entity.Status = Status.Normal;

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
                        Status =(int)entity.Carrier.Status,
                        Creator=ErmAdminID,
                        Type = (CRMCarrierType)dic.Values.ToArray()[0],
                        Place = dic.Keys.ToArray()[0],
                    },
                    Transport = new Needs.Ccs.Services.Models.ApiModels.Transport
                    {
                        EnterpriseName=entity.Carrier.Name,
                        Type=entity.VehicleType,
                        CarNumber1=entity.License,
                        CarNumber2=entity.HKLicense,
                        Weight=entity.Weight,
                        Status=(int)entity.Status,
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
                entity.EnterError += Vehicle_EnterError;
                entity.EnterSuccess += Vehicle_EnterSuccess;
                entity.Enter();
            }
            else {
                entity.EnterError += Vehicle_EnterError;
                entity.EnterSuccess += Vehicle_EnterSuccess;
                entity.Enter();
            }
        }

        protected bool IsExitHKLicense()
        {
            var result = false;
            var hklicense = Request.Form["HKLicense"];
            string id = Request.Form["ID"];
            if (string.IsNullOrEmpty(hklicense))
            {
                return result = true;
            }
            result = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Vehicles.Where(x => x.ID != id && x.Status == Status.Normal && x.HKLicense == hklicense).Count() < 1;
            return result;
        }

        protected bool IsExitLicense()
        {
            var result = false;
            var license = Request.Form["License"];
            string id = Request.Form["ID"];
            result = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Vehicles.Where(x => x.ID != id && x.Status == Status.Normal && x.License == license).Count() < 1;
            return result;
        }

        protected bool GetCarrierType()
        {
            var result = false;
            string id = Request.Form["ID"];
            if (!string.IsNullOrEmpty(id))
                result = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers[id].CarrierType == Needs.Ccs.Services.Enums.CarrierType.InteLogistics;
            return result;
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vehicle_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }


        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vehicle_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}
