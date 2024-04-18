using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views.Alls
{
    //public class ApiNoticesAll : Needs.Linq.Generic.Unique1Classics<Models.ApiNotice, ScCustomsReponsitory>
    //{
    //    public ApiNoticesAll()
    //    {
    //    }
    //    internal ApiNoticesAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
    //    {
    //    }

    //    protected override IQueryable<Models.ApiNotice> GetIQueryable(Expression<Func<Models.ApiNotice, bool>> expression, params LambdaExpression[] expressions)
    //    {
    //        var apiNoticesView = new Origins.ApiNoticesOrigin(this.Reponsitory);
    //        var linq = from entity in apiNoticesView
    //                   select new Models.ApiNotice
    //                   {
    //                       ID = entity.ID,
    //                       PushType = entity.PushType,
    //                       ClientID = entity.ClientID,
    //                       ItemID = entity.ItemID,
    //                       PushStatus = entity.PushStatus,
    //                       CreateDate = entity.CreateDate,
    //                       UpdateDate = entity.UpdateDate
    //                   };

    //        foreach (var predicate in expressions)
    //        {
    //            linq = linq.Where(predicate as Expression<Func<Models.ApiNotice, bool>>);
    //        }

    //        return linq.Where(expression);
    //    }

    //    protected override IEnumerable<Models.ApiNotice> OnReadShips(Models.ApiNotice[] results)
    //    {
    //        var clientsView = new Rolls.ClientsRoll(this.Reponsitory);

    //        return from result in results
    //               join client in clientsView on result.ClientID equals client.ID
    //               select new Models.ApiNotice
    //               {
    //                   ID = result.ID,
    //                   PushType = result.PushType,
    //                   ClientID = result.ClientID,
    //                   Client = client,
    //                   ItemID = result.ItemID,
    //                   PushStatus = result.PushStatus,
    //                   CreateDate = result.CreateDate,
    //                   UpdateDate = result.UpdateDate
    //               };

    //    }
    //}

    public class ApiNoticesAll : UniqueView<Models.ApiNotice, ScCustomsReponsitory>
    {
        public ApiNoticesAll()
        {
        }

        internal ApiNoticesAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ApiNotice> GetIQueryable()
        {
            var apiNoticesView = new Origins.ApiNoticesOrigin(this.Reponsitory);
            var clientsView = new Rolls.ClientsRoll(this.Reponsitory);

            return from result in apiNoticesView
                   join client in clientsView on result.ClientID equals client.ID
                   select new Models.ApiNotice
                   {
                       ID = result.ID,
                       PushType = result.PushType,
                       ClientID = result.ClientID,
                       Client = client,
                       ItemID = result.ItemID,
                       PushStatus = result.PushStatus,
                       CreateDate = result.CreateDate,
                       UpdateDate = result.UpdateDate
                   };
        }
    }
}
