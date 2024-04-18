using System;

namespace Yahv.Models
{
    /// <summary>
    /// 系统日志
    /// </summary>
    public class Oplogs : Linq.IUnique
    {
        public string ID { get; set; }

        public string Url { get; set; }

        public string Sys { get; set; }

        public ErpAdmin Admin { get; set; }

        public DateTime CreateDate { get; set; }

        public string Remark { get; set; }

        public string Operation { get; set; }

        public string Type { get; set; }        
        
    }
}
