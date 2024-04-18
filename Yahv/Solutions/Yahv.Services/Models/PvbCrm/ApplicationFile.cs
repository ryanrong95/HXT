using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yahv.Linq;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 收付款申请的凭证文件
    /// </summary>
    public class ApplicationFile : CenterFileMessage,IUnique
    {
        public string ID { get; set; }

        public string Payer { get; set; }

        public string Payee { get; set; }

        public DateTime? CreateDate { get; set; }
        
        public FileDescriptionStatus Status { get; set; }
    }
}
