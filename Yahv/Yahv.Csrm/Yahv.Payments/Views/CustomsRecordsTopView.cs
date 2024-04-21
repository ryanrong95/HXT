using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;
using Layers.Linq;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 清关费用记录
    /// </summary>
    //public class CustomsRecordsTopView : QueryView<CustomsRecord, PvbCrmReponsitory>
    //{
    //    public CustomsRecordsTopView()
    //    {

    //    }

    //    public CustomsRecordsTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
    //    {

    //    }

    //    protected override IQueryable<CustomsRecord> GetIQueryable()
    //    {
    //        return new Yahv.Services.Views.CustomsRecordsTopView<PvbCrmReponsitory>();
    //    }

    //    public void Add(CustomsRecord entity)
    //    {
    //        using (var reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
    //        {
    //            if (
    //                reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.CustomsRecords>()
    //                    .Any(item => item.WaybillID == entity.WaybillID))
    //            {
    //                throw new Exception("您已经确认清关费用!");
    //            }

    //            reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.CustomsRecords()
    //            {
    //                ID = PKeySigner.Pick(PKeyType.CustomsRecords),
    //                WaybillID = entity.WaybillID,
    //                ClientID = entity.ClientID,
    //                ConfirmDate = entity.ConfirmDate,
    //                CreateDate = DateTime.Now,
    //                Price = entity.Price,
    //            });
    //        }
    //    }
    //}
}