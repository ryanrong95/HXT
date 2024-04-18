using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Models
{
    
    public interface IClientBalance 
    {
        string InputID { get; }
        Needs.Underly.ClientAccountType Type { set; get; }
        Needs.Underly.Currency Currency { set; get; }
        decimal Amount { set; get; }
        DateTime CreateDate { get; }
    }
}
