using Needs.Underly;
using NtErp.Wss.Oss.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    public class Beneficiaries
    {
        Beneficiary Mainland;
        Beneficiary HK;
        Beneficiary Other;
        Beneficiary TT;

        public Beneficiary this[Needs.Underly.District index]
        {
            get
            {
                switch (index)
                {
                    case Needs.Underly.District.Unknown:
                        throw new Exception();
                    case Needs.Underly.District.CN:
                        return this.Mainland;
                    case Needs.Underly.District.Global:
                    case Needs.Underly.District.HK:
                        return this.HK;
                    case Needs.Underly.District.IN:
                    case Needs.Underly.District.US:
                    default:
                        return this.Other;
                }
            }
        }
        public Beneficiary Get(UserAccountType type, Currency currency)
        {
            if (type == UserAccountType.TT)
            {
                this.TT.Currency = currency;
                return this.TT;
            }

            if (currency == Currency.CNY)
            {
                return this.Mainland;
            }
            else if (currency == Currency.HKD)
            {
                return this.HK;
            }
            else
            {
                return this.Other;
            }
        }

        public Beneficiaries()
        {
            this.Mainland = new Models.Beneficiary
            {
                Account = "19052901040012474",
                Bank = "农业银行杭州物流中心支行",
                SwiftCode = "",
                Address = "",
                Contact = null,
                Company = new Company
                {
                    Address = "",
                    Code = "",
                    Name = "杭州比一比电子科技有限公司"
                },
                Currency = Needs.Underly.Currency.CNY,
                Methord = Methord.Remittance
            };
            this.HK = new Beneficiary
            {
                Account = "250-390-16942094 (HKD)",
                Bank = "Citibank (HongKong) Limited",
                SwiftCode = "CITIHKAX",
                Address = "11/F Citi Tower, One Bay East 83 Hoi Bun Road, Kwun Tong, Kowloon HongKong",
                Contact = null,
                Company = new Company
                {
                    Address = "",
                    Code = "",
                    Name = "IC360 GROUP LIMITED"
                },
                Currency = Needs.Underly.Currency.HKD,
                Methord = Methord.Remittance
            };
            this.Other = new Beneficiary
            {
                Account = "250-390-16942094 (USD)",
                Bank = "Citibank (HongKong) Limited",
                SwiftCode = "CITIHKAX",
                Address = "11/F Citi Tower, One Bay East 83 Hoi Bun Road, Kwun Tong, Kowloon HongKong",
                Contact = null,
                Company = new Company
                {
                    Address = "",
                    Code = "",
                    Name = "IC360 GROUP LIMITED"
                },
                Currency = Needs.Underly.Currency.USD,
                Methord = Methord.Remittance
            };

            this.TT = new Beneficiary
            {
                Account = "012-591-200-50598",
                Bank = "Bank of China（HongKong）Limited",
                SwiftCode = "",
                Address = "",
                Contact = null,
                Company = new Company
                {
                    Address = "",
                    Code = "",
                    Name = "B ONE B ELECTRONICS CO., LIMITED"
                },
                Currency = Needs.Underly.Currency.Unkown,
                Methord = Methord.Remittance
            };
        }

        static object locker = new object();
        static Beneficiaries current;
        static public Beneficiaries Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Beneficiaries();
                        }
                    }
                }
                return current;
            }
        }

    }
}
