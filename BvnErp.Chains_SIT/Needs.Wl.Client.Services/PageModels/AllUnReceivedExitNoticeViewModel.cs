using System;
using System.Collections.Generic;

namespace Needs.Wl.Client.Services.PageModels
{
    public class AllUnReceivedExitNoticeViewModel : Needs.Wl.Models.ExitNotice
    {
        public string ReceiverName { get; set; }
        public string ReceiveCompanyName { get; set; }
        public string DeliveryFileUrl { get; set; }
        public DateTime MainCreateDate { get; set; }
    }
}