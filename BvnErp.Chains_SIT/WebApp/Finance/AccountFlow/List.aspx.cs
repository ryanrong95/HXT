using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System.IO;
using Needs.Utils;
using System.Data;
using Needs.Utils.Converters;
using WebApp.Ccs.Utils;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Views;
using Needs.Ccs.Services.ApiSettings;
using Newtonsoft.Json;
using System.Net.Http;
using Needs.Ccs.Services.Models.HttpUtility;
using Layer.Data.Sqls.ScCustoms;
using Logs = Needs.Ccs.Services.Models.Logs;

namespace WebApp.Finance.AccountFlow
{
    /// <summary>
    /// 财务收款流水查询界面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboboxData();
        }

        /// <summary>
        /// 初始化所有的下拉框
        /// </summary>
        protected void LoadComboboxData()
        {
            this.Model.FeeType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FinanceFeeType>()
               .Select(item => new { item.Key, item.Value }).Json();
            //币种
            this.Model.CurrData = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { Value = item.Code, Text = item.Code + " " + item.Name }).Json();
            //金库
            this.Model.VaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault
                .Select(item => new { Value = item.ID, Text = item.Name }).Json();
            //账户
            this.Model.FinanceAccountData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                 .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal && item.AccountSource == AccountSource.standard)
                .Select(item => new { Value = item.ID, Text = item.AccountName }).Json();
        }

        /// <summary>
        /// 根据金库显示账户
        /// </summary>
        /// <returns></returns>
        protected object GetAccountByVault()
        {
            var vault = Request.Form["FinanceVault"];           
            return new
            {
                data = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(v => v.Status == Needs.Ccs.Services.Enums.Status.Normal && v.FinanceVaultID == vault).Select(item => new
                {
                    Value = item.ID,
                    Text = item.AccountName
                }).Json()
            };
        }

        //protected void data()
        //{
        //    string FeeType = Request.QueryString["FeeType"];
        //    string PayType = Request.QueryString["PayType"];
        //    string Currency = Request.QueryString["Currency"];
        //    string Type = Request.QueryString["ReceipType"];
        //    string StartDate = Request.QueryString["StartDate"];
        //    string EndDate = Request.QueryString["EndDate"];
        //    string Vault = Request.QueryString["Vault"];
        //    string Account = Request.QueryString["Account"];

        //    var accountFlows = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccountFlows.AsQueryable();

        //    if (!string.IsNullOrEmpty(FeeType))
        //    {
        //        var feeType = (FinanceFeeType)Enum.Parse(typeof(FinanceFeeType), FeeType.Trim());
        //        accountFlows = accountFlows.Where(t => t.FeeType == feeType);
        //    }
        //    if (!string.IsNullOrEmpty(PayType))
        //    {
        //        var payType = (PaymentType)Enum.Parse(typeof(PaymentType), PayType.Trim());
        //        accountFlows = accountFlows.Where(t => t.PaymentType == payType);
        //    }
        //    if (!string.IsNullOrEmpty(Currency))
        //    {
        //        accountFlows = accountFlows.Where(t => t.Currency == Currency);
        //    }
        //    if (!string.IsNullOrEmpty(Type))
        //    {
        //        var type = (FinanceType)Enum.Parse(typeof(FinanceType), Type.Trim());
        //        accountFlows = accountFlows.Where(t => t.Type == type);
        //    }

        //    if (!string.IsNullOrEmpty(StartDate))
        //    {
        //        accountFlows = accountFlows.Where(t => t.CreateDate >= Convert.ToDateTime(StartDate));
        //    }
        //    if (!string.IsNullOrEmpty(EndDate))
        //    {
        //        var endDate = Convert.ToDateTime(EndDate).AddDays(1);
        //        accountFlows = accountFlows.Where(t => t.CreateDate < endDate);
        //    }
        //    if (!string.IsNullOrEmpty(Vault))
        //    {
        //        accountFlows = accountFlows.Where(t => t.FinanceVault.ID == Vault.Trim());
        //    }
        //    if (!string.IsNullOrEmpty(Account))
        //    {
        //        accountFlows = accountFlows.Where(t => t.FinanceAccount.ID == Account.Trim());
        //    }

        //    accountFlows = accountFlows.OrderByDescending(t => t.CreateDate);

        //    Func<FinanceAccountFlow, object> convert = accountFlow => new
        //    {
        //        SeqNo = accountFlow.SeqNo,
        //        Vault = accountFlow.FinanceVault.Name,
        //        Account = accountFlow.FinanceAccount.AccountName,
        //        OtherAccount = accountFlow.OtherAccount,
        //        Type = accountFlow.Type.GetDescription(),
        //        FeeType = accountFlow.FeeTypeInt > 10000 ? ((FeeTypeEnum)accountFlow.FeeTypeInt).ToString()
        //                                                 : ((FinanceFeeType)accountFlow.FeeTypeInt).GetDescription(),    //accountFlow.FeeType.GetDescription(),
        //        PayType = accountFlow.PaymentType.GetDescription(),
        //        Currency = accountFlow.Currency,
        //        Amount = accountFlow.Amount,
        //        Balance = accountFlow.AccountBalance,
        //        Date = accountFlow.CreateDate.ToShortDateString(),
        //    };

        //    this.Paging(accountFlows, convert);
        //}  
        //优化  2020-10-27 by yeshuangshauang
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string FeeType = Request.QueryString["FeeType"];
            string PayType = Request.QueryString["PayType"];
            string Currency = Request.QueryString["Currency"];
            string Type = Request.QueryString["ReceipType"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string Vault = Request.QueryString["Vault"];
            string Account = Request.QueryString["Account"];

            using (var query = new Needs.Ccs.Services.Views.Finance.AccountFlow.FinanceAccountFlowsView())
            {
                var view = query;
                if (!string.IsNullOrEmpty(FeeType))
                {
                    var feeType = (FinanceFeeType)Enum.Parse(typeof(FinanceFeeType), FeeType.Trim());
                    view = view.SearchByFeeTypeInt(feeType);
                }
                if (!string.IsNullOrEmpty(PayType))
                {
                    var payType = (PaymentType)Enum.Parse(typeof(PaymentType), PayType.Trim());
                    view = view.SearchByPayType(payType);
                }
                if (!string.IsNullOrEmpty(Currency))
                {
                    view = view.SearchByCurrency(Currency);
                }
                if (!string.IsNullOrEmpty(Type))
                {
                    var type = (FinanceType)Enum.Parse(typeof(FinanceType), Type.Trim());
                    view = view.SearchByType(type);
                }
                if (!string.IsNullOrEmpty(StartDate))
                {
                    DateTime start = Convert.ToDateTime(StartDate);
                    view = view.SearchByStartDate(start);
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByEndDate(end);
                }
                if (!string.IsNullOrEmpty(Vault))
                {
                    view = view.SearchByVault(Vault.Trim());
                }
                if (!string.IsNullOrEmpty(Account))
                {
                    view = view.SearchByAccount(Account.Trim());
                }
                var hk_caiwu = System.Configuration.ConfigurationManager.AppSettings["HK_Caiwu"];
                if (!string.IsNullOrEmpty(hk_caiwu) && Needs.Wl.Admin.Plat.AdminPlat.Current.ID == hk_caiwu)
                {
                    view = view.SearchByHKCW();
                }
                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
        /// <summary>
        /// Excel导入账户流水 
        /// </summary>
        protected void ImportAccountFlow()
        {
            try
            {
                HttpPostedFile file = Request.Files["uploadExcel"];
                string ext = Path.GetExtension(file.FileName);
                if (ext != ".xls" && ext != ".xlsx")
                {
                    Response.Write((new { success = false, message = "文件格式错误，请上传.xls或.xlsx文件！" }).Json());
                    return;
                }

                //文件保存
                string fileName = file.FileName.ReName();

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Import);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);

                DataTable dt = Ccs.Utils.NPOIHelper.ExcelToDataTable(fileDic.FilePath, false);
                //获取账号
                string BankAccount = dt.Rows[0][1].ToString();
                if (string.IsNullOrEmpty(BankAccount))
                {
                    Response.Write((new { success = false, message = "银行账号为空" }).Json());
                    return;
                }

                var financeAccount = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                    .Where(item => item.BankAccount == BankAccount&&item.AccountSource==AccountSource.standard).FirstOrDefault();
                if (financeAccount == null)
                {
                    Response.Write((new { success = false, message = "系统查不到该账户" }).Json());
                    return;
                }
                //上传人作为付款人
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);              

                string[] chars = new string[] { "\n", " ", "\t", "\r" };
                List<FinancePayment> financePayments = new List<FinancePayment>();
                List<FinanceReceipt> financeReceipt = new List<FinanceReceipt>();
                for (int i = 2; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][5].ToString().RemoveChars(chars) == "贷")
                    {
                        FinanceReceipt receipt = new FinanceReceipt();
                        receipt.SeqNo = dt.Rows[i][6].ToString();
                        receipt.Payer = dt.Rows[i][4].ToString();
                        receipt.ReceiptType = PaymentType.TransferAccount;
                        //receipt.ReceiptDate = Convert.ToDateTime(dt.Rows[i][0].ToString());
                        string dateStr = dt.Rows[i][0].ToString();
                        DateTime date;
                        bool flag = DateTime.TryParse(dateStr, out date);
                        if (flag)
                        {
                            receipt.ReceiptDate = date;
                        }
                        else
                        {
                            receipt.ReceiptDate = DateTime.FromOADate(double.Parse(dateStr));
                        }

                        receipt.Currency = financeAccount.Currency;
                        receipt.Rate = 1.0M;
                        receipt.Amount = decimal.Parse(dt.Rows[i][2].ToString());
                        receipt.Account = financeAccount;
                        receipt.Vault = new FinanceVault { ID = financeAccount.FinanceVaultID };
                        receipt.Admin = admin;
                        receipt.Summary = dt.Rows[i][1].ToString();
                        if (receipt.Summary == "货款")
                        {
                            receipt.FeeType = FinanceFeeType.DepositReceived;
                        }
                        else
                        {
                            receipt.FeeType = FinanceFeeType.Other;
                        }
                        financeReceipt.Add(receipt);
                        receipt.Enter();                        
                        //更新账户余额
                        financeAccount.Balance = financeAccount.Balance + receipt.Amount;
                    }
                    else if (dt.Rows[i][5].ToString().RemoveChars(chars) == "借")
                    {
                        FinancePayment payment = new FinancePayment();
                        string dateStr = dt.Rows[i][0].ToString();
                        DateTime date;
                        bool flag = DateTime.TryParse(dateStr, out date);
                        if (flag)
                        {
                            payment.PayDate = date;
                        }
                        else
                        {
                            payment.PayDate = DateTime.FromOADate(double.Parse(dateStr));
                        }
                        payment.Summary = dt.Rows[i][1].ToString();
                        payment.Amount = decimal.Parse(dt.Rows[i][2].ToString());
                        payment.BankAccount = dt.Rows[i][3].ToString();
                        payment.PayFeeType = MatchFeeTypeNew(payment.Summary.Trim());
                        if (payment.PayFeeType == FinanceFeeType.PayPreBill)
                        {
                            throw new Exception("第" + (i+1).ToString() + "行 摘要只能填费用类型");                            
                        }
                        else if (payment.PayFeeType == FinanceFeeType.Poundage)
                        {
                            string str = dt.Rows[i][4].ToString().Trim();
                            if (string.IsNullOrEmpty(str))
                            {
                                payment.PayeeName = financeAccount.CustomizedCode;
                            }
                            else
                            {
                                payment.PayeeName = dt.Rows[i][4].ToString();
                            }
                        }
                        else
                        {
                            payment.PayeeName = dt.Rows[i][4].ToString();
                        }
                        payment.SeqNo = dt.Rows[i][6].ToString();
                        payment.Payer = admin;
                        payment.FinanceAccount = financeAccount;
                        payment.FinanceVault = new FinanceVault { ID = financeAccount.FinanceVaultID };
                        payment.Currency = financeAccount.Currency;
                        payment.ExchangeRate = 1.0M;
                        payment.PayType = PaymentType.TransferAccount;
                       
                        financePayments.Add(payment);
                       
                    }
                    else
                    {
                        throw new Exception("第" + (i+1).ToString() + "行第5列类型错误");
                    }
                }

                if (financeReceipt.Count() > 0)
                {
                    ReceiptPost2Center(financeReceipt);
                }

                foreach (var payment in financePayments)
                {
                    payment.Enter();
                    //更新账户余额
                    financeAccount.Balance = financeAccount.Balance - payment.Amount;
                }

                Post2Center(financePayments);

                Response.Write((new { success = true, message = "导入成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }

        private FinanceFeeType MatchFeeType(string feeDec)
        {
            if (feeDec.Contains("手续费"))
            {
                return FinanceFeeType.Poundage;
            }
            return FinanceFeeType.Fee;
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        protected void ExportExcel()
        {
            try
            {
                string FeeType = Request.Form["FeeType"];
                string PayType = Request.Form["PayType"];
                string Currency = Request.Form["Currency"];
                string Type = Request.Form["ReceipType"];
                string StartDate = Request.Form["StartDate"];
                string EndDate = Request.Form["EndDate"];
                string Vault = Request.Form["Vault"];
                string Account = Request.Form["Account"];

                var accountFlows = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccountFlows.AsQueryable();

                if (!string.IsNullOrEmpty(FeeType))
                {
                    var feeType = (FinanceFeeType)Enum.Parse(typeof(FinanceFeeType), FeeType.Trim());
                    accountFlows = accountFlows.Where(t => t.FeeType == feeType);
                }
                if (!string.IsNullOrEmpty(PayType))
                {
                    var payType = (PaymentType)Enum.Parse(typeof(PaymentType), PayType.Trim());
                    accountFlows = accountFlows.Where(t => t.PaymentType == payType);
                }
                if (!string.IsNullOrEmpty(Currency))
                {
                    accountFlows = accountFlows.Where(t => t.Currency == Currency);
                }
                if (!string.IsNullOrEmpty(Type))
                {
                    var type = (FinanceType)Enum.Parse(typeof(FinanceType), Type.Trim());
                    accountFlows = accountFlows.Where(t => t.Type == type);
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    accountFlows = accountFlows.Where(t => t.CreateDate >= Convert.ToDateTime(StartDate));
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    var endDate = Convert.ToDateTime(EndDate).AddDays(1);
                    accountFlows = accountFlows.Where(t => t.CreateDate < endDate);
                }
                if (!string.IsNullOrEmpty(Vault))
                {
                    accountFlows = accountFlows.Where(t => t.FinanceVault.ID == Vault.Trim());
                }
                if (!string.IsNullOrEmpty(Account))
                {
                    accountFlows = accountFlows.Where(t => t.FinanceAccount.ID == Account.Trim());
                }

                accountFlows = accountFlows.OrderByDescending(t => t.CreateDate);

                Func<FinanceAccountFlow, object> convert = accountFlow => new
                {
                    SeqNo = accountFlow.SeqNo,
                    Vault = accountFlow.FinanceVault.Name,
                    Account = accountFlow.FinanceAccount.AccountName,
                    OtherAccount = accountFlow.OtherAccount,
                    Type = accountFlow.Type.GetDescription(),
                    FeeType = accountFlow.FeeTypeInt > 10000 ? ((FeeTypeEnum)accountFlow.FeeTypeInt).ToString()
                                                             : ((FinanceFeeType)accountFlow.FeeTypeInt).GetDescription(),    //accountFlow.FeeType.GetDescription(),
                    PayType = accountFlow.PaymentType.GetDescription(),
                    Currency = accountFlow.Currency,
                    Amount = accountFlow.Amount,
                    Balance = accountFlow.AccountBalance,
                    Date = accountFlow.CreateDate.ToShortDateString(),
                };
                //写入数据
                DataTable dt = NPOIHelper.JsonToDataTable(accountFlows.Select(convert).ToArray().Json());

                string fileName = "账户流水" + DateTime.Now.Ticks + ".xls";

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                #region 设置导出格式

                var excelconfig = new ExcelConfig();
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.Title = "账户流水";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SeqNo", ExcelColumn = "流水号", Alignment = "left" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Vault", ExcelColumn = "金库", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Account", ExcelColumn = "账户名称", Alignment = "left" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OtherAccount", ExcelColumn = "对方户名", Alignment = "left" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Type", ExcelColumn = "收款/付款", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FeeType", ExcelColumn = "费用类型", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayType", ExcelColumn = "费用方式", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Amount", ExcelColumn = "金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Balance", ExcelColumn = "账户金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Date", ExcelColumn = "发生日期", Alignment = "center" });

                #endregion

                //调用导出方法
                NPOIHelper.ExcelDownload(dt, excelconfig);

                Response.Write((new
                {
                    success = true,
                    message = "导出成功",
                    url = fileDic.FileUrl
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "导出失败：" + ex.Message,
                }).Json());
            }
        }

        private FinanceFeeType MatchFeeTypeNew(string feeDec)
        {
            var feeType = FinanceFeeType.PayBankPaypal;
            switch (feeDec)
            {
                case "职工薪酬":
                    feeType = FinanceFeeType.PaySalary;
                    break;
                case "劳动保险费":
                    feeType = FinanceFeeType.PayEndowment;
                    break;
                case "住房公积金":
                    feeType = FinanceFeeType.PayHousingProvidentFund;
                    break;
                case "运杂费":
                    feeType = FinanceFeeType.PayIncidentals;
                    break;
                case "差旅费":
                    feeType = FinanceFeeType.PayBussiness;
                    break;
                case "广告宣传费":
                    feeType = FinanceFeeType.PayAdvertisement;
                    break;
                case "租赁费及物业费":
                    feeType = FinanceFeeType.PayPropertyMan;
                    break;
                case "业务招待费":
                    feeType = FinanceFeeType.PayEntertain;
                    break;
                case "借款利息":
                    feeType = FinanceFeeType.PayInterest;
                    break;
                case "报关服务费":
                    feeType = FinanceFeeType.PayService;
                    break;

                case "仓储保管费":
                    feeType = FinanceFeeType.PayWarehouse;
                    break;
                case "电话及网络通信费":
                    feeType = FinanceFeeType.PayNet;
                    break;
                case "包装费":
                    feeType = FinanceFeeType.PayPackage;
                    break;
                case "审计及会计服务费":
                    feeType = FinanceFeeType.PayAudit;
                    break;
                case "税金及附加":
                    feeType = FinanceFeeType.PayTax;
                    break;
                case "贸易通清关费":
                    feeType = FinanceFeeType.PayTrade;
                    break;
                case "印花税":
                    feeType = FinanceFeeType.PayStampTax;
                    break;
                case "房产税":
                    feeType = FinanceFeeType.PayHouse;
                    break;
                case "残保金":
                    feeType = FinanceFeeType.PayInjury;
                    break;
                case "借款附加费":
                    feeType = FinanceFeeType.PayBorrowAdded;
                    break;

                case "银行手续费":
                    feeType = FinanceFeeType.PayBankPaypal;
                    break;
                case "贴现利息":
                    feeType = FinanceFeeType.PayAcceptance;
                    break;
                case "存款利息":
                    feeType = FinanceFeeType.PayDepositInterest;
                    break;
                case "内部利息":
                    feeType = FinanceFeeType.PayInterInterest;
                    break;
                case "水电费":
                    feeType = FinanceFeeType.PayWater;
                    break;
                case "诉讼费":
                    feeType = FinanceFeeType.PayLawsuit;
                    break;
                case "坏账损失":
                    feeType = FinanceFeeType.PayBadBorrow;
                    break;
                case "办公用品及设备购置费":
                    feeType = FinanceFeeType.PayOffice;
                    break;
                case "车辆支出":
                    feeType = FinanceFeeType.PayCar;
                    break;
                case "其他费用":
                    feeType = FinanceFeeType.PayOtherFee;
                    break;
                default:
                    feeType = FinanceFeeType.PayPreBill;
                    break;
            }
            return feeType;
        }

        private void Post2Center(List<FinancePayment> financePayments)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                Dictionary<int, string> centerFeeTypeMap = new Dictionary<int, string>();
                //var ErmAdminID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == financePayments.FirstOrDefault().Payer.ID)?.ErmAdminID;
                List<CenterFee> fees = new List<CenterFee>();
                foreach (var item in financePayments)
                {
                    string CostApplyID = Needs.Overall.PKeySigner.Pick(PKeyType.PayApplicant);
                    Needs.Ccs.Services.Models.CostApply costApply = new Needs.Ccs.Services.Models.CostApply();
                    costApply.ID = CostApplyID;
                    costApply.PayeeBank = item.FinanceAccount.BankName;
                    costApply.PayeeAccountID = item.FinanceAccount.ID;
                    costApply.PayeeName = item.PayeeName;
                    costApply.PayeeAccount = item.BankAccount;
                    costApply.PayeeBank = item.BankName;
                    costApply.Currency = "CNY";
                    costApply.CostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.PaySuccess;
                    costApply.AdminID = item.Payer.ID;
                    costApply.Status = Status.Normal;
                    costApply.CreateDate = DateTime.Now;
                    costApply.UpdateDate = DateTime.Now;
                    costApply.MoneyType = Needs.Ccs.Services.Enums.MoneyTypeEnum.BankAutoApply;
                    costApply.CashType = CashTypeEnum.Common;
                    costApply.Amount = item.Amount;
                    costApply.PayTime = DateTime.Now;

                    Needs.Ccs.Services.Models.CostApplyItem costitem = new Needs.Ccs.Services.Models.CostApplyItem();
                    costitem.ID = ChainsGuid.NewGuidUp();
                    costitem.FeeType = item.PayFeeType;
                    costitem.Amount = item.Amount;
                    costitem.FeeDesc = item.PayFeeType.GetDescription().Replace("付款", "");
                    costApply.Items.Add(costitem);

                    CenterFee centerFee = new CenterFee();
                    centerFee.ReceiveAccountNo = item.BankAccount;
                    centerFee.AccountNo = item.FinanceAccount.BankAccount;
                    centerFee.SeqNo = item.SeqNo;
                    centerFee.Amount = item.Amount;
                    centerFee.Currency = item.Currency;
                    centerFee.Rate = item.ExchangeRate;
                    centerFee.PaymentDate = DateTime.Now;
                    int paymentType = PaymentTypeTransfer.Current.L2CTransfer(item.PayType);
                    centerFee.PaymentType = paymentType;
                    centerFee.CreatorID = "Admin00530";

                    string ceterFeetype = "";
                    if (centerFeeTypeMap.ContainsKey((int)item.PayFeeType))
                    {
                        ceterFeetype = centerFeeTypeMap[(int)item.PayFeeType];
                    }
                    else
                    {
                        ceterFeetype = FeeTypeTransfer.Current.L2COutTransfer(item.PayFeeType);
                        centerFeeTypeMap.Add((int)item.PayFeeType, ceterFeetype);
                    }


                    CenterFeeItem feeItem = new CenterFeeItem();
                    feeItem.FeeType = ceterFeetype;
                    feeItem.Amount = item.Amount;
                    centerFee.FeeItems = new List<CenterFeeItem>();
                    centerFee.FeeItems.Add(feeItem);
                    fees.Add(centerFee);

                    costApply.Enter();
                }

                #region 同步中心
                SendStrcut sendStrcut = new SendStrcut();
                sendStrcut.sender = "FSender001";
                sendStrcut.option = CenterConstant.Enter;
                sendStrcut.model = fees;
                //提交中心
                string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
                string requestUrl = URL + FinanceApiSetting.MultiFeeUrl;
                string apiclient = JsonConvert.SerializeObject(sendStrcut);

                Logs log = new Logs();
                log.Name = "批量费用同步";
                log.MainID = financePayments.FirstOrDefault().ID;
                log.AdminID = financePayments.FirstOrDefault().Payer.ID;
                log.Json = apiclient;
                log.Summary = "";
                log.Enter();

                HttpResponseMessage response = new HttpResponseMessage();
                response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);
                #endregion
            });
        }

        private void ReceiptPost2Center(List<FinanceReceipt> financeReceipts)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                List<CenterFinanceReceipt> receipts = new List<CenterFinanceReceipt>();
                foreach(var item in financeReceipts)
                {
                    CenterFinanceReceipt centerFinanceReceipt = new CenterFinanceReceipt(item);
                    receipts.Add(centerFinanceReceipt);
                }
                

                SendStrcut sendStrcut = new SendStrcut();
                sendStrcut.sender = "FSender001";               
                sendStrcut.option = CenterConstant.Enter;
                

                sendStrcut.model = receipts;
                //提交中心
                string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
                string requestUrl = URL + FinanceApiSetting.ReceiptBatchUrl;
                string apiclient = JsonConvert.SerializeObject(sendStrcut);

                Logs log = new Logs();
                log.Name = "批量收款同步";
                log.MainID = financeReceipts.FirstOrDefault().ID;
                log.AdminID = financeReceipts.FirstOrDefault().AdminID;
                log.Json = apiclient;
                log.Summary = "";
                log.Enter();

                HttpResponseMessage response = new HttpResponseMessage();
                response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);


            });
        }
    }
}