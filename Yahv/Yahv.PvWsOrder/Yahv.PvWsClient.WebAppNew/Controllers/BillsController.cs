using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Yahv.PvWsClient.WebAppNew.App_Utils;
using Yahv.PvWsClient.WebAppNew.Controllers.Attribute;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.Underly;
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsClient.WebAppNew.Controllers
{
    public class BillsController : UserController
    {
        #region 仓储对账
        /// <summary>
        /// 列表页面加载
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult WarehouseBills()
        {
            return View();
        }

        /// <summary>
        /// 明细页面加载
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult WarehouseBillDetail()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];
            var data = Yahv.Client.Current.MyOrderBills.SearchByOrderID(id.InputText()).ToArray().Select(item => new
            {
                item.OrderID,
                item.Business,
                item.Catalog,
                item.Subject,
                Currency = item.Currency.GetDescription(),
                item.LeftPrice,
                item.RightPrice,
                item.ReducePrice,
                item.CouponPrice,
                item.Remains,
                CreateDate = item.LeftDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.LeftDate.ToString("yyyy-MM-dd"),
            });
            return View(data);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetWarehouseBills()
        {
            var paramslist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                StartDate = Request.Form["StartDate"],
                EndDate = Request.Form["EndDate"],
                OrderID = Request.Form["OrderID"],
            };
            var WhBillsView = Yahv.Client.Current.MyOrderBills;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<PvWsOrder.Services.ClientModels.Bill, bool>> lambda = item => true;

            //开始时间
            if (!string.IsNullOrWhiteSpace(paramslist.StartDate))
            {
                lambda = item => item.CreateDate >= DateTime.Parse(paramslist.StartDate);
                lambdas.Add(lambda);
            }
            //结束时间
            if (!string.IsNullOrWhiteSpace(paramslist.EndDate))
            {
                lambda = item => item.CreateDate < DateTime.Parse(paramslist.EndDate).AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramslist.OrderID))
            {
                lambda = item => item.ID.Contains(paramslist.OrderID.Trim());
                lambdas.Add(lambda);
            }

            //获取仓储业务的订单类型
            var Ordertypes = new OrderType[] { OrderType.Delivery, OrderType.Recieved, OrderType.Transport };
            //查询数据集
            var data = WhBillsView.GetPageListBills(lambdas.ToArray(), Ordertypes, paramslist.rows, paramslist.page);

            ///转换成页面需要的数据
            var linq = data.Select(item => new
            {
                OrderID = item.ID,
                BillCreateDate = item.BillCreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                BillCreateDateDateString = item.BillCreateDate.ToString("yyyy-MM-dd"),
                TotalPrice = item.Remains,
                Type = item.Type.GetDescription(),
            });

            return this.Paging(linq, data.Total);
        }
        #endregion

        #region 发票管理
        /// <summary>
        /// 发票列表初始化
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult MyInvoices()
        {
            //数据绑定
            ViewBag.OrderTypeOptions = ExtendsEnum.ToDictionary<OrderType>().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            ViewBag.InvoiceStatusOptions = ExtendsEnum.ToDictionary<XDTInvoiceNoticeStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            return View();
        }

        /// <summary>
        /// 发票列表初始化
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult InvoiceDetail()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];
            var InvoiceInfos = Yahv.Alls.Current.XDTInvoice.Where(item => item.TinyOrderID == id).ToArray();
            //总金额
            var totalprice = InvoiceInfos.Sum(item => item.Amount);
            var list = InvoiceInfos.Select(item => new
            {
                item.TaxName,
                item.Model,
                item.Quantity,
                item.UnitName,
                item.Amount,
                InvoiceTaxRate = (item.InvoiceTaxRate.GetValueOrDefault() * 100).ToString("0") + "%",
                item.TaxPrice
            }).ToArray();

            return View(new { list, totalprice });
        }

        /// <summary>
        /// 根据查询条件,查询数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetMyInvoices()
        {
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            var paramslist = new
            {
                startDate = Request.Form["startDate"],
                endDate = Request.Form["endDate"],
                PartNumber = Request.Form["PartNumber"],
                OrderID = Request.Form["OrderID"],
                OrderType = Request.Form["OrderType"],
                InvoiceStatus = Request.Form["InvoiceStatus"],
            };

            //var MyInvoices = Yahv.Client.Current.MyInvoices;
            //转换
            Func<XDTOrderInvoice, object> convert = item => new
            {
                item.OrderID,
                item.TinyOrderID,
                item.InvoiceNo,
                InvoiceDate = item.InvoiceDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                InvoiceDateDateString = item.InvoiceDate?.ToString("yyyy-MM-dd"),
                orderType = item.orderType?.GetDescription(),
                InvoiceNoticeStatus = item.InvoiceStatus?.GetDescription(),
                item.Amount,
                InvoiceType = item.InvoiceType?.GetDescription(),
                InvoiceTypeInt = item.InvoiceType?.GetHashCode(),
                IsShow = item.InvoiceStatus == PvWsOrder.Services.XDTModels.InvoiceStatus.Invoiced,
                EntryID = item.EntryID,
                CustomsTaxNumber = item.Taxs.FirstOrDefault(a => a.TaxType == DecTaxType.Tariff)?.TaxNumber,
                AddedTaxNumber = item.Taxs.FirstOrDefault(a => a.TaxType == DecTaxType.AddedValueTax)?.TaxNumber,
                CustomsTaxID = item.Taxs.FirstOrDefault(a => a.TaxType == DecTaxType.Tariff)?.ID,
                AddedTaxID = item.Taxs.FirstOrDefault(a => a.TaxType == DecTaxType.AddedValueTax)?.ID,
                IsChecked = false, //复选框
            };

            using (var query = Yahv.Client.Current.MyInvoices)
            {
                var MyInvoices = query;

                //开始时间和结束时间
                if (!string.IsNullOrWhiteSpace(paramslist.startDate))
                {
                    MyInvoices = MyInvoices.SearchByStartDate(DateTime.Parse(paramslist.startDate));
                }
                if (!string.IsNullOrWhiteSpace(paramslist.endDate))
                {
                    MyInvoices = MyInvoices.SearchByEndDate(DateTime.Parse(paramslist.endDate).AddDays(1));
                }
                //型号
                if (!string.IsNullOrWhiteSpace(paramslist.PartNumber))
                {
                    MyInvoices = MyInvoices.SearchByPartNumber(paramslist.PartNumber.Trim());
                }
                //订单号
                if (!string.IsNullOrWhiteSpace(paramslist.OrderID))
                {
                    MyInvoices = MyInvoices.SearchByOrderID(paramslist.OrderID.Trim());
                }
                //发票状态
                if (!string.IsNullOrWhiteSpace(paramslist.InvoiceStatus))
                {
                    MyInvoices = MyInvoices.SearchByInvoiceStatus(paramslist.InvoiceStatus);
                }

                //订单类型
                if (!string.IsNullOrWhiteSpace(paramslist.OrderType))
                {
                    MyInvoices = MyInvoices.SearchByOrderType((OrderType)int.Parse(paramslist.OrderType));
                }

                return JsonResult(VueMsgType.success, "", MyInvoices.ToMyPage(convert, page, rows).Json());
            }
        }
        #endregion

        #region 海关发票

        /// <summary>
        /// 海关发票列表初始化
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult MyCusInvoices()
        {
            return View();
        }


        /// <summary>
        /// 根据查询条件,查询数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetMyCusInvoices()
        {
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            var paramslist = new
            {
                startDate = Request.Form["startDate"],
                endDate = Request.Form["endDate"],
                OrderID = Request.Form["OrderID"],
                ContrNo = Request.Form["ContrNo"],
                MultiField = Request.Form["MultiField"],
            };

            //var MyInvoices = Yahv.Client.Current.MyInvoices;
            //转换
            Func<XDTCustomsInvoiceInfo, object> convert = item => new
            {
                item.OrderID,
                item.TinyOrderID,
                item.InvoiceNo,
                EntryID = item.EntryID,
                DeclarePrice = item.DeclarePrice,
                Contrno = item.ContrNo,
                DDate = item.DDate.Value.ToString("yyyy-MM-dd"),
                AgencyAmount = item.AgencyAmount,
                InvoiceDate = item.InvoiceDate?.ToString("yyyy-MM-dd"),
                InvoiceType = item.InvoiceType.GetDescription(),
                CustomsTaxNumber = item.Taxs.FirstOrDefault(a => a.TaxType == DecTaxType.Tariff)?.TaxNumber,
                AddedTaxNumber = item.Taxs.FirstOrDefault(a => a.TaxType == DecTaxType.AddedValueTax)?.TaxNumber,
                CustomsTaxID = item.Taxs.FirstOrDefault(a => a.TaxType == DecTaxType.Tariff)?.ID,
                AddedTaxID = item.Taxs.FirstOrDefault(a => a.TaxType == DecTaxType.AddedValueTax)?.ID,
                IsChecked = false, //复选框
            };

            using (var query = Yahv.Client.Current.MyCusInvoices)
            {
                var MyInvoices = query;

                //过滤服务费开票
                MyInvoices = MyInvoices.SearchByInvoiceType(XDTInvoiceType.Service);

                //开始时间和结束时间
                if (!string.IsNullOrWhiteSpace(paramslist.startDate))
                {
                    MyInvoices = MyInvoices.SearchByStartDate(DateTime.Parse(paramslist.startDate));
                }
                if (!string.IsNullOrWhiteSpace(paramslist.endDate))
                {
                    MyInvoices = MyInvoices.SearchByEndDate(DateTime.Parse(paramslist.endDate).AddDays(1));
                }
                //订单号
                if (!string.IsNullOrWhiteSpace(paramslist.OrderID))
                {
                    MyInvoices = MyInvoices.SearchByOrderID(paramslist.OrderID.Trim());
                }
                //合同号
                if (!string.IsNullOrWhiteSpace(paramslist.ContrNo))
                {
                    MyInvoices = MyInvoices.SearchByContrNo(paramslist.ContrNo.Trim());
                }
                //订单号、合同号
                if (!string.IsNullOrEmpty(paramslist.MultiField))
                {
                    MyInvoices = MyInvoices.SearchByMultiField(paramslist.MultiField.Trim());
                }

                return JsonResult(VueMsgType.success, "", MyInvoices.ToMyPage(convert, page, rows).Json());
            }
        }



        #endregion

        #region 报关对账
        /// <summary>
        /// 报关对账单
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult DeclareBills()
        {
            return View();
        }

        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetDeclareBills()
        {

            var paramslist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                StartDate = Request.Form["StartDate"],
                EndDate = Request.Form["EndDate"],
                OrderID = Request.Form["OrderID"],
            };

            var DeclareBills = Yahv.Client.Current.MyOrderBills;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<PvWsOrder.Services.ClientModels.Bill, bool>> lambda = item => true;
            //开始时间
            if (!string.IsNullOrWhiteSpace(paramslist.StartDate))
            {
                lambda = item => item.CreateDate >= DateTime.Parse(paramslist.StartDate);
                lambdas.Add(lambda);
            }
            //结束时间
            if (!string.IsNullOrWhiteSpace(paramslist.EndDate))
            {
                lambda = item => item.CreateDate <= DateTime.Parse(paramslist.EndDate).AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramslist.OrderID))
            {
                lambda = item => item.ID.Contains(paramslist.OrderID.Trim());
                lambdas.Add(lambda);
            }
            //获取报关业务的订单类型
            var Ordertypes = new OrderType[] { OrderType.Declare, OrderType.TransferDeclare };
            //查询数据集
            var data = DeclareBills.GetPageListBillsForDeclareBill(lambdas.ToArray(), Ordertypes, paramslist.rows, paramslist.page);
            ///转换成页面需要的数据
            var linq = data.Select(item => new
            {
                OrderID = item.ID,
                BillCreateDate = item.BillCreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                BillCreateDateDateString = item.BillCreateDate.ToString("yyyy-MM-dd"),
                TotalPrice = item.Remains,
            });

            return this.Paging(linq, data.Total);
        }

        /// <summary>
        /// 报关对账单
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult DeclareBillDetail()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];
            var current = Client.Current;
            var orderbillProxy = current.MyOrder.GetOrderBillProxy(id);
            if (orderbillProxy == null)
            {
                return View("Error");
            }
            //var isPay = Client.Current.MyXDTOrder.Any(item => item.MainOrderID == orderbillProxy.Order.ID && item.PaidExchangeAmount > 0);
            var isIcgoo = Client.Current.MyXDTOrder.Any(item => item.MainOrderID == orderbillProxy.Order.ID && item.Type == 300);
            var isInside = Client.Current.MyXDTOrder.Any(item => item.MainOrderID == orderbillProxy.Order.ID && item.Type == 100);
            var CurrencyCode = orderbillProxy.orderitems.FirstOrDefault()?.Currency.GetCurrency().ShortName;
            var tinyorders = from item in orderbillProxy.orderitems
                             group item by item.TinyOrderID into items
                             select new
                             {
                                 Client.Current.MyDecHeads.FirstOrDefault(item => item.OrderID == items.Key)?.ContrNo,
                                 CustomsRate = items.FirstOrDefault()?.CustomsExchangeRate,
                                 RealRate = items.FirstOrDefault()?.RealExchangeRate,
                                 TinyOrderID = items.Key,
                                 Items = items.Select(item => new
                                 {
                                     item.Product.PartNumber,
                                     Name = item.ClassfiedName ?? item.Name,
                                     item.Quantity,
                                     item.UnitPrice,
                                     item.TotalPrice,
                                     item.TraiffRate,
                                     item.DeclareTotalPrice,
                                     item.Traiff,
                                     item.ExciseTaxRate,
                                     item.ExcisePrice,
                                     item.AddTaxRate,
                                     item.AddTax,
                                     item.AgencyFee,
                                     item.InspectionFee,
                                     TotalTaxFee = item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee,
                                     TotalDeclareValue = item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee + item.DeclareTotalPrice,
                                 }),
                                 TotalQuantity = items.Sum(item => item.Quantity),
                                 TotalPrice = items.Sum(item => item.TotalPrice),
                                 DeclareTotalPrice = items.Sum(item => item.DeclareTotalPrice),
                                 //ryan 20210113 外单税费小于50不收 钟苑平
                                 TotalTraiff = (items.Sum(item => item.Traiff) < 50) ? 0 : items.Sum(item => item.Traiff),
                                 TotalExcise = (items.Sum(item => item.ExcisePrice) < 50) ? 0 : items.Sum(item => item.ExcisePrice),
                                 TotalAddTax = (items.Sum(item => item.AddTax) < 50) ? 0 : items.Sum(item => item.AddTax),
                                 TotalAgencyFee = items.Sum(item => item.AgencyFee),
                                 TotalInspectionFee = items.Sum(item => item.InspectionFee),
                                 //TotalTaxFee = items.Sum(item => item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee),
                                 //TotalDeclareValue = items.Sum(item => item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee + item.DeclareTotalPrice),
                                 TotalTaxFee = (items.Sum(t => t.Traiff) < 50 ? 0 : items.Sum(t => t.Traiff))
                                             + (items.Sum(t => t.ExcisePrice) < 50 ? 0 : items.Sum(t => t.ExcisePrice))
                                             + (items.Sum(t => t.AddTax) < 50 ? 0 : items.Sum(t => t.AddTax))
                                             + items.Sum(t => t.AgencyFee) + items.Sum(t => t.InspectionFee),
                                 TotalDeclareValue = (items.Sum(t => t.Traiff) < 50 ? 0 : items.Sum(t => t.Traiff))
                                                   + (items.Sum(t => t.ExcisePrice) < 50 ? 0 : items.Sum(t => t.ExcisePrice))
                                                   + (items.Sum(t => t.AddTax) < 50 ? 0 : items.Sum(t => t.AddTax))
                                                   + items.Sum(t => t.AgencyFee) + items.Sum(t => t.InspectionFee) + items.Sum(t => t.DeclareTotalPrice),
                                 isAll = false,
                             };
            //对账单
            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
            var orderBill = new Yahv.PvWsOrder.Services.ClientViews.CenterFilesView().Where(item => item.WsOrderID == id && item.Type == (int)FileType.OrderBill)
                .OrderByDescending(item => item.CreateDate).FirstOrDefault();

            ////税费合计(CNY) 和 报关总金额(CNY) 的值要减去的金额, 注意这是要减去的金额, 小于50才减
            //var forMinusTinyorders = (from item in orderbillProxy.orderitems
            //                          group item by item.TinyOrderID into items
            //                          select new
            //                          {
            //                              ForMinusTotalTraiff = items.Sum(item => item.Traiff) < 50 ? items.Sum(item => item.Traiff) : 0,
            //                              ForMinusTotalExcise = items.Sum(item => item.ExcisePrice) < 50 ? items.Sum(item => item.ExcisePrice) : 0,
            //                              ForMinusTotalAddTax = items.Sum(item => item.AddTax) < 50 ? items.Sum(item => item.AddTax) : 0,
            //                          }).FirstOrDefault();

            //decimal needMinusValue = 0;
            //if (forMinusTinyorders != null)
            //{
            //    needMinusValue = forMinusTinyorders.ForMinusTotalTraiff + forMinusTinyorders.ForMinusTotalExcise + forMinusTinyorders.ForMinusTotalAddTax;
            //}

            //从 AdvanceRecords 表中根据大订单号查询显示在对账单底部文字的"代垫本金"
            decimal amountFor代垫本金 = new AdvanceRecordsView().GetAmountForDeclareTotalPrice(id);

            var model = new
            {
                orderbillProxy.Order.ID,
                CreateDate = orderbillProxy.Order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = orderbillProxy.Order.CreateDate.ToString("yyyy-MM-dd"),
                TinyOrders = tinyorders,
                orderbillProxy.purchaser.CompanyName,
                CompanyAddress = orderbillProxy.purchaser.Address,
                CompanyTel = orderbillProxy.purchaser.Tel,
                orderbillProxy.purchaser.UseOrgPersonTel,
                orderbillProxy.purchaser.BankName,
                orderbillProxy.purchaser.AccountName,
                orderbillProxy.purchaser.AccountId,
                DeclareTotalPrice = amountFor代垫本金, //isPay ? tinyorders.Sum(item => item.DeclareTotalPrice) : 0,--------
                //icgoo类型对账单明细合并时使用start
                TotalQuantity = tinyorders.Sum(item => item.TotalQuantity),
                TotalPrice = tinyorders.Sum(item => item.TotalPrice),
                DeclareTotalPrice2 = tinyorders.Sum(item => item.DeclareTotalPrice),
                TotalTaxFee = tinyorders.Sum(item => item.TotalTaxFee),
                TotalDeclareValue2 = tinyorders.Sum(item => item.TotalDeclareValue),
                TinyOrderItems = tinyorders.SelectMany(item => item.Items),
                IsIcgoo = isIcgoo,
                IsAll = false,
                //---------------end---------------
                TotalTraiff = tinyorders.Sum(item => item.TotalTraiff),
                TotalExcise = tinyorders.Sum(item => item.TotalExcise),
                TotalAddTax = tinyorders.Sum(item => item.TotalAddTax),
                TotalAgencyFee = tinyorders.Sum(item => item.TotalAgencyFee),
                TotalInspectionFee = tinyorders.Sum(item => item.TotalInspectionFee),
                TotalDeclareValue = tinyorders.Sum(item => item.TotalTaxFee) + amountFor代垫本金, //(isPay ? tinyorders.Sum(item => item.TotalDeclareValue) : tinyorders.Sum(item => item.TotalTaxFee)),-------
                DueDate = current.MyAgreement.GetDueDateNew(id).ToString("yyyy年MM月dd日"),  //current.MyAgreement.GetDueDate().ToString("yyyy年MM月dd日"),
                Isbill = orderBill != null,
                billUrl = orderBill == null ? "" : fileurl + orderBill.Url,
                billName = orderBill?.CustomName,
                CurrencyCode,
                orderBillStatus = orderBill == null ? false : orderBill.Status == Services.Models.FileDescriptionStatus.Approved,
            };
            return View(model);
        }

        /// <summary>
        /// 对账导出Excel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult ExportDeclareBills(string id)
        {
            var data = Yahv.Client.Current.MyOrderBills.SearchByOrderID(id.InputText()).ToArray().Select((item, index) => new
            {
                序号 = index + 1,
                账单日期 = item.LeftDate.ToString("yyyy-MM-dd HH:mm:ss"),
                订单编号 = item.OrderID,
                业务类型 = item.Business,
                账单分类 = item.Catalog,
                账单科目 = item.Subject,
                币种 = item.Currency.GetDescription(),
                应收 = item.LeftPrice,
                实收 = item.RightPrice,
                减免 = item.ReducePrice,
                优惠券金额 = item.CouponPrice,
                剩余金额 = item.Remains,
            });
            IWorkbook workbook = ExcelFactory.Create();
            Utils.Npoi.NPOIHelper npoi = new Utils.Npoi.NPOIHelper(workbook);
            int[] columnsWidth = { 10, 20, 20, 15, 15, 30, 15, 10, 15, 10, 30, 10, 10, 10, 10, 10, 10, 10, 10, 40, 20, 30, 30, 30, 30, 30 };
            npoi.EnumrableToExcel(data, 0, columnsWidth);
            //创建文件夹
            var fileName = DateTime.Now.Ticks + ".xlsx";
            string filepath = Server.MapPath("~/Files/") + fileName; //本地路径
            npoi.SaveAs(filepath);
            var localpath = PvWsOrder.Services.PvClientConfig.DomainUrl + "/Files/" + fileName; //浏览器路径
            return base.JsonResult(VueMsgType.success, "", localpath);
        }
        #endregion
    }
}
