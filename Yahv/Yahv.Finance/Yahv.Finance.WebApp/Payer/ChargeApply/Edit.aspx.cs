using System;
using System.Collections.Generic;
using System.Configuration;
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
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.ChargeApply
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
                this.Model.Data = Erp.Current.Finance.ChargeApplies.GetApply(id);
            }


            var accounts = Erp.Current.Finance.Accounts.Where(item => item.Status == GeneralStatus.Normal).ToArray();

            //付款账户 内部公司
            var payerAccounts = accounts.Where(item => item.NatureType == NatureType.Public && item.Enterprise != null && (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray()
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

            //收款账户 (非内部公司的账户)
            var payerAccountIds = payerAccounts.Select(item => item.ID).ToArray();
            this.Model.PayeeAccounts = accounts.Where(item => !payerAccountIds.Contains(item.ID)).ToArray()
                .Select(item => new
                {
                    item.ID,
                    Name = item?.Enterprise?.Name ?? item.Name,
                    item.BankName,
                    Currency = item.Currency.GetDescription(),
                    CurrencyID = (int)item.Currency,
                    item.Code,
                });



            //付款方式
            this.Model.PaymentMethord = ExtendsEnum.ToDictionary<PaymentMethord>().Select(item => new { value = item.Key, text = item.Value });

            //资金类型
            this.Model.AccountCatalogs = AccountCatalogsAlls.Current.Select(item => new { id = item.ID, text = item.Name });
            this.Model.AccountCatalogsJson = new MyAccountCatalogsTree(Erp.Current).Json(AccountCatalogType.Output.GetDescription()).Replace("\"name\":", "\"text\":");
            //币种
            this.Model.PayeeCurrencies = ExtendsEnum.ToDictionary<Currency>().Select(item => new { value = item.Key, text = item.Value });
        }
        #endregion

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };

            string id = Request.QueryString["id"];
            Services.Models.Origins.ChargeApply apply = null;
            Account payeeAccount = null;
            List<FlowAccount> flows = null;       //流水表
            List<ChargeApplyItem> applyItems = null;

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            using (var flowsView = new FlowAccountsRoll(reponsitory))
            {
                try
                {
                    var payeeAccountID = Request.Form["PayeeAccountID"];        //收款账户
                    var payerAccountID = Request.Form["PayerAccountID"];        //付款账户
                    var currency = (Currency)int.Parse(Request.Form["Currency"]);
                    var payerID = Request.Form["PayerID"];
                    var paymentDate = DateTime.Now;
                    var rate = ExchangeRates.Universal[currency, Currency.CNY];
                    bool isPaid = bool.Parse(Request.Form["IsPaid"]);       //是否银行自动扣款
                    applyItems = Request.Form["Items"].JsonTo<List<ChargeApplyItem>>();

                    payeeAccount = Erp.Current.Finance.Accounts.FirstOrDefault(item => item.ID == payeeAccountID);
                    if (payeeAccount == null)
                    {
                        throw new Exception("收款人不存在!");
                    }

                    if (!isPaid)
                    {
                        payerAccountID = null;
                    }
                    else
                    {
                        var payerAccount = Erp.Current.Finance.Accounts.FirstOrDefault(item => item.ID == payerAccountID);
                        payerID = payerAccount.EnterpriseID;
                        currency = payerAccount.Currency;
                    }

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        //申请
                        apply = new Services.Models.Origins.ChargeApply()
                        {
                            Currency = currency,
                            ApplierID = Request.Form["ApplierID"],
                            ApproverID = Request.Form["ApproverID"],
                            CreatorID = Erp.Current.ID,
                            IsPaid = isPaid,
                            Price = applyItems.Sum(c => c?.Price) ?? 0,
                            Status = ApplyStauts.Waiting,
                            Summary = Request.Form["Summary"],
                            PayeeAccountID = payeeAccount?.ID,
                            PayerAccountID = payerAccountID,
                            SenderID = SystemSender.Center.GetFixedID(),
                            Type = CostApplyType.Normal,        //默认正常
                            IsImmediately = false,      //默认false
                            PayerID = payerID,
                        };
                        apply.Enter();

                        //资金申请项
                        for (int i = 0; i < applyItems.Count; i++)
                        {
                            applyItems[i].Status = ApplyItemStauts.Paying;
                        }
                    }
                    else
                    {
                        //申请
                        apply = Erp.Current.Finance.ChargeApplies[id];
                        apply.Currency = currency;
                        apply.ApplierID = Request.Form["ApplierID"];
                        apply.ApproverID = Request.Form["ApproverID"];
                        apply.CreatorID = Erp.Current.ID;
                        apply.IsPaid = bool.Parse(Request.Form["IsPaid"]);
                        apply.Status = ApplyStauts.Waiting;
                        apply.Summary = Request.Form["Summary"] ?? null;
                        apply.PayeeAccountID = payeeAccount?.ID;
                        apply.PayerAccountID = payerAccountID;
                        apply.SenderID = SystemSender.Center.GetFixedID();
                        apply.Price = applyItems.Sum(c => c?.Price) ?? 0;
                        apply.PayerID = payerID;
                        apply.Enter();
                    }

                    //已收款，添加核销记录、流水表
                    if (isPaid)
                    {
                        flows = new List<FlowAccount>();
                        string flowId_temp = string.Empty;

                        if (!string.IsNullOrEmpty(Request.Form["FormCode"]) && flowsView.Any(item => item.FormCode == Request.Form["FormCode"]))
                        {
                            json.success = false;
                            json.data = "流水号已存在，不能重复录入!";
                            return json;
                        }


                        //资金申请项
                        for (int i = 0; i < applyItems.Count; i++)
                        {
                            flowId_temp = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);

                            new FlowAccount()
                            {
                                ID = flowId_temp,
                                Currency = apply.Currency,
                                CreateDate = DateTime.Now,
                                AccountID = apply.PayerAccountID,
                                AccountMethord = AccountMethord.Payment,
                                Balance = 0,        //余额    触发器计算
                                Balance1 = 0,       //人民币 余额    触发器计算
                                CreatorID = Erp.Current.ID,
                                Currency1 = Currency.CNY,
                                ERate1 = rate,
                                FormCode = Request.Form["FormCode"],
                                TargetAccountCode = payeeAccount?.Code,
                                TargetAccountName = payeeAccount?.Name,
                                PaymentDate = paymentDate,
                                Price = -applyItems[i].Price.Value,
                                Price1 = -(applyItems[i].Price.Value * rate).Round(),
                                PaymentMethord = (PaymentMethord)int.Parse(Request.Form["PaymentMethord"]),
                            }.Enter();

                            applyItems[i].Status = ApplyItemStauts.Paid;
                            applyItems[i].FlowID = flowId_temp;
                            applyItems[i].IsPaid = bool.Parse(Request.Form["IsPaid"]);
                        }
                    }

                    //费用申请项
                    Erp.Current.Finance.ChargeApplyItems.InsertOrUpdate(apply.ID, applyItems);

                    //附件
                    FilesEnter(apply.ID);

                    //日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.CostApply, Services.Enums.ApprovalStatus.Submit, apply.ID, Erp.Current.ID);

                    tran.Commit();

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用申请, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(id) ? "新增" : "修改", new { apply, applyItems, flows }.Json());
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    json.success = false;
                    json.data = "添加失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用申请, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(id) ? "新增" : "修改") + " 异常!", new { apply, applyItems, flows, exception = ex.ToString() }.Json());
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
                .SearchByFilesMapValue(FilesMapName.ChargeApplyID.ToString(), Request.QueryString["id"]);
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
        /// 加载申请项
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var id = Request.QueryString["id"];
            var view = Erp.Current.Finance.ChargeApplyItems.Where(item => item.ApplyID == id);

            return view.ToArray().Select(item => new
            {
                item.ID,
                item.AccountCatalogID,
                item.Price,
                Status = (int)item.Status,
                item.IsPaid,
            });
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
            var fileIds = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.ChargeApplyID.ToString(), applyId).Select(item => item.ID).ToArray();

            //前端获取的附件数据
            string fileData = Request.Form["files"].Replace("&quot;", "'");
            List<FilesDescription> files = fileData.JsonTo<List<FilesDescription>>();

            //非新增附件
            var fileIds_exist = files.Where(item => !string.IsNullOrEmpty(item.ID)).Select(item => item.ID).ToArray();

            for (int i = 0; i < files.Count(); i++)
            {
                files[i].Type = Services.Enums.FileDescType.ChargeApply;
                files[i].CreatorID = Erp.Current.ID;

                List<FilesMap> filesMaps = new List<FilesMap>();
                filesMaps.Add(new FilesMap()
                {
                    Name = FilesMapName.ChargeApplyID.ToString(),
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