using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Services.Models.LsOrder;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class LsOrderItemAlls : LsOrderItemTopView<PvLsOrderReponsitory>
    {
        public LsOrderItemAlls()
        {

        }

        public LsOrderItemAlls(PvLsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        public LsOrderItemAlls(PvLsOrderReponsitory reponsitory, IQueryable<LsOrderItem> iquery) : base(reponsitory, iquery)
        {

        }

        protected override IQueryable<LsOrderItem> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Status == GeneralStatus.Normal);
        }

        /// <summary>
        /// 根据订单ID查询详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public LsOrderItemAlls SearchByOrderID (string ID)
        {
            var linq = this.IQueryable.Where(item => item.OrderID == ID);

            return new LsOrderItemAlls(this.Reponsitory, linq);
        }
    }
}
