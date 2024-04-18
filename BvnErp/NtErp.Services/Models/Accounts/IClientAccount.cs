using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Models
{
   
    public interface IClientAccount
    {
        Needs.Underly.Currency Currency { set; get; }
        Needs.Underly.InputSource Source { set; get; }
        Needs.Underly.ClientAccountType Type { set; get; }
        string Period { set; get; }
        decimal Amount { set; get; }
        DateTime CreateDate { set; get; }
    }
}
