using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yahv.PvData.ExchangeRates
{
    class Program
    {
        static void Main(string[] args)
        {
            new Services.ExchangeRateCN().Crawling();

            Console.WriteLine("汇率抓取程序启动...");
            Console.ReadKey();
        }

        static void TestInsert()
        {
            using (var context = new Sqls.PvData.sqlDataContext())
            {
                var rates1 = context.ExchangeRates.ToArray();
                var rates2 = rates1.Select(rate => new Sqls.PvData.ExchangeRates()
                {
                    Type = rate.Type,
                    District = 2,
                    From = rate.From,
                    To = rate.To,
                    Value = rate.Value,
                    StartDate = rate.StartDate,
                    ModifyDate = rate.ModifyDate
                });
                context.ExchangeRates.InsertAllOnSubmit(rates2);
                context.SubmitChanges();
            }
        }

        static void TestUpdate()
        {
            using (var context = new Sqls.PvData.sqlDataContext())
            {
                var rate = context.ExchangeRates.First();
                rate.StartDate = DateTime.Now;
                context.SubmitChanges();
            }
        }
    }
}
