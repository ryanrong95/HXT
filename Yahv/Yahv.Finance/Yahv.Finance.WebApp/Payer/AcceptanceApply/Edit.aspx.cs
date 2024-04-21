using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using FilesDescription = Yahv.Finance.Services.Models.Origins.FilesDescription;
using FilesMap = Yahv.Finance.Services.Models.Origins.FilesMap;

namespace Yahv.Finance.WebApp.Payer.AcceptanceApply
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


            var accounts = Erp.Current.Finance.Accounts.Where(item => item.Currency == Currency.CNY && item.Status == GeneralStatus.Normal && item.EnterpriseID != null && item.NatureType == NatureType.Public);
            if (!Erp.Current.IsSuper)
            {
                accounts = accounts.Where(item => item.OwnerID == Erp.Current.ID);
            }
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
            var moAccounts = Erp.Current.Finance.Accounts.GetMoneyOrderAccounts();
            if (!Erp.Current.IsSuper)
            {
                moAccounts = moAccounts.Where(item => item.OwnerID == Erp.Current.ID);
            }
            this.Model.PayerAccounts = moAccounts.ToArray()
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

            //类型
            this.Model.AcceptanceTypes = ExtendsEnum.ToDictionary<AcceptanceType>().Select(item => new { text = item.Value, value = item.Key });

            //汇票
            this.Model.MoneyOrders = new MoneyOrdersRoll()
                .GetIEnumerable(item => item.Status == MoneyOrderStatus.Ticketed
                                        && item.CreatorID == Erp.Current.ID).Select(item => new
                                        {
                                            item.ID,
                                            item.Name,
                                            item.Code,
                                            item.Price,
                                            item.PayerAccountName,
                                            item.PayeeAccountID,
                                        }).ToArray();
        }
        #endregion

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };

            string id = Request.QueryString["id"];
            Services.Models.Origins.AcceptanceApply apply = null;
            AcceptanceLeft payerLeft = null;
            AcceptanceLeft payeeLeft = null;

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                try
                {

                    var payeeAccountID = Request.Form["PayeeAccountID"];        //收款账户
                    var payerAccountID = Request.Form["PayerAccountID"];        //付款账户
                    var payerPrice = decimal.Parse(Request.Form["PayerPrice"]);     //调出金额
                    //var payeePrice = decimal.Parse(Request.Form["PayeePrice"]);       //调入金额
                    var type = Request.Form["Type"];
                    var moneyOrderID = Request.Form["MoneyOrderID"];
                    var currency = Currency.CNY;

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        //申请
                        apply = new Services.Models.Origins.AcceptanceApply()
                        {
                            ApplierID = Request.Form["ApplierID"],
                            ApproverID = Request.Form["ApproverID"],
                            CreatorID = Erp.Current.ID,
                            Currency = currency,
                            MoneyOrderID = moneyOrderID,
                            PayeeAccountID = payeeAccountID,
                            PayerAccountID = payerAccountID,
                            Price = payerPrice,
                            Summary = Request.Form["Summary"] ?? null,
                            Status = ApplyStauts.Waiting,
                            Type = (AcceptanceType)int.Parse(type),
                            SenderID = SystemSender.Center.GetFixedID(),
                        };
                        apply.Enter();

                        //调出
                        payerLeft = new AcceptanceLeft()
                        {
                            AccountMethord = AccountMethord.Output,
                            ApplyID = apply.ID,
                            CreatorID = Erp.Current.ID,
                            Currency = currency,
                            PayeeAccountID = payeeAccountID,
                            PayerAccountID = payerAccountID,
                            Price = payerPrice,
                            Status = GeneralStatus.Normal,
                        };
                        payerLeft.Enter();

                        //调入
                        payeeLeft = new AcceptanceLeft()
                        {
                            AccountMethord = AccountMethord.Input,
                            ApplyID = apply.ID,
                            CreatorID = Erp.Current.ID,
                            Currency = currency,
                            PayeeAccountID = payeeAccountID,
                            PayerAccountID = payerAccountID,
                            Price = payerPrice,
                            Status = GeneralStatus.Normal,
                        };
                        payeeLeft.Enter();
                    }
                    //else
                    //{
                    //    //申请
                    //    apply = Erp.Current.Finance.SelfAppliesView[id];
                    //    apply.Currency = payerCurrency;
                    //    apply.ApplierID = Request.Form["ApplierID"];
                    //    apply.ApproverID = Request.Form["ApproverID"];
                    //    apply.CreatorID = Erp.Current.ID;
                    //    apply.Price = payerPrice;
                    //    apply.Status = ApplyStauts.Waiting;
                    //    apply.Summary = Request.Form["Summary"] ?? null;
                    //    apply.PayeeAccountID = payeeAccountID;
                    //    apply.PayerAccountID = payerAccountID;
                    //    apply.SenderID = SystemSender.Center.GetFixedID();
                    //    apply.TargetCurrency = payeeCurrency;
                    //    apply.TargetERate = erate;
                    //    apply.TargetPrice = payeePrice;

                    //    //调出
                    //    payerLeft = lefts.FirstOrDefault(item => item.ApplyID == id && item.AccountMethord == AccountMethord.Output);
                    //    if (payerLeft == null)
                    //        throw new Exception("未找到调出信息!");
                    //    payerLeft.PayeeAccountID = payeeAccountID;
                    //    payerLeft.PayerAccountID = payerAccountID;
                    //    payerLeft.Currency = payerCurrency;
                    //    payerLeft.ApplyID = apply.ID;
                    //    payerLeft.CreatorID = Erp.Current.ID;
                    //    payerLeft.Currency1 = Currency.CNY;
                    //    payerLeft.ERate1 = payerRate;
                    //    payerLeft.Price1 = (payerPrice * payerRate).Round();
                    //    payerLeft.Price = payerPrice;
                    //    payerLeft.Status = GeneralStatus.Normal;
                    //    payerLeft.AccountCatalogID = Request.Form["AccountCatalogID"];

                    //    //调入
                    //    payeeLeft = lefts.FirstOrDefault(item => item.ApplyID == id && item.AccountMethord == AccountMethord.Input);
                    //    if (payeeLeft == null)
                    //        throw new Exception("未找到调入信息!");
                    //    payeeLeft.AccountMethord = AccountMethord.Input;
                    //    payeeLeft.CreateDate = DateTime.Now;
                    //    payeeLeft.Currency = payeeCurrency;
                    //    payeeLeft.AccountCatalogID = accountCatalogID;
                    //    payeeLeft.ApplyID = apply.ID;
                    //    payeeLeft.CreatorID = Erp.Current.ID;
                    //    payeeLeft.Currency1 = Currency.CNY;
                    //    payeeLeft.ERate1 = payeeRate;
                    //    payeeLeft.PayeeAccountID = payeeAccountID;
                    //    payeeLeft.PayerAccountID = payerAccountID;
                    //    payeeLeft.Price = payeePrice;
                    //    payeeLeft.Price1 = (payeePrice * payeeRate).Round();
                    //    payeeLeft.Status = GeneralStatus.Normal;


                    //    apply.Enter();
                    //    payerLeft.Enter();
                    //    payeeLeft.Enter();
                    //}

                    //附件
                    FilesEnter(apply?.ID);
                    //日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.FundTransfer, Services.Enums.ApprovalStatus.Submit, apply?.ID, Erp.Current.ID);
                    tran.Commit();

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑调拨申请, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(id) ? "新增" : "修改", new { selfApply = apply, payerLeft, payeeLeft }.Json());
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    json.success = false;
                    json.data = "添加失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑调拨申请, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(id) ? "新增" : "修改") + " 异常!", new { selfApply = apply, payerLeft, payeeLeft, exception = ex.ToString() }.Json());
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
            var fileIds = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.AcceptanceApplyID.ToString(), applyId).Select(item => item.ID).ToArray();

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
                    Name = FilesMapName.AcceptanceApplyID.ToString(),
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