using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace WebApp.Order.Match
{
    public partial class MatchOld : Uc.PageBase
    {
        private  Needs.Ccs.Services.Models.Order OriginOrder { get; set; }
        private  List<MatchRowNo> rowNos { get; set; }
        private List<MatchRowNo> RowNos
        {
            get
            {
                if (rowNos == null)
                {
                    rowNos = getRowNoMatch();
                    return rowNos;
                }
                else
                {
                    return rowNos;
                }
            }
        }
        private class MatchRowNo
        {
            public string RowNo { get; set; }
            public string OrderItemID { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
            public string Name { get; set; }
            public string Unit { get; set; }
            public string Batch { get; set; }
        }

        private List<MatchRowNo> getRowNoMatch()
        {
            List<MatchRowNo> rows = new List<MatchRowNo>();
            string ID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[ID];
            var orderitems = order.Items.OrderBy(t => t.ID);
            int irow = 1;
            foreach (var t in orderitems)
            {
                MatchRowNo rowno = new MatchRowNo();
                rowno.RowNo = irow.ToString();
                rowno.OrderItemID = t.ID;
                rowno.UnitPrice = t.UnitPrice;
                rowno.TotalPrice = t.TotalPrice;
                rowno.Unit = t.Unit;
                rowno.Name =  t.Category==null? t.Name: t.Category.Name;
                rowno.Batch = t.Batch;
                rows.Add(rowno);
                irow++;
            }
            return rows;
        }

        private void getRowNoMatchSplit(string orderID)
        {
            List<MatchRowNo> rows = new List<MatchRowNo>();
            string ID = orderID;
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[ID];
            var orderitems = order.Items.OrderBy(t => t.ID);
            int irow = 1;
            foreach (var t in orderitems)
            {
                MatchRowNo rowno = new MatchRowNo();
                rowno.RowNo = irow.ToString();
                rowno.OrderItemID = t.ID;
                rowno.UnitPrice = t.UnitPrice;
                rowno.TotalPrice = t.TotalPrice;
                rowno.Unit = t.Unit;
                rowno.Name = t.Category == null ? t.Name : t.Category.Name;
                rowno.Batch = t.Batch;
                rows.Add(rowno);
                irow++;
            }
            this.rowNos = rows;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            rowNos = null;
            LoadData();
        }

        public void LoadData()
        {

        }
        /// <summary>
        /// 订单产品信息
        /// </summary>
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[ID];
            OriginOrder = order;

            List<CgDeliveriesTopViewModel> orderItemMatchs = new List<CgDeliveriesTopViewModel>();
            var orderitems = order.Items.OrderBy(t => t.ID);
            foreach (var t in orderitems)
            {
                CgDeliveriesTopViewModel match = new CgDeliveriesTopViewModel();
                match.RowNo = RowNos.Where(p => p.OrderItemID == t.ID).FirstOrDefault().RowNo;
                match.Name = t.Category == null ? t.Name : t.Category.Name;
                //match.Name = t.Name;
                match.Manufacturer = t.Manufacturer;
                match.Model = t.Model;
                match.Origin = t.Origin;
                match.Quantity = t.Quantity;
                match.UnitPrice = t.UnitPrice;
                match.TotalPrice = t.TotalPrice;
                match.OrderItemID = t.ID;
                match.Batch = t.Batch;
                orderItemMatchs.Add(match);
            }

            Response.Write(new
            {
                rows = orderItemMatchs.ToArray(),
                total = orderItemMatchs.Count()
            }.Json());
        }
        /// <summary>
        /// 到货信息
        /// </summary>
        protected void dataInputs()
        {
            string ID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[ID];
            var cgDeliveryView = new Needs.Ccs.Services.Views.CgDeliveriesTopViewOrigin().
                Where(item => item.MainOrderID == order.MainOrderID && (item.OrderID == order.ID || item.OrderID == null)).
                OrderBy(item => item.CaseNo).ToList();

            //cgDeliveryView = cgDeliveryView.Where(item => item.Type == 200).ToList();

            List<CgDeliveriesTopViewModel> orderItemMatchs = new List<CgDeliveriesTopViewModel>();
            var matchInfo = RowNos;
            foreach (var t in cgDeliveryView)
            {
                CgDeliveriesTopViewModel match = new CgDeliveriesTopViewModel();
                var item = matchInfo.Where(p => p.OrderItemID == t.OrderItemID).FirstOrDefault();
                match.ID = t.ID;
                match.CaseNo = t.CaseNo;
                match.RowNo = item == null ? "" : item.RowNo;
                match.Manufacturer = t.Manufacturer;
                match.Model = t.Model;
                match.Origin = t.Origin;
                match.Quantity = t.Quantity;
                match.UnitPrice = item == null ? 0m : item.UnitPrice;
                match.TotalPrice = Math.Round(match.UnitPrice.Value * match.Quantity, 2, MidpointRounding.AwayFromZero);
                match.Batch = t.Batch != null ? t.Batch : (item == null ? "" : item.Batch);
                match.Name = item == null ? "" : item.Name;
                match.Unit = item == null ? "" : item.Unit;
                orderItemMatchs.Add(match);
            }

            Response.Write(new
            {
                rows = orderItemMatchs.OrderBy(t => t.RowNo).OrderBy(t => t.CaseNo).ToArray(),
                total = orderItemMatchs.Count()
            }.Json());
        }
        protected void SplitDeclare()
        {
            Dictionary<string, int> boxes = new Dictionary<string, int>();
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            List<CgDeliveriesTopViewModel> selectedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CgDeliveriesTopViewModel>>(Model);
            int a = selectedModel.Where(t=>!string.IsNullOrEmpty(t.RowNo)).Select(t => t.RowNo).Distinct().Count();
            int b = selectedModel.Where(t => !string.IsNullOrEmpty(t.RowNo)).Count();
            if (a!=b)
            {
                foreach(var item in selectedModel)
                {
                    if (!boxes.ContainsKey(item.RowNo))
                    {
                        boxes[item.RowNo] = 0;
                    }
                    else
                    {
                        boxes[item.RowNo] += 1;
                    }
                }
                List<string> lisDupValues = boxes.Where(x => x.Value > 0).Select(x => x.Key).ToList();
                foreach(var item in lisDupValues)
                {
                    var dupModels = selectedModel.Where(t => t.RowNo == item).ToList();
                    string model = dupModels[0].Model;
                    string origin = dupModels[0].Origin;
                   
                    for(int i = 1; i < dupModels.Count(); i++)
                    {
                        //如果重复的序号，型号，产地不一致，说明不是多次装箱，则不允许匹配
                        //如果不是则是多次装修，允许
                        if (model != dupModels[i].Model || origin != dupModels[i].Origin)
                        {
                            Response.Write((new { success = false, message = "一条到货只能跟一个订单项目匹配!" }).Json());
                            return;
                        }
                        
                    }
                }
            }
            string orderid = selectedModel.FirstOrDefault().OrderID;
            getRowNoMatchSplit(orderid);
            var matchInfo = this.rowNos;
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderid];
            this.OriginOrder = order;
            //到货信息匹配OrderItemID
            foreach (var t in selectedModel)
            {
                if (!string.IsNullOrEmpty(t.RowNo))
                {
                    var item = matchInfo.Where(p => p.RowNo == t.RowNo).FirstOrDefault();
                    t.OrderItemID = item.OrderItemID;
                }
            }

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
            splitOrder.DeclarePrice = selectedModel.Sum(t => t.TotalPrice.Value);
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
                //MatchSplitOrder matchSplitOrder = new MatchSplitOrder(selectedModel, OriginOrder, splitOrder);
                //matchSplitOrder.Split();

                Response.Write((new { success = true, message = "拆分成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "拆分失败：" + ex.Message }).Json());
            }
        }

        protected void OrderDeliveryConfirm()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            List<CgDeliveriesTopViewModel> selectedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CgDeliveriesTopViewModel>>(Model);
            string orderid = selectedModel.FirstOrDefault().OrderID;
            getRowNoMatchSplit(orderid);
            var matchInfo = this.rowNos;
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderid];
            this.OriginOrder = order;
            //到货信息匹配OrderItemID
            foreach (var t in selectedModel)
            {
                if (!string.IsNullOrEmpty(t.RowNo))
                {
                    var item = matchInfo.Where(p => p.RowNo == t.RowNo).FirstOrDefault();
                    t.OrderItemID = item.OrderItemID;
                }
            }

            //没有被匹配的OrderItem
            var mappedItemIds = selectedModel.Select(t => t.OrderItemID).Distinct();
            var NotMappedItemIds = RowNos.Select(t => t.OrderItemID).Distinct().Where(t => !mappedItemIds.Contains(t)).Select(t => t).ToList();

            try
            {
                MatchConfirmOrder confirmOrder = new MatchConfirmOrder(selectedModel, OriginOrder, NotMappedItemIds);
                confirmOrder.Confirm();

                Response.Write((new { success = true, message = "确认成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "确认失败：" + ex.Message }).Json());
            }
        }

        protected void ConfirmCheck()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            List<CgDeliveriesTopViewModel> selectedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CgDeliveriesTopViewModel>>(Model);

            MatchCheck matchCheck = new MatchCheck(selectedModel.FirstOrDefault().OrderID);
            if (!matchCheck.CanMatch())
            {
                Response.Write(new { canContinue = false, info = "还有未处理的产品变更，不能确认!" }.Json());
                return;
            }
            
            getRowNoMatchSplit(selectedModel.FirstOrDefault().OrderID);
            var matchInfo = this.rowNos;
            var matchInfoRowNo = selectedModel.Where(t => !string.IsNullOrEmpty(t.RowNo)).ToList();
            if (!matchInfoRowNo.All(t => matchInfo.Any(b => b.RowNo == t.RowNo)))
            {
                Response.Write(new { canContinue = false, info = "匹配的序号需要在订单信息中出现!" }.Json());
                return;
            }

            //到货信息匹配OrderItemID
            foreach (var t in selectedModel)
            {
                if (!string.IsNullOrEmpty(t.RowNo))
                {
                    var item = matchInfo.Where(p => p.RowNo == t.RowNo).FirstOrDefault();
                    t.OrderItemID = item.OrderItemID;
                }
            }

            //没有被匹配的OrderItem
            var mappedItemIds = selectedModel.Select(t => t.OrderItemID).Distinct();
            var NotMappedItemIds = RowNos.Select(t => t.OrderItemID).Distinct().Where(t => !mappedItemIds.Contains(t));

            bool IsNeededChecked = false;
            string showMsg = "";

            if (selectedModel.Where(t => t.OrderItemID == null).Count() > 0)
            {
                IsNeededChecked = true;
                showMsg += "有未匹配订单项的到货信息,";
            }

            if (NotMappedItemIds.Count() > 0)
            {
                IsNeededChecked = true;
                showMsg += "有未匹配到货信息的订单项,如果点击确认,将删除该订单项";
            }

            showMsg += " 是否继续?";

            Response.Write(new { canContinue = true, result = IsNeededChecked, info = showMsg }.Json());
        }

        protected void SplitCheck()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            List<CgDeliveriesTopViewModel> selectedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CgDeliveriesTopViewModel>>(Model);
            string orderid = selectedModel.FirstOrDefault().OrderID;
            getRowNoMatchSplit(orderid);
            var matchInfo = this.rowNos;
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderid];
            this.OriginOrder = order;

            bool IsNeededChecked = false;
            var matchInfoRowNo = selectedModel.Where(t => !string.IsNullOrEmpty(t.RowNo)).ToList();
            if (!matchInfoRowNo.All(t => matchInfo.Any(b => b.RowNo == t.RowNo)))
            {
                Response.Write(new { canContinue = false, info = "匹配的序号需要在订单信息中出现!" }.Json());
                return;
            }

            //是否有未处理的产品变更和订单变更
            if (!isAnyUnClassifiedProduct(orderid))
            {
                Response.Write(new { canContinue = false, info = "存在未处理的产品变更!" }.Json());
                return;
            }
          
            //到货信息匹配OrderItemID
            foreach (var t in selectedModel)
            {
                if (!string.IsNullOrEmpty(t.RowNo))
                {
                    var item = matchInfo.Where(p => p.RowNo == t.RowNo).FirstOrDefault();
                    t.OrderItemID = item.OrderItemID;
                }
            }

            //没有被匹配的OrderItem
            var mappedItemIds = selectedModel.Select(t => t.OrderItemID).Distinct();
            var NotMappedItemIds = RowNos.Select(t => t.OrderItemID).Distinct().Where(t => !mappedItemIds.Contains(t));

            if (NotMappedItemIds.Count() > 0)
            {
                //如果有未匹配到货的订单项，则该订单可以拆分
                Response.Write(new { canContinue = true, result = IsNeededChecked, info = "" }.Json());
            }
            else
            {
                //如果所有的订单项都被匹配，则需跟多检测，是否可以拆分
                if (selectedModel.Count == 1 && OriginOrder.Items.Count == 1)
                {
                    if (selectedModel.FirstOrDefault().Quantity >= OriginOrder.Items.FirstOrDefault().Quantity)
                    {
                        Response.Write(new { canContinue = false, info = "拆分的项数要小于订单的项数" }.Json());
                    }
                    else
                    {
                        Response.Write(new { canContinue = true, result = IsNeededChecked, info = "" }.Json());
                    }
                }
                else
                {
                    var SelectedOriginalModelCount = selectedModel.Where(t => t.OrderItemID != "" || t.OrderItemID != null).Select(t => t.Model).Distinct().Count();
                    if (SelectedOriginalModelCount > OriginOrder.Items.Select(t => t.Model).Distinct().Count())
                    {
                        Response.Write(new { canContinue = false, info = "拆分的项数要小于订单的项数" }.Json());
                    }
                    else if (SelectedOriginalModelCount == OriginOrder.Items.Select(t => t.Model).Distinct().Count())
                    {
                        var selectedQty = selectedModel.Where(t => t.OrderItemID != "" || t.OrderItemID != null).Sum(t => t.Quantity);
                        var selectedItemIDs = selectedModel.Where(t => t.OrderItemID != "" || t.OrderItemID != null).Select(t => t.OrderItemID).ToList();
                        var orderQty = OriginOrder.Items.Where(t => selectedItemIDs.Contains(t.ID)).Sum(t => t.Quantity);
                        if(selectedQty>= orderQty)
                        {
                            Response.Write(new { canContinue = false, info = "拆分的项数要小于订单的项数" }.Json());
                        }
                        else
                        {
                            Response.Write(new { canContinue = true, result = IsNeededChecked, info = "" }.Json());
                        }
                    }
                    else
                    {
                        string info = "";
                        //判断是否有没有匹配订单项的到货
                        var count = selectedModel.Where(t => t.RowNo == "").Count();
                        if (count > 0)
                        {
                            IsNeededChecked = true;
                            info = "到货信息未匹配订单项，是否继续?";
                        }

                        Response.Write(new { canContinue = true, result = IsNeededChecked, info = info }.Json());
                    }
                }

            }
        }

        protected bool IsOrderReturned()
        {
            string orderID = Request.Form["OrderID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderID];

            return order.OrderStatus == OrderStatus.Returned;
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
    }
}