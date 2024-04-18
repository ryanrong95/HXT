using Needs.Wl.Models;

namespace Needs.Wl.Client.Services.Models
{
    public class UserPayExchangeApply : Needs.Wl.Models.PayExchangeApply
    {
        public override User User { get; set; }

        public Needs.Wl.Models.PayExchangeApplyFiles Files { get; set; }
    }
}
