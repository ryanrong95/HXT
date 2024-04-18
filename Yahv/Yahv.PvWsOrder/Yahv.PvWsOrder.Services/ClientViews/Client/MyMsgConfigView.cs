using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 消息设置视图
    /// </summary>
    public class MyMsgConfigView : UniqueView<MyMsgConfig, ScCustomReponsitory>
    {
        private IUser user;

        public MyMsgConfigView(IUser User)
        {
            this.user = User;
        }

        protected override IQueryable<MyMsgConfig> GetIQueryable()
        {
            var sendBaseInfos = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.SendBaseInfo>();

            var linq = from sendBaseInfo in sendBaseInfos
                       select new MyMsgConfig
                       {
                           ID = sendBaseInfo.ID,
                           ClientID = sendBaseInfo.ClientID,
                           ClientCode = sendBaseInfo.ClientCode,
                           SendSpotID = sendBaseInfo.SendSpotID,
                           Mobile = sendBaseInfo.Mobile,
                           Email = sendBaseInfo.Email,
                           WeChatID = sendBaseInfo.WeChatID,
                           SendType = sendBaseInfo.SendType,
                           Status = sendBaseInfo.Status,
                           CreateDate = sendBaseInfo.CreateDate,
                           UpdateDate = sendBaseInfo.UpdateDate,
                           Summary = sendBaseInfo.Summary,
                       };
            return linq;
        }

        /// <summary>
        /// 获取消息开关状态
        /// </summary>
        /// <returns>true - 打开, false - 关闭</returns>
        public bool GetMsgStatus()
        {
            var normalExist = this.IQueryable.Where(t => t.ClientID == this.user.XDTClientID && t.Status == (int)GeneralStatus.Normal).Any();
            return normalExist;
        }

        /// <summary>
        /// 设置消息开关状态
        /// </summary>
        public void SetMsgStatus(bool on)
        {
            var status = on ? (int)GeneralStatus.Normal : (int)GeneralStatus.Closed;
            this.Reponsitory.Update<Layers.Data.Sqls.ScCustoms.SendBaseInfo>(new
            {
                Status = status,
            }, item => item.ClientID == this.user.XDTClientID);
        }

    }
}
