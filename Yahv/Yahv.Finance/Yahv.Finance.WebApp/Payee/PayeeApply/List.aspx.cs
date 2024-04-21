using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using PKeyType = Yahv.Finance.Services.PKeyType;

namespace Yahv.Finance.WebApp.Payee
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var accounts = new Yahv.Finance.Services.Views.Rolls.AccountsRoll().ToList();
                this.Model.Accounts = accounts.Select(item => new
                {
                    AccountID = item.ID,
                    ShortName = item.ShortName ?? item.Name,
                    CompanyName = item.Enterprise?.Name,
                    BankName = item.BankName,
                    Code = item.Code,
                    CurrencyDes = item.Currency.GetDescription(),
                });


            }
        }

        protected object data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string accountid = Request.QueryString["s_accountid"];
            string formcode = Request.QueryString["s_formcode"];

            using (var query1 = new Services.Views.Rolls.PayeeLeftsRoll())
            {
                var view = query1;

                //不是超级管理员，只能看到自己的
                if (!Erp.Current.IsSuper)
                {
                    view = view.SearchByCreatorID(Erp.Current.ID);
                }

                if (!string.IsNullOrWhiteSpace(accountid))
                {
                    view = view.SearchByAccountID(accountid);
                }

                if (!string.IsNullOrWhiteSpace(formcode))
                {
                    view = view.SearchByFormCode(formcode);
                }

                return view.ToMyPage(page, rows).Json();
            }
        }

        protected object addRange()
        {
            var result = new JMessage();

            var url = Request["urls"]; //excel存 放的路径
            string getUrl = string.Empty;
            string str = "_uploader/PayeeApply/";

            if (!string.IsNullOrEmpty(url))
            {
                int IndexofA = url.LastIndexOf(str);
                getUrl = url.Substring(IndexofA).Substring(str.Length);
            }

            var vpath = $"/{string.Join("/", str)}/";
            var path2 = Server.MapPath(vpath) + getUrl;

            try
            {
                var npoiHelper = new NPOIHelper(path2);
                var dt = npoiHelper.ExcelToDataTable();
                Dictionary<int, string> errorMsgs = CheckData(dt);

                if (errorMsgs.Count > 0)
                {
                    string fileName = npoiHelper.GenerateErrorExcel(errorMsgs);
                    FileInfo fileInfo = new FileInfo(fileName);
                    var fileFullName = fileInfo.FullName;
                    Uri uri = new Uri(url);
                    var name = "http://" + uri.Host + "//" + fileFullName.Substring(
                        fileFullName.IndexOf(str.Replace("/", "\\")),
                        fileFullName.Length - fileFullName.IndexOf(str.Replace("/", "\\")));

                    result.IsFailed(message: name);
                    return result;
                }
                else
                {
                    return AddRange(dt);
                }
            }
            catch (Exception ex)
            {
                result.IsFailed(ex);
                return result;
            }
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> CheckData(DataTable dt)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            Dictionary<string, decimal> total = new Dictionary<string, decimal>(); //余额检查

            using (var accountsView = new AccountsRoll())
            using (var accountCatalogsView = new AccountCatalogsRoll())
            using (var enterprisesView = new EnterprisesRoll())
            {
                var accounts = accountsView.Where(item => item.Status == GeneralStatus.Normal)
                    .Select(item => item.Code).ToArray();
                var enterprises = enterprisesView.Select(item => new { item.ID, item.Name }).ToArray();

                string accountCatalog = string.Empty; //收款类型（供应链业务-预收账款）
                string payeeCode = string.Empty; //收款银行账号
                string paymentMethord = string.Empty; //收款方式
                string payerName = string.Empty; //付款公司
                string payerNature = string.Empty; //付款账户性质

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //收款类型
                    accountCatalog = dt.Rows[i]["收款类型"].ToString().Trim();
                    if (string.IsNullOrEmpty(accountCatalog))
                    {
                        dic.Add(i + 1, $"收款类型不能为空!");
                        continue;
                    }
                    else if (string.IsNullOrEmpty(accountCatalogsView.GetID(
                        accountCatalog.Split(new string[] { "-", "—", "——" }, StringSplitOptions.RemoveEmptyEntries))))
                    {
                        dic.Add(i + 1, $"[{accountCatalog}] 收款类型格式不正确!");
                        continue;
                    }

                    //收款银行账号
                    payeeCode = dt.Rows[i]["收款银行账号"].ToString().Trim();
                    if (string.IsNullOrWhiteSpace(payeeCode))
                    {
                        dic.Add(i + 1, $"收款银行账号不能为空!");
                        continue;
                    }
                    else if (!accounts.Contains(payeeCode))
                    {
                        dic.Add(i + 1, $"[{payeeCode}] 收款银行账号不存在，请您检查是否录入正确!");
                        continue;
                    }

                    //金额
                    if (string.IsNullOrEmpty(dt.Rows[i]["金额"].ToString()))
                    {
                        dic.Add(i + 1, $"金额不能为空!");
                        continue;
                    }

                    //收款方式
                    paymentMethord = dt.Rows[i]["收款方式"].ToString().Trim();
                    if (string.IsNullOrEmpty(paymentMethord))
                    {
                        dic.Add(i + 1, $"收款方式不能为空!");
                        continue;
                    }
                    else if (GetPaymentMethord(paymentMethord) == null)
                    {
                        dic.Add(i + 1, $"[{paymentMethord}]收款方式不存在!");
                        continue;
                    }

                    //付款公司
                    payerName = dt.Rows[i]["付款公司"].ToString().Trim();
                    if (string.IsNullOrEmpty(payerName))
                    {
                        dic.Add(i + 1, $"付款公司不能为空!");
                        continue;
                    }
                    else if (!enterprises.Any(item => item.Name == payerName))
                    {
                        dic.Add(i + 1, $"[{payerName}]付款公司不存在!");
                        continue;
                    }

                    //付款账户性质
                    payerNature = dt.Rows[i]["付款账户性质"].ToString().Trim();
                    if (string.IsNullOrEmpty(payerNature))
                    {
                        dic.Add(i + 1, $"付款账户性质不能为空!");
                        continue;
                    }
                    else if (GetPayerNature(payerNature) == null)
                    {
                        dic.Add(i + 1, $"[{payerNature}]付款账户性质不能为空!");
                        continue;
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\") + "Receive.xls";
            DownLoadFile(fileName);
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="dt"></param>
        private JMessage AddRange(DataTable dt)
        {
            var result = new JMessage();

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            using (var accountsView = new AccountsRoll(reponsitory))
            using (var accountCatalogs = new AccountCatalogsRoll(reponsitory))
            using (var payeeLeftsView = new PayeeLeftsRoll(reponsitory))
            using (var flowsView = new FlowAccountsRoll(reponsitory))
            {
                var leftList = new List<PayeeLeft>();
                var flowList = new List<FlowAccount>();

                try
                {
                    string accountCatalog = string.Empty; //收款类型（供应链业务-预收账款）
                    string payeeCode = string.Empty; //收款银行账号
                    string formCode = string.Empty; //流水号
                    decimal price = 0; //金额
                    int? paymentMethord = null; //收款方式
                    string payerName = string.Empty; //付款公司
                    int? payerNature = 0; //付款账户性质
                    string summary = string.Empty; //摘要
                    Currency currency = Currency.Unknown;
                    decimal eRate1 = 1;

                    //收款账号
                    var codes = GetCodes(dt);

                    //账户
                    var accounts = accountsView.Where(item => item.Status == GeneralStatus.Normal && codes.Contains(item.Code))
                        .Select(item => new
                        { item.ID, item.Code, item.Name, item.Currency, EnterpriseName = item.Enterprise.Name })
                        .ToArray();

                    var payeeLeftIds = PKeySigner.Series(Yahv.Finance.Services.PKeyType.PayerLeft, dt.Rows.Count, orderBy: PKeySigner.OrderBy.Ascending);
                    var flowIds = PKeySigner.Series(Yahv.Finance.Services.PKeyType.FlowAcc, dt.Rows.Count, orderBy: PKeySigner.OrderBy.Ascending);
                    var list = new List<PayeeLeftInputDto>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var flowId = flowIds[i];

                        DataRow dr = dt.Rows[i];

                        accountCatalog = accountCatalogs.GetID(dr["收款类型"].ToString().Trim()
                            .Split(new string[] { "-", "—", "_" }, StringSplitOptions.RemoveEmptyEntries));
                        payeeCode = dr["收款银行账号"].ToString().Trim();
                        formCode = dr["流水号"].ToString().Trim();
                        price = decimal.Parse(dr["金额"].ToString().Trim());
                        paymentMethord = GetPaymentMethord(dr["收款方式"].ToString());
                        payerName = dr["付款公司"].ToString();
                        payerNature = GetPayerNature(dr["付款账户性质"].ToString());
                        summary = dr["摘要"].ToString();

                        dynamic account = accounts.FirstOrDefault(item => item.Code == payeeCode);
                        currency = account.Currency;
                        eRate1 = Yahv.Finance.Services.ExchangeRates.Universal[currency, Currency.CNY];

                        leftList.Add(new PayeeLeft()
                        {
                            ID = payeeLeftIds[i],
                            AccountCatalogID = accountCatalog,
                            AccountID = account?.ID,
                            AccountName = account?.Name,
                            PayerName = payerName,
                            Currency = account.Currency,
                            Price = price,

                            Currency1 = Currency.CNY,
                            ERate1 = eRate1,
                            Price1 = price * eRate1,

                            CreatorID = Erp.Current.ID,
                            FlowID = flowId,
                            Summary = summary,
                            PayerNature = (NatureType)payerNature,
                            CreateDate = DateTime.Now,
                            Status = GeneralStatus.Normal,
                        });

                        flowList.Add(new FlowAccount()
                        {
                            ID = flowId,
                            AccountMethord = AccountMethord.Receipt,
                            AccountID = account?.ID,
                            Currency = currency,
                            Price = price,
                            PaymentDate = DateTime.Now,
                            FormCode = formCode,
                            Currency1 = Currency.CNY,
                            ERate1 = eRate1,
                            Price1 = price * eRate1,
                            CreatorID = Erp.Current.ID,
                            TargetAccountName = payerName,
                            PaymentMethord = (Underly.PaymentMethord)paymentMethord,
                            CreateDate = DateTime.Now,
                            Type = FlowAccountType.BankStatement,
                        });

                        //互通芯达通数据
                        if (ConfigurationManager.AppSettings["XdtCompanyName"].Contains($",{account.EnterpriseName},"))
                        {
                            list.Add(new PayeeLeftInputDto()
                            {
                                CreatorID = Erp.Current.ID,
                                Summary = summary,
                                Rate = eRate1,
                                Account = account?.Code,
                                ReceiptDate = DateTime.Now,
                                Amount = price,
                                Payer = payerName,
                                SeqNo = formCode,
                                FeeType = accountCatalog,
                                ReceiptType = (int)AccountMethord.Receipt,
                                AccountSource = (int)payerNature,
                            });
                        }
                    }

                    if (flowList.Any())
                    {
                        flowsView.AddRange(flowList);
                    }

                    if (leftList.Any())
                    {
                        payeeLeftsView.AddRange(leftList);
                    }

                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }

                    AddXdtSync(list);
                    result.IsSuccess("操作成功!");
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款管理, Services.Oplogs.GetMethodInfo(), "批量新增",
                        new { leftList, flowList }.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }

                    result.IsFailed(ex);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款管理, Services.Oplogs.GetMethodInfo(), "批量新增 异常!",
                        new { leftList, flowList, exception = ex.ToString() }.Json());
                }
            }

            return result;
        }

        /// <summary>
        /// 给芯达通互通数据
        /// </summary>
        /// <param name="list"></param>
        private void AddXdtSync(List<PayeeLeftInputDto> list)
        {
            if (list.Count <= 0) return;
            var url = XdtApi.PayeeLeftEnterBatch.GetApiAddress();
            object data = new object();
            string result = string.Empty;

            try
            {
                using (var payeeLeftsView = new PayeeLeftsRoll())
                using (var flowsView = new FlowAccountsRoll())
                {
                    data = new
                    {
                        Sender = SystemSender.Center.GetFixedID(),
                        Option = OptionConsts.insert,
                        Model = list.ToArray()
                    };

                    result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款接口_Api, Services.Oplogs.GetMethodInfo(),
                        "Api 批量收款", result + data.Json(), url: url);
                }
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款接口_Api, Services.Oplogs.GetMethodInfo(),
                    "Api 批量收款异常!" + ex.Message, result + data.Json(), url: url);
            }


        }

        #region 自定义函数

        /// <summary>
        /// 获取收款方式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int? GetPaymentMethord(string name)
        {
            var paymentMethor = ExtendsEnum.ToDictionary<PaymentMethord>()
                .FirstOrDefault(item => item.Value == name);

            if (string.IsNullOrEmpty(paymentMethor.Key))
            {
                return null;
            }

            return int.Parse(paymentMethor.Key);
        }

        /// <summary>
        /// 获取付款账户性质
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int? GetPayerNature(string name)
        {
            var paymentMethor = ExtendsEnum.ToDictionary<NatureType>()
                .FirstOrDefault(item => item.Value == name);

            if (string.IsNullOrEmpty(paymentMethor.Key))
            {
                return null;
            }

            return int.Parse(paymentMethor.Key);
        }

        /// <summary>
        /// 获取所有卡号
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string[] GetCodes(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }

            var list = new List<string>();
            foreach (DataRow dataRow in dt.Rows)
            {
                if (list.All(item => item != dataRow["收款银行账号"].ToString()))
                    list.Add(dataRow["收款银行账号"].ToString());
            }

            return list.ToArray();
        }
        #endregion
    }
}