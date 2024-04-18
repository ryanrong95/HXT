using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 报关单
    /// </summary>
    public class ClientDecHeadsView : UniqueView<DeclareOrderViewModel, ScCustomReponsitory>
    {
        IUser User;

        public ClientDecHeadsView(IUser user)
        {
            this.User = user;
        }

        protected override IQueryable<DeclareOrderViewModel> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>().Where(item => item.ClientID == User.XDTClientID);
            //if (!User.IsMain)
            //{
            //    orders = orders.Where(item => item.AdminID == User.ID);
            //}

            var linq = from head in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>()
                       join order in orders on head.OrderID equals order.ID
                       where head.IsSuccess == true
                       orderby head.DDate descending
                       select new DeclareOrderViewModel
                       {
                           ID = head.ID,
                           OrderID = head.OrderID,
                           EntryId = head.EntryId,
                           ContrNo = head.ContrNo,
                           DDate = head.DDate.GetValueOrDefault(),
                       };
            return linq;
        }

        /// <summary>
        /// 根据传入参数获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        virtual protected IQueryable<DeclareOrderViewModel> GetExpressionData(LambdaExpression[] expressions)
        {
            var linq = this.GetIQueryable();
            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    linq = linq.Where(expression as Expression<Func<DeclareOrderViewModel, bool>>);
                }
            }
            return linq;
        }


        /// <summary>
        /// 根据传入参数和查询条件获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public PageList<DeclareOrderViewModel> GetPageList(LambdaExpression[] expressions, int PageSize, int PageIndex)
        {
            var dec = this.GetExpressionData(expressions);

            int total = dec.Count();
            var linq = (from head in dec.Skip(PageSize * (PageIndex - 1)).Take(PageSize)
                        join declist in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecLists>() on head.ID equals declist.DeclarationID into declists
                        orderby head.DDate descending
                        select new DeclareOrderViewModel
                        {
                            ID = head.ID,
                            OrderID = head.OrderID,
                            EntryId = head.EntryId,
                            ContrNo = head.ContrNo,
                            DDate = head.DDate,
                            TotalDeclarePrice = declists.Sum(item => item.DeclTotal),
                            Currency = declists.FirstOrDefault().TradeCurr
                        }).ToArray();

            return new PageList<DeclareOrderViewModel>(PageIndex, PageSize, linq, total);
        }
    }

    /// <summary>
    /// 报关单数据
    /// </summary>
    public class ClientDecHeadDataView : UniqueView<ClientOrderDataViewModel, ScCustomReponsitory>
    {
        IUser User;

        public ClientDecHeadDataView(IUser user)
        {
            this.User = user;
        }

        protected override IQueryable<ClientOrderDataViewModel> GetIQueryable()
        {
            var decLists = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecLists>();
            var decHeads = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>().Where(item => item.IsSuccess == true);
            var orders = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>().Where(item => item.Status == (int)GeneralStatus.Normal && item.ClientID == User.XDTClientID);
            //if (!User.IsMain)
            //{
            //    orders = orders.Where(item => item.AdminID == User.ID);
            //}

            var linq = from decList in decLists
                       join decHead in decHeads on decList.DeclarationID equals decHead.ID
                       join order in orders on decHead.OrderID equals order.ID
                       select new ClientOrderDataViewModel
                       {
                           DecListID = decList.ID,
                           OrderID = decList.OrderID,
                           UserID = order.UserID,
                           DDate = decHead.DDate,
                           ContrNo = decHead.ContrNo,
                           EntryId = decHead.EntryId,
                           GoodsModel = decList.GoodsModel,
                           DecHeadID = decHead.ID,
                           DeclTotal = decList.DeclTotal,
                           CustomsExchangeRate = decHead.CustomsExchangeRate.GetValueOrDefault(),
                           OrderItemID = decList.OrderItemID,
                           OriginCountry = decList.OriginCountry,
                           GNo = decList.GNo,
                           CodeTS = decList.CodeTS,
                           GName = decList.GName,
                           GModel = decList.GModel,
                           GoodsBrand = decList.GoodsBrand,
                           GQty = decList.GQty,
                           NetWt = decList.NetWt,
                           DeclPrice = decList.DeclPrice,
                           TradeCurr = decList.TradeCurr,
                           InvoiceCompany = decHead.OwnerName,
                       };
            return linq;
        }

        /// <summary>
        /// 根据传入参数获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        virtual protected IQueryable<ClientOrderDataViewModel> GetExpressionData(LambdaExpression[] expressions)
        {
            var linq = this.GetIQueryable();
            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    linq = linq.Where(expression as Expression<Func<ClientOrderDataViewModel, bool>>);
                }
            }
            return linq;
        }

        /// <summary>
        /// 根据传入参数和查询条件获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public PageList<ClientOrderDataViewModel> GetPageList(LambdaExpression[] expressions, int PageSize, int PageIndex)
        {
            //根据查询条件过滤
            var declist = this.GetExpressionData(expressions);
            int total = declist.Count();
            var results = declist.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToArray();

            var headids = results.Select(item => item.DecHeadID).Distinct();
            var orderitemids = results.Select(a => a.OrderItemID).ToArray();
            var orderitems = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderItems>().Where(item => item.Status == (int)GeneralStatus.Normal)
                .Where(item => orderitemids.Contains(item.ID)).Select(item => new { item.ID, item.ProductUniqueCode }).ToArray();
            var orderitemRates = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderItemRateView>().Where(item => orderitemids.Contains(item.OrderItemID))
                .Select(item => new { item.OrderItemID, item.ImportTaxRate, item.AddedValueTaxRate, item.ImportTaxReceiptRate }).ToArray();
            var orderitemCategories = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderItemCategories>().Where(item => item.Status == (int)GeneralStatus.Normal)
                .Where(item => orderitemids.Contains(item.OrderItemID)).Select(item => new { item.OrderItemID, item.TaxCode, item.TaxName }).ToArray();
            var baseCountrys = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.BaseCountries>().Where(item => results.Select(a => a.OriginCountry).Contains(item.Code))
                .Select(item => new { item.Code, item.Name }).ToArray();
            var importTaxCodes = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t => t.TaxType == 1)
                .Where(item => headids.Contains(item.DecTaxID)).Select(item => new { item.DecTaxID, item.TaxNumber });    //关税
            var addedValueTaxCodes = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t => t.TaxType == 2)
                .Where(item => headids.Contains(item.DecTaxID)).Select(item => new { item.DecTaxID, item.TaxNumber });//进口增值税

            #region 计算税率
            //获取DecHeadID的ID数组，获取税率

            var linq1 = from head in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>().Where(item => headids.Contains(item.ID))
                        join list in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecLists>() on head.ID equals list.DeclarationID
                        join item in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderItemRateView>() on list.OrderItemID equals item.OrderItemID
                        select new
                        {
                            DecListID = list.ID,
                            DecHeadID = head.ID,
                            DeclTotal = list.DeclTotal,
                            CustomsRate = head.CustomsExchangeRate.GetValueOrDefault(),
                            TariffRate = item.ImportTaxRate.GetValueOrDefault(),
                            Vat = item.AddedValueTaxRate.GetValueOrDefault(),
                        };

            var linq2 = (from list in linq1
                         group list by list.DecHeadID into lists
                         select new
                         {
                             DecHeadID = lists.Key,
                             TotalValueVat = lists.Sum(t => t.DeclTotal * t.CustomsRate * (1 + t.TariffRate) * t.Vat)
                         }).ToArray();

            #endregion


            var linq = from result in results
                       join rate in linq2 on result.DecHeadID equals rate.DecHeadID into rates
                       from _rate in rates.DefaultIfEmpty()
                       join orderitem in orderitems on result.OrderItemID equals orderitem.ID
                       join orderitemRate in orderitemRates on result.OrderItemID equals orderitemRate.OrderItemID into orderItemRateView2
                       from orderItemRate in orderItemRateView2.DefaultIfEmpty()
                       join orderitemCategory in orderitemCategories on result.OrderItemID equals orderitemCategory.OrderItemID into orderItemCategoriesInto
                       from orderItemCategory in orderItemCategoriesInto.DefaultIfEmpty()
                       join basecoutry in baseCountrys on result.OriginCountry equals basecoutry.Code
                       join importTacCode in importTaxCodes on result.DecHeadID equals importTacCode.DecTaxID into importTacCodeInto
                       from importTacCode in importTacCodeInto.DefaultIfEmpty()
                       join addedValueTaxCode in addedValueTaxCodes on result.DecHeadID equals addedValueTaxCode.DecTaxID into addedValueTaxCodeInto
                       from addedValueTaxCode in addedValueTaxCodeInto.DefaultIfEmpty()
                       select new ClientOrderDataViewModel
                       {
                           DecHeadID = result.DecHeadID,
                           ClientID = result.ClientID,
                           OrderID = result.OrderID,
                           UserID = result.UserID,
                           OrderItemID = result.OrderItemID,

                           DecListID = result.DecListID,
                           DDate = result.DDate,
                           CodeTS = result.CodeTS,
                           GName = result.GName,
                           GModel = result.GModel,
                           GNo = result.GNo,
                           GoodsBrand = result.GoodsBrand,
                           GoodsModel = result.GoodsModel,
                           OriginCountry = result.OriginCountry,
                           OriginCountryName = basecoutry.Name,
                           GQty = result.GQty,
                           NetWt = result.NetWt,
                           DeclPrice = result.DeclPrice,
                           DeclTotal = result.DeclTotal,
                           TradeCurr = result.TradeCurr,
                           ContrNo = result.ContrNo,
                           EntryId = result.EntryId,
                           InvoiceCompany = result.InvoiceCompany,

                           CustomsExchangeRate = result.CustomsExchangeRate,
                           TariffRate = (orderItemRate?.ImportTaxRate).GetValueOrDefault(),
                           Vat = (orderItemRate?.AddedValueTaxRate).GetValueOrDefault(),
                           DeclTotalRMB = result.DeclTotal * result.CustomsExchangeRate,

                           TaxName = orderItemCategory?.TaxName,
                           TaxCode = orderItemCategory?.TaxCode,
                           ReceiptRate = (orderItemRate?.ImportTaxReceiptRate).GetValueOrDefault(),
                           ProductUniqueCode = orderitem.ProductUniqueCode,
                           TotalValueVat = (_rate?.TotalValueVat).GetValueOrDefault(),
                           ImportTaxCode = importTacCode?.TaxNumber,
                           AddValueTaxCode = addedValueTaxCode?.TaxNumber
                       };

            return new PageList<ClientOrderDataViewModel>(PageIndex, PageSize, linq, total);
        }
    }

    /// <summary>
    /// 报关单数据数据模型
    /// </summary>
    public class ClientOrderDataViewModel : IUnique
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
        public string TradeCurr { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }

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

        /// <summary>
        /// 开票公司
        /// </summary>
        public string InvoiceCompany { get; set; }

        /// <summary>
        /// 报关单号
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// OwnerName
        /// </summary>
        public string OwnerName { get; set; }

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
        public string ProductUniqueCode { get; set; }

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

    /// <summary>
    /// 报关单
    /// </summary>
    public class DeclareOrderViewModel : IUnique
    {
        /// <summary>
        /// 报关单ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 报关单号
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 报关总金额
        /// </summary>
        public decimal TotalDeclarePrice { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
    }
}
