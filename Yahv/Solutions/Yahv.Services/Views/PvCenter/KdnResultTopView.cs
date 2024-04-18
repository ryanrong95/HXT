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
    /// 快递鸟相应视图
    /// </summary>
    public class KdnResultTopView : QueryView<Models.KdnResult, PvCenterReponsitory>
    {
        public KdnResultTopView()
        {

        }

        protected override IQueryable<Models.KdnResult> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.KdnResults>()
                   select new Models.KdnResult
                   {
                       ID = entity.ID,
                       OrderCode = entity.OrderCode,
                       LogisticCode = entity.LogisticCode,
                       CreateDate = entity.CreateDate,
                       KDNOrderCode = entity.KDNOrderCode,
                       DestinatioCode = entity.DestinatioCode,
                       OriginCode = entity.OriginCode,
                       ShipperCode = entity.ShipperCode,
                       Html = entity.Html
                   };
        }

        public void Enter(Models.KdnResult entity)
        {
            if (string.IsNullOrWhiteSpace(entity.ID))
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.KdnResults
                {
                    ID = entity.OrderCode,
                    OrderCode = entity.OrderCode,
                    LogisticCode = entity.LogisticCode,
                    CreateDate = DateTime.Now,
                    KDNOrderCode = entity.KDNOrderCode,
                    DestinatioCode = entity.DestinatioCode,
                    OriginCode = entity.OriginCode,
                    ShipperCode = entity.ShipperCode,
                    Html = entity.Html
                });
            }
            else
            {

                //保留了ID的设计主要是为了同步支持一个OrderCode多条记录

                if (entity.ID != entity.OrderCode)
                {
                    throw new Exception();
                }

                this.Reponsitory.Update(new Layers.Data.Sqls.PvCenter.KdnResults
                {
                    OrderCode = entity.OrderCode,
                    LogisticCode = entity.LogisticCode,
                    CreateDate = entity.CreateDate,
                    KDNOrderCode = entity.KDNOrderCode,
                    DestinatioCode = entity.DestinatioCode,
                    OriginCode = entity.OriginCode,
                    ShipperCode = entity.ShipperCode,
                }, item => item.ID == entity.ID);
            }
        }


        public object[] test(object dic)
        {
            object[] data = null;

            var linq = from file in data
                       select new
                       {
                           id = "",

                       };

            foreach (var property in dic.GetType().GetProperties())
            {
                //linq.Join(); 
            }


            return linq.ToArray();
        }


        public void query()
        {
            test(new
            {
                WsOrderID = "",
            });

        }

    }
}
