using MvcApp.Buyer.Services.Extends;
using MvcApp.Buyer.Services.View;
using NtErp.Wss.Oss.Services;
using NtErp.Wss.Oss.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{

    /*
    Request["transportMode"];index
    Request["payMode"];   Wallet||Credit
    Request["InvoiceRequired"];  bool
    Request["taxID"];		index
    Request["summary"];	string 
    Request["district"];	index
    Request["address1"]; index
    Request["address2"]; index
    */

    public class OrdersController : UserController
    {
        // GET: Orders
        public ActionResult Index()
        {
            //订单号 产品型号    客户编号 数量  单价 总金额 订单时间 订单状态    操作

            using (var view = new NtErp.Wss.Oss.Services.Views.OrderAlls())
            {
                string uid = Buyer.Services.OldSso.Current.ID;
                int pageindex = 1;
                if (!string.IsNullOrWhiteSpace(Request["pageindex"]))
                {
                    pageindex = int.Parse(Request["pageindex"]);
                }
                int pagesize = 10;
                if (!string.IsNullOrWhiteSpace(Request["pagesize"]))
                {
                    pagesize = int.Parse(Request["pagesize"]);
                }
                string oid = Request["id"];
                string starttime = Request["starttime"];
                string endtime = Request["endtime"];
                string status = Request["status"];

                var rzlt = view.Where(item => item.Client.ID == uid);
                if (!string.IsNullOrWhiteSpace(oid))
                {
                    rzlt = rzlt.Where(item => item.ID.Contains(oid));
                }
                DateTime stime, etime;
                if (DateTime.TryParse(starttime, out stime))
                {
                    rzlt = rzlt.Where(item => item.CreateDate >= stime);
                }

                if (DateTime.TryParse(endtime, out etime))
                {
                    rzlt = rzlt.Where(item => item.CreateDate < etime.AddDays(1));
                }
                if (stime != null && etime != null && stime > etime)
                {
                    eJson(new
                    {
                        status = 400,
                        message = "开始时间不能大于结束时间"
                    });
                }
                OrderStatus stu;
                if (Enum.TryParse(status, out stu))
                {
                    if (stu > 0)
                    {
                        rzlt = rzlt.Where(item => item.Status == stu);
                    }
                }

                var k = rzlt.OrderByDescending(item => item.CreateDate).ToArray();

                var source = rzlt.OrderByDescending(item => item.CreateDate).Select(order => new
                {
                    order.ID,
                    order.Beneficiary,
                    Items = order.Items,
                    order.Total,
                    order.CreateDate,
                    order.Status,
                    order.SendRate,
                    order.Paid
                });
                return Json(base.Paging(source, pageindex, pagesize), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 订单-创建
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Replace()
        {
            string address1 = Request["address1"];
            string address2 = Request["address2"];

            MvcApp.Buyer.Services.TransportTerm transportTerm;

            if (!Enum.TryParse(Request["transportMode"], out transportTerm))
            {
                throw new Exception();
            }

            // 支付方式
            UserAccountType type;
            if (!Enum.TryParse(Request["type"], out type))
            {
                throw new Exception();
            }

            string payMode = Request["payMode"];
            bool invoiceRequired = bool.Parse(Request["InvoiceRequired"]);
            string taxID = Request["InvoiceID"];
            string summary = Request["summary"];


            Needs.Underly.District district;
            if (!Enum.TryParse(Request["district"], out district))
            {
                throw new Exception();
            }
            Needs.Underly.Currency currency;
            if (!Enum.TryParse(Request["currency"], out currency))
            {
                throw new Exception();
            }

            using (Needs.Linq.LinqContext.Current)
            {
                var consigneesView = new ConsigneesView();
                var consignee1 = consigneesView.SingleOrDefault(item => item.ID == address1);
                if (consignee1 == null)
                {
                    return Json(new
                    {
                        status = 400,
                        message = "10032", // 请填写完整收货地址
                    });
                }
                var consignee2 = consigneesView.SingleOrDefault(item => item.ID == address2);
                if (consignee2 == null)
                {
                    return Json(new
                    {
                        status = 400,
                        message = "10035", // 请填写完整发票收票地址
                    });
                }
                var invoice = new InvoicesView().SingleOrDefault(item => item.ID == taxID);
                if (invoiceRequired && invoice == null)
                {
                    return Json(new
                    {
                        status = 400,
                        message = "10033", // 请填写开票信息
                    });
                }
                var newInvoice = invoiceRequired ? invoice.ToOss(consignee2) : new NtErp.Wss.Oss.Services.Models.Invoice
                {
                    Required = invoiceRequired,
                    Type = InvoiceType.None,
                };


                var bene = Beneficiaries.Current.Get(type, currency);

                List<string> list = Session[Buyer.Services.OldSso.session_cartid] as List<string>;
                if (list.Count == 0)
                {
                    return Json(new
                    {
                        status = 400,
                        message = "10036", // 请重新回到购物车添加产品项
                    });
                }
                var cartsView = new CartsView();
                var carts = cartsView.Where(item => list.Contains(item.ServiceOutputID)).ToArray();

                List<OrderItem> orderItemlist = new List<OrderItem>();

                foreach (var cart in carts)
                {
                    orderItemlist.Add(new OrderItem
                    {
                        Quantity = cart.Quantity,
                        CustomerCode = cart.CustomerCode,
                        From = OrderItemFrom.Cart,
                        Origin = "",
                        UnitPrice = cart.GetPrice(cart.Quantity, district, bene.Currency),
                        Product = cart.ToProduct(),
                        Status = OrderItemStatus.Normal,
                        Leadtime = cart.GetLeadtime(),
                        Supplier = new Company
                        {
                            Name = cart.ToSupplier(),
                            Type = CompanyType.Supplier,
                            Address = "",
                            Code = ""
                        },
                        Weight = null,
                        ServiceID = cart.ServiceOutputID,
                    });
                }


                var order = new Order
                {
                    Beneficiary = bene,
                    Client = Buyer.Services.OldSso.Current.ToOss(),
                    Consignee = consignee1.ToOssParty(district),
                    CreateDate = DateTime.Now,
                    Deliverer = new Party
                    {
                        Address = bene.Company?.Address,
                        Company = new Company
                        {
                            Type = NtErp.Wss.Oss.Services.CompanyType.Plot,
                            Address = bene.Company?.Address,
                            Name = bene.Company?.Name,
                            Code = ""
                        },
                        Contact = new Contact
                        {
                            Company = new Company
                            {
                                Type = NtErp.Wss.Oss.Services.CompanyType.Plot,
                                Address = bene.Company?.Address,
                                Name = bene.Company?.Name,
                                Code = ""
                            },
                            Name = "VirtualDeliverer",
                            Email = "",
                            Mobile = "",
                            Tel = ""
                        },
                        District = district == Needs.Underly.District.CN ? Needs.Underly.District.CN : Needs.Underly.District.HK,
                        Postzip = ""
                    },
                    Invoice = newInvoice,
                    Items = new OrderItems(orderItemlist),
                    Premiums = null,
                    SendRate = 0m,
                    Status = OrderStatus.Paying,
                    Summary = summary,

                    TransportTerm = new TransportTerm
                    {
                        Address = "",
                        Carrier = transportTerm.ToNew().ToString(),
                        FreightMode = FreightMode.Collect,
                        PriceClause = PriceClause.FOB,
                        TransportMode = transportTerm.ToNew(),
                    },
                    Type = OrderType.Normal,
                    UpdateDate = DateTime.Now,
                };

                order.Enter();
                // 清空session 
                Session[Buyer.Services.OldSso.session_cartid] = null;
                // 清空购物车
                foreach (var item in carts)
                {
                    item.Remove();
                }

                if (type == UserAccountType.Cash || type == UserAccountType.Credit)
                {
                    //再商议，漏洞

                    order.Pay(type == UserAccountType.Cash, type == UserAccountType.Credit);
                }

                return Json(new
                {
                    status = 200,
                    message = "10031",
                });
            }
        }
        /// <summary>
        /// 订单-结算产品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Products()
        {
            Needs.Underly.District district;
            if (!Enum.TryParse(Request["district"], true, out district))
            {
                throw new Exception();
            }
            var bene = Beneficiaries.Current[district];

            List<string> list = Session[Buyer.Services.OldSso.session_cartid] as List<string> ?? new List<string>();
            var cartsView = new CartsView();
            var carts = cartsView.Where(item => list.Contains(item.ServiceOutputID)).ToArray();

            var rzlt = carts.Select(cart =>
            {
                var from1 = cart.GetFrom() == "ForCart" ? OrderItemFrom.VirtualCart : OrderItemFrom.Cart;
                return new OrderItem
                {
                    Quantity = cart.Quantity,
                    CustomerCode = cart.CustomerCode,
                    From = from1,
                    Origin = "",
                    UnitPrice = from1 == OrderItemFrom.VirtualCart ? cart.GetVirtualPrice() : cart.GetPrice(cart.Quantity, district, bene.Currency),
                    Product = cart.ToProduct(),
                    Status = OrderItemStatus.Normal,
                    Supplier = new Company
                    {
                        Name = cart.ToSupplier(),
                        Type = CompanyType.Supplier,
                        Address = "",
                        Code = ""
                    },
                    Weight = null,
                    ServiceID = cart.ServiceOutputID,
                };
            });

            return Json(rzlt, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetBeneficiaries()
        {
            var bene = Beneficiaries.Current.Get(UserAccountType.TT, Needs.Underly.Currency.USD);
            Needs.Underly.District district;
            if (Enum.TryParse(Request["district"], true, out district))
            {
                //throw new Exception();
                bene = Beneficiaries.Current[district];
            }

            return Json(bene, JsonRequestBehavior.AllowGet);
        }
    }
}