using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ApiNoticeClassifyPushStatusModel : IUnique
    {
        public string ID { get; set; } = string.Empty;

        public string PreProductCategoryID { get; set; } = string.Empty;

        public Enums.PushStatus PushStatus { get; set; }
    }
}
