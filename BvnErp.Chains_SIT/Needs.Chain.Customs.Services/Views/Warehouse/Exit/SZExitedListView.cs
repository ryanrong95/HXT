using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 深证已出库列表
    /// </summary>
    public class SZExitedListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public SZExitedListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public SZExitedListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public class SZExitedListModel
        {
            /// <summary>
            /// 出库通知 ID
            /// </summary>
            public string ExitNoticeID { get; set; } = string.Empty;

            /// <summary>
            /// 订单编号
            /// </summary>
            public string OrderID { get; set; } = string.Empty;

            /// <summary>
            /// 客户编号
            /// </summary>
            public string ClientCode { get; set; } = string.Empty;

            /// <summary>
            /// 客户名称
            /// </summary>
            public string ClientName { get; set; } = string.Empty;

            /// <summary>
            /// 件数
            /// </summary>
            public int PackNo { get; set; }

            /// <summary>
            /// 制单人
            /// </summary>
            public string AdminName { get; set; } = string.Empty;

            /// <summary>
            /// 送货类型
            /// </summary>
            public Enums.ExitType ExitType { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateDate { get; set; }

            /// <summary>
            /// 出库通知状态
            /// </summary>
            public Enums.ExitNoticeStatus ExitNoticeStatus { get; set; }

            /// <summary>
            /// AdminID in ExitNotice
            /// </summary>
            public string ExitNoticeAdminID { get; set; } = string.Empty;

            /// <summary>
            /// 是否有客户确认单文件
            /// </summary>
            public bool IsHasReceiptConfirmationFile { get; set; } = false;

            /// <summary>
            /// 出库时间
            /// </summary>
            public DateTime? OutStockTime { get; set; }
        }

        private IQueryable<SZExitedListModel> GetCommon(LambdaExpression[] expressions)
        {
            var exitNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                .Where(t => t.WarehouseType == (int)Enums.WarehouseType.ShenZhen
                         && t.ExitNoticeStatus >= (int)Enums.ExitNoticeStatus.Exited);

            //var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var linq = from exitNotice in exitNotices
                       join order in orders
                            on new
                            {
                                OrderID = exitNotice.OrderID,
                                ExitNoticeDataStatus = exitNotice.Status,
                                OrderDataStatus = (int)Enums.Status.Normal,
                                //ExitNoticeStatus = exitNotice.ExitNoticeStatus,
                                //WarehouseType = exitNotice.WarehouseType,
                            }
                            equals new
                            {
                                OrderID = order.ID,
                                ExitNoticeDataStatus = (int)Enums.Status.Normal,
                                OrderDataStatus = order.Status,
                                //ExitNoticeStatus = (int)Enums.ExitNoticeStatus.Exited,
                                //WarehouseType = (int)Enums.WarehouseType.ShenZhen,
                            }
                       join client in clients
                            on new { ClientID = order.ClientID, ClientStatus = (int)Enums.Status.Normal, }
                            equals new { ClientID = client.ID, ClientStatus = client.Status, }
                       join company in companies
                           on new { CompanyID = client.CompanyID, CompanyStatus = (int)Enums.Status.Normal, }
                           equals new { CompanyID = company.ID, CompanyStatus = company.Status, }
                       orderby exitNotice.CreateDate descending
                       select new SZExitedListModel
                       {
                           ExitNoticeID = exitNotice.ID,
                           OrderID = exitNotice.OrderID,
                           ClientCode = client.ClientCode,
                           ClientName = company.Name,
                           ExitType = (Enums.ExitType)exitNotice.ExitType,
                           CreateDate = exitNotice.CreateDate,
                           ExitNoticeStatus = (Enums.ExitNoticeStatus)exitNotice.ExitNoticeStatus,
                           ExitNoticeAdminID = exitNotice.AdminID,
                           OutStockTime = exitNotice.OutStockTime,
                       };

            foreach (var expression in expressions)
            {
                linq = linq.Where(expression as Expression<Func<SZExitedListModel, bool>>);
            }

            return linq;
        }

        private IQueryable<SZExitedListModel> GetList(int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var adminsTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();
            var exitDelivers = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitDelivers>();

            var commons = GetCommon(expressions);
            commons = commons.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            var linq = from common in commons
                       join admin in adminsTopView
                            on new { AdminID = common.ExitNoticeAdminID, AdminStatus = (int)Enums.Status.Normal, }
                            equals new { AdminID = admin.ID, AdminStatus = admin.Status, }
                            into adminsTopView2
                       from admin in adminsTopView2.DefaultIfEmpty()

                       join exitDeliver in exitDelivers
                           on new { ExitNoticeID = common.ExitNoticeID, ExitDeliverDataStatus = (int)Enums.Status.Normal, }
                           equals new { ExitNoticeID = exitDeliver.ExitNoticeID, ExitDeliverDataStatus = exitDeliver.Status, }
                           into exitDelivers2
                       from exitDeliver in exitDelivers2.DefaultIfEmpty()

                       orderby common.CreateDate descending
                       select new SZExitedListModel
                       {
                           ExitNoticeID = common.ExitNoticeID,
                           OrderID = common.OrderID,
                           ClientCode = common.ClientCode,
                           ClientName = common.ClientName,
                           ExitType = common.ExitType,
                           CreateDate = common.CreateDate,
                           ExitNoticeStatus = common.ExitNoticeStatus,
                           ExitNoticeAdminID = common.ExitNoticeAdminID,
                           OutStockTime = common.OutStockTime,

                           PackNo = exitDeliver.PackNo,
                           AdminName = admin.RealName,
                       };

            return linq;
        }

        public IEnumerable<SZExitedListModel> GetResult(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            totalCount = GetCommon(expressions).Count();

            var resultList = GetList(pageIndex, pageSize, expressions).ToList();

            //避免修改或错误数据导致的一个 出库通知对应多个文件的情况
            string[] exitNoticeIDs = resultList.Select(t => t.ExitNoticeID).ToArray();

            string[] theExistFilesExitNoticeIDs = (from exitNoticeFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>()
                                                   where exitNoticeIDs.Contains(exitNoticeFile.ExitNoticeID)
                                                      && exitNoticeFile.Status == (int)Enums.Status.Normal
                                                      && exitNoticeFile.FileType == (int)Enums.FileType.ReceiptConfirmationFile
                                                   select new
                                                   {
                                                       ExitNoticeID = exitNoticeFile.ExitNoticeID,
                                                   }).Select(t => t.ExitNoticeID).Distinct().ToArray();

            foreach (var result in resultList)
            {
                if (theExistFilesExitNoticeIDs.Contains(result.ExitNoticeID))
                {
                    result.IsHasReceiptConfirmationFile = true;
                }
            }

            return resultList;
        }
    }
}
