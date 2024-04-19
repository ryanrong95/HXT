using Needs.Underly;
using NtErp.Wss.Oss.Services.Models;
using NtErp.Wss.Oss.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    public class Debts
    {
        public int DateIndex { get; internal set; }
        public decimal Debt { get; internal set; }


    }

    public class Repaid
    {
        public DateTime Datetime { get; internal set; }
        public decimal Debt { get; internal set; }


    }

    public class Credits
    {
        public int DateIndex { get; internal set; }
        public decimal Total { get; internal set; }

        public decimal Repaid { get; internal set; }


    }


    public class DebtStat
    {
        public decimal CreditDebt { get; internal set; }

        public decimal UserBlance { get; internal set; }
    }




    public class Account
    {
        public Currency Currency { get; internal set; }
        public decimal TotalPrice { get; internal set; }

        public decimal OutPutPrice { get; internal set; }
        public decimal Price { get; internal set; }
        public string Symbol
        {
            get
            {
                return this.Currency.GetLegal().Symbol;
            }
        }
    }

}
