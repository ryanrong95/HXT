using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ReImportItemModel : IUnique
    {
        public string ID { get; set; }
        public string ImportID { get; set; }
        public string FinanceRepID { get; set; }
        public string Seq { get; set; }
        public decimal? USD { get; set; }
        public decimal? RMB { get; set; }
        public decimal? DeclareRate { get; set; }
        public string Currency { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
