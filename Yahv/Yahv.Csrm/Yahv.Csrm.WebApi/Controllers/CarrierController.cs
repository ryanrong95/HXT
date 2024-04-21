using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Yahv.Csrm.WebApi.Models;
using Yahv.Services;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class CarrierController : ClientController
    {
        #region 芯达通向Crm同步-靳珊珊
        // GET: Carrier
        [HttpPost]
        public ActionResult Enter([System.Web.Http.FromBody]Models.CarrierModel model)
        {
            var enterprise = new YaHv.Csrm.Services.Models.Origins.Enterprise();

            #region 承运商
            if (model.Carrier != null)
            {
                if (string.IsNullOrWhiteSpace(model.Carrier.Name))
                {
                    return eJson(new JMessage { code = 100, success = false, data = "承运商名称不能为空" });
                }
                else
                {
                    var carrier = new YaHv.Csrm.Services.Views.Rolls.CarriersRoll().FirstOrDefault(item => item.Enterprise.Name == model.Carrier.Name);
                    //if (model.Carrier.Name == "芯达通物流部")
                    //{
                    //    model.Carrier.EnterpriseID = enterprise.ID = "XdtPCL";
                    //    enterprise.ID = "XdtPCL";
                    //    enterprise.Name = "芯达通物流部";
                    //    enterprise.Place = Origin.CHN.GetOrigin().Code;
                    //}
                    //else
                    //{
                    enterprise = carrier == null ? new YaHv.Csrm.Services.Models.Origins.Enterprise { Name = model.Carrier.Name, AdminCode = "", Place = model.Carrier.Place } : carrier.Enterprise;
                    //承运商国家或地区是否修改
                    if (enterprise.Place != model.Carrier.Place)
                    {
                        enterprise.Place = model.Carrier.Place;
                    }
                    //}
                    //删除
                    if (model.Carrier.Status == GeneralStatus.Deleted)
                    {
                        carrier.AbandonSuccess += Carrier_AbandonSuccess;
                        carrier.Abandon();
                    }
                    //新增或编辑
                    else if (model.Carrier.Status == GeneralStatus.Normal)
                    {
                        if (carrier == null)
                        {
                            carrier = new YaHv.Csrm.Services.Models.Origins.Carrier();
                        }
                        carrier.ID = enterprise.ID;
                        carrier.Enterprise = enterprise;
                        carrier.Icon = carrier.Icon == null ? "" : carrier.Icon;
                        carrier.Code = model.Carrier.Code;
                        carrier.Type = model.Carrier.Type;
                        carrier.CreatorID = model.Carrier.Creator;
                        carrier.Summary = model.Carrier.Summary;
                        carrier.EnterSuccess += Carrier_EnterSuccess;
                        carrier.Enter();
                    }
                }
            }
            #endregion

            #region 司机 
            if (model.Driver != null)
            {
                YaHv.Csrm.Services.Models.Origins.Driver driver = new YaHv.Csrm.Services.Models.Origins.Driver();

                // string creatorid2 = new YaHv.Csrm.Services.Views.Rolls.AdminsAllRoll().FirstOrDefault(item => item.RealName == model.Driver.Creator)?.ID;
                driver.Enterprise = enterprise;
                driver.Name = model.Driver.Name;
                driver.IDCard = model.Driver.IDCard;
                driver.Mobile = string.IsNullOrWhiteSpace(model.Driver.Mobile) ? "" : model.Driver.Mobile;
                driver.Mobile2 = model.Driver.Mobile2;
                driver.CustomsCode = model.Driver.CustomsCode;
                driver.PortCode = model.Driver.PortCode;
                driver.LBPassword = model.Driver.LBPassword;
                driver.CardCode = model.Driver.CardCode;
                driver.IsChcd = model.Driver.IsChcd;//是否中港贸易
                driver.CreatorID = model.Driver.Creator;
                if (model.Driver.Status == GeneralStatus.Deleted)
                {
                    driver.AbandonSuccess += Driver_AbandonSuccess;
                    driver.Abandon();
                }
                else if (model.Driver.Status == GeneralStatus.Normal)
                {
                    driver.EnterSuccess += Driver_EnterSuccess;
                    driver.Enter();
                }

            }
            #endregion

            #region 车辆信息
            if (model.Transport != null)
            {
                YaHv.Csrm.Services.Models.Origins.Transport transport = new YaHv.Csrm.Services.Models.Origins.Transport();

                transport.Enterprise = enterprise;
                transport.Type = model.Transport.Type;
                transport.CarNumber1 = model.Transport.CarNumber1;
                transport.CarNumber2 = model.Transport.CarNumber2;
                transport.Weight = model.Transport.Weight;
                if (model.Transport.Status == GeneralStatus.Deleted)
                {
                    transport.AbandonSuccess += Transport_AbandonSuccess;
                    transport.Abandon();
                }
                else if (model.Transport.Status == GeneralStatus.Normal)
                {
                    // string creatorid = new YaHv.Csrm.Services.Views.Rolls.AdminsAllRoll().FirstOrDefault(item => item.RealName == model.Transport.creator)?.ID;
                    transport.CreatorID = model.Transport.Creator;
                    transport.EnterSuccess += Transport_EnterSuccess;
                    transport.Enter();
                }
            }

            #endregion

            return eJson();
        }

        private void Transport_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = "Transport保存成功" });
        }

        private void Transport_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = "Transport删除成功" });
        }

        private void Driver_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = "Driver保存成功" });
        }

        private void Driver_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = "Driver删除成功" });
        }

        private void Carrier_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = "承运商删除成功" });
        }

        private void Carrier_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            eJson(new JMessage { code = 200, success = true, data = "承运商保存成功" });
        }
        #endregion

        #region 只新增承运商-乔霞
        public ActionResult Index([System.Web.Http.FromBody]Models.Carrier model)
        {
            var enterprise = new YaHv.Csrm.Services.Models.Origins.Enterprise();

            #region 承运商
            if (model != null)
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    return eJson(new JMessage { code = 100, success = false, data = "承运商名称不能为空" });

                }

                else
                {
                    var carrier1 = new YaHv.Csrm.Services.Views.Rolls.CarriersRoll()[model.EnterpriseID];
                    if (carrier1 != null)
                    {
                        return eJson(new JMessage { code = 300, success = false, data = "承运商已存在" });
                    }
                    else
                    {
                        carrier1 = carrier1 ?? new YaHv.Csrm.Services.Models.Origins.Carrier();
                        enterprise = new YaHv.Csrm.Services.Views.Rolls.EnterprisesRoll()[model.EnterpriseID] ?? new YaHv.Csrm.Services.Models.Origins.Enterprise { Name = model.Name, AdminCode = "", Place = model.Place };
                        //承运商国家或地区是否修改
                        if (enterprise.Place != model.Place)
                        {
                            enterprise.Place = model.Place;
                        }
                        //承运商只新增
                        carrier1.Enterprise = enterprise;
                        carrier1.Icon = carrier1.Icon == null ? "" : carrier1.Icon;
                        carrier1.Code = model.Code;
                        carrier1.Type = model.Type;
                        carrier1.CreatorID = model.Creator;
                        carrier1.Summary = model.Summary;
                        carrier1.IsInternational = model.IsInternational;
                        carrier1.EnterSuccess += Carrier1_EnterSuccess; ;
                        carrier1.Enter();
                    }

                }
            }
            #endregion

            #region 异步调用芯达通接口
            model.CarrierType = new Models.CarrierModel().ConvertType(model.Type, model.Place);
            var entity = new Models.CarrierModel { Carrier = model, Driver = null, Transport = null }.Json();
            string response = "";
            Task t1 = new Task(() =>
            {
                try
                {
                    response = Commons.HttpPostRaw(Commons.UnifyApiUrl + "/Carriers/Enter", entity);
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

        private void Carrier1_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var carrier = sender as YaHv.Csrm.Services.Models.Origins.Carrier;
            eJson(new JMessage { code = 200, success = true, data = carrier.Enterprise.ID });
        }
        #endregion

        #region 承运商类型-乔霞
        /// <summary>
        /// 获取承运商类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Type()
        {
            var type = ExtendsEnum.ToArray<CarrierType>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
            return Json(type, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 承运商、司机、车辆同时新增-乔霞
        string CarrierID { set; get; }
        string DriverID { set; get; }
        string TransportID { set; get; }
        string ContactID { set; get; }
        bool success1 = false;//承运商
        bool success2 = false;//司机
        bool success3 = false;//车辆
        bool success4 = false;//联系人
        public ActionResult AllEnter([System.Web.Http.FromBody]Models.CarrierModel model)
        {
            try
            {
                var enterprise = new YaHv.Csrm.Services.Models.Origins.Enterprise();
                var result = new Result();
                #region 承运商
                if (model.Carrier != null)
                {
                    if (string.IsNullOrWhiteSpace(model.Carrier.Name))
                    {
                        return eJson(new JMessage { code = 100, success = false, data = "承运商名称不能为空" });
                    }
                    else
                    {
                        var qx_carrier = new YaHv.Csrm.Services.Views.Rolls.CarriersRoll()[model.Carrier.EnterpriseID];
                        model.Carrier.Status = GeneralStatus.Normal;
                        enterprise = new YaHv.Csrm.Services.Views.Rolls.EnterprisesRoll()[model.Carrier.EnterpriseID] ?? new YaHv.Csrm.Services.Models.Origins.Enterprise { Name = model.Carrier.Name, AdminCode = "", Place = model.Carrier.Place };
                        //承运商国家或地区是否修改
                        if (enterprise.Place != model.Carrier.Place)
                        {
                            enterprise.Place = model.Carrier.Place;
                        }

                        if (qx_carrier == null)
                        {
                            qx_carrier = new YaHv.Csrm.Services.Models.Origins.Carrier();
                            qx_carrier.Enterprise = enterprise;
                            qx_carrier.Icon = qx_carrier.Icon == null ? "" : qx_carrier.Icon;
                            qx_carrier.Code = model.Carrier.Code;
                            qx_carrier.Type = model.Carrier.Type;
                            qx_carrier.CreatorID = model.Carrier.Creator;
                            qx_carrier.Summary = model.Carrier.Summary;
                            qx_carrier.Status = GeneralStatus.Normal;
                            qx_carrier.IsInternational = model.Carrier.IsInternational;
                            qx_carrier.EnterSuccess += Qx_carrier_EnterSuccess;
                            qx_carrier.Enter();
                        }
                        else
                        {
                            if (qx_carrier.Code == model.Carrier.Code)
                            {
                                this.CarrierID = qx_carrier.ID;
                                success1 = true;
                            }
                            else
                            {
                                qx_carrier.Code = model.Carrier.Code;
                                qx_carrier.EnterSuccess += Qx_carrier_EnterSuccess;
                                qx_carrier.Enter();
                            }
                        }
                    }
                }
                #endregion

                #region 司机 
                if (model.Driver != null)
                {
                    model.Driver.EnterpriseName = model.Carrier.Name;
                    model.Driver.CarrierCode = model.Carrier.Code;
                    model.Driver.Status = GeneralStatus.Normal;
                    YaHv.Csrm.Services.Models.Origins.Driver qx_driver = new YaHv.Csrm.Services.Models.Origins.Driver();

                    qx_driver.Enterprise = enterprise;
                    qx_driver.Name = model.Driver.Name;
                    qx_driver.IDCard = model.Driver.IDCard;
                    qx_driver.Mobile = string.IsNullOrWhiteSpace(model.Driver.Mobile) ? "" : model.Driver.Mobile;
                    qx_driver.Mobile2 = model.Driver.Mobile2;
                    qx_driver.CustomsCode = model.Driver.CustomsCode;
                    qx_driver.PortCode = model.Driver.PortCode;
                    qx_driver.LBPassword = model.Driver.LBPassword;
                    qx_driver.CardCode = model.Driver.CardCode;
                    qx_driver.IsChcd = model.Driver.IsChcd;//是否中港贸易
                    qx_driver.Status = GeneralStatus.Normal;
                    qx_driver.CreatorID = model.Driver.Creator;
                    qx_driver.Status = GeneralStatus.Normal;
                    qx_driver.EnterSuccess += Qx_driver_EnterSuccess; ;
                    qx_driver.Enter();

                }
                #endregion

                #region 车辆信息
                if (model.Transport != null)
                {
                    model.Transport.EnterpriseName = model.Carrier.Name;
                    model.Transport.CarrierCode = model.Carrier.Code;
                    model.Transport.Status = GeneralStatus.Normal;
                    YaHv.Csrm.Services.Models.Origins.Transport qx_transport = new YaHv.Csrm.Services.Models.Origins.Transport();

                    qx_transport.Enterprise = enterprise;
                    qx_transport.Type = model.Transport.Type;
                    qx_transport.CarNumber1 = model.Transport.CarNumber1;
                    qx_transport.CarNumber2 = model.Transport.CarNumber2;
                    qx_transport.Weight = model.Transport.Weight;
                    qx_transport.CreatorID = model.Transport.Creator;
                    qx_transport.Status = GeneralStatus.Normal;
                    qx_transport.EnterSuccess += Qx_transport_EnterSuccess; ;
                    qx_transport.Enter();
                }

                #endregion

                #region 联系人
                if (model.Contact != null)
                {
                    YaHv.Csrm.Services.Models.Origins.Contact qx_contact = new YaHv.Csrm.Services.Models.Origins.Contact();
                    qx_contact.Enterprise = enterprise;
                    qx_contact.EnterpriseID = enterprise.ID;
                    qx_contact.Name = model.Contact.Name;
                    qx_contact.Tel = model.Contact.Tel;
                    qx_contact.Mobile = model.Contact.Mobile;
                    qx_contact.Email = model.Contact.Email;
                    qx_contact.Type = model.Contact.Type == null ? ContactType.Offline : (ContactType)model.Contact.Type;
                    qx_contact.CreatorID = model.Contact.Creator;
                    qx_contact.Status = YaHv.Csrm.Services.Status.Normal;
                    qx_contact.EnterSuccess += Qx_contact_EnterSuccess;

                    qx_contact.Enter();
                }


                #endregion

                #region 异步调用芯达通接口
                model.Carrier.CarrierType = new Models.CarrierModel().ConvertType(model.Carrier.Type, model.Carrier.Place);

                var entity = new Models.CarrierModel { Carrier = model.Carrier, Driver = model.Driver, Transport = model.Transport }.Json();
                string response = "";
                Task t1 = new Task(() =>
                {
                    try
                    {
                        response = Commons.HttpPostRaw(Commons.UnifyApiUrl + "/Carriers/WarehouseEnter", entity);
                    }
                    catch (Exception ex)
                    {
                        eJson(new JMessage { code = 400, success = false, data = "XDT接口调用失败" + ex });
                    }

                });
                t1.Start();
                Task.WaitAll(t1);

                #endregion
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 0, data = ex.Message });
            }
            return Json(new
            {
                Carrier = new { success = success1, ID = this.CarrierID },
                Driver = new { success = success2, ID = this.DriverID },
                Transport = new { success = success3, ID = this.TransportID },
                Contact = new { success = success4, ID = this.ContactID }
            });
            //return Json(new { success = success1 && success2 && success3, code = 200,  data = new { CarrierID = this.CarrierID, DriverID = this.DriverID, TransportID = this.TransportID } });
        }

        private void Qx_driver_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var driverid = sender as YaHv.Csrm.Services.Models.Origins.Driver;
            this.DriverID = driverid.ID;
            success2 = true;
            //eJson(new JMessage {code = 200, success = true,data = new { CarrierID = this.CarrierID, DriverID = this.DriverID, TransportID = this.TransportID }.Json() });
        }

        private void Qx_carrier_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var carrier = sender as YaHv.Csrm.Services.Models.Origins.Carrier;
            this.CarrierID = carrier.Enterprise.ID;
            success1 = true;
            // eJson(new JMessage { code = 200, success = true, data = new { CarrierID = this.CarrierID, DriverID = this.DriverID, TransportID = this.TransportID }.Json() });
        }

        private void Qx_transport_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var transport = sender as YaHv.Csrm.Services.Models.Origins.Transport;
            this.TransportID = transport.ID;
            success3 = true;
            //eJson(new JMessage { code = 200, success = true, data = new { CarrierID = this.CarrierID, DriverID = this.DriverID, TransportID = this.TransportID }.Json() });
        }
        private void Qx_contact_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var contact = sender as YaHv.Csrm.Services.Models.Origins.Contact;
            this.ContactID = contact.ID;
            success4 = true;
        }

        #endregion

        #region 承运商，联系人新增-乔霞

        public ActionResult Contact([System.Web.Http.FromBody]Models.Carrier model)
        {
            var enterprise = new YaHv.Csrm.Services.Models.Origins.Enterprise();

            #region 承运商
            if (model != null)
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    return eJson(new JMessage { code = 100, success = false, data = "承运商名称不能为空" });

                }

                else
                {
                    var qx_carrier = new YaHv.Csrm.Services.Views.Rolls.CarriersRoll()[model.EnterpriseID];
                    if (qx_carrier == null)
                    {
                        qx_carrier = qx_carrier ?? new YaHv.Csrm.Services.Models.Origins.Carrier();
                        enterprise = new YaHv.Csrm.Services.Views.Rolls.EnterprisesRoll()[model.EnterpriseID] ?? new YaHv.Csrm.Services.Models.Origins.Enterprise { Name = model.Name, AdminCode = "", Place = model.Place };
                        //承运商国家或地区是否修改
                        if (enterprise.Place != model.Place)
                        {
                            enterprise.Place = model.Place;
                        }
                        //承运商只新增
                        qx_carrier.Enterprise = enterprise;
                        qx_carrier.Icon = qx_carrier.Icon == null ? "" : qx_carrier.Icon;
                        qx_carrier.Code = model.Code;
                        qx_carrier.Type = model.Type;
                        qx_carrier.CreatorID = model.Creator;
                        qx_carrier.Summary = model.Summary;
                        qx_carrier.IsInternational = model.IsInternational;
                        qx_carrier.EnterSuccess += Qx_carrier_EnterSuccess;
                        qx_carrier.Enter();
                    }
                    else
                    {
                        if (model.Code == qx_carrier.Code)
                        {
                            this.success1 = true;
                            this.CarrierID = qx_carrier.ID;
                        }
                        else
                        {
                            qx_carrier.Code = model.Code;
                            qx_carrier.EnterSuccess += Qx_carrier_EnterSuccess;
                            qx_carrier.Enter();
                        }

                    }
                }
            }
            #endregion

            #region 联系人
            if (model.ContactName != null)
            {
                YaHv.Csrm.Services.Models.Origins.Contact qx_contact = new YaHv.Csrm.Services.Models.Origins.Contact();

                qx_contact.EnterpriseID = model.EnterpriseID;
                qx_contact.Name = model.ContactName;
                qx_contact.Type = ContactType.Offline;
                qx_contact.CreatorID = model.Creator;
                qx_contact.Status = YaHv.Csrm.Services.Status.Normal;
                qx_contact.EnterSuccess += Qx_contact_EnterSuccess;
                qx_contact.Enter();
            }


            #endregion

            #region 异步调用芯达通接口
            model.CarrierType = new Models.CarrierModel().ConvertType(model.Type, model.Place);
            var entity = new Models.CarrierModel { Carrier = model, Driver = null, Transport = null }.Json();
            string response = "";
            Task t1 = new Task(() =>
            {
                try
                {
                    response = Commons.HttpPostRaw(Commons.UnifyApiUrl + "/Carriers/Enter", entity);
                }
                catch (Exception ex)
                {
                    eJson(new JMessage { code = 400, success = false, data = "XDT接口调用失败" + ex });
                }

            });
            t1.Start();
            Task.WaitAll(t1);

            #endregion
            return Json(new
            {
                Carrier = new { success = success1, ID = this.CarrierID },
                Contact = new { success = success4, ID = this.ContactID }
            });
        }




        #endregion
    }

}
