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

namespace WebApp.Logistics.Carrier
{
    public partial class Edit : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            SetDropdownlist();
            LoadData();
        }

        private void SetDropdownlist()
        {
            this.Model.CarrierTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CarrierType>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();
        }

        /// <summary>
        /// 初始化基本信息
        /// </summary>
        protected void LoadData()
        {
            string tariffID = Request.QueryString["ID"];
            this.Model.IDdata = tariffID.Json();
            if (!string.IsNullOrWhiteSpace(tariffID))
            {
                var tariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers[tariffID];

                this.Model.CarriersInfo = new
                {
                    ID = tariff.ID,
                    CarrierType = tariff.CarrierType,
                    Code = tariff.Code,
                    QueryMark = tariff.QueryMark,
                    Name = tariff.Name,
                    Summary = tariff.Summary,
                    ContactName = tariff.Contact.Name,
                    ContactMobile = tariff.Contact.Mobile,
                    tariff.Contact.Fax,
                    tariff.Address
                }.Json();
            }
            else
            {
                this.Model.CarriersInfo = new { }.Json();
            }
        }

        /// <summary>
        ///判断名称是否重复
        /// </summary>
        /// <returns></returns>
        protected bool IsExitName()
        {
            var result = false;
            var name = Request.Form["Name"];
            string id = Request.Form["ID"];
            result = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.Where(x => x.ID != id && x.Status == Status.Normal && x.Name == name).Count() < 1;
            return result;
        }

        /// <summary>
        ///判断简称是否存在
        /// </summary>
        /// <returns></returns>
        protected bool IsExitCode()
        {
            var result = false;
            var code = Request.Form["Code"];
            string id = Request.Form["ID"];
            result = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.Where(x => x.ID != id && x.Status == Status.Normal && x.Code == code).Count() < 1;
            return result;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save()
        {
            var ContactName = Request.Form["ContactName"];
            var Mobile = Request.Form["ContactMobile"];
            var CarrierType = Request.Form["CarrierType"];
            string id = Request.Form["ID"];
            string fax = Request.Form["Fax"];
            string address = Request.Form["Address"];
            var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers[id] ?? new Needs.Ccs.Services.Models.Carrier();

            entity.Code = Request.Form["Code"];
            entity.QueryMark = Request.Form["QueryMark"];
            entity.Name = Request.Form["Name"];
            entity.CarrierType = (CarrierType)int.Parse(CarrierType);
            entity.Summary = Request.Form["Summary"].Trim();
            // if (!string.IsNullOrEmpty(address))
            entity.Address = address;
            if (entity.Contact == null)
                entity.Contact = new Needs.Ccs.Services.Models.Contact();
            entity.Contact.Name = ContactName;
            entity.Contact.Mobile = Mobile;
            // if (!string.IsNullOrEmpty(fax))
            entity.Contact.Fax = fax;
            entity.Status = Status.Normal;
            if (string.IsNullOrEmpty(URL))
            {
                entity.EnterError += Carrier_EnterError;
                entity.EnterSuccess += Carrier_EnterSuccess;
                entity.Enter();
                if (entity.CarrierType == Needs.Ccs.Services.Enums.CarrierType.DomesticExpress)
                {
                    CompanySave(entity);
                }
            }
            else
            {

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
                        Status = (int)entity.Status,
                        Type = (CRMCarrierType)dic.Values.ToArray()[0],
                        Place = dic.Keys.ToArray()[0],
                        Creator = ErmAdminID,

                    },
                };
                string apiCarrier = JsonConvert.SerializeObject(carrier);
                response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiCarrier);
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Response.Write((new { success = false, message = "请求CRM接口失败：" }).Json());
                    return;
                }

                entity.EnterError += Carrier_EnterError;
                entity.EnterSuccess += Carrier_EnterSuccess;
                entity.Enter();
                if (entity.CarrierType == Needs.Ccs.Services.Enums.CarrierType.DomesticExpress)
                {
                    CompanySave(entity);
                }

            }
        }

        private void CompanySave(Needs.Ccs.Services.Models.Carrier entity)
        {
            var company = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies[entity.ID] as
             Needs.Ccs.Services.Models.ExpressCompany ?? new Needs.Ccs.Services.Models.ExpressCompany();
            company.Name = entity.Name;
            company.Code = entity.Code;
            company.CustomerName = "";
            company.CustomerPwd = "";
            company.MonthCode = "";
            company.Enter();
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Carrier_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());

        }


        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Carrier_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}