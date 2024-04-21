using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 快递鸟请求视图
    /// </summary>
    public class KdnRequestsTopView : QueryView<KdnRequest, PvbCrmReponsitory>
    {
        protected override IQueryable<KdnRequest> GetIQueryable()
        {
            return new Yahv.Services.Views.KdnRequestTopView<PvbCrmReponsitory>();
        }

        public KdnRequest this[string id]
        {
            get { return this.SingleOrDefault(item => item.ID == id); }
        }

        /// <summary>
        /// 修改快递鸟请求试图
        /// </summary>
        /// <remarks>只能修改快递运费、快递其他运费</remarks>
        /// <param name="entity"></param>
        public void Modify(KdnRequest entity)
        {
            using (var reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.KdnRequests>(new
                {
                    Cost = entity.Cost,
                    OtherCost = entity.OtherCost,
                }, item => item.ID == entity.ID);
            }
        }
    }
}
