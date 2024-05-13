//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Wms.Services.Models;
//using Yahv.Linq;

//namespace Wms.Services.Views
//{
//    public class NoticeRoll : QueryView<NoticeDetail, PvWmsRepository>
//    {
//        protected override IQueryable<NoticeDetail> GetIQueryable()
//        {
//            return from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
//                   join notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on waybill.wbID equals notice.WaybillID into notices
//                   join summary in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Summaries>() on waybill.wbID equals summary.ID
//                   select new NoticeDetail
//                   {
//                       Waybill = new Waybills { ID = waybill.wbID },
//                       Data = notices.Select(item => new Notices { }).ToArray(),
//                       Summary = new Summaries { ID = summary.ID }


//                   };


//        }
//    }
//}
