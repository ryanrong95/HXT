using Needs.Utils.Serializers;
using NtErp.Wss.Oss.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var view = new NtErp.Wss.Oss.Services.Dynamics.OrdersDynamic())
            {
                foreach (var item in view.ToArray())
                {
                    Console.WriteLine(((object)item).Json());
                }
            }

            using (var view = new OrderAlls())
            {
                var linq = view;

                foreach (var item in linq.ToArray())
                {
                    Console.WriteLine(item.ID);
                }
            }

            var company = new NtErp.Wss.Oss.Services.Models.Company
            {
                Name = "公司名称1",
                Address = "北京中关村",
                Code = "Company Code 001",
            };
            var contact = new NtErp.Wss.Oss.Services.Models.Contact
            {
                Name = "xiaoxiao",
                //Company = company,
                Email = "xiaoxiao@gamil.com",
                Mobile = "18000000000",
                Tel = ""
            };

            #region items

            var items = new List<NtErp.Wss.Oss.Services.Models.OrderItem>();

            var item1 = new NtErp.Wss.Oss.Services.Models.OrderItem
            {
                CustomerCode = "D001",
                Origin = "产地",
                Quantity = 10,
                UnitPrice = 2.1M,
                Supplier = new NtErp.Wss.Oss.Services.Models.Company
                {
                    Type = NtErp.Wss.Oss.Services.CompanyType.Supplier,
                    Name = "B1B",
                },
                Weight = 0,
                From = NtErp.Wss.Oss.Services.OrderItemFrom.Cart,
                Status = NtErp.Wss.Oss.Services.OrderItemStatus.Normal,
                ID = "service" + DateTime.Now.Ticks

            };
            item1.Product = new NtErp.Wss.Oss.Services.Models.StandardProduct()
            {
                ID = Guid.NewGuid().ToString("N"),
                SignCode = "product001",
                Manufacturer = new NtErp.Wss.Oss.Services.Models.Company
                {
                    Type = NtErp.Wss.Oss.Services.CompanyType.Manufactruer,
                    Name = "MF001"
                },
                Name = "产品001",
                Packaging = "包装",
                PackageCase = "封装",
                Batch = "2018-10",
                DateCode = "222",
                Description = "产品描述"
            };
            items.Add(item1);

            #endregion


            var order = new NtErp.Wss.Oss.Services.Models.Order
            {
                Type = NtErp.Wss.Oss.Services.OrderType.Normal,
                Summary = "备注一下，当天发",
                Client = Needs.Underly.FkoFactory<NtErp.Wss.Oss.Services.Models.ClientTop>.Create("US000013365"),
                Invoice = new NtErp.Wss.Oss.Services.Models.Invoice
                {
                    Required = true,
                    Type = NtErp.Wss.Oss.Services.Models.InvoiceType.VAT,
                    Address = "luoyangcheng",
                    Postzip = "100000",
                    Bank = "银行",
                    Account = "银行账号",
                    BankAddress = "开户行地址",
                    SwiftCode = "银行编码",

                    Company = company,
                    Contact = contact
                },
                TransportTerm = new NtErp.Wss.Oss.Services.Models.TransportTerm
                {
                    Carrier = "ss",
                    FreightMode = NtErp.Wss.Oss.Services.FreightMode.Prepaid,
                    TransportMode = NtErp.Wss.Oss.Services.TransportMode.Ups,
                    PriceClause = NtErp.Wss.Oss.Services.PriceClause.CIF,
                },
                Beneficiary = new NtErp.Wss.Oss.Services.Models.Beneficiary
                {
                    Bank = "中国银行",
                    Methord = NtErp.Wss.Oss.Services.Models.Methord.Remittance,
                    Currency = Needs.Underly.Currency.CNY,
                    Account = "6211111111",
                    Address = "北京分行",
                    SwiftCode = "swiftcode001",

                    Contact = contact
                },
                Consignee = new NtErp.Wss.Oss.Services.Models.Party
                {
                    Address = "收货地址北京市海淀区中关村",
                    Postzip = "100000",
                    District = Needs.Underly.District.CN,

                    Company = company,
                    Contact = contact
                },
                Deliverer = new NtErp.Wss.Oss.Services.Models.Party
                {
                    Address = "发货地址中国香港",
                    Postzip = "999077",

                    Company = company,
                    Contact = contact
                },
                Items = new NtErp.Wss.Oss.Services.Models.OrderItems(items)
            };

            Console.WriteLine(item1 == order.Items[0]);
            Console.WriteLine();

            order.Premiums.Add(new NtErp.Wss.Oss.Services.Models.Premium
            {
                Count = 1,
                CreateDate = DateTime.Now,
                ID = null,
                Name = "测试运费",
                Price = 15,
                Summary = "",
            });



            var entity = order;

            string json1 = entity.Json(Formatting.Indented);

            Console.WriteLine(json1);
            var djson = json1.JsonTo<NtErp.Wss.Oss.Services.Models.Order>();
            string json2 = djson.Json(Formatting.Indented);

            Console.WriteLine(json2);



            //string json1 = order.Json(Formatting.Indented);


            //Console.WriteLine(json1);

            //var djson = json1.JsonTo<NtErp.Wss.Oss.Services.Models.Order>();

            //string json2 = djson.Json(Formatting.Indented);

            //Console.WriteLine(json2);

            order.Enter();

        }
    }
}
