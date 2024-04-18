using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Linq;

namespace WebApp.Logistics.Voyage
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
            loadEditdata();
        }

        /// <summary>
        /// 初始化基本信息
        /// </summary>
        protected void LoadData()
        {
            //承运商
            this.Model.CarrierData = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.Where(x => x.CarrierType == CarrierType.InteLogistics && x.Status==Status.Normal).Select(item => new
            {
                ID = item.ID,
                Code = item.Code,
                Name = item.Name
            }).OrderBy(x => x.Name).Json();
        }


        protected void loadEditdata()
        {
            string ID = Request.QueryString["ID"]; //VoyageID
            this.Model.IDdata = ID.Json();
            if (!string.IsNullOrEmpty(ID))
            {
                var voyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure[ID];

                this.Model.ManifestInfo = new
                {
                    Voyage = voyage.ID,
                    CarrierID = voyage.Carrier?.ID,
                    Vehicle = voyage?.HKLicense,
                    Driver = voyage?.DriverName,
                    TransportTime = voyage?.TransportTime?.ToString("yyyy-MM-dd"),
                    Summary = voyage?.Summary,
                    VoyageType = (int)voyage?.Type,
                }.Json();
            }
            else
            {
                this.Model.ManifestInfo = new { }.Json();
            }

        }
        /// <summary>
        /// 根据承运商ID，加载车辆和司机
        /// </summary>
        /// <returns></returns>
        protected object GetVehicleDriverbyID()
        {
            var ID = Request.Form["CarrierID"];
          
            var data = new
            {
                Vehicles = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Vehicles.Where(x => x.Carrier.ID == ID && x.Status == Status.Normal).Select(item => new
                {
                    item.ID,
                    VehicleType = ((VehicleType)Enum.Parse(typeof(VehicleType), Enum.GetName(typeof(VehicleType), item.VehicleType))).GetDescription(),
                    item.License,
                    item.HKLicense
                }).OrderBy(x => x.License),
                Drivers = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Drivers.Where(x => x.Carrier.ID == ID && x.Status==Status.Normal).Select(item => new
                {
                    item.ID,
                    item.Mobile,
                    DriverName = item.Name,
                    item.License
                }).OrderBy(x => x.Mobile)
            };
            return data;
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save()
        {
            string carrierID = Request.Form["CarrierID"];
            var carrer = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers[carrierID] ?? new Needs.Ccs.Services.Models.Carrier();
            var ID = Request.Form["ID"];
            var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure[ID] ?? new Needs.Ccs.Services.Models.Voyage();
            entity.ID =ID;
            entity.Carrier = carrer;
            entity.HKLicense = Request.Form["HKLicense"] == null ? "" : Request.Form["HKLicense"];
            entity.DriverName = Request.Form["DriverName"] == null ? "" : Request.Form["DriverName"];
            entity.DriverCode= Request.Form["License"] == null ? "" : Request.Form["License"]; 
            entity.Summary = Request.Form["Summary"] == null ? "" : Request.Form["Summary"];
            entity.Type = (Needs.Ccs.Services.Enums.VoyageType)int.Parse(Request.Form["VoyageType"]);

            string strTransportTime = Request.Form["TransportTime"];
            DateTime dtTransportTime;
            if (!string.IsNullOrEmpty(strTransportTime) && DateTime.TryParse(strTransportTime, out dtTransportTime))
            {
                entity.TransportTime = dtTransportTime;
            }

            entity.EnterError += ManifestSure_EnterError;
            entity.EnterSuccess += ManifestSure_EnterSuccess;
            entity.Enter();
           
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManifestSure_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManifestSure_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}