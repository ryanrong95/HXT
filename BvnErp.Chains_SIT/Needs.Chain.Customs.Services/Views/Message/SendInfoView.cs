using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class SendInfoView : UniqueView<Models.SendBaseInfo, ScCustomsReponsitory>
    {
        public SendInfoView()
        {
        }

        protected SendInfoView(ScCustomsReponsitory reponsitory, IQueryable<Models.SendBaseInfo> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.SendBaseInfo> GetIQueryable()
        {
            var spotView = new SendSpotViewOri(this.Reponsitory);

            return from baseinfo in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SendBaseInfo>()
                   join spot in spotView on baseinfo.SendSpotID equals spot.ID                
                   select new Models.SendBaseInfo
                   {
                       ID = baseinfo.ID,
                       ClientID = baseinfo.ClientID,
                       SendSpotID = baseinfo.SendSpotID,
                       Mobile = baseinfo.Mobile,
                       Email = baseinfo.Email,
                       WeChatID = baseinfo.WeChatID,
                       SendType = (MsgSendType)baseinfo.SendType,
                       Status = (Status)baseinfo.Status,
                       CreateDate = baseinfo.CreateDate,
                       UpdateDate = baseinfo.UpdateDate,
                       Summary = baseinfo.Summary,
                       Spot = spot
                   };
        }
    }
}
