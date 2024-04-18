using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Orders.Common
{
    public partial class InWayBill : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboboxData();
                LoadData();
            }
        }

        private void LoadComboboxData()
        {
            //公司承运商
            this.Model.carrierData = Erp.Current.WsOrder.Carriers.Select(item => new { Value = item.ID, Text = item.Name });
        }

        private void LoadData()
        {
            this.Model.InputWaybillData = "";

            var orderId = Request.QueryString["ID"];
            var query = new PvWsOrder.Services.Views.OrderAlls().FirstOrDefault(o => o.ID == orderId);
            //收货运单
            if (query?.OrderInput?.Waybill != null)
            {
                var waybill = query?.OrderInput?.Waybill;
                this.Model.waybillData = new
                {
                    ID = waybill.ID,
                    FatherID = waybill.FatherID,
                    Code = waybill.Code,
                    Type = waybill.Type,
                    Subcodes = waybill.Subcodes,
                    CarrierID = waybill.CarrierID,
                    Consignor = new
                    {
                        ID = waybill.Consignor.ID,
                        Company = waybill.Consignor.Company,
                        Place = waybill.Consignor.Place,
                        Address = waybill.Consignor.Address,
                        Contact = waybill.Consignor.Contact,
                        Phone = waybill.Consignor.Phone,
                        Zipcode = waybill.Consignor.Zipcode,
                        Email = waybill.Consignor.Email,
                        CreateDate = waybill.Consignor.CreateDate,
                        IDType = waybill.Consignor?.IDType?.GetDescription(),
                        IDNumber = waybill.Consignor.IDNumber,
                    },
                    Consignee = new
                    {
                        ID = waybill.Consignee.ID,
                        Company = waybill.Consignee.Company,
                        Place = waybill.Consignee.Place,
                        Address = waybill.Consignee.Address,
                        Contact = waybill.Consignee.Contact,
                        Phone = waybill.Consignee.Phone,
                        Zipcode = waybill.Consignee.Zipcode,
                        Email = waybill.Consignee.Email,
                        CreateDate = waybill.Consignee.CreateDate,
                        IDType = waybill.Consignee?.IDType?.GetDescription(),
                        IDNumber = waybill.Consignee.IDNumber,
                    },
                    FreightPayer = waybill.FreightPayer,
                    TotalParts = waybill.TotalParts.GetValueOrDefault(),
                    TotalWeight = waybill.TotalWeight.GetValueOrDefault(),
                    TotalVolume = waybill.TotalVolume.GetValueOrDefault(),
                    CarrierAccount = waybill.CarrierAccount,
                    VoyageNumber = waybill.VoyageNumber,
                    Condition = waybill.Condition,
                    CreateDate = waybill.CreateDate,
                    ModifyDate = waybill.ModifyDate,
                    EnterCode = waybill.EnterCode,
                    Status = waybill.Status,
                    CreatorID = waybill.CreatorID,
                    ModifierID = waybill.ModifierID,
                    TransferID = waybill.TransferID,
                    IsClearance = waybill.IsClearance,
                    Packaging = waybill.PackageDec,
                    Supplier = waybill.Supplier,
                    ExcuteStatus = waybill.ExcuteStatus,
                    WayLoading = new
                    {
                        ID = waybill.WayLoading.ID,
                        TakingDate = waybill.WayLoading.TakingDate == null ? "" : waybill.WayLoading.TakingDate.ToString().Replace("T", ""),
                        TakingAddress = waybill.WayLoading.TakingAddress,
                        TakingContact = waybill.WayLoading.TakingContact,
                        TakingPhone = waybill.WayLoading.TakingPhone,
                        CarNumber1 = waybill.WayLoading.CarNumber1,
                        Driver = waybill.WayLoading.Driver,
                        Carload = waybill.WayLoading.Carload,
                        CreateDate = waybill.WayLoading.CreateDate,
                        ModifyDate = waybill.WayLoading.ModifyDate,
                        CreatorID = waybill.WayLoading.CreatorID,
                        ModifierID = waybill.WayLoading.ModifierID,
                    }
                };
            }
        }
    }
}