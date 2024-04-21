using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Payments.Models;
using Yahv.Payments.Tools;
using Yahv.Underly;

namespace Yahv.Payments
{
    public class _Test
    {

        public _Test()
        {

            System.IO.Path.Combine();

            string payer = "北京远大创新科技有限公司";
            string payee = "山东汉旗科技有限公司";

            //账户

            //PaymentManager.Npc[payer, payee]["业务"].Digital.Cost(Currency.Unknown, 1, "asdf");
            //PaymentManager.Npc[payer, payee]["业务"].Digital.Recharge(Currency.Unknown, 1, "bank", "code");

            //Console.WriteLine(PaymentManager.Npc[payer, payee].Digital[Currency.Unknown].Available);

            //信用

            Console.WriteLine(PaymentManager.Npc[payer, payee]["业务"].Credit["杂费"][Currency.HKD].Available);
            PaymentManager.Npc[payer, payee]["业务"].Credit["杂费"].Credit(Currency.Unknown, 1);



            //PaymentManager.Npc[payer, payee]["业务"].Receivable["杂费"]
            //    .XdtRecord("vastOrderID", "tinyOrderID", new XdtFee[10]);

            //调用

            //PaymentManager.Npc[payer, payee]["业务"].Credit.For("", "", "").Cost(Currency.CNY, 1000);
            //PaymentManager.Npc[payer, payee]["业务"].Credit["杂费", "车场费"].For("").Cost(Currency.CNY, 1);
            //PaymentManager.Npc[payer, payee].CreditRepay.Repay(Currency.HKD, 50, null);

            //充值
            //花费
            //实收
            //PaymentManager.Npc[payer, payee]["业务"].Digital.Recharge(Currency.Unknown, 1, "bank", "code");
            //PaymentManager.Npc[payer, payee]["业务"].Digital.Cost(Currency.Unknown, 1, "asdf");//完成流水账+实收
            //PaymentManager.Npc[payer, payee]["Business"].Digital.For("").Cost();
            //PaymentManager.Npc[payer, payee]["Business"].Digital["杂费", "车场费"].For("").Cost();
            //PaymentManager.Npc[payer, payee]["Business"].Digital[Currency.CNY].Available;

            //PaymentManager.Npc.AnonymPayer("付款客户", payee)["Business"].Receivable["杂费", "车场费"].Record();

            //其他


            PaymentManager.Erp("adminid")[payer, payee]["业务"].Credit["杂费"].Credit(Currency.Unknown, 1);
            PaymentManager.Site("siteUserID")[payer, payee]["业务"].Credit["杂费"].Credit(Currency.Unknown, 1);

            Console.WriteLine(PaymentManager.Erp("adminid")[payer, payee]["业务"].DebtTerm["杂费"].ExchangeType);

            //查看是否逾期
            var ss = PaymentManager.Erp("adminid")[payer, payee]["业务"].DebtTerm[DateTime.Now].IsOverdue;
            var sss = PaymentManager.Erp("adminid")[payer, payee]["业务"].DebtTerm[DateTime.Now, "分类"].IsOverdue;

            //应收记账
            //PaymentManager.Npc["DBAEAB43B47EB4299DD1D62F764E6B6A", "9F6E6800CFAE7749EB6C486619254B9C"]["代仓储"].Receivable["清关费"].Record(Currency.HKD, 50, "orderId00001");

            //PaymentManager.Npc[payer,payee]["代仓储"].Receivable["科目"].Record();


            //应收
            //PaymentManager.Npc[payer, payee]["代仓储"].Receivable["清关费"].Record(Currency.HKD, 50, "orderId00001");

            //应付
            //PaymentManager.Npc[payer, payee]["代仓储"].Payable["科目"].Record(Currency.HKD, 50, "orderId00001");
            //PaymentManager.Npc[payer, payee]["代仓储"].Payable["科目"].Record<PvbCrmReponsitory>(Currency.HKD, 50, "orderId00001");
            //实付
            //PaymentManager.Npc[payer, payee]["代仓储"].Payment["Paybl20190925000007"].Record<PvbErmReponsitory>();

            Console.WriteLine(ExchangeRates.Current.Count());
            Console.WriteLine(ExchangeRates.Fixed[Currency.CNY, Currency.USD]);




            //PaymentManager.Npc[payer, payee]["代仓储"].Receivable["科目"].Record(Currency.HKD, 50, "orderId00001");
            //PaymentManager.Npc[payer, payee]["代仓储"].Receivable["杂费", "inputname"].Record(Currency.HKD, 50, "orderId00001");
            //PaymentManager.Npc[payer, payee]["代仓储"].Receivable.RecordOneVehicle();

            //PaymentManager.Npc[payer, payee]["代仓储"].Receivable["杂费", "包车费"].Record(Currency.CNY, 1200, "orderId00001");

            //Payments[payer, payee]["业务"].Account["杂费", ""].Cost();
            //var kkkk = Payments[payer, payee]["业务"].Account["杂费"][Currency.CNY].Available;

            //ExchangeRates.Current[ ]

            //PayTools.Payable | Recievable[“业务”][“杂费，基本服务费”] 返回 币种与Price

            //CouponManager。
        }
    }
}
