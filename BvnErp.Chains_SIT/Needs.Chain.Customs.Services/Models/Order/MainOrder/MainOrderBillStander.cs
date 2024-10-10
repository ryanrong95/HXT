using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MainOrderBillStander
    {
        /// <summary>
        /// 恒远或者华芯通相关固定信息
        /// </summary>
        public Purchaser PurchaserContext { get; set; }

        /// <summary>
        /// 订单对象，只是查询了Order表中的相关信息
        /// </summary>
        public List<Order> Orders { get; set; }
        /// <summary>
        /// 一个对账单，用来读取相关显示信息
        /// </summary>
        public OrderBill OrderBill { get; set; }
        /// <summary>
        /// 是否展示对账单
        /// </summary>
        public bool IsShowBill { get; set; }

        #region 税费
        public decimal BillTotalQty { get; set; }
        public decimal BillTotalPrice { get; set; }
        public decimal BillTotalCNYPrice { get; set; }
        public decimal? BillTotalTariff { get; set; }
        public decimal? BillTotalExciseTax { get; set; }
        public decimal? BillTotalAddedValueTax { get; set; }
        public decimal BillTotalAgencyFee { get; set; }
        public decimal BillTotalIncidentalFee { get; set; }
        /// <summary>
        /// 税费合计
        /// </summary>
        public decimal? BillTotalTaxAndFee { get; set; }
        /// <summary>
        /// 报关总金额
        /// </summary>
        public decimal? BillTotalDeclarePrice { get; set; }
        #endregion

        #region 文件
        public string FileID { get; set; }
        public string FileStatus { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }

        #endregion
        public MainOrderBillStander(Purchaser purchaserContext, List<Order> orders)
        {
            this.PurchaserContext = purchaserContext;
            this.Orders = orders;
        }

        #region MyClass Help

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
            public decimal? ExciseTax { get; set; }
            public decimal? AddedValueTax { get; set; }
            public decimal AgencyFee { get; set; }
            public decimal IncidentalFee { get; set; }
            public decimal MyProperty { get; set; }
            public decimal? InspectionFee { get; set; }


            #endregion

        }

        /// <summary>
        /// 我的订单项附加费用帮助类
        /// </summary>
        class GroupPremiumsHelp
        {
            /// <summary>
            /// 订单ID
            /// </summary>
            public string OrderID { get; set; }

            /// <summary>
            /// 附加费用类型
            /// </summary>
            public OrderPremiumType Type { get; set; }

            /// <summary>
            /// 总值
            /// </summary>
            public decimal TotalPrice { get; set; }
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

            /// <summary>
            /// 是否报关
            /// </summary>
            public bool IsSuccess { get; set; }

            /// <summary>
            /// 报关日期
            /// </summary>
            public DateTime DDate { get; set; }


        }

        #endregion


        public List<MainOrderBillItem> Bills
        {
            get
            {
                List<MainOrderBillItem> Bills = new List<MainOrderBillItem>();
                //判断是第几次循环，赋值用
                int icount = 0;
                var ordersID = this.Orders.Select(t => t.ID);

                OrderBill[] arry_bill;
                MyOrderItemHelp[] arry_item;
                GroupPremiumsHelp[] group_premiums;
                MyContractHelp[] arry_contract;

                using (var orderBillView = new Needs.Ccs.Services.Views.OrdersBillsView())
                using (var orderItemView = new OrderItemsView())
                using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var linq_bills = from ob in orderBillView
                                     where ordersID.Contains(ob.ID)
                                     select ob;
                    arry_bill = linq_bills.ToArray();

                    var linq_item = from item in orderItemView
                                    where ordersID.Contains(item.OrderID)
                                    select new MyOrderItemHelp
                                    {
                                        ProductName = item.Category.Name,
                                        OrderID = item.OrderID,
                                        TotalPrice = item.TotalPrice,
                                        Model = item.Model,
                                        Quantity = item.Quantity,
                                        UnitPrice = item.UnitPrice,
                                        TariffRate = item.ImportTax.Rate,
                                        ExciseTax = item.ExciseTax == null ? 0M : item.ExciseTax.Value,
                                        AddedValueTax = item.AddedValueTax.Value,
                                        InspectionFee = item.InspectionFee,
                                        Traiff = item.ImportTax.Value,
                                    };
                    arry_item = linq_item.ToArray();


                    var linq_premiums = from item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                                        where ordersID.Contains(item.OrderID) && item.Status == (int)Enums.Status.Normal
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
                                      select new GroupPremiumsHelp
                                      {
                                          OrderID = groups.Key.OrderID,
                                          Type = groups.Key.Type,
                                          TotalPrice = groups.Sum(item => item.Count * item.UnitPrice * item.Rate)
                                      }).ToArray();

                    var linq_dechead = from item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                                       where ordersID.Contains(item.OrderID) && item.CusDecStatus != "04"
                                       select new MyContractHelp
                                       {
                                           OrderID = item.OrderID,
                                           ContractNO = item.ContrNo,
                                           IsSuccess = item.IsSuccess,
                                           DDate = (DateTime)item.DDate
                                       };
                    arry_contract = linq_dechead.ToArray();
                }



                foreach (var orderid in ordersID)
                {
                    var bill = arry_bill.FirstOrDefault(item => item.ID == orderid);
                    if (icount == 0)
                    {
                        this.OrderBill = bill;
                        #region 

                        if (arry_contract != null && arry_contract.Length > 0)
                        {
                            var arry_contract_new = arry_contract.Where(item => item.IsSuccess == true).OrderBy(item => item.DDate).ToArray();
                            if (arry_contract_new != null && arry_contract_new.Length > 0)
                            {
                                // bill = arry_bill.FirstOrDefault(item => item.ID == arry_contract_new[0].OrderID);
                                this.OrderBill.IsSuccess = true;
                                this.OrderBill.DDate = arry_contract_new[0].DDate;
                                //bill.IsSuccess = true;
                                //bill.DDate = arry_contract_new[0].DDate;
                            }
                            else
                            {
                                //bill = arry_bill.FirstOrDefault(item => item.ID == orderid);
                                // bill.IsSuccess = false;
                                //bill.DDate = bill.CreateDate;
                                this.OrderBill.IsSuccess = false;
                                this.OrderBill.DDate = bill.CreateDate;
                            }
                        }
                        else
                        {
                            //bill = arry_bill.FirstOrDefault(item => item.ID == orderid);
                            //bill.IsSuccess = false;
                            //bill.DDate = bill.CreateDate;
                            this.OrderBill.IsSuccess = false;
                            this.OrderBill.DDate = bill.CreateDate;
                        }
                        #endregion
                    }
                    icount++;

                    if (bill.CustomsExchangeRate == 0 || bill.RealExchangeRate == 0)
                    {
                        this.IsShowBill = false;
                    }
                    else
                    {
                        //税点
                        var taxpoint = 1 + bill.Agreement.InvoiceTaxRate;
                        //代理费率、最低代理费
                        decimal agencyRate = bill.AgencyFeeExchangeRate * bill.Agreement.AgencyRate;
                        bool isAverage = false;
                        decimal minAgencyFee = bill.Agreement.MinAgencyFee;
                        switch (bill.Order.OrderBillType)
                        {
                            case OrderBillType.Normal:
                                {
                                    //isAverage = bill.DeclarePrice * agencyRate < minAgencyFee ? true : false;
                                    var declarePrice = arry_item.Where(item => item.OrderID == bill.Order.ID).Sum(item => item.TotalPrice);
                                    isAverage = declarePrice * agencyRate < minAgencyFee ? true : false;
                                }
                                break;

                            case OrderBillType.MinAgencyFee:
                                isAverage = false;
                                break;

                            case OrderBillType.Pointed:
                                isAverage = true;
                                break;
                        }

                        //平摊代理费、其他杂费
                        //decimal AgencyFee = bill.AgencyFee * taxpoint;                       
                        decimal AgencyFee = (group_premiums.SingleOrDefault(item => item.OrderID == orderid
                            && item.Type == OrderPremiumType.AgencyFee)?.TotalPrice ?? 0m) * taxpoint;

                        //decimal aveAgencyFee = AgencyFee / bill.Items.Count();
                        int orderItemCount = arry_item.Where(item => item.OrderID == orderid).ToList().Count();
                        decimal aveAgencyFee = AgencyFee / orderItemCount;

                        //decimal aveOtherFee = bill.OtherFee * taxpoint / bill.Items.Count();
                        //decimal aveOtherFee = (group_premiums.SingleOrDefault(item => item.OrderID == orderid
                        //    && item.Type != OrderPremiumType.AgencyFee
                        //    && item.Type != OrderPremiumType.InspectionFee)?.TotalPrice ?? 0m) * taxpoint / bill.Items.Count();

                        var otherFee = group_premiums.Where(item => item.OrderID == orderid
                            && item.Type != OrderPremiumType.AgencyFee
                            && item.Type != OrderPremiumType.InspectionFee);

                        decimal aveOtherFee = 0m;
                        decimal allOtherFee = 0m;

                        if (otherFee != null)
                        {
                            allOtherFee = otherFee.Sum(item => item.TotalPrice) * taxpoint;
                            aveOtherFee = allOtherFee / bill.Items.Count();
                        }

                        //decimal aveOtherFee = group_premiums.Where(item => item.OrderID == orderid
                        //    && item.Type != OrderPremiumType.AgencyFee
                        //    && item.Type != OrderPremiumType.InspectionFee).Sum(item => item.TotalPrice) * taxpoint / bill.Items.Count();

                        MainOrderBillItem Item = new MainOrderBillItem();

                        Item.Products = new List<MainOrderBillItemProduct>();
                        Item.OrderID = bill.ID;
                        Item.ContrNo = arry_contract.SingleOrDefault(item => item.OrderID == orderid)?.ContractNO;
                        Item.RealExchangeRate = bill.RealExchangeRate;
                        Item.CustomsExchangeRate = bill.CustomsExchangeRate;
                        Item.OrderType = bill.OrderType;
                        Item.AgencyFee = AgencyFee;

                        Item.Products = arry_item.Where(item => item.OrderID == bill.Order.ID).Select(item => new
                        MainOrderBillItemProduct
                        {
                            ProductName = item.ProductName.Trim(),
                            Model = item.Model.Trim(),
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            TotalPrice = item.TotalPrice,
                            TariffRate = item.TariffRate,
                            TotalCNYPrice = (item.TotalPrice * bill.ProductFeeExchangeRate),
                            Traiff = item.Traiff,
                            ExciseTax = item.ExciseTax,
                            AddedValueTax = item.AddedValueTax.Value,
                            AgencyFee = isAverage ? aveAgencyFee : (item.TotalPrice * agencyRate * taxpoint),
                            IncidentalFee = item.InspectionFee == null ? aveOtherFee : (item.InspectionFee.Value * taxpoint + aveOtherFee),
                            InspectionFee = item.InspectionFee,
                        }).ToList();

                        //修正 代理费、杂费
                        if (orderItemCount > 1)
                        {
                            decimal 前N_1代理费 = 0;
                            decimal 前N_1其它杂费 = 0;
                            for (int i = 0; i < Item.Products.Count; i++)
                            {
                                decimal inspectionFee = (Item.Products[i].InspectionFee == null) ? 0 : Item.Products[i].InspectionFee.Value * taxpoint;

                                if (i < Item.Products.Count - 1) //前N-1个型号
                                {
                                    Item.Products[i].AgencyFee = Item.Products[i].AgencyFee.ToRound(2);
                                    前N_1代理费 += Item.Products[i].AgencyFee;

                                    Item.Products[i].IncidentalFee = inspectionFee + aveOtherFee.ToRound(2);
                                    前N_1其它杂费 += aveOtherFee.ToRound(2);
                                }
                                else //最后一个型号
                                {
                                    Item.Products[i].AgencyFee = AgencyFee - 前N_1代理费;
                                    Item.Products[i].IncidentalFee = inspectionFee + (allOtherFee - 前N_1其它杂费);
                                }
                            }
                        }

                        Item.PartProducts = new List<MainOrderBillItemProduct>();

                        //不明白？
                        Item.PartProducts.Add(Item.Products.FirstOrDefault());

                        Item.totalQty = Item.Products.Sum(t => t.Quantity);
                        Item.totalPrice = Item.Products.Sum(t => t.TotalPrice);
                        Item.totalCNYPrice = Item.Products.Sum(t => t.TotalCNYPrice);
                        Item.totalTraiff = Item.Products.Sum(t => t.Traiff);
                        Item.totalExciseTax = Item.Products.Sum(t => t.ExciseTax);
                        Item.totalAddedValueTax = Item.Products.Sum(t => t.AddedValueTax);
                        Item.totalAgencyFee = Item.Products.Sum(t => t.AgencyFee);
                        Item.totalIncidentalFee = Item.Products.Sum(t => t.IncidentalFee);

                        //ryan 20210113 外单税费小于50不收 钟苑平
                        //if (bill.OrderType != OrderType.Outside && Item.totalTraiff < 50)
                        if (Item.totalTraiff < 50)
                        {
                            Item.totalTraiff = 0;
                        }
                        if (Item.totalExciseTax < 50)
                        {
                            Item.totalExciseTax = 0;
                        }
                        if (Item.totalAddedValueTax < 50)
                        {
                            Item.totalAddedValueTax = 0;
                        }

                        Bills.Add(Item);
                    }
                }

                this.BillTotalQty = Bills.Sum(t => t.totalQty);
                this.BillTotalPrice = Bills.Sum(t => t.totalPrice);
                this.BillTotalCNYPrice = Bills.Sum(t => t.totalCNYPrice);
                this.BillTotalTariff = Bills.Sum(t => t.totalTraiff);
                this.BillTotalExciseTax = Bills.Sum(t => t.totalExciseTax);
                this.BillTotalAddedValueTax = Bills.Sum(t => t.totalAddedValueTax);
                this.BillTotalAgencyFee = Bills.Sum(t => t.totalAgencyFee);
                this.BillTotalIncidentalFee = Bills.Sum(t => t.totalIncidentalFee);

                this.BillTotalTaxAndFee = this.BillTotalTariff + this.BillTotalExciseTax + this.BillTotalAddedValueTax + this.BillTotalAgencyFee + this.BillTotalIncidentalFee;
                this.BillTotalDeclarePrice = this.BillTotalCNYPrice + this.BillTotalTaxAndFee;

                return Bills;
            }
        }

        public MainOrderFile OrderBillFile
        {
            get
            {
                if (this.Orders.Count == 0)
                {
                    return null;
                }
                else
                {
                    var order = this.Orders.FirstOrDefault();
                    // var file = order.MainOrderFiles.Where(t => t.FileType == FileType.OrderBill&&t.Status==Status.Normal).OrderByDescending(t=>t.CreateDate).FirstOrDefault();
                    var file = new CenterLinkXDTFilesTopView().Where(t => t.MainOrderID == order.MainOrderID && t.FileType == FileType.OrderBill && t.Status == Status.Normal).OrderByDescending(t => t.CreateDate).FirstOrDefault();
                    return file;
                }
            }
        }

        public MainOrder MainOrder
        {
            get
            {
                var order = new MainOrdersView().Where(t => t.ID == this.Orders.FirstOrDefault().MainOrderID).FirstOrDefault();
                return order;
            }
        }
    }
}
