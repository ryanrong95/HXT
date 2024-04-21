using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Finance.Services.Models.Origins;
using Layers.Data;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Linq.Extends;
using Yahv.Payments;
using Yahv.Underly.Enums;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Extends;

namespace Yahv.Finance.WebApp.Payee
{
    public partial class Add : ErpParticlePage
    {
        private DbTransaction tran = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var payeeLeftId = Request.QueryString["PayeeLeftID"];

                if (!string.IsNullOrWhiteSpace(payeeLeftId))
                {
                    this.Model.PayeeLeftID = payeeLeftId;
                    var payeeLeft = Erp.Current.Finance.PayeeLefts[payeeLeftId];
                    this.Model.DataPayeeLeft = payeeLeft;
                    if (payeeLeft != null)
                    {
                        this.Model.DataFlowAccount = Erp.Current.Finance.FlowAccounts[payeeLeft.FlowID];
                    }
                }

                //收款方式
                var acceptances = new string[]
                {
                    //((int)PaymentMethord.BankAcceptanceBill).ToString()
                    //,((int)PaymentMethord.CommercialAcceptanceBill).ToString()
                };
                this.Model.PaymentMethord = ExtendsEnum.ToDictionary<PaymentMethord>()
                    .Where(item => !acceptances.Contains(item.Key))
                    .Select(item => new { value = item.Key, text = item.Value });
                //账户性质
                this.Model.PayerNature = ExtendsEnum.ToDictionary<NatureType>().Where(item => item.Key != ((int)NatureType.UnKnown).ToString()).Select(item => new { value = item.Key, text = item.Value });
                //账户
                this.Model.Accounts = GetAccounts();
                ////承兑账户ID
                //this.Model.MoAccountTypeIds = Erp.Current.Finance.Accounts.GetMoAccountTypeID();
                ////银行承兑账户
                //this.Model.MosBank = GetMoneyOrdersByType(PaymentMethord.BankAcceptanceBill, string.IsNullOrEmpty(payeeLeftId));
                ////商业承兑账户
                //this.Model.MosCommercial = GetMoneyOrdersByType(PaymentMethord.CommercialAcceptanceBill, string.IsNullOrEmpty(payeeLeftId));
            }
        }

        #region 加载数据
        /// <summary>
        /// 加载附件
        /// </summary>
        protected void filedata()
        {
            string PayeeLeftID = Request.QueryString["PayeeLeftID"];

            using (var query1 = new Yahv.Finance.Services.Views.Rolls.FilesDescriptionRoll())
            {
                var view = query1;

                view = view.SearchByFilesMapValue("PayeeLeftID", PayeeLeftID);

                var files = view.ToArray();

                string fileWebUrlPrefix = ConfigurationManager.AppSettings["FileWebUrlPrefix"];

                Func<FilesDescription, object> convert = item => new
                {
                    FileID = item.ID,
                    CustomName = item.CustomName,
                    FileFormat = "",
                    Url = item.Url,    //数据库相对路径
                                       //WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),//查看路径
                                       //WebUrl = (DateTime.Compare(item.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0)
                                       //? FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl()
                                       //: FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
                    WebUrl = string.Concat(fileWebUrlPrefix, "/" + item.Url),
                };
                Response.Write(new
                {
                    rows = files.Select(convert).ToArray(),
                    total = files.Count(),
                }.Json());
            }
        }
        #endregion

        #region 功能按钮
        #region 提交保存

        protected void Submit()
        {

            FlowAccount flowAccount = null;
            Enterprise enterprise = null;
            PayeeLeft payeeLeft = null;
            MoneyOrder moneyOrder = null;

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var flowsView = new FlowAccountsRoll(reponsitory))
            using (var moneyOrderView = new MoneyOrdersRoll(reponsitory))
            using (var accountsView = new AccountsRoll(reponsitory))
            using (tran = reponsitory.OpenTransaction())
            {
                try
                {
                    string AccountCatalog = Request.Form["AccountCatalog"];
                    string Account = Request.Form["Account"];
                    string CurrencyInt = Request.Form["CurrencyInt"];
                    string FormCode = Request.Form["FormCode"];
                    string Price = Request.Form["Price"];
                    string ReceiptDate = Request.Form["ReceiptDate"];
                    string PayerAccountName = Request.Form["PayerAccountName"];
                    string PaymentMethord = Request.Form["PaymentMethord"];
                    var PayerNature = (Underly.NatureType)(int.Parse(Request.Form["PayerNature"]));
                    string Summary = Request.Form["Summary"];
                    string TargetAccountCode = Request.Form["TargetAccountCode"];
                    string accountName = Request.Form["PayeeMan"];
                    string fileData = Request.Form["FileData"].Replace("&quot;", "'");
                    //string moneyOrderId = Request.Form["MoneyOrderID"];

                    //收款时间处理
                    DateTime? DtReceiptDate = null;
                    DateTime resultReceiptDate;
                    if (DateTime.TryParse(ReceiptDate, out resultReceiptDate))
                    {
                        DtReceiptDate = resultReceiptDate;
                    }

                    if (!string.IsNullOrEmpty(FormCode) && flowsView.Any(item => item.FormCode == FormCode))
                    {
                        throw new Exception("流水号已存在，不能重复录入!");
                    }

                    //if (!string.IsNullOrEmpty(moneyOrderId))
                    //{
                    //    moneyOrder = moneyOrderView[moneyOrderId];
                    //}

                    //附件
                    List<FilesDescription> files = fileData.JsonTo<List<FilesDescription>>();

                    string newPayeeLeftID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.PayeeLeft);
                    string newFlowID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);

                    var currency = (Currency)(int.Parse(CurrencyInt));
                    var eRate1 = Yahv.Finance.Services.ExchangeRates.Universal[currency, Currency.CNY];

                    var paymentMethord = (Underly.PaymentMethord)(int.Parse(PaymentMethord));
                    FlowAccountType faType = FlowAccountType.BankStatement;

                    //如果是承兑汇票，修改流水表账户类型
                    //if (paymentMethord == Underly.PaymentMethord.CommercialAcceptanceBill ||
                    //    paymentMethord == Underly.PaymentMethord.BankAcceptanceBill)
                    //{
                    //    faType = FlowAccountType.MoneyOrder;
                    //    moneyOrder.Status = MoneyOrderStatus.Ticketed;
                    //    moneyOrder.ModifierID = Erp.Current.ID;
                    //    moneyOrder.ModifyDate = DateTime.Now;

                    //    //是否为承兑账户
                    //    if (!accountsView.IsAcceptanceAccount(Account))
                    //    {
                    //        throw new Exception("请您选择承兑账户!");
                    //    }
                    //}

                    //新增客户(公司)
                    enterprise = null;

                    if (!string.IsNullOrEmpty(PayerAccountName) && PayerNature == NatureType.Public)
                    {
                        enterprise = Erp.Current.Finance.Enterprises
                            .Where(t => t.Name == PayerAccountName && t.Status == GeneralStatus.Normal).FirstOrDefault();

                        if (enterprise == null)
                        {
                            //要新增一个 Enterprise
                            enterprise = new Enterprise()
                            {
                                ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.Enterprise),
                                Name = PayerAccountName,
                                Type = EnterpriseAccountType.Client,
                                District = null,
                                CreatorID = Erp.Current.ID,
                                ModifierID = Erp.Current.ID,
                            };
                        }
                    }

                    payeeLeft = new PayeeLeft()
                    {
                        ID = newPayeeLeftID,
                        AccountCatalogID = AccountCatalog,
                        AccountID = Account,
                        AccountName = accountName,
                        PayerName = PayerAccountName,
                        Currency = currency,
                        Price = decimal.Parse(Price),

                        Currency1 = Currency.CNY,
                        ERate1 = eRate1,
                        Price1 = decimal.Parse(Price) * eRate1,

                        CreatorID = Erp.Current.ID,
                        FlowID = newFlowID,
                        Summary = Summary,
                        PayerNature = PayerNature,
                    };



                    flowAccount = new FlowAccount()
                    {
                        ID = newFlowID,
                        Type = faType,
                        AccountMethord = AccountMethord.Receipt,
                        AccountID = Account,
                        Currency = currency,
                        Price = decimal.Parse(Price),
                        Balance = decimal.Parse(Price),
                        PaymentDate = DtReceiptDate,
                        FormCode = FormCode,
                        Currency1 = Currency.CNY,
                        ERate1 = eRate1,
                        Price1 = decimal.Parse(Price) * eRate1,
                        Balance1 = decimal.Parse(Price) * eRate1,
                        CreatorID = Erp.Current.ID,
                        TargetAccountName = PayerAccountName,
                        TargetAccountCode = string.IsNullOrWhiteSpace(TargetAccountCode) ? null : TargetAccountCode,

                        PaymentMethord = paymentMethord,
                        //MoneyOrderID = moneyOrderId,
                    };

                    for (int i = 0; i < files.Count(); i++)
                    {
                        files[i].Type = Services.Enums.FileDescType.ReceiptVoucher;
                        files[i].CreatorID = Erp.Current.ID;

                        List<FilesMap> filesMaps = new List<FilesMap>();
                        filesMaps.Add(new FilesMap()
                        {
                            Name = "PayeeLeftID",
                            Value = newPayeeLeftID,
                        });
                        files[i].FilesMapsArray = filesMaps.ToArray();
                    }

                    if (enterprise != null)
                    {
                        enterprise.Enter();
                    }
                    flowAccount.Enter();
                    payeeLeft.AddSuccess += PayeeLeft_AddSuccess;
                    payeeLeft.AddAccountWork += PayeeLeft_AddAccountWork;
                    payeeLeft.Enter();
                    //moneyOrder?.Enter();    //修改承兑汇票状态
                    new FilesDescriptionRoll().Add(files.ToArray());
                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款管理, Services.Oplogs.GetMethodInfo(), "新增", new { payeeLeft, flowAccount, enterprise, moneyOrder }.Json());
                    Response.Write((new { success = true, message = "提交成功", }).Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款管理, Services.Oplogs.GetMethodInfo(), "新增 异常!", new { payeeLeft, flowAccount, enterprise, moneyOrder, exception = ex.ToString() }.Json());
                    Response.Write((new { success = false, message = $"提交异常!{ex.Message}", }).Json());
                }
            }
        }

        /// <summary>
        /// 添加认领数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PayeeLeft_AddAccountWork(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as PayeeLeft;
            AccountWork model = new AccountWork();
            try
            {
                model.PayeeLeftID = entity.ID;
                model.Conduct = Conduct.Chain.GetDescription();
                model.Enter();
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款认领, Services.Oplogs.GetMethodInfo(), "新增", model.Json());
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款认领, Services.Oplogs.GetMethodInfo(), "新增异常!" + ex.Message, model.Json());
            }
        }

        /// <summary>
        /// 添加成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PayeeLeft_AddSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as PayeeLeft;
            var result = string.Empty;
            var url = XdtApi.PayeeLeftEnter.GetApiAddress();
            InputParam<PayeeLeftInputDto> data = null;
            try
            {
                if (tran?.Connection != null)
                {
                    tran.Commit();
                }

                using (var payeeLeftsView = new PayeeLeftsRoll())
                using (var flowsView = new FlowAccountsRoll())
                {
                    //如果不是芯达通不需要互通
                    if (ConfigurationManager.AppSettings["XdtCompanyName"] != null && !ConfigurationManager
                        .AppSettings["XdtCompanyName"].Contains($",{entity.AccountName},"))
                    {
                        return;
                    }

                    entity = payeeLeftsView[entity.ID];
                    var flow = flowsView.FirstOrDefault(item => item.ID == entity.FlowID);
                    data = new InputParam<PayeeLeftInputDto>()
                    {
                        Sender = SystemSender.Xindatong.GetFixedID(),
                        Option = OptionConsts.insert,
                        Model = new PayeeLeftInputDto()
                        {
                            CreatorID = entity.CreatorID,
                            Summary = entity.Summary,
                            Rate = entity.ERate1,
                            Account = entity.AccountCode,
                            ReceiptDate = entity.ReceiptDate.Value,
                            Amount = entity.Price,
                            Payer = entity.PayerName,
                            SeqNo = entity.FormCode,
                            FeeType = entity.AccountCatalogID,
                            ReceiptType = (int)flow.PaymentMethord,
                            AccountSource = (int)entity.PayerNature,
                        }
                    };
                    result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款接口_Api, Services.Oplogs.GetMethodInfo(), "Api 新增", result + data.Json(), url: url);
                }
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款接口_Api, Services.Oplogs.GetMethodInfo(), "Api 新增异常!" + ex.Message, result + data.Json(), url: url);
            }
        }

        #endregion

        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        protected void upload()
        {
            var list = new List<Yahv.Services.Models.UploadResult>();
            var files = Request.Files;
            if (files.Count <= 0)
            {
                return;
            }
            string topID = FilesDescriptionRoll.GenID();
            int counter = 0;
            var digit = files.Count;
            HttpPostedFile file;
            string fileName = string.Empty;
            foreach (string key in files.AllKeys)
            {
                file = Request.Files[key];
                fileName = files.Count == 1 ? string.Concat(topID, Path.GetExtension(file.FileName)) : string.Concat(topID, "-", (counter++).ToString().PadLeft(digit, '0'), Path.GetExtension(file.FileName));
                string Url = string.Join("/", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), fileName);
                DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["FileSavePath"]);
                string fullFileName = Path.Combine(di.FullName, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), fileName);

                FileInfo fi = new FileInfo(fullFileName);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                file.SaveAs(fi.FullName);
                list.Add(new Yahv.Services.Models.UploadResult
                {
                    FileID = System.IO.Path.GetFileNameWithoutExtension(Url),
                    FileName = file.FileName,
                    Url = Url,
                });
            }
            Response.Write(list.Json());
        }
        #endregion
        #endregion

        #region 功能函数
        /// <summary>
        /// 收款类型树
        /// </summary>
        /// <returns></returns>
        protected object AccountCatalogsTree()
        {
            var treeStr = new MyAccountCatalogsTree(Erp.Current).Json(AccountCatalogType.Input.GetDescription());
            treeStr = treeStr.Replace("\"name\":", "\"text\":");
            return treeStr;
        }

        #endregion

        #region 自定义函数
        /// <summary>
        /// 获取未签收汇票
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private object GetMoneyOrdersByType(PaymentMethord type, bool IsInsert)
        {
            using (var view = new MoneyOrdersRoll())
            {
                var list = new List<MoneyOrderDto>();
                Expression<Func<MoneyOrder, bool>> predicate = item => true;

                //如果不是管理员只显示自己的承兑汇票
                if (!Erp.Current.IsSuper)
                {
                    predicate = predicate.And(item => item.CreatorID == Erp.Current.ID);
                }

                //新增只显示 已签收的承兑汇票
                //if (IsInsert)
                //{
                //    predicate = predicate.And(item => item.Status == MoneyOrderStatus.Saved);
                //}

                //银行承兑
                if (type == PaymentMethord.BankAcceptanceBill)
                {
                    predicate = predicate.And(item => item.Type == MoneyOrderType.Bank);
                    list = view.GetIEnumerable(predicate).ToList();
                }

                //商业承兑
                if (type == PaymentMethord.CommercialAcceptanceBill)
                {
                    predicate = predicate.And(item => item.Type == MoneyOrderType.Commercial);
                    list = view.GetIEnumerable(predicate).ToList();
                }

                return list.Select(item => new
                {
                    item.ID,
                    item.Name,
                    item.Code,
                    item.Price,
                    item.PayerAccountName,
                    item.PayeeAccountID,
                });
            }
        }

        /// <summary>
        /// 获取账户
        /// </summary>
        /// <returns></returns>
        private object GetAccounts()
        {
            //默认只显示自己管理的账户
            var accounts = Erp.Current.Finance.Accounts
                .Where(item => item.NatureType == NatureType.Public && item.OwnerID == Erp.Current.ID
                                                                    && (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray();

            if (Erp.Current.IsSuper)
            {
                accounts = Erp.Current.Finance.Accounts
                    .Where(item => item.NatureType == NatureType.Public
                                   && (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray();
            }

            return accounts.Select(item => new
            {
                AccountID = item.ID,
                ShortName = item.ShortName ?? item.Name,
                CompanyName = item.Enterprise?.Name,
                BankName = item.BankName,
                Code = item.Code,
                CurrencyInt = item.Currency.GetHashCode(),
                CurrencyDes = item.Currency.GetDescription(),
            });
        }
        #endregion

    }
}