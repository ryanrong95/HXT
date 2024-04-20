using Needs.Underly;

namespace NtErp.Wss.Services.Generic
{
    public class Debts
    {
        public int DateIndex { get; internal set; }
        public decimal Debt { get; internal set; }
    }

    public class Account
    {
        public Currency Currency { get; internal set; }
        public decimal Price { get; internal set; }
    }
}
