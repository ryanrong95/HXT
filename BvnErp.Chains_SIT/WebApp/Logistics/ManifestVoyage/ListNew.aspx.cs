using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Logistics.ManifestVoyage
{
    public partial class ListNew : Uc.PageBase
    {
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
        {
            //列表中, 承运商下拉选项
            this.Model.CarriersForList = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CarriersNew
                .Where(item => item.CarrierType == Needs.Wl.Models.Enums.CarrierType.InteLogistics
                            && item.Status == Needs.Wl.Models.Enums.Status.Normal)
                .Select(item => new { item.Code, item.Name }).Json();
            //列表中, 截单状态下拉选项
            this.Model.CutStatus = EnumUtils.ToDictionary<Needs.Wl.Models.Enums.CutStatus>().Select(item => new { item.Key, item.Value }).Json();

            //高级编辑框, 承运商类型下拉选项
            this.Model.CarrierTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CarrierType>()
                .Where(t => t.Key == Needs.Wl.Models.Enums.CarrierType.InteLogistics.GetHashCode().ToString())
                .Select(item => new
                {
                    TypeValue = item.Key,
                    TypeText = item.Value,
                    IsInteLogistics = item.Key == Needs.Wl.Models.Enums.CarrierType.InteLogistics.GetHashCode().ToString(),
                }).Json();

            //高级编辑框, 车辆类型下拉选项
            this.Model.VehicleType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.VehicleType>().Select(item => new { Key = item.Key, Value = item.Value }).Json();




        }

        /// <summary>
        /// 运输批次列表数据
        /// </summary>
        protected void VoyageListData()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string voyageNo = Request.QueryString["VoyageNo"];
            string carrier = Request.QueryString["Carrier"];
            string cutStatus = Request.QueryString["CutStatus"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(voyageNo))
            {
                lamdas.Add((Expression<Func<Needs.Wl.Logistics.Services.PageModels.ManifestVoyageListModel, bool>>)(item => item.VoyageNo.Contains(voyageNo.Trim())));
            }
            if (!string.IsNullOrEmpty(carrier))
            {
                lamdas.Add((Expression<Func<Needs.Wl.Logistics.Services.PageModels.ManifestVoyageListModel, bool>>)(item => item.Carrier.Contains(carrier.Trim())));
            }
            if (!string.IsNullOrEmpty(cutStatus))
            {
                Needs.Wl.Models.Enums.CutStatus cutStatusEnum = (Needs.Wl.Models.Enums.CutStatus)int.Parse(cutStatus.Trim());
                lamdas.Add((Expression<Func<Needs.Wl.Logistics.Services.PageModels.ManifestVoyageListModel, bool>>)(item => item.CutStatus == cutStatusEnum));
            }

            int totalNum = 0;
            var manifestVoyageList = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestVoyageListView.GetResult(out totalNum, page, rows, lamdas.ToArray());

            Func<Needs.Wl.Logistics.Services.PageModels.ManifestVoyageListModel, object> convert = item => new
            {
                ID = item.VoyageNo,
                VoyageNo = item.VoyageNo,
                Carrier = item.Carrier,
                DriverName = item.DriverName,
                HKLicense = item.HKLicense,
                CreateTime = item.CreateTime.ToString("yyyy-MM-dd"),
                CutStatusValue = item.CutStatus,
                CutStatus = item.CutStatus.GetDescription(),
                TransportTime = item.TransportTime?.ToString("yyyy-MM-dd") ?? string.Empty,
                VoyageType = item.VoyageType.GetDescription(),
            };

            Response.Write(new
            {
                rows = manifestVoyageList.Select(convert).ToArray(),
                total = totalNum,
            }.Json());
        }

        /// <summary>
        /// 截单操作
        /// </summary>
        protected void SureCut()
        {
            try
            {
                string id = Request.Form["ID"];
                var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure[id];
                var heads = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHeadList.Where(t=>t.VoyageID == id);
                if (entity == null)
                {
                    Response.Write((new { success = false, message = "数据错误" }).Json());
                    return;
                }
                else if (string.IsNullOrEmpty(entity.CarrierCode) || string.IsNullOrEmpty(entity.VehicleLicence) || string.IsNullOrEmpty(entity.DriverName))
                {
                    Response.Write((new { success = false, message = "请先录入承运商、车辆、司机等信息" }).Json());
                    return;
                }
                else if (heads.Any(t => !t.IsSuccess && t.Status != "04")) 
                {
                    Response.Write((new { success = false, message = "存在未申报的报关单，请等待" }).Json());
                    return;
                }
                else
                {
                    entity.SureCut();
                    entity.SureCutToWmsApi();
                    Response.Write((new { success = true, message = "操作成功" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 根据 承运商类型 Value 加载 承运商"简称"和"简称"
        /// </summary>
        /// <returns></returns>
        protected void GetCarrierCodeAndName()
        {
            string carrierTypeValue = Request.Form["CarrierTypeValue"];
            int carrierTypeValueInt = int.Parse(carrierTypeValue);

            var carriersInfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CarriersNew
                                .Where(t => t.Status == Needs.Wl.Models.Enums.Status.Normal
                                        && t.CarrierType == (Needs.Wl.Models.Enums.CarrierType)carrierTypeValueInt).ToList();

            Response.Write((new
            {
                success = true,
                carriersCodeInfo = carriersInfo.Select(item => new { TypeValue = item.ID, TypeText = item.Code, }),
                carriersNameInfo = carriersInfo.Select(item => new { TypeValue = item.ID, TypeText = item.Name, }),
            }).Json());
        }

        /// <summary>
        /// 根据承运商ID 获取 承运商信息、承运商联系人信息、车辆可选列表、司机可选列表
        /// </summary>
        protected void GetVehicleInfoAndDriverInfo()
        {
            string carrierId = Request.Form["CarrierId"];

            var carrierInfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CarriersNew
                                .Where(t => t.Status == Needs.Wl.Models.Enums.Status.Normal
                                        && t.ID == carrierId)
                                .Select(item => new
                                {
                                    ContactID = item.ContactID,
                                    CarrierID = item.ID,
                                    CarrierCode = item.Code,
                                    CarrierName = item.Name,
                                    CarrierQueryMark = item.QueryMark,
                                    CarrierAddress = item.Address,
                                }).FirstOrDefault();

            var contactInfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ContactsNew
                                .Where(t => t.ID == carrierInfo.ContactID
                                        && t.Status == Needs.Wl.Models.Enums.Status.Normal)
                                .Select(item => new
                                {
                                    ContactMobile = item.Mobile,
                                    ContactName = item.Name,
                                    ContactFax = item.Fax,
                                }).FirstOrDefault();

            var vehiclesInfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.VehiclesNew
                .Where(t => t.CarrierID == carrierId
                         && t.Status == Needs.Wl.Models.Enums.Status.Normal)
                .Select(item => new
                {
                    VehicleID = item.ID,
                    VehicleType = ((Needs.Wl.Models.Enums.VehicleType)Enum.Parse(typeof(Needs.Wl.Models.Enums.VehicleType),
                                    Enum.GetName(typeof(Needs.Wl.Models.Enums.VehicleType), item.Type))).GetDescription(),
                    item.License,
                    item.HKLicense,
                    item.Weight,
                    item.Size
                }).OrderBy(x => x.License);
            ///运输批次只显示中港贸易的
            var driversInfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DriversNew
               .Where(t => t.CarrierID == carrierId
                   && t.Status == Needs.Wl.Models.Enums.Status.Normal && t.IsChcd==true)
               .Select(item => new
               {
                   DriverID = item.ID,
                   DriverName = item.Name,
                   Mobile = item.Mobile,
                   HSCode = item.HSCode,
                   HKMobile = item.HKMobile,
                   DriverCardNo = item.DriverCardNo,
                   PortElecNo = item.PortElecNo,
                   LaoPaoCode = item.LaoPaoCode,
                   License = item.License,
               }).OrderBy(x => x.Mobile);

            Response.Write((new
            {
                success = true,
                Carrier = carrierInfo,
                Contact = contactInfo,
                Vehicles = vehiclesInfo,
                Drivers = driversInfo,
            }).Json());
        }

        /// <summary>
        /// 根据 运输批次号 获取 所有信息（承运商信息、承运商联系人信息、车辆信息、司机信息）
        /// </summary>
        protected void GetAllInfoByVoyageNo()
        {
            string voyageNo = Request.Form["VoyageNo"];

            var voyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.VoyagesNew.Where(t => t.ID == voyageNo).ToList();

            var carrier = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CarriersNew
                .Where(t => t.Code == voyage.FirstOrDefault().CarrierCode
                         && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();

            //TODO:代码Review 
            //var carrierQuery = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CarriersNew;
            //carrierQuery.Predicate = p => p.Code == voyage.FirstOrDefault().CarrierCode && p.Status == Needs.Wl.Models.Enums.Status.Normal;
            //var carrier = carrierQuery.FirstOrDefault();

            var voyage1 = voyage.Select(item => new
            {
                //运输批次
                VoyageNo = item.ID,
                VoyageTypeInt = (int)item.Type,
                VoyageTransportTime = item.TransportTime != null ? ((DateTime)item.TransportTime).ToString("yyyy-MM-dd") : string.Empty,
                VoyageSummary = item.Summary,

                //承运商及联系人
                CarrierID = carrier != null ? carrier.ID : string.Empty,
                CarrierTypeInt = item.CarrierType ?? Needs.Wl.Models.Enums.CarrierType.InteLogistics.GetHashCode(),
                IsInteLogistics = item.CarrierType == null ? true : item.CarrierType == Needs.Wl.Models.Enums.CarrierType.InteLogistics.GetHashCode(),
                CarrierType = item.CarrierType != null ? ((Needs.Wl.Models.Enums.CarrierType)item.CarrierType).GetDescription() : string.Empty,
                CarrierCode = item.CarrierCode,
                CarrierName = item.CarrierName,
                CarrierQueryMark = item.CarrierQueryMark,
                ContactMobile = item.ContactMobile,
                CarrierAddress = item.CarrierAddress,
                ContactName = item.ContactName,
                ContactFax = item.ContactFax,

                //车辆信息
                VehicleLicence = item.VehicleLicence,
                VehicleType = item.VehicleType != null ? ((Needs.Wl.Models.Enums.VehicleType)item.VehicleType).GetDescription() : string.Empty,
                VehicleWeight = item.VehicleWeight,
                VehicleSize = item.VehicleSize,
                VehicleHKLicense = item.HKLicense,

                //司机信息
                DriverName = item.DriverName,
                DriverMobile = item.DriverMobile,
                DriverHSCode = item.DriverHSCode,
                DriverHKMobile = item.DriverHKMobile,
                DriverCardNo = item.DriverCardNo,
                DriverPortElecNo = item.DriverPortElecNo,
                DriverLaoPaoCode = item.DriverLaoPaoCode,
                DriverLicence = item.DriverCode,
            })
                .FirstOrDefault();

            Response.Write((new
            {
                success = true,
                voyageInfo = voyage1,
            }).Json());
        }

        /// <summary>
        /// 精简保存
        /// </summary>
        protected void SaveSimple()
        {
            try
            {
                string voyageNo = Request.Form["VoyageNo"];
                string transportTime = Request.Form["TransportTime"];
                string voyageType = Request.Form["VoyageType"];
                int voyageTypeInt = int.Parse(voyageType);
                string voyageSummary = Request.Form["VoyageSummary"];
                DateTime transportTimeDateTime;
                DateTime.TryParse(transportTime, out transportTimeDateTime);

                voyageNo = voyageNo.Trim();
                var voyageTypeEnum = (Needs.Wl.Models.Enums.VoyageType)voyageTypeInt;
                voyageSummary = voyageSummary.Trim();

                //开始保存
                var voyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.VoyagesNew
                    .Where(t => t.ID == voyageNo && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();

                if (voyage == null)
                {
                    voyage = new Needs.Wl.Models.Voyage1();
                    voyage.ID = voyageNo;
                    voyage.TransportTime = transportTimeDateTime;
                    voyage.CutStatus = Needs.Wl.Models.Enums.CutStatus.UnCutting;
                    voyage.HKDeclareStatus = false;
                    voyage.Status = Needs.Wl.Models.Enums.Status.Normal;
                    voyage.CreateTime = DateTime.Now;

                }

                voyage.Type = voyageTypeEnum;
                voyage.UpdateDate = DateTime.Now;
                voyage.Summary = voyageSummary;

                voyage.Enter();

                Response.Write((new { success = true, msg = "保存成功", }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, msg = "保存失败：" + ex.Message, }).Json());
            }
        }

        /// <summary>
        /// 高级保存
        /// </summary>
        protected void SaveComplicated()
        {
            try
            {
                #region 获取提交的数据

                //高级编辑框的内容 运输批次
                //voyageNo、voyageTypeEnum、transportTimeDateTime、voyageSummary
                string voyageNo = Request.Form["VoyageNo"];
                string voyageType = Request.Form["VoyageType"];
                int voyageTypeInt = int.Parse(voyageType);
                string transportTime = Request.Form["TransportTime"];
                DateTime transportTimeDateTime;
                DateTime.TryParse(transportTime, out transportTimeDateTime);
                string voyageSummary = Request.Form["VoyageSummary"];

                voyageNo = voyageNo.Trim();
                var voyageTypeEnum = (Needs.Wl.Models.Enums.VoyageType)voyageTypeInt;
                voyageSummary = voyageSummary.Trim();

                //高级编辑框的内容 承运商
                //carrierTypeEnum、CarrierCode、CarrierName、QueryMark
                //ContactMobile、CarrierAddress、ContactName、Fax
                string CarrierType = Request.Form["CarrierType"];

                int CarrierTypeInt = 0;
                Dictionary<string, string> dicCarrierType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CarrierType>();
                foreach (var item in dicCarrierType)
                {
                    if (item.Value == CarrierType)
                    {
                        CarrierTypeInt = Convert.ToInt32(item.Key);
                        break;
                    }
                    if (item.Key == CarrierType)
                    {
                        CarrierTypeInt = Convert.ToInt32(item.Key);
                        break;
                    }
                }

                string CarrierCode = Request.Form["CarrierCode"];
                string CarrierName = Request.Form["CarrierName"];
                string QueryMark = Request.Form["QueryMark"];

                string ContactMobile = Request.Form["ContactMobile"];
                string CarrierAddress = Request.Form["CarrierAddress"];
                string ContactName = Request.Form["ContactName"];
                string Fax = Request.Form["Fax"];

                var carrierTypeEnum = (Needs.Wl.Models.Enums.CarrierType)CarrierTypeInt;
                CarrierCode = CarrierCode.Trim();
                CarrierName = CarrierName.Trim();
                QueryMark = QueryMark.Trim();

                ContactMobile = ContactMobile.Trim();
                CarrierAddress = CarrierAddress.Trim();
                ContactName = ContactName.Trim();
                Fax = Fax.Trim();

                //高级编辑框的内容 车辆
                //VehicleLicense、VehicleTypeEnum、VehicleWeight、VehicleHKLicense
                string VehicleLicense = Request.Form["VehicleLicense"];
                string VehicleType = Request.Form["VehicleType"];

                int VehicleTypeInt = 0;
                Dictionary<string, string> dicVehicleType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.VehicleType>();
                foreach (var item in dicVehicleType)
                {
                    if (item.Value == VehicleType)
                    {
                        VehicleTypeInt = Convert.ToInt32(item.Key);
                        break;
                    }
                    if (item.Key == VehicleType)
                    {
                        VehicleTypeInt = Convert.ToInt32(item.Key);
                        break;
                    }
                }



                string VehicleWeight = Request.Form["VehicleWeight"];
                string VehicleSize = Request.Form["VehicleSize"];
                string VehicleHKLicense = Request.Form["VehicleHKLicense"];

                VehicleLicense = VehicleLicense.Trim();
                var VehicleTypeEnum = (Needs.Wl.Models.Enums.VehicleType)VehicleTypeInt;
                VehicleWeight = VehicleWeight.Trim();
                VehicleSize = VehicleSize.Trim();
                VehicleHKLicense = VehicleHKLicense.Trim();

                //高级编辑框的内容 司机
                //DriverName、DriverMobile、DriverHSCode、DriverHKMobile
                //DriverDriverCardNo、DriverPortElecNo、DriverLaoPaoCode、DriverLicense
                string DriverName = Request.Form["DriverName"];
                string DriverMobile = Request.Form["DriverMobile"];
                string DriverHSCode = Request.Form["DriverHSCode"];
                string DriverHKMobile = Request.Form["DriverHKMobile"];

                string DriverDriverCardNo = Request.Form["DriverDriverCardNo"];
                string DriverPortElecNo = Request.Form["DriverPortElecNo"];
                string DriverLaoPaoCode = Request.Form["DriverLaoPaoCode"];
                string DriverLicense = Request.Form["DriverLicense"];

                DriverName = DriverName.Trim();
                DriverMobile = DriverMobile.Trim();
                DriverHSCode = DriverHSCode.Trim();
                DriverHKMobile = DriverHKMobile.Trim();

                DriverDriverCardNo = DriverDriverCardNo.Trim();
                DriverPortElecNo = DriverPortElecNo.Trim();
                DriverLaoPaoCode = DriverLaoPaoCode.Trim();
                DriverLicense = DriverLicense.Trim();

                #endregion

                #region 开始保存

                // ================================================ 开始保存 ================================================

                var voyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.VoyagesNew
                    .Where(t => t.ID == voyageNo && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();

                if (voyage == null)
                {
                    voyage = new Needs.Wl.Models.Voyage1();
                    voyage.ID = voyageNo;
                    voyage.CutStatus = Needs.Wl.Models.Enums.CutStatus.UnCutting;
                    voyage.HKDeclareStatus = false;
                    voyage.Status = Needs.Wl.Models.Enums.Status.Normal;
                    voyage.CreateTime = DateTime.Now;

                }

                //高级编辑框的内容 运输批次
                voyage.Type = voyageTypeEnum;
                voyage.UpdateDate = DateTime.Now;
                voyage.Summary = voyageSummary;
                voyage.TransportTime = transportTimeDateTime >= new DateTime(2, 1, 1) ? transportTimeDateTime : (DateTime?)null;

                //高级编辑框的内容 承运商
                //carrierTypeEnum、CarrierCode、CarrierName、QueryMark
                //ContactMobile、CarrierAddress、ContactName、Fax
                voyage.CarrierType = carrierTypeEnum.GetHashCode();
                voyage.CarrierCode = CarrierCode;
                voyage.CarrierName = CarrierName;
                voyage.CarrierQueryMark = QueryMark;

                voyage.ContactMobile = ContactMobile;
                voyage.CarrierAddress = CarrierAddress;
                voyage.ContactName = ContactName;
                voyage.ContactFax = Fax;

                //高级编辑框的内容 车辆
                //VehicleLicense、VehicleTypeEnum、VehicleWeight、VehicleHKLicense
                voyage.VehicleLicence = VehicleLicense;
                voyage.VehicleType = VehicleTypeEnum.GetHashCode();
                voyage.VehicleWeight = VehicleWeight;
                voyage.VehicleSize = VehicleSize;
                voyage.HKLicense = VehicleHKLicense;

                //高级编辑框的内容 司机
                //DriverName、DriverMobile、DriverHSCode、DriverHKMobile
                //DriverDriverCardNo、DriverPortElecNo、DriverLaoPaoCode、DriverLicense
                voyage.DriverName = DriverName;
                voyage.DriverMobile = DriverMobile;
                voyage.DriverHSCode = DriverHSCode;
                voyage.DriverHKMobile = DriverHKMobile;

                voyage.DriverCardNo = DriverDriverCardNo;
                voyage.DriverPortElecNo = DriverPortElecNo;
                voyage.DriverLaoPaoCode = DriverLaoPaoCode;
                voyage.DriverCode = DriverLicense;

                /////////////////////////////////////////////////////////////////////////////////////////////////////////


                #region 做一些限制（承运商、车辆、司机各自的编辑页面中的限制）
                Needs.Wl.Models.Voyage1 voyageCheck = new Needs.Wl.Models.Voyage1();
                //承运商 简称单独唯一，名称单独唯一
                //简称 + 名称
                voyageCheck.CarrierCode = voyage.CarrierCode;
                voyageCheck.CarrierName = voyage.CarrierName;

                //车辆 车牌号单独唯一，香港车牌号单独唯一
                //车牌号 + 车辆类型
                voyageCheck.VehicleLicence = voyage.VehicleLicence;
                voyageCheck.HKLicense = voyage.HKLicense;
                voyageCheck.VehicleType = voyage.VehicleType;

                //司机 手机号单独唯一
                //姓名 + 证件号码
                voyageCheck.DriverName = voyage.DriverName;
                voyageCheck.DriverCode = voyage.DriverCode;
                voyageCheck.DriverMobile = voyage.DriverMobile;

                string checkResult = CheckDatas(voyageCheck);
                if (!string.IsNullOrEmpty(checkResult))
                {
                    Response.Write((new { success = false, msg = checkResult, }).Json());
                    return;
                }


                #endregion


                voyage.Enter();


                //保存其他内容 承运商
                var carrier = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CarriersNew
                    .Where(t => t.Code == voyage.CarrierCode && t.Name == voyage.CarrierName && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();

                if (carrier == null)
                {
                    carrier = new Needs.Wl.Models.Carrier();

                    carrier.Code = voyage.CarrierCode;
                    carrier.Name = voyage.CarrierName;
                    carrier.CarrierType = (Needs.Wl.Models.Enums.CarrierType)voyage.CarrierType;
                    carrier.QueryMark = voyage.CarrierQueryMark;
                    carrier.Status = Needs.Wl.Models.Enums.Status.Normal;
                    carrier.UpdateDate = DateTime.Now;
                    carrier.Address = voyage.CarrierAddress;

                    carrier.CreateDate = DateTime.Now;
                    carrier.ID = string.Concat(carrier.Name, carrier.Code).MD5();
                }
                else
                {
                    carrier.Code = voyage.CarrierCode;
                    carrier.Name = voyage.CarrierName;
                    carrier.CarrierType = (Needs.Wl.Models.Enums.CarrierType)voyage.CarrierType;
                    carrier.QueryMark = voyage.CarrierQueryMark;
                    carrier.Status = Needs.Wl.Models.Enums.Status.Normal;
                    carrier.UpdateDate = DateTime.Now;
                    carrier.Address = voyage.CarrierAddress;
                }

                //保存其他内容 联系人
                var contact = new Needs.Wl.Models.Contact()
                {
                    CreateDate = DateTime.Now,
                    Status = Needs.Wl.Models.Enums.Status.Normal,
                };

                if (!string.IsNullOrEmpty(carrier.ContactID))
                {
                    //编辑
                    contact = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ContactsNew.Where(t => t.ID == carrier.ContactID && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();
                    contact.Name = voyage.ContactName;
                    contact.Mobile = voyage.ContactMobile;
                    contact.Fax = voyage.ContactFax;
                }
                else
                {
                    //新增
                    contact.Name = voyage.ContactName;
                    contact.Mobile = voyage.ContactMobile;
                    contact.Fax = voyage.ContactFax;
                    contact.ID = string.Concat(contact.Name, contact.CreateDate, contact.Email, contact.Mobile).MD5();
                }

                carrier.ContactID = contact.ID;

                contact.Enter();
                carrier.Enter();
                

                //保存其他内容 车辆
                var vehicle = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.VehiclesNew
                    .Where(t => t.Type == (Needs.Wl.Models.Enums.VehicleType)voyage.VehicleType
                             && t.License == voyage.VehicleLicence
                             && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();

                if (vehicle == null)
                {
                    vehicle = new Needs.Wl.Models.Vehicle();

                    vehicle.CarrierID = carrier.ID;
                    vehicle.Type = (Needs.Wl.Models.Enums.VehicleType)voyage.VehicleType;
                    vehicle.License = voyage.VehicleLicence;
                    vehicle.HKLicense = voyage.HKLicense;
                    vehicle.UpdateDate = DateTime.Now;
                    vehicle.Weight = voyage.VehicleWeight;
                    vehicle.Size = voyage.VehicleSize;

                    vehicle.CreateDate = DateTime.Now;
                    vehicle.Status = Needs.Wl.Models.Enums.Status.Normal;
                    vehicle.ID = string.Concat(vehicle.Type, vehicle.License).MD5();
                }
                else
                {
                    vehicle.CarrierID = carrier.ID;
                    vehicle.Type = (Needs.Wl.Models.Enums.VehicleType)voyage.VehicleType;
                    vehicle.License = voyage.VehicleLicence;
                    vehicle.HKLicense = voyage.HKLicense;
                    vehicle.UpdateDate = DateTime.Now;
                    vehicle.Weight = voyage.VehicleWeight;
                    vehicle.Size = voyage.VehicleSize;
                }

                vehicle.Enter();

                //保存其他内容 司机
                var driver = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DriversNew
                    .Where(t => t.Name == voyage.DriverName
                             && t.License == voyage.DriverCode
                             && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();

                if (driver == null)
                {
                    driver = new Needs.Wl.Models.Driver();

                    driver.CarrierID = carrier.ID;
                    driver.Name = voyage.DriverName;
                    driver.Code = null;
                    driver.Mobile = voyage.DriverMobile;
                    driver.License = voyage.DriverCode;
                    driver.UpdateDate = DateTime.Now;
                    driver.HSCode = voyage.DriverHSCode;
                    driver.DriverCardNo = voyage.DriverCardNo;
                    driver.HKMobile = voyage.DriverHKMobile;
                    driver.PortElecNo = voyage.DriverPortElecNo;
                    driver.LaoPaoCode = voyage.DriverLaoPaoCode;

                    driver.CreateDate = DateTime.Now;
                    driver.Status = Needs.Wl.Models.Enums.Status.Normal;
                    driver.ID = string.Concat(driver.Name, driver.License).MD5();
                }
                else
                {
                    driver.CarrierID = carrier.ID;
                    driver.Name = voyage.DriverName;
                    driver.Code = null;
                    driver.Mobile = voyage.DriverMobile;
                    driver.License = voyage.DriverCode;
                    driver.UpdateDate = DateTime.Now;
                    driver.HSCode = voyage.DriverHSCode;
                    driver.DriverCardNo = voyage.DriverCardNo;
                    driver.HKMobile = voyage.DriverHKMobile;
                    driver.PortElecNo = voyage.DriverPortElecNo;
                    driver.LaoPaoCode = voyage.DriverLaoPaoCode;
                }

                driver.Enter();

                #endregion

                Response.Write((new { success = true, msg = "保存成功", }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, msg = "保存失败：" + ex.Message, }).Json());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="voyage"></param>
        /// <returns></returns>
        private string CheckDatas(Needs.Wl.Models.Voyage1 voyageCheck)
        {
            StringBuilder sbRedult = new StringBuilder();

            //承运商 简称单独唯一，名称单独唯一
            //简称 + 名称
            //voyageCheck.CarrierCode = voyage.CarrierCode;
            //voyageCheck.CarrierName = voyage.CarrierName;

            var carrier = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CarriersNew
                .Where(t => t.Code == voyageCheck.CarrierCode
                         && t.Name == voyageCheck.CarrierName
                         && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();
            if (carrier == null)
            {
                carrier = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CarriersNew
                    .Where(t => t.Code == voyageCheck.CarrierCode
                             && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();
                if (carrier != null)
                {
                    sbRedult.Append("承运商简称已存在！<br>");
                }

                carrier = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CarriersNew
                    .Where(t => t.Name == voyageCheck.CarrierName
                             && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();
                if (carrier != null)
                {
                    sbRedult.Append("承运商名称已存在！<br>");
                }
            }

            //车辆 车牌号单独唯一，香港车牌号单独唯一
            //车牌号 + 车辆类型
            //voyageCheck.VehicleLicence = voyage.VehicleLicence;
            //voyageCheck.HKLicense = voyage.HKLicense;
            //voyageCheck.VehicleType = voyage.VehicleType;

            var vehicle = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.VehiclesNew
                .Where(t => t.License == voyageCheck.VehicleLicence
                         && t.Type == (Needs.Wl.Models.Enums.VehicleType)voyageCheck.VehicleType
                         && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();
            if (vehicle == null)
            {
                vehicle = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.VehiclesNew
                    .Where(t => t.License == voyageCheck.VehicleLicence
                             && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();
                if (vehicle != null)
                {
                    sbRedult.Append("车牌号已存在！<br>");
                }

                if (!string.IsNullOrEmpty(voyageCheck.HKLicense))
                {
                    vehicle = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.VehiclesNew
                    .Where(t => t.HKLicense == voyageCheck.HKLicense
                             && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();
                    if (vehicle != null)
                    {
                        sbRedult.Append("香港车牌号已存在！<br>");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(voyageCheck.HKLicense))
                {
                    string id = vehicle.ID;

                    vehicle = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.VehiclesNew
                        .Where(t => t.ID != id
                                 && t.HKLicense == voyageCheck.HKLicense
                                 && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();
                    if (vehicle != null)
                    {
                        sbRedult.Append("香港车牌号已存在！<br>");
                    }
                }
            }

            //司机 手机号单独唯一
            //姓名 + 证件号码
            //voyageCheck.DriverName = voyage.DriverName;
            //voyageCheck.DriverCode = voyage.DriverCode;
            //voyageCheck.DriverMobile = voyage.DriverMobile;

            var driver = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DriversNew
                .Where(t => t.Name == voyageCheck.DriverName
                         && t.License == voyageCheck.DriverCode
                         && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();

            if (driver == null)
            {
                driver = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DriversNew
                    .Where(t => t.Mobile == voyageCheck.DriverMobile
                             && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();

                if (driver != null)
                {
                    sbRedult.Append("司机手机号已存在！<br>");
                }
            }
            else
            {
                string id = driver.ID;

                driver = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DriversNew
                    .Where(t => t.ID != id
                             && t.Mobile == voyageCheck.DriverMobile
                             && t.Status == Needs.Wl.Models.Enums.Status.Normal).FirstOrDefault();

                if (driver != null)
                {
                    sbRedult.Append("司机手机号已存在！<br>");
                }
            }

            return sbRedult.ToString();
        }



    }

    /// <summary>
    /// 当前操作
    /// </summary>
    public enum CurrentAction
    {
        Add = 0,

        Edit = 1,
    }

    /// <summary>
    /// 编辑模式
    /// </summary>
    public enum EditMode
    {
        /// <summary>
        /// 精简
        /// </summary>
        Simple = 0,

        /// <summary>
        /// 高级
        /// </summary>
        Complicated = 1,
    }
}