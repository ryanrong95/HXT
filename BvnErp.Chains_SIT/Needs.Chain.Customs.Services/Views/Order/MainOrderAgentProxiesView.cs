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
    public class MainOrderAgentProxiesView : UniqueView<Models.OrderAgentProxy, ScCustomsReponsitory>
    {
        public MainOrderAgentProxiesView()
        {

        }

        protected override IQueryable<Models.OrderAgentProxy> GetIQueryable()
        {
            var orderView = new OrdersView(this.Reponsitory);
            //var filesView = new MainOrderFilesView(this.Reponsitory).Where(file => file.FileType == Enums.FileType.AgentTrustInstrument &&
            //                                                                   file.Status == Enums.Status.Normal).OrderByDescending(file => file.CreateDate);
            var filesView = new CenterLinkXDTFilesTopView(this.Reponsitory).Where(file => file.FileType == Enums.FileType.AgentTrustInstrument&&file.Status != Enums.Status.Delete
                                                                            ).OrderByDescending(file => file.CreateDate);
            return from order in orderView
                   join file in filesView on order.MainOrderID equals file.MainOrderID into files
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
                       MainFile = files.FirstOrDefault()
                   };
        }
    }
}
