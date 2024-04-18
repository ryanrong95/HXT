using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    public class Picker : IUnique
    {
        public string ID { get; set; }

        public string ClientID { get; set; }

        public Enums.IDType IDType { get; set; }

        public string IDCode { get; set; }

        public string Contact { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public GeneralStatus Status { get; set; }

        public string IDTypeDec
        {
            get
            {
                return this.IDType.GetDescription();
            }
        }
    }
}
