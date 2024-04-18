using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.Message
{
    public partial class Declare : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            string clientID = "0046215E337BC531B5EA2F6A8B5DA591";
            var baseInfo = new SendInfoView().Where(t => t.ClientID == clientID).ToArray();
            List<SendInfo> infos = new List<SendInfo>();
            
            foreach(SendBaseInfo info in baseInfo)
            {
                SendInfo sendInfo = new SendInfo();
                sendInfo.Name = info.Spot.Name;
                sendInfo.TypeValue = (int)info.Spot.Type;
                sendInfo.SendMsg = info.Spot.SendMessage;
                sendInfo.iMobile = (info.SendType & MsgSendType.SMS)>0;
                sendInfo.Mobile = info.Mobile;
                sendInfo.iEmail = (info.SendType & MsgSendType.Email) > 0;
                sendInfo.Email = info.Email;
                sendInfo.iWeChat = (info.SendType & MsgSendType.WeChat) > 0;
                sendInfo.WeChatID = info.WeChatID;
                infos.Add(sendInfo);
            }

            this.Model.ConfigInfo = infos.Json();

            var SpotInfo = new SendSpotViewOri().
                                Where(t=>t.Status==Status.Normal&&t.SystemID==(BusinessType.Declare)).
                                Select(t=>new SpotInfo { Name = t.Name,TypeValue = (int)t.Type }).ToArray();
            SpotInfo = SpotInfo.OrderBy(t => t.TypeValue).ToArray();
            this.Model.SpotName = SpotInfo.Json();
        }

        private class SendInfo
        {
            public string Name { get; set; }
            public int TypeValue { get; set; }
            public string SendMsg { get; set; }
            public bool iMobile { get; set; }
            public string Mobile { get; set; }
            public bool iEmail { get; set; }
            public string Email { get; set; }
            public bool iWeChat { get; set; }
            public string WeChatID { get; set; }
        }

        private class SpotInfo
        {
            public string Name { get; set; }
            public int TypeValue { get; set; }
        }

        protected void SaveMessage()
        {

            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();
        }

    }
}