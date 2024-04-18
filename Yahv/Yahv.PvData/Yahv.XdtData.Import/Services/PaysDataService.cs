using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbCrm;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Utils.Converters.Contents;
using Yahv.XdtData.Import.Connections;
using Yahv.XdtData.Import.Enums;
using Yahv.XdtData.Import.Extends;
using Yahv.XdtData.Import.Models;

namespace Yahv.XdtData.Import.Services
{
    /// <summary>
    /// 财务数据导入
    /// </summary>
    public sealed class PaysDataService : IDataService
    {
        #region 芯达通数据

        private Temp_FinanceAccounts[] financeAccountses;       //银行账户
        private Temp_FinanceReceiptsView[] financeReceipts;     //银行收款
        private Temp_OrderReceiptsView[] orderReceipts;         //应收实收

        #endregion

        #region 代仓储数据
        private Yahv.Services.Models.Admin[] admins;
        #endregion

        #region 需要保存的数据
        private List<Payees> payees = new List<Payees>();       //收款账户
        private List<FlowAccounts> flowAccounts = new List<FlowAccounts>();        //流水表 
        private List<Vouchers> vouchers = new List<Vouchers>();     //申请
        private List<Receivables> receivableses = new List<Receivables>();      //应收
        private List<Receiveds> receivedses = new List<Receiveds>();            //实收
        #endregion

        #region 自定义

        private string PayeeID = System.Configuration.ConfigurationManager.AppSettings["DeclareCompany"].MD5();        //深圳市芯达通供应链管理有限公司
        private string PayerID;        //杭州比一比电子科技有限公司
        private string PayerName;
        private string ClientID;

        private int MethordType = 3;        //汇款方式 转账
        private string Npc = "Npc-Robot";
        private string Conduct = "代报关";
        #endregion

        public PaysDataService(string clientID)
        {
            string clientName = System.Configuration.ConfigurationManager.AppSettings[clientID];
            this.ClientID = clientID;
            this.PayerID = clientName.MD5();
            this.PayerName = clientName;
        }

        public IDataService Query()
        {
            using (var reponsitory = new PvbCrmReponsitory())
            using (var adminView = new Yahv.Services.Views.AdminsAll<PvbCrmReponsitory>(reponsitory))
            {
                admins = adminView.ToArray();
                financeAccountses = reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Temp_FinanceAccounts>().ToArray();
                financeReceipts = reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Temp_FinanceReceiptsView>().ToArray();

                //DateTime begin = Convert.ToDateTime("1900-01-01");
                //DateTime end = Convert.ToDateTime("2019-10-26");

                //DateTime begin = Convert.ToDateTime("2019-10-26");
                //DateTime end = Convert.ToDateTime("2019-11-01");

                //DateTime begin = Convert.ToDateTime("2019-11-01");
                //DateTime end = Convert.ToDateTime("2019-12-01");

                //DateTime begin = Convert.ToDateTime("2019-12-01");
                //DateTime end = Convert.ToDateTime("2020-02-01");

                //DateTime begin = Convert.ToDateTime("2020-02-01");
                //DateTime end = Convert.ToDateTime("2020-04-01");

                //DateTime begin = Convert.ToDateTime("2020-04-01");
                //DateTime end = Convert.ToDateTime("2020-05-01");

                //DateTime begin = Convert.ToDateTime("2020-05-01");
                //DateTime end = Convert.ToDateTime("2020-06-06");
                //orderReceipts = reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Temp_OrderReceiptsView>().Where(item => item.ClientID == ClientID && item.OrderType == 2
                //&& item.CreateDate >= begin && item.CreateDate < end).ToArray();

                //DateTime begin = Convert.ToDateTime("2020-06-19");
                //DateTime end = Convert.ToDateTime("2020-07-01");
                //orderReceipts = reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Temp_OrderReceiptsView>().Where(item => item.ClientID == ClientID && item.OrderType == 2
                //&& item.CreateDate >= begin && item.CreateDate < end).ToArray();

                //DateTime begin = Convert.ToDateTime("2020-07-01");
                //DateTime end = Convert.ToDateTime("2020-07-08");
                //orderReceipts = reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Temp_OrderReceiptsView>().Where(item => item.ClientID == ClientID && item.OrderType == 2
                //&& item.CreateDate >= begin && item.CreateDate < end).ToArray();

                string[] ids = new string[]
                {
                    //"XL00220200805502-05",
                };
                orderReceipts = reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Temp_OrderReceiptsView>().Where(item => ids.Contains(item.OrderID)).ToArray();
            }

            return this;
        }

        #region bak
        public IDataService _bak__Encapsule()
        {
            using (var reponsitory = new PvbCrmReponsitory())
            using (var flowView = new Yahv.Services.Views.FlowAccountsTopView<PvbCrmReponsitory>(reponsitory))
            using (var vouchersView = new Yahv.Services.Views.VouchersStatisticsView<PvbCrmReponsitory>(reponsitory))
            {
                #region 收款账户
                //芯达通收款账号
                var payeesView = reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Payees>()
                    .Where(item => item.RealID == null || item.RealID == "").ToArray();

                foreach (var bank in financeAccountses)
                {
                    if (!payeesView.Any(
                            item =>
                                item.Account.Trim() == bank.BankAccount.Trim() &&
                                ((Currency)item.Currency).GetCurrency().ShortName == bank.Currency))
                    {
                        payees.Add(new Payees()
                        {
                            ID = PKeySigner.Pick(PkeyType.Payee),
                            EnterpriseID = PayeeID,
                            Methord = (int)Methord.Transfer,
                            Bank = bank.BankName,
                            BankAddress = bank.BankAddress,
                            Account = bank.BankAccount,
                            SwiftCode = bank.SwiftCode,
                            Currency = GetCurrency(bank.Currency),
                            Creator = GetAdminID(admins, bank.AdminID),
                            Status = (int)GeneralStatus.Normal,
                            CreateDate = bank.CreateDate,
                            UpdateDate = bank.UpdateDate,
                        });
                    }
                }
                #endregion

                #region 预收账款

                var flowsAdvance =
                    flowView.Where(item => item.Type == AccountType.BankStatement && item.Price > 0).ToArray();
                foreach (var receipt in financeReceipts)
                {
                    if (flowsAdvance.Any(item => item.FormCode == receipt.SeqNo))
                    {
                        continue;
                    }

                    flowAccounts.Add(new FlowAccounts()
                    {
                        ID = PKeySigner.Pick(PKeyType.FlowAccount),
                        Currency = (int)GetCurrency(receipt.Currency),
                        AdminID = GetAdminID(admins, receipt.AdminID),
                        //Business = Conduct,       //预收账款，不考虑业务
                        Payee = PayeeID,
                        Payer = PayerID,
                        Type = (int)AccountType.BankStatement,
                        Price = receipt.Amount,
                        CreateDate = receipt.CreateDate,
                        Currency1 = (int)Currency.CNY,
                        ERate1 = 1,
                        Price1 = receipt.Amount,
                        FormCode = receipt.SeqNo,
                        Bank = receipt.BankName,
                        Account = receipt.BankAccount,
                        ReceiptDate = receipt.ReceiptDate,
                    });
                }
                #endregion

                #region 应收和申请
                var voucherView = vouchersView.ToArray();
                bool isInsertVoucher = false;
                SubjectDto subject;        //分类科目
                string recevibleId = string.Empty;
                string flowId_Temp = string.Empty;
                Temp_FinanceReceiptsView finaReceipt;
                List<string> tinyOrderIds = new List<string>();

                //类型、应收ID key:OrderFeeType,value:应收ID
                //类型是5、FeeSourceID=null,表示商检费 key：5 value:应收ID
                //类型为5,、FeeSourceID!=null, key:FeeSourceID,value:应收ID
                //一个单子，非杂费，只能有一种
                Dictionary<string, string> dic = new Dictionary<string, string>();

                foreach (var mainOrderId in orderReceipts.GroupBy(item => item.MainOrderId).Select(item => item.Key))
                {
                    if (voucherView.Any(item => item.OrderID == mainOrderId))
                    {
                        continue;
                    }

                    isInsertVoucher = false;

                    tinyOrderIds = orderReceipts.Where(item => item.MainOrderId == mainOrderId)
                            .GroupBy(item => item.OrderID)
                            .Select(item => item.Key).ToList();

                    //遍历循环小订单ID 进行应收、实收
                    foreach (var orderId in tinyOrderIds)
                    {
                        //应收
                        foreach (var rec in orderReceipts.Where(item => item.MainOrderId == mainOrderId && item.OrderID == orderId && item.OrderType == 1))
                        {
                            subject = GetCatalogAndSubject(rec.OrderFeeType, rec.PremiumType);

                            //添加申请
                            if (!isInsertVoucher)
                            {
                                //一个订单添加一次申请
                                vouchers.Add(new Vouchers()
                                {
                                    ID = PKeySigner.Pick(PKeyType.Vouchers),
                                    OrderID = mainOrderId,
                                    Type = (int)VoucherType.Receipt,
                                    Payer = PayerID,
                                    Payee = PayeeID,
                                    CreatorID = GetAdminID(admins, rec.AdminID),
                                    Currency = (int)GetCurrency(rec.Currency),
                                    CreateDate = rec.CreateDate,
                                    IsSettlement = false,
                                    Status = (int)GeneralStatus.Normal,
                                });
                                isInsertVoucher = true;
                            }

                            recevibleId = PKeySigner.Pick(PKeyType.Receivables);
                            //添加应收
                            receivableses.Add(new Receivables()
                            {
                                ID = recevibleId,
                                Payer = PayerID,
                                Payee = PayeeID,
                                Business = Conduct,
                                Catalog = subject.Catalog,
                                Subject = subject.Subject,
                                TinyID = rec.OrderID,
                                //ItemID = itemID,
                                //ApplicationID = applicationID,

                                Currency = GetCurrency(rec.Currency),
                                Price = rec.Amount,

                                Quantity = rec.Count,

                                Currency1 = 1,
                                Rate1 = 1,
                                Price1 = rec.Amount,

                                SettlementCurrency = 1,
                                SettlementPrice = (rec.Amount).Round(),
                                SettlementRate = 1,

                                OrderID = mainOrderId,
                                //WaybillID = waybillID,
                                AdminID = GetAdminID(admins, rec.AdminID),

                                CreateDate = rec.CreateDate,
                                OriginalIndex = int.Parse(rec.CreateDate.ToString("yyyyMM")),
                                OriginalDate = rec.CreateDate,
                                ChangeDate = rec.CreateDate,
                                ChangeIndex = int.Parse(rec.CreateDate.ToString("yyyyMM")),

                                Status = (int)GeneralStatus.Normal,
                            });

                            //recevibleId = PaymentManager.Erp(GetAdminID(admins, rec.AdminID))[PayerName, PayeeID][Conduct]
                            //    .Receivable[subject.Catalog, subject.Subject]
                            //    .Record_InitData(GetCurrency(rec.Currency), rec.Amount, orderId, createTime: rec.CreateDate, quantity: rec.Count);

                            //非杂费（包括杂费的商检费）
                            if (rec.FeeSourceID == null)
                            {
                                dic.Add(rec.OrderFeeType.ToString(), recevibleId);
                            }
                            //其他杂费
                            else
                            {
                                dic.Add(rec.FeeSourceID.ToString(), recevibleId);
                            }
                        }


                        //核销实收
                        foreach (var red in orderReceipts.Where(item => item.OrderID == mainOrderId && item.OrderID == orderId && item.OrderType == 2))
                        {
                            //非杂费（包括杂费的商检费）
                            if (red.FeeSourceID == null)
                            {
                                if (dic.ContainsKey(red.OrderFeeType.ToString()))
                                {
                                    recevibleId = dic[red.OrderFeeType.ToString()];
                                }
                            }
                            //其他杂费
                            else
                            {
                                recevibleId = dic[red.FeeSourceID.ToString()];
                            }

                            //财务确认
                            flowId_Temp = PKeySigner.Pick(PKeyType.FlowAccount);
                            //添加实收
                            receivedses.Add(new Layers.Data.Sqls.PvbCrm.Receiveds()
                            {
                                ID = PKeySigner.Pick(PKeyType.Receiveds),
                                CreateDate = red.CreateDate,
                                Price = -red.Amount,
                                AdminID = GetAdminID(admins, red.AdminID),
                                OrderID = mainOrderId,
                                ReceivableID = recevibleId,
                                AccountType = (int)AccountType.BankStatement,
                                FlowID = flowId_Temp,

                                Currency1 = (int)Currency.CNY,
                                Rate1 = 1,
                                Price1 = -red.Amount * 1,
                            });

                            //核销流水
                            finaReceipt = financeReceipts.FirstOrDefault(item => item.SeqNo == red.SeqNo);
                            flowAccounts.Add(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                            {
                                ID = flowId_Temp,
                                Type = (int)AccountType.BankStatement,
                                AdminID = GetAdminID(admins, red.AdminID),
                                Business = Conduct,
                                CreateDate = red.CreateDate,
                                OrderID = mainOrderId,
                                Payee = PayeeID,
                                Payer = PayerID,

                                Price = red.Amount,
                                Currency = (int)Currency.CNY,

                                Currency1 = (int)Currency.CNY,
                                ERate1 = 1,
                                Price1 = red.Amount * 1,

                                Account = finaReceipt.BankAccount,
                                Bank = finaReceipt.BankName,
                                FormCode = red.SeqNo,
                            });
                        }

                        if (dic.Count > 0)
                        {
                            dic.Clear();
                        }
                    }
                }
                #endregion
            }

            return this;
        }
        #endregion

        public IDataService Encapsule()
        {
            using (var reponsitory = new PvbCrmReponsitory())
            using (new Yahv.Services.Views.FlowAccountsTopView<PvbCrmReponsitory>(reponsitory)) using (var vouchersView = new Yahv.Services.Views.VouchersStatisticsView<PvbCrmReponsitory>(reponsitory))
            {
                #region 应收和申请
                var tinyIds = orderReceipts.Select(item => item.OrderID).Distinct().ToArray();
                var voucherView = vouchersView.Where(item => tinyIds.Contains(item.TinyID)).ToArray();
                bool isInsertVoucher = false;
                SubjectDto subject;        //分类科目
                string recevibleId = string.Empty;
                string flowId_Temp = string.Empty;
                Temp_FinanceReceiptsView finaReceipt;
                List<string> tinyOrderIds = new List<string>();

                //类型、应收ID key:OrderFeeType,value:应收ID
                //类型是5、FeeSourceID=null,表示商检费 key：5 value:应收ID
                //类型为5,、FeeSourceID!=null, key:FeeSourceID,value:应收ID
                //一个单子，非杂费，只能有一种
                Dictionary<string, string> dic = new Dictionary<string, string>();

                VoucherStatistic voucher = null;
                VoucherStatistic[] vouchers;
                VoucherStatistic[] linq = null;

                decimal remainPrice = 0;      //剩余金额
                decimal payPrice = 0;
                IEnumerable<ReceivedPriceDto> actual;


                foreach (var mainOrderId in orderReceipts.GroupBy(item => item.MainOrderId).Select(item => item.Key))
                {
                    tinyOrderIds = orderReceipts.Where(item => item.MainOrderId == mainOrderId)
                        .GroupBy(item => item.OrderID)
                        .Select(item => item.Key).ToList();

                    //遍历循环小订单ID 进行应收、实收
                    foreach (var orderId in tinyOrderIds)
                    {
                        //核销实收
                        foreach (var red in orderReceipts.Where(item => item.MainOrderId == mainOrderId && item.OrderID == orderId && item.OrderType == 2))
                        {
                            subject = GetCatalogAndSubject(red.OrderFeeType, red.PremiumType);

                            if (receivedses.Count > 0)
                            {
                                actual = from r in receivedses
                                         group r by r.ReceivableID
                                    into g
                                         select new ReceivedPriceDto
                                         {
                                             ReceivableID = g.Key,
                                             Price = g.Sum(t => t.Price),
                                         };


                                linq = (from v in voucherView
                                        join _reced in actual on v.ReceivableID equals _reced.ReceivableID into recedJoin
                                        from reced in recedJoin.DefaultIfEmpty()
                                        where v.OrderID == mainOrderId && v.TinyID == orderId
                                              && v.Currency == ((Currency)GetCurrency(red.Currency))
                                              && v.Catalog == subject.Catalog && v.Subject == subject.Subject
                                        select new VoucherStatistic()
                                        {
                                            ReceivableID = v.ReceivableID,
                                            LeftPrice = v.LeftPrice,
                                            RightPrice = (v.RightPrice ?? 0) + reced?.Price,
                                        }).ToArray();
                            }
                            else
                            {
                                linq = voucherView.Where(item => item.OrderID == mainOrderId && item.TinyID == orderId
                                                                 && item.Currency == ((Currency)GetCurrency(red.Currency))
                                                                 && item.Catalog == subject.Catalog && item.Subject == subject.Subject).ToArray();
                            }

                            vouchers = linq.Where(
                                item => item.Remains > 0).OrderBy(item => item.LeftPrice).ToArray();

                            if (vouchers.Length <= 0)
                            {
                                vouchers = linq.Where(
                                    item => item.Remains > 0).OrderBy(item => item.LeftPrice).OrderByDescending(item => item.LeftPrice).Take(1).ToArray();
                            }

                            if (vouchers.Length <= 0)
                            {
                                continue;
                            }

                            remainPrice = Math.Abs(red.Amount);

                            var flowIds = Yahv.XdtData.Import.Extends.PKeyTypeExtend.Pick(PKeyType.FlowAccount, vouchers.Length);
                            var receivedIds = Yahv.XdtData.Import.Extends.PKeyTypeExtend.Pick(PKeyType.Receiveds, vouchers.Length);
                            vouchers = vouchers.OrderBy(item => item.Remains).ToArray();
                            for (int i = 0; i < vouchers.Length; i++)
                            {
                                if (remainPrice <= 0)
                                {
                                    break;
                                }

                                recevibleId = vouchers[i].ReceivableID;
                                payPrice = remainPrice > vouchers[i].Remains ? vouchers[i].Remains : remainPrice;

                                if (i == vouchers.Length - 1)
                                {
                                    if (remainPrice > vouchers[i].Remains)
                                    {
                                        payPrice = remainPrice;
                                    }
                                }


                                //财务确认
                                flowId_Temp = flowIds[i];
                                //添加实收
                                receivedses.Add(new Layers.Data.Sqls.PvbCrm.Receiveds()
                                {
                                    ID = receivedIds[i],
                                    CreateDate = red.CreateDate,
                                    Price = payPrice,
                                    AdminID = GetAdminID(admins, red.AdminID),
                                    OrderID = mainOrderId,
                                    ReceivableID = recevibleId,
                                    AccountType = (int)AccountType.BankStatement,
                                    FlowID = flowId_Temp,

                                    Currency1 = (int)Currency.CNY,
                                    Rate1 = 1,
                                    Price1 = payPrice * 1,
                                });

                                //核销流水
                                finaReceipt = financeReceipts.FirstOrDefault(item => item.SeqNo == red.SeqNo);
                                flowAccounts.Add(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                                {
                                    ID = flowId_Temp,
                                    Type = (int)AccountType.BankStatement,
                                    AdminID = GetAdminID(admins, red.AdminID),
                                    Business = Conduct,
                                    CreateDate = red.CreateDate,
                                    OrderID = mainOrderId,
                                    Payee = PayeeID,
                                    Payer = PayerID,

                                    Price = -payPrice,
                                    Currency = (int)Currency.CNY,

                                    Currency1 = (int)Currency.CNY,
                                    ERate1 = 1,
                                    Price1 = -payPrice * 1,

                                    Account = finaReceipt.BankAccount,
                                    Bank = finaReceipt.BankName,
                                    FormCode = red.SeqNo,
                                });

                                remainPrice -= payPrice;
                            }
                        }
                    }
                }
                #endregion
            }

            return this;
        }

        public void Enter()
        {
            using (var conn = ConnManager.Current.PvbCrm)
            {
                //conn.BulkInsert(payees);
                //conn.BulkInsert(flowAccounts);
                //conn.BulkInsert(vouchers);
                //conn.BulkInsert(receivableses);
                //conn.BulkInsert(receivedses);

                //Reallocation();

                //var ss = flowAccounts;
                //var sss = receivedses;
                //conn.BulkInsert(flowAccounts);
                //conn.BulkInsert(receivedses);
            }
        }

        /// <summary>
        /// 重新分配金额
        /// </summary>
        private void Reallocation()
        {
            string orderid = string.Empty;
            try
            {
                using (var reponsitory = new PvbCrmReponsitory(false))
                using (var flowView = new Yahv.Services.Views.FlowAccountsTopView<PvbCrmReponsitory>(reponsitory))
                using (var vouchersView = new Yahv.Services.Views.VouchersStatisticsView<PvbCrmReponsitory>(reponsitory))
                using (var receivedsView = new Yahv.Services.Views.ReceivedsTopView<PvbCrmReponsitory>(reponsitory))
                {
                    //总账单信息
                    string[] ids = new[]
                    {
                    "NL02020191008040",
                    "NL02020190831003",
                    "NL02020190912008",
                    "NL02020191008040",
                    "NL02020191108003",
                    "NL02020191108005",
                };
                    var voucherArray = vouchersView.Where(item => item.Catalog == "货款"
                    && ids.Contains(item.TinyID)
                    ).ToArray();

                    //查找应收实收相差金额大于10000的货款
                    //var array = voucherArray.Where(item => (Math.Abs(item.LeftPrice - (item.RightPrice ?? 0)) > 1000)
                    //&& item.RightPrice != null).ToArray();
                    var array = voucherArray.Where(item => item.RightPrice != null
                    //&& (Math.Abs(item.LeftPrice - item.RightPrice.Value) > 1000)
                    ).ToArray();

                    //根据小订单ID、业务、分类 分组
                    var group = from v in array
                                group v by new { v.TinyID, v.Business, v.Catalog } into g
                                select new
                                {
                                    g.Key.TinyID,
                                    g.Key.Business,
                                    g.Key.Catalog
                                };

                    //实收流水视图
                    var receivableIds = voucherArray.Select(v => v.ReceivableID).Distinct().ToArray();
                    var receFlows = (from r in receivedsView
                                     join f in flowView on r.FlowID equals f.ID
                                     where receivableIds.Contains(r.ReceivableID)
                                     select new
                                     {
                                         r.ID,
                                         r.ReceivableID,
                                         r.Price,
                                         r.FlowID,
                                         f.FormCode,
                                     }).ToArray();



                    decimal total = 0;      //实收总额
                    foreach (var obj in group)
                    {
                        orderid = obj.TinyID;

                        //应收ID
                        receivableIds = voucherArray.Where(item => item.TinyID == obj.TinyID && item.Business == obj.Business &&
                                       item.Catalog == obj.Catalog).Select(item => item.ReceivableID).ToArray();

                        //实收ID
                        var flowIds = receFlows.Where(item => receivableIds.Contains(item.ReceivableID))
                                .Select(item => item.FlowID).ToArray();

                        //实收总额
                        total = voucherArray.Where(item => receivableIds.Contains(item.ReceivableID)).Sum(item => item.RightPrice) ?? 0;

                        //如果多个流水核销，不处理
                        if (receFlows.Where(item => receivableIds.Contains(item.ReceivableID)).Select(item => item.FormCode)
                                .Distinct().Count() > 1)
                        {
                            continue;
                        }

                        //更新实收金额为0
                        reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receiveds>(new
                        {
                            Price = 0m,
                            Price1 = 0m,
                        }, item => receivableIds.Contains(item.ReceivableID));

                        //更新流水表金额为0
                        reponsitory.Update<Layers.Data.Sqls.PvbCrm.FlowAccounts>(new
                        {
                            Price = 0m,
                            Price1 = 0m,
                        }, item => flowIds.Contains(item.ID));


                        decimal remainPrice = Math.Abs(total);
                        decimal payPrice = 0;
                        //foreach (var reb in voucherArray.Where(item => receivableIds.Contains(item.ReceivableID)))
                        var vouchers = voucherArray.Where(item => receivableIds.Contains(item.ReceivableID)).ToArray();
                        for (int i = 0; i < vouchers.Length; i++)
                        {
                            if (remainPrice <= 0)
                            {
                                break;
                            }

                            payPrice = remainPrice > vouchers[i].LeftPrice ? vouchers[i].LeftPrice : remainPrice;

                            if (i == vouchers.Length - 1)
                            {
                                if (remainPrice > vouchers[i].LeftPrice)
                                {
                                    payPrice = remainPrice;
                                }
                            }

                            //根据应收ID更新实收
                            var red = receFlows.FirstOrDefault(item => item.ReceivableID == vouchers[i].ReceivableID);
                            if (red == null)
                            {
                                continue;
                            }
                            reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receiveds>(new
                            {
                                Price = payPrice,
                                Price1 = payPrice,
                            }, item => item.ID == red.ID);
                            //根据实收ID更新流水
                            reponsitory.Update<Layers.Data.Sqls.PvbCrm.FlowAccounts>(new
                            {
                                Price = -payPrice,
                                Price1 = -payPrice,
                            }, item => item.ID == red.FlowID);

                            remainPrice -= payPrice;
                        }
                    }

                    reponsitory.Submit();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        /// <summary>
        /// 根据符号获取币种
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        private int GetCurrency(string shortName)
        {
            Currency currency = Currency.CNY;

            if (Currency.HKD.GetCurrency().ShortName.ToLower() == shortName.ToLower())
            {
                currency = Currency.HKD;
            }

            if (Currency.USD.GetCurrency().ShortName.ToLower() == shortName.ToLower())
            {
                currency = Currency.USD;
            }

            return (int)currency;
        }

        /// <summary>
        /// 根据芯达通ID查找AdminID，如果没有就默认npc
        /// </summary>
        /// <param name="admins"></param>
        /// <param name="xdtAdminID"></param>
        /// <returns></returns>
        private string GetAdminID(Yahv.Services.Models.Admin[] admins, string xdtAdminID)
        {
            string adminID = Npc;

            if (admins.Any(item => item.OriginID == xdtAdminID))
            {
                adminID = admins.FirstOrDefault(item => item.OriginID == xdtAdminID).ID;
            }

            return adminID;
        }

        private SubjectDto GetCatalogAndSubject(int orderType, int? premiumType)
        {
            dynamic result = new SubjectDto();

            switch (orderType)
            {
                //货款
                case 1:
                    result.Catalog = "货款";
                    result.Subject = null;
                    break;
                //关税
                case 2:
                    result.Catalog = "税款";
                    result.Subject = "关税";
                    break;
                //增值税
                case 3:
                    result.Catalog = "税款";
                    result.Subject = "销售增值税";
                    break;
                //代理费
                case 4:
                    result.Catalog = "代理费";
                    result.Subject = "代理费";
                    break;
                //杂费（线上只有代理费商检费）
                case 5:
                    result.Catalog = "杂费";
                    //result.Subject = premiumType == null ? "商检费" : ((OrderPremiumType)premiumType).GetDescription();
                    result.Subject = premiumType == null ? "商检费" : "其他";
                    break;
            }

            return result;
        }

        /// <summary>
        /// 实收金额传输类
        /// </summary>
        internal class ReceivedPriceDto
        {
            public string ReceivableID { get; set; }
            public decimal Price { get; set; }
        }
    }
}
