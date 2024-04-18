using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 应收账款单抬头列表视图
    /// </summary>
    public class MonoListView : QueryView<MonoListViewModel, ScCustomsReponsitory>
    {
        public MonoListView()
        {
        }

        protected MonoListView(ScCustomsReponsitory reponsitory, IQueryable<MonoListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<MonoListViewModel> GetIQueryable()
        {
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var decTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>();

            var iQuery = from decHead in decHeads
                         join decTax in decTaxs on decHead.ID equals decTax.ID
                         where decHead.IsSuccess == true
                            && decHead.CusDecStatus != "04"
                            && decTax.InvoiceType == (int)Enums.InvoiceType.Full
                         orderby decHead.DDate descending
                         select new MonoListViewModel
                         {
                             DecHeadID = decHead.ID,
                             OrderID = decHead.OrderID,
                             OwnerName = decHead.OwnerName,
                             DDate = decHead.DDate,
                             ContractNo = decHead.ContrNo,
                             ConsignorCode = decHead.ConsignorCode
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<MonoListViewModel> iquery = this.IQueryable.Cast<MonoListViewModel>().OrderByDescending(item => item.DDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myDecHead = iquery.ToArray();

            //DecHeadID
            var decHeadIDs = ienum_myDecHead.Select(item => item.DecHeadID);

            //OrderID
            var orderIDs = ienum_myDecHead.Select(item => item.OrderID);

            #region 币种、委托金额、海关汇率

            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var linq_order = from order in orders
                             where orderIDs.Contains(order.ID)
                                && order.Status == (int)Enums.Status.Normal
                             select new
                             {
                                 OrderID = order.ID,
                                 Currency = order.Currency,
                                 AttorneyAmount = order.DeclarePrice,
                                 CustomsExchangeRate = order.CustomsExchangeRate,
                             };

            var ienums_order = linq_order.ToArray();

            #endregion

            #region 报关金额

            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();

            var linq_declarationAmount = from decList in decLists
                                         where decHeadIDs.Contains(decList.DeclarationID)
                                         group decList by new { decList.DeclarationID, } into g
                                         select new
                                         {
                                             DecHeadID = g.Key.DeclarationID,
                                             DeclarationAmount = g.Sum(t => t.DeclTotal),
                                         };

            var ienums_declarationAmount = linq_declarationAmount.ToArray();

            #endregion

            #region 发票号、开票时间

            var invoiceNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>();
            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>();

            var linq_invoiceNo = from invoiceNoticeItem in invoiceNoticeItems
                                 join invoiceNotice in invoiceNotices on invoiceNoticeItem.InvoiceNoticeID equals invoiceNotice.ID
                                 where invoiceNoticeItem.Status == (int)Enums.Status.Normal
                                    && orderIDs.Contains(invoiceNoticeItem.OrderID)
                                 select new
                                 {
                                     OrderID = invoiceNoticeItem.OrderID,
                                     InvoiceNo = invoiceNoticeItem.InvoiceNo,
                                     InvoiceTime = invoiceNotice.UpdateDate,
                                 };

            var ienums_invoiceNo = linq_invoiceNo.Distinct().ToArray();

            #endregion

            var ienums_linq = from decHead in ienum_myDecHead
                              join order in ienums_order on decHead.OrderID equals order.OrderID
                              join declarationAmount in ienums_declarationAmount on decHead.DecHeadID equals declarationAmount.DecHeadID
                              let invoiceNo = ienums_invoiceNo.Where(t => t.OrderID == decHead.OrderID).FirstOrDefault()
                              select new MonoListViewModel
                              {
                                  DecHeadID = decHead.DecHeadID,
                                  OrderID = decHead.OrderID,
                                  OwnerName = decHead.OwnerName,
                                  DDate = decHead.DDate,

                                  Currency = order.Currency,
                                  AttorneyAmount = order.AttorneyAmount,
                                  CustomsExchangeRate = order.CustomsExchangeRate,

                                  DeclarationAmount = declarationAmount.DeclarationAmount,

                                  InvoiceNo = invoiceNo != null ? invoiceNo.InvoiceNo : null,
                                  InvoiceTime = invoiceNo != null ? (DateTime?)invoiceNo.InvoiceTime : null,

                                  ContractNo = decHead.ContractNo,
                                  ConsignorCode = decHead.ConsignorCode
                              };

            var results = ienums_linq.ToArray();

            #region 发票金额

            var linq_thisOrdersInvoiceNoticeItems = from invoiceNoticeItem in invoiceNoticeItems
                                                    where orderIDs.Contains(invoiceNoticeItem.OrderID)
                                                       && invoiceNoticeItem.Status == (int)Enums.Status.Normal
                                                    select new
                                                    {
                                                        InvoiceNoticeID = invoiceNoticeItem.InvoiceNoticeID,
                                                    };

            var ienums_thisOrdersInvoiceNoticeItems = linq_thisOrdersInvoiceNoticeItems.ToArray();

            var invoiceNoticeIDs = ienums_thisOrdersInvoiceNoticeItems.Select(t => t.InvoiceNoticeID).Distinct().ToArray();


            var linq_realNeedInvoiceNotice = from invoiceNoticeItem in invoiceNoticeItems
                                             where invoiceNoticeIDs.Contains(invoiceNoticeItem.InvoiceNoticeID)
                                                && invoiceNoticeItem.Status == (int)Enums.Status.Normal
                                             select new
                                             {
                                                 OrderID = invoiceNoticeItem.OrderID,
                                                 InvoiceNoticeID = invoiceNoticeItem.InvoiceNoticeID,
                                                 Amount = invoiceNoticeItem.Amount,
                                                 Difference = invoiceNoticeItem.Difference,
                                             };

            var ienums_realNeedInvoiceNotice = linq_realNeedInvoiceNotice.ToArray();


            for (int i = 0; i < results.Length; i++)
            {
                var thisOrderInvoiceNoticeIDs = ienums_realNeedInvoiceNotice.Where(t => t.OrderID == results[i].OrderID).Select(t => t.InvoiceNoticeID).ToArray();

                var thisOrderRelationInvoiceNoticeItems = ienums_realNeedInvoiceNotice.Where(t => thisOrderInvoiceNoticeIDs.Contains(t.InvoiceNoticeID)).ToList();

                results[i].InvoiceAmount = thisOrderRelationInvoiceNoticeItems.Sum(t => t.Amount + t.Difference);
            }

            #endregion

            #region 当天汇率

            var dDates = ienum_myDecHead.Select(t => t.DDate?.Date).Distinct().ToArray();

            var exchangeRates = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRates>();
            var exchangeRateLogs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRateLogs>();

            //查当前实时汇率表中的所有值
            var linq_exchangeRate = from exchangeRate in exchangeRates
                                    where exchangeRate.Type == (int)Enums.ExchangeRateType.RealTime
                                    select new
                                    {
                                        ExchangeRateID = exchangeRate.ID,
                                        Code = exchangeRate.Code,
                                        Rate = exchangeRate.Rate,
                                        UpdateDate = exchangeRate.UpdateDate,
                                    };

            var ienums_exchangeRate = linq_exchangeRate.ToArray();

            var exchangeRateIDs = ienums_exchangeRate.Select(t => t.ExchangeRateID);

            //查实时汇率日志表中的可能值
            var linq_exchangeRateLog = from exchangeRateLog in exchangeRateLogs
                                       join exchangeRate in exchangeRates on exchangeRateLog.ExchangeRateID equals exchangeRate.ID
                                       where exchangeRateIDs.Contains(exchangeRateLog.ExchangeRateID)
                                          && dDates.Contains(exchangeRateLog.CreateDate.Date)
                                       select new
                                       {
                                           Code = exchangeRate.Code,
                                           Rate = exchangeRateLog.Rate,
                                           CreateDate = exchangeRateLog.CreateDate,
                                       };

            var ienums_exchangeRateLog = linq_exchangeRateLog.ToArray();

            //对每一条数据，先用日志表的值，再用当前实时汇率表中的值
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].DDate != null)
                {
                    var theExchangeRateLog = ienums_exchangeRateLog
                        .Where(t => t.Code == results[i].Currency
                                 && t.CreateDate.ToString("yyyy-MM-dd") == results[i].DDate?.ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.CreateDate).FirstOrDefault();

                    if (theExchangeRateLog != null)
                    {
                        results[i].ThatDayExchangeRate = theExchangeRateLog.Rate;
                    }
                }
            }

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].DDate != null)
                {
                    var theExchangeRate = ienums_exchangeRate
                        .Where(t => t.Code == results[i].Currency
                                 && t.UpdateDate?.ToString("yyyy-MM-dd") == results[i].DDate?.ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.UpdateDate).FirstOrDefault();

                    if (theExchangeRate != null)
                    {
                        results[i].ThatDayExchangeRate = theExchangeRate.Rate;
                    }
                }
            }

            #endregion

            //#region 账单金额

            //List<BillModel> Bills = GetBills(orderIDs.ToArray());

            //for (int i = 0; i < results.Length; i++)
            //{
            //    var bill = Bills.Where(t => t.OrderID == results[i].OrderID).FirstOrDefault();

            //    if (bill != null)
            //    {
            //        results[i].BillAmount = bill.totalCNYPrice + (bill.totalTraiff ?? 0) + (bill.totalAddedValueTax ?? 0) + bill.totalAgencyFee + bill.totalIncidentalFee;
            //    }
            //}

            //#endregion

            Func<MonoListViewModel, object> convert = item => new
            {
                DecHeadID = item.DecHeadID,
                OrderID = item.OrderID,
                OwnerName = item.OwnerName,
                DDate = item.DDate?.ToString("yyyy-MM-dd HH:mm:ss"),

                Currency = item.Currency,
                AttorneyAmount = item.AttorneyAmount,
                CustomsExchangeRate = item.CustomsExchangeRate,

                DeclarationAmount = item.DeclarationAmount,

                InvoiceAmount = item.InvoiceNo != null ? Convert.ToString(item.InvoiceAmount) : "-",

                InvoiceNo = item.InvoiceNo != null ? item.InvoiceNo : "-",
                InvoiceTime = item.InvoiceNo != null ? item.InvoiceTime?.ToString("yyyy-MM-dd HH:mm:ss") : "-",

                ThatDayExchangeRate = item.ThatDayExchangeRate,
                UnInvoiceAmount = item.InvoiceNo != null ? 0 : item.InvoiceAmount,

                BillAmount = "",  //item.BillAmount.ToRound(2),

                ContractNo = item.ContractNo,
                ConsignorCode = item.ConsignorCode
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {
                    item.OwnerName,
                    item.OrderID,
                    item.Currency,
                    item.DDate,
                    item.DeclarationAmount,
                    item.AttorneyAmount,
                    item.BillAmount,
                    item.InvoiceAmount,
                    item.UnInvoiceAmount,
                    item.InvoiceNo,
                    item.InvoiceTime,
                    item.ThatDayExchangeRate,
                    item.CustomsExchangeRate,
                    item.ContractNo,
                    item.ConsignorCode
                };

                return results.Select(convert).Select(convertAgain).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }


        private List<BillModel> GetBills(string[] orderIDs)
        {
            List<BillModel> Bills = new List<BillModel>();

            Models.OrderBill[] arry_bill;
            MyOrderItemHelp[] arry_item;
            Models.GroupPremiumsHelp[] group_premiums;
            MyContractHelp[] arry_contract;

            var orderBillView = new Needs.Ccs.Services.Views.OrdersBillsView(this.Reponsitory);
            //var orderItemView = new OrderItemsView(this.Reponsitory);

            var linq_bills = from ob in orderBillView
                             where orderIDs.Contains(ob.ID)
                             select ob;
            arry_bill = linq_bills.ToArray();

            //var linq_item = from item in orderItemView
            //                where orderIDs.Contains(item.OrderID)
            //                select new MyOrderItemHelp
            //                {
            //                    ProductName = item.Category.Name,
            //                    OrderID = item.OrderID,
            //                    TotalPrice = item.TotalPrice,
            //                    Model = item.Model,
            //                    Quantity = item.Quantity,
            //                    UnitPrice = item.UnitPrice,
            //                    TariffRate = item.ImportTax.Rate,
            //                    AddedValueTax = item.AddedValueTax.Value,
            //                    InspectionFee = item.InspectionFee,
            //                    Traiff = item.ImportTax.Value,
            //                };

            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderItemCategories = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();
            var orderItemTaxes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();
            var premiumsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>();

            var linq_item = from orderItem in orderItems
                            join orderItemCategory in orderItemCategories on orderItem.ID equals orderItemCategory.OrderItemID into orderItemCategories2
                            from orderItemCategory in orderItemCategories2.DefaultIfEmpty()
                            join importTax in orderItemTaxes.Where(t => t.Type == (int)Enums.CustomsRateType.ImportTax) on orderItem.ID equals importTax.OrderItemID
                            into importTaxes
                            from importTax in importTaxes.DefaultIfEmpty()

                            join addedValueTax in orderItemTaxes.Where(t => t.Type == (int)Enums.CustomsRateType.AddedValueTax) on orderItem.ID equals addedValueTax.OrderItemID
                            into addedValueTaxes
                            from addedValueTax in addedValueTaxes.DefaultIfEmpty()

                            join inspFee in premiumsView.Where(f => f.Type == (int)Enums.OrderPremiumType.InspectionFee) on orderItem.ID equals inspFee.OrderItemID into inspFees
                            from inspFee in inspFees.DefaultIfEmpty()

                            where orderIDs.Contains(orderItem.OrderID)
                            select new MyOrderItemHelp
                            {
                                ProductName = orderItemCategory.Name,
                                OrderID = orderItem.OrderID,
                                TotalPrice = orderItem.TotalPrice,
                                Model = orderItem.Model,
                                Quantity = orderItem.Quantity,
                                UnitPrice = orderItem.UnitPrice,
                                TariffRate = importTax.Rate,
                                AddedValueTax = addedValueTax.Value,
                                InspectionFee = inspFee == null ? null : (decimal?)inspFee.UnitPrice * inspFee.Count * inspFee.Rate,
                                Traiff = importTax.Value,
                            };

            arry_item = linq_item.ToArray();


            var linq_premiums = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                                where orderIDs.Contains(item.OrderID) && item.Status == (int)Enums.Status.Normal
                                select new
                                {
                                    OrderID = item.OrderID,
                                    Type = (Enums.OrderPremiumType)item.Type,
                                    Count = item.Count,
                                    UnitPrice = item.UnitPrice,
                                    //Currency = item.Currency,
                                    Rate = item.Rate,
                                };
            group_premiums = (from item in linq_premiums.ToArray()
                              group item by new { item.OrderID, item.Type } into groups
                              select new Models.GroupPremiumsHelp
                              {
                                  OrderID = groups.Key.OrderID,
                                  Type = groups.Key.Type,
                                  TotalPrice = groups.Sum(item => item.Count * item.UnitPrice * item.Rate)
                              }).ToArray();

            var linq_dechead = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                               where orderIDs.Contains(item.OrderID)
                               select new MyContractHelp
                               {
                                   OrderID = item.OrderID,
                                   ContractNO = item.ContrNo
                               };
            arry_contract = linq_dechead.ToArray();


            foreach (var orderid in orderIDs)
            {
                var bill = arry_bill.FirstOrDefault(item => item.ID == orderid);



                //税点
                var taxpoint = 1 + bill.Agreement.InvoiceTaxRate;
                //代理费率、最低代理费
                decimal agencyRate = bill.AgencyFeeExchangeRate * bill.Agreement.AgencyRate;
                bool isAverage = false;
                decimal minAgencyFee = bill.Agreement.MinAgencyFee;
                switch (bill.Order.OrderBillType)
                {
                    case Enums.OrderBillType.Normal:
                        {
                            //isAverage = bill.DeclarePrice * agencyRate < minAgencyFee ? true : false;
                            var declarePrice = arry_item.Where(item => item.OrderID == bill.Order.ID).Sum(item => item.TotalPrice);
                            isAverage = declarePrice * agencyRate < minAgencyFee ? true : false;
                        }
                        break;

                    case Enums.OrderBillType.MinAgencyFee:
                        isAverage = false;
                        break;

                    case Enums.OrderBillType.Pointed:
                        isAverage = true;
                        break;
                }

                //平摊代理费、其他杂费
                //decimal AgencyFee = bill.AgencyFee * taxpoint;                       
                decimal AgencyFee = (group_premiums.SingleOrDefault(item => item.OrderID == orderid
                    && item.Type == Enums.OrderPremiumType.AgencyFee)?.TotalPrice ?? 0m) * taxpoint;

                //decimal aveAgencyFee = AgencyFee / bill.Items.Count();
                int orderItemCount = arry_item.Where(item => item.OrderID == orderid).ToList().Count();
                decimal aveAgencyFee = AgencyFee / orderItemCount;

                //decimal aveOtherFee = bill.OtherFee * taxpoint / bill.Items.Count();
                //decimal aveOtherFee = (group_premiums.SingleOrDefault(item => item.OrderID == orderid
                //    && item.Type != OrderPremiumType.AgencyFee
                //    && item.Type != OrderPremiumType.InspectionFee)?.TotalPrice ?? 0m) * taxpoint / bill.Items.Count();

                var otherFee = group_premiums.Where(item => item.OrderID == orderid
                    && item.Type != Enums.OrderPremiumType.AgencyFee
                    && item.Type != Enums.OrderPremiumType.InspectionFee);

                decimal aveOtherFee = 0m;

                if (otherFee != null)
                {
                    aveOtherFee = otherFee.Sum(item => item.TotalPrice) * taxpoint / bill.Items.Count();
                }

                //decimal aveOtherFee = group_premiums.Where(item => item.OrderID == orderid
                //    && item.Type != OrderPremiumType.AgencyFee
                //    && item.Type != OrderPremiumType.InspectionFee).Sum(item => item.TotalPrice) * taxpoint / bill.Items.Count();

                BillModel Item = new BillModel();

                Item.Products = new List<Models.MainOrderBillItemProduct>();
                Item.OrderID = bill.ID;
                Item.ContrNo = arry_contract.SingleOrDefault(item => item.OrderID == orderid)?.ContractNO;
                Item.RealExchangeRate = bill.RealExchangeRate;
                Item.CustomsExchangeRate = bill.CustomsExchangeRate;
                Item.OrderType = bill.OrderType;
                Item.AgencyFee = AgencyFee;

                Item.Products = arry_item.Where(item => item.OrderID == bill.Order.ID).Select(item => new
                Models.MainOrderBillItemProduct
                {
                    ProductName = item.ProductName.Trim(),
                    Model = item.Model.Trim(),
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    TariffRate = item.TariffRate,
                    TotalCNYPrice = (item.TotalPrice * bill.ProductFeeExchangeRate),
                    Traiff = item.Traiff,
                    AddedValueTax = item.AddedValueTax.Value,
                    AgencyFee = isAverage ? aveAgencyFee : (item.TotalPrice * agencyRate * taxpoint),
                    IncidentalFee = item.InspectionFee == null ? aveOtherFee : (item.InspectionFee.Value * taxpoint + aveOtherFee)
                }).ToList();

                Item.PartProducts = new List<Models.MainOrderBillItemProduct>();

                //不明白？
                Item.PartProducts.Add(Item.Products.FirstOrDefault());

                Item.totalQty = Item.Products.Sum(t => t.Quantity);
                Item.totalPrice = Item.Products.Sum(t => t.TotalPrice);
                Item.totalCNYPrice = Item.Products.Sum(t => t.TotalCNYPrice);
                Item.totalTraiff = Item.Products.Sum(t => t.Traiff);
                Item.totalAddedValueTax = Item.Products.Sum(t => t.AddedValueTax);
                Item.totalAgencyFee = Item.Products.Sum(t => t.AgencyFee);
                Item.totalIncidentalFee = Item.Products.Sum(t => t.IncidentalFee);

                //ryan 20210113 外单税费小于50不收 钟苑平
                //if (bill.OrderType != Enums.OrderType.Outside && Item.totalTraiff < 50)
                if (Item.totalTraiff < 50)
                {
                    Item.totalTraiff = 0;
                }
                if (Item.totalAddedValueTax < 50)
                {
                    Item.totalAddedValueTax = 0;
                }

                Bills.Add(Item);
            }

            return Bills;
        }


        /// <summary>
        /// 根据客户名称查询
        /// </summary>
        /// <param name="ownerName"></param>
        /// <returns></returns>
        public MonoListView SearchByOwnerName(string ownerName)
        {
            var linq = from query in this.IQueryable
                       where query.OwnerName.Contains(ownerName)
                       select query;

            var view = new MonoListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据订单编号查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public MonoListView SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new MonoListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据报关日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public MonoListView SearchByDDateBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.DDate >= begin
                       select query;

            var view = new MonoListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据报关日期结束时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public MonoListView SearchByDDateEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.DDate < end
                       select query;

            var view = new MonoListView(this.Reponsitory, linq);
            return view;
        }

        ///// <summary>
        ///// 根据发票号查询
        ///// </summary>
        ///// <param name="invoiceNo"></param>
        ///// <returns></returns>
        //public DualServiceListView SearchByInvoiceNo(string invoiceNo)
        //{

        //}

    }

    public class MonoListViewModel
    {
        /// <summary>
        /// DecHeadID
        /// </summary>
        public string DecHeadID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 报关金额
        /// </summary>
        public decimal DeclarationAmount { get; set; }

        /// <summary>
        /// 委托金额
        /// </summary>
        public decimal AttorneyAmount { get; set; }

        /// <summary>
        /// 账单金额
        /// </summary>
        public decimal BillAmount { get; set; }

        /// <summary>
        /// 发票金额
        /// </summary>
        public decimal? InvoiceAmount { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 开票时间
        /// </summary>
        public DateTime? InvoiceTime { get; set; }

        /// <summary>
        /// 当天汇率
        /// </summary>
        public decimal? ThatDayExchangeRate { get; set; }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal? CustomsExchangeRate { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNo { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string ConsignorCode { get; set; }
    }


    public class BillModel
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        public string ContrNo { get; set; }
        public decimal CustomsExchangeRate { get; set; }
        public decimal RealExchangeRate { get; set; }
        public Enums.OrderType OrderType { get; set; }
        public decimal AgencyFee { get; set; }
        public List<Models.MainOrderBillItemProduct> Products { get; set; }

        public List<Models.MainOrderBillItemProduct> PartProducts { get; set; }

        //合计 客户端页面展示用
        public decimal totalQty { get; set; }
        public decimal totalPrice { get; set; }
        public decimal totalCNYPrice { get; set; }
        public decimal? totalTraiff { get; set; }
        public decimal? totalAddedValueTax { get; set; }
        public decimal totalAgencyFee { get; set; }
        public decimal totalIncidentalFee { get; set; }
    }


    /// <summary>
    /// 我的订单项目帮助类
    /// </summary>
    class MyOrderItemHelp
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 总值
        /// </summary>
        public decimal TotalPrice { get; set; }


        #region 补充与原有对象 ：MainOrderBillItemProduct

        public string ProductName { get; set; }
        public string Model { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TariffRate { get; set; }
        public decimal TotalCNYPrice { get; set; }
        public decimal? Traiff { get; set; }
        public decimal? AddedValueTax { get; set; }
        public decimal AgencyFee { get; set; }
        public decimal IncidentalFee { get; set; }
        public decimal MyProperty { get; set; }
        public decimal? InspectionFee { get; set; }


        #endregion

    }


    /// <summary>
    /// 我的订单合同帮助类
    /// </summary>
    class MyContractHelp
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNO { get; set; }
    }



    //////////////////////////////////////////////////////////////////////////////////////////


    public class BillAmountsView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public BillAmountsView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public BillAmountsView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public List<BillModel> GetBillAmounts(string[] orderIDs)
        {
            List<BillModel> Bills = new List<BillModel>();

            Models.OrderBill[] arry_bill;
            MyOrderItemHelp[] arry_item;
            Models.GroupPremiumsHelp[] group_premiums;
            MyContractHelp[] arry_contract;

            var orderBillView = new Needs.Ccs.Services.Views.OrdersBillsView(this.Reponsitory);
            //var orderItemView = new OrderItemsView(this.Reponsitory);

            var linq_bills = from ob in orderBillView
                             where orderIDs.Contains(ob.ID)
                             select ob;
            arry_bill = linq_bills.ToArray();

            //var linq_item = from item in orderItemView
            //                where orderIDs.Contains(item.OrderID)
            //                select new MyOrderItemHelp
            //                {
            //                    ProductName = item.Category.Name,
            //                    OrderID = item.OrderID,
            //                    TotalPrice = item.TotalPrice,
            //                    Model = item.Model,
            //                    Quantity = item.Quantity,
            //                    UnitPrice = item.UnitPrice,
            //                    TariffRate = item.ImportTax.Rate,
            //                    AddedValueTax = item.AddedValueTax.Value,
            //                    InspectionFee = item.InspectionFee,
            //                    Traiff = item.ImportTax.Value,
            //                };

            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderItemCategories = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();
            var orderItemTaxes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();
            var premiumsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>();

            var linq_item = from orderItem in orderItems
                            join orderItemCategory in orderItemCategories on orderItem.ID equals orderItemCategory.OrderItemID into orderItemCategories2
                            from orderItemCategory in orderItemCategories2.DefaultIfEmpty()
                            join importTax in orderItemTaxes.Where(t => t.Type == (int)Enums.CustomsRateType.ImportTax) on orderItem.ID equals importTax.OrderItemID
                            into importTaxes
                            from importTax in importTaxes.DefaultIfEmpty()

                            join addedValueTax in orderItemTaxes.Where(t => t.Type == (int)Enums.CustomsRateType.AddedValueTax) on orderItem.ID equals addedValueTax.OrderItemID
                            into addedValueTaxes
                            from addedValueTax in addedValueTaxes.DefaultIfEmpty()

                            join inspFee in premiumsView.Where(f => f.Type == (int)Enums.OrderPremiumType.InspectionFee) on orderItem.ID equals inspFee.OrderItemID into inspFees
                            from inspFee in inspFees.DefaultIfEmpty()

                            where orderIDs.Contains(orderItem.OrderID)
                            select new MyOrderItemHelp
                            {
                                ProductName = orderItemCategory.Name,
                                OrderID = orderItem.OrderID,
                                TotalPrice = orderItem.TotalPrice,
                                Model = orderItem.Model,
                                Quantity = orderItem.Quantity,
                                UnitPrice = orderItem.UnitPrice,
                                TariffRate = importTax.Rate,
                                AddedValueTax = addedValueTax.Value,
                                InspectionFee = inspFee == null ? null : (decimal?)inspFee.UnitPrice * inspFee.Count * inspFee.Rate,
                                Traiff = importTax.Value,
                            };

            arry_item = linq_item.ToArray();


            var linq_premiums = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                                where orderIDs.Contains(item.OrderID) && item.Status == (int)Enums.Status.Normal
                                select new
                                {
                                    OrderID = item.OrderID,
                                    Type = (Enums.OrderPremiumType)item.Type,
                                    Count = item.Count,
                                    UnitPrice = item.UnitPrice,
                                    //Currency = item.Currency,
                                    Rate = item.Rate,
                                };
            group_premiums = (from item in linq_premiums.ToArray()
                              group item by new { item.OrderID, item.Type } into groups
                              select new Models.GroupPremiumsHelp
                              {
                                  OrderID = groups.Key.OrderID,
                                  Type = groups.Key.Type,
                                  TotalPrice = groups.Sum(item => item.Count * item.UnitPrice * item.Rate)
                              }).ToArray();

            var linq_dechead = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                               where orderIDs.Contains(item.OrderID)
                               select new MyContractHelp
                               {
                                   OrderID = item.OrderID,
                                   ContractNO = item.ContrNo
                               };
            arry_contract = linq_dechead.ToArray();


            foreach (var orderid in orderIDs)
            {
                var bill = arry_bill.FirstOrDefault(item => item.ID == orderid);



                //税点
                var taxpoint = 1 + bill.Agreement.InvoiceTaxRate;
                //代理费率、最低代理费
                decimal agencyRate = bill.AgencyFeeExchangeRate * bill.Agreement.AgencyRate;
                bool isAverage = false;
                decimal minAgencyFee = bill.Agreement.MinAgencyFee;
                switch (bill.Order.OrderBillType)
                {
                    case Enums.OrderBillType.Normal:
                        {
                            //isAverage = bill.DeclarePrice * agencyRate < minAgencyFee ? true : false;
                            var declarePrice = arry_item.Where(item => item.OrderID == bill.Order.ID).Sum(item => item.TotalPrice);
                            isAverage = declarePrice * agencyRate < minAgencyFee ? true : false;
                        }
                        break;

                    case Enums.OrderBillType.MinAgencyFee:
                        isAverage = false;
                        break;

                    case Enums.OrderBillType.Pointed:
                        isAverage = true;
                        break;
                }

                //平摊代理费、其他杂费
                //decimal AgencyFee = bill.AgencyFee * taxpoint;                       
                decimal AgencyFee = (group_premiums.SingleOrDefault(item => item.OrderID == orderid
                    && item.Type == Enums.OrderPremiumType.AgencyFee)?.TotalPrice ?? 0m) * taxpoint;

                //decimal aveAgencyFee = AgencyFee / bill.Items.Count();
                int orderItemCount = arry_item.Where(item => item.OrderID == orderid).ToList().Count();
                decimal aveAgencyFee = AgencyFee / orderItemCount;

                //decimal aveOtherFee = bill.OtherFee * taxpoint / bill.Items.Count();
                //decimal aveOtherFee = (group_premiums.SingleOrDefault(item => item.OrderID == orderid
                //    && item.Type != OrderPremiumType.AgencyFee
                //    && item.Type != OrderPremiumType.InspectionFee)?.TotalPrice ?? 0m) * taxpoint / bill.Items.Count();

                var otherFee = group_premiums.Where(item => item.OrderID == orderid
                    && item.Type != Enums.OrderPremiumType.AgencyFee
                    && item.Type != Enums.OrderPremiumType.InspectionFee);

                decimal aveOtherFee = 0m;

                if (otherFee != null)
                {
                    aveOtherFee = otherFee.Sum(item => item.TotalPrice) * taxpoint / bill.Items.Count();
                }

                //decimal aveOtherFee = group_premiums.Where(item => item.OrderID == orderid
                //    && item.Type != OrderPremiumType.AgencyFee
                //    && item.Type != OrderPremiumType.InspectionFee).Sum(item => item.TotalPrice) * taxpoint / bill.Items.Count();

                BillModel Item = new BillModel();

                Item.Products = new List<Models.MainOrderBillItemProduct>();
                Item.OrderID = bill.ID;
                Item.ContrNo = arry_contract.SingleOrDefault(item => item.OrderID == orderid)?.ContractNO;
                Item.RealExchangeRate = bill.RealExchangeRate;
                Item.CustomsExchangeRate = bill.CustomsExchangeRate;
                Item.OrderType = bill.OrderType;
                Item.AgencyFee = AgencyFee;

                Item.Products = arry_item.Where(item => item.OrderID == bill.Order.ID).Select(item => new
                Models.MainOrderBillItemProduct
                {
                    ProductName = item.ProductName.Trim(),
                    Model = item.Model.Trim(),
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    TariffRate = item.TariffRate,
                    TotalCNYPrice = (item.TotalPrice * bill.ProductFeeExchangeRate),
                    Traiff = item.Traiff,
                    AddedValueTax = item.AddedValueTax.Value,
                    AgencyFee = isAverage ? aveAgencyFee : (item.TotalPrice * agencyRate * taxpoint),
                    IncidentalFee = item.InspectionFee == null ? aveOtherFee : (item.InspectionFee.Value * taxpoint + aveOtherFee)
                }).ToList();

                Item.PartProducts = new List<Models.MainOrderBillItemProduct>();

                //不明白？
                Item.PartProducts.Add(Item.Products.FirstOrDefault());

                Item.totalQty = Item.Products.Sum(t => t.Quantity);
                Item.totalPrice = Item.Products.Sum(t => t.TotalPrice);
                Item.totalCNYPrice = Item.Products.Sum(t => t.TotalCNYPrice);
                Item.totalTraiff = Item.Products.Sum(t => t.Traiff);
                Item.totalAddedValueTax = Item.Products.Sum(t => t.AddedValueTax);
                Item.totalAgencyFee = Item.Products.Sum(t => t.AgencyFee);
                Item.totalIncidentalFee = Item.Products.Sum(t => t.IncidentalFee);

                //ryan 20210113 外单税费小于50不收 钟苑平
                //if (bill.OrderType != Enums.OrderType.Outside && Item.totalTraiff < 50)
                if (Item.totalTraiff < 50)
                {
                    Item.totalTraiff = 0;
                }
                if (Item.totalAddedValueTax < 50)
                {
                    Item.totalAddedValueTax = 0;
                }

                Bills.Add(Item);
            }

            return Bills;
        }

    }


}
