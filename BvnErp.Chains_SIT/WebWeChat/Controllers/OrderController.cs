using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl.Logs.Services;
using Needs.Wl.Models;
using Needs.Wl.Web.WeChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebWeChat.Models;
using WeChatPlat = Needs.Wl.User.Plat.WeChatPlat;

namespace WebWeChat.Controllers
{
    [UserHandleError(ExceptionType = typeof(Exception))]
    [UserAuthorize(UserAuthorize = true)]
    public class OrderController : UserController
    {
        #region 我的订单

        public ActionResult MyOrders(string id)
        {
            var model = new MyOrdersViewModel();
            //数据绑定
            model.InvoiceStatusOptions = EnumUtils.ToDictionary<InvoiceStatus>().Select(item => new { value = item.Key, text = item.Value }).Json();
            model.PayExchangeStatusOptions = EnumUtils.ToDictionary<PayExchangeStatus>().Select(item => new { value = item.Key, text = item.Value }).Json();
            model.Index = id;
            return View(model);
        }

        /// <summary>
        /// 获取我的订单
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMyOrderList()
        {
            var invoiceStatus = Request.Form["invoiceStatus"];  //开票状态
            var payExchangeStatus = Request.Form["payExchangeStatus"];  //付汇状态
            var orderDate = Request.Form["orderDate"]; //订单日期

            var orders = WeChatPlat.Current.WebSite.MyOrdersExtends1;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<UserOrderExtends, bool>> expression = item => true;

            if ((!string.IsNullOrWhiteSpace(orderDate)) && orderDate != "all")
            {
                if (orderDate == "curMonth") //当月订单
                {
                    Expression<Func<UserOrderExtends, bool>> lambda = item => item.CreateDate.Month == DateTime.Now.Month;
                    lambdas.Add(lambda);
                }
                else if (orderDate == "thrMonth")  //三个月的订单
                {
                    Expression<Func<UserOrderExtends, bool>> lambda = item => item.CreateDate >= DateTime.Now.AddMonths(-3);
                    lambdas.Add(lambda);
                }
                else if (orderDate == "curYear")  //当年订单
                {
                    Expression<Func<UserOrderExtends, bool>> lambda = item => item.CreateDate.Year >= DateTime.Now.Year;
                    lambdas.Add(lambda);
                }
            }
            if ((!string.IsNullOrWhiteSpace(invoiceStatus)) && invoiceStatus != "all") //开票状态
            {
                Expression<Func<Needs.Ccs.Services.Models.UserOrderExtends, bool>> lambda = item => item.InvoiceStatus == (InvoiceStatus)int.Parse(invoiceStatus);
                lambdas.Add(lambda);
            }
            if ((!string.IsNullOrWhiteSpace(payExchangeStatus)) && payExchangeStatus != "all") //付汇状态
            {
                if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.UnPay)
                {
                    Expression<Func<Needs.Ccs.Services.Models.UserOrderExtends, bool>> lambda = item => item.PaidExchangeAmount == 0;
                    lambdas.Add(lambda);
                }
                else if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.Partial)
                {
                    Expression<Func<Needs.Ccs.Services.Models.UserOrderExtends, bool>> lambda = item => item.PaidExchangeAmount < item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }
                else
                {
                    Expression<Func<Needs.Ccs.Services.Models.UserOrderExtends, bool>> lambda = item => item.PaidExchangeAmount == item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }
            }

            #region 页面需要数据
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

            Func<Needs.Ccs.Services.Models.UserOrderExtends, object> convert = item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                PaySuppliersName = item.OrderConsignee.ClientSupplier.ChineseName,
                OrderStatus = item.OrderStatus.GetDescription(),
                InvoiceStatus = item.InvoiceStatus.GetDescription(),
                PayExchangeStatus = item.PayExchangeStatus.GetDescription(),
                Remittance = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2).ToString(),
                Remittanced = item.PaidExchangeAmount.ToRound(2).ToString(),
                isShowBill = item.OrderStatus > OrderStatus.Quoted && item.OrderStatus <= OrderStatus.Completed,
                isShowLog = item.OrderStatus != OrderStatus.Draft
            };
            return this.Paging1(orderlist, orderlist.Total, convert);
            #endregion
        }

        /// <summary>
        /// GET: 订单详情
        /// </summary>
        /// <returns></returns>
        public ActionResult Info(string id)
        {
            id = id.InputText();
            var model = new ConfirmViewModel();
            var confirm = WeChatPlat.Current.WebSite.MyOrders1[id];
            if (confirm == null)
            {
                return View("Error");
            }
            //产品明细
            model.Products = confirm.Items.Select(item => new ComfirmProducts
            {
                Origin = Needs.Wl.Admin.Plat.AdminPlat.Countries.Where(c => c.Code == item.Origin).FirstOrDefault() == null ? "" : Needs.Wl.Admin.Plat.AdminPlat.Countries.Where(c => c.Code == item.Origin).FirstOrDefault().Name,
                Unit = Needs.Wl.Admin.Plat.AdminPlat.Units.Where(c => c.Code == item.Unit).FirstOrDefault() == null ? "" : Needs.Wl.Admin.Plat.AdminPlat.Units.Where(c => c.Code == item.Unit).FirstOrDefault().Name,
                Batch = item.Batch,
                GrossWeight = item.GrossWeight,
                Name = item.Category?.Name ?? item.Name,
                TotalPrice = item.TotalPrice,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
            }).ToArray();
            model.Products_TotalPrice = model.Products.Sum(item => item.TotalPrice).ToString("0.00");

            //订单信息
            model.ID = confirm.ID;
            var currency = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Where(c => c.Code == confirm.Currency).FirstOrDefault();
            model.Currency = currency == null ? "" : currency.Name;
            model.CurrencyCode = confirm.Currency;
            model.IsFullVehicle = confirm.IsFullVehicle ? "是" : "否";
            model.IsAdvanceMoneny = confirm.IsLoan ? "是" : "否";
            var pack = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.Where(item => item.Code == confirm.WarpType).FirstOrDefault();
            if (pack != null)
            {
                model.WrapType = pack.Name;
            }

            model.PackNo = confirm.PackNo.ToString();
            model.Summary = confirm.Summary;
            model.CreateDate = confirm.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
            model.OrderMaker = confirm.OrderMaker;
            var fileurl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"] + @"/";
            var deliveryFile = confirm.Files.Where(item => item.FileType == FileType.DeliveryFiles).FirstOrDefault();
            if (deliveryFile != null)
            {
                model.DeliveryFiles = fileurl + deliveryFile.Url.ToUrl();
            }

            //香港交货方式信息
            var OrderConsignee = confirm.OrderConsignee;
            model.Supplier = OrderConsignee.ClientSupplier.ChineseName;
            model.HKDeliveryType = OrderConsignee.Type.GetDescription();
            model.supplierContact = OrderConsignee.Contact;
            model.supplierContactMobile = OrderConsignee.Mobile;
            model.SupplierAddress = OrderConsignee.Address;
            model.WayBillNo = OrderConsignee.WayBillNo;
            if (OrderConsignee.Type == HKDeliveryType.PickUp)
            {
                model.PickupTime = DateTime.Parse(OrderConsignee.PickUpTime.ToString()).ToString("yyyy-MM-dd");
                model.isPickUp = true;
            }

            //国内交货信息
            var OrderConsignor = confirm.OrderConsignor;
            model.SZDeliveryType = OrderConsignor.Type.GetDescription();
            model.clientContact = OrderConsignor.Contact;
            model.clientContactMobile = OrderConsignor.Mobile;
            model.clientConsigneeAddress = OrderConsignor.Address;
            model.isSZPickUp = OrderConsignor.Type == SZDeliveryType.PickUpInStore;
            model.IDNumber = OrderConsignor.IDNumber;

            //付汇供应商信息
            model.PayExchangeSupplier = confirm.PayExchangeSuppliers.Select(item => new
            {
                Name = item.ClientSupplier.ChineseName,
            }).ToArray();
            model.PIFiles = confirm.Files.Where(item => item.FileType == FileType.OriginalInvoice).Select(item => new
            {
                Name = item.Name,
                Status = item.Status.GetDescription(),
                Url = fileurl + item.Url,
            }).ToArray();
            //报关委托书
            var file = confirm.Files.Where(item => item.FileType == FileType.AgentTrustInstrument).FirstOrDefault();
            if (file != null)
            {
                model.AgentProxyURL = fileurl + file.Url;
                model.AgentProxyName = file.Name;
                model.AgentProxyStatus = file.FileStatus.GetDescription();
            }
            model.IsShowAgentProxy = (int)confirm.OrderStatus >= (int)OrderStatus.Quoted;
            //原因
            if (confirm.OrderStatus == OrderStatus.Canceled)
            {
                var log = confirm.Logs.OrderByDescending(item => item.CreateDate).FirstOrDefault();
                if (log != null)
                {
                    model.ReasonTitile = "取消原因";
                    model.Reason = log.Summary;
                }
            }
            else if (confirm.OrderStatus == OrderStatus.Returned)
            {
                var log = confirm.Logs.OrderByDescending(item => item.CreateDate).FirstOrDefault();
                if (log != null)
                {
                    model.ReasonTitile = "退回原因";
                    model.Reason = log.Summary;
                }
            }
            else if (confirm.IsHangUp)
            {
                var hangUpOrder = WeChatPlat.Current.WebSite.MyHangUpOrdersView[id];
                if (hangUpOrder != null)
                {
                    model.ReasonTitile = "挂起原因";
                    model.Reason = hangUpOrder.HangUpReason;
                }
            }
            return View(model);
        }

        /// <summary>
        /// 订单导航
        /// </summary>
        /// <returns></returns>
        public ActionResult Menu_folding()
        {
            CompanyInfo model = new CompanyInfo();
            model.CompanyName = WeChatPlat.Current.Client.Company.Name;
            return View(model);
        }

        /// <summary>
        /// 最新的日志
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult NewLog(string id)
        {
            id = id.InputText();
            var order = WeChatPlat.Current.MyOrders[id];
            if (order == null)
            {
                return View("Error");
            }
            var log = order.Traces().ToList().Select(item => new
            {
                Step = item.Step.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = item.Summary,
                isCompleted = order.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Completed,
                isDot = false
            }).ToArray();
            ViewBag.OrderId = id;
            return View(log);
        }

        /// <summary>
        /// 发票详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="v">上级页面ID</param>
        /// <returns></returns>
        public async Task<ActionResult> Invoice(string id)
        {
            id = id.InputText();
            var model = new OrdersInvoiceViewModel();
            var current = WeChatPlat.Current.Client;
            var order = WeChatPlat.Current.MyOrders[id];
            if (order == null)
            {
                return View("Error");
            }

            model.invoice = new InvoiceModel();
            var invoice = await current.InvoiceAsync();//客户的开票信息
            var invoiceConsignee = await current.InvoiceConsigneeAsyn();  //发票收件地址
            var agreement = await order.AgreementAsync();//订单的补充协议
            var orderInvoices = order.Invoices(); //订单的发票信息

            //发票信息
            model.invoice.invoiceType = agreement.InvoiceType.GetDescription();
            model.invoice.invoiceDeliveryType = invoice.DeliveryType.GetDescription();
            model.invoice.invoiceTitle = invoice.Title;
            model.invoice.invoiceTel = invoice.Tel;
            model.invoice.invoiceTaxCode = invoice.TaxCode;
            model.invoice.invoiceAddress = invoice.Address;
            model.invoice.invoiceAccount = invoice.BankAccount;
            model.invoice.invoiceBank = invoice.BankName;
            model.invoice.contact = invoiceConsignee.Name;
            model.invoice.mobile = invoiceConsignee.Mobile;
            model.invoice.contactAddress = invoiceConsignee.Address;

            //如果订单已经开票
            if (orderInvoices.RecordCount > 0)
            {
                var waybill = await order.InvoicesWaybill().FirstOrDefaultAsync(); //发票运单
                if (waybill != null)
                {
                    model.invoice.ExpressCompany = waybill.Carrier.Name;
                    model.invoice.ExpressCode = waybill.WaybillCode;
                    model.invoice.CreateDate = waybill.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
                }

                model.invoice.InvoiceNo = orderInvoices.GetOrderInvoicesNo();
            }

            model.ID = id;

            return View(model);
        }

        #endregion

        #region 草稿订单

        /// <summary>
        /// 获取草稿订单
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDraftOrders()
        {
            var invoiceStatus = Request.Form["invoiceStatus"];  //开票状态
            var payExchangeStatus = Request.Form["payExchangeStatus"];  //付汇状态
            var orderDate = Request.Form["orderDate"]; //订单日期

            var orders = WeChatPlat.Current.WebSite.MyDraftOrdersView1;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<DraftOrder, bool>> expression = item => true;

            if ((!string.IsNullOrWhiteSpace(orderDate)) && orderDate != "all")
            {
                if (orderDate == "curMonth") //当月订单
                {
                    Expression<Func<DraftOrder, bool>> lambda = item => item.CreateDate.Month == DateTime.Now.Month;
                    lambdas.Add(lambda);
                }
                else if (orderDate == "thrMonth")  //三个月的订单
                {
                    Expression<Func<DraftOrder, bool>> lambda = item => item.CreateDate >= DateTime.Now.AddMonths(-3);
                    lambdas.Add(lambda);
                }
                else if (orderDate == "curYear")  //当年订单
                {
                    Expression<Func<DraftOrder, bool>> lambda = item => item.CreateDate.Year >= DateTime.Now.Year;
                    lambdas.Add(lambda);
                }
            }
            if ((!string.IsNullOrWhiteSpace(invoiceStatus)) && invoiceStatus != "all") //开票状态
            {
                Expression<Func<DraftOrder, bool>> lambda = item => item.InvoiceStatus == (InvoiceStatus)int.Parse(invoiceStatus);
                lambdas.Add(lambda);
            }
            if ((!string.IsNullOrWhiteSpace(payExchangeStatus)) && payExchangeStatus != "all") //付汇状态
            {
                if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.UnPay)
                {
                    Expression<Func<DraftOrder, bool>> lambda = item => item.PaidExchangeAmount == 0;
                    lambdas.Add(lambda);
                }
                else if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.Partial)
                {
                    Expression<Func<DraftOrder, bool>> lambda = item => item.PaidExchangeAmount < item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }
                else
                {
                    Expression<Func<DraftOrder, bool>> lambda = item => item.PaidExchangeAmount == item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }
            }

            #region 页面需要数据
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

            Func<Needs.Ccs.Services.Models.DraftOrder, object> convert = item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                PaySuppliersName = item.OrderConsignee.ClientSupplier.ChineseName,
                OrderStatus = item.OrderStatus.GetDescription(),
                InvoiceStatus = item.InvoiceStatus.GetDescription(),
                OrderMaker = item.OrderMaker,
            };
            return this.Paging1(orderlist, orderlist.Total, convert);
            #endregion
        }

        /// <summary>
        /// POST:草稿订单删除
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteOrder(string orderID)
        {
            orderID = orderID.InputText();
            var order = WeChatPlat.Current.MyOrders[orderID];
            order.Deleted += Order_Deleted;
            order.Delete();
            return base.JsonResult(VueMsgType.success, "订单删除成功");
        }

        private void Order_Deleted(object sender, Needs.Wl.Models.Hanlders.OrderDeletedEventArgs e)
        {
            var current = Needs.Wl.User.Plat.WeChatPlat.Current;
            e.Order.Log(current.ID, "用户[" + current.RealName + "]删除了草稿订单。");
        }

        #endregion

        #region 待归类订单

        /// <summary>
        /// 获取待归类订单
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUnclassified()
        {
            var invoiceStatus = Request.Form["invoiceStatus"];  //开票状态
            var payExchangeStatus = Request.Form["payExchangeStatus"];  //付汇状态
            var orderDate = Request.Form["orderDate"]; //订单日期
            var orders = WeChatPlat.Current.WebSite.MyUnClassfiedOrdersView1;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<UnClassfiedOrder, bool>> expression = item => true;

            if ((!string.IsNullOrWhiteSpace(orderDate)) && orderDate != "all")
            {
                if (orderDate == "curMonth") //当月订单
                {
                    Expression<Func<UnClassfiedOrder, bool>> lambda = item => item.CreateDate.Month == DateTime.Now.Month;
                    lambdas.Add(lambda);
                }
                else if (orderDate == "thrMonth")  //三个月的订单
                {
                    Expression<Func<UnClassfiedOrder, bool>> lambda = item => item.CreateDate >= DateTime.Now.AddMonths(-3);
                    lambdas.Add(lambda);
                }
                else if (orderDate == "curYear")  //当年订单
                {
                    Expression<Func<UnClassfiedOrder, bool>> lambda = item => item.CreateDate.Year >= DateTime.Now.Year;
                    lambdas.Add(lambda);
                }
            }
            if ((!string.IsNullOrWhiteSpace(invoiceStatus)) && invoiceStatus != "all") //开票状态
            {
                Expression<Func<UnClassfiedOrder, bool>> lambda = item => item.InvoiceStatus == (InvoiceStatus)int.Parse(invoiceStatus);
                lambdas.Add(lambda);
            }
            if ((!string.IsNullOrWhiteSpace(payExchangeStatus)) && payExchangeStatus != "all") //付汇状态
            {
                if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.UnPay)
                {
                    Expression<Func<UnClassfiedOrder, bool>> lambda = item => item.PaidExchangeAmount == 0;
                    lambdas.Add(lambda);
                }
                else if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.Partial)
                {
                    Expression<Func<UnClassfiedOrder, bool>> lambda = item => item.PaidExchangeAmount < item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }
                else
                {
                    Expression<Func<UnClassfiedOrder, bool>> lambda = item => item.PaidExchangeAmount == item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }
            }

            #region 页面需要数据
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

            Func<Needs.Ccs.Services.Models.UnClassfiedOrder, object> convert = item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                Suppliers = item.OrderConsignee.ClientSupplier.ChineseName,
                OrderMaker = item.OrderMaker,
                OrderStatus = item.OrderStatus.GetDescription(),
            };
            return this.Paging1(orderlist, orderlist.Total, convert);
            #endregion
        }
        #endregion

        #region 待确认订单
        /// <summary>
        /// 获取待确认的订单
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPreConfirmsOrders()
        {
            var invoiceStatus = Request.Form["invoiceStatus"];  //开票状态
            var payExchangeStatus = Request.Form["payExchangeStatus"];  //付汇状态
            var orderDate = Request.Form["orderDate"]; //订单日期

            var orders = WeChatPlat.Current.WebSite.MyQuotedOrdersView1;

            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<QuotedOrder, bool>> expression = item => true;

            if ((!string.IsNullOrWhiteSpace(orderDate)) && orderDate != "all")
            {
                if (orderDate == "curMonth") //当月订单
                {
                    Expression<Func<QuotedOrder, bool>> lambda = item => item.CreateDate.Month == DateTime.Now.Month;
                    lambdas.Add(lambda);
                }
                else if (orderDate == "thrMonth")  //三个月的订单
                {
                    Expression<Func<QuotedOrder, bool>> lambda = item => item.CreateDate >= DateTime.Now.AddMonths(-3);
                    lambdas.Add(lambda);
                }
                else if (orderDate == "curYear")  //当年订单
                {
                    Expression<Func<QuotedOrder, bool>> lambda = item => item.CreateDate.Year >= DateTime.Now.Year;
                    lambdas.Add(lambda);
                }
            }
            if ((!string.IsNullOrWhiteSpace(invoiceStatus)) && invoiceStatus != "all") //开票状态
            {
                Expression<Func<QuotedOrder, bool>> lambda = item => item.InvoiceStatus == (InvoiceStatus)int.Parse(invoiceStatus);
                lambdas.Add(lambda);
            }
            if ((!string.IsNullOrWhiteSpace(payExchangeStatus)) && payExchangeStatus != "all") //付汇状态
            {
                if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.UnPay)
                {
                    Expression<Func<QuotedOrder, bool>> lambda = item => item.PaidExchangeAmount == 0;
                    lambdas.Add(lambda);
                }
                else if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.Partial)
                {
                    Expression<Func<QuotedOrder, bool>> lambda = item => item.PaidExchangeAmount < item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }
                else
                {
                    Expression<Func<QuotedOrder, bool>> lambda = item => item.PaidExchangeAmount == item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }
            }

            #region 页面需要数据
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

            Func<QuotedOrder, object> convert = item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                PaySuppliersName = item.OrderConsignee.ClientSupplier.ChineseName,
                Contact = item.OrderConsignor.Contact,
                OrderMaker = item.OrderMaker,
                OrderStatus = item.OrderStatus.GetDescription(),
            };
            return this.Paging1(orderlist, orderlist.Total, convert);
            #endregion
        }

        /// <summary>
        /// 订单确认 页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Confirm(string id)
        {
            id = id.InputText();
            var model = new ConfirmViewModel();
            model.AgentProxyFiles = new FileModel[0];
            var confirm = WeChatPlat.Current.WebSite.MyQuotedOrdersView1[id];
            if (confirm == null)
            {
                return View("Error");
            }
            var contry = Needs.Wl.Admin.Plat.AdminPlat.Countries;
            var unit = Needs.Wl.Admin.Plat.AdminPlat.Units;
            //税点
            var taxpoint = 1 + confirm.Client.Agreement.InvoiceTaxRate;
            //代理费率、最低代理费
            decimal agencyRate = confirm.AgencyFeeExchangeRate * confirm.Client.Agreement.AgencyRate;
            decimal minAgencyFee = confirm.Client.Agreement.MinAgencyFee;
            bool isAverage = confirm.DeclarePrice * agencyRate < minAgencyFee ? true : false;
            //平摊代理费、其他杂费
            decimal aveAgencyFee = confirm.AgencyFee * taxpoint / confirm.Items.Count();
            //产品明细
            model.Products = confirm.Items.Select(item => new ComfirmProducts
            {
                Origin = contry.Where(c => c.Code == item.Origin).FirstOrDefault() == null ? "" : contry.Where(c => c.Code == item.Origin).FirstOrDefault().Name,
                Unit = unit.Where(c => c.Code == item.Unit).FirstOrDefault() == null ? "" : unit.Where(c => c.Code == item.Unit).FirstOrDefault().Name,
                Batch = item.Batch,
                GrossWeight = item.GrossWeight,
                Name = item.Category?.Name ?? item.Name,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice,
                DeclareValue = item.TotalPrice * confirm.RealExchangeRate.Value,
                TraiffRate = item.ImportTax.Rate,
                Traiff = item.ImportTax.Value.Value,
                AddTaxRate = item.AddedValueTax.Rate,
                AddTax = item.AddedValueTax.Value.Value,
                AgencyFee = aveAgencyFee,
                InspectionFee = item.InspectionFee.GetValueOrDefault() * taxpoint,
                TotalTaxFee = (decimal)(item.ImportTax.Value) + (decimal)(item.AddedValueTax.Value) + aveAgencyFee + item.InspectionFee.GetValueOrDefault() * taxpoint,
                TotalDeclareValue = (decimal)(item.ImportTax.Value) + (decimal)(item.AddedValueTax.Value) + aveAgencyFee + item.InspectionFee.GetValueOrDefault() * taxpoint + item.TotalPrice * confirm.RealExchangeRate.Value,
                TotalTaxFee_Except_TraAndAdd = (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint) + item.InspectionFee.GetValueOrDefault() * taxpoint,
                TotalDeclareValue_Except_TraAndAdd = (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint) + item.InspectionFee.GetValueOrDefault() * taxpoint + item.TotalPrice * confirm.RealExchangeRate.Value,
            }).ToArray();

            //产品合计
            //ryan 20210113 外单税费小于50不收 钟苑平
            //var SumTraiff = (confirm.Type != OrderType.Outside && model.Products.Sum(item => item.Traiff) < 50) ? 0 : model.Products.Sum(item => item.Traiff);
            //var SumAddValue = (confirm.Type != OrderType.Outside && model.Products.Sum(item => item.AddTax) < 50) ? 0 : model.Products.Sum(item => item.AddTax);
            var SumTraiff = (model.Products.Sum(item => item.Traiff) < 50) ? 0 : model.Products.Sum(item => item.Traiff);
            var SumAddValue = (model.Products.Sum(item => item.AddTax) < 50) ? 0 : model.Products.Sum(item => item.AddTax);
            model.Products_Num = model.Products.Sum(item => item.Quantity.ToRound(2));
            model.Products_DeclareValue = model.Products.Sum(item => item.DeclareValue).ToString("0.00");
            model.Products_TotalPrice = model.Products.Sum(item => item.TotalPrice).ToString("0.00");
            model.Products_Traiff = SumTraiff.ToString("0.00");
            model.Products_AddTax = SumAddValue.ToString("0.00");
            model.Products_AgencyFee = model.Products.Sum(item => item.AgencyFee).ToString("0.00");
            model.Products_InspectionFee = model.Products.Sum(item => item.InspectionFee).ToString("0.00");
            model.Products_TotalTaxFee = (model.Products.Sum(item => item.TotalTaxFee_Except_TraAndAdd) + SumTraiff + SumAddValue).ToString("0.00");
            model.Products_TotalDeclareValue = (model.Products.Sum(item => item.TotalDeclareValue_Except_TraAndAdd) + SumTraiff + SumAddValue).ToString("0.00");

            //订单信息
            model.ID = confirm.ID;
            var currency = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Where(c => c.Code == confirm.Currency).FirstOrDefault();
            model.Currency = currency == null ? "" : currency.Name;
            model.CurrencyCode = confirm.Currency;
            model.IsFullVehicle = confirm.IsFullVehicle ? "是" : "否";
            model.IsAdvanceMoneny = confirm.IsLoan ? "是" : "否";
            var pack = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.Where(item => item.Code == confirm.WarpType).FirstOrDefault();
            if (pack != null)
            {
                model.WrapType = pack.Name;
            }

            model.PackNo = confirm.PackNo.ToString();
            model.Summary = confirm.Summary;
            model.CreateDate = confirm.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
            model.OrderMaker = confirm.OrderMaker;
            var fileurl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"] + @"/";
            var deliveryFile = confirm.Files.Where(item => item.FileType == FileType.DeliveryFiles).FirstOrDefault();
            if (deliveryFile != null)
            {
                model.DeliveryFiles = fileurl + deliveryFile.Url.ToUrl();
            }

            //香港交货方式信息
            var OrderConsignee = confirm.OrderConsignee;
            model.Supplier = OrderConsignee.ClientSupplier.ChineseName;
            model.HKDeliveryType = OrderConsignee.Type.GetDescription();
            model.supplierContact = OrderConsignee.Contact;
            model.supplierContactMobile = OrderConsignee.Mobile;
            model.SupplierAddress = OrderConsignee.Address;
            model.WayBillNo = OrderConsignee.WayBillNo;
            if (OrderConsignee.Type == HKDeliveryType.PickUp)
            {
                model.PickupTime = DateTime.Parse(OrderConsignee.PickUpTime.ToString()).ToString("yyyy-MM-dd");
                model.isPickUp = true;
            }

            //国内交货信息
            var OrderConsignor = confirm.OrderConsignor;
            model.SZDeliveryType = OrderConsignor.Type.GetDescription();
            model.clientContact = OrderConsignor.Contact;
            model.clientContactMobile = OrderConsignor.Mobile;
            model.clientConsigneeAddress = OrderConsignor.Address;
            model.isSZPickUp = OrderConsignor.Type == SZDeliveryType.PickUpInStore;
            model.IDNumber = OrderConsignor.IDNumber;

            //发票信息
            model.invoice = new WebWeChat.Models.InvoiceModel();
            var invoice = confirm.Client.Invoice;  //发票对象
            model.invoice.invoiceType = confirm.Client.Agreement.InvoiceType.GetDescription();
            model.invoice.invoiceDeliveryType = invoice.DeliveryType.GetDescription();
            model.invoice.invoiceTitle = invoice.Title;
            model.invoice.invoiceTel = invoice.Tel;
            model.invoice.invoiceTaxCode = invoice.TaxCode;
            model.invoice.invoiceAddress = invoice.Address;
            model.invoice.invoiceAccount = invoice.BankAccount;
            model.invoice.invoiceBank = confirm.Client.Invoice.BankName;
            model.invoice.contactName = confirm.Client.InvoiceConsignee.Name;
            model.invoice.contactMobile = confirm.Client.InvoiceConsignee.Mobile;
            model.invoice.contactTel = confirm.Client.InvoiceConsignee.Tel;
            model.invoice.contactAddress = confirm.Client.InvoiceConsignee.Address;

            //付汇供应商信息
            model.PayExchangeSupplier = confirm.PayExchangeSuppliers.Select(item => new
            {
                Name = item.ClientSupplier.ChineseName,
            }).ToArray();
            model.PIFiles = confirm.Files.Where(item => item.FileType == FileType.OriginalInvoice).Select(item => new
            {
                Name = item.Name,
                Status = item.Status.GetDescription(),
                Url = fileurl + item.Url.ToUrl()
            }).ToArray();
            var AgentProxyURL = confirm.Files.Where(item => item.FileType == FileType.AgentTrustInstrument).FirstOrDefault();
            if (AgentProxyURL != null)
            {
                //报关委托书
                model.AgentProxyURL = fileurl + AgentProxyURL.Url.ToUrl();
                model.AgentProxyName = AgentProxyURL.Name;
                model.AgentProxyStatus = AgentProxyURL.FileStatus.GetDescription();
            }
            return View(model);
        }

        /// <summary>
        /// POST:订单确认
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckPreConfirm(string orderID, FileModel[] file)
        {
            try
            {
                orderID = orderID.InputText();
                var order = Needs.Wl.User.Plat.WeChatPlat.Current.WebSite.MyQuotedOrdersView1[orderID];//订单明细
                if (order.OrderStatus != OrderStatus.Quoted)
                {
                    return View("Error");
                }
                var user = WeChatPlat.Current.Client.Users()[WeChatPlat.Current.ID];
                order.SetUser(user.ToCssUser());
                order.QuoteConfirm();

                //代理报关委托书保存
                if (file != null)
                {
                    var agentfile = order.Files.Where(item => item.FileType == FileType.AgentTrustInstrument).FirstOrDefault();
                    if (agentfile == null)
                    {
                        agentfile = new Needs.Ccs.Services.Models.OrderFile
                        {
                            OrderID = order.ID,
                            User = user.ToCssUser(),
                            Name = file[0].FileName,
                            FileType = FileType.AgentTrustInstrument,
                            FileFormat = file[0].FileFormat,
                            Url = file[0].URL,
                            FileStatus = OrderFileStatus.Auditing
                        };
                        agentfile.Enter();
                    }
                    else if (agentfile.FileStatus != OrderFileStatus.Audited)
                    {
                        agentfile.User = user.ToCssUser();
                        agentfile.Name = file[0].FileName;
                        agentfile.FileFormat = file[0].FileFormat;
                        agentfile.Url = file[0].URL;
                        agentfile.FileStatus = OrderFileStatus.Auditing;
                        agentfile.Enter();
                    }
                }
                return base.JsonResult(VueMsgType.success, "订单确认成功");
            }
            catch (Exception e)
            {
                return base.JsonResult(VueMsgType.error, e.Message);
            }
        }

        /// <summary>
        /// 待确认订单取消
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public JsonResult CancelConfirm(string orderID, string reason)
        {
            orderID = orderID.InputText();
            var order = WeChatPlat.Current.WebSite.MyQuotedOrdersView1[orderID];
            if (order == null)
            {
                return base.JsonResult(VueMsgType.error, "该订单不存在");
            }
            if (order.OrderStatus == OrderStatus.Canceled)
            {
                return base.JsonResult(VueMsgType.error, "该订单已取消，请勿重复操作");
            }
            order.SetUser(WeChatPlat.Current.Client.Users()[WeChatPlat.Current.ID].ToCssUser());
            order.CanceledSummary = reason;
            order.Cancel();
            return base.JsonResult(VueMsgType.success, "订单取消成功");
        }
        #endregion

        #region 已退回订单

        /// <summary>
        /// 获取已退回订单
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRejectedOrders()
        {
            var invoiceStatus = Request.Form["invoiceStatus"];  //开票状态
            var payExchangeStatus = Request.Form["payExchangeStatus"];  //付汇状态
            var orderDate = Request.Form["orderDate"]; //订单日期

            var orders = WeChatPlat.Current.WebSite.MyReturnedOrdersView1;

            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<ReturnedOrder, bool>> expression = item => true;

            if ((!string.IsNullOrWhiteSpace(orderDate)) && orderDate != "all")
            {
                if (orderDate == "curMonth") //当月订单
                {
                    Expression<Func<ReturnedOrder, bool>> lambda = item => item.CreateDate.Month == DateTime.Now.Month;
                    lambdas.Add(lambda);
                }
                else if (orderDate == "thrMonth")  //三个月的订单
                {
                    Expression<Func<ReturnedOrder, bool>> lambda = item => item.CreateDate >= DateTime.Now.AddMonths(-3);
                    lambdas.Add(lambda);
                }
                else if (orderDate == "curYear")  //当年订单
                {
                    Expression<Func<ReturnedOrder, bool>> lambda = item => item.CreateDate.Year >= DateTime.Now.Year;
                    lambdas.Add(lambda);
                }
            }
            if ((!string.IsNullOrWhiteSpace(invoiceStatus)) && invoiceStatus != "all") //开票状态
            {
                Expression<Func<ReturnedOrder, bool>> lambda = item => item.InvoiceStatus == (InvoiceStatus)int.Parse(invoiceStatus);
                lambdas.Add(lambda);
            }
            if ((!string.IsNullOrWhiteSpace(payExchangeStatus)) && payExchangeStatus != "all") //付汇状态
            {
                if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.UnPay)
                {
                    Expression<Func<ReturnedOrder, bool>> lambda = item => item.PaidExchangeAmount == 0;
                    lambdas.Add(lambda);
                }
                else if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.Partial)
                {
                    Expression<Func<ReturnedOrder, bool>> lambda = item => item.PaidExchangeAmount < item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }
                else
                {
                    Expression<Func<ReturnedOrder, bool>> lambda = item => item.PaidExchangeAmount == item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }
            }

            #region 页面需要数据
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

            Func<ReturnedOrder, object> convert = item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                Suppliers = item.OrderConsignee.ClientSupplier.ChineseName,
                OrderStatus = item.OrderStatus.GetDescription(),
                Summary = item.ReturnedSummary,
            };
            return this.Paging1(orderlist, orderlist.Total, convert);
            #endregion
        }

        /// <summary>
        /// 已退回订单取消
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public JsonResult CancelRejected(string orderID, string reason)
        {
            orderID = orderID.InputText();
            var order = WeChatPlat.Current.WebSite.MyReturnedOrdersView1[orderID];
            if (order == null)
            {
                return base.JsonResult(VueMsgType.error, "该订单不存在");
            }
            if (order.OrderStatus == OrderStatus.Canceled)
            {
                return base.JsonResult(VueMsgType.error, "该订单已取消，请勿重复操作");
            }
            order.SetUser(WeChatPlat.Current.Client.Users()[WeChatPlat.Current.ID].ToCssUser());
            order.CanceledSummary = reason;
            order.Cancel();
            return base.JsonResult(VueMsgType.success, "订单取消成功");
        }

        #endregion

        #region 对账单

        /// <summary>
        /// 对账单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public ActionResult OrderBills(string id)
        {
            id = id.InputText();
            var bill = WeChatPlat.Current.WebSite.MyOrderBills[id];

            if (bill == null)
            {
                return View("Error");
            }
            else
            {
                OrderBillsViewModel model = new OrderBillsViewModel();
                //税点
                var taxpoint = 1 + bill.Agreement.InvoiceTaxRate;
                //代理费率、最低代理费
                decimal agencyRate = bill.AgencyFeeExchangeRate * bill.Agreement.AgencyRate;
                decimal minAgencyFee = bill.Agreement.MinAgencyFee;
                bool isAverage = bill.DeclarePrice * agencyRate < minAgencyFee ? true : false;
                //平摊代理费、其他杂费
                decimal aveAgencyFee = bill.AgencyFee * taxpoint / bill.Items.Count();
                decimal aveOtherFee = bill.OtherFee * taxpoint / bill.Items.Count();

                var purchaser = PurchaserContext.Current;


                //基本信息
                var client = bill.Client;
                model.User_name = client.Company.Name;
                model.User_tel = client.Company.Contact.Tel;
                model.AgentName = purchaser.CompanyName;
                model.AgentAddress = purchaser.Address;
                model.AgentTel = purchaser.Tel;
                model.AgentFax = purchaser.UseOrgPersonTel;
                model.Account = purchaser.BankName;
                model.AccountID = purchaser.AccountId;
                model.RealExchangeRate = bill.RealExchangeRate;
                model.CustomsExchangeRate = bill.CustomsExchangeRate;
                model.DueDate = bill.GetDueDate().ToString("yyyy年MM月dd日");
                model.OrderID = bill.ID;
                model.CreateDate = bill.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
                model.IsLoan = bill.IsLoan;
                model.Currency = bill.Currency;
                model.ContractNO = bill.ContractNO;

                //产品明细
                var list = bill.Items.Select(item => new
                {
                    Name = item.Category.Name,
                    Model = item.Model,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DeclarePrice = item.TotalPrice,
                    TariffRate = item.ImportTax.Rate,
                    TotalCNYPrice = (item.TotalPrice * bill.ProductFeeExchangeRate),
                    Traiff = item.ImportTax.Value,
                    AddedValueTax = item.AddedValueTax.Value,
                    AgencyFee = isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint,
                    IncidentalFee = bill.InspFees.Where(fee => fee.OrderItemID == item.ID)
                                              .Select(fee => fee.Count * fee.UnitPrice * fee.Rate)
                                              .FirstOrDefault() * taxpoint + aveOtherFee
                }).ToArray();
                //总和
                //ryan 20210113 外单税费小于50不收 钟苑平
                //var SumTraiff = (bill.OrderType != OrderType.Outside && list.Sum(item => item.Traiff.Value) < 50) ? 0 : list.Sum(item => item.Traiff.Value);
                //var SumAddedValue = (bill.OrderType != OrderType.Outside && list.Sum(item => item.AddedValueTax.Value) < 50) ? 0 : list.Sum(item => item.AddedValueTax.Value);
                var SumTraiff = (list.Sum(item => item.Traiff.Value) < 50) ? 0 : list.Sum(item => item.Traiff.Value);
                var SumAddedValue = (list.Sum(item => item.AddedValueTax.Value) < 50) ? 0 : list.Sum(item => item.AddedValueTax.Value);
                model.SumQuantity = list.Sum(item => item.Quantity).ToString("0");
                model.SumDeclarePrice = list.Sum(item => item.DeclarePrice).ToString("0.00");
                model.SumTotalCNYPrice = list.Sum(item => item.TotalCNYPrice).ToString("0.00");
                model.SumTraiff = SumTraiff.ToString("0.00");
                model.SumAddedValueTax = SumAddedValue.ToString("0.00");
                model.SumAgencyFee = list.Sum(item => item.AgencyFee).ToString("0.00");
                model.SumIncidentalFee = list.Sum(item => item.IncidentalFee).ToString("0.00");
                model.SumTotalTax = (list.Sum(item => item.AgencyFee + item.IncidentalFee) + SumTraiff + SumAddedValue).ToString("0.00");
                model.SumTotalDeclarePrice = (list.Sum(item => item.TotalCNYPrice + item.AgencyFee + item.IncidentalFee) + SumTraiff + SumAddedValue).ToString("0.00");
                model.Productlist = list.Select(item => new
                {
                    Name = item.Name,
                    Model = item.Model,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TariffRate = item.TariffRate,
                    DeclarePrice = item.DeclarePrice.ToString("0.00"),
                    TotalCNYPrice = item.TotalCNYPrice.ToString("0.00"),
                    Traiff = item.Traiff.Value.ToString("0.00"),
                    AddedValueTax = item.AddedValueTax.Value.ToString("0.00"),
                    AgencyFee = item.AgencyFee.ToString("0.00"),
                    IncidentalFee = item.IncidentalFee.ToString("0.00"),
                    TotalTax = (SumTraiff + SumAddedValue + item.AgencyFee + item.IncidentalFee).ToString("0.00"),
                    TotalDeclarePrice = (item.TotalCNYPrice + SumTraiff + SumAddedValue + item.AgencyFee + item.IncidentalFee).ToString("0.00")
                }).ToArray();
                var order = WeChatPlat.Current.WebSite.MyOrders[id];
                if (order != null)
                {
                    var file = order.Files.Where(item => item.FileType == FileType.OrderBill).FirstOrDefault();
                    if (file != null)
                    {
                        var fileurl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"] + @"/";
                        model.BillFileUrl = fileurl + file.Url;
                        model.BillFileStatus = file.FileStatus == OrderFileStatus.Audited;
                        model.BillFileName = file.Name;
                    }
                }
                return View(model);
            }
        }

        /// <summary>
        /// 删除对账单
        /// </summary>
        /// <returns></returns>
        public JsonResult DelBillFile(string id)
        {
            try
            {
                var order = WeChatPlat.Current.WebSite.MyOrders1[id];
                if (order == null)
                {
                    return base.JsonResult(VueMsgType.error, "该订单不存在！删除失败！");
                }
                var auditedFile = order.Files.Where(item => item.FileType == FileType.OrderBill && item.FileStatus == OrderFileStatus.Audited).FirstOrDefault();
                if (auditedFile != null)
                {
                    return base.JsonResult(VueMsgType.error, "原对账单已审核通过！删除失败！");
                }
                var auditingFile = order.Files.Where(item => item.FileType == FileType.OrderBill && item.FileStatus == OrderFileStatus.Auditing).FirstOrDefault();
                if (auditingFile == null)
                {
                    return base.JsonResult(VueMsgType.error, "对账单不存在！删除失败！");

                }
                auditingFile.Delete();
                return base.JsonResult(VueMsgType.success, "删除成功！");
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, "删除失败！");
            }
        }

        #endregion  
    }
}