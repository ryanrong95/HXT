using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Stocks.Temporary
{
    public partial class Handle : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var waybill = new WaybillsTopView<PvCenterReponsitory>().Single(item => item.ID == ID);
            if (waybill != null)
            {
                this.Model.waybillData = new
                {
                    EnterCode = waybill.EnterCode,
                    WaybillCode = waybill.Code + " " + waybill.Subcodes,
                    Supplier = waybill.Supplier,
                    Place = waybill.Consignor.Place,
                    BankAddress = waybill.Consignor.Address,
                };
            }
        }

        protected object LoadFiles()
        {
            string ID = Request.QueryString["ID"];
            var query = new CenterFilesTopView().Where(item => item.WaybillID == ID);
            var linq = query.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                Url = PvWsOrder.Services.Common.FileDirectory.ServiceRoot + t.Url,
            });

            return linq;
        }

        protected object data()
        {
            string ID = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.TempStorages.GetTempStorage().Where(item => item.WaybillID == ID);
            return this.Paging(query, t => new
            {
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                Manufacturer = t.Manufacturer,
                PartNumber = t.PartNumber,
                DateCode = t.DateCode,
                Origin = t.Origin.GetOrigin().Code,
                Quantity = t.Quantity,
            });
        }

        /// <summary>
        /// 更新运单入仓号
        /// </summary>
        protected void Submit()
        {
            try
            {
                string ID = Request.Form["ID"];
                string EnterCode = Request.Form["EnterCode"];
                if (string.IsNullOrEmpty(EnterCode))
                {
                    throw new Exception("入仓号不能为空!!!");
                }
                else
                {
                    var count = new WsClientsTopView<PvWsOrderReponsitory>().Count(item => item.EnterCode == EnterCode);
                    if (count != 1)
                    {
                        throw new Exception("入仓号不正确，找不到对应的客户");
                    }
                }
                //更新运单的入仓号
                var waybill = new WaybillsTopView<PvCenterReponsitory>().Single(item => item.ID == ID);
                new Yahv.Services.Models.Waybills()
                {
                    ID = waybill.ID,
                    Code = waybill.Code,
                    FatherID = waybill.FatherID,
                    Type = waybill.Type,
                    Subcodes = waybill.Subcodes,
                    CarrierID = waybill.CarrierID,
                    ConsignorID = waybill.ConsignorID,
                    ConsigneeID = waybill.ConsigneeID,
                    FreightPayer = waybill.FreightPayer,
                    TotalParts = waybill.TotalParts,
                    TotalWeight = waybill.TotalWeight,
                    CarrierAccount = waybill.CarrierAccount,
                    VoyageNumber = waybill.VoyageNumber,
                    Condition = waybill.Condition ?? string.Empty,
                    CreateDate = waybill.CreateDate,
                    ModifyDate = waybill.ModifyDate,
                    EnterCode = EnterCode,
                    Status = waybill.Status,
                    CreatorID = waybill.CreatorID,
                    ModifierID = waybill.ModifierID,
                    TransferID = waybill.TransferID,
                    IsClearance = waybill.IsClearance,
                    Packaging = waybill.Packaging,
                    Supplier = waybill.Supplier,
                    ExcuteStatus = waybill.ExcuteStatus,
                    Summary = waybill.Summary,
                    TotalVolume = waybill.TotalVolume,
                }.Enter();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}