using Needs.Underly;
using NtErp.Wss.Oss.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Converters;

namespace NtErp.Wss.Oss.Services.Extends
{
    static public class ClientsExtends
    {
        /// <summary>
        /// 获取余额
        /// </summary>
        /// <param name="client">客户</param>
        /// <param name="currency">币种</param>
        /// <param name="type">账户类型</param>
        /// <returns></returns>
        static public decimal GetBalance(this Models.ClientTop client, Currency currency, UserAccountType type)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                var linq_outputs = from one in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserOutputs>()
                                   where one.Type == (int)type
                                    && one.Currency == (int)currency
                                    && one.ClientID == client.ID
                                   select one.Amount;

                return GetTotal(client, currency, type) - linq_outputs.ToArray().Sum();
            }
        }

        /// <summary>
        /// 获取总额
        /// </summary>
        /// <param name="client">客户</param>
        /// <param name="currency">币种</param>
        /// <param name="type">账户类型</param>
        /// <returns></returns>
        static public decimal GetTotal(this Models.ClientTop client, Currency currency, UserAccountType type)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                var linq_inputs = from one in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserInputs>()
                                  where one.Type == (int)type
                                    && one.Currency == (int)currency
                                    && one.ClientID == client.ID
                                  select one.Amount;

                return linq_inputs.ToArray().Sum();
            }
        }

        /// <summary>
        /// 信用欠款总额
        /// </summary>
        /// <returns></returns>
        static public Credits[] GetDebts(this Models.ClientTop client, Currency currency)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                var linq = from one in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserOutputs>()
                           where one.Type == (int)UserAccountType.Credit && one.Currency == (int)currency
                            && one.ClientID == client.ID && one.From == (int)OutputTo.Pay
                           group one by one.DateIndex into groups
                           where groups.Sum(item => item.Amount) > 0
                           select new Debts
                           {
                               DateIndex = groups.Key.Value,
                               Debt = groups.Sum(item => item.Amount)
                           };

                var repaids = GetCreditRepayment(client, currency);

                return linq.ToList().Select(item => new Credits { DateIndex = item.DateIndex, Total = item.Debt, Repaid = repaids.Where(tem => tem.DateIndex == item.DateIndex).FirstOrDefault()?.Debt ?? 0m }).ToArray();
            }
        }


        static public DebtStat GetDebtStat(this Models.ClientTop client, Currency currency, int dataindex)
        {
            var v = GetDebt(client, currency, dataindex);
            return new DebtStat { CreditDebt = GetDebt(client, currency, dataindex).Debt, UserBlance = GetBalance(client, currency, UserAccountType.Cash) };
        }

        static public Debts[] GetCreditRepayment(this Models.ClientTop client, Currency currency)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                var linq = from one in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserOutputs>()
                           where one.Type == (int)UserAccountType.Credit && one.Currency == (int)currency && one.From == 2
                            && one.ClientID == client.ID
                           group one by one.DateIndex into groups
                           select new Debts
                           {
                               DateIndex = groups.Key.Value,
                               Debt = groups.Sum(item => Math.Abs(item.Amount))
                           };

                return linq.ToArray();
            }
        }

        static public Repaid[] GetRepaids(this Models.ClientTop client, Currency currency, int dataindex)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                var linq = from one in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserOutputs>()
                           where one.Type == (int)UserAccountType.Credit && one.Currency == (int)currency && one.From == 2 && one.DateIndex == dataindex
                            && one.ClientID == client.ID
                           select new Repaid
                           {
                               Datetime = one.CreateDate,
                               Debt = Math.Abs(one.Amount)
                           };

                return linq.ToArray();
            }
        }

        static public int GetDateIndex(this DateTime datetime)
        {
            return int.Parse(datetime.AddHours(1).ToString("yyyyMM"));
        }

        /// <summary>
        /// 信用欠款
        /// </summary>
        static public Debts GetDebt(this Models.ClientTop client, Currency currency, int dateIndex)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                var linq = from one in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserOutputs>()
                           where one.Type == (int)UserAccountType.Credit && one.Currency == (int)currency
                            && one.ClientID == client.ID && one.DateIndex == dateIndex
                           group one by one.DateIndex into groups
                           where groups.Sum(item => item.Amount) > 0
                           select new Debts
                           {
                               DateIndex = groups.Key.Value,
                               Debt = groups.Sum(item => item.Amount),

                           };

                return linq.SingleOrDefault() ?? new Debts { DateIndex = dateIndex, Debt = 0m };
            }
        }

        /// <summary>
        /// 信用总额
        /// </summary>
        static public Debts GetDebtTotal(this Models.ClientTop client, Currency currency, int dateIndex)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                var linq = from one in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserOutputs>()
                           where one.Type == (int)UserAccountType.Credit && one.Currency == (int)currency
                            && one.ClientID == client.ID && one.DateIndex == dateIndex && one.From == (int)OutputTo.Pay
                           group one by one.DateIndex into groups
                           where groups.Sum(item => item.Amount) > 0
                           select new Debts
                           {
                               DateIndex = groups.Key.Value,
                               Debt = groups.Sum(item => item.Amount),

                           };

                return linq.SingleOrDefault();
            }
        }

        /// <summary>
        /// 借款 （信用）
        /// </summary>
        static public bool Borrow(this Models.ClientTop client, decimal price, Models.Order order)
        {
            Currency curreny = order.Beneficiary.Currency;
            var value = client.GetBalance(curreny, UserAccountType.Credit);
            if (value <= 0)
            {
                return false;
            }

            if (value < price)
            {
                return false;
            }

            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
                    ClientID = client.ID,
                    Type = (int)UserAccountType.Credit,
                    From = (int)OutputTo.Pay,
                    OrderID = order.ID,
                    UserInputID = null,
                    Currency = (int)curreny,
                    Amount = price,
                    DateIndex = DateTime.Now.GetDateIndex(),
                    CreateDate = DateTime.Now
                });
            }

            return true;
        }

        /// <summary>
        /// 减低债务（减少信用花费） 
        /// </summary>
        static public bool Reduces(this Models.ClientTop client, decimal price, Models.Order order)
        {
            decimal left = price;
            var debt = client.GetDebt(order.Beneficiary.Currency, order.CreateDate.GetDateIndex());

            if (price <= 0)
            {
                return false;
            }

            if (price > debt.Debt)
            {
                return false;
            }

            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
                    ClientID = client.ID,
                    Type = (int)UserAccountType.Credit,
                    From = (int)OutputTo.Pay,
                    OrderID = order.ID,
                    UserInputID = null,
                    Currency = (int)order.Beneficiary.Currency,
                    Amount = 0 - Math.Abs(price),
                    DateIndex = order.CreateDate.GetDateIndex(),
                    CreateDate = DateTime.Now,
                    //AdminID = Needs.Overall.Inner.Current.ID,
                });
            }

            return true;
        }

        /// <summary>
        /// 退款 （现金）
        /// </summary>
        static public void Refund(this Models.ClientTop client, decimal price, Models.Order order)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
                    ClientID = client.ID,
                    Type = (int)UserAccountType.Cash,
                    From = (int)OutputTo.Pay,
                    OrderID = order.ID,
                    UserInputID = null,
                    Currency = (int)order.Beneficiary.Currency,
                    Amount = 0 - Math.Abs(price),
                    DateIndex = null,
                    CreateDate = DateTime.Now
                });
            }
        }

        static public int[] GetDebtIndexes(this Models.ClientTop client)
        {
            using (var view = new UserOutputsView())
            {
                var arry = view.Where(item => item.ClientID == client.ID
                    && item.Type == UserAccountType.Credit)
                    .Select(item => item.DateIndex ?? 0).Distinct()
                    .OrderByDescending(item => item).ToArray();
                return arry;
            }
        }

        static public CreditItemGroup[] GetDebts(this Models.ClientTop client, int dateIndex)
        {
            using (var view = new UserOutputsView())
            {
                var linq1 = from item in view
                            where item.ClientID == client.ID
                            && item.Type == UserAccountType.Credit
                            && item.DateIndex == dateIndex
                            select new CreditItem
                            {
                                Currency = item.Currency,
                                From = item.From,
                                CreateDate = item.CreateDate,
                                OrderID = item.OrderID,
                                Prcie = item.Amount,
                                DateIndex = (int)item.DateIndex
                            };
                var arry = linq1.ToArray();
                var rlinq = from item in arry
                            group item by item.Currency into groups
                            select new CreditItemGroup
                            {
                                Currency = groups.Key,
                                Items = groups.ToArray(),
                                DateIndex = dateIndex
                            };

                return rlinq.ToArray();
            }
        }

        static public Account[] GetBalances(this Models.ClientTop client, UserAccountType type)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                var linq_outputs = from one in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserOutputs>()
                                   where one.Type == (int)type
                                     && one.ClientID == client.ID
                                   group one by one.Currency into groups
                                   select new Account
                                   {
                                       Currency = (Currency)groups.Key,
                                       Price = groups.Sum(item => item.Amount)
                                   };

                var arry = linq_outputs.ToArray();

                return client.GetTotals(type).Select(item => new Account
                {

                    Currency = item.Currency,
                    TotalPrice = item.Price.Twoh(),
                    OutPutPrice = (arry.SingleOrDefault(ouput => ouput.Currency == item.Currency)?.Price ?? 0m).Twoh(),
                    Price = (item.Price - (arry.SingleOrDefault(ouput => ouput.Currency == item.Currency)?.Price ?? 0m)).Twoh()
                }).ToArray();
            }
        }
        static public Account[] GetTotals(this Models.ClientTop client, UserAccountType type)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                var linq_inputs = from one in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserInputs>()
                                  where one.Type == (int)type
                                    && one.ClientID == client.ID
                                  group one by one.Currency into groups
                                  select new Account
                                  {
                                      Currency = (Currency)groups.Key,
                                      Price = groups.Sum(item => item.Amount)
                                  };

                return linq_inputs.ToArray();
            }
        }

        /// <summary>
        /// 充值 （现金）
        /// </summary>
        /// <param name="price">额度(可以负值)</param>
        /// <param name="code">流水号</param>
        static public void Recharge(this Models.ClientTop client, Currency currency, decimal price, string code = null)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserInputs
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserInput),
                    ClientID = client.ID,
                    Type = (int)UserAccountType.Cash,
                    From = (int)InputFrom.Cach,
                    Currency = (int)currency,
                    Amount = price,
                    CreateDate = DateTime.Now,
                    Code = code,
                });
            }
        }

        /// <summary>
        /// 批复 （额度）
        /// </summary>
        /// <param name="price">额度(可以负值)</param>
        /// <param name="code">流水号</param>
        static public void Approve(this Models.ClientTop client, Currency currency, decimal price, string code = null)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserInputs
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserInput),
                    ClientID = client.ID,
                    Type = (int)UserAccountType.Credit,
                    From = (int)InputFrom.Credit,
                    Currency = (int)currency,
                    Amount = price,
                    CreateDate = DateTime.Now,
                    Code = code,
                });
                //Needs.Overall.Inner.Current.ID;
            }
        }

        static public int GetPayingCount(this Models.ClientTop client)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                return reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Orders>().Count(t => t.ClientID == client.ID && t.Status == (int)OrderStatus.Paying);
            }
        }

        static public int GetCompletedCount(this Models.ClientTop client)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                return reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Orders>().Count(t => t.ClientID == client.ID && t.Status == (int)OrderStatus.Completed);
            }
        }
    }
}
