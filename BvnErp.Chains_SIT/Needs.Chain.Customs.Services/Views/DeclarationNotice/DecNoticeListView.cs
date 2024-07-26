using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 报关通知待制单列表视图
    /// </summary>
    public class DecNoticeListView : UniqueView<Models.DecNoticeListModel, ScCustomsReponsitory>
    {
        public DecNoticeListView()
        {

        }

        internal DecNoticeListView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.DecNoticeListModel> GetIQueryable()
        {
            throw new NotImplementedException();
        }

        private IQueryable<Models.DecNoticeListModel> GetCommon(int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            var declarationNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var decNoticeVoyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();
            var orderVoyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>();

            var baseLists = from declarationNotice in declarationNotices
                            join order in orders on declarationNotice.OrderID equals order.ID
                            join client in clients on order.ClientID equals client.ID
                            join company in companies on client.CompanyID equals company.ID
                            where declarationNotice.Status == (int)Enums.DeclareNoticeStatus.UnDec
                                && order.OrderStatus == (int)Enums.OrderStatus.QuoteConfirmed
                                && order.Status == (int)Enums.Status.Normal
                                && client.Status == (int)Enums.Status.Normal
                                && company.Status == (int)Enums.Status.Normal
                            select new DecNoticeListModel
                            {
                                DecNoticeID = declarationNotice.ID,
                                OrderID = order.ID,
                                CreateDate = declarationNotice.CreateDate,
                                Currency = order.Currency,
                                ClientID = client.ID,
                                ClientName = company.Name,
                                CompanyID = company.ID,
                            };

            var decNoticeLists = from baseList in baseLists
                                 join decNoticeVoyage in decNoticeVoyages on new
                                 {
                                     DecNoticeID = baseList.DecNoticeID,
                                     DecNoticeVoyageStatus = (int)Enums.Status.Normal
                                 } equals new
                                 {
                                     DecNoticeID = decNoticeVoyage.DecNoticeID,
                                     DecNoticeVoyageStatus = decNoticeVoyage.Status
                                 } into decNoticeVoyages2
                                 from decNoticeVoyage in decNoticeVoyages2.DefaultIfEmpty()
                                 join voyage in voyages on new
                                 {
                                     VoyageID = decNoticeVoyage.VoyageID,
                                     VoyageStatus = (int)Enums.Status.Normal
                                 } equals new
                                 {
                                     VoyageID = voyage.ID,
                                     VoyageStatus = voyage.Status
                                 } into voyages2
                                 from voyage in voyages2.DefaultIfEmpty()
                                 join orderVoyage in orderVoyages on new
                                 {
                                     OrderID = baseList.OrderID,
                                     OrderVoyageType = (int)Enums.OrderSpecialType.CharterBus,
                                     OrderVoyageStatus = (int)Enums.Status.Normal,
                                 }
                                 equals new
                                 {
                                     OrderID = orderVoyage.OrderID,
                                     OrderVoyageType = orderVoyage.Type,
                                     OrderVoyageStatus = orderVoyage.Status,
                                 }
                                 into orderVoyagesCharterBus
                                 from orderVoyageCharterBus in orderVoyagesCharterBus.DefaultIfEmpty()
                                 join orderVoyage in orderVoyages on new
                                 {
                                     OrderID = baseList.OrderID,
                                     OrderVoyageType = (int)Enums.OrderSpecialType.HighValue,
                                     OrderVoyageStatus = (int)Enums.Status.Normal,
                                 }
                                 equals new
                                 {
                                     OrderID = orderVoyage.OrderID,
                                     OrderVoyageType = orderVoyage.Type,
                                     OrderVoyageStatus = orderVoyage.Status,
                                 }
                                 into orderVoyagesHighValue
                                 from orderVoyageHighValue in orderVoyagesHighValue.DefaultIfEmpty()

                                 join orderVoyage in orderVoyages on new
                                 {
                                     OrderID = baseList.OrderID,
                                     OrderVoyageType = (int)Enums.OrderSpecialType.Inspection,
                                     OrderVoyageStatus = (int)Enums.Status.Normal,
                                 }
                                 equals new
                                 {
                                     OrderID = orderVoyage.OrderID,
                                     OrderVoyageType = orderVoyage.Type,
                                     OrderVoyageStatus = orderVoyage.Status,
                                 }
                                 into orderVoyagesInspection
                                 from orderVoyageInspection in orderVoyagesInspection.DefaultIfEmpty()

                                 join orderVoyage in orderVoyages on new
                                 {
                                     OrderID = baseList.OrderID,
                                     OrderVoyageType = (int)Enums.OrderSpecialType.Quarantine,
                                     OrderVoyageStatus = (int)Enums.Status.Normal,
                                 }
                                 equals new
                                 {
                                     OrderID = orderVoyage.OrderID,
                                     OrderVoyageType = orderVoyage.Type,
                                     OrderVoyageStatus = orderVoyage.Status,
                                 }
                                 into orderVoyagesQuarantine
                                 from orderVoyageQuarantine in orderVoyagesQuarantine.DefaultIfEmpty()

                                 join orderVoyage in orderVoyages on new
                                 {
                                     OrderID = baseList.OrderID,
                                     OrderVoyageType = (int)Enums.OrderSpecialType.CCC,
                                     OrderVoyageStatus = (int)Enums.Status.Normal,
                                 }
                                 equals new
                                 {
                                     OrderID = orderVoyage.OrderID,
                                     OrderVoyageType = orderVoyage.Type,
                                     OrderVoyageStatus = orderVoyage.Status,
                                 }
                                 into orderVoyagesCCC
                                 from orderVoyageCCC in orderVoyagesCCC.DefaultIfEmpty()
                                 orderby baseList.CreateDate descending
                                 select new Models.DecNoticeListModel()
                                 {
                                     DecNoticeID = baseList.DecNoticeID,
                                     OrderID = baseList.OrderID,
                                     CreateDate = baseList.CreateDate,
                                     Currency = baseList.Currency,
                                     ClientID = baseList.ClientID,
                                     ClientName = baseList.ClientName,
                                     CompanyID = baseList.CompanyID,

                                     VoyageID = decNoticeVoyage.VoyageID,
                                     VoyageType = (voyage.Type != null) ? voyage.Type : 0,
                                     IsCharterBus = orderVoyageCharterBus != null,
                                     IsHighValue = orderVoyageHighValue != null,
                                     IsInspection = orderVoyageInspection != null,
                                     IsQuarantine = orderVoyageQuarantine != null,
                                     IsCCC = orderVoyageCCC != null,
                                 };


            foreach (var expression in expressions)
            {
                decNoticeLists = decNoticeLists.Where(expression as Expression<Func<Needs.Ccs.Services.Models.DecNoticeListModel, bool>>);
            }

            return decNoticeLists;
        }

        private IQueryable<Models.DecNoticeListModel> GetList(int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            var decNoticeLists = GetCommon(pageIndex, pageSize, expressions);
            decNoticeLists = decNoticeLists.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            var declarationNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>();
            //var sortings = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var deliveriesTopViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclaresTopView>();

            var clientAdminsView = new ClientAdminsView(this.Reponsitory);

            var icgooOrderMaps = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>();


            var totalDeclarePriceSumLists = from decNoticeList in decNoticeLists
                                            join orderItem in orderItems
                                                on new { decNoticeList.OrderID, OrderItemStatus = (int)Enums.Status.Normal }
                                                equals new { orderItem.OrderID, OrderItemStatus = orderItem.Status, }
                                            group new { decNoticeList, orderItem } by new { decNoticeList.DecNoticeID } into g
                                            select new Models.DecNoticeListModel()
                                            {
                                                DecNoticeID = g.Key.DecNoticeID,
                                                TotalDeclarePrice = g.Sum(t => t.orderItem.TotalPrice),
                                                TotalQty = g.Sum(t => t.orderItem.Quantity),
                                            };


            var totalQtySumLists = from decNoticeList in decNoticeLists
                                   join declarationNoticeItem in declarationNoticeItems
                                        on new { DecNoticeID = decNoticeList.DecNoticeID, }
                                        equals new { DecNoticeID = declarationNoticeItem.DeclarationNoticeID, }
                                   join sorting in deliveriesTopViews
                                        on new
                                        {
                                            SortingID = declarationNoticeItem.SortingID,
                                            //SortingStatus = (int)Enums.Status.Normal,
                                        }
                                        equals new
                                        {
                                            SortingID = sorting.UnqiueID,
                                            // SortingStatus = sorting.Status
                                        }
                                   join orderItem in orderItems
                                        on new { OrderItemID = sorting.ItemID, OrderItemStatus = (int)Enums.Status.Normal }
                                        equals new { OrderItemID = orderItem.ID, OrderItemStatus = orderItem.Status }
                                   group new { decNoticeList, sorting } by new { decNoticeList.DecNoticeID } into g
                                   select new Models.DecNoticeListModel()
                                   {
                                       DecNoticeID = g.Key.DecNoticeID,
                                       TotalModelQty = g.Count(t => t.sorting.UnqiueID != null),
                                       //TotalQty = g.Sum(t => t.orderItem.Quantity),
                                       TotalGrossWeight = g.Sum(t => t.sorting.Weight == null ? 0 : t.sorting.Weight.Value),
                                   };

            var data = from decNoticeList in decNoticeLists
                       join totalDeclarePriceSumList in totalDeclarePriceSumLists
                            on decNoticeList.DecNoticeID equals totalDeclarePriceSumList.DecNoticeID into totalDeclarePriceSumList2
                       from totalDeclarePriceSumList in totalDeclarePriceSumList2.DefaultIfEmpty()
                       join totalQtySumList in totalQtySumLists
                            on decNoticeList.DecNoticeID equals totalQtySumList.DecNoticeID into totalQtySumLists2
                       from totalQtySumList in totalQtySumLists2.DefaultIfEmpty()

                       join clientAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.Merchandiser && t.Status == Enums.Status.Normal)
                            on decNoticeList.ClientID equals clientAdmin.ClientID into clientAdmins
                       from cadmin in clientAdmins.DefaultIfEmpty()

                       join icgooOrderMap in icgooOrderMaps
                            on new { OrderID = decNoticeList.OrderID, IcgooOrderMapStatus = (int)Enums.Status.Normal, }
                            equals new { OrderID = icgooOrderMap.OrderID, IcgooOrderMapStatus = icgooOrderMap.Status, } into icgooOrderMaps2
                       from icgooOrderMap in icgooOrderMaps2.DefaultIfEmpty()

                       select new Models.DecNoticeListModel()
                       {
                           DecNoticeID = decNoticeList.DecNoticeID,
                           OrderID = decNoticeList.OrderID,
                           CreateDate = decNoticeList.CreateDate,
                           Currency = decNoticeList.Currency,
                           ClientID = decNoticeList.ClientID,
                           ClientName = decNoticeList.ClientName,
                           CompanyID = decNoticeList.CompanyID,

                           VoyageID = decNoticeList.VoyageID,
                           VoyageType = decNoticeList.VoyageType,
                           IsCharterBus = decNoticeList.IsCharterBus,
                           IsHighValue = decNoticeList.IsHighValue,
                           IsInspection = decNoticeList.IsInspection,
                           IsQuarantine = decNoticeList.IsQuarantine,
                           IsCCC = decNoticeList.IsCCC,

                           TotalDeclarePrice = totalDeclarePriceSumList.TotalDeclarePrice,

                           TotalModelQty = totalQtySumList.TotalModelQty,
                           TotalQty = totalDeclarePriceSumList.TotalQty,
                           TotalGrossWeight = totalQtySumList.TotalGrossWeight,

                           DeclarantName = cadmin.Admin.RealName,

                           IcgooOrder = icgooOrderMap.IcgooOrder,
                       };

            return data;
        }

        private int GetCount(int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            return GetCommon(pageIndex, pageSize, expressions).Count();
        }

        private IQueryable<Models.DecNoticeListModel> GetBoxIndexs(int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            var decNoticeLists = GetCommon(pageIndex, pageSize, expressions);
            decNoticeLists = decNoticeLists.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            var declarationNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>();
            var deliveriesTopViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclaresTopView>();

            var boxIndexLists = from decNoticeList in decNoticeLists
                                join declarationNoticeItem in declarationNoticeItems
                                     on new { DecNoticeID = decNoticeList.DecNoticeID, }
                                     equals new { DecNoticeID = declarationNoticeItem.DeclarationNoticeID, }
                                join sorting in deliveriesTopViews
                                     on new
                                     {
                                         SortingID = declarationNoticeItem.SortingID,
                                         //SortingStatus = (int)Enums.Status.Normal,
                                     }
                                     equals new
                                     {
                                         SortingID = sorting.UnqiueID,
                                         //SortingStatus = sorting.Status
                                     }
                                group new { decNoticeList, sorting } by new { decNoticeList.DecNoticeID, sorting.BoxCode } into g
                                select new Models.DecNoticeListModel()
                                {
                                    DecNoticeID = g.Key.DecNoticeID,
                                    BoxIndex = g.Key.BoxCode,
                                };

            return from decNoticeList in decNoticeLists
                   join boxIndexList in boxIndexLists
                        on decNoticeList.DecNoticeID equals boxIndexList.DecNoticeID into boxIndexList2
                   from boxIndexList in boxIndexList2.DefaultIfEmpty()
                   select new Models.DecNoticeListModel()
                   {
                       DecNoticeID = decNoticeList.DecNoticeID,
                       BoxIndex = boxIndexList.BoxIndex,
                   };
        }

        public IEnumerable<Models.DecNoticeListModel> GetResult(out int totalCount, int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            var decNoticeList = GetList(pageIndex, pageSize, expressions).ToList();
            var count = GetCount(pageIndex, pageSize, expressions);
            var boxIndexList = GetBoxIndexs(pageIndex, pageSize, expressions).ToList();

            totalCount = count;

            return from declareNotice in decNoticeList
                   select new Models.DecNoticeListModel()
                   {
                       DecNoticeID = declareNotice.DecNoticeID,
                       OrderID = declareNotice.OrderID,
                       ClientName = declareNotice.ClientName,
                       CompanyID = declareNotice.CompanyID,
                       ClientID = declareNotice.ClientID,
                       CreateDate = declareNotice.CreateDate,
                       DeclarantName = declareNotice.DeclarantName,
                       OrderSpecialTypeName = CalcOrderSpecialTypeName(declareNotice),
                       VoyageID = declareNotice.VoyageID,
                       VoyageTypeName = (declareNotice.VoyageType != 0) ? ((Needs.Ccs.Services.Enums.VoyageType)declareNotice.VoyageType).GetDescription() : string.Empty,
                       PackNo = CalcPackNo(declareNotice.DecNoticeID, boxIndexList),
                       TotalDeclarePrice = declareNotice.TotalDeclarePrice,
                       Currency = declareNotice.Currency,
                       TotalQty = declareNotice.TotalQty,
                       TotalModelQty = declareNotice.TotalModelQty,
                       TotalGrossWeight = declareNotice.TotalGrossWeight,
                       IcgooOrder = declareNotice.IcgooOrder,
                   };
        }

        private string CalcOrderSpecialTypeName(Needs.Ccs.Services.Models.DecNoticeListModel decNoticeListModel)
        {
            //StringBuilder sb = new StringBuilder();
            //if (decNoticeListModel.IsCharterBus)
            //{
            //    sb.Append(Enums.OrderSpecialType.CharterBus.GetDescription() + "/");
            //}
            //else
            //{
            //    sb.Append("-/");
            //}
            //if (decNoticeListModel.IsHighValue)
            //{
            //    sb.Append(Enums.OrderSpecialType.HighValue.GetDescription() + "/");
            //}
            //else
            //{
            //    sb.Append("-/");
            //}
            //if (decNoticeListModel.IsInspection)
            //{
            //    sb.Append(Enums.OrderSpecialType.Inspection.GetDescription() + "/");
            //}
            //else
            //{
            //    sb.Append("-/");
            //}
            //if (decNoticeListModel.IsQuarantine)
            //{
            //    sb.Append(Enums.OrderSpecialType.Quarantine.GetDescription() + "/");
            //}
            //else
            //{
            //    sb.Append("-/");
            //}
            //if (decNoticeListModel.IsCCC)
            //{
            //    sb.Append(Enums.OrderSpecialType.CCC.GetDescription() + "/");
            //}
            //else
            //{
            //    sb.Append("-/");
            //}

            //return sb.ToString().Trim('/');

            List<Enum> orderSpecialTypeEnumsList = new List<Enum>();
            if (decNoticeListModel.IsCharterBus)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.CharterBus);
            }

            if (decNoticeListModel.IsHighValue)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.HighValue);
            }

            if (decNoticeListModel.IsInspection)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.Inspection);
            }

            if (decNoticeListModel.IsQuarantine)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.Quarantine);
            }

            if (decNoticeListModel.IsCCC)
            {
                orderSpecialTypeEnumsList.Add(Enums.OrderSpecialType.CCC);
            }

            Enum[] orderSpecialTypeEnumsArray = orderSpecialTypeEnumsList.ToArray();

            return orderSpecialTypeEnumsArray.GetEnumsDescriptions("|");
        }

        private int CalcPackNo(string decNoticeID, List<Needs.Ccs.Services.Models.DecNoticeListModel> boxIndexList)
        {
            //计算件数             
            var sumPacks = 0;
            var alonePack = boxIndexList.Where(t => t.DecNoticeID == decNoticeID).Select(t => t.BoxIndex).Distinct().ToList();
            alonePack.Remove(null);
            var multi = alonePack.Where(a => a.IndexOf('-') > -1).Select(a => a.ToUpper()).Distinct().ToList();
            var sumRepeat = 0;

            multi.ForEach(a =>
            {
                try
                {
                    var arry = a.Split('-');
                    //型号A 装了WL01；型号B装了 WL01-WL05；这种件数是5件;
                    //WL08 和WL05-WL10 同时存在  
                    int startCase = int.Parse(arry[0].Replace("HXT", ""));
                    int endCase = int.Parse(arry[1].Replace("HXT", ""));
                    var repeat = alonePack.Where(c => !c.Contains("-")).Where(c => int.Parse(c.ToUpper().Replace("HXT", "")) >= startCase && int.Parse(c.ToUpper().Replace("HXT", "")) <= endCase).Count();
                    sumRepeat += repeat;
                    sumPacks += endCase - startCase + 1;
                }
                catch (Exception ex)
                {

                }
            });

            sumPacks += alonePack.Count() - multi.Count() - sumRepeat;
            return sumPacks;
        }
    }

    /// <summary>
    /// 报关通知，待制单，装箱单
    /// </summary>
    public class PackingListView : UniqueView<Models.PackingListModel, ScCustomsReponsitory>
    {
        public PackingListView()
        {

        }

        internal PackingListView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.PackingListModel> GetIQueryable()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Models.PackingListModel> GetResult(string orderId)
        {
            var sortings = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();
            //var sortings = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclaresTopView>();
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            //var products = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Products>();
            var orderItemCategories = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();

            var result = from sorting in sortings
                         join orderItem in orderItems
                            on new
                            {
                                OrderItemID = sorting.OrderItemID,
                                OrderID = sorting.OrderID,
                                OrderItemsStatus = (int)Enums.Status.Normal,
                                SortingStatus = sorting.Status,
                            }
                            equals new
                            {
                                OrderItemID = orderItem.ID,
                                OrderID = orderId,
                                OrderItemsStatus = orderItem.Status,
                                SortingStatus = (int)Enums.Status.Normal,
                            }
                            //join product in products on orderItem.ProductID equals product.ID

                         join orderItemCategory in orderItemCategories
                             on new { OrderItemID = orderItem.ID, OrderItemCategoriesStatus = (int)Enums.Status.Normal, }
                             equals new { OrderItemID = orderItemCategory.OrderItemID, OrderItemCategoriesStatus = orderItemCategory.Status, }
                             into orderItemCategories2
                         from orderItemCategory in orderItemCategories2.DefaultIfEmpty()

                         orderby sorting.BoxIndex
                         select new Models.PackingListModel()
                         {
                             OrderID = sorting.OrderID,
                             BoxIndex = sorting.BoxIndex,
                             ProductName = orderItemCategory.Name,
                             Model = orderItem.Model,
                             OrderItemCategoryType = orderItemCategory.Type,
                             Quantity = sorting.Quantity,
                             TotalPrice = orderItem.TotalPrice,
                             NetWeight = sorting.NetWeight,
                             GrossWeight = sorting.GrossWeight,
                             Origin = orderItem.Origin,
                             HSCode = orderItemCategory.HSCode,
                         };

            return result;
        }
    }
}
