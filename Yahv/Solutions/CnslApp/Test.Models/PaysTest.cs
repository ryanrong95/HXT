using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json;
using Yahv.Payments;
using Yahv.Payments.Models;
using Yahv.Payments.Models.Rolls;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
//using Admin = Yahv.RFQ.Services.Models.Admin;

namespace CnslApp.Test.Models
{
    public class PaysTest
    {
        private static string payer = "中山天祥光电科技有限公司";
        private static string payee = "深圳市芯达通供应链管理有限公司";
        //private static string payee = "香港万路通国际物流有限公司";
        private static string conduct = "供应链";
        private static string adminId = "SA01";

        public static void Test()
        {
            Yahv.Services.Initializers.WhsBoot();
            Yahv.Services.Initializers.OrderBoot();

            #region 科目

            //var subject = SubjectManager.Current[conduct][CatalogConsts.货款];

            #endregion

            #region 工具费用

            //var ss = PaymentTools.Receivables[conduct, "杂费", "清关费有入仓号"];
            //Console.WriteLine(ss.Quotes.Currency);
            //Console.WriteLine(ss.Quotes.Price); 

            #endregion

            #region 仓储费

            //PaymentManager.Npc[payer, payee][ConductConsts.代仓储]
            //    .Receivable[CatalogConsts.仓储费, SubjectConsts.仓储费].RecordStorage(Currency.CNY, 50);

            #endregion

            #region 信用支付

            //PaymentManager.Erp("SA01").Credit.For("应收ID").Pay(Currency.CNY, 1000); 

            #endregion

            #region 客户确认账单（标记仓储费）

            //PaymentManager.Erp(adminId)[payer, payee][conduct].Receivable.Confirm("NL02020200305001", Currency.CNY, true);

            #endregion

            #region 使用优惠券(注意payer和payee位置)

            //var usedMap = new UsedMap()
            //{
            //    ReceivableID = "Receb201912170006",
            //    CouponID = "Coupon201912170001",
            //    Quantity = 1,
            //};
            //CouponManager.Current["DBAEAB43B47EB4299DD1D62F764E6B6A", "E412B9866786518849AAE9674D57F373"].Confirm(adminId, usedMap);

            #endregion

            #region 应收实收一体（匿名）

            ////应收、实收
            //PaymentManager.Erp(adminId)[payer, payee][conduct].Receivable["杂费", "包车费"]
            //.Record(Currency.CNY, 150, "NL02020200710001", source: "香港库房", rightPrice: 150);

            #endregion

            #region 应收(包含源金额)

            //PaymentManager.Erp(adminId)[payer, payee][conduct].Receivable["杂费", "包车费"]
            //.Record(Currency.CNY, 150, "WL02520210707020", originPrice: 120);

            #endregion

            #region 应付实付一体（匿名）

            ////应付、实付
            //PaymentManager.Erp(adminId)[payer, payee][conduct].Payable["杂费", "提货费"]
            //.Record(Currency.CNY, 50, "NL02020200305001", rightPrice: 50);

            #endregion

            #region 租赁费

            //PaymentManager.Npc["97D43F32EBC092B417F4838771EA7F69", "DBAEAB43B47EB4299DD1D62F764E6B6A"][conduct]
            //    .Receivable["杂费", "库位租赁费"].Record(Currency.CNY, 1000, "LsOrder201912190008");


            #endregion

            #region 现金余额

            //var ss = PaymentManager.Erp(adminId)[payer, payee][conduct].Digital[Currency.CNY].Available;

            #endregion

            #region 多订单财务确认

            //PaymentManager.Erp(adminId).Received.For("Receb201912250023", "Receb201912250024").Confirm(new VoucherInput()
            //{
            //    CreateDate = DateTime.Now,
            //    Type = VoucherType.Receipt,
            //    Payer = "A434BE582059F6C1624549B6231A1502",
            //    Payee = "DBAEAB43B47EB4299DD1D62F764E6B6A",
            //    Bank = "招商银行",
            //    Account = "6225998292829282",
            //    Beneficiary = "54D64B381E837CC6642A1F44301D3565",
            //    CreatorID = adminId,
            //    FormCode = "FormCode",
            //    Business = Business.WarehouseServicing.GetDescription(),

            //    Currency = Currency.CNY,
            //    Price = 200,
            //});

            #endregion

            #region 是否逾期

            //var overDue = PaymentManager.Npc["A434BE582059F6C1624549B6231A1502", "DBAEAB43B47EB4299DD1D62F764E6B6A"][ConductConsts.代仓储].DebtTerm[DateTime.Now.AddYears(2)].IsOverdue;

            #endregion

            #region 代付货款

            ////万路通对客户进行收款
            //var recebId = PaymentManager.Erp(adminId)["杭州比一比电子科技有限公司", "香港万路通国际物流有限公司"][conduct].Receivable[CatalogConsts.货款, SubjectConsts.代付货款]
            //     .Record(Currency.CNY, 1500, "NL02020200107002", applicationID: "applicationID");

            ////客户向万路通付款
            //PaymentManager.Erp(adminId).Received.For("Receb201912300003").Confirm(new VoucherInput()
            //{
            //    CreateDate = DateTime.Now,
            //    Type = VoucherType.Receipt,
            //    Payer = "A434BE582059F6C1624549B6231A1502",       //客户
            //    Payee = "10528B3C5358D2A78B32DD90F911C13B",       //万路通
            //    Bank = "招商银行",
            //    Account = "6225998292829282",
            //    Beneficiary = "54D64B381E837CC6642A1F44301D3565",
            //    CreatorID = adminId,
            //    FormCode = "FormCode",
            //    Business = Business.WarehouseServicing.GetDescription(),

            //    Currency = Currency.CNY,
            //    Price = 1500,
            //});

            ////万路通对供应商进行应付
            //var payblId = PaymentManager.Erp(adminId)["10528B3C5358D2A78B32DD90F911C13B",     //万路通
            //  "667957328F0A0EA1B0FBD00AADD2379D"      //供应商
            //  ][conduct].Payable[CatalogConsts.货款, SubjectConsts.代付货款]
            //      .Record(Currency.CNY, 1500, "NL02020200107002", AgentID: payer.MD5());

            ////对供应商进行实付，并将客户余额扣除
            //PaymentManager.Erp(adminId).Payment.For(payblId).Confirm(new VoucherInput()
            //{
            //    CreateDate = DateTime.Now,
            //    Type = VoucherType.Payment,
            //    Payer = "10528B3C5358D2A78B32DD90F911C13B",       //万路通
            //    Payee = "667957328F0A0EA1B0FBD00AADD2379D",       //供应商
            //    Bank = "招商银行",
            //    Account = "6225998292829282",
            //    Beneficiary = "54D64B381E837CC6642A1F44301D3565",
            //    CreatorID = adminId,
            //    FormCode = "FormCode",
            //    Business = Business.WarehouseServicing.GetDescription(),

            //    Currency = Currency.CNY,
            //    Price = 1500,
            //});

            #endregion

            #region 代收货款

            ////万路通对客户的客户进行收款
            //PaymentManager.Erp(adminId).AnonymPayer("匿名付款人", payee)[conduct]
            //    .Receivable[CatalogConsts.货款, SubjectConsts.代收货款]
            //    .Record(Currency.CNY, 10000, "NL02020200108001", AgentID: payer.MD5());

            ////万路通对客户的客户进行收款确认
            //PaymentManager.Erp(adminId).Received.For("Receb202001100004").Confirm(new VoucherInput()
            //{
            //    CreateDate = DateTime.Now,
            //    Type = VoucherType.Receipt,
            //    Payee = "DBAEAB43B47EB4299DD1D62F764E6B6A",       //万路通
            //    Bank = "招商银行",
            //    Account = "6225998292829282",
            //    CreatorID = adminId,
            //    FormCode = "FormCode",
            //    Business = Business.WarehouseServicing.GetDescription(),

            //    Currency = Currency.CNY,
            //    Price = 10000,
            //});
            #endregion

            #region 应收重记
            //PaymentManager.Erp(adminId)[payer, payee][conduct].Receivable.For("Receb202001020031").ReRecord(1000);
            #endregion

            #region 修改应收本位币
            //PaymentManager.Erp(adminId)[payer, payee][conduct].Receivable.For("Receb202011090006").ModifyRmb(9.035m);
            #endregion

            #region 应收（匿名）

            //PaymentManager.Erp(adminId).AnonymPayer("匿名付款人", payee)[conduct]
            //    .Receivable[CatalogConsts.杂费, SubjectConsts.停车费]
            //    .Record(Currency.CNY, 10, "NL02020200521001");

            #endregion

            #region 应付（匿名）

            //payer = "香港畅运国际物流有限公司";
            //PaymentManager.Erp(adminId).AnonymPayee("匿名收款人", payer)[conduct]
            //    .Payable[CatalogConsts.杂费, SubjectConsts.停车费]
            //    .Record(Currency.CNY, 10, "NL02020200521001");

            #endregion

            #region 取消减免（收款）
            //PaymentManager.Erp("SA01").Received.
            //              ReductionCancel("Reced202002060001");
            #endregion

            #region 可用金额

            //var ss=PaymentManager.Npc["A434BE582059F6C1624549B6231A1502", "DBAEAB43B47EB4299DD1D62F764E6B6A"]["代仓储"].Digital[Currency.CNY].Available;

            #endregion

            #region 废弃账单
            //PaymentManager.Erp(adminId).Received.Abolish("NL02020200108001");
            #endregion

            #region 应付（通过申请添加）

            //PaymentManager.Npc[payee, payer][conduct].Payable["杂费", "提货费"]
            //    .Record(Currency.CNY, 1000, applicationID: "apply01");

            #endregion

            #region 应收（脱离订单）

            //PaymentManager.Npc[payer, payee][conduct].Receivable["杂费", "提货费"]
            //    .Record(Currency.CNY, 100, applicationID: Guid.NewGuid().ToString("n"));

            #endregion

            #region 银行收款
            //银行收款
            //PaymentManager.Erp(adminId)[payer, payee].Digital
            //    .AdvanceFromCustomers(Currency.CNY, 20, "招商银行", "6225998292829282", "2020032900005", DateTime.Now);
            #endregion

            #region 删除应收
            //PaymentManager.Npc.Received.For("Receb202004010018").Delete();
            #endregion



            #region 芯达通 应收
            //XdtFee[] array = new XdtFee[]
            //    {
            //       //new XdtFee(CatalogConsts.税款,SubjectConsts.关税,Currency.CNY, 0,"OrderItem20200509000005"),
            //       //new XdtFee(CatalogConsts.税款,SubjectConsts.销售增值税,Currency.CNY, 1.56m,"OrderItem20200509000005"),
            //       //new XdtFee(CatalogConsts.代理费,SubjectConsts.代理费,Currency.CNY, 0.13038m,"OrderItem20200509000005"),
            //       new XdtFee(CatalogConsts.货款,null,Currency.CNY, 5000,"OrderItem20200509000006"),
            //       new XdtFee(CatalogConsts.税款,SubjectConsts.销售增值税,Currency.CNY, 13,"OrderItem20200509000006"),
            //       new XdtFee(CatalogConsts.代理费,SubjectConsts.代理费,Currency.CNY, 1.06m,"OrderItem20200509000006"),
            //    };
            //PaymentManager.Erp(adminId)[payer, payee][conduct]
            //    .Receivable.XdtRecord("NL02020200518001", "NL02020200518001-01", 1
            //    , array: array);

            //using (StreamReader sr1 = new StreamReader("../../Data/XL00220200619507-04.json"))
            //{
            //    string json1 = sr1.ReadToEnd();
            //    List<XdtFee> list1 = JsonConvert.DeserializeObject<List<XdtFee>>(json1);

            //    Stopwatch sw = new Stopwatch();
            //    sw.Start();

            //    PaymentManager.Erp(adminId)[payer, payee][conduct]
            //        .Receivable.XdtRecord("NL02020200630001", "NL02020200630001-01", 1
            //        , array: list1.ToArray());

            //    sw.Stop();
            //    Console.WriteLine($"用时：{sw.Elapsed.TotalSeconds}秒!");
            //}

            //using (StreamReader sr1 = new StreamReader("../../Data/XL00220200619507-04.json"))
            //{
            //    string json1 = sr1.ReadToEnd();
            //    List<XdtFee> list1 = JsonConvert.DeserializeObject<List<XdtFee>>(json1);

            //    Stopwatch sw = new Stopwatch();
            //    sw.Start();

            //    Parallel.For(0, 5, (i, state) =>
            //    {
            //        PaymentManager.Erp(adminId)[payer, payee][conduct]
            //           .Receivable.XdtRecord("NL02020200710001", "NL02020200710001-" + i, 1
            //           , array: list1.ToArray());
            //    });

            //    sw.Stop();
            //    Console.WriteLine($"用时：{sw.Elapsed.TotalSeconds}秒!");
            //}
            #endregion

            #region 芯达通 对账单实收[非货款核销]

            //PaymentManager.Erp(adminId).Received.For("NL02020200630001-01")       //小订单ID
            //    .Confirm_XinDaTong(new VoucherInput()
            //    {
            //        Type = VoucherType.Receipt,         //收款类型
            //        Business = ConductConsts.代报关,       //业务
            //        CreatorID = adminId,
            //        CreateDate = DateTime.Now,

            //        Payer = payer,
            //        Payee = payee,
            //        //FormCode = Guid.NewGuid().ToString("N"),
            //        FormCode = "213213432",
            //        Bank = "平安银行东乐支行",
            //        Account = "11012581098202",

            //        Currency = Currency.CNY,
            //        //Price = 1000,
            //    }, new XdtFee[]
            //    {
            //       //new XdtFee(CatalogConsts.税款,SubjectConsts.关税,Currency.CNY, 168.37m),
            //       //new XdtFee(CatalogConsts.税款,SubjectConsts.销售增值税,Currency.CNY, 8497.49m),
            //       //new XdtFee(CatalogConsts.代理费,SubjectConsts.代理费,Currency.CNY, 207.08m),
            //       new XdtFee(CatalogConsts.税款,SubjectConsts.关税,Currency.CNY, 68.37m),
            //       new XdtFee(CatalogConsts.税款,SubjectConsts.销售增值税,Currency.CNY, 497.49m),
            //       new XdtFee(CatalogConsts.代理费,SubjectConsts.代理费,Currency.CNY, 107.08m),
            //    });
            #endregion

            #region 芯达通 付汇实收[货款核销]

            //XdtFee[] array1 = new XdtFee[]
            //    {
            //       //new XdtFee(CatalogConsts.税款,SubjectConsts.关税,Currency.CNY, 100),
            //       //new XdtFee(CatalogConsts.税款,SubjectConsts.销售增值税,Currency.CNY, 200),
            //       //new XdtFee(CatalogConsts.税款,SubjectConsts.消费税,Currency.CNY, 300),
            //       //new XdtFee(CatalogConsts.代理费,SubjectConsts.代理费,Currency.CNY, 400),
            //       new XdtFee(CatalogConsts.货款,null,Currency.CNY, 510)
            //       //new XdtFee(CatalogConsts.杂费,SubjectConsts.商检费,Currency.CNY, 10),
            //    };

            //PaymentManager.Erp(adminId)[payer, payee][conduct]
            //    .Receivable.XdtRecord("NL02020200401009", "NL02020200329001"
            //    , itemID: "itemID", applicationID: "applyID"
            //    , array: array1);

            //PaymentManager.Erp(adminId).Received.For("NL02020200518001-01")       //小订单ID
            //    .Confirm_Remit_XinDaTong(new VoucherInput()
            //    {
            //        Type = VoucherType.Receipt,         //收款类型
            //        Business = ConductConsts.代报关,       //业务
            //        CreatorID = adminId,
            //        CreateDate = DateTime.Now,

            //        Payer = payer,
            //        Payee = payee,
            //        FormCode = "2020060800001",
            //        Bank = "平安银行东乐支行",
            //        Account = "11012581098202",

            //        Currency = Currency.CNY,
            //        //Price = 500,
            //    }, new XdtFee[]
            //    {
            //        //new XdtFee(CatalogConsts.货款,null,Currency.CNY, 5000)
            //        new XdtFee(CatalogConsts.代理费,SubjectConsts.代理费,Currency.CNY, 1.06m),
            //        new XdtFee(CatalogConsts.税款,SubjectConsts.销售增值税,Currency.CNY, 13)
            //    });

            #endregion

            #region 芯达通 取消实收

            //PaymentManager.Erp(adminId).Received.Abandon("123158998554");

            //PaymentManager.Erp(adminId).Received.Abandon("NL02020200710001-01", CatalogConsts.货款, CatalogConsts.货款, 200, Currency.HKD);
            #endregion

            #region 申请记账
            //ApplyFee[] array = new ApplyFee[]
            //    {
            //       new ApplyFee("NL02020200710001",CatalogConsts.货款,SubjectConsts.代付货款,100),
            //       new ApplyFee("NL02020200630001",CatalogConsts.货款,SubjectConsts.代付货款,200),
            //       new ApplyFee("NL02020200604001",CatalogConsts.货款,SubjectConsts.代付货款,300),
            //    };

            //PaymentManager.Erp(adminId)[payer, payee][conduct]
            //    .Receivable.ApplyRecord(Currency.CNY, "apply01", array);
            #endregion

            #region 核销应收
            //PaymentManager.Erp(adminId).Received.For("Receb202010220016", "Receb202010220017", "Receb202010220018").Confirm(new VoucherInput()
            //{
            //    CreateDate = DateTime.Now,
            //    Type = VoucherType.Receipt,
            //    Payer = payer.MD5(),
            //    Payee = payee.MD5(),
            //    Bank = "招商银行",
            //    Account = "6225998292829282",
            //    CreatorID = adminId,
            //    FormCode = "10001",
            //    Business = conduct,
            //    AccountType = AccountType.BankStatement,
            //    Currency = Currency.CNY,
            //    Price = 600,
            //});
            #endregion

            #region 核销应收RMB
            //PaymentManager.Erp(adminId).Received.For("Receb202011090003").ConfirmRmb(new VoucherInput()
            //{
            //    CreateDate = DateTime.Now,
            //    Type = VoucherType.Receipt,
            //    Payer = payer.MD5(),
            //    Payee = payee.MD5(),
            //    Bank = "平安银行",
            //    Account = "11012581098202",
            //    CreatorID = adminId,
            //    FormCode = "202011090001",
            //    Business = conduct,
            //    AccountType = AccountType.BankStatement,
            //    Currency = Currency.CNY,
            //});
            #endregion

            #region 根据申请ID废弃 核销、应收
            //PaymentManager.Erp(adminId).Received.AbolishByApplicationID("apply01");
            #endregion

            #region 核销应付
            //PaymentManager.Erp(adminId).Payment.For("Paybl202010220001").Confirm(new VoucherInput()
            //{
            //    CreateDate = DateTime.Now,
            //    Type = VoucherType.Payment,
            //    Payer = payee.MD5(),
            //    Payee = payer.MD5(),

            //    Bank = "招商银行",
            //    Currency = Currency.CNY,
            //    Account = "6225998292829282",
            //    FormCode = "10003",

            //    CreatorID = adminId,
            //    //Price = decimal.Parse(Request.Form["Price"]),
            //    Business = conduct,
            //    ApplicationID = "apply01",
            //    AccountType = AccountType.BankStatement,
            //    Price = 100,
            //});
            #endregion

            #region 应付
            //PaymentManager.Erp(adminId)[payer, payee][conduct].Payable["货款", "代付货款"]
            //               .Record(Currency.CNY, 600, applicationID: "apply01");
            #endregion

            #region 根据申请ID删除 核销、应付
            //PaymentManager.Erp(adminId).Payment.AbolishByApplicationID("apply01");
            #endregion


            #region 根据申请ID获取虚拟余额
            //Console.WriteLine(PaymentManager.Erp(adminId).Payment.GetVirtualBalance("apply01"));
            #endregion
        }
    }
}
