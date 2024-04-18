using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly;
using Needs.Linq;

namespace NtErp.Services.Models
{
    public class UserInput : IUserInput
    {
        public decimal Amount { set; get; }

        public string Code { set; get; }

        public DateTime CreateDate { internal set; get; }

        public Currency Currency { set; get; }

        public string InputID { set; get; }
        public InputSource Source { set; get; }
        public ClientAccountType Type { set; get; }        
    }
}
