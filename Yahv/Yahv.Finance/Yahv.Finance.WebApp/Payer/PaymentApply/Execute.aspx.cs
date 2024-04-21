using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using ApprovalStatus = Yahv.Underly.ApprovalStatus;

namespace Yahv.Finance.WebApp.Payer.PaymentApply
{
    public partial class Execute : ErpParticlePage
    {
        private DbTransaction tran = null;

        #region 加载数据

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        public void InitData()
        {
            var id = Request.QueryString["id"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                var data = Erp.Current.Finance.PayerAppliesView[id];

                this.Model.Data = new
                {
                    PayeeAccountID = data.PayeeAccount?.Name,
                    PayeeCurrency = data.PayeeAccount?.Currency.GetDescription(),
                    PayeeCode = data.PayeeAccount?.Code,
                    PayeeBank = data.PayeeAccount?.BankName,
                    PayerAccountID = data.PayerAccount?.ID,
                    PayerCurrency = data.PayerAccount?.Currency.GetDescription(),
                    PayerCode = data.PayerAccount?.Code,
                    PayerBank = data.PayerAccount?.BankName,
                    IsPaid = data.IsPaid,
                    AccountCatalogID = data.PayerLeft.AccountCatalogID,
                    Price = data.Price,
                    ApplierID = data.Applier?.RealName,
                    Summary = data.Summary,

                    PaymentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    PayeeCurrencyID = data?.PayeeAccount?.Currency,
                    PayerCurrencyID = data?.PayerAccount?.Currency,
                };

                //付款类型
                this.Model.AccountCatalogs =
                    AccountCatalogsAlls.Current.Json(AccountCatalogType.Output.GetDescription());
                //付款方式
                this.Model.PaymentMethord = ExtendsEnum.ToDictionary<PaymentMethord>()
                    .Select(item => new { value = item.Key, text = item.Value });
                //付款账户 内部公司
                var accounts = Erp.Current.Finance.Accounts
                    .Where(item => item.Status == GeneralStatus.Normal && item.EnterpriseID == data.PayerID).ToArray();
                var payerAccounts = accounts.Where(item =>
                        item.NatureType == NatureType.Public && item.Enterprise != null &&
                        (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray()
                    .Select(item => new
                    {
                        item.ID,
                        ShortName = item.ShortName ?? item.Name,
                        CompanyName = item.Enterprise?.Name,
                        item.BankName,
                        Currency = item.Currency.GetDescription(),
                        CurrencyID = (int)item.Currency,
                        item.Code,
                    });
                this.Model.PayerAccounts = payerAccounts;
            }
        }

        #endregion

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "操作成功!" };

            FlowAccount flowAccount = null;
            PayerRight payerRight = null;

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (tran = reponsitory.OpenTransaction())
            using (var accountsView = new AccountsRoll(reponsitory))
            using (var flowsView = new FlowAccountsRoll(reponsitory))
            using (var payerRightsView = new PayerRightsRoll(reponsitory))
            using (var payerLeftsView = new PayerLeftsRoll(reponsitory))
            {
                try
                {
                    string id = Request.QueryString["id"];
                    var entity = Erp.Current.Finance.PayerAppliesView[id];
                    if (entity == null)
                    {
                        json.success = false;
                        json.data = "未找到该申请信息!";
                        return json;
                    }

                    //修改申请状态
                    entity.Status = ApplyStauts.Completed;

                    //付款账户
                    var account = accountsView.FirstOrDefault(item => item.ID == Request.Form["PayerAccountID"]);
                    if (account == null)
                    {
                        throw new Exception($"付款账户不存在!");
                    }

                    if (!string.IsNullOrEmpty(Request.Form["FormCode"]) &&
                        flowsView.Any(item => item.FormCode == Request.Form["FormCode"]))
                    {
                        json.success = false;
                        json.data = "流水号已存在，不能重复录入!";
                        return json;
                    }

                    //付款账户对收款账户的汇率
                    var targetRate = !string.IsNullOrEmpty(Request.Form["TargetRate"]) ? decimal.Parse(Request.Form["TargetRate"]) : 1;
                    //付款金额
                    var price = decimal.Parse(Request.Form["Price"]);
                    //手续费
                    var serviceCharge = Request.Form["ServiceCharge"];
                    //流水号
                    var formCode = Request.Form["FormCode"];
                    //付款日期
                    var paymentDate = DateTime.Parse(Request.Form["PaymentDate"]);
                    //付款方式
                    var paymentMethord = (PaymentMethord)int.Parse(Request.Form["PaymentMethord"]);
                    var flowList = new List<FlowAccount>();
                    var payerRightList = new List<PayerRight>();

                    entity.Price = price;
                    entity.PayerAccountID = account.ID;
                    entity.Currency = account.Currency;
                    entity.ExcuterID = Erp.Current.ID;

                    //添加流水
                    var rate = ExchangeRates.Universal[account.Currency, Currency.CNY];
                    string flowId_temp = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);
                    flowList.Add(new FlowAccount()
                    {
                        ID = flowId_temp,
                        Currency = entity.Currency,
                        CreateDate = DateTime.Now,
                        AccountID = entity.PayerAccountID,
                        AccountMethord = AccountMethord.Payment,
                        CreatorID = Erp.Current.ID,
                        Currency1 = Currency.CNY,
                        ERate1 = rate,
                        FormCode = formCode,
                        TargetAccountCode = entity.PayeeAccount.Code,
                        TargetAccountName = entity.PayeeAccount.Name,
                        TargetRate = targetRate,
                        PaymentDate = paymentDate,
                        Price = -entity.Price,
                        Price1 = -(entity.Price * rate).Round(),
                        PaymentMethord = paymentMethord,
                    });

                    //添加核销
                    payerRightList.Add(new PayerRight()
                    {
                        Currency = entity.Currency,
                        CreateDate = DateTime.Now,
                        CreatorID = Erp.Current.ID,
                        Currency1 = Currency.CNY,
                        FlowID = flowId_temp,
                        ERate1 = rate,
                        PayerLeftID = entity.PayerLeft.ID,
                        Price = entity.Price,
                        Price1 = (entity.Price * rate).Round()
                    });

                    //修改应付 付款账户
                    var lefts = payerLeftsView.Where(item => item.ApplyID == entity.ID).ToArray();
                    foreach (var payerLeft in lefts)
                    {
                        payerLeft.PayerAccountID = account.ID;
                        payerLeft.Enter();
                    }

                    //手续费
                    if (!serviceCharge.IsNullOrEmpty())
                    {
                        var catalogId = new AccountCatalogsRoll().GetID("付款类型", "综合业务", "费用", "银行手续费");
                        var charge = decimal.Parse(serviceCharge);

                        var payerLeft = new PayerLeft()
                        {
                            PayerAccountID = account.ID,
                            PayerID = entity.PayerID,
                            Currency = entity.Currency,
                            ApplyID = entity.ID,
                            CreatorID = Erp.Current.ID,
                            Currency1 = Currency.CNY,
                            ERate1 = rate,
                            Price1 = (charge * rate).Round(),
                            Price = charge,
                            Status = GeneralStatus.Normal,
                            AccountCatalogID = catalogId,
                        };
                        payerLeft.Enter();

                        //流水
                        flowId_temp = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);
                        flowList.Add(new FlowAccount()
                        {
                            ID = flowId_temp,
                            Currency = account.Currency,
                            CreateDate = DateTime.Now,
                            AccountID = account.ID,
                            AccountMethord = AccountMethord.Payment,
                            CreatorID = Erp.Current.ID,
                            Currency1 = Currency.CNY,
                            ERate1 = rate,
                            FormCode = formCode,
                            PaymentDate = paymentDate,
                            Price = -charge,
                            Price1 = -(charge * rate).Round(),
                            PaymentMethord = paymentMethord,
                            Type = FlowAccountType.BankStatement,
                        });

                        //添加核销
                        payerRightList.Add(new PayerRight()
                        {
                            Currency = account.Currency,
                            CreateDate = DateTime.Now,
                            CreatorID = Erp.Current.ID,
                            Currency1 = Currency.CNY,
                            FlowID = flowId_temp,
                            ERate1 = rate,
                            PayerLeftID = payerLeft.ID,
                            Price = charge,
                            Price1 = (charge * rate).Round()
                        });
                    }

                    if (flowList.Count > 0)
                    {
                        flowsView.AddRange(flowList);
                    }

                    if (payerRightList.Count > 0)
                    {
                        payerRightsView.AddRange(payerRightList);
                    }

                    //附件
                    FilesEnter(id);

                    //添加审批日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.ProductsFee,
                        Services.Enums.ApprovalStatus.Payment, id, Erp.Current.ID, Request.Form["Comments"]);
                    entity.Temp1 = formCode; //临时传参   流水号
                    entity.Temp2 = Request.Form["PaymentDate"]; //临时传参    付款日期
                    entity.Temp3 = targetRate;
                    entity.EnterSuccess += Entity_EnterSuccess;
                    entity.Enter();

                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.货款申请, Services.Oplogs.GetMethodInfo(), "支付",
                        new { flowAccount, payerRight }.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }

                    json.success = false;
                    json.data = "操作失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.货款申请, Services.Oplogs.GetMethodInfo(), "支付 异常!",
                        new { flowAccount, payerRight, exception = ex.ToString() }.Json());
                    return json;
                }
            }

            return json;
        }

        /// <summary>
        /// 同步CrmPays
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as PayerApply;

            try
            {
                if (tran?.Connection != null)
                {
                    tran.Commit();
                }

                using (var accountsView = new AccountsRoll())
                {
                    if (entity == null)
                    {
                        Services.Oplogs.Oplog(Erp.Current.ID, LogModular.代付货款申请_To_Crm, Services.Oplogs.GetMethodInfo(),
                            $"代付货款申请同步Crm异常!", "未找到数据!" + entity.Json());
                        return;
                    }

                    //付款公司
                    var payerAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayerAccountID);
                    //收款公司
                    var payeeAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayeeAccountID);

                    var rate = entity.Temp3 == null ? 1 : decimal.Parse(entity.Temp3.ToString());

                    Yahv.Payments.PaymentManager.Erp(Erp.Current.ID)[payerAccount?.Enterprise.Name,
                            payeeAccount?.Enterprise?.Name].Digital
                        .AdvanceToSuppliers(payeeAccount.Currency, entity.Price * rate, payeeAccount?.BankName,
                            payeeAccount?.Code, entity.Temp1, DateTime.Parse(entity.Temp2));


                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.代付货款申请_To_Crm, Services.Oplogs.GetMethodInfo(),
                        $"代付货款申请同步Crm!", entity.Json());
                }
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.代付货款申请_To_Crm, Services.Oplogs.GetMethodInfo(),
                    $"代付货款申请同步Crm异常!", ex.Message + entity.Json());
            }


        }

        #endregion

        #region 功能函数

        /// <summary>
        /// 获取admins
        /// </summary>
        /// <returns></returns>
        protected object getAdmins()
        {
            return Yahv.Erp.Current.Finance.Admins
                .Where(t => t.Status != Underly.AdminStatus.Closed)
                .OrderBy(t => t.RealName)
                .Select(item => new { value = item.ID, text = item.RealName })
                .ToArray();
        }


        /// <summary>
        /// 加载附件
        /// </summary>
        protected void filedata()
        {
            var files = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(
                FilesMapName.PayerApplyID.ToString(), Request.QueryString["id"]);
            string fileWebUrlPrefix = ConfigurationManager.AppSettings["FileWebUrlPrefix"];
            Func<FilesDescription, object> convert = item => new
            {
                ID = item.ID,
                CustomName = item.CustomName,
                FileFormat = "",
                Url = item.Url, //数据库相对路径
                WebUrl = string.Concat(fileWebUrlPrefix, "/" + item.Url),
            };
            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count(),
            }.Json());
        }

        /// <summary>
        /// 汇率
        /// </summary>
        /// <returns></returns>
        protected decimal? getRate()
        {
            try
            {
                var payerCurrency = (Currency)int.Parse(Request.Form["payerCurrency"]);
                var payeeCurrency = (Currency)int.Parse(Request.Form["payeeCurrency"]);

                return ExchangeRates.Universal[payerCurrency, payeeCurrency];
            }
            catch (Exception)
            {
                return 1;
            }
        }

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
                fileName = files.Count == 1
                    ? string.Concat(topID, Path.GetExtension(file.FileName))
                    : string.Concat(topID, "-", (counter++).ToString().PadLeft(digit, '0'),
                        Path.GetExtension(file.FileName));
                string Url = string.Join("/", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(),
                    DateTime.Now.Day.ToString(), fileName);
                DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["FileSavePath"]);
                string fullFileName = Path.Combine(di.FullName, DateTime.Now.Year.ToString(),
                    DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), fileName);

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

        #region 私有函数

        /// <summary>
        /// 新增附件
        /// </summary>
        /// <param name="applyId"></param>
        private void FilesEnter(string applyId)
        {
            var fileIds = Erp.Current.Finance.FilesDescriptionView
                .SearchByFilesMapValue(FilesMapName.PayerApplyID.ToString(), applyId).Select(item => item.ID).ToArray();

            //前端获取的附件数据
            string fileData = Request.Form["files"].Replace("&quot;", "'");
            List<FilesDescription> files = fileData.JsonTo<List<FilesDescription>>();

            //非新增附件
            var fileIds_exist = files.Where(item => !string.IsNullOrEmpty(item.ID)).Select(item => item.ID).ToArray();

            for (int i = 0; i < files.Count(); i++)
            {
                files[i].Type = Services.Enums.FileDescType.ProductsFeeVoucher;
                files[i].CreatorID = Erp.Current.ID;

                List<FilesMap> filesMaps = new List<FilesMap>();
                filesMaps.Add(new FilesMap()
                {
                    Name = FilesMapName.PayerApplyID.ToString(),
                    Value = applyId,
                });
                files[i].FilesMapsArray = filesMaps.ToArray();
            }

            //删除不存在的附件
            var ids = fileIds.Where(item => !fileIds_exist.Contains(item)).ToArray();

            Erp.Current.Finance.FilesDescriptionView.Add(files.Where(item => string.IsNullOrEmpty(item.ID)).ToArray());
            Erp.Current.Finance.FilesDescriptionView.Delete(ids);
        }

        /// <summary>
        /// 添加手续费
        /// </summary>
        private void AddServiceCharge()
        {

        }

        #endregion

    }
}