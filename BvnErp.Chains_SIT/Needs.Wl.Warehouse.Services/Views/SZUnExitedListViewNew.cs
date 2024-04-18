using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.Warehouse.Services.PageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Warehouse.Services.Views
{
    public class SZUnExitedListViewNew : View<SZUnExitedListViewNewModels, ScCustomsReponsitory>
    {
        public SZUnExitedListViewNew()
        {

        }

        protected override IQueryable<SZUnExitedListViewNewModels> GetIQueryable()
        {
            var exitNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                .Where(t => t.ExitNoticeStatus == (int)Needs.Wl.Models.Enums.ExitNoticeStatus.UnExited
                         || t.ExitNoticeStatus == (int)Needs.Wl.Models.Enums.ExitNoticeStatus.Exiting);
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var adminsTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var voyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>()
                .Where(t => t.CutStatus != (int)Needs.Wl.Models.Enums.CutStatus.UnCutting
                         && t.Status == (int)Needs.Wl.Models.Enums.Status.Normal);

            var exitDelivers = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitDelivers>();

            var exitNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>();
            var sortings = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();

            var linq2 = from exitNoticeItem in exitNoticeItems                 
                       join sorting in sortings on exitNoticeItem.SortingID equals sorting.ID                   
                       join decHead in decHeads
                          on new { OrderID = sorting.OrderID, IsSuccess = true, }
                          equals new { OrderID = decHead.OrderID, IsSuccess = decHead.IsSuccess, }
                       join voyage in voyages
                          on new { VoyNo = decHead.VoyNo, }
                          equals new { VoyNo = voyage.ID, }
                       select new
                       {
                           ExitNoticeID = exitNoticeItem.ExitNoticeID,                       
                       };

            var exit = linq2.Distinct();
            var filterExitNotices = from c in exit
                                    join exitNotice in exitNotices on c.ExitNoticeID equals exitNotice.ID                                   
                                    select new
                                    {
                                        ID = c.ExitNoticeID,
                                        OrderID = exitNotice.OrderID,
                                        WarehouseType = exitNotice.WarehouseType,
                                        Status = exitNotice.Status,
                                        AdminID = exitNotice.AdminID,
                                        ExitType = exitNotice.ExitType,
                                        ExitNoticeStatus = exitNotice.ExitNoticeStatus,
                                        IsPrint = exitNotice.IsPrint,
                                        CreateDate = exitNotice.CreateDate,
                                    };

            string[] mainOrderIDs = filterExitNotices.Select(t => t.OrderID).ToArray();

            var filterOrders = (from c in orders
                                where c.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                                   && mainOrderIDs.Contains(c.MainOrderId)
                                group c by new { c.MainOrderId } into g
                                select new
                                {
                                    MainOrderId = g.Key.MainOrderId, ///c.MainOrderId,
                                    Status = g.First().Status,
                                    ClientID = g.First().ClientID
                                });  //.Distinct();

            var linq = from exitNotice in filterExitNotices
                       join order in filterOrders
                          on new
                          {
                              OrderID = exitNotice.OrderID,
                              WarehouseType = exitNotice.WarehouseType,
                              ExitNoticeDataStatus = exitNotice.Status,
                              OrderDataStatus = (int)Needs.Wl.Models.Enums.Status.Normal,
                          }
                          equals new
                          {
                              OrderID = order.MainOrderId,
                              WarehouseType = (int)Needs.Wl.Models.Enums.WarehouseType.ShenZhen,
                              ExitNoticeDataStatus = (int)Needs.Wl.Models.Enums.Status.Normal,
                              OrderDataStatus = order.Status,
                          }
                       join client in clients
                           on new { ClientID = order.ClientID, ClientStatus = (int)Needs.Wl.Models.Enums.Status.Normal, }
                           equals new { ClientID = client.ID, ClientStatus = client.Status, }
                       join company in companies
                          on new { CompanyID = client.CompanyID, CompanyStatus = (int)Needs.Wl.Models.Enums.Status.Normal, }
                          equals new { CompanyID = company.ID, CompanyStatus = company.Status, }
                       join admin in adminsTopView
                          on new { AdminID = exitNotice.AdminID, AdminStatus = (int)Needs.Wl.Models.Enums.Status.Normal, }
                          equals new { AdminID = admin.ID, AdminStatus = admin.Status, }
                     

                       join exitDeliver in exitDelivers
                            on new { ExitNoticeID = exitNotice.ID, ExitDeliverStatus = (int)Needs.Wl.Models.Enums.Status.Normal, }
                            equals new { ExitNoticeID = exitDeliver.ExitNoticeID, ExitDeliverStatus = exitDeliver.Status, }
                            into exitDelivers2
                       from exitDeliver in exitDelivers2.DefaultIfEmpty()

                       orderby exitNotice.CreateDate descending
                       select new SZUnExitedListViewNewModels
                       {
                           ExitNoticeID = exitNotice.ID,
                           OrderID = order.MainOrderId,
                           ClientCode = client.ClientCode,
                           ClientName = company.Name,
                           ExitType = (Needs.Wl.Models.Enums.ExitType)exitNotice.ExitType,
                           AdminName = admin.RealName,
                           ExitNoticeStatus = (Needs.Wl.Models.Enums.ExitNoticeStatus)exitNotice.ExitNoticeStatus,
                           IsPrint = exitNotice.IsPrint,
                           CreateDate = exitNotice.CreateDate,

                           PackNo = exitDeliver.PackNo,
                       };

            return linq;
            //}

            //protected override IQueryable<SZUnExitedListViewNewModels> OnReadShips(IQueryable<SZUnExitedListViewNewModels> resultIQuerys)
            //{
            //    var exitDelivers = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitDelivers>();

            //    var linq = from resultIQuery in resultIQuerys
            //               join exitDeliver in exitDelivers
            //                    on new { ExitNoticeID = resultIQuery.ExitNoticeID, ExitDeliverStatus = (int)Needs.Wl.Models.Enums.Status.Normal, }
            //                    equals new { ExitNoticeID = exitDeliver.ExitNoticeID, ExitDeliverStatus = exitDeliver.Status, }
            //                    into exitDelivers2
            //               from exitDeliver in exitDelivers2.DefaultIfEmpty()

            //               select new SZUnExitedListViewNewModels
            //               {
            //                   ExitNoticeID = resultIQuery.ExitNoticeID,
            //                   OrderID = resultIQuery.OrderID,
            //                   ClientCode = resultIQuery.ClientCode,
            //                   ClientName = resultIQuery.ClientName,
            //                   ExitType = resultIQuery.ExitType,
            //                   AdminName = resultIQuery.AdminName,
            //                   ExitNoticeStatus = resultIQuery.ExitNoticeStatus,
            //                   IsPrint = resultIQuery.IsPrint,
            //                   CreateDate = resultIQuery.CreateDate,

            //                   PackNo = exitDeliver.PackNo,
            //               };

            //    return linq;
        }
    }
    }
