using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Layers.Data.Sqls;
using Layers.Linq;
using System.Linq.Expressions;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 待客户确认的报关订单
    /// </summary>
    public class UnConfirmedOrders : OrderAlls
    {
        IUser User;

        public UnConfirmedOrders(IUser user)
        {
            this.User = user;
        }

        public UnConfirmedOrders(IUser User, PvWsOrderReponsitory reponsitory, IQueryable<WsOrder> iquery) : base(reponsitory, iquery)
        {
            this.User = User;
        }


        protected override IQueryable<WsOrder> GetIQueryable()
        {
            var linq = base.GetIQueryable().Where(item => item.ClientID == this.User.EnterpriseID && item.MainStatus == CgOrderStatus.待确认);

            if (!User.IsMain)
            {
                linq = linq.Where(item => item.CreatorID == User.ID);
            }
            return linq;
        }


        #region 查询条件查询
        /// <summary>
        /// 根据供应商查询
        /// </summary>
        /// <param name="Supplier"></param>
        /// <returns></returns>
        public UnConfirmedOrders SearchBySupplier(string Supplier)
        {
            return new UnConfirmedOrders(this.User, this.Reponsitory, base.SearchBySupplierName(Supplier));
        }

        /// <summary>
        /// 根据型号查询
        /// </summary>
        /// <param name="PartNumber"></param>
        /// <returns></returns>
        public UnConfirmedOrders SearchByPart(string PartNumber)
        {
            return new UnConfirmedOrders(this.User, this.Reponsitory, base.SearchByPartNumber(PartNumber));
        }
        #endregion


        /// <summary>
        /// 报关订单列表数据查询
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public override PageList<OrderExtends> GetPageListOrders(LambdaExpression[] expressions, int PageSize, int PageIndex)
        {
            var Orders = GetOrders(expressions).OrderByDescending(item => item.CreateDate);
            var total = Orders.Count();
            var linq = Orders.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToArray();

            var declareprices = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderDeclareStatisticsView>().Where(item => linq.Select(a => a.ID).Contains(item.OrderID)).ToArray();

            var data = from order in linq
                       join declareprice in declareprices on order.ID equals declareprice.OrderID into _declareprices
                       select new OrderExtends
                       {
                           ID = order.ID,
                           Type = order.Type,
                           ClientID = order.ClientID,
                           SupplierID = order.SupplierID,
                           CreateDate = order.CreateDate,
                           MainStatus = order.MainStatus,
                           Output = new OrderOutput
                           {
                               ID = order.Output.ID,
                               Currency = order.Output.Currency,
                           },
                           TotalTraiff = _declareprices.Sum(item => item.CutomsPrice).GetValueOrDefault(),
                           TotalAddTax = _declareprices.Sum(item => item.DutyPrice).GetValueOrDefault(),
                           TotalAgencyFee = _declareprices.Sum(item => item.AgentPrice).GetValueOrDefault(),
                           TotalInspectionFee = _declareprices.Sum(item => item.otherPrice).GetValueOrDefault(),
                       };

            return new PageList<OrderExtends>(PageIndex, PageSize, data, total);
        }


        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DeclareOrder GetOrderDetail(string ID)
        {
            var order = this[ID]; //获取当前订单数据

            //获取对账单生成的实时汇率
            decimal RealRate = 0;
            using (ScCustomReponsitory res = new ScCustomReponsitory())
            {
                RealRate = res.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>().FirstOrDefault(item => item.MainOrderId == ID).RealExchangeRate.GetValueOrDefault();
            }

            //获取运单数据
            var outwaybill = new WayBillAlls(this.Reponsitory)[order.Output.WayBillID];

            OrderItemAlls orderItemAlls = new OrderItemAlls(this.Reponsitory);
            var orderitems = orderItemAlls.GetItemBillByOrderID(ID);
            var tinyOrderRates = orderItemAlls.GetTinyOrderRates(order.ID);
            for (int i = 0; i < orderitems.Length; i++)
            {
                var tinyOrderRate = tinyOrderRates.Where(t => t.TinyOrderID == orderitems[i].TinyOrderID).FirstOrDefault();
                decimal realRate = tinyOrderRate != null ? tinyOrderRate.RealRate : 0;
                decimal customsRate = tinyOrderRate != null ? tinyOrderRate.CustomsRate : 0;

                orderitems[i].RealExchangeRate = realRate;
                orderitems[i].CustomsExchangeRate = customsRate;
                orderitems[i].DeclareTotalPrice = orderitems[i].TotalPrice * realRate;
            }

            var orderFiles = new CenterFilesView().SearchByOrderID(order.ID).ToArray();

            var data = new DeclareOrder
            {
                ID = order.ID,
                Type = order.Type,
                ClientID = order.ClientID,
                InvoiceID = order.InvoiceID,
                PayeeID = order.PayeeID,
                BeneficiaryID = order.BeneficiaryID,
                MainStatus = order.MainStatus,
                PaymentStatus = order.PaymentStatus,
                InvoiceStatus = order.InvoiceStatus,
                SettlementCurrency = order.SettlementCurrency,
                Summary = order.Summary,
                CreateDate = order.CreateDate,
                ModifyDate = order.ModifyDate,
                CreatorID = order.CreatorID,
                SupplierID = order.SupplierID,
                OutWaybill = outwaybill,
                OrderItems = orderitems.ToArray(),
                Input = new OrderInput
                {
                    ID = order.Input.ID,
                    BeneficiaryID = order.Input.BeneficiaryID,
                    Conditions = order.Input.Conditions,
                    Currency = order.Input.Currency,
                    IsPayCharge = order.Input.IsPayCharge,
                    WayBillID = order.Input.WayBillID
                },
                Output = new OrderOutput
                {
                    ID = order.Output.ID,
                    BeneficiaryID = order.Output.BeneficiaryID,
                    IsReciveCharge = order.Output.IsReciveCharge,
                    Conditions = order.Output.Conditions,
                    WayBillID = order.Output.WayBillID,
                    Currency = order.Output.Currency
                },
                OrderFiles = orderFiles,
            };
            //转报关没有Inwaybill数据
            if (order.Type == OrderType.Declare)
            {
                data.InWaybill = new WayBillAlls(this.Reponsitory)[order.Input.WayBillID];
            }
            return data;
        }


        /// <summary>
        /// 客户确认
        /// </summary>
        /// <param name="ID"></param>
        public void ClientConfirm(string ID)
        {
            var order = this.GetDecNoticeDataByOrderID(ID);
            if (order == null)
            {
                throw new Exception("订单不存在！");
            }

            base.Confirm(order, this.User.ID);
        }
    }
}
