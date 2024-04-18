using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Packing
{
    /// <summary>
    /// 装箱信息
    /// 展示订单及库房分拣信息
    /// </summary>
    public partial class Display : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[ID];
            this.Model.Order = new
            {
                order.ID,
                order.IsHangUp
            }.Json();
        }

        /// <summary>
        /// 订单产品信息
        /// </summary>
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[ID];

            Func<Needs.Ccs.Services.Models.OrderItem, object> convert = item => new
            {
                item.ID,
                Batch = item.Batch,
                Name = item.Category == null ? item.Name : item.Category.Name,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Quantity = item.Quantity,
                DeclaredQuantity = item.DeclaredQuantity ?? 0,
                Origin = item.Origin,
                GrossWeight = item.GrossWeight,
                TotalPrice = item.TotalPrice.ToRound(2),
                ProductDeclareStatus = item.ProductDeclareStatus.GetDescription()
            };

            Response.Write(new
            {
                rows = order.Items.Select(convert).ToArray(),
                total = order.Items.Count()
            }.Json());
        }

        /// <summary>
        /// 装箱信息
        /// </summary>
        protected void dataPackings()
        {
            string OrderID = Request.QueryString["ID"];
            var datas = new SortingsView().GetSortingPackingFromDeliveriesTopView(OrderID);
            Func<SortingPackingFromDeliveriesTopViewModel, object> convert = item => new
            {
                ID = item.SortingID,
                //PackingID = item.Packing.ID,
                SortingID = item.SortingID,
                BoxIndex = item.BoxIndex,
                //NetWeight = item.NetWeight,
                GrossWeight = item.GrossWeight,
                Model = item.Model,//产品型号
                //ProductName = item.OrderItem.Name,  //产品名称
                CustomsName = item.CustomsName,  //报关品名
                Quantity = item.Quantity,
                Origin = item.OriginCode,
                Manufacturer = item.Manufacturer,
                DecStatus = item.DecStatus.GetDescription(),
                Status = item.PackingStatus.GetDescription(),
                PickDate = item.PackingDate?.ToString("yyyy-MM-dd")
            };
            Response.Write(new
            {
                rows = datas.Select(convert).ToArray(),
                total = datas.Count()
            }.Json());
        }

        protected void SplitDeclare()
        {
            try
            {
                string approveOnOff = ConfigurationManager.AppSettings["ApproveOnOff"];

                string ReferenceInfo1 = Request.Form["ReferenceInfo1"];
                string ReferenceInfo2 = Request.Form["ReferenceInfo2"];

                string[] BoxIndexs = Request.Form["id"].Split(',');
                string OrderID = Request.Form["OrderID"];
                List<string> packs = new List<string>();
                foreach (var box in BoxIndexs)
                {
                    if (!packs.Contains(box))
                    {
                        packs.Add(box);
                    }
                }

                string rtnMsg = string.Empty;

                if (string.IsNullOrEmpty(approveOnOff) || "no" == approveOnOff)
                {
                    rtnMsg = DoSplitOrderPage(OrderID, packs);
                }
                else
                {
                    rtnMsg = PreApproveSplitOrder(approveOnOff, OrderID, packs, ReferenceInfo1, ReferenceInfo2);
                }

                if (string.IsNullOrEmpty(rtnMsg))
                {
                    if (string.IsNullOrEmpty(approveOnOff) || "no" == approveOnOff)
                    {
                        Response.Write((new { success = true, message = "拆分成功" }).Json());
                    }
                    else
                    {
                        Response.Write((new { success = true, message = "请等待审批通过，可在“订单审批”中查看审批状态" }).Json());
                    }
                }
                else
                {
                    Response.Write((new { success = false, message = rtnMsg }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "拆分失败：" + ex.Message }).Json());
            }
        }

        private string PreApproveSplitOrder(string approveOnOff, string OrderID, List<string> packs, string ReferenceInfo1, string ReferenceInfo2)
        {
            string rtnMsg = string.Empty;

            //检查是否只有一个箱子 Begin
            rtnMsg = CheckOnlyOneBox(OrderID, packs);
            if (!string.IsNullOrEmpty(rtnMsg))
            {
                return rtnMsg;
            }
            //检查是否只有一个箱子 End

            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

            string eventInfo = JsonConvert.SerializeObject(new EventInfoSplitOrder
            {
                ApplyAdminName = admin.RealName,
                TinyOrderID = OrderID,
                Packs = packs,
            });

            var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(
                                        admin.ID,
                                        Needs.Ccs.Services.Enums.ApprovalType.SplitOrderApproval,
                                        null,
                                        OrderID,
                                        null,
                                        eventInfo);
            if (attachApproval.IsUnApprovedConflictEvent)
            {
                rtnMsg = "订单 " + OrderID + " 有未审批的订单申请：" + attachApproval.StrUnApprovedConflictEventTypes;
                return rtnMsg;
            }
            attachApproval.GenerateUnApprovalInfo();

            if ("auto" == approveOnOff) //自动审批
            {
                string approveManID = ConfigurationManager.AppSettings["ApproveManID"];
                var XDTAdmin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(approveManID);
                string referenceInfo = ReferenceInfo1 + "这是一个超级分隔符" + ReferenceInfo2;  //attachApproval.GetReferenceInfoHtmlForSplitOrder(OrderID);
                attachApproval.ApproveSuccess(XDTAdmin, referenceInfo, isAuto: true);  //审批通过
                attachApproval.ExecuteTargetOperation();  //执行目标操作
            }

            return string.Empty;
        }

        /// <summary>
        /// 检查是否只有一个箱子
        /// </summary>
        /// <param name="OrderID"></param>
        /// <param name="packs"></param>
        /// <returns></returns>
        private string CheckOnlyOneBox(string OrderID, List<string> packs)
        {
            var packingBill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.SortingPackings.GetSortingPacking();
            var datas = packingBill.Where(Item => Item.OrderID == OrderID && packs.Contains(Item.BoxIndex));
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[OrderID];
            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

            SplitOrder splitOrder = new SplitOrder();
            splitOrder.Operator = admin;
            splitOrder.Packs = packs;
            splitOrder.OldOrderID = OrderID;
            splitOrder.MainOrderID = order.MainOrderID;

            splitOrder.SplitQuantity = new Dictionary<string, decimal>();
            foreach (var t in datas)
            {
                if (!splitOrder.SplitQuantity.ContainsKey(t.OrderItem.ID))
                {
                    splitOrder.SplitQuantity.Add(t.OrderItem.ID, t.OrderItem.Quantity);
                }

                //如果两个Orderitem,被装在两个不同的箱子，则需要将两个OrderItem的数量加总
                if (splitOrder.SplitItems.Any(p => p.ID == t.OrderItem.ID))
                {
                    var item = splitOrder.SplitItems.Where(p => p.ID == t.OrderItem.ID).FirstOrDefault();
                    item.Quantity += t.Quantity;
                }
                else
                {
                    t.OrderItem.Quantity = t.Quantity;
                    splitOrder.SplitItems.Add(t.OrderItem);
                }
            }

            var sumqty = splitOrder.SplitItems.Sum(t => t.Quantity);
            var orderSumqty = order.Items.Sum(t => t.Quantity);
            if (sumqty == orderSumqty)
            {
                return "拆分失败：拆分数量不能与原订单数量相同";
            }

            return string.Empty;
        }

        /// <summary>
        /// 页面上做拆分订单操作
        /// </summary>
        /// <param name="OrderID"></param>
        /// <param name="packs"></param>
        /// <returns></returns>
        private string DoSplitOrderPage(string OrderID, List<string> packs)
        {
            try
            {
                //var packingBill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.SortingPackings.GetSortingPacking();
                //var datas = packingBill.Where(Item => Item.OrderID == OrderID && packs.Contains(Item.BoxIndex));
                var datas = new Needs.Ccs.Services.Views.SortingsView().GetSortingPackingFromDeliveriesTopView(OrderID, packs.ToArray());

                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[OrderID];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                SplitOrder splitOrder = new SplitOrder();
                splitOrder.Operator = admin;
                splitOrder.Packs = packs;
                splitOrder.OldOrderID = OrderID;
                splitOrder.MainOrderID = order.MainOrderID;

                splitOrder.SplitQuantity = new Dictionary<string, decimal>();
                splitOrder.OrderItemInputMap = new Dictionary<string, string>();

                foreach (var t in datas)
                {
                    var theOrderItem = new OrderItemsView().Where(item => item.ID == t.OrderItemID).FirstOrDefault();

                    if (!splitOrder.SplitQuantity.ContainsKey(t.OrderItemID))
                    {
                        splitOrder.SplitQuantity.Add(t.OrderItemID, theOrderItem.Quantity);
                        var inputid = datas.Where(p => p.OrderItemID == t.OrderItemID).FirstOrDefault();
                        splitOrder.OrderItemInputMap.Add(t.OrderItemID, inputid.InputID);
                    }

                    //如果两个Orderitem,被装在两个不同的箱子，则需要将两个OrderItem的数量加总
                    if (splitOrder.SplitItems.Any(p => p.ID == t.OrderItemID))
                    {
                        var item = splitOrder.SplitItems.Where(p => p.ID == t.OrderItemID).FirstOrDefault();
                        item.Quantity += t.Quantity;
                    }
                    else
                    {
                        //t.OrderItem.Quantity = t.Quantity;
                        //splitOrder.SplitItems.Add(t.OrderItem);


                        theOrderItem.Quantity = t.Quantity;
                        splitOrder.SplitItems.Add(theOrderItem);
                    }
                }

                var sumqty = splitOrder.SplitItems.Sum(t => t.Quantity);
                var orderSumqty = order.Items.Sum(t => t.Quantity);
                if (sumqty == orderSumqty)
                {
                    return "拆分失败：拆分数量不能与原订单数量相同";
                }

                splitOrder.Type = order.Type;
                splitOrder.AdminID = admin.ID;
                splitOrder.UserID = order.UserID;
                splitOrder.Client = order.Client;
                splitOrder.ClientAgreement = order.ClientAgreement;
                splitOrder.Currency = order.Currency;
                splitOrder.CustomsExchangeRate = order.CustomsExchangeRate;
                splitOrder.RealExchangeRate = order.RealExchangeRate;
                splitOrder.IsFullVehicle = order.IsFullVehicle;
                splitOrder.IsLoan = order.IsLoan;
                splitOrder.PackNo = packs.Count;
                splitOrder.WarpType = order.WarpType;
                splitOrder.DeclarePrice = splitOrder.SplitItems.Sum(t => t.TotalPrice);
                splitOrder.InvoiceStatus = InvoiceStatus.UnInvoiced;
                splitOrder.PaidExchangeAmount = 0;
                splitOrder.IsHangUp = order.IsHangUp;
                splitOrder.OrderStatus = OrderStatus.QuoteConfirmed;
                splitOrder.Status = order.Status;
                splitOrder.CreateDate = DateTime.Now;
                splitOrder.UpdateDate = DateTime.Now;
                splitOrder.Summary = order.Summary;
                foreach (var payExchangeSupplier in order.PayExchangeSuppliers)
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
                splitOrder.OrderConsignee.Type = order.OrderConsignee.Type;
                splitOrder.OrderConsignee.ClientSupplier = order.OrderConsignee.ClientSupplier;
                if (order.OrderConsignee.Type == HKDeliveryType.SentToHKWarehouse)
                {
                    splitOrder.OrderConsignee.Contact = null;
                    splitOrder.OrderConsignee.Mobile = null;
                    splitOrder.OrderConsignee.Address = null;
                    splitOrder.OrderConsignee.PickUpTime = null;
                    splitOrder.OrderConsignee.WayBillNo = order.OrderConsignee.WayBillNo;
                }
                else
                {
                    splitOrder.OrderConsignee.Contact = order.OrderConsignee.Contact;
                    splitOrder.OrderConsignee.Mobile = order.OrderConsignee.Mobile;
                    splitOrder.OrderConsignee.Address = order.OrderConsignee.Address;
                    splitOrder.OrderConsignee.PickUpTime = order.OrderConsignee.PickUpTime;
                    splitOrder.OrderConsignee.WayBillNo = null;
                }
                #endregion

                #region OrderConsignor
                splitOrder.OrderConsignor = new OrderConsignor();
                splitOrder.OrderConsignor.OrderID = splitOrder.ID;
                splitOrder.OrderConsignor.Type = order.OrderConsignor.Type;
                if (order.OrderConsignor.Type == SZDeliveryType.PickUpInStore)
                {
                    splitOrder.OrderConsignor.Contact = order.OrderConsignor.Contact;
                    splitOrder.OrderConsignor.Mobile = order.OrderConsignor.Mobile;
                    splitOrder.OrderConsignor.IDType = order.OrderConsignor.IDType;
                    splitOrder.OrderConsignor.IDNumber = order.OrderConsignor.IDNumber;
                    splitOrder.OrderConsignor.Address = null;
                }
                else
                {
                    splitOrder.OrderConsignor.Name = order.OrderConsignor.Name;
                    splitOrder.OrderConsignor.Contact = order.OrderConsignor.Contact;
                    splitOrder.OrderConsignor.Mobile = order.OrderConsignor.Mobile;
                    splitOrder.OrderConsignor.Address = order.OrderConsignor.Address;
                    splitOrder.OrderConsignor.IDType = null;
                    splitOrder.OrderConsignor.IDNumber = null;
                }
                #endregion


                splitOrder.Split();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return "拆分失败：" + ex.Message;
            }
        }

        /// <summary>
        /// 注意，该函数有反射调用
        /// </summary>
        /// <param name="OrderID"></param>
        /// <param name="packs"></param>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public string DoSplitOrderAuto(string OrderID, List<string> packs, string adminID)
        {
            try
            {
                //var packingBill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.SortingPackings.GetSortingPacking();
                //var packingBill = new Needs.Ccs.Services.Views.SortingsView().GetSortingPacking();
                //var datas = packingBill.Where(Item => Item.OrderID == OrderID && packs.Contains(Item.BoxIndex));
                var datas = new Needs.Ccs.Services.Views.SortingsView().GetSortingPackingFromDeliveriesTopView(OrderID, packs.ToArray());
                //var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[OrderID];
                var order = new Needs.Ccs.Services.Views.OrdersView()[OrderID];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(adminID);

                SplitOrder splitOrder = new SplitOrder();
                splitOrder.Operator = admin;
                splitOrder.Packs = packs;
                splitOrder.OldOrderID = OrderID;
                splitOrder.MainOrderID = order.MainOrderID;

                splitOrder.SplitQuantity = new Dictionary<string, decimal>();
                splitOrder.OrderItemInputMap = new Dictionary<string, string>();
                foreach (var t in datas)
                {
                    var theOrderItem = new OrderItemsView().Where(item => item.ID == t.OrderItemID).FirstOrDefault();

                    if (!splitOrder.SplitQuantity.ContainsKey(t.OrderItemID))
                    {
                        splitOrder.SplitQuantity.Add(t.OrderItemID, theOrderItem.Quantity);
                        var inputid = datas.Where(p => p.OrderItemID == t.OrderItemID).FirstOrDefault();
                        splitOrder.OrderItemInputMap.Add(t.OrderItemID, inputid.InputID);
                    }

                    //如果两个Orderitem,被装在两个不同的箱子，则需要将两个OrderItem的数量加总
                    if (splitOrder.SplitItems.Any(p => p.ID == t.OrderItemID))
                    {
                        var item = splitOrder.SplitItems.Where(p => p.ID == t.OrderItemID).FirstOrDefault();
                        item.Quantity += t.Quantity;
                    }
                    else
                    {
                        //t.OrderItem.Quantity = t.Quantity;
                        //splitOrder.SplitItems.Add(t.OrderItem);


                        theOrderItem.Quantity = t.Quantity;
                        splitOrder.SplitItems.Add(theOrderItem);
                    }
                }

                var sumqty = splitOrder.SplitItems.Sum(t => t.Quantity);
                var orderSumqty = order.Items.Sum(t => t.Quantity);
                if (sumqty == orderSumqty)
                {
                    return "拆分失败：拆分数量不能与原订单数量相同";
                }

                splitOrder.Type = order.Type;
                splitOrder.AdminID = admin.ID;
                splitOrder.UserID = order.UserID;
                splitOrder.Client = order.Client;
                splitOrder.ClientAgreement = order.ClientAgreement;
                splitOrder.Currency = order.Currency;
                splitOrder.CustomsExchangeRate = order.CustomsExchangeRate;
                splitOrder.RealExchangeRate = order.RealExchangeRate;
                splitOrder.IsFullVehicle = order.IsFullVehicle;
                splitOrder.IsLoan = order.IsLoan;
                splitOrder.PackNo = packs.Count;
                splitOrder.WarpType = order.WarpType;
                splitOrder.DeclarePrice = splitOrder.SplitItems.Sum(t => t.TotalPrice);
                splitOrder.InvoiceStatus = InvoiceStatus.UnInvoiced;
                splitOrder.PaidExchangeAmount = 0;
                splitOrder.IsHangUp = order.IsHangUp;
                splitOrder.OrderStatus = OrderStatus.QuoteConfirmed;
                splitOrder.Status = order.Status;
                splitOrder.CreateDate = DateTime.Now;
                splitOrder.UpdateDate = DateTime.Now;
                splitOrder.Summary = order.Summary;
                foreach (var payExchangeSupplier in order.PayExchangeSuppliers)
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
                splitOrder.OrderConsignee.Type = order.OrderConsignee.Type;
                splitOrder.OrderConsignee.ClientSupplier = order.OrderConsignee.ClientSupplier;
                if (order.OrderConsignee.Type == HKDeliveryType.SentToHKWarehouse)
                {
                    splitOrder.OrderConsignee.Contact = null;
                    splitOrder.OrderConsignee.Mobile = null;
                    splitOrder.OrderConsignee.Address = null;
                    splitOrder.OrderConsignee.PickUpTime = null;
                    splitOrder.OrderConsignee.WayBillNo = order.OrderConsignee.WayBillNo;
                }
                else
                {
                    splitOrder.OrderConsignee.Contact = order.OrderConsignee.Contact;
                    splitOrder.OrderConsignee.Mobile = order.OrderConsignee.Mobile;
                    splitOrder.OrderConsignee.Address = order.OrderConsignee.Address;
                    splitOrder.OrderConsignee.PickUpTime = order.OrderConsignee.PickUpTime;
                    splitOrder.OrderConsignee.WayBillNo = null;
                }
                #endregion

                #region OrderConsignor
                splitOrder.OrderConsignor = new OrderConsignor();
                splitOrder.OrderConsignor.OrderID = splitOrder.ID;
                splitOrder.OrderConsignor.Type = order.OrderConsignor.Type;
                if (order.OrderConsignor.Type == SZDeliveryType.PickUpInStore)
                {
                    splitOrder.OrderConsignor.Contact = order.OrderConsignor.Contact;
                    splitOrder.OrderConsignor.Mobile = order.OrderConsignor.Mobile;
                    splitOrder.OrderConsignor.IDType = order.OrderConsignor.IDType;
                    splitOrder.OrderConsignor.IDNumber = order.OrderConsignor.IDNumber;
                    splitOrder.OrderConsignor.Address = null;
                }
                else
                {
                    splitOrder.OrderConsignor.Name = order.OrderConsignor.Name;
                    splitOrder.OrderConsignor.Contact = order.OrderConsignor.Contact;
                    splitOrder.OrderConsignor.Mobile = order.OrderConsignor.Mobile;
                    splitOrder.OrderConsignor.Address = order.OrderConsignor.Address;
                    splitOrder.OrderConsignor.IDType = null;
                    splitOrder.OrderConsignor.IDNumber = null;
                }
                #endregion


                splitOrder.Split();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return "拆分失败：" + ex.Message;
            }
        }

        //private string getSplitOrderID(string orderID)
        //{
        //    string orderid = "";
        //    string MainOrderID = orderID;
        //    if (orderID.Contains("-"))
        //    {
        //        string[] ids = orderID.Split('-');
        //        MainOrderID = ids[0];
        //    }

        //    var singleOrder = new OrdersView().Where(item => item.MainOrderID == MainOrderID).OrderByDescending(item => item.CreateDate).FirstOrDefault();
        //    if (singleOrder.ID.Contains("-"))
        //    {
        //        string[] ids = singleOrder.ID.Split('-');
        //        string no = (Convert.ToInt16(ids[1]) + 1).ToString().PadLeft(3, '0');
        //        orderid = ids[0] +"-"+ no;
        //    }
        //    else
        //    {
        //        orderid = orderID + "-001";
        //    }


        //    return orderid;
        //}

    }
}