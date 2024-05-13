using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Wms.Services.chonggous.Models;
using Yahv.Underly;
using Yahv.Utils.Http;

namespace Wms.Services.chonggous
{
    public class PushMsg
    {
        /// <summary>
        /// 代报关 1 或代仓储 2
        /// </summary>
        public int SystemID { get; set; }
        public int SpotType { get; set; }

        public string OrderID { get; set; }

        public string WaybillNo { get; set; }

        public string DriverName { get; set; }

        public string DriverPhone { get; set; }

        public PushMsg(int systemID, int spotType, string orderID)
        {
            this.SystemID = systemID;
            this.SpotType = spotType;
            this.OrderID = orderID;
        }

        public PushMsg(int systemID, int spotType, string orderID, string waybillNo, string driverName, string driverPhone)
        {
            this.SystemID = systemID;
            this.SpotType = spotType;
            this.OrderID = orderID;
            this.WaybillNo = waybillNo;
            this.DriverName = driverName;
            this.DriverPhone = driverPhone;            
        }

        public void push()
        {
            MsgDTO msgDTO = new MsgDTO();
            try
            {
                msgDTO.clientCode = this.OrderID.Substring(0, 5);
                msgDTO.orderID = this.OrderID;
                msgDTO.spotType = this.SpotType;
                msgDTO.systemID = this.SystemID;
                msgDTO.waybillNo = this.WaybillNo;
                msgDTO.driverName = this.DriverName;
                msgDTO.driverPhone = this.DriverPhone;

                string requestUrl = FromType.SendMsgToClient.GetDescription();
                ApiHelper.Current.JPost(requestUrl, msgDTO);

                CgLogs_Operator logs = new CgLogs_Operator();
                logs.Conduct = "发送通知";
                logs.CreatorID = Npc.Robot.ToString();
                logs.MainID = this.OrderID;
                logs.Type = Enums.LogOperatorType.Insert;
                logs.CreateDate = DateTime.Now;
                logs.Content = $"{this.OrderID} {this.SpotType} 推送消息成功 {JsonConvert.SerializeObject(msgDTO)}";
                logs.Enter();
            }
            catch (Exception ex)
            {
                CgLogs_Operator logs = new CgLogs_Operator();
                logs.Conduct = "发送通知";
                logs.CreatorID = Npc.Robot.ToString();
                logs.MainID = this.OrderID;
                logs.Type = Enums.LogOperatorType.Insert;
                logs.CreateDate = DateTime.Now;
                logs.Content = $"{this.OrderID} {this.SpotType} 推送消息失败 {JsonConvert.SerializeObject(msgDTO)}";
                logs.Enter();
            }
        }
    }
}
