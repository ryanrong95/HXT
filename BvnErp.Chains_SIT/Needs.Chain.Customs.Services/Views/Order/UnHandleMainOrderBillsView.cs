using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 待上传，待审核 代理委托报关单界面用
    /// </summary>
    public class UnHandleMainOrderBillsView : UniqueView<Models.OrderBill, ScCustomsReponsitory>
    {
        public UnHandleMainOrderBillsView()
        {

        }

        protected override IQueryable<OrderBill> GetIQueryable()
        {
            var clientsView = new ClientsView(this.Reponsitory);

            //var filesView = new MainOrderFilesViewOriginal(this.Reponsitory).Where(file => file.FileType == Enums.FileType.OrderBill &&
            //                                                                     file.Status == Enums.Status.Normal).OrderByDescending(file => file.CreateDate);

            var linq = from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>()
                       join client in clientsView on order.ClientID equals client.ID
                       join file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CenterLinkXDTFilesTopView>() on
                           new
                           {
                               OrderID = order.ID,
                               FileType = (int)Enums.FileType.OrderBill,
                               OrderFileStatus = (int)Enums.Status.Normal
                           }
                      equals
                          new
                          {
                              OrderID = file.MainOrderID,
                              FileType = file.FileType,
                              OrderFileStatus = file.Status
                          } into files
                       from orderfile in files.DefaultIfEmpty()
                       where order.Status == (int)Enums.Status.Normal 
                       select new Models.OrderBill()
                       {
                           ID = order.ID,
                           Order = new Order
                           {
                               ID = order.ID,
                           },
                           Client = client,
                           CreateDate = order.CreateDate,
                           MainOrderFileStatus = orderfile == null ? OrderFileStatus.NotUpload : (OrderFileStatus)orderfile.FileStatus,
                       };
            return linq;
        }
    }
}