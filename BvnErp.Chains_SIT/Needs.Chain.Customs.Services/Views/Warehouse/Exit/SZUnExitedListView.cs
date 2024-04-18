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
    /// 深证库房 出库通知 待出库 View
    /// </summary>
    public class SZUnExitedListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public SZUnExitedListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public SZUnExitedListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public class SZUnExitedListModel
        {
            /// <summary>
            /// ExitNoticeID
            /// </summary>
            public string ExitNoticeID { get; set; } = string.Empty;

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
            public string ClientName { get; set; } = string.Empty;

            /// <summary>
            /// 件数
            /// </summary>
            public int PackNo { get; set; }

            /// <summary>
            /// 送货类型
            /// </summary>
            public Enums.ExitType ExitType { get; set; }

            /// <summary>
            /// 制单人
            /// </summary>
            public string AdminName { get; set; } = string.Empty;

            /// <summary>
            /// 出库状态
            /// </summary>
            public Enums.ExitNoticeStatus ExitNoticeStatus { get; set; }

            /// <summary>
            /// 打印状态
            /// Enums.IsPrint
            /// </summary>
            public int? IsPrint { get; set; }

            /// <summary>
            /// ExitNotice 生成时间，即制单时间
            /// </summary>
            public DateTime CreateDate { get; set; }
        }

        private IEnumerable<SZUnExitedListModel> GetCommon(LambdaExpression[] expressions)
        {
            var exitNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                .Where(t => t.ExitNoticeStatus == (int)Enums.ExitNoticeStatus.UnExited
                         || t.ExitNoticeStatus == (int)Enums.ExitNoticeStatus.Exiting);
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var adminsTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var voyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>()
                .Where(t => t.CutStatus != (int)Enums.CutStatus.UnCutting
                         && t.Status == (int)Enums.Status.Normal);

            var linq = from exitNotice in exitNotices
                       join order in orders
                          on new
                          {
                              OrderID = exitNotice.OrderID,
                              WarehouseType = exitNotice.WarehouseType,
                              ExitNoticeDataStatus = exitNotice.Status,
                              OrderDataStatus = (int)Enums.Status.Normal,
                          }
                          equals new
                          {
                              OrderID = order.ID,
                              WarehouseType = (int)Enums.WarehouseType.ShenZhen,
                              ExitNoticeDataStatus = (int)Enums.Status.Normal,
                              OrderDataStatus = order.Status,
                          }
                       join client in clients
                           on new { ClientID = order.ClientID, ClientStatus = (int)Enums.Status.Normal, }
                           equals new { ClientID = client.ID, ClientStatus = client.Status, }
                       join company in companies
                          on new { CompanyID = client.CompanyID, CompanyStatus = (int)Enums.Status.Normal, }
                          equals new { CompanyID = company.ID, CompanyStatus = company.Status, }
                       join admin in adminsTopView
                          on new { AdminID = exitNotice.AdminID, AdminStatus = (int)Enums.Status.Normal, }
                          equals new { AdminID = admin.ID, AdminStatus = admin.Status, }
                       join decHead in decHeads
                          on new { OrderID = exitNotice.OrderID, IsSuccess = true, }
                          equals new { OrderID = decHead.OrderID, IsSuccess = decHead.IsSuccess, }
                       join voyage in voyages
                          on new { VoyNo = decHead.VoyNo, }
                          equals new { VoyNo = voyage.ID, }

                       orderby exitNotice.CreateDate descending
                       select new SZUnExitedListModel
                       {
                           ExitNoticeID = exitNotice.ID,
                           OrderID = order.ID,
                           ClientCode = client.ClientCode,
                           ClientName = company.Name,
                           ExitType = (Enums.ExitType)exitNotice.ExitType,
                           AdminName = admin.RealName,
                           ExitNoticeStatus = (Enums.ExitNoticeStatus)exitNotice.ExitNoticeStatus,
                           IsPrint = exitNotice.IsPrint,
                           CreateDate = exitNotice.CreateDate,
                       };

            foreach (var expression in expressions)
            {
                linq = linq.Where(expression as Expression<Func<SZUnExitedListModel, bool>>);
            }

            return linq;
        }

        private IEnumerable<SZUnExitedListModel> GetList(int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var exitDelivers = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitDelivers>();

            var commons = GetCommon(expressions);
            commons = commons.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            var linq = from common in commons
                       join exitDeliver in exitDelivers
                            on new { ExitNoticeID = common.ExitNoticeID, ExitDeliverStatus = (int)Enums.Status.Normal, }
                            equals new { ExitNoticeID = exitDeliver.ExitNoticeID, ExitDeliverStatus = exitDeliver.Status, }
                            into exitDelivers2
                       from exitDeliver in exitDelivers2.DefaultIfEmpty()

                       select new SZUnExitedListModel
                       {
                           ExitNoticeID = common.ExitNoticeID,
                           OrderID = common.OrderID,
                           ClientCode = common.ClientCode,
                           ClientName = common.ClientName,
                           ExitType = common.ExitType,
                           AdminName = common.AdminName,
                           ExitNoticeStatus = common.ExitNoticeStatus,
                           IsPrint = common.IsPrint,
                           CreateDate = common.CreateDate,

                           PackNo = exitDeliver.PackNo,
                       };

            return linq;
        }

        public IEnumerable<SZUnExitedListModel> GetResult(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            totalCount = GetCommon(expressions).Count();

            var resultList = GetList(pageIndex, pageSize, expressions).ToList();

            return resultList;
        }

    }
}
