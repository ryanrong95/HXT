using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models.ApiModels;
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

namespace WebApp.Logistics.Driver
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

            this.Model.CarrierData = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.Where(x => (x.CarrierType == CarrierType.DomesticLogistics || x.CarrierType == CarrierType.InteLogistics) && x.Status == Status.Normal).Select(item => new
            {
                value = item.ID,
                text = item.Name,
                Type = item.CarrierType
            }).Json();


            if (!string.IsNullOrWhiteSpace(driverID))
            {
                var driver = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Drivers[driverID];

                this.Model.DriversInfo = new
                {
                    driver.ID,
                    driver.License,
                    driver.Mobile,
                    driver.Name,
                    CarrierID = driver.Carrier.ID,
                    driver.HSCode,
                    driver.HKMobile,
                    driver.DriverCardNo,
                    driver.PortElecNo,
                    driver.LaoPaoCode,
                    driver.IsChcd
                }.Json();
            }
            else
            {
                this.Model.DriversInfo = new { }.Json();
            }

        }

        /// <summary>
        /// 判断手机号是否重复
        /// </summary>
        /// <returns></returns>
        protected bool IsExitMobile()
        {
            var result = false;
            var Mobile = Request.Form["Mobile"];
            string id = Request.Form["ID"];
            if (string.IsNullOrWhiteSpace(Mobile))
            {

                return true;
            }
            result = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Drivers.Where(x => x.ID != id && x.Status == Status.Normal && x.Mobile == Mobile).Count() < 1;
            return result;
        }

        /// <summary>
        /// 判断身份证号码是否重复
        /// </summary>
        /// <returns></returns>
        //protected bool IsExitIDCard()
        //{
        //    var result = false;
        //    string id = Request.Form["ID"];
        //    var License = Request.Form["License"];
        //    result = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Drivers.Where(x => x.ID != id && x.Status == Status.Normal && x.License == License).Count()<1;
        //    return result;
        //}

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save()
        {

            string id = Request.Form["ID"];
            string carrierID = Request.Form["CarrierID"];
            var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Drivers[id] ?? new Needs.Ccs.Services.Models.Driver();
            var carrer = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers[carrierID] ?? new Needs.Ccs.Services.Models.Carrier();
            entity.Carrier = carrer;
            var IsChcd = Request.Form["IsChcdBack"];
            entity.Name = Request.Form["Name"];
            entity.Mobile = Request.Form["Mobile"];
            entity.License = Request.Form["License"];
            entity.HSCode = Request.Form["HSCode"];
            entity.HKMobile = Request.Form["HKMobile"];
            entity.DriverCardNo = Request.Form["DriverCardNo"];
            entity.PortElecNo = Request.Form["PortElecNo"];
            entity.LaoPaoCode = Request.Form["LaoPaoCode"];
            entity.IsChcd = Convert.ToBoolean(IsChcd);
            entity.Status = Status.Normal;

            try
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
                            Status = 200,
                            Creator = ErmAdminID,
                            Type = (CRMCarrierType)dic.Values.ToArray()[0],
                            Place = dic.Keys.ToArray()[0],
                        },
                        Driver = new Needs.Ccs.Services.Models.ApiModels.Driver
                        {
                            EnterpriseName = entity.Carrier.Name,
                            Name = entity.Name,
                            Mobile = entity.Mobile,
                            IDCard = entity.License,
                            Status = (int)entity.Status,
                            CustomsCode = entity.HSCode,
                            CardCode = entity.DriverCardNo,
                            Mobile2 = entity.HKMobile,
                            PortCode = entity.PortElecNo,
                            LBPassword = entity.LaoPaoCode,
                            Creator = ErmAdminID,
                            IsChcd = entity.IsChcd
                        }
                    };
                    string apiCarrier = JsonConvert.SerializeObject(carrier);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiCarrier);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求CRM接口失败：" + response.RequestMessage }).Json());
                        return;
                    }

                    entity.EnterError += Driver_EnterError;
                    entity.EnterSuccess += Driver_EnterSuccess;
                    entity.Enter();
                }
                else
                {

                    entity.EnterError += Driver_EnterError;
                    entity.EnterSuccess += Driver_EnterSuccess;
                    entity.Enter();

                }
            }
            catch (Exception ex)
            {


            }

        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Driver_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }


        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Driver_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}