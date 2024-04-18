using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Invoice
{
    public partial class Apply : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// </summary>
        protected void LoadData()
        {
            string IDs = Request.QueryString["IDs"];
            var orderID = IDs.Split(',')[0];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnInvoicedOrders[orderID];
            //订单客户
            var client = order.Client;
            //客户发票
            var invoice = client.Invoice;
            //客户补充协议
            var agreement = order.ClientAgreement;
            //发票的邮寄信息
            var invoiceConsignee = client.InvoiceConsignee;

            this.Model.InvoiceData = new
            {
                InvoiceType = agreement.InvoiceType.GetDescription(),
                DeliveryType = invoice.DeliveryType.GetDescription(),
                CompanyName = order.Client.Company.Name,
                TaxCode = invoice.TaxCode,
                BankInfo = invoice.BankName + "  " + invoice.BankAccount,
                AddressTel = invoice.Address + "  " + invoice.Tel
            }.Json();
            this.Model.MaileDate = new
            {
                ReceipCompany = order.Client.Company.Name,
                ReceiterName = invoiceConsignee.Name,
                ReceiterTel = invoiceConsignee.Mobile,
                DetailAddres = invoiceConsignee.Address
            }.Json();
        }

        /// <summary>
        /// 加载产品信息
        /// </summary>
        protected void ProductData()
        {
            var IDs = Request.QueryString["IDs"].Split(',');
            //取第一个订单
            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnInvoicedOrders.Where(x => IDs.Contains(x.ID)).ToArray();
            var order = orders.FirstOrDefault();
            //订单客户
            var client = order.Client;
            //客户补充协议
            var agreement = order.ClientAgreement;

            InvoiceItemAmountCalc calc = new InvoiceItemAmountCalc(IDs.ToList());
            List<InvoiceItemAmountHelp> helper = calc.AmountResult();

            //开票类型筛选
            if (agreement.InvoiceType == InvoiceType.Full)
            {
                #region 
                // var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceOrderItem.Where(x => IDs.Contains(x.OrderID));
                //var listInvoiceOrderItem = orderItem.Select(item => new
                //{
                //    OrderID = item.OrderID,
                //    OrderItemID = item.ID,
                //    ProductName = item.Category.Name,  //产品名称
                //    ProductModel = item.Product.Model,//型号
                //    item.Unit,
                //    item.Quantity,
                //    Price = item.UnitPrice,
                //    TotalPrice = item.TotalPrice.ToRound(2),
                //    agreement.InvoiceTaxRate, //税率
                //    Amount = item.GetSalesTotalPriceRat(orders, agreement.InvoiceTaxRate),
                //    UnitPrice = item.SalesUnitPriceRat,
                //    item.TaxName,//税务名称
                //    item.TaxCode,
                //    Difference = 0.0000,
                //}).ToList();

                //var totaldata = new
                //{
                //    Amount = listInvoiceOrderItem.Sum(t => t.Amount).ToRound(2),
                //    TotalPrice = listInvoiceOrderItem.Sum(t => t.TotalPrice).ToRound(2),
                //};

                //Response.Write(new
                //{
                //    rows = listInvoiceOrderItem,
                //    total = listInvoiceOrderItem.Count(),
                //    totaldata = totaldata,
                //}.Json());
                #endregion
                var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceOrderItem.Where(x => IDs.Contains(x.OrderID));
               
                //前台显示
                Func<InvoiceOrderItem, object> convert = item => new
                {
                    OrderID = item.OrderID,
                    OrderItemID = item.ID,
                    ProductName = item.Category.Name,  //产品名称
                    ProductModel = item.Model,//型号
                    item.Unit,
                    item.Quantity,
                    Price = item.UnitPrice,
                    TotalPrice = item.TotalPrice.ToRound(2),
                    agreement.InvoiceTaxRate, //税率
                    Amount = item.GetSalesTotalPriceRatSpeed(orders, agreement, helper),
                    UnitPrice = item.SalesUnitPriceRat,
                    item.TaxName,//税务名称
                    item.TaxCode,
                    Difference = 0.0000,
                };

                List<InvoiceOrderItem> listInvoiceOrderItem = orderItem.ToList();

                var totaldata = new
                {
                    Amount = listInvoiceOrderItem.Sum(t => t.GetSalesTotalPriceRatSpeed(orders, agreement, helper)).ToRound(2),
                    TotalPrice = listInvoiceOrderItem.Sum(t => t.TotalPrice).ToRound(2),
                };

                Response.Write(new
                {
                    rows = listInvoiceOrderItem.Select(convert).ToList(),
                    total = listInvoiceOrderItem.Count(),
                    totaldata = totaldata,
                }.Json());


            }
            else
            {
                var orderFees = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderPremiums.Where(item => IDs.Contains(item.OrderID)).ToArray();
                var totalPrice = orderFees == null ? 0M : orderFees.Sum(fee => fee.Count * fee.UnitPrice * fee.Rate);

                //var orderFees = helper.Sum(t => t.AgencyFee) + helper.Sum(t => t.MiscFees);
                //var totalPrice = orderFees;
                //代理费和杂费（含商检费）

                //含税总金额
                var totalPriceWithTax = totalPrice * (1 + agreement.InvoiceTaxRate);

                List<object> list = new List<object>() {
                    new {
                        ProductName = "*物流辅助服务*服务费",
                        Quantity = 1,
                        InvoiceTaxRate = agreement.InvoiceTaxRate,
                        Price = totalPrice.ToRound(4),
                        TotalPrice = totalPrice.ToRound(2),
                        UnitPrice = totalPriceWithTax.ToRound(4),
                        Amount = totalPriceWithTax.ToRound(2),
                        TaxName="*物流辅助服务*服务费",//税务名称
                        TaxCode="3040407040000000000"
                    }
                };

                var totaldata = new
                {
                    Amount = totalPriceWithTax.ToRound(2),
                    TotalPrice = totalPrice.ToRound(2),
                };

                Response.Write(new
                {
                    rows = list.ToList(),
                    total = list.Count(),
                    totaldata = totaldata,
                }.Json());
            }
        }


        //protected void ProductData()
        //{
        //    var IDs = Request.QueryString["IDs"].Split(',');
        //    //取第一个订单
        //    var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnInvoicedOrders.Where(x => IDs.Contains(x.ID));
        //    //订单客户
        //    var client = order.FirstOrDefault().Client;
        //    //客户补充协议
        //    var agreement = client.Agreement;

        //    //开票类型筛选
        //    if (agreement.InvoiceType == InvoiceType.Full)
        //    {
        //        //Stopwatch stopwatch = new Stopwatch();

        //        //stopwatch.Start();
        //        var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceOrderItem.Where(x => IDs.Contains(x.OrderID));
        //        var listInvoiceOrderItem = new List<dynamic>();
        //        decimal amount = 0;
        //        decimal totalPrice = 0;
        //        foreach (InvoiceOrderItem item in orderItem)
        //        {
        //            var currentAmount = item.GetSalesTotalPriceRat(order);
        //            var currentTotalPrice = item.TotalPrice.ToRound(2);
        //            amount += currentAmount;
        //            totalPrice += currentTotalPrice;
        //            listInvoiceOrderItem.Add(new
        //            {
        //                OrderID = item.OrderID,
        //                OrderItemID = item.ID,
        //                ProductName = item.Category.Name,  //产品名称
        //                ProductModel = item.Product.Model,//型号
        //                item.Unit,
        //                item.Quantity,
        //                Price = item.UnitPrice,
        //                TotalPrice = currentTotalPrice,
        //                agreement.InvoiceTaxRate, //税率
        //                Amount = currentAmount,
        //                UnitPrice = item.SalesUnitPriceRat,
        //                item.TaxName,//税务名称
        //                item.TaxCode,
        //                Difference = 0.0000,
        //            });
        //        }

        //        var totaldata = new
        //        {
        //            Amount = amount.ToRound(2),
        //            TotalPrice = totalPrice.ToRound(2),
        //        };

        //        //前台显示
        //        //Func<InvoiceOrderItem, object> convert = item => new
        //        //{
        //        //    OrderID = item.OrderID,
        //        //    OrderItemID = item.ID,
        //        //    ProductName = item.Category.Name,  //产品名称
        //        //    ProductModel = item.Product.Model,//型号
        //        //    item.Unit,
        //        //    item.Quantity,
        //        //    Price = item.UnitPrice,
        //        //    //Currency = order.Currency,
        //        //    TotalPrice = item.TotalPrice.ToRound(2),
        //        //    agreement.InvoiceTaxRate, //税率
        //        //    UnitPrice = item.SalesUnitPriceRat,
        //        //    Amount = item.SalesTotalPriceRat.ToRound(2),//含税总额
        //        //    item.TaxName,//税务名称
        //        //    item.TaxCode,
        //        //    Difference = 0.0000,
        //        //};

        //        Response.Write(new
        //        {
        //            //rows = listInvoiceOrderItem.Select(convert).ToList(),
        //            rows = listInvoiceOrderItem,
        //            total = listInvoiceOrderItem.Count(),
        //            totaldata = totaldata,
        //        }.Json());

        //        //stopwatch.Stop();
        //        //TimeSpan timeSpan = stopwatch.Elapsed;
        //    }
        //    else
        //    {
        //        var orderFees = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderPremiums.Where(item => IDs.Contains(item.OrderID));
        //        //代理费和杂费（含商检费）
        //        var totalPrice = orderFees.Sum(fee => fee.Count * fee.UnitPrice * fee.Rate);
        //        //含税总金额
        //        var totalPriceWithTax = totalPrice * (1 + agreement.InvoiceTaxRate);

        //        List<object> list = new List<object>() {
        //            new {
        //                ProductName = "*物流辅助服务*服务费",
        //                Quantity = 1,
        //                InvoiceTaxRate = agreement.InvoiceTaxRate,
        //                Price = totalPrice.ToRound(4),
        //                TotalPrice = totalPrice.ToRound(2),
        //                UnitPrice = totalPriceWithTax.ToRound(4),
        //                Amount = totalPriceWithTax.ToRound(2),
        //                TaxName="*物流辅助服务*服务费",//税务名称
        //                TaxCode="3040407040000000000"
        //            }
        //        };

        //        var totaldata = new
        //        {
        //            Amount = totalPriceWithTax.ToRound(2),
        //            TotalPrice = totalPrice.ToRound(2),
        //        };

        //        Response.Write(new
        //        {
        //            rows = list.ToList(),
        //            total = list.Count(),
        //            totaldata = totaldata,
        //        }.Json());
        //    }
        //}

        /// <summary>
        /// 申请开票
        /// </summary>
        protected void SubmitApply()
        {
            try
            {
                decimal limit = InvoiceXmlConfig.XianEPerFp;
                string AmountLimit = Request.Form["AmountLimit"];
                if (!string.IsNullOrEmpty(AmountLimit))
                {
                    decimal alimit = Convert.ToDecimal(AmountLimit);
                    if(alimit> limit)
                    {
                        Response.Write((new { success = false, message = "限额不能大于"+ limit }).Json());
                        return;
                    }
                    limit = alimit;
                }

                string IDs = Request.Form["IDs"];
                string[] arrId = IDs.Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');
                string Summary = Request.Form["Summary"];               
                string ProductData = Request.Form["ProductData"].Replace("&quot;", "'");
                IEnumerable<InvoiceNoticeItem> noticeItems = ProductData.JsonTo<IEnumerable<InvoiceNoticeItem>>();

                var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnInvoicedOrders.Where(item => arrId.Contains(item.ID)).AsEnumerable();
                var InvoiceContext = new InvoiceContext();
                InvoiceContext.Apply = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                InvoiceContext.orders = orders;
                InvoiceContext.Summary = Summary;
                InvoiceContext.noticeItems = noticeItems;
                InvoiceContext.AmountLimit = limit;
                InvoiceContext.Client = orders.FirstOrDefault().Client;
                InvoiceContext.SubmitApply();
                Response.Write((new { success = true, message = "申请成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "申请失败：" + ex.Message }).Json());
            }
        }
    }
}