using NtErp.Wss.Oss.Services.Models;
using NtErp.Wss.Oss.Services.Extends;
using NtErp.Wss.Oss.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly;
using System.Collections;

namespace NtErp.Wss.Oss.Services
{
    public class CreditItem
    {
        public Currency Currency { get; set; }
        public OutputTo From { get; set; }
        public string OrderID { get; set; }
        public decimal Prcie { get; set; }
        public DateTime CreateDate { get; set; }
        public int DateIndex { get; set; }
    }


    public class CreditItemGroup
    {
        public int DateIndex { get; set; }
        public Currency Currency { get; set; }
        public CreditItem[] Items { get; set; }
        /// <summary>
        /// 应还款
        /// </summary>
        public decimal Repaying
        {
            get
            {
                return this.Items.Where(t => t.From == OutputTo.Pay).Sum(t => t.Prcie);
            }
        }
        /// <summary>
        /// 已还款
        /// </summary>
        public decimal Repaid
        {
            get
            {
                return this.Items.Where(t => t.From == OutputTo.Repay).Sum(t => t.Prcie);
            }
        }
        public decimal Total
        {
            get
            {
                return this.Items.Sum(item => item.Prcie);
            }
        }
    }

    public class UserCredits
    {
        string clientID;

        [Obsolete("这个写法是临时的，未来会被剔除")]
        public UserCredits(string clientID)
        {
            this.clientID = clientID;
        }

        public int[] DateIndexes
        {
            get
            {
                using (var view = new UserOutputsView())
                {
                    var arry = view.Where(item => item.ClientID == this.clientID
                        && item.Type == UserAccountType.Credit)
                        .Select(item => item.DateIndex ?? 0).Distinct()
                        .OrderByDescending(item => item).ToArray();
                    return arry;
                }
            }
        }

        public CreditItemGroup[] GetItems(int index)
        {
            using (var view = new UserOutputsView())
            {
                var linq1 = from item in view
                            where item.ClientID == this.clientID
                            && item.Type == UserAccountType.Credit
                            && item.DateIndex == index
                            select new CreditItem
                            {
                                Currency = item.Currency,
                                CreateDate = item.CreateDate,
                                OrderID = item.OrderID,
                                Prcie = item.Amount,
                            };
                var arry = linq1.ToArray();
                var rlinq = from item in arry
                            group item by item.Currency into groups
                            select new CreditItemGroup
                            {
                                Currency = groups.Key,
                                Items = groups.ToArray()
                            };

                return rlinq.ToArray();
            }
        }
    }
}
