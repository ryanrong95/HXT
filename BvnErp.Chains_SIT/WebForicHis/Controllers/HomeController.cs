using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebForicHis.Models;

namespace WebForicHis.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// 获取大赢家供应商和PI
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGoricHis()
        {

            var dyjIDs = new List<string>() {
"19527162910656"
            };

            //var dyjIDs = new Needs.Ccs.Services.Views.IcgooMapView().OrderByDescending(t => t.CreateDate).Select(t => t.IcgooOrder).Distinct();

            //new ForicOrderInfo("191261342928").SaveForicHisData(); 

            foreach (var id in dyjIDs)
            {
                new ForicOrderInfo(id).SaveForicHisData();
            }

            return View();
        }


        public ActionResult GetForicAll()
        {

            //var dyjIDs = new Needs.Ccs.Services.Views.IcgooMapView().OrderByDescending(t => t.CreateDate).Select(t => t.IcgooOrder).Distinct();

            var dyjIDs = new List<string>() {
                "198116040481",
"20211144722444",
"20428161544346",
"204291661510",
"2054135720803",
"2055141228177",
"2055154457650",
"20551518631",
"2055143427937",
"2055143153460",
"20549141714",
"2056125930193"


            };

            foreach (var id in dyjIDs)
            {
                new ForicOrderInfo(id).SaveForicAllData();
            }

            return View();
        }


        /// <summary>
        /// 获取大赢家ProductUnionCode
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProductUnionCode()
        {

            //var dyjIDs = new Needs.Ccs.Services.Views.IcgooMapView().OrderByDescending(t => t.CreateDate).Select(t => t.IcgooOrder).Distinct();
            //var dyjIDs = new List<string>() { //"196221549784",
            //                                "19624131653827",
            //                                "19624153949397",
            //                                "1962416712195",
            //                                "19625125659960",
            //                                "19625155138985",
            //                                //"19625165652857",
            //                                //"1962516744896",
            //                                "19626125645366",
            //                                "19626153743205",
            //                                "1962615438355",
            //                                "1962713741243",
            //                                "19627153637220",
            //                                //"19627154518468",
            //                                "196281356112",
            //                                "19628154410429",
            //                                "1962815508634",
            //                                "19629122235943",
            //                                "19629142426625",
            //                                "1962914278122",
            //                                "1972125322852",
            //                                "1972153829994",
            //                                //"197215431599"
            //};


            //var dyjIDs = new List<string>() { "19624131653827",
            //                                "19624153949397",
            //                                "19625125659960",
            //                                "19625155138985",
            //                                "19626125645366",
            //                                "19626153743205",
            //                                "1962713741243",
            //                                "19627153637220",
            //                                "196281356112",
            //                                "19628154410429",
            //                                "1962815508634",
            //                                "19629122235943",
            //                                "19629142426625",
            //                                "1972125322852",
            //                                "1972153829994" };

            var dyjIDs = new List<string>() {
"19618152450720",
"1961915247991",
"19619153651409",
"1962113313926",
"19622125946378",
"19624131653827",
"19624153949397",
"19625125659960",
"19625155138985",
"19626125645366",
"19626153743205",
"1962713741243",
"19627153637220",
"196281356112",
"19628154410429",
"1962815508634",
"19629122235943",
"19629142426625",
"1972125322852",
"1972153829994",
"19629122235943B"

            };


            new ForicOrderInfo("19724125726113").SaveProductUnionCode(); //191210153411606
            //foreach (var id in dyjIDs)
            //{
            //    new ForicOrderInfo(id).SaveProductUnionCode();
            //}

            return View();
        }

        /// <summary>
        /// 获取icgoo的PI文件
        /// </summary>
        /// <returns></returns>
        public ActionResult GetIcgooHis()
        {
            var date = DateTime.Parse("2020-12-03 01:01:01");
            var dadate = DateTime.Parse("2019-05-30 11:41:49");
            //var icgooIDs = new Needs.Ccs.Services.Views.IcgooMapView().Where(t => t.CreateDate < date && t.CreateDate > dadate && t.IcgooOrder.Contains("CM")).OrderByDescending(t => t.CreateDate).Select(t => t.IcgooOrder).Distinct();

            var icgooIDs = new List<string>() {
            "CM23247",
"CM23495",
"CM23515",
"CM23849",
"CM24067",
"CM24131",
"CM24390",
"CM24567",
"CM24723",
"CM24823",
"CM24834",
"CM24916",
"CM24999",
"CM25019",
"CM25101",
"CM25133",
"CM25209",
"CM25434",
"CM25526",
"CM25564",
"CM25675",
"CM25743",
"CM25749",
"CM25941"
            };


            foreach (var id in icgooIDs)
            {
                new IcgooOrderInfo(id).GetPI();
            }


            return View();
        }

        /// <summary>
        /// 批量导出销售合同
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportSalesContract()
        {

            var ss = new Needs.Ccs.Services.Views.OrdersView().Where(t=>t.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled &&t.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned
                                                                    && t.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Classified && t.ClientAgreement.InvoiceType == Needs.Ccs.Services.Enums.InvoiceType.Full).Select(t=>t.MainOrderID).Distinct();
            //var ss = new List<string>();
            //ss.Add("WL47220200909001");
            foreach (var id in ss)
            {
                //取第一个订单
                var orders = new Needs.Ccs.Services.Views.SalesContractOrdersView().Where(x => x.MainOrderID == id).ToArray();

                var orderIDs = orders.Select(t => t.ID).ToList();
                var order = orders.FirstOrDefault();
                //订单客户
                var client = order.Client;
                //客户补充协议
                var agreement = client.Agreement;

                //基础信息
                var model = new Needs.Ccs.Services.Models.SalesContract
                {
                    ID = id,
                    SalesDate = order.CreateDate,
                    Buyer = new Needs.Ccs.Services.Models.InvoiceBaseInfo
                    {
                        Title = client.Invoice.Title,
                        Address = client.Invoice.Address,
                        BankName = client.Invoice.BankName,
                        BankAccount = client.Invoice.BankAccount,
                        Tel = client.Invoice.Tel
                    },
                    Seller = new Needs.Ccs.Services.Models.InvoiceBaseInfo
                    {
                        Title = "深圳市华芯通供应链管理有限公司",
                        Address = "深圳市龙岗区吉华路393号英达丰科技园1号楼",
                        BankName = "中国银行股份有限公司深圳罗岗支行",
                        BankAccount = "764071904447",
                        Tel = "0755-83988698",
                        SealUrl = "Content\\images\\SZXDT.png"
                    },
                    InvoiceType = agreement.InvoiceType
                };

                //型号信息
                var salesItems = new List<ContractItem>();

                var orderItem = new Needs.Ccs.Services.Views.InvoiceOrderItemView().Where(x => orderIDs.Contains(x.OrderID));
                InvoiceItemAmountCalc calc = new InvoiceItemAmountCalc(orderIDs);
                List<InvoiceItemAmountHelp> helper = calc.AmountResult();
                var units = new Needs.Ccs.Services.Views.BaseUnitsView().ToList();

                foreach (var item in orderItem)
                {
                    var sale = new ContractItem
                    {

                        OrderItemID = item.ID,
                        ProductName = item.Name,
                        Model = item.Model,
                        Quantity = item.Quantity,
                        Unit = units.Where(u => u.Code == item.Unit).FirstOrDefault()?.Name ?? item.Unit,
                        //UnitPrice = item.UnitPrice,
                        TotalPrice = item.GetSalesTotalPriceRatSpeed(orders, agreement, helper)
                    };

                    salesItems.Add(sale);
                }

                model.ContractItems = salesItems;

                //保存文件
                string fileName = id + ".pdf";

                var contractPdf = new Needs.Ccs.Services.Models.SalesContractToPdf(model);
                contractPdf.SaveAs(System.IO.Path.Combine("D:/foric/admin/Files/Sales/", fileName));
            }


            

            return View();
        }

    }
}