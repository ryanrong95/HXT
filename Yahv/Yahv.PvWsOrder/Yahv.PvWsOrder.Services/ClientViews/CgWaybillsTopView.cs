using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Enums;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class CgWaybillsTopView : Yahv.Linq.UniqueView<CgWaybillsTopViewModel, PvWsOrderReponsitory>
    {
        protected override IQueryable<CgWaybillsTopViewModel> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.CgWaybillsTopView>()
                   select new CgWaybillsTopViewModel
                   {
                       OrderID = entity.OrderID,
                       WareHouseID = entity.WareHouseID,
                       wbCode = entity.wbCode,
                       NoticeType = (CgNoticeType)entity.NoticeType,
                   };
        }
    }

    public class CgWaybillsTopViewModel : IUnique
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// OrderID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// WareHouseID
        /// </summary>
        public string WareHouseID { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string wbCode { get; set; }

        /// <summary>
        /// 通知类型
        /// </summary>
        public CgNoticeType NoticeType { get; set; }

    }
}
