using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.SzMvc.Services
{
    public class PvWsOrderApiSetting
    {
        /// <summary>
        /// 子系统接口地址
        /// </summary>
        public string ApiName { get; private set; }
        /// <summary>
        /// 深圳代仓储开票通知
        /// </summary>
        public string ApplyInvoice { get; private set; }
        public PvWsOrderApiSetting()
        {
            ApiName = "PvWsOrderApiUrl";
            ApplyInvoice = "InvoiceNotice/SzInvoiceEnter";
        }
    }
}
