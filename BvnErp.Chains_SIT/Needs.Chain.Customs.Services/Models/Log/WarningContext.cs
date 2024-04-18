using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public  class WarningContext:IUnique
    {
        public string ID { get; set; }
        public string MainID { get; set; }
        public string Title { get; set; }
        public string Context { get; set; }
        public DateTime CreateDate { get; set; }      
        public bool Readed { get; set; }
        public DateTime? ReadDate { get; set; }      
        public Enums.SendNoticeType NoticeType { get; set; }
        public string AdminID { get; set; }
        public string Email { get; set; }
        public string Moblie { get; set; }
    }
}
