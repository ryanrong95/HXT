using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AccountTypeTransfer
    {

        public AccountType Combine(string accountType)
        {
            
            string[] types = accountType.Split(',');
            AccountType fin = (AccountType)Convert.ToInt16(types[0]);
            for (int i= 1; i < types.Length; i++)
            {
                fin |= (AccountType)Convert.ToInt16(types[i]);
            }
            return fin;
        }

        public List<int> Separate(AccountType types)
        {
            List<int> separatedTypes = new List<int>();

            if((types & AccountType.basic) > 0)
            {
                separatedTypes.Add((int)AccountType.basic);
            }

            if ((types & AccountType.normal) > 0)
            {
                separatedTypes.Add((int)AccountType.normal);
            }

            if ((types & AccountType.cash) > 0)
            {
                separatedTypes.Add((int)AccountType.cash);
            }

            if ((types & AccountType.business) > 0)
            {
                separatedTypes.Add((int)AccountType.business);
            }

            if ((types & AccountType.bank) > 0)
            {
                separatedTypes.Add((int)AccountType.bank);
            }

            if ((types & AccountType.wechat) > 0)
            {
                separatedTypes.Add((int)AccountType.wechat);
            }

            if ((types & AccountType.alipay) > 0)
            {
                separatedTypes.Add((int)AccountType.alipay);
            }

            if ((types & AccountType.offshore) > 0)
            {
                separatedTypes.Add((int)AccountType.offshore);
            }

            return separatedTypes;
        }
    }
}
