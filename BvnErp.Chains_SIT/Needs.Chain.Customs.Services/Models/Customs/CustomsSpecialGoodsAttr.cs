using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CustomsSpecialGoodsAttr : IUnique
    {
        public string ID { get; set; }

        public string HSCode { get; set; }

        public string GoodsAttr { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

    }
}
