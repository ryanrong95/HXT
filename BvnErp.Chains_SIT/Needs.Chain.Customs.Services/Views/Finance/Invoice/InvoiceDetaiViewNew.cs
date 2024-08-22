using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 开票明细视图
    /// </summary>
    public class InvoiceDetaiViewNew : QueryView<InvoiceDetaiViewNewModel, ScCustomsReponsitory>
    {
        public InvoiceDetaiViewNew()
        {

        }

        public InvoiceDetaiViewNew(ScCustomsReponsitory reponsitory, IQueryable<InvoiceDetaiViewNewModel> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<InvoiceDetaiViewNewModel> GetIQueryable()
        {
            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>();
            var invoiceNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var iQuery = from invoiceNoticeItem in invoiceNoticeItems
                         join invoiceNotice in invoiceNotices on invoiceNoticeItem.InvoiceNoticeID equals invoiceNotice.ID
                         join client in clients on invoiceNotice.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         where invoiceNotice.Status == (int)Enums.InvoiceNoticeStatus.Confirmed
                         select new InvoiceDetaiViewNewModel
                         {
                             InvoiceNoticeItemID = invoiceNoticeItem.ID,
                             OrderID = invoiceNoticeItem.OrderID,
                             CompanyName = company.Name,
                             InvoiceTime = invoiceNotice.UpdateDate,
                             InvoiceNoticeItemCreateDate = invoiceNoticeItem.CreateDate,
                             OrderItemID = invoiceNoticeItem.OrderItemID,
                             InvoiceNo = invoiceNoticeItem.InvoiceNo,
                             InvoiceTaxRate = invoiceNotice.InvoiceTaxRate,
                             Difference = invoiceNoticeItem.Difference,
                             Amount = invoiceNoticeItem.Amount,
                         };

            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<InvoiceDetaiViewNewModel> iquery = this.IQueryable.Cast<InvoiceDetaiViewNewModel>();
            int total = iquery.Count();

            iquery = iquery.OrderByDescending(item => item.InvoiceNoticeItemCreateDate);

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myInvoiceNoticeItems = iquery.ToArray();

            //OrderItemID
            var orderItemsID = ienum_myInvoiceNoticeItems.Select(item => item.OrderItemID);






            #region 品名、型号、数量

            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderItemCategories = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();
            var baseUnits = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>();

            var linq_orderItems = from orderItem in orderItems
                                  join orderItemCategory in orderItemCategories on orderItem.ID equals orderItemCategory.OrderItemID into orderItemCategories2
                                  from orderItemCategory in orderItemCategories2.DefaultIfEmpty()
                                  join baseUnit in baseUnits on orderItem.Unit equals baseUnit.Code into baseUnits2
                                  from baseUnit in baseUnits2.DefaultIfEmpty()
                                  where orderItemsID.Contains(orderItem.ID)
                                  select new
                                  {
                                      OrderItemID = orderItem.ID,
                                      ProductName = orderItemCategory != null ? orderItemCategory.TaxName : "",
                                      ProductModel = orderItem.Model,
                                      Quantity = orderItem.Quantity,
                                      UnitName = baseUnit != null ? baseUnit.Name : "",
                                      TaxName = orderItemCategory != null ? orderItemCategory.TaxName : "",
                                      TaxCode = orderItemCategory != null ? orderItemCategory.TaxCode : "",
                                  };

            var ienums_orderItems = linq_orderItems.ToArray();

            #endregion







            var ienums_linq = from invoiceNoticeItem in ienum_myInvoiceNoticeItems
                              join orderItem in ienums_orderItems on invoiceNoticeItem.OrderItemID equals orderItem.OrderItemID into ienums_orderItems2
                              from orderItem in ienums_orderItems2.DefaultIfEmpty()
                              select new InvoiceDetaiViewNewModel
                              {
                                  InvoiceNoticeItemID = invoiceNoticeItem.InvoiceNoticeItemID,
                                  OrderID = invoiceNoticeItem.OrderID,
                                  CompanyName = invoiceNoticeItem.CompanyName,
                                  InvoiceTime = invoiceNoticeItem.InvoiceTime,
                                  InvoiceNoticeItemCreateDate = invoiceNoticeItem.InvoiceNoticeItemCreateDate,
                                  InvoiceTaxRate = invoiceNoticeItem.InvoiceTaxRate,

                                  ProductName = orderItem != null ? orderItem.ProductName : "*经纪代理服务*经纪代理",
                                  ProductModel = orderItem != null ? orderItem.ProductModel : "",
                                  Quantity = orderItem != null ? orderItem.Quantity : 1,
                                  InvoiceNo = invoiceNoticeItem.InvoiceNo,
                                  UnitName = orderItem != null ? orderItem.UnitName : "",

                                  DetailUnitPrice = orderItem == null ?
                                                (invoiceNoticeItem.Amount + invoiceNoticeItem.Difference) :
                                                (invoiceNoticeItem.Amount + invoiceNoticeItem.Difference) / orderItem.Quantity,
                                  DetailAmount = invoiceNoticeItem.Amount + invoiceNoticeItem.Difference,
                                  DetailSalesTotalPrice = (invoiceNoticeItem.Amount + invoiceNoticeItem.Difference) / (1 + invoiceNoticeItem.InvoiceTaxRate),

                                  TaxName = orderItem != null ? orderItem.TaxName : "",
                                  TaxCode = orderItem != null ? orderItem.TaxCode : "",

                                  DetailSalesUnitPrice = orderItem == null ?
                                        (invoiceNoticeItem.Amount + invoiceNoticeItem.Difference) / (1 + invoiceNoticeItem.InvoiceTaxRate) :
                                        (invoiceNoticeItem.Amount + invoiceNoticeItem.Difference) / (1 + invoiceNoticeItem.InvoiceTaxRate) / (orderItem != null ? orderItem.Quantity : 1),
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<InvoiceDetaiViewNewModel, object> convert = item => new
            {
                ID = item.InvoiceNoticeItemID,
                item.OrderID,
                ProductName = item.ProductName,  //item.OrderItem == null ? "*经纪代理服务*经纪代理" : item.OrderItem?.Category.TaxName,  //报关品名
                ProductModel = item.ProductModel,//型号
                Unit = item.UnitName,//单位
                Quantity = item.Quantity,//数量
                DetailUnitPrice = item.DetailUnitPrice.ToRound(4), //含税单价
                DetailAmount = item.DetailAmount.ToRound(2),//含税总额
                Name = item.CompanyName,
                item.InvoiceNo,
                UpdateDate = item.InvoiceTime?.ToString("yyyy/MM/dd"),
                TaxAmount = (item.DetailSalesTotalPrice * item.InvoiceTaxRate).ToRound(2),
                DetailSalesUnitPrice = item.DetailSalesUnitPrice.ToRound(4), //单价
                DetailSalesTotalPrice = item.DetailSalesTotalPrice.ToRound(4), //金额
                item.InvoiceTaxRate, //税率
                item.TaxName,//税务名称
                item.TaxCode,
                item.Difference,
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据 订单号 查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public InvoiceDetaiViewNew SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new InvoiceDetaiViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 开票公司 查询
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public InvoiceDetaiViewNew SearchByCompanyName(string companyName)
        {
            var linq = from query in this.IQueryable
                       where query.CompanyName.Contains(companyName)
                       select query;

            var view = new InvoiceDetaiViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 开票时间开始时间 查询
        /// </summary>
        /// <returns></returns>
        public InvoiceDetaiViewNew SearchByInvoiceTimeStartDate(DateTime startDate)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceTime >= startDate
                       select query;

            var view = new InvoiceDetaiViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 开票时间结束时间 查询
        /// </summary>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public InvoiceDetaiViewNew SearchByInvoiceTimeEndDate(DateTime endDate)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceTime < endDate.AddDays(1)
                       select query;

            var view = new InvoiceDetaiViewNew(this.Reponsitory, linq);
            return view;
        }
    }

    public class InvoiceDetaiViewNewModel
    {
        /// <summary>
        /// InvoiceNoticeItemID
        /// </summary>
        public string InvoiceNoticeItemID { get; set; }

        /// <summary>
        /// OrderID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 含税单价
        /// </summary>
        public decimal DetailUnitPrice { get; set; }

        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal DetailAmount { get; set; }

        /// <summary>
        /// 开票公司
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceTime { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal DetailSalesUnitPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal DetailSalesTotalPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Difference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime InvoiceNoticeItemCreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderItemID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Amount { get; set; }
    }

    public class InvoiceXmlDetaiViewNew : QueryView<InvoiceDetaiViewNewModel, ScCustomsReponsitory>
    {
        public InvoiceXmlDetaiViewNew()
        {

        }

        public InvoiceXmlDetaiViewNew(ScCustomsReponsitory reponsitory, IQueryable<InvoiceDetaiViewNewModel> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<InvoiceDetaiViewNewModel> GetIQueryable()
        {
            var invoiceNoticeXmlItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>();
            var invoiceNoticeXmls = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>();
            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>();



            var iQuery = from invoiceNoticeXmlItem in invoiceNoticeXmlItems
                         join invoiceNoticeXml in invoiceNoticeXmls on invoiceNoticeXmlItem.InvoiceNoticeXmlID equals invoiceNoticeXml.ID      
                         join invoiceNoticeItem in invoiceNoticeItems on invoiceNoticeXmlItem.InvoiceNoticeItemID equals invoiceNoticeItem.ID
                         where invoiceNoticeXml.InvoiceNo != null
                         select new InvoiceDetaiViewNewModel
                         {
                             InvoiceNoticeItemID = invoiceNoticeItem.ID,
                             OrderID = invoiceNoticeItem.OrderID,
                             CompanyName = invoiceNoticeXml.Gfmc,
                             InvoiceTime = invoiceNoticeXml.InvoiceDate,
                             InvoiceNoticeItemCreateDate = invoiceNoticeItem.CreateDate,
                             OrderItemID = invoiceNoticeItem.OrderItemID,
                             InvoiceNo = invoiceNoticeXml.InvoiceNo,  
                             Quantity = invoiceNoticeXmlItem.Sl,
                             DetailUnitPrice = invoiceNoticeXmlItem.Dj,
                             DetailAmount = invoiceNoticeXmlItem.Je + invoiceNoticeXmlItem.Se,                          
                             TaxAmount = invoiceNoticeXmlItem.Se,
                             InvoiceTaxRate = invoiceNoticeXmlItem.Slv
                         };

            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<InvoiceDetaiViewNewModel> iquery = this.IQueryable.Cast<InvoiceDetaiViewNewModel>();
            int total = iquery.Count();

            iquery = iquery.OrderByDescending(item => item.InvoiceNoticeItemCreateDate);

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myInvoiceNoticeItems = iquery.ToArray();

            //OrderItemID
            var orderItemsID = ienum_myInvoiceNoticeItems.Select(item => item.OrderItemID);






            #region 品名、型号、数量

            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderItemCategories = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();
            var baseUnits = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>();

            var linq_orderItems = from orderItem in orderItems
                                  join orderItemCategory in orderItemCategories on orderItem.ID equals orderItemCategory.OrderItemID into orderItemCategories2
                                  from orderItemCategory in orderItemCategories2.DefaultIfEmpty()
                                  join baseUnit in baseUnits on orderItem.Unit equals baseUnit.Code into baseUnits2
                                  from baseUnit in baseUnits2.DefaultIfEmpty()
                                  where orderItemsID.Contains(orderItem.ID)
                                  select new
                                  {
                                      OrderItemID = orderItem.ID,
                                      ProductName = orderItemCategory != null ? orderItemCategory.TaxName : "",
                                      ProductModel = orderItem.Model,
                                      Quantity = orderItem.Quantity,
                                      UnitName = baseUnit != null ? baseUnit.Name : "",
                                      TaxName = orderItemCategory != null ? orderItemCategory.TaxName : "",
                                      TaxCode = orderItemCategory != null ? orderItemCategory.TaxCode : "",                                    
                                  };

            var ienums_orderItems = linq_orderItems.ToArray();

            #endregion







            var ienums_linq = from invoiceNoticeItem in ienum_myInvoiceNoticeItems
                              join orderItem in ienums_orderItems on invoiceNoticeItem.OrderItemID equals orderItem.OrderItemID into ienums_orderItems2
                              from orderItem in ienums_orderItems2.DefaultIfEmpty()
                              select new InvoiceDetaiViewNewModel
                              {
                                  InvoiceNoticeItemID = invoiceNoticeItem.InvoiceNoticeItemID,
                                  OrderID = invoiceNoticeItem.OrderID,
                                  CompanyName = invoiceNoticeItem.CompanyName,
                                  InvoiceTime = invoiceNoticeItem.InvoiceTime,
                                  InvoiceNoticeItemCreateDate = invoiceNoticeItem.InvoiceNoticeItemCreateDate,
                                  InvoiceTaxRate = invoiceNoticeItem.InvoiceTaxRate,

                                  ProductName = orderItem != null ? orderItem.ProductName : "*经纪代理服务*经纪代理",
                                  ProductModel = orderItem != null ? orderItem.ProductModel : "",
                                  Quantity = invoiceNoticeItem.Quantity,
                                  InvoiceNo = invoiceNoticeItem.InvoiceNo,
                                  UnitName = orderItem != null ? orderItem.UnitName : "",

                                  DetailUnitPrice = invoiceNoticeItem.DetailAmount/ invoiceNoticeItem.Quantity,
                                  DetailAmount = invoiceNoticeItem.DetailAmount,

                                  TaxName = orderItem != null ? orderItem.TaxName : "",
                                  TaxCode = orderItem != null ? orderItem.TaxCode : "",
                                  TaxAmount = invoiceNoticeItem.TaxAmount,
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<InvoiceDetaiViewNewModel, object> convert = item => new
            {
                ID = item.InvoiceNoticeItemID,
                item.OrderID,
                ProductName = item.ProductName,  //item.OrderItem == null ? "*经纪代理服务*经纪代理" : item.OrderItem?.Category.TaxName,  //报关品名
                ProductModel = item.ProductModel,//型号
                Unit = item.UnitName,//单位
                Quantity = item.Quantity,//数量
                DetailUnitPrice = item.DetailUnitPrice.ToRound(4), //含税单价
                DetailAmount = item.DetailAmount.ToRound(2),//含税总额
                Name = item.CompanyName,
                item.InvoiceNo,
                UpdateDate = item.InvoiceTime?.ToString("yyyy/MM/dd"),
                TaxAmount = item.TaxAmount.ToRound(2),
                //DetailSalesUnitPrice = item.DetailSalesUnitPrice.ToRound(4), //单价
                //DetailSalesTotalPrice = item.DetailSalesTotalPrice.ToRound(4), //金额
                item.InvoiceTaxRate, //税率
                item.TaxName,//税务名称
                item.TaxCode,
                item.Difference,
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据 订单号 查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public InvoiceXmlDetaiViewNew SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new InvoiceXmlDetaiViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 开票公司 查询
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public InvoiceXmlDetaiViewNew SearchByCompanyName(string companyName)
        {
            var linq = from query in this.IQueryable
                       where query.CompanyName.Contains(companyName)
                       select query;

            var view = new InvoiceXmlDetaiViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 开票时间开始时间 查询
        /// </summary>
        /// <returns></returns>
        public InvoiceXmlDetaiViewNew SearchByInvoiceTimeStartDate(DateTime startDate)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceTime >= startDate
                       select query;

            var view = new InvoiceXmlDetaiViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 开票时间结束时间 查询
        /// </summary>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public InvoiceXmlDetaiViewNew SearchByInvoiceTimeEndDate(DateTime endDate)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceTime < endDate.AddDays(1)
                       select query;

            var view = new InvoiceXmlDetaiViewNew(this.Reponsitory, linq);
            return view;
        }
    }

}
