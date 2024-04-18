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
    public class SendBaseInfoViewOri : UniqueView<Models.SendBaseInfo, ScCustomsReponsitory>
    {
        public SendBaseInfoViewOri()
        {
        }

        internal SendBaseInfoViewOri(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SendBaseInfo> GetIQueryable()
        {
            return from baseInfo in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SendBaseInfo>()

                   select new Models.SendBaseInfo
                   {
                       ID = baseInfo.ID,
                       ClientID = baseInfo.ClientID,
                       SendSpotID = baseInfo.SendSpotID,
                       Mobile = baseInfo.Mobile,
                       Email = baseInfo.Email,
                       WeChatID = baseInfo.WeChatID,
                       SendType = (MsgSendType)baseInfo.SendType,
                       Status = (Status)baseInfo.Status,
                       CreateDate = baseInfo.CreateDate,
                       UpdateDate = baseInfo.UpdateDate,
                       Summary = baseInfo.Summary,
                   };
        }

    }
}
