using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using ApprovalStatus = Yahv.Finance.Services.Enums.ApprovalStatus;

namespace Yahv.Finance.WebApp.Payer.FundTransfer
{
    public partial class Edit : ErpParticlePage
    {
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
                var data = Erp.Current.Finance.SelfAppliesView[id];

                using (var view = new SelfLeftsRoll())
                {
                    this.Model.Data = new
                    {
                        PayeeAccountID = data.PayeeAccountID,
                        PayerAccountID = data.PayerAccountID,
                        ApplierID = data.ApplierID,
                        ApproverID = data.ApproverID,
                        Summary = data.Summary,
                        Price = data.Price,
                        data.Status,
                        AccountCatalogID = view.FirstOrDefault(item => item.ApplyID == id)?.AccountCatalogID,
                        PayerPrice = data.Price,
                        PayeePrice = data.TargetPrice,
                        Rate = data.TargetERate,
                    };
                }
            }


            var accounts = Erp.Current.Finance.Accounts.Where(item => item.Status == GeneralStatus.Normal && item.EnterpriseID != null && item.NatureType == NatureType.Public);

            //调入账户
            this.Model.PayeeAccounts = accounts.Where(item => (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray()
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

            //调出账户
            this.Model.PayerAccounts = accounts.Where(item => (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray()
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

            //账款分类
            //this.Model.AccountCatalogs = new MyAccountCatalogsTree(Erp.Current).Json(AccountCatalogType.Output.GetDescription()).Replace("\"name\":", "\"text\":");
            this.Model.AccountCatalogs = ExtendsEnum.ToDictionary<FundTransferType>().Select(item => new { text = item.Value, value = item.Key });
        }
        #endregion

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };

            string id = Request.QueryString["id"];
            SelfApply selfApply = null;
            SelfLeft payerLeft = null;
            SelfLeft payeeLeft = null;

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            using (var lefts = new SelfLeftsRoll(reponsitory))
            {
                try
                {

                    var payeeAccountID = Request.Form["PayeeAccountID"];        //收款账户
                    var payerAccountID = Request.Form["PayerAccountID"];        //付款账户
                    var payeeCurrency = (Currency)int.Parse(Request.Form["PayeeCurrencyID"]);        //调入币种
                    var payerCurrency = (Currency)int.Parse(Request.Form["PayerCurrencyID"]);        //调出币种
                    var payerPrice = decimal.Parse(Request.Form["PayerPrice"]);     //调出金额
                    var payeePrice = decimal.Parse(Request.Form["PayeePrice"]);       //调入金额
                    var accountCatalogID = Request.Form["AccountCatalogID"];

                    var erate = ExchangeRates.Universal[payerCurrency, payeeCurrency];
                    var payeeRate = ExchangeRates.Universal[payeeCurrency, Currency.CNY];
                    var payerRate = ExchangeRates.Universal[payerCurrency, Currency.CNY];

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        //申请
                        selfApply = new SelfApply()
                        {
                            Currency = payerCurrency,       //调出币种
                            Price = payerPrice,             //调出金额
                            ApplierID = Request.Form["ApplierID"],
                            ApproverID = Request.Form["ApproverID"],
                            CreatorID = Erp.Current.ID,
                            Status = ApplyStauts.Waiting,
                            Summary = Request.Form["Summary"] ?? null,
                            PayeeAccountID = payeeAccountID,
                            PayerAccountID = payerAccountID,
                            SenderID = SystemSender.Center.GetFixedID(),
                            TargetCurrency = payeeCurrency,
                            TargetERate = erate,
                            TargetPrice = payeePrice,
                        };
                        selfApply.Enter();

                        //调出
                        payerLeft = new SelfLeft()
                        {
                            PayeeAccountID = payeeAccountID,
                            PayerAccountID = payerAccountID,
                            Currency = payerCurrency,
                            ApplyID = selfApply.ID,
                            CreatorID = Erp.Current.ID,
                            Currency1 = Currency.CNY,
                            ERate1 = payerRate,
                            Price1 = (payerPrice * payerRate).Round(),
                            Price = payerPrice,
                            Status = GeneralStatus.Normal,
                            AccountCatalogID = accountCatalogID,
                            CreateDate = DateTime.Now,
                            AccountMethord = AccountMethord.Output,
                        };
                        payerLeft.Enter();

                        //调入
                        payeeLeft = new SelfLeft()
                        {
                            AccountMethord = AccountMethord.Input,
                            CreateDate = DateTime.Now,
                            Currency = payeeCurrency,
                            AccountCatalogID = accountCatalogID,
                            ApplyID = selfApply.ID,
                            CreatorID = Erp.Current.ID,
                            Currency1 = Currency.CNY,
                            ERate1 = payeeRate,
                            PayeeAccountID = payeeAccountID,
                            PayerAccountID = payerAccountID,
                            Price = payeePrice,
                            Price1 = (payeePrice * payeeRate).Round(),
                            Status = GeneralStatus.Normal,
                        };
                        payeeLeft.Enter();
                    }
                    else
                    {
                        //申请
                        selfApply = Erp.Current.Finance.SelfAppliesView[id];
                        selfApply.Currency = payerCurrency;
                        selfApply.ApplierID = Request.Form["ApplierID"];
                        selfApply.ApproverID = Request.Form["ApproverID"];
                        selfApply.CreatorID = Erp.Current.ID;
                        selfApply.Price = payerPrice;
                        selfApply.Status = ApplyStauts.Waiting;
                        selfApply.Summary = Request.Form["Summary"] ?? null;
                        selfApply.PayeeAccountID = payeeAccountID;
                        selfApply.PayerAccountID = payerAccountID;
                        selfApply.SenderID = SystemSender.Center.GetFixedID();
                        selfApply.TargetCurrency = payeeCurrency;
                        selfApply.TargetERate = erate;
                        selfApply.TargetPrice = payeePrice;

                        //调出
                        payerLeft = lefts.FirstOrDefault(item => item.ApplyID == id && item.AccountMethord == AccountMethord.Output);
                        if (payerLeft == null)
                            throw new Exception("未找到调出信息!");
                        payerLeft.PayeeAccountID = payeeAccountID;
                        payerLeft.PayerAccountID = payerAccountID;
                        payerLeft.Currency = payerCurrency;
                        payerLeft.ApplyID = selfApply.ID;
                        payerLeft.CreatorID = Erp.Current.ID;
                        payerLeft.Currency1 = Currency.CNY;
                        payerLeft.ERate1 = payerRate;
                        payerLeft.Price1 = (payerPrice * payerRate).Round();
                        payerLeft.Price = payerPrice;
                        payerLeft.Status = GeneralStatus.Normal;
                        payerLeft.AccountCatalogID = Request.Form["AccountCatalogID"];

                        //调入
                        payeeLeft = lefts.FirstOrDefault(item => item.ApplyID == id && item.AccountMethord == AccountMethord.Input);
                        if (payeeLeft == null)
                            throw new Exception("未找到调入信息!");
                        payeeLeft.AccountMethord = AccountMethord.Input;
                        payeeLeft.CreateDate = DateTime.Now;
                        payeeLeft.Currency = payeeCurrency;
                        payeeLeft.AccountCatalogID = accountCatalogID;
                        payeeLeft.ApplyID = selfApply.ID;
                        payeeLeft.CreatorID = Erp.Current.ID;
                        payeeLeft.Currency1 = Currency.CNY;
                        payeeLeft.ERate1 = payeeRate;
                        payeeLeft.PayeeAccountID = payeeAccountID;
                        payeeLeft.PayerAccountID = payerAccountID;
                        payeeLeft.Price = payeePrice;
                        payeeLeft.Price1 = (payeePrice * payeeRate).Round();
                        payeeLeft.Status = GeneralStatus.Normal;


                        selfApply.Enter();
                        payerLeft.Enter();
                        payeeLeft.Enter();
                    }

                    //附件
                    FilesEnter(selfApply.ID);
                    //日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.FundTransfer, ApprovalStatus.Submit, selfApply.ID, Erp.Current.ID);
                    tran.Commit();

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.资金调拨申请, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(id) ? "新增" : "修改", new { selfApply, payerLeft, payeeLeft }.Json());
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    json.success = false;
                    json.data = "添加失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.资金调拨申请, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(id) ? "新增" : "修改") + " 异常!", new { selfApply, payerLeft, payeeLeft, exception = ex.ToString() }.Json());
                    return json;
                }
            }
            return json;
        }
        #endregion

        #region 功能函数
        /// <summary>
        /// 获取admins
        /// </summary>
        /// <returns></returns>
        protected object getApplyAdmins()
        {
            return Yahv.Erp.Current.Finance.Admins
                .GetApplyAdmins()
                .OrderBy(t => t.RealName)
                .Select(item => new
                {
                    value = item.ID,
                    text = item.RealName,
                    selected = item.ID == Erp.Current.ID
                })
                .ToArray();
        }

        protected object getApproveAdmins()
        {
            return Yahv.Erp.Current.Finance.Admins
                .GetApproveAdmins()
                .OrderBy(t => t.RealName)
                .Select(item => new { value = item.ID, text = item.RealName })
                .ToArray();
        }


        /// <summary>
        /// 加载附件
        /// </summary>
        protected void filedata()
        {
            var files = Erp.Current.Finance.FilesDescriptionView
                .SearchByFilesMapValue(FilesMapName.SelfApplyID.ToString(), Request.QueryString["id"]);
            string fileWebUrlPrefix = ConfigurationManager.AppSettings["FileWebUrlPrefix"];
            Func<FilesDescription, object> convert = item => new
            {
                ID = item.ID,
                CustomName = item.CustomName,
                FileFormat = "",
                Url = item.Url,    //数据库相对路径
                WebUrl = string.Concat(fileWebUrlPrefix, "/" + item.Url),
            };
            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count(),
            }.Json());
        }

        /// <summary>
        /// 审批日志
        /// </summary>
        /// <returns></returns>
        protected object getLogs()
        {
            string applyId = Request.QueryString["id"];

            return Erp.Current.Finance.LogsApplyStepView.Where(item => item.ApplyID == applyId)
                .OrderByDescending(item => item.CreateDate).ToArray().ToArray()
                .Select(item => new
                {
                    item.ID,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ApproverName = item.Approver?.RealName,
                    Status = item.Status.GetDescription(),
                    Summary = item.Summary,
                });
        }

        /// <summary>
        /// 计算汇率
        /// </summary>
        /// <returns></returns>
        protected object calcRate()
        {
            try
            {
                Currency from = (Currency)int.Parse(Request.QueryString["from"]);
                Currency to = (Currency)int.Parse(Request.QueryString["to"]);

                return ExchangeRates.Universal[from, to].ToRound1(4);
            }
            catch (Exception ex)
            {
                return "未知";
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

        #region 私有函数
        /// <summary>
        /// 新增附件
        /// </summary>
        /// <param name="applyId"></param>
        private void FilesEnter(string applyId)
        {
            var fileIds = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.SelfApplyID.ToString(), applyId).Select(item => item.ID).ToArray();

            //前端获取的附件数据
            string fileData = Request.Form["files"].Replace("&quot;", "'");
            List<FilesDescription> files = fileData.JsonTo<List<FilesDescription>>();

            //非新增附件
            var fileIds_exist = files.Where(item => !string.IsNullOrEmpty(item.ID)).Select(item => item.ID).ToArray();

            for (int i = 0; i < files.Count(); i++)
            {
                files[i].Type = Services.Enums.FileDescType.SelfApplyVoucher;
                files[i].CreatorID = Erp.Current.ID;

                List<FilesMap> filesMaps = new List<FilesMap>();
                filesMaps.Add(new FilesMap()
                {
                    Name = FilesMapName.SelfApplyID.ToString(),
                    Value = applyId,
                });
                files[i].FilesMapsArray = filesMaps.ToArray();
            }
            //删除不存在的附件
            var ids = fileIds.Where(item => !fileIds_exist.Contains(item)).ToArray();

            Erp.Current.Finance.FilesDescriptionView.Add(files.Where(item => string.IsNullOrEmpty(item.ID)).ToArray());
            Erp.Current.Finance.FilesDescriptionView.Delete(ids);
        }
        #endregion
    }
}