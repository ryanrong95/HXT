using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 提货通知日志记录视图
    /// </summary>
    public class DeliveryNoticeLogView : UniqueView<Models.DeliveryNoticeLog, ScCustomsReponsitory>
    {
        public DeliveryNoticeLogView()
        {
        }

        internal DeliveryNoticeLogView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DeliveryNoticeLog> GetIQueryable()
        {
          //  var deliveryNoticeView = new Views.DeliveryNoticeView(this.Reponsitory);
            var adminView = new Views.AdminsTopView(this.Reponsitory);
            return from deliveryNoticeLog in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveryNoticeLogs>()
                   join admin in adminView on deliveryNoticeLog.AdminID equals admin.ID
                   //join deliveryNotice in deliveryNoticeView on deliveryNoticeLog.DeliveryNoticeID equals deliveryNotice.ID
                   select new Models.DeliveryNoticeLog
                   {
                       ID = deliveryNoticeLog.ID,
                       DeliveryNoticeID = deliveryNoticeLog.DeliveryNoticeID,
                       Admin = admin,
                       OperType = (Enums.DeliveryOperType)deliveryNoticeLog.OperType,
                       CreateDate = deliveryNoticeLog.CreateDate,
                       Summary = deliveryNoticeLog.Summary
                   };


        }
    }

}
