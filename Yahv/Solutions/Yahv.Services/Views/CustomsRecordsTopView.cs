using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 清关费记录
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class CustomsRecordsTopView<TReponsitory> : UniqueView<CustomsRecord, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public CustomsRecordsTopView()
        {

        }

        public CustomsRecordsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<CustomsRecord> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.CustomsRecordsTopView>()
                   select new CustomsRecord()
                   {
                       CreateDate = entity.CreateDate,
                       WaybillID = entity.WaybillID,
                       Price = entity.Price,
                       ID = entity.ID,
                       ClientID = entity.ClientID,
                       ConfirmDate = entity.ConfirmDate
                   };
        }
    }
}