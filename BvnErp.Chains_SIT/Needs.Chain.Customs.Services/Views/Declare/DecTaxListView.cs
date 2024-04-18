using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecTaxListView : View<Needs.Ccs.Services.Views.DecTaxListView.DecTaxListViewModels, ScCustomsReponsitory>
    {
        public class DecTaxListViewModels : IUnique
        {
            public string ID { get; set; }

            public string DecHeadID { get; set; }

            public string ContrNo { get; set; }

            public string OrderID { get; set; }

            public DateTime? DDate { get; set; }

            public string Currency { get; set; }

            public decimal? DecAmount { get; set; }

            public decimal? OrderAgentAmount { get; set; }

            public string EntryId { get; set; }

            public DateTime? PayDate { get; set; }

            public DateTime? DeductionTime { get; set; }

            public decimal? TariffValue { get; set; }

            public decimal? ExciseTaxValue { get; set; }

            public decimal? AddedValue { get; set; }

            public string TariffFileUrl { get; set; }

            public string ExciseTaxFileUrl { get; set; }

            public string VatFileUrl { get; set; }

            public Enums.InvoiceType InvoiceType { get; set; }

            public Enums.DecTaxStatus DecTaxStatus { get; set; }

            public string OwnerName { get; set; }

            public DateTime? FillinDate { get; set; }
        }

        protected override IQueryable<DecTaxListViewModels> GetIQueryable()
        {
            var decTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var resultIQuerys = from decTax in decTaxs
                                join decHead in decHeads on decTax.ID equals decHead.ID
                                join order in orders
                                     on new { OrderID = decHead.OrderID, OrderIDDataStatus = (int)Enums.Status.Normal, }
                                     equals new { OrderID = order.ID, OrderIDDataStatus = order.Status, }
                                join client in clients on order.ClientID equals client.ID
                                join company in companies on client.CompanyID equals company.ID
                                orderby decHead.DDate descending
                                select new DecTaxListViewModels
                                {
                                    DecHeadID = decHead.ID,
                                    ContrNo = decHead.ContrNo,
                                    OrderID = decHead.OrderID,
                                    DDate = decHead.DDate,
                                    OrderAgentAmount = order.DeclarePrice,
                                    EntryId = decHead.EntryId,
                                    Currency = order.Currency,
                                    InvoiceType = (Enums.InvoiceType)decTax.InvoiceType,
                                    DecTaxStatus = (Enums.DecTaxStatus)decTax.Status,
                                    //OwnerName = decHead.OwnerName,
                                    OwnerName = company.Name
                                };

            return this.OnReadShips(resultIQuerys);
        }

        private IQueryable<DecTaxListViewModels> GetDecAmountSumTab(IQueryable<string> decheadIDs)
        {
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();

            return from decList in decLists
                   where decheadIDs.Contains(decList.DeclarationID)
                   group decList by new { decList.DeclarationID } into g
                   select new DecTaxListViewModels
                   {
                       DecHeadID = g.Key.DeclarationID,
                       DecAmount = g.Sum(t => t.DeclTotal),
                   };
        }

        private IQueryable<DecTaxListViewModels> GetVatfFlowTab2(IQueryable<string> decheadIDs)
        {
            var decTaxFlows = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>();

            return from decTaxFlow in decTaxFlows
                   where decTaxFlow.TaxType == (int)Enums.DecTaxType.AddedValueTax
                        && decheadIDs.Contains(decTaxFlow.DecTaxID)
                   group decTaxFlow by new { decTaxFlow.DecTaxID } into g
                   select new DecTaxListViewModels
                   {
                       DecHeadID = g.Key.DecTaxID,
                       PayDate = g.FirstOrDefault() != null ? g.FirstOrDefault().PayDate : null,
                       FillinDate = g.FirstOrDefault() != null ? g.FirstOrDefault().FillinDate : null,
                       DeductionTime = g.FirstOrDefault() != null ? g.FirstOrDefault().DeductionTime : null,
                   };
        }

        private IQueryable<DecTaxListViewModels> GetTariffValueSum(IQueryable<string> orderIDs)
        {
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderItemTaxes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();

            return from orderItem in orderItems
                   join orderItemTax in orderItemTaxes on orderItem.ID equals orderItemTax.OrderItemID
                   where orderItemTax.Type == (int)Enums.CustomsRateType.ImportTax && orderIDs.Contains(orderItem.OrderID)
                   group new { orderItem, orderItemTax } by new { orderItem.OrderID } into g
                   select new DecTaxListViewModels
                   {
                       OrderID = g.Key.OrderID,
                       TariffValue = g.Sum(t => t.orderItemTax.Value),
                   };
        }

        private IQueryable<DecTaxListViewModels> GetExciseTaxValueSum(IQueryable<string> orderIDs)
        {
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderItemTaxes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();

            return from orderItem in orderItems
                   join orderItemTax in orderItemTaxes on orderItem.ID equals orderItemTax.OrderItemID
                   where orderItemTax.Type == (int)Enums.CustomsRateType.ConsumeTax && orderIDs.Contains(orderItem.OrderID)
                   group new { orderItem, orderItemTax } by new { orderItem.OrderID } into g
                   select new DecTaxListViewModels
                   {
                       OrderID = g.Key.OrderID,
                       ExciseTaxValue = g.Sum(t => t.orderItemTax.Value),
                   };
        }

        private IQueryable<DecTaxListViewModels> GetAddedValueSum(IQueryable<string> orderIDs)
        {
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderItemTaxes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();

            return from orderItem in orderItems
                   join orderItemTax in orderItemTaxes on orderItem.ID equals orderItemTax.OrderItemID
                   where orderItemTax.Type == (int)Enums.CustomsRateType.AddedValueTax && orderIDs.Contains(orderItem.OrderID)
                   group new { orderItem, orderItemTax } by new { orderItem.OrderID } into g
                   select new DecTaxListViewModels
                   {
                       OrderID = g.Key.OrderID,
                       AddedValue = g.Sum(t => t.orderItemTax.Value),
                   };
        }

        private IQueryable<DecTaxListViewModels> GetTariffFileTab2(IQueryable<string> decheadIDs)
        {
            var decHeadFiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>();

            return from decHeadFile in decHeadFiles
                   where decHeadFile.FileType == (int)Enums.FileType.DecHeadTariffFile
                        && decheadIDs.Contains(decHeadFile.DecHeadID)
                   group decHeadFile by new { decHeadFile.DecHeadID } into g
                   select new DecTaxListViewModels
                   {
                       DecHeadID = g.Key.DecHeadID,
                       TariffFileUrl = g.FirstOrDefault() != null ? g.FirstOrDefault().Url : string.Empty,
                   };
        }

        private IQueryable<DecTaxListViewModels> GetExciseTaxFileTab2(IQueryable<string> decheadIDs)
        {
            var decHeadFiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>();

            return from decHeadFile in decHeadFiles
                   where decHeadFile.FileType == (int)Enums.FileType.DecHeadExciseTaxFile
                        && decheadIDs.Contains(decHeadFile.DecHeadID)
                   group decHeadFile by new { decHeadFile.DecHeadID } into g
                   select new DecTaxListViewModels
                   {
                       DecHeadID = g.Key.DecHeadID,
                       ExciseTaxFileUrl = g.FirstOrDefault() != null ? g.FirstOrDefault().Url : string.Empty,
                   };
        }

        private IQueryable<DecTaxListViewModels> GetVatFileTab2(IQueryable<string> decheadIDs)
        {
            var decHeadFiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>();

            return from decHeadFile in decHeadFiles
                   where decHeadFile.FileType == (int)Enums.FileType.DecHeadVatFile
                        && decheadIDs.Contains(decHeadFile.DecHeadID)
                   group decHeadFile by new { decHeadFile.DecHeadID } into g
                   select new DecTaxListViewModels
                   {
                       DecHeadID = g.Key.DecHeadID,
                       VatFileUrl = g.FirstOrDefault() != null ? g.FirstOrDefault().Url : string.Empty,
                   };
        }

        protected IQueryable<DecTaxListViewModels> OnReadShips(IQueryable<DecTaxListViewModels> resultIQuerys)
        {
            var decheadIDs = resultIQuerys.Select(t => t.DecHeadID);
            var orderIDs = resultIQuerys.Select(t => t.OrderID);

            var decAmountSumTab = GetDecAmountSumTab(decheadIDs);
            var vatfFlowTab2 = GetVatfFlowTab2(decheadIDs);
            var tariffValueSum = GetTariffValueSum(orderIDs);
            var exciseTaxValueSum = GetExciseTaxValueSum(orderIDs);
            var addedValueSum = GetAddedValueSum(orderIDs);
            var tariffFileTab2 = GetTariffFileTab2(decheadIDs);
            var exciseTaxFileTab2 = GetExciseTaxFileTab2(decheadIDs);
            var vatFileTab2 = GetVatFileTab2(decheadIDs);

            return from result in resultIQuerys

                   join decAmountSum in decAmountSumTab on result.DecHeadID equals decAmountSum.DecHeadID into decAmountSumTabInto
                   from decAmountSum in decAmountSumTabInto.DefaultIfEmpty()

                   join vatfFlow in vatfFlowTab2 on result.DecHeadID equals vatfFlow.DecHeadID into vatfFlowTab2Into
                   from vatfFlow in vatfFlowTab2Into.DefaultIfEmpty()

                   join tariffValue in tariffValueSum on result.OrderID equals tariffValue.OrderID into tariffValueSumInto
                   from tariffValue in tariffValueSumInto.DefaultIfEmpty()

                   join exciseTaxValue in exciseTaxValueSum on result.OrderID equals exciseTaxValue.OrderID into exciseTaxValueSumInto
                   from exciseTaxValue in exciseTaxValueSumInto.DefaultIfEmpty()

                   join addedValue in addedValueSum on result.OrderID equals addedValue.OrderID into addedValueSumInto
                   from addedValue in addedValueSumInto.DefaultIfEmpty()

                   join tariffFile in tariffFileTab2 on result.DecHeadID equals tariffFile.DecHeadID into tariffFileTab2Into
                   from tariffFile in tariffFileTab2Into.DefaultIfEmpty()

                   join exciseTaxFile in exciseTaxFileTab2 on result.DecHeadID equals exciseTaxFile.DecHeadID into exciseTaxFileTab2Into
                   from exciseTaxFile in exciseTaxFileTab2Into.DefaultIfEmpty()

                   join vatFile in vatFileTab2 on result.DecHeadID equals vatFile.DecHeadID into vatFileTab2Into
                   from vatFile in vatFileTab2Into.DefaultIfEmpty()


                   select new DecTaxListViewModels
                   {
                       DecHeadID = result.DecHeadID,
                       ContrNo = result.ContrNo,
                       OrderID = result.OrderID,
                       DDate = result.DDate,
                       Currency = result.Currency,
                       DecAmount = decAmountSum.DecAmount,
                       OrderAgentAmount = result.OrderAgentAmount,
                       EntryId = result.EntryId,
                       PayDate = vatfFlow.PayDate,
                       FillinDate = vatfFlow.FillinDate,
                       DeductionTime = vatfFlow.DeductionTime,
                       TariffValue = tariffValue.TariffValue,
                       ExciseTaxValue = exciseTaxValue.ExciseTaxValue,
                       AddedValue = addedValue.AddedValue,
                       TariffFileUrl = tariffFile.TariffFileUrl,
                       ExciseTaxFileUrl = exciseTaxFile.ExciseTaxFileUrl,
                       VatFileUrl = vatFile.VatFileUrl,
                       InvoiceType = result.InvoiceType,
                       DecTaxStatus = result.DecTaxStatus,
                       OwnerName = result.OwnerName,
                   };

        }

    }


    public class DecTaxListViewRJ : QueryView<DecTaxListViewModelNEWs, ScCustomsReponsitory>
    {
        public DecTaxListViewRJ()
        {
        }

        internal DecTaxListViewRJ(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected DecTaxListViewRJ(ScCustomsReponsitory reponsitory, IQueryable<DecTaxListViewModelNEWs> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<DecTaxListViewModelNEWs> GetIQueryable()
        {
            var decTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var clientAdmins = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>().Where(t => t.Type == (int)Enums.ClientAdminType.Merchandiser && t.Status == (int)Enums.Status.Normal);

            var resultIQuerys = from decTax in decTaxs
                                join decHead in decHeads on decTax.ID equals decHead.ID
                                join order in orders on decHead.OrderID equals order.ID
                                join client in clients on order.ClientID equals client.ID
                                join company in companies on client.CompanyID equals company.ID
                                join clientAdmin in clientAdmins on client.ID equals clientAdmin.ClientID
                                select new DecTaxListViewModelNEWs
                                {
                                    DecHeadID = decHead.ID,
                                    ContrNo = decHead.ContrNo,
                                    OrderID = decHead.OrderID,
                                    DDate = decHead.DDate,
                                    OrderAgentAmount = order.DeclarePrice,
                                    EntryId = decHead.EntryId,
                                    Currency = order.Currency,
                                    InvoiceType = (Enums.InvoiceType)decTax.InvoiceType,
                                    DecTaxStatus = (Enums.DecTaxStatus)decTax.Status,
                                    OwnerName = company.Name,
                                    AdminID = clientAdmin.AdminID
                                };
            return resultIQuerys;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<DecTaxListViewModelNEWs> iquery = this.IQueryable.Cast<DecTaxListViewModelNEWs>().OrderByDescending(item => item.DDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myDectax = iquery.ToArray();

            //报关单ID
            var arr_DeclareIDs = ienum_myDectax.Select(t => t.DecHeadID).ToArray();
            var arr_OrderIDs = ienum_myDectax.Select(t => t.OrderID).ToArray();

            //报关单附件
            var decHeadFiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>();
            var arr_decHeadFiles = (from decHeadFile in decHeadFiles
                                    where arr_DeclareIDs.Contains(decHeadFile.DecHeadID)
                                    group decHeadFile by new { decHeadFile.DecHeadID, decHeadFile.FileType } into g
                                    select new
                                    {
                                        DecHeadID = g.Key.DecHeadID,
                                        FileType = g.Key.FileType,
                                        FileUrl = g.FirstOrDefault() != null ? g.FirstOrDefault().Url : string.Empty,
                                    }).ToArray();

            //报关单金额
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            var arr_DeclareAmount = (from decList in decLists
                                     where arr_DeclareIDs.Contains(decList.DeclarationID)
                                     group decList by new { decList.DeclarationID } into g
                                     select new
                                     {
                                         DecHeadID = g.Key.DeclarationID,
                                         DecAmount = g.Sum(t => t.DeclTotal),
                                     }).ToArray();


            //税费金额
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderItemTaxes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();

            var arr_Tax = (from orderItem in orderItems
                           join orderItemTax in orderItemTaxes on orderItem.ID equals orderItemTax.OrderItemID
                           where arr_OrderIDs.Contains(orderItem.OrderID)
                           group new { orderItem, orderItemTax } by new { orderItem.OrderID, orderItemTax.Type } into g
                           select new
                           {
                               OrderID = g.Key.OrderID,
                               Type = g.Key.Type,
                               Value = g.Sum(t => t.orderItemTax.Value),
                           }).ToArray();


            //缴税日期等
            var decTaxFlows = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>();

            var arr_Flows = (from decTaxFlow in decTaxFlows
                             where decTaxFlow.TaxType == (int)Enums.DecTaxType.AddedValueTax
                                  && arr_DeclareIDs.Contains(decTaxFlow.DecTaxID)
                             group decTaxFlow by new { decTaxFlow.DecTaxID } into g
                             select new
                             {
                                 DecHeadID = g.Key.DecTaxID,
                                 PayDate = g.FirstOrDefault() != null ? g.FirstOrDefault().PayDate : null,
                                 FillinDate = g.FirstOrDefault() != null ? g.FirstOrDefault().FillinDate : null,
                                 DeductionTime = g.FirstOrDefault() != null ? g.FirstOrDefault().DeductionTime : null,
                             }).ToArray();



            var ienums_linq = from result in ienum_myDectax
                              join decAmountSum in arr_DeclareAmount on result.DecHeadID equals decAmountSum.DecHeadID into decAmountSumTabInto
                              from decAmountSum in decAmountSumTabInto.DefaultIfEmpty()

                              join vatfFlow in arr_Flows on result.DecHeadID equals vatfFlow.DecHeadID into vatfFlowTab2Into
                              from vatfFlow in vatfFlowTab2Into.DefaultIfEmpty()

                              join tariffValue in arr_Tax.Where(t => t.Type == (int)Enums.CustomsRateType.ImportTax) on result.OrderID equals tariffValue.OrderID into tariffValueSumInto
                              from tariffValue in tariffValueSumInto.DefaultIfEmpty()

                              join exciseTaxValue in arr_Tax.Where(t => t.Type == (int)Enums.CustomsRateType.ExportTax) on result.OrderID equals exciseTaxValue.OrderID into exciseTaxValueSumInto
                              from exciseTaxValue in exciseTaxValueSumInto.DefaultIfEmpty()

                              join addedValue in arr_Tax.Where(t => t.Type == (int)Enums.CustomsRateType.AddedValueTax) on result.OrderID equals addedValue.OrderID into addedValueSumInto
                              from addedValue in addedValueSumInto.DefaultIfEmpty()

                              join tariffFile in arr_decHeadFiles.Where(t => t.FileType == (int)Enums.FileType.DecHeadTariffFile) on result.DecHeadID equals tariffFile.DecHeadID into tariffFileTab2Into
                              from tariffFile in tariffFileTab2Into.DefaultIfEmpty()

                              join exciseTaxFile in arr_decHeadFiles.Where(t => t.FileType == (int)Enums.FileType.DecHeadExciseTaxFile) on result.DecHeadID equals exciseTaxFile.DecHeadID into exciseTaxFileTab2Into
                              from exciseTaxFile in exciseTaxFileTab2Into.DefaultIfEmpty()

                              join vatFile in arr_decHeadFiles.Where(t => t.FileType == (int)Enums.FileType.DecHeadVatFile) on result.DecHeadID equals vatFile.DecHeadID into vatFileTab2Into
                              from vatFile in vatFileTab2Into.DefaultIfEmpty()


                              select new DecTaxListViewModelNEWs
                              {
                                  DecHeadID = result.DecHeadID,
                                  ContrNo = result.ContrNo,
                                  OrderID = result.OrderID,
                                  DDate = result.DDate,
                                  Currency = result.Currency,
                                  DecAmount = decAmountSum.DecAmount,
                                  OrderAgentAmount = result.OrderAgentAmount,
                                  EntryId = result.EntryId,
                                  PayDate = vatfFlow?.PayDate,
                                  FillinDate = vatfFlow?.FillinDate,
                                  DeductionTime = vatfFlow?.DeductionTime,
                                  TariffValue = tariffValue?.Value,
                                  ExciseTaxValue = exciseTaxValue?.Value,
                                  AddedValue = addedValue?.Value,
                                  TariffFileUrl = tariffFile?.FileUrl,
                                  ExciseTaxFileUrl = exciseTaxFile?.FileUrl,
                                  VatFileUrl = vatFile?.FileUrl,
                                  InvoiceType = result.InvoiceType,
                                  DecTaxStatus = result.DecTaxStatus,
                                  OwnerName = result.OwnerName,
                                  AdminID = result.AdminID
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


            Func<DecTaxListViewModelNEWs, object> convert = decTax => new
            {
                ID = decTax.DecHeadID,
                ContrNo = decTax.ContrNo,
                OrderID = decTax.OrderID,
                Currency = decTax.Currency,
                SwapAmount = decTax.DecAmount,
                OrderAgentAmount = decTax.OrderAgentAmount?.ToRound(2),
                EntryId = decTax.EntryId,
                DDate = decTax.DDate?.ToString("yyyy-MM-dd"),
                PayDate = decTax.PayDate != null ? decTax.PayDate?.ToShortDateString() : null,
                FillinDate = decTax.FillinDate != null ? decTax.FillinDate?.ToShortDateString() : string.Empty,
                DeductionTime = decTax.DeductionTime != null ? decTax.DeductionTime?.ToShortDateString() : null,
                IsDecHeadTariffFile = decTax.TariffValue < 50M ? "-" : (string.IsNullOrEmpty(decTax.TariffFileUrl) ? "未上传" : "已上传"),
                IsDecHeadExciseTaxFile = decTax.ExciseTaxValue.GetValueOrDefault() < 50M ? "-" : (string.IsNullOrEmpty(decTax.ExciseTaxFileUrl) ? "未上传" : "已上传"),
                IsDecHeadVatFile = decTax.AddedValue < 50M ? "-" : (string.IsNullOrEmpty(decTax.VatFileUrl) ? "未上传" : "已上传"),
                TariffURL = FileDirectory.Current.FileServerUrl + @"/" + (!string.IsNullOrEmpty(decTax.TariffFileUrl) ? decTax.TariffFileUrl.ToUrl() : string.Empty),
                ExciseTaxURL = FileDirectory.Current.FileServerUrl + @"/" + (!string.IsNullOrEmpty(decTax.ExciseTaxFileUrl) ? decTax.ExciseTaxFileUrl.ToUrl() : string.Empty),
                VatURL = FileDirectory.Current.FileServerUrl + @"/" + (!string.IsNullOrEmpty(decTax.VatFileUrl) ? decTax.VatFileUrl.ToUrl() : string.Empty),
                InvoiceType = decTax.InvoiceType.GetDescription(),
                InvoiceTypeData = decTax.InvoiceType,
                DecTaxStatus = decTax.DecTaxStatus,
                Status = decTax.DecTaxStatus.GetDescription(),
                OwnerName = decTax.OwnerName,

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
        /// 查询合同号
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>视图</returns>
        public DecTaxListViewRJ SearchByContrNo(string contractID)
        {
            var linq = from query in this.IQueryable
                       where query.ContrNo.Contains(contractID)
                       select query;

            var view = new DecTaxListViewRJ(this.Reponsitory, linq);
            return view;
        }


        /// <summary>
        /// 报关单号查询
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public DecTaxListViewRJ SearchByEntryID(string entryId)
        {
            var linq = from query in this.IQueryable
                       where query.EntryId.Contains(entryId)
                       select query;

            var view = new DecTaxListViewRJ(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查询订单号
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>视图</returns>
        public DecTaxListViewRJ SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new DecTaxListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecTaxListViewRJ SearchByInvoiceType(Needs.Ccs.Services.Enums.InvoiceType InvoiceType)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceType == InvoiceType
                       select query;

            var view = new DecTaxListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecTaxListViewRJ SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate >= fromtime
                       select query;

            var view = new DecTaxListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecTaxListViewRJ SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate <= totime
                       select query;

            var view = new DecTaxListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecTaxListViewRJ SearchByOwnerName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.OwnerName.Contains(ClientName)
                       select query;

            var view = new DecTaxListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecTaxListViewRJ SearchByAdminID(string adminid)
        {
            var linq = from query in this.IQueryable
                       where query.AdminID == adminid
                       select query;

            var view = new DecTaxListViewRJ(this.Reponsitory, linq);
            return view;
        }

        public DecTaxListViewRJ SearchByDecTaxStatus(Needs.Ccs.Services.Enums.DecTaxStatus DecTaxStatus)
        {
            var linq = from query in this.IQueryable
                           where query.DecTaxStatus == DecTaxStatus
                       select query;

            var view = new DecTaxListViewRJ(this.Reponsitory, linq);
            return view;
        }
    }



    public class DecTaxListViewModelNEWs : IUnique
    {
        public string ID { get; set; }

        public string DecHeadID { get; set; }

        public string ContrNo { get; set; }

        public string OrderID { get; set; }

        public DateTime? DDate { get; set; }

        public string Currency { get; set; }

        public decimal? DecAmount { get; set; }

        public decimal? OrderAgentAmount { get; set; }

        public string EntryId { get; set; }

        public DateTime? PayDate { get; set; }

        public DateTime? DeductionTime { get; set; }

        public decimal? TariffValue { get; set; }

        public decimal? ExciseTaxValue { get; set; }

        public decimal? AddedValue { get; set; }

        public string TariffFileUrl { get; set; }

        public string ExciseTaxFileUrl { get; set; }

        public string VatFileUrl { get; set; }

        public Enums.InvoiceType InvoiceType { get; set; }

        public Enums.DecTaxStatus DecTaxStatus { get; set; }

        public string OwnerName { get; set; }

        public DateTime? FillinDate { get; set; }

        /// <summary>
        /// 跟单员
        /// </summary>
        public string AdminID { get; set; }
    }
}
