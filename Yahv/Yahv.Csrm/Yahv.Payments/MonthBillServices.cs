using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Payments.Views;
using Yahv.Underly;
using static Yahv.Payments.MonthBillServices.MonthBill;
using Yahv.Utils.Serializers;
using static Yahv.Payments.MonthBillServices;

namespace Yahv.Payments
{
    public class MonthBillServices
    {
        /// <summary>
        /// 客户的月结账单
        /// </summary>
        public class MonthBill
        {
            /// <summary>
            /// 财务账
            /// </summary>
            public List<financeBill> FinanceBill { get; internal set; }
            /// <summary>
            /// 总计
            /// </summary>
             public List<BillTotal> Subtotal_finace { get; set; }
            /// <summary>
            /// 服务费
            /// </summary>
            public List<RelatedInvoice> Invoices_servcing { get; internal set; }
            /// <summary>
            /// 全额发票
            /// </summary>
            public List<RelatedInvoice> Invoices_vat { get; internal set; }
            ///// <summary>
            ///// 开票类型
            ///// </summary>
            public InvoiceType InvoiceType { get; set; }
            /// <summary>
            /// 给前端提供json的
            /// </summary>
            //public object Spz { get; internal set; }

            internal MonthBill()
            {

            }
        }

        public MonthBill Single(string payer, int dateIndex, string payee = null)
        {
            // 获取客户ID
            string clientID = payer;
            // 获取收款公司，收益公司
            string payeeID = payee;
            // 固定代仓储业务
            string conduct = Business.WarehouseServicing.GetDescription();


            #region 财务账单

            using (var reponsitory = new PvbCrmReponsitory())
            {
                #region 原始数据

                var receivables_linq = from rece in reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Receivables>()
                                       where rece.ChangeIndex == dateIndex && rece.Payer == clientID && rece.Business == conduct &&rece.OrderID!=null
                                       select new
                                       {
                                           rece.ID,
                                           rece.Business,
                                           rece.Catalog,
                                           rece.Subject,
                                           rece.Payer,
                                           rece.Payee,
                                           SettlementCurrency = (Currency)rece.SettlementCurrency,
                                           rece.SettlementPrice,
                                           rece.OrderID,
                                           rece.AdminID,
                                           rece.OriginalDate,
                                           rece.ChangeDate,

                                       };
                var receivables = receivables_linq.ToArray();

                var receiveds_linq = from rece in reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Receiveds>()
                                     join left in receivables_linq.Select(item => item.ID).Distinct() on rece.ReceivableID equals left
                                     select new
                                     {
                                         rece.ReceivableID,
                                         rece.AccountCode,
                                         rece.Price,
                                         //rece.PaidPrice,
                                         rece.CouponID,
                                         rece.CreateDate,
                                         rece.AccountType

                                     };
                var receiveds = receiveds_linq.ToArray();

                //var rblambda = lambdas.First(item => item is Expression<Func<Layers.Data.Sqls.PvbCrm.Receivables, bool>>)
                //    as Expression<Func<Layers.Data.Sqls.PvbCrm.Receivables, bool>>;
                //receivablesView = receivablesView.Where(rblambda);


                var localsLinq = from rv in receivables
                                 join rd in receiveds on rv.ID equals rd.ReceivableID into rds
                                 select new
                                 {
                                     ReceivableID = rv.ID,
                                     OrderID = rv.OrderID,
                                     Payee = rv.Payee,
                                     Payer = rv.Payer,
                                     Business = rv.Business,
                                     Catalog = rv.Catalog,
                                     Subject = rv.Subject,
                                     Currency = rv.SettlementCurrency,
                                     LeftPrice = rv.SettlementPrice,
                                     LeftDate = rv.ChangeDate,
                                     AdminID = rv.AdminID,
                                   // AccountType = receiveds?.FirstOrDefault().AccountType,
                                     receiveds = rds.Select(item => new
                                     {
                                         item.ReceivableID,
                                         item.AccountCode,
                                         item.Price,
                                         item.AccountType,
                                         item.CouponID,
                                         item.CreateDate
                                     }),
                                 };

                #endregion

                var finaces = from pd in localsLinq
                              group pd by pd.LeftDate into groups_pd
                              select new financeBill
                              {
                                  PayDate = groups_pd.Key,
                                  Items = (from c in groups_pd
                                           group c by c.Currency into groups_c
                                           select new CurrenryData
                                           {
                                               CurrencyName = groups_c.Key.GetDescription(),
                                               Currency = groups_c.Key,
                                               Items = (from o in groups_c
                                                        group o by o.OrderID into groups_o
                                                        select new OrderData
                                                        {
                                                            OrderID = groups_o.Key,
                                                            Items = groups_o.Select(item => new BillData
                                                            {
                                                                Business = item.Business,
                                                                Catalog = item.Catalog,
                                                                Subject = item.Subject,
                                                                LeftDate = item.LeftDate,
                                                                OrderID = item.OrderID,
                                                                Currency = item.Currency,
                                                                LeftPrice = item.LeftPrice,
                                                                //Items = item.receiveds,
                                                                RightPrice = item.receiveds.Sum(r => r.Price),
                                                                NotPayAmount=item.LeftPrice- item.receiveds.Sum(r => r.Price),
                                                            })
                                                        })
                                           })
                              };

                //应收与实收 总计
                var subtotal_finace = from item in localsLinq
                                      group item by item.Currency into groups
                                      select new BillTotal
                                      {

                                          CurrencyName=groups.Key.GetDescription(),
                                          Currency = groups.Key,
                                          LeftPrice = groups.Sum(item => item.LeftPrice),
                                          RightPrice = groups.Sum(item =>item.receiveds.Sum(r => r.Price))
                                      };


                var ordersID = localsLinq.Select(item => item.OrderID);

                ///应开的服务费发票或 全额发票
                var invoices = new OrderInvoiceSynonymTopView().Where(x => ordersID.Contains(x.OrderID)).ToArray();
                //实开的服务费发票或 全额发票
                var openedInvoices = new OpenedServiceInvoice().Where(x => ordersID.Contains(x.OrderID)).ToArray();
                //海关发票
                var customsInvoices = new CustomsInvoiceSynonymTopView().Where(x => ordersID.Contains(x.OrderID)).ToArray();

                //服务费发票
                var invoices_servcing = from item in invoices
                                        join opend in openedInvoices on item.OrderID equals opend.OrderID into opens
                                        join _custom in customsInvoices on item.OrderID equals _custom.OrderID into customs
                                        where item.InvoiceType == InvoiceType.Servicing
                                        select new RelatedInvoice
                                        {
                                            OrderID=  item.OrderID,
                                            LeftPrice= item.LeftPrice,
                                            IsOpened = opens.Sum(o => o.RightPrice).HasValue,
                                            InvoiceNo = item.InvoiceNo,
                                            CustomsLeft=customs.Sum(o=>o.CustomsLeft),
                                            CustomsRight = customs.Sum(o => o.RightPrice),
                                            InvoiceDate = customs.Max(o=>o.InvoiceDate)
                                        };


                //全额
                var invoices_vat = from item in invoices
                                   join opend in openedInvoices on item.OrderID equals opend.OrderID into opens
                                   where item.InvoiceType == InvoiceType.VATInvoice
                                   select new RelatedInvoice
                                   {
                                       OrderID = item.OrderID,
                                       LeftPrice = item.LeftPrice,
                                       RightPrice = opens.Sum(o => o.RightPrice),
                                       InvoiceNo = string.Join("|", opens.Select(o => o.InvoiceNo).Distinct())
                                       
                                   };


                var result = new MonthBill
                {
                    FinanceBill = finaces.ToList(),
                    Subtotal_finace = subtotal_finace.ToList(),
                    Invoices_servcing = invoices_servcing.ToList(),
                    Invoices_vat = invoices_vat.ToList(),
                    InvoiceType = invoices.FirstOrDefault()==null?InvoiceType.VATInvoice: invoices.FirstOrDefault().InvoiceType
                };
                return result;
                #region  注掉的
                ////1.按照还款日期分组
                //var dateGroup = linq.ToList().Where(x => x.OrderID != null).GroupBy(x => x.LeftDate).ToDictionary(x => x.Key, b => b.ToList());
                //var result = new List<financeBill>();
                //foreach (var dateData in dateGroup)
                //{
                //    var dateResult = new financeBill { PayDate = dateData.Key, OrderData = new List<OrderData>() };
                //    var orderGroup = dateData.Value.GroupBy(b => b.OrderID).ToDictionary(k => k.Key, v => v.ToList());
                //    foreach (var orderData in orderGroup)
                //    {
                //        //2.按照订单分组
                //        var orderResult = new OrderData { OrderID = orderData.Key, CurrenryData = new List<CurrenryData>(), InvoiceData = new RelatedInvoice(), CustomInvoiceData = new RelatedInvoice() };
                //        var currenryGroup = orderData.Value.GroupBy(b => b.Currency).ToDictionary(k => k.Key, v => v.ToList());


                //        foreach (var currenryData in currenryGroup)
                //        { //3.按照币种分组
                //            var currenryResult = new CurrenryData { Currency = currenryData.Key, BillData = new List<BillData>() };
                //            var receivableGroup = currenryData.Value.GroupBy(b => b.ReceivableID).ToDictionary(k => k.Key, v => v.ToList());
                //            foreach (var item in receivableGroup)
                //            {
                //                var receivableGroupFirst = item.Value.FirstOrDefault();
                //                var rightRrice = item.Value.Sum(s => s.receiveds.Sum(r => r.Price));
                //                var paidPrice = 0;// item.Value.Sum(s => s.receiveds.Sum(r=>r.PaidPrice));
                //                var accountType = (AccountType)receivableGroupFirst.AccountType;
                //                currenryResult.BillData.Add(new BillData
                //                {
                //                    ReceivableID = item.Key,
                //                    Business = receivableGroupFirst.Business,
                //                    Catalog = receivableGroupFirst.Catalog,
                //                    Subject = receivableGroupFirst.Subject,
                //                    LeftPrice = receivableGroupFirst.LeftPrice,
                //                    AccountType = accountType,
                //                    RightPrice = rightRrice,
                //                    CreditPay = accountType == AccountType.CreditCost ? rightRrice : 0M,
                //                    PaidPrice = accountType == AccountType.CreditCost ? paidPrice : 0M,
                //                    BankPay = accountType == AccountType.Cash ? rightRrice : 0M,
                //                    NotPayAmount = (receivableGroupFirst.LeftPrice - rightRrice),
                //                    LeftDate = receivableGroupFirst.LeftDate
                //                });
                //            }

                //            orderResult.CurrenryData.Add(currenryResult);
                //        }

                //        //4.处理发票
                //        //首先判断是全额还是服务费 ，服务费的需 +海关发票
                //        var notOpenInvoice = invoice.FirstOrDefault(x => x.OrderID == orderData.Key);
                //        // var open = openedInvoice.Where(x => x.OrderID == orderData.Key);
                //        if (notOpenInvoice.InvoiceType == InvoiceType.VATInvoice)
                //        {

                //            orderResult.InvoiceType = 0;
                //            orderResult.InvoiceData.LeftPrice = notOpenInvoice.LeftPrice;
                //            orderResult.InvoiceData.OrderID = notOpenInvoice.OrderID;
                //            var open = openedInvoice.Where(x => x.OrderID == orderData.Key);
                //            //  var InvoiceNo = open.Select(x => x.InvoiceNo).Distinct();
                //            // InvoiceNo = string.Join(";", InvoiceNo);
                //            decimal rightPrice = 0;
                //            string invoiceNo = "";
                //            string[] ss = { };
                //            // 处理发票号
                //            if (open != null)
                //            {
                //                foreach (var item in open)
                //                {
                //                    invoiceNo = item.InvoiceNo + ";";
                //                    rightPrice += item.RightPrice.Value;
                //                }
                //                orderResult.InvoiceData.RightPrice = rightPrice;
                //                orderResult.InvoiceData.InvoiceNo = invoiceNo;
                //                orderResult.InvoiceData.InvoiceDate = open.FirstOrDefault().InvoiceDate;
                //            }

                //        }
                //        else
                //        {
                //            orderResult.InvoiceType = 1;
                //            orderResult.InvoiceData = openedInvoice.FirstOrDefault(x => x.OrderID == orderData.Key);
                //            orderResult.InvoiceData.LeftPrice = notOpenInvoice.LeftPrice;
                //            orderResult.InvoiceData.OrderID = notOpenInvoice.OrderID;
                //            orderResult.CustomInvoiceData = customsInvoice.FirstOrDefault(x => x.OrderID == orderData.Key);

                //        }

                //        dateResult.OrderData.Add(orderResult);
                //    }

                //    result.Add(dateResult);
                //}

                //bill.FinanceBill = result;
                // return bill;
                #endregion
            }

            #endregion

            #region 商品账 （本期不做）

            #endregion

            #region 开票情况
            //获取当前客户的所有发票数据
            //var invoice = new OrderInvoiceSynonymTopView().Where(x => x.ClientName == clientName).ToArray();
            //var customsInvoice = new CustomsInvoiceSynonymTopView().Where(x => x.ClientName == clientName).ToArray();

            #endregion

            //return bill;
        }

    }

    //public class MothBillServicesForStudy
    //{
    //    public object Paging(string where, int pi, int ps) { return null; }
    //    public object single(string id) { return null; }

    //    public void Enter(object entity) { }
    //    public void Delete(string id) { }

    //}

    //public class MyClassForStudy
    //{

    //    public void Enter() { }
    //    public void Delete() { }
    //}
}
