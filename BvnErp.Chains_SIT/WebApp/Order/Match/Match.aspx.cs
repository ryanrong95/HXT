using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Match
{
    public partial class Match : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 到货信息
        /// </summary>
        protected void dataInputs()
        {
            string ID = Request.QueryString["ID"];
            using (var query = new Needs.Ccs.Services.Views.MatchView())
            {
                var view = query;
                view = view.SearchByOrderID(ID);
                view = view.SearchSortingDecStatusNo();
                view = view.SearchWarehouseType();

                var result = view.ToMyPage(1, 100);

                Response.Write(result.Json());
            }
        }

        protected void SplitCheck()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            List<MatchViewModel> selectedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MatchViewModel>>(Model);
            string orderid = selectedModel.FirstOrDefault().OrderID;
            //是否有未处理的产品变更和订单变更
            if (!isAnyUnClassifiedProduct(orderid))
            {
                Response.Write(new { canContinue = false, info = "存在未处理的产品变更!" }.Json());
                return;
            }

            ////选的带OrderItem的项数要小于原订单的orderItem项数，保证原订单要有orderItem
            //var SelectedOriginalModelCount = selectedModel.Where(t => t.OrderItemID != "" && t.OrderItemID != null).Select(t => t.Model).Distinct().Count();
            //var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderid];
            //if (SelectedOriginalModelCount >= order.Items.Count())
            //{
            //    Response.Write(new { canContinue = false, info = "拆分的项数要小于订单的项数" }.Json());
            //    return;
            //}

            Response.Write(new { canContinue = true, info = "" }.Json());
        }

        protected bool isAnyUnClassifiedProduct(string orderID)
        {
            var ModelChange = new Needs.Ccs.Services.Views.OrderItemChangeNoticesView().GetTop(1, item => item.OrderID == orderID && item.ProcessState == Needs.Ccs.Services.Enums.ProcessState.UnProcess).Count();
            if (ModelChange > 0)
            {
                return false;
            }
            //var orderChangeNotice = new Needs.Ccs.Services.Views.OrderChangeView().GetTop(1, item => item.OrderID == orderID&&item.processState != Needs.Ccs.Services.Enums.ProcessState.Processed).Count();
            //if (orderChangeNotice > 0)
            //{
            //    return false;
            //}           
            return true;
        }

        protected void SplitDeclare()
        {
            Dictionary<string, int> boxes = new Dictionary<string, int>();
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            string PayExchangeID = Request.Form["SplitPE"];
            List<MatchViewModel> selectedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MatchViewModel>>(Model);
            string orderid = selectedModel.FirstOrDefault().OrderID;
            var OriginOrder = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderid];

            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            Needs.Ccs.Services.Models.Order splitOrder = new Needs.Ccs.Services.Models.Order();
            splitOrder.SetAdmin(admin);

            splitOrder.MainOrderID = OriginOrder.MainOrderID;
            splitOrder.Type = OriginOrder.Type;
            splitOrder.AdminID = admin.ID;
            splitOrder.UserID = OriginOrder.UserID;
            splitOrder.Client = OriginOrder.Client;
            splitOrder.ClientAgreement = OriginOrder.ClientAgreement;
            splitOrder.Currency = OriginOrder.Currency;
            splitOrder.CustomsExchangeRate = OriginOrder.CustomsExchangeRate;
            splitOrder.RealExchangeRate = OriginOrder.RealExchangeRate;
            splitOrder.IsFullVehicle = OriginOrder.IsFullVehicle;
            splitOrder.IsLoan = OriginOrder.IsLoan;
            splitOrder.PackNo = selectedModel.Select(t => t.CaseNo).Distinct().Count();
            splitOrder.WarpType = OriginOrder.WarpType;
            splitOrder.DeclarePrice = selectedModel.Sum(t => t.TotalPrice);
            splitOrder.InvoiceStatus = InvoiceStatus.UnInvoiced;
            splitOrder.PaidExchangeAmount = 0;
            splitOrder.IsHangUp = OriginOrder.IsHangUp;
            splitOrder.OrderStatus = OrderStatus.QuoteConfirmed;
            splitOrder.Status = OriginOrder.Status;
            splitOrder.CreateDate = DateTime.Now;
            splitOrder.UpdateDate = DateTime.Now;
            splitOrder.Summary = OriginOrder.Summary;
            foreach (var payExchangeSupplier in OriginOrder.PayExchangeSuppliers)
            {
                splitOrder.PayExchangeSuppliers.Add(new OrderPayExchangeSupplier
                {
                    OrderID = splitOrder.ID,
                    ClientSupplier = payExchangeSupplier.ClientSupplier
                });
            }

            #region OrderConsignee
            splitOrder.OrderConsignee = new OrderConsignee();
            splitOrder.OrderConsignee.OrderID = splitOrder.ID;
            splitOrder.OrderConsignee.Type = OriginOrder.OrderConsignee.Type;
            splitOrder.OrderConsignee.ClientSupplier = OriginOrder.OrderConsignee.ClientSupplier;
            if (OriginOrder.OrderConsignee.Type == HKDeliveryType.SentToHKWarehouse)
            {
                splitOrder.OrderConsignee.Contact = null;
                splitOrder.OrderConsignee.Mobile = null;
                splitOrder.OrderConsignee.Address = null;
                splitOrder.OrderConsignee.PickUpTime = null;
                splitOrder.OrderConsignee.WayBillNo = OriginOrder.OrderConsignee.WayBillNo;
                splitOrder.OrderConsignee.CarrierID = OriginOrder.OrderConsignee.CarrierID;
            }
            else
            {
                splitOrder.OrderConsignee.Contact = OriginOrder.OrderConsignee.Contact;
                splitOrder.OrderConsignee.Mobile = OriginOrder.OrderConsignee.Mobile;
                splitOrder.OrderConsignee.Address = OriginOrder.OrderConsignee.Address;
                splitOrder.OrderConsignee.PickUpTime = OriginOrder.OrderConsignee.PickUpTime;
                splitOrder.OrderConsignee.WayBillNo = null;
            }
            #endregion

            #region OrderConsignor
            splitOrder.OrderConsignor = new OrderConsignor();
            splitOrder.OrderConsignor.OrderID = splitOrder.ID;
            splitOrder.OrderConsignor.Type = OriginOrder.OrderConsignor.Type;
            if (OriginOrder.OrderConsignor.Type == SZDeliveryType.PickUpInStore)
            {
                splitOrder.OrderConsignor.Contact = OriginOrder.OrderConsignor.Contact;
                splitOrder.OrderConsignor.Mobile = OriginOrder.OrderConsignor.Mobile;
                splitOrder.OrderConsignor.IDType = OriginOrder.OrderConsignor.IDType;
                splitOrder.OrderConsignor.IDNumber = OriginOrder.OrderConsignor.IDNumber;
                splitOrder.OrderConsignor.Address = null;
            }
            else
            {
                splitOrder.OrderConsignor.Name = OriginOrder.OrderConsignor.Name;
                splitOrder.OrderConsignor.Contact = OriginOrder.OrderConsignor.Contact;
                splitOrder.OrderConsignor.Mobile = OriginOrder.OrderConsignor.Mobile;
                splitOrder.OrderConsignor.Address = OriginOrder.OrderConsignor.Address;
                splitOrder.OrderConsignor.IDType = null;
                splitOrder.OrderConsignor.IDNumber = null;
            }
            #endregion


            try
            {
                MatchSplitOrderNew matchSplitOrder = new MatchSplitOrderNew(selectedModel, OriginOrder, splitOrder);
                matchSplitOrder.Handle();

                SCPayExchangeHandler payExchangeHandler = new SCPayExchangeHandler(splitOrder, OriginOrder);
                payExchangeHandler.AdjustPayExchange(PayExchangeID);

                Response.Write((new { success = true, message = "拆分成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "拆分失败：" + ex.Message }).Json());
            }
        }

        protected void ConfirmCheck()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            List<MatchViewModel> selectedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MatchViewModel>>(Model);
            string orderid = selectedModel.FirstOrDefault().OrderID;
            //是否有未处理的产品变更和订单变更
            if (!isAnyUnClassifiedProduct(orderid))
            {
                Response.Write(new { canContinue = false, info = "存在未处理的产品变更!" }.Json());
                return;
            }

            Response.Write(new { canContinue = true, info = "" }.Json());
        }

        protected void OrderDeliveryConfirm()
        {
            Dictionary<string, int> boxes = new Dictionary<string, int>();
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            List<MatchViewModel> selectedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MatchViewModel>>(Model);
            string orderid = selectedModel.FirstOrDefault().OrderID;
            var OriginOrder = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderid];

            try
            {
                MatchConfirmOrderNew confirmOrder = new MatchConfirmOrderNew(selectedModel, OriginOrder);
                confirmOrder.Handle();

                Response.Write((new { success = true, message = "确认成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "确认失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 验证是否需要处理付汇拆分
        /// </summary>
        protected void PayExchangeCheck()
        {
            string OrderID = Request.Form["OrderID"];
            string SplitTotalPrice = Request.Form["SplitTotalPrice"];

            try
            {
                var order = new Needs.Ccs.Services.Views.Orders2View().FirstOrDefault(t => t.ID == OrderID);

                var splitprice = decimal.Parse(SplitTotalPrice);
                //订单金额 减去 拆分金额 小于已付汇金额，需要对付汇进行拆分
                if ((order.DeclarePrice - splitprice) < order.PaidExchangeAmount)
                {
                    Response.Write((new { success = true, check = true, DeclarePrice = order.DeclarePrice, PaidExchangeAmount = order.PaidExchangeAmount, message = "验证成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = true, check = false, message = "验证成功" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "验证失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 付汇记录
        /// </summary>
        protected void PayExchangeRecord()
        {
            string OrderID = Request.QueryString["OrderID"];

            var view = new Needs.Ccs.Services.Views.PayExchangeApplieItemsOriginView().Where(t => t.OrderID == OrderID);
            var list = view.ToArray();

            Response.Write(new
            {
                rows = list.Select(
                            t => new
                            {
                                t.ID,
                                t.PayExchangeApplyID,
                                t.OrderID,
                                t.Amount,
                                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                                t.Status,
                                t.SupplierEnglishName,
                                t.BankName,
                                t.BankAccount,
                                PayExchangeApplyStatus = t.PayExchangeApplyStatus.GetDescription(),
                                t.IsAdvanceMoney,
                                t.FatherID
                            }
                         ).ToArray(),
                total = list.Count(),
            }.Json());
        }
    }
}