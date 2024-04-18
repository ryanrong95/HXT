using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services
{
    public class DyjPIViewModel
    {
        public string success { get; set; }

        public string msg { get; set; }

        public string user_host_address { get; set; }

        public int count { get; set; }

        public List<ForicMessageData> data { get; set; }
    }

    public class ForicMessageData
    {
        public string 付款公司 { get; set; }

        public string 客户公司ID { get; set; }

        public string PI文件路径 { get; set; }

        public string CI文件路径 { get; set; }

        public string 客户公司名称 { get; set; }

        public string 付款公司ID { get; set; }

        public string 电话 { get; set; }

        public string 传真 { get; set; }

        public List<ForicDataItem> 明细列表 { get; set; }
    }

    public class ForicDataItem
    {
        public string ID { get; set; }

        public string PartNo { get; set; }

        public decimal Quantity { get; set; }

        public decimal Amount { get; set; }

        public string CurrencyType { get; set; }

        public string MFC { get; set; }

        public string Area { get; set; }

        public string Pack { get; set; }
    }
}
