using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Models.ApiModels;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi
{
    /// <summary>
    /// 承运商贯通
    /// </summary>
    public class CarriersController : ApiController
    {
        /// <summary>
        /// 物流信息持久化
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public HttpResponseMessage Enter([System.Web.Http.FromBody]CarrierModel model)
        {
            try
            {
                #region 承运商
                if (model.Carrier != null)
                {

                    if (string.IsNullOrWhiteSpace(model.Carrier.Name))
                    {

                        return ApiResultModel.OutputResult(new JMessage { code = 100, success = false, data = "承运商不存在" });
                    }
                    else
                    {
                        var carrier = new Needs.Ccs.Services.Views.CarriersView()[model.Carrier.ID] ?? new Needs.Ccs.Services.Models.Carrier { Name = model.Carrier.Name, Code = model.Carrier.Code };
                        if (model.Carrier.Status == 400)
                        {
                            carrier.AbandonSuccess += Carrier_AbandonSuccess;
                            carrier.Abandon();
                        }
                        else if (model.Carrier.Status == (int)Needs.Ccs.Services.Enums.Status.Normal)
                        {
                            carrier.Code = model.Carrier.Code;
                            carrier.Name = model.Carrier.Name;
                            carrier.CarrierType = model.Carrier.CarrierType;
                            carrier.Summary = model.Carrier.Summary;
                            carrier.Status = (Status)model.Carrier.Status;
                            carrier.Contact = new Contact() { Name = "", Mobile = "" };
                            carrier.EnterSuccess += Carrier_EnterSuccess;
                            carrier.Enter();
                        }
                    }
                }
                #endregion

                #region  司机
                if (model.Driver != null)
                {
                    var carrier = new Needs.Ccs.Services.Views.CarriersView()[model.Carrier.ID];
                    if (carrier == null)
                    {
                        return ApiResultModel.OutputResult(new JMessage { code = 100, success = false, data = "承运商不存在" });
                    }
                    else
                    {


                        if (string.IsNullOrWhiteSpace(model.Driver.Name))
                            return ApiResultModel.OutputResult(new JMessage { code = 100, success = false, data = "司机不存在" });
                        else
                        {
                            var driver = new Needs.Ccs.Services.Views.DriverView()[model.Driver.ID] ?? new Needs.Ccs.Services.Models.Driver() { Name = model.Driver.Name, License = model.Driver.IDCard };
                            if (model.Driver.Status == 400 || (model.Carrier.CarrierType == Needs.Ccs.Services.Enums.CarrierType.DomesticExpress || model.Carrier.CarrierType == Needs.Ccs.Services.Enums.CarrierType.InteExpress))
                            {
                                driver.AbandonSuccess += Carrier_AbandonSuccess;
                                driver.Abandon();
                            }
                            else if (model.Carrier.Status == (int)Needs.Ccs.Services.Enums.Status.Normal && (model.Carrier.CarrierType == Needs.Ccs.Services.Enums.CarrierType.DomesticLogistics || model.Carrier.CarrierType == Needs.Ccs.Services.Enums.CarrierType.InteLogistics))
                            {

                                driver.Name = model.Driver.Name;
                                driver.License = model.Driver.IDCard;
                                driver.Mobile = model.Driver.Mobile;
                                driver.HKMobile = model.Driver.Mobile2;
                                driver.HSCode = model.Driver.CustomsCode;
                                driver.LaoPaoCode = model.Driver.LBPassword;
                                driver.PortElecNo = model.Driver.PortCode;
                                driver.DriverCardNo = model.Driver.CardCode;
                                driver.Status = (Status)model.Driver.Status;
                                driver.IsChcd = model.Driver.IsChcd;//是否中港贸易
                                driver.Carrier = new Needs.Ccs.Services.Models.Carrier { Name = model.Carrier.Name, Code = model.Carrier.Code };
                                driver.EnterSuccess += Driver_EnterSuccess;
                                driver.Enter();

                            }
                        }
                    }
                }

                #endregion

                #region  车辆
                if (model.Transport != null)
                {

                    var transport = new Needs.Ccs.Services.Views.VehicleView()[model.Transport.ID] ?? new Needs.Ccs.Services.Models.Vehicle() { VehicleType = model.Transport.Type, License = model.Transport.CarNumber1 };
                    if (model.Transport.Status == 400 || (model.Carrier.CarrierType == Needs.Ccs.Services.Enums.CarrierType.DomesticExpress || model.Carrier.CarrierType == Needs.Ccs.Services.Enums.CarrierType.InteExpress))
                    {
                        transport.AbandonSuccess += Transport_AbandonSuccess;
                        transport.Abandon();
                    }
                    else if (model.Transport.Status == (int)Needs.Ccs.Services.Enums.Status.Normal && (model.Carrier.CarrierType == Needs.Ccs.Services.Enums.CarrierType.DomesticLogistics || model.Carrier.CarrierType == Needs.Ccs.Services.Enums.CarrierType.InteLogistics))
                    {
                        if (string.IsNullOrWhiteSpace(model.Transport.CarNumber1))
                            return ApiResultModel.OutputResult(new JMessage { code = 100, success = false, data = "车牌不存在" });
                        transport.License = model.Transport.CarNumber1;
                        transport.HKLicense = model.Transport.CarNumber2;
                        transport.VehicleType = model.Transport.Type;
                        transport.Weight = model.Transport.Weight;
                        transport.Status = (Status)model.Transport.Status;
                        transport.Carrier = new Needs.Ccs.Services.Models.Carrier { Name = model.Carrier.Name, Code = model.Carrier.Code };
                        transport.EnterSuccess += Transport_EnterSuccess;
                        transport.Enter();
                    }
                }

                #endregion

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return ApiResultModel.OutputResult(json);
            }
            catch (Exception ex)
            {

                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return ApiResultModel.OutputResult(json);
            }


        }




        private void Carrier_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            ApiResultModel.OutputResult(new JMessage { code = 200, success = true, data = "承运商删除成功" });
        }
        private void Carrier_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            ApiResultModel.OutputResult(new JMessage { code = 200, success = true, data = "承运商保存成功" });
        }


        private void Driver_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            ApiResultModel.OutputResult(new JMessage { code = 200, success = true, data = "司机删除成功" });
        }
        private void Driver_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            ApiResultModel.OutputResult(new JMessage { code = 200, success = true, data = "司机保存成功" });
        }

        private void Transport_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            ApiResultModel.OutputResult(new JMessage { code = 200, success = true, data = "车辆删除成功" });
        }
        private void Transport_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            ApiResultModel.OutputResult(new JMessage { code = 200, success = true, data = "车辆信息保存成功" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public HttpResponseMessage DriverEnter([System.Web.Http.FromBody]Needs.Ccs.Services.Models.ApiModels.Driver model)
        {
            try
            {
                if (model != null)
                {
                    var entity = new Needs.Ccs.Services.Models.Carrier { Name = model.EnterpriseName, Code = model.CarrierCode };
                    var carrier = new Needs.Ccs.Services.Views.CarriersView()[entity.ID];
                    if (carrier == null)
                    {
                        return ApiResultModel.OutputResult(new JMessage { code = 100, success = false, data = "承运商不存在" });
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(model.Name))
                            return ApiResultModel.OutputResult(new JMessage { code = 100, success = false, data = "司机不存在" });
                        else
                        {
                            var driver = new Needs.Ccs.Services.Views.DriverView()[model.ID] ?? new Needs.Ccs.Services.Models.Driver() { Name = model.EnterpriseName, Code = model.CarrierCode };
                            driver.Name = model.Name;
                            driver.License = model.IDCard;
                            driver.Mobile = model.Mobile;
                            driver.HKMobile = model.Mobile2;
                            driver.HSCode = model.CustomsCode;
                            driver.LaoPaoCode = model.LBPassword;
                            driver.PortElecNo = model.PortCode;
                            driver.DriverCardNo = model.CardCode;
                            driver.Status = Status.Normal;
                            driver.IsChcd = model.IsChcd;//是否中港贸易
                            driver.Carrier = entity;
                            driver.EnterSuccess += Driver_EnterSuccess;
                            driver.Enter();
                        }
                    }
                }
                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return ApiResultModel.OutputResult(json);
            }
            catch (Exception ex)
            {

                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return ApiResultModel.OutputResult(json);
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public HttpResponseMessage TransportEnter([System.Web.Http.FromBody]Needs.Ccs.Services.Models.ApiModels.Transport model)
        {
            try
            {
                if (model != null)
                {
                    //承运商是否存在
                    var carrier = new Needs.Ccs.Services.Models.Carrier { Name = model.EnterpriseName, Code = model.CarrierCode };
                    if (carrier == null)
                    {
                        return ApiResultModel.OutputResult(new JMessage { code = 100, success = false, data = "承运商不存在" });
                    }
                    var transport = new Needs.Ccs.Services.Models.Vehicle() { VehicleType = model.Type, License = model.CarNumber1 };

                    if (string.IsNullOrWhiteSpace(model.CarNumber1))
                    {
                        return ApiResultModel.OutputResult(new JMessage { code = 100, success = false, data = "车牌不能为空" });
                    }

                    transport.License = model.CarNumber1;
                    transport.HKLicense = model.CarNumber2;
                    transport.VehicleType = model.Type;
                    transport.Weight = model.Weight;
                    transport.Status = Status.Normal;
                    transport.Carrier = carrier;
                    transport.EnterSuccess += Transport_EnterSuccess;
                    transport.Enter();
                }
                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return ApiResultModel.OutputResult(json);
            }
            catch (Exception ex)
            {

                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return ApiResultModel.OutputResult(json);
            }

        }

        /// <summary>
        /// 库房新增承运信息接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public HttpResponseMessage WarehouseEnter([System.Web.Http.FromBody]CarrierModel model)
        {
            try
            {
                #region 承运商
                if (model.Carrier != null)
                {

                    if (string.IsNullOrWhiteSpace(model.Carrier.Name))
                    {

                        return ApiResultModel.OutputResult(new JMessage { code = 100, success = false, data = "承运商不存在" });
                    }
                    else
                    {
                        var carrier = new Needs.Ccs.Services.Views.CarriersView().FirstOrDefault(x => x.Name == model.Carrier.Name & x.Code == model.Carrier.Code);
                        {
                            var entity = new Needs.Ccs.Services.Models.Carrier { Name = model.Carrier.Name, Code = model.Carrier.Code };
                            entity.Code = model.Carrier.Code;
                            entity.Name = model.Carrier.Name;
                            entity.CarrierType = model.Carrier.CarrierType;
                            entity.Status = Status.Normal;
                            entity.Contact = new Contact() { Name = "", Mobile = "" };
                            entity.EnterSuccess += Carrier_EnterSuccess;
                            entity.Enter();

                        }
                    }
                }
                #endregion

                #region  司机
                if (string.IsNullOrWhiteSpace(model.Driver.Name))
                    return ApiResultModel.OutputResult(new JMessage { code = 100, success = false, data = "司机不存在" });
                else
                {
                    var driver = new Needs.Ccs.Services.Views.DriverView().FirstOrDefault(x => x.Name == model.Driver.Name & x.Mobile == model.Driver.Mobile);
                    if (driver == null)
                    {
                        var entity = new Needs.Ccs.Services.Models.Driver() { Name = model.Driver.Name };
                        entity.Name = model.Driver.Name;
                        entity.Mobile = model.Driver.Mobile;
                        entity.Status = Status.Normal;
                        entity.IsChcd = model.Driver.IsChcd;//是否中港贸易
                        entity.Carrier = new Needs.Ccs.Services.Models.Carrier { Name = model.Carrier.Name, Code = model.Carrier.Code };
                        entity.EnterSuccess += Driver_EnterSuccess;
                        entity.Enter();
                    }

                }
                #endregion

                #region 车辆

                var transport = new Needs.Ccs.Services.Views.VehicleView().FirstOrDefault(x => x.License == model.Transport.CarNumber1 || x.HKLicense == model.Transport.CarNumber2);
                if (string.IsNullOrWhiteSpace(model.Transport.CarNumber1))
                    return ApiResultModel.OutputResult(new JMessage { code = 100, success = false, data = "车牌不存在" });
                if (transport == null)
                {
                    var entity = new Needs.Ccs.Services.Models.Vehicle() { VehicleType = model.Transport.Type, License = model.Transport.CarNumber1 };
                    entity.License = model.Transport.CarNumber1;
                    entity.HKLicense = model.Transport.CarNumber2;
                    entity.VehicleType = model.Transport.Type;
                    entity.Weight = model.Transport.Weight;
                    entity.Status = Status.Normal;
                    entity.Carrier = new Needs.Ccs.Services.Models.Carrier { Name = model.Carrier.Name, Code = model.Carrier.Code };
                    entity.EnterSuccess += Transport_EnterSuccess;
                    entity.Enter();
                }

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return ApiResultModel.OutputResult(json);
                #endregion
            }
            catch (Exception ex)
            {

                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return ApiResultModel.OutputResult(json);
            }
           
           
        }
    }
    
}
