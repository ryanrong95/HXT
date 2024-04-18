using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class MyMsgConfig : IUnique
    {
        public string ID { get; set; }

        public string ClientID { get; set; }

        public string ClientCode { get; set; }
        
        public string SendSpotID { get; set; }
        
        public string Mobile { get; set; }
        
        public string Email { get; set; }
        
        public string WeChatID { get; set; }
        
        public int? SendType { get; set; }
        
        public int Status { get; set; }
        
        public DateTime CreateDate { get; set; }
        
        public DateTime UpdateDate { get; set; }
        
        public string Summary { get; set; }
    }
}
