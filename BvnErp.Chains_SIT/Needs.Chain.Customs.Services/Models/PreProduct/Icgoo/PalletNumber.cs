using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PalletNumber:IUnique
    {
        public string ID { get; set; }
        public string Stock { get; set; }
        public string Pallet { get; set; }
        public DateTime NoticeTime { get; set; }
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Summary { get; set; }
    }
}
