using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models.LsOrder;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 租赁订单通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class LsOrderTopView<TReponsitory> : UniqueView<LsOrder, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public LsOrderTopView()
        {

        }

        public LsOrderTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<LsOrder> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.LsOrderTopView>()
                   join status in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.Logs_PvLsOrderCurrentTopView>() on entity.ID equals status.MainID
                   where status.MainStatus != (int)LsOrderStatus.Closed
                   select new LsOrder
                   {
                       ID = entity.ID,
                       FatherID = entity.FatherID,
                       Type = (LsOrderType)entity.Type,
                       Source = (LsOrderSource)entity.Source,
                       ClientID = entity.ClientID,
                       PayeeID = entity.PayeeID,
                       BeneficiaryID = entity.BeneficiaryID,
                       Currency = (Currency)entity.Currency,
                       InvoiceID = entity.InvoiceID,
                       IsInvoiced = entity.IsInvoiced,
                       StartDate = entity.StartDate,
                       EndDate = entity.EndDate,
                       Creator = entity.Creator,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Summary = entity.Summary,
                       Status = (LsOrderStatus)status.MainStatus,
                       InvoiceStatus = (OrderInvoiceStatus)status.InvoiceStatus,
                       InheritStatus = entity.InheritStatus,
                   };
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="LsOrders"></param>
        static public void Enter(LsOrder LsOrders)
        {
            using (PvLsOrderReponsitory Reponsitory = new PvLsOrderReponsitory())
            {
                var count = Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.Orders>().Count(a => a.ID == LsOrders.ID);
                if (count == 0)
                {
                    LsOrders.ID = Layers.Data.PKeySigner.Pick(PKeyType.LsOrder);
                    Reponsitory.Insert(new Layers.Data.Sqls.PvLsOrder.Orders
                    {
                        ID = LsOrders.ID,
                        FatherID = LsOrders.FatherID,
                        Type = (int)LsOrders.Type,
                        Source = (int)LsOrders.Source,
                        ClientID = LsOrders.ClientID,
                        PayeeID = LsOrders.PayeeID,
                        BeneficiaryID = LsOrders.BeneficiaryID,
                        Currency = (int)LsOrders.Currency,
                        InvoiceID = LsOrders.InvoiceID,
                        IsInvoiced = LsOrders.IsInvoiced,
                        StartDate = LsOrders.StartDate,
                        EndDate = LsOrders.EndDate,
                        Status = (int)LsOrders.Status,
                        Creator = LsOrders.Creator,
                        CreateDate = LsOrders.CreateDate,
                        ModifyDate = LsOrders.ModifyDate,
                        Summary = LsOrders.Summary,
                        InheritStatus = LsOrders.InheritStatus,
                    });
                }
                else
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
                    {
                        FatherID = LsOrders.FatherID,
                        Type = (int)LsOrders.Type,
                        Source = (int)LsOrders.Source,
                        ClientID = LsOrders.ClientID,
                        PayeeID = LsOrders.PayeeID,
                        BeneficiaryID = LsOrders.BeneficiaryID,
                        Currency = (int)LsOrders.Currency,
                        InvoiceID = LsOrders.InvoiceID,
                        IsInvoiced = LsOrders.IsInvoiced,
                        StartDate = LsOrders.StartDate,
                        EndDate = LsOrders.EndDate,
                        Status = (int)LsOrders.Status,
                        Creator = LsOrders.Creator,
                        CreateDate = LsOrders.CreateDate,
                        ModifyDate = LsOrders.ModifyDate,
                        Summary = LsOrders.Summary,
                        InheritStatus = LsOrders.InheritStatus,
                    }, item => item.ID == LsOrders.ID);
                }
            }
        }
    }
}
