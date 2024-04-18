using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class TakingInfosReturnModel
    {
        public string TakingID { get; set; }

        public string TakingMan { get; set; }

        public string TakingTel { get; set; }

        public string ProofTypeValue { get; set; }

        public string ProofTypeDes { get; set; }

        public string ProofNumber { get; set; }
    }
}