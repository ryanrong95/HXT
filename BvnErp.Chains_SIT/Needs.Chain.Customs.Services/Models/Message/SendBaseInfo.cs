using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Needs.Ccs.Services.Models
{
    public class SendBaseInfo:IUnique
    {
        public string ID { get; set; }
        public string ClientID { get; set; }
        public string SendSpotID { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string WeChatID { get; set; }
        public MsgSendType SendType { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }
        public SendSpotModel Spot { get; set; }

        public SendBaseInfo()
        {
            this.Status = Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SendBaseInfo>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.SendBaseInfo>(new Layer.Data.Sqls.ScCustoms.SendBaseInfo
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        SendSpotID = this.SendSpotID,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        WeChatID = this.WeChatID,
                        SendType = (int)this.SendType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SendBaseInfo>(new
                    {
                        ClientID = this.ClientID,
                        SendSpotID = this.SendSpotID,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        WeChatID = this.WeChatID,
                        SendType = (int)this.SendType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }
        }
    }
}
