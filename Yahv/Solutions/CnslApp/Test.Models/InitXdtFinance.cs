using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbCrm;
using Layers.Linq;
using Yahv;
using Yahv.Payments;
using Yahv.Payments.Models.Rolls;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Admin = Yahv.Services.Models.Admin;

namespace CnslApp.Test.Models
{
    /// <summary>
    /// 同步芯达通数据至代仓储
    /// </summary>
    //public class InitXdtFinance
    //{
    //    //深圳市芯达通供应链管理有限公司
    //    private string PayeeID = "DBAEAB43B47EB4299DD1D62F764E6B6A";

    //    private string PayerID = "A434BE582059F6C1624549B6231A1502";
    //    private string PayerName = "杭州比一比电子科技有限公司";

    //    private int MethordType = 3;        //汇款方式 转账
    //    private string Npc = "Npc-Robot";
    //    private string Conduct = "代报关";

    //    public void Init()
    //    {
    //        using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
    //        using (var adminView = new Yahv.Services.Views.AdminsAll<PvbCrmReponsitory>())
    //        {
    //            return;

    //            //本地Admins
    //            var admins = adminView.ToArray();

    //            //初始化银行收款
    //            InitFinanceReceipts(reponsitory, admins);

    //            //初始化应收、实收
    //            InitRecords(reponsitory, admins);
    //        }
    //    }

    //    #region 自定义方法
    //    /// <summary>
    //    /// 初始化银行收款
    //    /// </summary>
    //    private void InitFinanceReceipts(PvbCrmReponsitory reponsitory, Admin[] admins)
    //    {
    //        using (var flowView = new Yahv.Payments.Views.FlowAccountsTopView(reponsitory))
    //        using (var adminView = new Yahv.Services.Views.AdminsAll<PvbCrmReponsitory>())
    //        {
    //            //芯达通银行收款信息
    //            var xdtView = GetFinanceReceiptsView(reponsitory);
    //            var xdtAccountsView = GetFinanceAccountsView(reponsitory);

    //            //本地银行流水
    //            var flowAccounts = flowView.Where(item => item.Type == AccountType.BankStatement && item.Price > 0).ToArray();

    //            if (xdtView == null || !xdtView.Any())
    //            {
    //                return;
    //            }

    //            //添加收款账号
    //            //InitBankAccountInfo(xdtView, reponsitory);
    //            InitBankAccountsInfo(xdtAccountsView, reponsitory);

    //            //添加银行收款流水
    //            foreach (var receipt in xdtView)
    //            {
    //                try
    //                {
    //                    if (flowAccounts.All(item => item.FormCode != receipt.SeqNo))
    //                    {
    //                        PaymentManager.Erp(GetAdminID(admins, receipt.AdminID))[receipt.Payer, PayeeID].Digital.Recharge(GetCurrency(receipt.Currency), receipt.Amount, receipt.BankName, receipt.BankAccount, receipt.SeqNo, receipt.CreateDate, createTime: receipt.CreateDate);
    //                    }
    //                }
    //                catch (Exception ex)
    //                {
    //                    Console.WriteLine($"流水号[{receipt.SeqNo}]，充值失败!");
    //                }
    //            }

    //        }
    //    }

    //    /// <summary>
    //    /// 初始化应收、实收
    //    /// </summary>
    //    /// <param name="reponsitory"></param>
    //    private void InitRecords(PvbCrmReponsitory reponsitory, Admin[] admins)
    //    {
    //        using (var vouchersView = new Yahv.Payments.Views.VouchersStatisticsView(reponsitory))
    //        {
    //            var ordersView = GetOrderReceiptsView(reponsitory);     //获取OrderReceipts 视图
    //            var vouchers = vouchersView.ToArray();
    //            SubjectDto subject;        //分类科目
    //            string recevibleId = string.Empty;      //应收ID

    //            //类型、应收ID key:OrderFeeType,value:应收ID
    //            //类型是5、FeeSourceID=null,表示商检费 key：5 value:应收ID
    //            //类型为5,、FeeSourceID!=null, key:FeeSourceID,value:应收ID
    //            //一个单子，非杂费，只能有一种
    //            Dictionary<string, string> dic = new Dictionary<string, string>();

    //            int total = ordersView.GroupBy(item => item.OrderID).Count();
    //            int success = 0;

    //            //循环订单ID
    //            foreach (var orderId in ordersView.GroupBy(item => item.OrderID).Select(item => item.Key))
    //            {
    //                try
    //                {
    //                    if (vouchers.Any(item => item.OrderID == orderId))
    //                    {
    //                        continue;
    //                    }

    //                    //添加应收
    //                    foreach (var rec in ordersView.Where(item => item.OrderID == orderId && item.OrderType == 1))
    //                    {
    //                        subject = GetCatalogAndSubject(rec.OrderFeeType, rec.PremiumType);
    //                        recevibleId = PaymentManager.Erp(GetAdminID(admins, rec.AdminID))[PayerName, PayeeID][Conduct]
    //                            .Receivable[subject.Catalog, subject.Subject]
    //                            .Record_InitData(GetCurrency(rec.Currency), rec.Amount, orderId, createTime: rec.CreateDate, quantity: rec.Count);

    //                        //非杂费（包括杂费的商检费）
    //                        if (rec.FeeSourceID == null)
    //                        {
    //                            dic.Add(rec.OrderFeeType.ToString(), recevibleId);
    //                        }
    //                        //其他杂费
    //                        else
    //                        {
    //                            dic.Add(rec.FeeSourceID.ToString(), recevibleId);
    //                        }
    //                    }

    //                    //核销实收
    //                    foreach (var red in ordersView.Where(item => item.OrderID == orderId && item.OrderType == 2))
    //                    {
    //                        if (red.FeeSourceID == null)
    //                        {
    //                            if (dic.ContainsKey(red.OrderFeeType.ToString()))
    //                            {
    //                                PaymentManager.Erp(GetAdminID(admins, red.AdminID)).Received.For(dic[red.OrderFeeType.ToString()])
    //                                    .Confirm(new VoucherInput()
    //                                    {
    //                                        Type = VoucherType.Receipt,
    //                                        Currency = GetCurrency(red.Currency),
    //                                        Payer = PayerID,
    //                                        Payee = PayeeID,
    //                                        FormCode = red.SeqNo,
    //                                        Business = Conduct,
    //                                        Price = red.Amount,
    //                                        CreateDate = red.CreateDate,
    //                                        AccountType = AccountType.BankStatement,
    //                                    });
    //                            }
    //                        }
    //                        else
    //                        {
    //                            PaymentManager.Erp(GetAdminID(admins, red.AdminID))
    //                                   .Received.For(dic[red.FeeSourceID.ToString()])
    //                                   .Confirm(new VoucherInput()
    //                                   {
    //                                       Type = VoucherType.Receipt,
    //                                       Currency = GetCurrency(red.Currency),
    //                                       Payer = PayerID,
    //                                       Payee = PayeeID,
    //                                       FormCode = red.SeqNo,
    //                                       Business = Conduct,
    //                                       Price = red.Amount,
    //                                       CreateDate = red.CreateDate,
    //                                       AccountType = AccountType.BankStatement,
    //                                   });
    //                        }
    //                    }

    //                    if (dic.Count > 0)
    //                    {
    //                        dic.Clear();
    //                    }
    //                    ++success;
    //                    Console.WriteLine($"订单[{orderId}]，添加完成!");
    //                }
    //                catch (Exception ex)
    //                {
    //                    Console.WriteLine($"订单[{orderId}]，添加失败!{ex.Message}");
    //                }
    //            }

    //            Console.WriteLine($"订单添加结束，一共有{total}个订单，成功{success}个，失败{total - success}个!");
    //        }
    //    }

    //    /// <summary>
    //    /// 芯达通财务收款视图
    //    /// </summary>
    //    /// <param name="reponsitory"></param>
    //    /// <returns></returns>
    //    private Temp_FinanceReceiptsView[] GetFinanceReceiptsView(PvbCrmReponsitory reponsitory)
    //    {
    //        //只获取杭州比一比 收款数据
    //        return (from entity in reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Temp_FinanceReceiptsView>()
    //                where entity.Payer == PayerName
    //                select entity).ToArray();
    //    }

    //    /// <summary>
    //    /// 芯达通账户视图
    //    /// </summary>
    //    /// <param name="reponsitory"></param>
    //    /// <returns></returns>
    //    private Temp_FinanceAccounts[] GetFinanceAccountsView(PvbCrmReponsitory reponsitory)
    //    {
    //        //只获取杭州比一比 收款数据
    //        return (from entity in reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Temp_FinanceAccounts>()
    //                select entity).ToArray();
    //    }

    //    /// <summary>
    //    /// 芯达通应收、实收
    //    /// </summary>
    //    /// <param name="reponsitory"></param>
    //    /// <returns></returns>
    //    private Temp_OrderReceiptsView[] GetOrderReceiptsView(PvbCrmReponsitory reponsitory)
    //    {
    //        return (from entity in reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Temp_OrderReceiptsView>()
    //                    //where entity.OrderID == "NL02020190814010"
    //                select entity).ToArray();
    //    }

    //    /// <summary>
    //    /// 根据银行收款，添加银行收款账号信息
    //    /// </summary>
    //    /// <param name="view"></param>
    //    /// <param name="reponsitory"></param>
    //    private void InitBankAccountInfo(Temp_FinanceReceiptsView[] view, PvbCrmReponsitory reponsitory)
    //    {
    //        //我们收款账号
    //        var payeesView = reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Payees>()
    //            .Where(item => item.RealID == null || item.RealID == "").ToArray();

    //        //芯达通收款账号
    //        var banks = view.Select(item => new
    //        {
    //            item.AccountName,
    //            item.BankAccount,
    //            item.SwiftCode,
    //            item.Currency,
    //            item.BankName,
    //            item.BankAddress
    //        }).Distinct();

    //        foreach (var bank in banks)
    //        {
    //            if (!payeesView.Any(
    //                    item =>
    //                        item.Account.Trim() == bank.BankAccount.Trim() &&
    //                        ((Currency)item.Currency).GetCurrency().ShortName == bank.Currency))
    //            {
    //                new YaHv.Csrm.Services.Models.Origins.Payee()
    //                {
    //                    EnterpriseID = PayeeID,
    //                    Methord = Methord.Transfer,
    //                    Bank = bank.BankName,
    //                    BankAddress = bank.BankAddress,
    //                    Account = bank.BankAccount,
    //                    SwiftCode = bank.SwiftCode,
    //                    Currency = GetCurrency(bank.Currency),
    //                    Creator = Npc,

    //                }.Enter();
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 根据银行账户添加收款信息
    //    /// </summary>
    //    /// <param name="view"></param>
    //    /// <param name="reponsitory"></param>
    //    private void InitBankAccountsInfo(Temp_FinanceAccounts[] view, PvbCrmReponsitory reponsitory)
    //    {
    //        //我们收款账号
    //        var payeesView = reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Payees>()
    //            .Where(item => item.RealID == null || item.RealID == "").ToArray();

    //        //芯达通收款账号
    //        var banks = view.Select(item => new
    //        {
    //            item.AccountName,
    //            item.BankAccount,
    //            item.SwiftCode,
    //            item.Currency,
    //            item.BankName,
    //            item.BankAddress
    //        }).Distinct();

    //        foreach (var bank in banks)
    //        {
    //            if (!payeesView.Any(
    //                    item =>
    //                        item.Account.Trim() == bank.BankAccount.Trim() &&
    //                        ((Currency)item.Currency).GetCurrency().ShortName == bank.Currency))
    //            {
    //                new YaHv.Csrm.Services.Models.Origins.Payee()
    //                {
    //                    EnterpriseID = PayeeID,
    //                    Methord = Methord.Transfer,
    //                    Bank = bank.BankName,
    //                    BankAddress = bank.BankAddress,
    //                    Account = bank.BankAccount,
    //                    SwiftCode = bank.SwiftCode,
    //                    Currency = GetCurrency(bank.Currency),
    //                    Creator = Npc,

    //                }.Enter();
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 根据符号获取币种
    //    /// </summary>
    //    /// <param name="shortName"></param>
    //    /// <returns></returns>
    //    private Currency GetCurrency(string shortName)
    //    {
    //        Currency currency = Currency.CNY;

    //        if (Currency.HKD.GetCurrency().ShortName.ToLower() == shortName.ToLower())
    //        {
    //            currency = Currency.HKD;
    //        }

    //        if (Currency.USD.GetCurrency().ShortName.ToLower() == shortName.ToLower())
    //        {
    //            currency = Currency.USD;
    //        }

    //        return currency;
    //    }

    //    /// <summary>
    //    /// 根据芯达通ID查找AdminID，如果没有就默认npc
    //    /// </summary>
    //    /// <param name="admins"></param>
    //    /// <param name="xdtAdminID"></param>
    //    /// <returns></returns>
    //    private string GetAdminID(Yahv.Services.Models.Admin[] admins, string xdtAdminID)
    //    {
    //        string adminID = Npc;

    //        if (admins.Any(item => item.OriginID == xdtAdminID))
    //        {
    //            adminID = admins.FirstOrDefault(item => item.OriginID == xdtAdminID).ID;
    //        }

    //        return adminID;
    //    }

    //    private SubjectDto GetCatalogAndSubject(int orderType, int? premiumType)
    //    {
    //        dynamic result = new SubjectDto();

    //        switch (orderType)
    //        {
    //            //货款
    //            case 1:
    //                result.Catalog = "货款";
    //                result.Subject = null;
    //                break;
    //            //关税
    //            case 2:
    //                result.Catalog = "税款";
    //                result.Subject = "关税";
    //                break;
    //            //增值税
    //            case 3:
    //                result.Catalog = "税款";
    //                result.Subject = "海关增值税";
    //                break;
    //            //代理费
    //            case 4:
    //                result.Catalog = "代理费";
    //                result.Subject = "代理费";
    //                break;
    //            //杂费（线上只有代理费商检费）
    //            case 5:
    //                result.Catalog = "杂费";
    //                result.Subject = premiumType == null ? "商检费" : ((OrderPremiumType)premiumType).GetDescription();
    //                break;
    //        }

    //        return result;
    //    }
    //    #endregion
    //}

    ////货款
    ////关税（税款_关税）
    ////增值税（税款_ 海关增值税）
    ////代理费（代理费_代理费）
    ////杂费()



    ////代理费和商检费

    ///// <summary>
    ///// 订单收款费用类型
    ///// </summary>
    //public enum OrderFeeType
    //{
    //    /// <summary>
    //    /// 货款
    //    /// </summary>
    //    [Description("货款")]
    //    Product = 1,

    //    /// <summary>
    //    /// 关税
    //    /// </summary>
    //    [Description("关税")]
    //    Tariff = 2,

    //    /// <summary>
    //    /// 增值税
    //    /// </summary>
    //    [Description("增值税")]
    //    AddedValueTax = 3,

    //    /// <summary>
    //    /// 代理费
    //    /// </summary>
    //    [Description("代理费")]
    //    AgencyFee = 4,

    //    /// <summary>
    //    /// 杂费
    //    /// </summary>
    //    [Description("杂费")]
    //    Incidental = 5,
    //}

    ///// <summary>
    ///// 订单附加费用类型(代理费和商检费 0,1)
    ///// </summary>
    //public enum OrderPremiumType
    //{
    //    /// <summary>
    //    /// 代理费
    //    /// </summary>
    //    [Description("代理费")]
    //    AgencyFee,

    //    /// <summary>
    //    /// 商检费
    //    /// </summary>
    //    [Description("商检费")]
    //    InspectionFee,

    //    /// <summary>
    //    /// 送货费
    //    /// </summary>
    //    [Description("送货费")]
    //    DeliveryFee,

    //    /// <summary>
    //    /// 快递费
    //    /// </summary>
    //    [Description("快递费")]
    //    ExpressFee,

    //    /// <summary>
    //    /// 清关费
    //    /// </summary>
    //    [Description("清关费")]
    //    CustomClearanceFee,

    //    /// <summary>
    //    /// 提货费
    //    /// </summary>
    //    [Description("提货费")]
    //    PickUpFee,

    //    /// <summary>
    //    /// 停车费
    //    /// </summary>
    //    [Description("停车费")]
    //    ParkingFee,

    //    /// <summary>
    //    /// 入仓费
    //    /// </summary>
    //    [Description("入仓费")]
    //    EntryFee,

    //    /// <summary>
    //    /// 仓储费
    //    /// </summary>
    //    [Description("仓储费")]
    //    StorageFee,

    //    /// <summary>
    //    /// 收货异常费用
    //    /// </summary>
    //    [Description("收货异常费用")]
    //    UnNormalFee,

    //    /// <summary>
    //    /// 其他(的杂费)
    //    /// </summary>
    //    [Description("其他")]
    //    OtherFee
    //}

    //public class SubjectDto
    //{
    //    public string Catalog { get; set; }
    //    public string Subject { get; set; }
    //}
}
