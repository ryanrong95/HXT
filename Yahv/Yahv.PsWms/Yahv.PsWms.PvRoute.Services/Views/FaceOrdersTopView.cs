using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.PvRoute.Services.Views
{
    public class FaceOrdersTopView : QueryView<Models.FaceOrder, PvRouteReponsitory>, IDisposable
    {
        public FaceOrdersTopView()
        {

        }
        protected override IQueryable<Models.FaceOrder> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvRoute.FaceOrders>()
                   select new Models.FaceOrder
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Source = (PrintSource)entity.Source,
                       CreatorID = entity.CreatorID,
                       Html = entity.Html,
                       SendJson = entity.SendJson,
                       ReceiveJson = entity.ReceiveJson,
                       SessionID = entity.SessionID,
                       MainID = entity.MainID,
                       MyID = entity.MyID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (PrintState)entity.Status,
                   };
        }

        public void Enter(Models.FaceOrder entity)
        {
            if (this.Reponsitory.ReadTable<Layers.Data.Sqls.PvRoute.FaceOrders>().Where(item => item.ID == entity.Code).Count() > 0)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(entity.ID))
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvRoute.FaceOrders
                {
                    ID = entity.Code,
                    Code = entity.Code,
                    Source = (int)entity.Source,
                    CreatorID = entity.CreatorID,
                    Html = entity.Html,
                    SendJson = entity.SendJson,
                    ReceiveJson = entity.ReceiveJson,
                    SessionID = entity.SessionID,
                    MainID = entity.MainID,
                    MyID = entity.MyID,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = (int)PrintState.Waiting,
                });
            }
            else
            {
                //保留了ID的设计主要是为了同步支持一个OrderCode多条记录
                if (entity.ID != entity.Code)
                {
                    throw new Exception();
                }

                this.Reponsitory.Update<Layers.Data.Sqls.PvRoute.FaceOrders>(new
                {
                    Code = entity.Code,
                    Source = (int)entity.Source,
                    CreatorID = entity.CreatorID,
                    Html = entity.Html,
                    SendJson = entity.SendJson,
                    ReceiveJson = entity.ReceiveJson,
                    SessionID = entity.SessionID,
                    MainID = entity.MainID,
                    MyID = entity.MyID,
                    CreateDate = entity.CreateDate,
                    ModifyDate = DateTime.Now,
                    Status = (int)entity.Status,
                }, item => item.ID == entity.ID);
            }
        }
    }
}
