using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly.Attributes;
using Yahv.Utils.Converters;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 快递鸟请求视图
    /// </summary>
    public class KdnRequestTopView : KdnRequestTopView<PvCenterReponsitory>
    {
        public KdnRequestTopView()
        {

        }

        protected override IQueryable<Models.KdnRequest> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.KdnRequests>()
                   select new Models.KdnRequest
                   {
                       ID = entity.ID,
                       ShipperCode=entity.ShipperCode,
                       OrderCode = entity.OrderCode,
                       ExpType = entity.ExpType,
                       Quantity = entity.Quantity,
                       PayType = (Models.PayType)entity.PayType,
                       MonthCode = entity.MonthCode,
                       SenderAddress = entity.SenderAddress,
                       SenderCompany = entity.SenderCompany,
                       SenderName = entity.SenderName,
                       SenderMobile = entity.SenderMobile,
                       SenderTel = entity.SenderTel,
                       ReceiverAddress = entity.ReceiverAddress,
                       ReceiverCompany = entity.ReceiverCompany,
                       ReceiverName = entity.ReceiverName,
                       ReceiverMobile = entity.ReceiverMobile,
                       ReceiverTel = entity.ReceiverTel,
                       Remark = entity.Remark,
                       Currency = (Underly.Currency)entity.Currency,
                       Cost = entity.Cost,
                       OtherCost = entity.OtherCost,
                       CreateDate = entity.CreateDate,
                   };
        }

        public void Enter(Models.KdnRequest entity)
        {
            if (string.IsNullOrWhiteSpace(entity.ID))
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.KdnRequests
                {
                    ID = entity.OrderCode,
                    OrderCode = entity.OrderCode,
                    ShipperCode = entity .ShipperCode,
                    ExpType = entity.ExpType,
                    Quantity = entity.Quantity,
                    PayType = (int)entity.PayType,
                    MonthCode = entity.MonthCode,
                    SenderAddress = entity.SenderAddress,
                    SenderCompany = entity.SenderCompany,
                    SenderName = entity.SenderName,
                    SenderMobile = entity.SenderMobile,
                    SenderTel = entity.SenderTel,
                    ReceiverAddress = entity.ReceiverAddress,
                    ReceiverCompany = entity.ReceiverCompany,
                    ReceiverName = entity.ReceiverName,
                    ReceiverMobile = entity.ReceiverMobile,
                    ReceiverTel = entity.ReceiverTel,
                    Remark = entity.Remark,
                    Currency = (int)entity.Currency,
                    Cost = entity.Cost,
                    OtherCost = entity.OtherCost,
                    CreateDate = DateTime.Now,
                });
            }
            else
            {

                //保留了ID的设计主要是为了同步支持一个OrderCode多条记录

                if (entity.ID != entity.OrderCode)
                {
                    throw new Exception();
                }

                this.Reponsitory.Update(new Layers.Data.Sqls.PvCenter.KdnRequests
                {
                    OrderCode = entity.OrderCode,
                    ExpType = entity.ExpType,
                    Quantity = entity.Quantity,
                    PayType = (int)entity.PayType,
                    MonthCode = entity.MonthCode,
                    SenderAddress = entity.SenderAddress,
                    SenderCompany = entity.SenderCompany,
                    SenderName = entity.SenderName,
                    SenderMobile = entity.SenderMobile,
                    SenderTel = entity.SenderTel,
                    ReceiverAddress = entity.ReceiverAddress,
                    ReceiverCompany = entity.ReceiverCompany,
                    ReceiverName = entity.ReceiverName,
                    ReceiverMobile = entity.ReceiverMobile,
                    ReceiverTel = entity.ReceiverTel,
                    Remark = entity.Remark,
                    Currency = (int)entity.Currency,
                    Cost = entity.Cost,
                    OtherCost = entity.OtherCost,
                    CreateDate = entity.CreateDate,
                }, item => item.ID == entity.ID);
            }
        }
    }

    public class KdnRequestTopView<TReponsitory> : QueryView<Models.KdnRequest, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public KdnRequestTopView()
        {

        }
        public KdnRequestTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.KdnRequest> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.KdnRequestsTopView>()
                   select new Models.KdnRequest
                   {
                       ID = entity.ID,
                       OrderCode = entity.OrderCode,
                       ExpType = entity.ExpType,
                       Quantity = entity.Quantity,
                       PayType = (Models.PayType)entity.PayType,
                       MonthCode = entity.MonthCode,
                       SenderAddress = entity.SenderAddress,
                       SenderCompany = entity.SenderCompany,
                       SenderName = entity.SenderName,
                       SenderMobile = entity.SenderMobile,
                       SenderTel = entity.SenderTel,
                       ReceiverAddress = entity.ReceiverAddress,
                       ReceiverCompany = entity.ReceiverCompany,
                       ReceiverName = entity.ReceiverName,
                       ReceiverMobile = entity.ReceiverMobile,
                       ReceiverTel = entity.ReceiverTel,
                       Remark = entity.Remark,
                       Currency = (Underly.Currency)entity.Currency,
                       Cost = entity.Cost,
                       OtherCost = entity.OtherCost,
                       CreateDate = entity.CreateDate,
                   };
        }
    }
}
