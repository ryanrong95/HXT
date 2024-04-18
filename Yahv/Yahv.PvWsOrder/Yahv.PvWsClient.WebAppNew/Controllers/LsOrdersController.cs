using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Yahv.PvWsClient.WebAppNew.Controllers.Attribute;
using Yahv.PvWsClient.WebAppNew.Models;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Services.Models.LsOrder;
using Yahv.Underly;

namespace Yahv.PvWsClient.WebAppNew.Controllers
{
    public class LsOrdersController : UserController
    {
        #region 租赁订单列表
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult List()
        {
            return View();
        }

        /// <summary>
        /// 获取页面数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetList()
        {
            var paramsList = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                OrderID = Request.Form["OrderID"],
            };
            var lsOrders = Client.Current.MyLsOrdersList.Where(item => true);
            if (!string.IsNullOrWhiteSpace(paramsList.OrderID))
            {
                lsOrders = lsOrders.Where(item => item.ID.Contains(paramsList.OrderID.Trim()));
            }
            lsOrders = lsOrders.OrderByDescending(item => item.CreateDate);
            var total = lsOrders.Count();
            //数据结果
            var data = lsOrders.Skip((paramsList.page - 1) * paramsList.rows).Take(paramsList.rows).ToArray().Select(item => new
            {
                item.ID,
                StartDate = item.StartDate.ToString("yyyy-MM-dd"),
                EndDate = item.EndDate.ToString("yyyy-MM-dd"),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                item.SpecID,
                item.Quantity,
                item.ItemID,
                Month = item.Months.ToString() + "个月",
                UnitPrice = item.UnitPrice.ToString("0.00") + item.Currency.GetDescription(),
                TotalPrice = item.TotalPrice.ToString("0.00") + item.Currency.GetDescription(),
                Status = item.Status.GetDescription(),
                IsCancel = item.Status == LsOrderStatus.Unpaid,
                Inherit = item.Status == LsOrderStatus.Expired && !item.InheritStatus,
                IsUpload = item.Status == LsOrderStatus.Unpaid,
            });

            return this.Paging(data, total);
        }
        #endregion

        #region 租赁订单详情
        /// <summary>
        /// 详情页面初始化
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult Detail()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];
            var list = Alls.Current.LsOrderItems.SearchByOrderID(id).ToArray().Select(item => new
            {
                item.Product.SpecID,
                item.Product.Load,
                item.Product.Volume,
                item.Quantity,
                item.UnitPrice,
                Month = item.Lease.Month.ToString() + "个月",
                TotalPrice = item.Quantity * item.UnitPrice * item.Lease.Month,
            }).ToList();

            var model = new
            {
                LsOrderID = id,
                List = list,
                TotalPrice = list.Sum(item => item.TotalPrice),
            };

            return View(model);
        }
        #endregion

        #region 租赁订单新增
        /// <summary>
        /// 初始化数据加载
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult Add()
        {
            var model = new LsOrderModel
            {
                Products = Alls.Current.LsProducts.ToArray().Select(item => new LsProductModel
                {
                    ID = item.ID,
                    SpecID = item.SpecID,
                    Load = item.Load,
                    Volume = item.Volume,
                    LsPrices = item.LsProductPrice.OrderBy(a => a.Month).Select(a => new LsProductPrice { Price = a.Price, Month = a.Month, }).ToArray(),
                    Quantity = item.Quantity,
                    Amount = 0,
                    Month = 0,
                    UnitPrice = 0,
                    TotalPrice = 0,
                }).ToArray(),
                Items = new LsOrderItemModel[0]
            };

            return View(model);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult LsOrderSubmit(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<LsOrderModel>(data);
                var current = Client.Current;
                var clientInfo = current.MyClients;

                #region 页面数据转化成租赁订单数据

                var orderItems = model.Items.Select(item => new LsOrderItem
                {
                    Quantity = item.Quantity,
                    Currency = Currency.CNY,
                    UnitPrice = item.UnitPrice,
                    ProductID = item.ProductID,
                    Lease = new OrderItemsLease
                    {
                        StartDate = DateTime.Parse(item.StartDate),
                        EndDate = DateTime.Parse(item.StartDate).AddMonths(item.Month),
                        Status = LsStatus.Subsist,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Summary = item.Summary,
                    },
                }).ToArray();

                DateTime orderStartDate = orderItems.Select(t => t.Lease.StartDate).Min();
                DateTime orderEndDate = orderItems.Select(t => t.Lease.EndDate).Max();

                //租赁订单初始化
                var lsOrder = new LsOrderExtends
                {
                    Type = LsOrderType.Lease,
                    Source = LsOrderSource.WarehouseServicing,
                    ClientID = clientInfo.ID,
                    PayeeID = PvWsOrder.Services.PvClientConfig.CompanyID,
                    Currency = Currency.CNY,
                    InvoiceID = current.MyInvoice.SingleOrDefault()?.ID,
                    IsInvoiced = false,
                    InheritStatus = false,
                    Creator = current.ID,
                    OrderItems = orderItems,
                    StartDate = orderStartDate,
                    EndDate = orderEndDate,
                };
                //租赁订单项

                lsOrder.Enter();
                #endregion

                return JsonResult(VueMsgType.success, "新增成功", lsOrder.ID);
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
            }
        }
        #endregion

        #region 续租订单新增
        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult Renewal()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];

            var product = Client.Current.MyLsOrdersList.GetLsOrderDetail(id).Select(item => new
            {
                item.OrderID,
                item.ProductID,
                item.Product.SpecID,
                item.Product.Load,
                item.Product.Volume,
                LsPrices = item.Product.LsProductPrice.OrderBy(a => a.Month).Select(a => new LsProductPrice
                {
                    Price = a.Price,
                    Month = a.Month,
                }).ToArray(),
                item.Product.Quantity,
                StartDate = item.Lease.EndDate.AddDays(1).ToString("yyyy-MM-dd"),
                item.UnitPrice,
                Amount = item.Quantity,
                item.Lease.Month,
                TotalPrice = item.Quantity * item.UnitPrice * item.Lease.Month,
            }).ToList();
            var model = new
            {
                Items = product,
                TotalAmount = product.Sum(item => item.TotalPrice),
            };
            return View(model);
        }

        [HttpPost]
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult RenewalSubmit(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<LsOrderModel>(data);
                var current = Client.Current;
                var clientInfo = current.MyClients;

                var orderItems = model.Items.Select(item => new LsOrderItem
                {
                    Quantity = item.Amount,
                    Currency = Currency.CNY,
                    UnitPrice = item.UnitPrice,
                    ProductID = item.ProductID,
                    Lease = new OrderItemsLease
                    {
                        StartDate = DateTime.Parse(item.StartDate),
                        EndDate = DateTime.Parse(item.StartDate).AddMonths(item.Month),
                        Status = LsStatus.Subsist,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Summary = item.Summary,
                    },
                }).ToArray();

                DateTime orderStartDate = orderItems.Select(t => t.Lease.StartDate).Min();
                DateTime orderEndDate = orderItems.Select(t => t.Lease.EndDate).Max();

                var lsOrder = new LsOrderExtends
                {
                    FatherID = model.Items.FirstOrDefault()?.OrderID,
                    Type = LsOrderType.Lease,
                    Source = LsOrderSource.WarehouseServicing,
                    ClientID = clientInfo.ID,
                    PayeeID = PvWsOrder.Services.PvClientConfig.CompanyID,
                    Currency = Currency.CNY,
                    InvoiceID = current.MyInvoice.SingleOrDefault()?.ID,
                    IsInvoiced = false,
                    InheritStatus = false,
                    Creator = current.ID,
                    OrderItems = orderItems,
                    StartDate = orderStartDate,
                    EndDate = orderEndDate,
                };

                //租赁订单项
                lsOrder.Enter();

                return JsonResult(VueMsgType.success, "新增成功", lsOrder.ID);
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
            }
        }
        #endregion

        #region 租赁订单取消

        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult LsOrderCancel(string ItemID)
        {
            try
            {
                Client.Current.MyLsOrdersList.Cancel(ItemID);

                return JsonResult(VueMsgType.success, "租仓订单取消成功");
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, ex.Message);
            }
        }
        #endregion
    }
}