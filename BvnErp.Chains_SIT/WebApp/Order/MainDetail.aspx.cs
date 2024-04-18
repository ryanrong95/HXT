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
using Needs.Wl;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Ccs.Services.Views;

namespace WebApp.Order
{
    /// <summary>
    /// 订单详情界面
    /// </summary>
    public partial class MainDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            string ReplaceQuotes = "这里是一个双引号";
            this.Model.ReplaceQuotes = ReplaceQuotes;

            string mainOrderID = Request.QueryString["ID"];

            var OrderIDs = new Orders2View().Where(item => item.MainOrderID == mainOrderID)
                          .Select(item => item.ID).ToList();

            List<MainDetailViewModel> itemViewModel = new List<MainDetailViewModel>();

            foreach (string orderid in OrderIDs)
            {
                //第二个参数订单，类型只是为了查询香港装箱信息，主订单用不到这个信息，所以固定传Inside
                Needs.Ccs.Services.Views.OrderDetailOrderItemsView view = new Needs.Ccs.Services.Views.OrderDetailOrderItemsView(orderid, OrderType.Inside);
                view.AllowPaging = false;
                List<Needs.Ccs.Services.Views.OrderDetailOrderItemsModel> list = (List<Needs.Ccs.Services.Views.OrderDetailOrderItemsModel>)view.ToList();
                foreach (var t in list)
                {
                    t.Model = t.Model.Replace("\"", ReplaceQuotes);
                    t.UnitPrice = t.UnitPrice.ToRound(4);
                    t.TotalPrice = t.TotalPrice.ToRound(2);
                    t.CategoryTypeName = t.GetSpecialType();
                }

                MainDetailViewModel item = new MainDetailViewModel();
                item.OrderItems = new List<OrderDetailOrderItemsModel>();
                item.OrderID = orderid;
                item.OrderItems = list;

                itemViewModel.Add(item);
            }

            this.Model.OrderItems = itemViewModel.Json();


            string ID = OrderIDs.FirstOrDefault();
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[ID];


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
                        ((Needs.Ccs.Services.Enums.IDType)Enum.Parse(typeof(Needs.Ccs.Services.Enums.IDType), order.OrderConsignor.IDType)).GetDescription(),
                //订单是否包车
                IsFullVehicle = order.IsFullVehicle ? "是" : "否",

                IsLoan = order.IsLoan ? "是" : "否",
                PackNo = order.PackNo?.ToString() ?? "",
                WarpType = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.Where(pack => pack.Code == order.WarpType).FirstOrDefault()?.Name ?? order.WarpType,

                PayExchangeSuppliers = order.PayExchangeSuppliers,
                DeliveryFile = (DateTime.Compare(order.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0)
                    ? FileDirectory.Current.PvDataFileUrl + "/" + order.MainOrderFiles.Where(file => file.FileType == FileType.DeliveryFiles).FirstOrDefault()?.Url.ToUrl()
                    : FileDirectory.Current.FileServerUrl + "/" + order.Files.Where(file => file.FileType == FileType.DeliveryFiles).FirstOrDefault()?.Url.ToUrl(),
               
                    // DeliveryFile = FileDirectory.Current.FileServerUrl + "/" + order.Files.Where(file => file.FileType == FileType.DeliveryFiles).FirstOrDefault()?.Url.ToUrl(),


            }.Json().Replace("'", "#39;");
            this.Model.OrderStatus = order.OrderStatus;
        }

        protected void data()
        {
            string ID = Request.QueryString["ID"];

            var OrderIDs = new Orders2View().Where(item => item.MainOrderID == ID)
                          .Select(item => item.ID).ToList();

            Func<Needs.Ccs.Services.Views.OrderDetailOrderItemsModel, object> convert = item => new
            {
                ID = item.OrderItemID,
                Name = !string.IsNullOrEmpty(item.OrderItemCategoryName) ? item.OrderItemCategoryName : item.OrderItemName,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.ToString("0.0000"),
                TotalPrice = item.TotalPrice.ToRound(2).ToString("0.00"),
                Unit = item.Unit,
                Origin = item.Origin,
                GrossWeight = item.GrossWeight?.ToString("0.0000"),               
            };

            foreach (string orderid in OrderIDs)
            {
                //第二个参数订单，类型只是为了查询香港装箱信息，主订单用不到这个信息，所以固定传Inside
                Needs.Ccs.Services.Views.OrderDetailOrderItemsView view = new Needs.Ccs.Services.Views.OrderDetailOrderItemsView(orderid, OrderType.Inside);
                view.AllowPaging = false;

                IList<Needs.Ccs.Services.Views.OrderDetailOrderItemsModel> list = view.ToList();

            }

        }


        /// <summary>
        /// 初始化订单附件
        /// </summary>
        protected void dataFiles()
        {
            string mainOrderID = Request.QueryString["ID"];

            var OrderIDs = new Orders2View().Where(item => item.MainOrderID == mainOrderID)
                          .Select(item => item.ID).ToList();

            string orderID = OrderIDs.FirstOrDefault();
            if (!string.IsNullOrEmpty(orderID))
            {
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[orderID];
                var files = order.MainOrderFiles.Where(file => file.FileType == FileType.OriginalInvoice);
                //判断是否从中心库读取文件
                var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
               
                Func<Needs.Ccs.Services.Models.MainOrderFile, object> convert = orderFile => new
                {
                    orderFile.ID,
                    orderFile.Name,
                    FileType = orderFile.FileType.GetDescription(),
                    orderFile.FileFormat,
                    Url = DateTime.Compare(orderFile.CreateDate, t1) > 0 ?FileDirectory.Current.PvDataFileUrl + "/" + orderFile?.Url.ToUrl():
                      FileDirectory.Current.FileServerUrl + "/" + orderFile.Url.ToUrl()
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