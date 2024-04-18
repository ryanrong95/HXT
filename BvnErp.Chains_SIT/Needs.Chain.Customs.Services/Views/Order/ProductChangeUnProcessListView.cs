using Layer.Data.Sqls;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 产品变更 待处理 列表 View
    /// </summary>
    public class ProductChangeUnProcessListView
    {
        private ScCustomsReponsitory _reponsitory { get; set; }

        public ProductChangeUnProcessListView()
        {
            _reponsitory = new ScCustomsReponsitory();
        }

        public ProductChangeUnProcessListView(ScCustomsReponsitory reponsitory)
        {
            this._reponsitory = reponsitory;
        }

        /// <summary>
        /// 产品变更 待处理 列表 Model
        /// </summary>
        [Serializable]
        public class ProductChangeUnProcessListModel
        {
            /// <summary>
            /// OrderItemID
            /// </summary>
            public string OrderItemID { get; set; } = string.Empty;

            /// <summary>
            /// OrderID
            /// </summary>
            public string OrderID { get; set; } = string.Empty;

            /// <summary>
            /// 客户编号
            /// </summary>
            public string ClientCode { get; set; } = string.Empty;

            /// <summary>
            /// 客户名称
            /// </summary>
            public string CompanyName { get; set; } = string.Empty;

            /// <summary>
            /// 品名
            /// </summary>
            public string ProductName { get; set; } = string.Empty;

            /// <summary>
            /// 型号
            /// </summary>
            public string ProductModel { get; set; } = string.Empty;

            /// <summary>
            /// 类型
            /// </summary>
            public Enums.OrderItemChangeType Type { get; set; } = Enums.OrderItemChangeType.None;

            /// <summary>
            /// 添加时间
            /// </summary>
            public DateTime CreateDate { get; set; } = new DateTime(1900, 1, 1);

            /// <summary>
            /// 处理状态
            /// </summary>
            public Enums.ProcessState ProcessState { get; set; } = Enums.ProcessState.UnProcess;

            /// <summary>
            /// 是否被锁定
            /// </summary>
            public bool IsLocked { get; set; }

            /// <summary>
            /// 锁定人ID
            /// </summary>
            public string LockerID { get; set; } = string.Empty;

            /// <summary>
            /// 锁定人姓名
            /// </summary>
            public string LockerName { get; set; } = string.Empty;

            /// <summary>
            /// 锁定时间
            /// </summary>
            public DateTime? LockTime { get; set; }

            public long RowNumber { get; set; }

            public string Types { get; set; } = string.Empty;
        }

        /// <summary>
        /// 在 OrderItemChangeNotices 表中获取需要的 OrderItemID
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        private IEnumerable<ProductChangeUnProcessListModel> GetNeedOrderItemIDTable()
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(@"
                    SELECT   ROW_NUMBER() OVER ( PARTITION BY OrderItemChangeNotices.OrderItemID ORDER BY OrderItemChangeNotices.CreateDate ASC ) AS RowNumber ,
                            OrderItemChangeNotices.OrderItemID,
                            OrderItemChangeNotices.CreateDate
                    FROM     dbo.OrderItemChangeNotices
                            INNER JOIN dbo.OrderItems ON OrderItemChangeNotices.OrderItemID = OrderItems.ID
                                                            AND OrderItemChangeNotices.Status = 200
                                                            AND OrderItems.Status = 200
                                                            AND OrderItemChangeNotices.ProcessStatus = 1  ");


            return this._reponsitory.Query<ProductChangeUnProcessListModel>(sbSql.ToString());
        }

        /// <summary>
        /// 获取 OrderItemChangeNotices 表中目标的 OrderItemID
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        private IEnumerable<ProductChangeUnProcessListModel> GetTargetOrderItemIDTable(params Func<ProductChangeUnProcessListModel, bool>[] funcs)
        {
            var orderItems = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orders = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();

            var needOrderItemIDTable = GetNeedOrderItemIDTable();

            var targetOrderItemIDTable = from needOrderItemID in needOrderItemIDTable
                                         join orderItem in orderItems
                                         on new { OrderItemID = needOrderItemID.OrderItemID, OrderItemStatus = (int)Enums.Status.Normal, }
                                         equals new { OrderItemID = orderItem.ID, OrderItemStatus = orderItem.Status, }

                                         join order in orders
                                         on new { OrderID = orderItem.OrderID, OrderStatus = (int)Enums.Status.Normal, }
                                         equals new { OrderID = order.ID, OrderStatus = order.Status, } into orders2
                                         from order in orders2.DefaultIfEmpty()

                                         join client in clients
                                                on new { ClientID = order.ClientID, ClientStatus = (int)Enums.Status.Normal, }
                                                equals new { ClientID = client.ID, ClientStatus = client.Status, } into clients2
                                         from client in clients2.DefaultIfEmpty()

                                         where needOrderItemID.RowNumber == 1
                                         select new ProductChangeUnProcessListModel()
                                         {
                                             OrderItemID = needOrderItemID.OrderItemID,
                                             CreateDate = needOrderItemID.CreateDate,
                                             ClientCode = client.ClientCode,
                                             OrderID = order.ID,
                                             
                                         };

            foreach (var func in funcs)
            {
                targetOrderItemIDTable = targetOrderItemIDTable.Where(func);
            }

            return targetOrderItemIDTable;
        }

        /// <summary>
        /// 获取数据结果
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        private IEnumerable<ProductChangeUnProcessListModel> GetDataResult(int pageIndex, int pageSize, params Func<ProductChangeUnProcessListModel, bool>[] funcs)
        {
            var targetOrderItemIDTable = GetTargetOrderItemIDTable(funcs);

            targetOrderItemIDTable = targetOrderItemIDTable.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            IEnumerable<string> targetOrderItemIDs = targetOrderItemIDTable.Select(t => t.OrderItemID);

            var orderItemChangeNotices = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>();
            var orderItems = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();

            var orders = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var orderItemCategories = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();
            var productClassifyLocks = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyLocks>();
            var adminsTopView = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

            orderItemChangeNotices = orderItemChangeNotices.Where(t => targetOrderItemIDs.Contains(t.OrderItemID)
                                                                    && t.Status == (int)Enums.Status.Normal
                                                                    && t.ProcessStatus == (int)Enums.ProcessState.UnProcess);

            var result = from orderItemChangeNotice in orderItemChangeNotices
                         join orderItem in orderItems
                                on new { OrderItemID = orderItemChangeNotice.OrderItemID, OrderItemStatus = (int)Enums.Status.Normal, }
                                equals new { OrderItemID = orderItem.ID, OrderItemStatus = orderItem.Status, }

                         join order in orders
                                 on new { OrderID = orderItem.OrderID, OrderStatus = (int)Enums.Status.Normal, }
                                 equals new { OrderID = order.ID, OrderStatus = order.Status, } into orders2
                         from order in orders2.DefaultIfEmpty()

                         join client in clients
                                on new { ClientID = order.ClientID, ClientStatus = (int)Enums.Status.Normal, }
                                equals new { ClientID = client.ID, ClientStatus = client.Status, } into clients2
                         from client in clients2.DefaultIfEmpty()

                         join company in companies
                                on new { CompanyID = client.CompanyID, CompanyStatus = (int)Enums.Status.Normal, }
                                equals new { CompanyID = company.ID, CompanyStatus = company.Status, } into companies2
                         from company in companies2.DefaultIfEmpty()

                         join orderItemCategory in orderItemCategories
                                on new { OrderItemID = orderItemChangeNotice.OrderItemID, OrderItemCategoryStatus = (int)Enums.Status.Normal, }
                                equals new { OrderItemID = orderItemCategory.OrderItemID, OrderItemCategoryStatus = orderItemCategory.Status, } into orderItemCategories2
                         from orderItemCategory in orderItemCategories2.DefaultIfEmpty()

                         join productClassifyLock in productClassifyLocks on orderItemChangeNotice.OrderItemID equals productClassifyLock.ID into productClassifyLocks2
                         from productClassifyLock in productClassifyLocks2.DefaultIfEmpty()

                         join admin in adminsTopView
                                on new { AdminID = productClassifyLock.AdminID, AdminsTopViewStatus = (int)Enums.Status.Normal, }
                                equals new { AdminID = admin.ID, AdminsTopViewStatus = admin.Status, } into adminsTopView2
                         from admin in adminsTopView2.DefaultIfEmpty()

                         select new ProductChangeUnProcessListModel()
                         {
                             OrderID = orderItem.OrderID,
                             OrderItemID = orderItemChangeNotice.OrderItemID,
                             ClientCode = client.ClientCode,
                             CompanyName = company.Name,
                             ProductName = orderItemCategory.Name,
                             ProductModel = orderItem.Model,
                             Type = (Enums.OrderItemChangeType)orderItemChangeNotice.Type,
                             CreateDate = orderItemChangeNotice.CreateDate,
                             IsLocked = productClassifyLock != null ? productClassifyLock.IsLocked : false,
                             LockerID = productClassifyLock.AdminID,
                             LockerName = admin.RealName,
                             LockTime = productClassifyLock.LockDate,
                         };

            return result;
        }

        /// <summary>
        /// 获取结果(合并数据)
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public IEnumerable<ProductChangeUnProcessListModel> GetResult(out int totalCount, int pageIndex, int pageSize, params Func<ProductChangeUnProcessListModel, bool>[] funcs)
        {
            List<ProductChangeUnProcessListModel> listProductChangeUnProcess = GetDataResult(pageIndex, pageSize, funcs).ToList();

            List<ProductChangeUnProcessListModel> listResult = (from productChangeUnProcess in listProductChangeUnProcess
                                                                group productChangeUnProcess by new
                                                                {
                                                                    productChangeUnProcess.OrderID,
                                                                    productChangeUnProcess.OrderItemID,
                                                                    productChangeUnProcess.ClientCode,
                                                                    productChangeUnProcess.CompanyName,
                                                                    productChangeUnProcess.ProductName,
                                                                    productChangeUnProcess.ProductModel,
                                                                    productChangeUnProcess.IsLocked,
                                                                    productChangeUnProcess.LockerID,
                                                                    productChangeUnProcess.LockerName,
                                                                    productChangeUnProcess.LockTime,

                                                                } into g
                                                                select new ProductChangeUnProcessListModel()
                                                                {
                                                                    OrderID = g.Key.OrderID,
                                                                    OrderItemID = g.Key.OrderItemID,
                                                                    ClientCode = g.Key.ClientCode,
                                                                    CompanyName = g.Key.CompanyName,
                                                                    ProductName = g.Key.ProductName,
                                                                    ProductModel = g.Key.ProductModel,
                                                                    IsLocked = g.Key.IsLocked,
                                                                    LockerID = g.Key.LockerID,
                                                                    LockerName = g.Key.LockerName,
                                                                    LockTime = g.Key.LockTime,
                                                                }).ToList();

            foreach (var result in listResult)
            {
                var listTheUnProcess = listProductChangeUnProcess.Where(t => t.OrderItemID == result.OrderItemID).ToList();

                string[] types = listTheUnProcess.OrderBy(t => t.Type).Select(t => t.Type.GetDescription()).ToArray();
                result.Types = string.Join("|", types);

                result.CreateDate = listTheUnProcess.OrderBy(t => t.CreateDate).FirstOrDefault().CreateDate;
            }

            listResult = listResult.OrderBy(t => t.CreateDate).ToList();




            var targetOrderItemIDTable = GetTargetOrderItemIDTable(funcs);
            totalCount = targetOrderItemIDTable.Count();



            return listResult;
        }
    }
}
