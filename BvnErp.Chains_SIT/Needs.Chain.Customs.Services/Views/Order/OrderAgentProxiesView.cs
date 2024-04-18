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
    /// 代理报关委托书
    /// </summary>
    public class OrderAgentProxiesView : UniqueView<Models.OrderAgentProxy, ScCustomsReponsitory>
    {
        public OrderAgentProxiesView()
        {

        }

        protected override IQueryable<Models.OrderAgentProxy> GetIQueryable()
        {
            var orderView = new OrdersView(this.Reponsitory);
            var filesView = new OrderFilesView(this.Reponsitory).Where(file => file.FileType == Enums.FileType.AgentTrustInstrument &&
                                                                               file.Status == Enums.Status.Normal).OrderByDescending(file => file.CreateDate);

            return from order in orderView
                   join file in filesView on order.ID equals file.OrderID into files
                   //from file in files.DefaultIfEmpty()
                       //where order.OrderStatus >= Enums.OrderStatus.Quoted && order.OrderStatus < Enums.OrderStatus.Completed
                   select new Models.OrderAgentProxy
                   {
                       ID = order.ID,
                       Order = order,
                       Client = order.Client,
                       Currency = order.Currency,
                       DeclarePrice = order.DeclarePrice,
                       PackNo = order.PackNo,
                       WarpType = order.WarpType,
                       CreateDate = order.CreateDate,
                       OrderStatus = order.OrderStatus,
                       File = files.FirstOrDefault()
                   };
        }
    }
}
