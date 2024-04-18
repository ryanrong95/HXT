using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Models
{

    public interface IUserInput
    {
        string InputID { get; }
        Needs.Underly.ClientAccountType Type { set; get; }
        Needs.Underly.InputSource Source { set; get; }
        Needs.Underly.Currency Currency { set; get; }
        decimal Amount { set; get; }
        string Code { set; get; }
        DateTime CreateDate { get; }
    }
}
