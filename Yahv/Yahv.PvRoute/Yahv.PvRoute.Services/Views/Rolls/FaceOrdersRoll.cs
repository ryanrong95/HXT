using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvRoute.Services.Models;
using Yahv.PvRoute.Services.Views.Origins;

namespace Yahv.PvRoute.Services.Views.Rolls
{
    class FaceOrdersRoll : QueryView<FaceOrder, PvRouteReponsitory>
    {

        public FaceOrdersRoll()
        {

        }

        public FaceOrdersRoll(PvRouteReponsitory reponsitory, IQueryable<FaceOrder> iQueryable) : base(reponsitory, iQueryable)
        {
           
        }

        protected override IQueryable<FaceOrder> GetIQueryable()
        {
            var faceOrderOrigin = new FaceOrderOrigin(this.Reponsitory);

            return faceOrderOrigin;
        }
    }
}
