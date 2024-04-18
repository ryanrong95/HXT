using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat.Views
{
    public class ClientOrderDataViewNew : View<ClientOrderDataViewNewModel, ScCustomsReponsitory>
    {

        public ClientOrderDataViewNew()
        {

        }

        protected override IQueryable<ClientOrderDataViewNewModel> GetIQueryable()
        {
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();

            var importTaxCodes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t=>t.TaxType == (int)Needs.Ccs.Services.Enums.DecTaxType.Tariff);
            var addedValueTaxCodes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t => t.TaxType == (int)Needs.Ccs.Services.Enums.DecTaxType.AddedValueTax);

            var baseCountries = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>();
            //var importTaxes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();
            //var addedValueTaxes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();
            var orderItemRateView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemRateView>();
            var orderItemCategories = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();

            return from decList in decLists
                   join decHead in decHeads
                        on new
                        {
                            DecHeadID = decList.DeclarationID,
                            DecHeadIsSuccess = true,
                        }
                        equals new
                        {
                            DecHeadID = decHead.ID,
                            DecHeadIsSuccess = decHead.IsSuccess,
                        }
                   join order in orders
                        on new
                        {
                            OrderID = decHead.OrderID,
                            OrderDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                        }
                        equals new
                        {
                            OrderID = order.ID,
                            OrderDataStatus = order.Status,
                        }
                   join client in clients
                        on new
                        {
                            ClientID = order.ClientID,
                            ClientType = (int?)Needs.Ccs.Services.Enums.ClientType.Internal,
                            ClientDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                        }
                        equals new
                        {
                            ClientID = client.ID,
                            ClientType = client.ClientType,
                            ClientDataStatus = client.Status,
                        }

                        // --------------------------------- Left Join Begin -----------------------------------------

                   join baseCountry in baseCountries
                            on decList.OriginCountry equals baseCountry.Code
                            into baseCountriesInto
                   from baseCountry in baseCountriesInto.DefaultIfEmpty()

                   //join importTax in importTaxes
                   //    on new
                   //    {
                   //        OrderItemID = decList.OrderItemID,
                   //        OrderItemTaxesType = (int)Needs.Ccs.Services.Enums.CustomsRateType.ImportTax,
                   //        OrderItemTaxesDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                   //    }
                   //    equals new
                   //    {
                   //        OrderItemID = importTax.OrderItemID,
                   //        OrderItemTaxesType = importTax.Type,
                   //        OrderItemTaxesDataStatus = importTax.Status,
                   //    }
                   //    into importTaxesInto
                   //from importTax in importTaxesInto.DefaultIfEmpty()

                   //join addedValueTax in addedValueTaxes
                   //     on new
                   //     {
                   //         OrderItemID = decList.OrderItemID,
                   //         OrderItemTaxesType = (int)Needs.Ccs.Services.Enums.CustomsRateType.AddedValueTax,
                   //         OrderItemTaxesDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                   //     }
                   //     equals new
                   //     {
                   //         OrderItemID = addedValueTax.OrderItemID,
                   //         OrderItemTaxesType = addedValueTax.Type,
                   //         OrderItemTaxesDataStatus = addedValueTax.Status,
                   //     }
                   //     into addedValueTaxesInto
                   //from addedValueTax in addedValueTaxesInto.DefaultIfEmpty()

                   join orderItemRate in orderItemRateView on decList.OrderItemID equals orderItemRate.OrderItemID into orderItemRateView2
                   from orderItemRate in orderItemRateView2.DefaultIfEmpty()

                   join orderItemCategory in orderItemCategories
                        on new
                        {
                            OrderItemID = decList.OrderItemID,
                            OrderItemCategoryDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                        }
                        equals new
                        {
                            OrderItemID = orderItemCategory.OrderItemID,
                            OrderItemCategoryDataStatus = orderItemCategory.Status,
                        }
                        into orderItemCategoriesInto
                   from orderItemCategory in orderItemCategoriesInto.DefaultIfEmpty()

                       //join results2Tab in results2Tabs
                       //     on resultIQuery.DecHeadID
                       //     equals results2Tab.DecHeadID
                       //     into results2TabsInto
                       //from results2Tab in results2TabsInto.DefaultIfEmpty()

                   join orderItem in orderItems
                        on new
                        {
                            OrderItemID = decList.OrderItemID,
                            OrderItemDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                        }
                        equals new
                        {
                            OrderItemID = orderItem.ID,
                            OrderItemDataStatus = orderItem.Status,
                        }
                        into orderItemsInto
                   from orderItem in orderItemsInto.DefaultIfEmpty()

                   join importTacCode in importTaxCodes on decHead.ID equals importTacCode.DecTaxID into importTacCodeInto
                   from importTacCode in importTacCodeInto.DefaultIfEmpty()

                   join addedValueTaxCode in addedValueTaxCodes on decHead.ID equals addedValueTaxCode.DecTaxID into addedValueTaxCodeInto
                   from addedValueTaxCode in addedValueTaxCodeInto.DefaultIfEmpty()

                   // --------------------------------- Left Join End -----------------------------------------

                   orderby decHead.DDate descending, decHead.ContrNo descending, decList.GNo ascending
                   select new ClientOrderDataViewNewModel
                   {
                       ClientID = order.ClientID,
                       UserID = order.UserID,
                       OrderID = decList.OrderID,
                       OrderItemID = decList.OrderItemID,
                       DecListID = decList.ID,
                       DecHeadID = decList.DeclarationID,
                       DDate = decHead.DDate,
                       CodeTS = decList.CodeTS,
                       GName = decList.GName,
                       GModel = decList.GModel,
                       GoodsBrand = decList.GoodsBrand,
                       GoodsModel = decList.GoodsModel,
                       OriginCountry = decList.OriginCountry,
                       GQty = decList.GQty,
                       NetWt = decList.NetWt,
                       DeclPrice = decList.DeclPrice,
                       DeclTotal = decList.DeclTotal,
                       TradeCurr = decList.TradeCurr,
                       ContrNo = decHead.ContrNo,
                       CustomsExchangeRate = decHead.CustomsExchangeRate ?? 0,
                       OwnerName = decHead.OwnerName,
                       EntryId = decHead.EntryId,

                       OriginCountryName = baseCountry.Name,
                       TariffRate = orderItemRate != null ? orderItemRate.ImportTaxRate ?? 0 : 0,  //importTax.Rate,
                       Vat = orderItemRate != null ? orderItemRate.AddedValueTaxRate ?? 0 : 0,  //addedValueTax.Rate,
                       DeclTotalRMB = decList.DeclTotal * (decHead.CustomsExchangeRate ?? 0),
                       InvoiceCompany = decHead.OwnerName,
                       TaxName = orderItemCategory.TaxName,
                       TaxCode = orderItemCategory.TaxCode,
                       ReceiptRate = orderItemRate != null ? orderItemRate.ImportTaxReceiptRate ?? 0 : 0,  //importTax.ReceiptRate,
                       ProductUniqueCode = orderItem.ProductUniqueCode,
                       GNo = decList.GNo,
                       ImportTaxCode = importTacCode != null ? importTacCode.TaxNumber : "",
                       AddValueTaxCode = addedValueTaxCode != null ? addedValueTaxCode.TaxNumber : "",
                   };
        }

        public List<ClientOrderDataViewNewModel> GetResults2Tab(ClientOrderDataViewNewModel[] results)
        {
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            //var importTaxes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();
            //var addedValueTaxes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();
            var orderItemRateView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemRateView>();

            var decHeadCustomsRateUses = (from result in results
                                          group result by new { result.DecHeadID, } into g
                                          select new ClientOrderDataViewNewModel
                                          {
                                              DecHeadID = g.Key.DecHeadID,
                                              CustomsRate = g.FirstOrDefault() == null ? 0 : g.FirstOrDefault().CustomsExchangeRate,
                                          }).ToList();

            string[] decHeadIDInDecHeadCustomsRateUses = decHeadCustomsRateUses.Select(t => t.DecHeadID).ToArray();

            var results1Tabs = (from decList in decLists.Where(t => decHeadIDInDecHeadCustomsRateUses.Contains(t.DeclarationID))
                                    //join decHeadCustomsRateUse in decHeadCustomsRateUses on decList.DeclarationID equals decHeadCustomsRateUse.DecHeadID
                                //join importTax in importTaxes
                                //     on new
                                //     {
                                //         OrderItemID = decList.OrderItemID,
                                //         OrderItemTaxesType = (int)Needs.Ccs.Services.Enums.CustomsRateType.ImportTax,
                                //         OrderItemTaxesDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                                //     }
                                //     equals new
                                //     {
                                //         OrderItemID = importTax.OrderItemID,
                                //         OrderItemTaxesType = importTax.Type,
                                //         OrderItemTaxesDataStatus = importTax.Status,
                                //     }
                                //join addedValueTax in addedValueTaxes
                                //     on new
                                //     {
                                //         OrderItemID = decList.OrderItemID,
                                //         OrderItemTaxesType = (int)Needs.Ccs.Services.Enums.CustomsRateType.AddedValueTax,
                                //         OrderItemTaxesDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                                //     }
                                //     equals new
                                //     {
                                //         OrderItemID = addedValueTax.OrderItemID,
                                //         OrderItemTaxesType = addedValueTax.Type,
                                //         OrderItemTaxesDataStatus = addedValueTax.Status,
                                //     }

                                join orderItemRate in orderItemRateView on decList.OrderItemID equals orderItemRate.OrderItemID

                                select new ClientOrderDataViewNewModel
                                {
                                    DeclTotal = decList.DeclTotal,
                                    DecHeadID = decList.DeclarationID,
                                    //CustomsRate = decHeadCustomsRateUse.CustomsRate,
                                    TariffRate = orderItemRate.ImportTaxRate ?? 0,  //importTax.Rate,
                                    Vat = orderItemRate.AddedValueTaxRate ?? 0,  //addedValueTax.Rate,
                                }).ToList();

            results1Tabs = (from results1Tab in results1Tabs
                            join decHeadCustomsRateUse in decHeadCustomsRateUses on results1Tab.DecHeadID equals decHeadCustomsRateUse.DecHeadID
                            select new ClientOrderDataViewNewModel
                            {
                                DeclTotal = results1Tab.DeclTotal,
                                DecHeadID = results1Tab.DecHeadID,
                                CustomsRate = decHeadCustomsRateUse.CustomsRate,
                                TariffRate = results1Tab.TariffRate,
                                Vat = results1Tab.Vat,
                            }).ToList();

            var results2Tabs = (from results1Tab in results1Tabs
                                group results1Tab by new { results1Tab.DecHeadID, } into g
                                select new ClientOrderDataViewNewModel
                                {
                                    DecHeadID = g.Key.DecHeadID,
                                    TotalValueVat = g.Sum(t => t.DeclTotal * t.CustomsRate * (1 + t.TariffRate) * t.Vat),
                                }).ToList();

            return results2Tabs;
        }
    }

    public class ClientOrderDataViewNewModel : IUnique
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; } = string.Empty;

        /// <summary>
        /// UserID
        /// </summary>
        public string UserID { get; set; } = string.Empty;

        /// <summary>
        /// OrderID
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// OrderItemID
        /// </summary>
        public string OrderItemID { get; set; } = string.Empty;

        /// <summary>
        /// DecListID
        /// </summary>
        public string DecListID { get; set; } = string.Empty;

        /// <summary>
        /// DecHeadID
        /// </summary>
        public string DecHeadID { get; set; } = string.Empty;

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 10位商编
        /// </summary>
        public string CodeTS { get; set; } = string.Empty;

        /// <summary>
        /// 报关品名
        /// </summary>
        public string GName { get; set; } = string.Empty;

        /// <summary>
        /// 规格型号（申报要素）
        /// </summary>
        public string GModel { get; set; } = string.Empty;

        /// <summary>
        /// 货物品牌
        /// </summary>
        public string GoodsBrand { get; set; } = string.Empty;

        /// <summary>
        /// 货物型号
        /// </summary>
        public string GoodsModel { get; set; } = string.Empty;

        /// <summary>
        /// 原产地国别
        /// </summary>
        public string OriginCountry { get; set; } = string.Empty;

        /// <summary>
        /// 原产地国别名
        /// </summary>
        public string OriginCountryName { get; set; } = string.Empty;

        /// <summary>
        /// 成交数量
        /// </summary>
        public decimal GQty { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal? NetWt { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal DeclPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal DeclTotal { get; set; }

        /// <summary>
        /// 成交币制
        /// </summary>
        public string TradeCurr { get; set; } = string.Empty;

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; } = string.Empty;

        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal CustomsExchangeRate { get; set; }


        /// <summary>
        /// 关税率
        /// </summary>
        public decimal TariffRate { get; set; }

        /// <summary>
        /// 报关总价（RMB）
        /// </summary>
        public decimal DeclTotalRMB { get; set; }

        ///// <summary>
        ///// 应交关税(RMB)
        ///// </summary>
        //public decimal TariffPay { get; set; }

        ///// <summary>
        ///// 实交关税(RMB)(当总关税小于50RMB时为0；否则等于应交关税)
        ///// </summary>
        //public decimal TariffPayed { get; set; }

        ///// <summary>
        ///// 实交增值税(RMB)(当总增值税小于50RMB时为0；否则等于应交增值税)
        ///// </summary>
        //public decimal ValueVatPayed { get; set; }

        ///// <summary>
        ///// 完税价格(报关总价 + 应交关税)
        ///// </summary>
        //public decimal CustomsValue { get; set; }

        ///// <summary>
        ///// 完税价格增值税(完税价格 * 0.16)
        ///// </summary>
        //public decimal CustomsValueVat { get; set; }

        /// <summary>
        /// 开票公司
        /// </summary>
        public string InvoiceCompany { get; set; } = string.Empty;

        /// <summary>
        /// 报关单号
        /// </summary>
        public string EntryId { get; set; } = string.Empty;

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; } = string.Empty;

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; } = string.Empty;

        /// <summary>
        /// OwnerName
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public decimal CustomsRate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Vat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal TotalValueVat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal ReceiptRate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProductUniqueCode { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public int GNo { get; set; }

        /// <summary>
        /// 关税发票号
        /// </summary>
        public string ImportTaxCode { get; set; }

        /// <summary>
        /// 增值税发票号
        /// </summary>
        public string AddValueTaxCode { get; set; }
    }
}
