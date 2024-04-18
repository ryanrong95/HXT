using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Quoted
{
    /// <summary>
    /// 已报价订单详情界面
    /// </summary>
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[ID];
            var invoice = order.Client.Invoice;
            this.Model.OrderInfo = new
            {
                order.ID,
                CompanyName = order.Client.Company.Name,
                DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                Currency = order.Currency,
                OrderStatus = order.OrderStatus.GetDescription(),
                InvoiceType = order.ClientAgreement.InvoiceType.GetDescription(),
                CreateDate = order.CreateDate.ToShortDateString(),

                Consignee = order.OrderConsignee,
                ConsigneeType = order.OrderConsignee.Type.GetDescription(),
                Consignor = order.OrderConsignor,
                ConsignorType = order.OrderConsignor.Type.GetDescription(),
                IDType = string.IsNullOrWhiteSpace(order.OrderConsignor.IDType) ? null :
                        ((IDType)Enum.Parse(typeof(IDType), order.OrderConsignor.IDType)).GetDescription(),
                IsFullVehicle = order.IsFullVehicle ? "是" : "否",
                IsLoan = order.IsLoan ? "是" : "否",
                PackNo = order.PackNo?.ToString() ?? "",
                WarpType = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.Where(pack => pack.Code == order.WarpType).FirstOrDefault()?.Name ?? order.WarpType,

                PayExchangeSuppliers = order.PayExchangeSuppliers,
                 // DeliveryFile = FileDirectory.Current.FileServerUrl + "/" + order.Files.Where(file => file.FileType == FileType.DeliveryFiles).FirstOrDefault()?.Url.ToUrl(),
                DeliveryFile = (DateTime.Compare(order.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0)
                    ? FileDirectory.Current.PvDataFileUrl + "/" + order.MainOrderFiles.Where(file => file.FileType == FileType.DeliveryFiles).FirstOrDefault()?.Url.ToUrl()
                    : FileDirectory.Current.FileServerUrl + "/" + order.MainOrderFiles.Where(file => file.FileType == FileType.DeliveryFiles).FirstOrDefault()?.Url.ToUrl(),
                Invoice = invoice,
                InvoiceDeliveryType = invoice.DeliveryType.GetDescription(),
                InvoiceConsignee = order.Client.InvoiceConsignee
            }.Json().Replace("'", "#39;");
        }

        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[ID];
            //税点
            var taxpoint = 1 + order.ClientAgreement.InvoiceTaxRate;
            //代理费率、最低代理费
            decimal agencyRate = order.AgencyFeeExchangeRate * order.ClientAgreement.AgencyRate;
            decimal minAgencyFee = order.ClientAgreement.MinAgencyFee;
            bool isAverage = order.DeclarePrice * agencyRate < minAgencyFee ? true : false;
            //平摊代理费
            var aveAgencyFee = (order.AgencyFee * taxpoint / order.Items.Count()).ToRound(4);

            Func<Needs.Ccs.Services.Models.OrderItem, object> convert = item => new
            {
                item.ID,
                Name = item.Category?.Name ?? item.Name,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice,
                Unit = item.Unit,
                Origin = item.Origin,
                GrossWeight = item.GrossWeight,
                TraiffRate = item.ImportTax.Rate,
                Traiff = item.ImportTax.Value,
                AddTaxRate = item.AddedValueTax.Rate,
                AddTax = item.AddedValueTax.Value,
                AgencyFee = isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint,
                InspectionFee = (item.InspectionFee.GetValueOrDefault() * taxpoint)
            };

            Response.Write(new
            {
                rows = order.Items.Select(convert).ToArray(),
                total = order.Items.Count()
            }.Json());
        }

        /// <summary>
        /// 初始化订单附件
        /// </summary>
        protected void dataFiles()
        {
            string orderID = Request.QueryString["ID"];
            if (!string.IsNullOrEmpty(orderID))
            {
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderID];
                var files = order.MainOrderFiles.Where(file => file.FileType == FileType.OriginalInvoice);
                var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
                Func<Needs.Ccs.Services.Models.MainOrderFile, object> convert = orderFile => new
                {
                    orderFile.ID,
                    orderFile.Name,
                    FileType = orderFile.FileType.GetDescription(),
                    orderFile.FileFormat,
                    Url = DateTime.Compare(orderFile.CreateDate, t1) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + orderFile.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + orderFile.Url.ToUrl(),
                   // Url = FileDirectory.Current.PvDataFileUrl + "/" + orderFile.Url.ToUrl()
                };

                Response.Write(new
                {
                    rows = files.Select(convert).ToList(),
                    total = files.Count()
                }.Json());
            }
            else
            {
                Response.Write(new { }.Json());
            }
        }
    }
}