using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using ApprovalStatus = Yahv.Underly.ApprovalStatus;

namespace Yahv.Finance.WebApp.Payer.FundTransfer
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
                var data = Erp.Current.Finance.SelfAppliesView[id];

                using (var view = new SelfLeftsRoll())
                using (var accountsView = new AccountsRoll())
                {
                    this.Model.Data = new
                    {
                        PayeeAccountID = data.PayeeAccount?.ShortName ?? data.PayeeAccount?.Name,
                        PayeeCurrency = data.PayeeAccount?.Currency.GetDescription(),
                        PayeeCode = data.PayeeAccount?.Code,
                        PayeeBank = data.PayeeAccount?.BankName,
                        PayerAccountID = data.PayerAccount?.ShortName ?? data.PayerAccount?.Name,
                        PayerCurrency = data.PayerAccount?.Currency.GetDescription(),
                        PayerCode = data.PayerAccount?.Code,
                        PayerBank = data.PayerAccount?.BankName,
                        AccountCatalogID = view.FirstOrDefault(item => item.ApplyID == id)?.AccountCatalogID,
                        ApplierID = data.Applier?.RealName,
                        ApproverID = data.Approver?.RealName,
                        Summary = data.Summary,
                        PayerPrice = data.Price,
                        PayeePrice = data.TargetPrice,
                        Rate = data.TargetERate,
                        PaymentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        ExchangeDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        IsAcceptanceFromPayee = accountsView.IsAcceptanceAccount(data?.PayeeAccount?.ID),  //调入账户是否为承兑户
                    };

                    //调拨类型
                    this.Model.AccountCatalogs = ExtendsEnum.ToDictionary<FundTransferType>().Select(item => new { text = item.Value, value = item.Key });
                    //付款方式
                    this.Model.PaymentMethord = ExtendsEnum.ToDictionary<PaymentMethord>().Select(item => new { value = item.Key, text = item.Value });
                    //银行承兑账户
                    this.Model.MosBank = GetMoneyOrdersByType(PaymentMethord.BankAcceptanceBill, data?.PayerAccount?.ID);
                    //商业承兑账户
                    this.Model.MosCommercial = GetMoneyOrdersByType(PaymentMethord.CommercialAcceptanceBill, data?.PayerAccount?.ID);
                }
            }
        }
        #endregion

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "支付成功!" };

            string id = Request.QueryString["id"];
            FlowAccount payerFlow = null;       //调出流水
            SelfRight payerRight = null;        //调出核销
            FlowAccount payeeFlow = null;       //调入流水
            SelfRight payeeRight = null;        //调入核销
            MoneyOrder moneyOrder = null;       //承兑汇票
            Endorsement endorsement = null;     //背书转让

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var leftView = new SelfLeftsRoll(reponsitory))
            using (tran = reponsitory.OpenTransaction())
            using (var flowsView = new FlowAccountsRoll(reponsitory))
            using (var accountsView = new AccountsRoll(reponsitory))
            using (var moneyOrderView = new MoneyOrdersRoll(reponsitory))
            {
                try
                {
                    var entity = Erp.Current.Finance.SelfAppliesView[id];
                    if (entity == null)
                    {
                        json.success = false;
                        json.data = "未找到该申请信息!";
                        return json;
                    }

                    //if (Erp.Current.Finance.FlowAccounts.CalculateBalance(entity.PayerAccountID, -entity.Price) < 0)
                    //{
                    //    throw new Exception("余额不足!");
                    //}

                    //修改申请状态
                    entity.Status = ApplyStauts.Completed;


                    var rate = ExchangeRates.Universal[entity.Currency, entity.TargetCurrency];     //调出对调入币种的汇率
                    var payerRate = ExchangeRates.Universal[entity.Currency, Currency.CNY];     //调出币种的本位币汇率
                    var payeeRate = ExchangeRates.Universal[entity.TargetCurrency, Currency.CNY];   //调入币种的本位币汇率

                    var outFormCode = Request.Form["OutFormCode"];
                    var inFormCode = Request.Form["InFormCode"];
                    var paymentMethord = (PaymentMethord)int.Parse(Request.Form["PaymentMethord"]);     //支付方式
                    var faType = FlowAccountType.BankStatement;     //账户管理类型
                    string moneyOrderId = Request.Form["MoneyOrderID"];
                    var operation = Operation.Common;

                    #region 调出
                    var payerLeft = leftView.FirstOrDefault(item => item.ApplyID == id && item.AccountMethord == AccountMethord.Output);
                    if (payerLeft == null)
                    {
                        throw new Exception("未能找到应调出信息!");
                    }

                    if (!string.IsNullOrEmpty(outFormCode) && flowsView.Any(item => item.FormCode == outFormCode))
                    {
                        json.success = false;
                        json.data = "调出流水号已存在，不能重复录入!";
                        return json;
                    }

                    //如果付款方式是承兑汇票 修改 管理账户类型
                    if (paymentMethord == PaymentMethord.BankAcceptanceBill
                        || paymentMethord == PaymentMethord.CommercialAcceptanceBill)
                    {
                        moneyOrder = moneyOrderView[moneyOrderId];
                        faType = FlowAccountType.MoneyOrder;
                        moneyOrder.Status = MoneyOrderStatus.Ticketed;
                        moneyOrder.ModifierID = Erp.Current.ID;
                        moneyOrder.ModifyDate = DateTime.Now;

                        //背书转让（调入账户为承兑账户）
                        if (accountsView.IsAcceptanceAccount(entity.PayeeAccountID))
                        {
                            moneyOrder.PayeeAccountID = entity.PayeeAccountID;

                            operation = Operation.Endorsement;
                            //添加背书转让
                            endorsement = new Endorsement()
                            {
                                CreateDate = DateTime.Now,
                                CreatorID = Erp.Current.ID,
                                EndorseDate = DateTime.Parse(Request.Form["EndorseDate"]),
                                IsTransfer = Request.Form["IsTransfer"] == "1",
                                MoneyOrderID = moneyOrder.ID,
                                PayerAccountID = entity.PayerAccountID,
                                PayeeAccountID = entity.PayeeAccountID,
                            };
                            //背书转让
                            endorsement.Enter();
                        }
                        //贴现
                        else
                        {
                            operation = Operation.Acceptance;
                            moneyOrder.Status = MoneyOrderStatus.Exchanged;

                            moneyOrder.ExchangeDate = DateTime.Parse(Request.Form["ExchangeDate"]);
                            moneyOrder.ExchangePrice = entity.Price - (entity.Price - entity.TargetPrice);

                            if (!moneyOrder.IsMoney)
                            {
                                throw new Exception("该承兑汇票不能贴现!");
                            }
                        }
                    }


                    //调出流水
                    payerFlow = new FlowAccount()
                    {
                        ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                        Currency = entity.PayerAccount.Currency,
                        CreateDate = DateTime.Now,
                        AccountID = entity.PayerAccountID,
                        AccountMethord = AccountMethord.Output,
                        CreatorID = Erp.Current.ID,
                        Currency1 = Currency.CNY,
                        ERate1 = payerRate,
                        FormCode = outFormCode,
                        TargetAccountCode = entity.PayeeAccount.Code,
                        TargetAccountName = entity.PayeeAccount.Name,
                        PaymentDate = DateTime.Parse(Request.Form["PaymentDate"]),
                        Price = -entity.Price,
                        Price1 = -(entity.Price * payerRate).Round(),
                        PaymentMethord = paymentMethord,
                        Type = faType,
                        MoneyOrderID = moneyOrderId ?? null,
                    };

                    //调出核销
                    payerRight = new SelfRight()
                    {
                        SelfLeftID = payerLeft.ID,
                        FlowID = payerFlow.ID,
                        CreateDate = DateTime.Now,
                        CreatorID = Erp.Current.ID,
                        OriginCurrency = entity.Currency,
                        OriginPrice = entity.Price,
                        Rate = rate,
                        TargetCurrency = entity.TargetCurrency,
                        TargetPrice = entity.TargetPrice,
                        OriginCurrency1 = Currency.CNY,
                        OriginERate1 = payerRate,
                        OriginPrice1 = (entity.Price * payerRate).Round(),
                        TargetCurrency1 = Currency.CNY,
                        TargetERate1 = payeeRate,
                        TargetPrice1 = (entity.TargetPrice * payeeRate).Round(),
                    };

                    #endregion

                    #region 调入
                    var payeeLeft = leftView.FirstOrDefault(item => item.ApplyID == id && item.AccountMethord == AccountMethord.Input);
                    if (payeeLeft == null)
                    {
                        throw new Exception("未能找到应调入信息!");
                    }
                    if (!string.IsNullOrEmpty(inFormCode) && flowsView.Any(item => item.FormCode == inFormCode))
                    {
                        json.success = false;
                        json.data = "调入流水号已存在，不能重复录入!";
                        return json;
                    }

                    //如果付款方式是承兑汇票并且调入账户为承兑户 修改 管理账户类型
                    if ((paymentMethord == PaymentMethord.BankAcceptanceBill
                        || paymentMethord == PaymentMethord.CommercialAcceptanceBill)
                        && accountsView.IsAcceptanceAccount(entity.PayeeAccountID))
                    {
                        faType = FlowAccountType.MoneyOrder;
                    }
                    else
                    {
                        faType = FlowAccountType.BankStatement;
                    }

                    //调入流水
                    payeeFlow = new FlowAccount()
                    {
                        ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                        Currency = entity.PayeeAccount.Currency,
                        CreateDate = DateTime.Now,
                        AccountID = entity.PayeeAccountID,
                        AccountMethord = AccountMethord.Input,
                        CreatorID = Erp.Current.ID,
                        Currency1 = Currency.CNY,
                        ERate1 = payeeRate,
                        FormCode = inFormCode,
                        TargetAccountCode = entity.PayerAccount.Code,
                        TargetAccountName = entity.PayerAccount.Name,
                        PaymentDate = DateTime.Parse(Request.Form["PaymentDate"]),
                        Price = entity.TargetPrice,
                        Price1 = (entity.TargetPrice * payeeRate).Round(),
                        PaymentMethord = paymentMethord,
                        Type = faType,
                        MoneyOrderID = moneyOrderId ?? null,
                    };

                    //调入核销
                    payeeRight = new SelfRight()
                    {
                        CreateDate = DateTime.Now,
                        CreatorID = Erp.Current.ID,
                        FlowID = payeeFlow.ID,
                        OriginCurrency = entity.TargetCurrency,
                        OriginCurrency1 = Currency.CNY,
                        OriginERate1 = payeeRate,
                        OriginPrice = entity.TargetPrice,
                        OriginPrice1 = (entity.TargetPrice * payeeRate).Round(),
                        Rate = 1,       //调入核销 源币种和目标币种一致
                        SelfLeftID = payeeLeft.ID,
                        TargetCurrency = entity.TargetCurrency,
                        TargetCurrency1 = Currency.CNY,
                        TargetERate1 = payeeRate,
                        TargetPrice = entity.TargetPrice,
                        TargetPrice1 = (entity.TargetPrice * payeeRate).Round(),
                    };
                    #endregion

                    //调出
                    payerFlow.Add();
                    payerRight.Enter();

                    //调入
                    payeeFlow.Add();
                    payeeRight.Enter();

                    //承兑汇票
                    moneyOrder?.Enter();

                    FilesEnter(id, operation, moneyOrder?.ID, endorsement?.ID);     //附件
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.FundTransfer, Services.Enums.ApprovalStatus.Payment, id, Erp.Current.ID, Request.Form["Comments"]);        //添加审批日志
                    //申请
                    entity.Success += Entity_Success;
                    entity.Enter();

                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.资金调拨申请, Services.Oplogs.GetMethodInfo(), "支付", new { payerRight, payerFlow, payeeRight, payeeFlow }.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }
                    json.success = false;
                    json.data = "支付失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.资金调拨申请, Services.Oplogs.GetMethodInfo(), "支付 异常!", new { payerRight, payerFlow, payeeRight, payeeFlow, exception = ex.ToString() }.Json());
                    return json;
                }
            }

            return json;
        }

        private void Entity_Success(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as SelfApply;
            var result = string.Empty;
            var url = XdtApi.FundTransfer.GetApiAddress();
            InputParam<FundTransferInputDto> data = null;
            try
            {
                if (tran?.Connection != null)
                {
                    tran.Commit();
                }

                using (var selfLeftsView = new SelfLeftsRoll())
                using (var selfRightsView = new SelfRightsRoll())
                using (var flowsView = new FlowAccountsRoll())
                using (var accountsView = new AccountsRoll())
                {
                    var outLeft = selfLeftsView.FirstOrDefault(item => item.ApplyID == entity.ID && item.AccountMethord == AccountMethord.Output);
                    var inLeft = selfLeftsView.FirstOrDefault(item => item.ApplyID == entity.ID && item.AccountMethord == AccountMethord.Input);

                    var outRight = selfRightsView.FirstOrDefault(item => item.SelfLeftID == outLeft.ID);
                    var inRight = selfRightsView.FirstOrDefault(item => item.SelfLeftID == inLeft.ID);

                    var outFlow = flowsView.FirstOrDefault(item => item.ID == outRight.FlowID);
                    var inFlow = flowsView.FirstOrDefault(item => item.ID == inRight.FlowID);

                    if (outFlow == null || string.IsNullOrEmpty(outFlow.ID) || inFlow == null || string.IsNullOrEmpty(inFlow.ID))
                    {
                        Services.Oplogs.Oplog(Erp.Current.ID, LogModular.调拨接口_Api, Services.Oplogs.GetMethodInfo(), "Api 异常", $"未找到流水!{entity.Json()}", url: url);
                        return;
                    }
                    else
                    {
                        var payeeAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayeeAccountID);
                        var payerAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayerAccountID);
                        var erate = ExchangeRates.Universal[outFlow.Currency, inFlow.Currency];

                        //调出账户 如果不是芯达通不需要互通
                        if (ConfigurationManager.AppSettings["XdtCompanyName"] != null && !ConfigurationManager
                            .AppSettings["XdtCompanyName"].Contains($",{payerAccount?.Enterprise?.Name},"))
                        {
                            return;
                        }

                        data = new InputParam<FundTransferInputDto>()
                        {
                            Sender = SystemSender.Center.GetFixedID(),
                            Option = OptionConsts.insert,
                            Model = new FundTransferInputDto()
                            {
                                CreatorID = entity.ApplierID,
                                Rate = erate,
                                FeeType = outLeft.AccountCatalogID,
                                PaymentType = (int)outFlow.PaymentMethord,
                                PaymentDate = DateTime.Parse(outFlow.PaymentDate.ToString()),

                                OutAmount = Math.Abs(outFlow.Price),
                                OutCurrency = outFlow.Currency.GetCurrency().ShortName,
                                OutSeqNo = outFlow.FormCode,
                                OutAccountNo = payerAccount.Code,

                                InAmount = inFlow.Price,
                                InCurrency = inFlow.Currency.GetCurrency().ShortName,
                                InSeqNo = inFlow.FormCode,
                                InAccountNo = payeeAccount.Code,

                            }
                        };
                    };
                    result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.调拨接口_Api, Services.Oplogs.GetMethodInfo(), "向芯达通互通", result + data.Json(), url: url);
                }
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.调拨接口_Api, Services.Oplogs.GetMethodInfo(), "向芯达通互通异常!" + ex.Message, result + data.Json(), url: url);
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
            var files = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.SelfApplyID.ToString(), Request.QueryString["id"]);
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
                .OrderByDescending(item => item.CreateDate).ToArray()
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
        private void FilesEnter(string applyId, Operation operation, string moId, string endorsId)
        {
            var fileIds = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.SelfApplyID.ToString(), applyId).Select(item => item.ID).ToArray();

            //前端获取的附件数据
            string fileData = Request.Form["files"].Replace("&quot;", "'");
            List<FilesDescription> files = fileData.JsonTo<List<FilesDescription>>();
            List<FilesDescription> filesSync = fileData.JsonTo<List<FilesDescription>>();

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

            //承兑贴现
            if (operation == Operation.Acceptance)
            {
                for (int i = 0; i < filesSync.Count(); i++)
                {
                    filesSync[i].Type = Services.Enums.FileDescType.MoneyOrder;
                    filesSync[i].CreatorID = Erp.Current.ID;

                    List<FilesMap> filesMaps = new List<FilesMap>();
                    filesMaps.Add(new FilesMap()
                    {
                        Name = FilesMapName.MoneyOrderID.ToString(),
                        Value = moId,
                    });
                    filesSync[i].FilesMapsArray = filesMaps.ToArray();
                }

                files = files.Concat(filesSync).ToList();
            }

            //背书转让
            if (operation == Operation.Endorsement)
            {
                for (int i = 0; i < filesSync.Count(); i++)
                {
                    filesSync[i].Type = Services.Enums.FileDescType.Endorsement;
                    filesSync[i].CreatorID = Erp.Current.ID;

                    List<FilesMap> filesMaps = new List<FilesMap>();
                    filesMaps.Add(new FilesMap()
                    {
                        Name = FilesMapName.EndorsementID.ToString(),
                        Value = endorsId,
                    });
                    filesSync[i].FilesMapsArray = filesMaps.ToArray();
                }

                files = files.Concat(filesSync).ToList();
            }

            //删除不存在的附件
            var ids = fileIds.Where(item => !fileIds_exist.Contains(item)).ToArray();

            Erp.Current.Finance.FilesDescriptionView.Add(files.Where(item => string.IsNullOrEmpty(item.ID)).ToArray());
            Erp.Current.Finance.FilesDescriptionView.Delete(ids);
        }

        /// <summary>
        /// 获取未签收汇票
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private object GetMoneyOrdersByType(PaymentMethord type, string payeeAccountID)
        {
            using (var view = new MoneyOrdersRoll())
            {
                var list = new List<MoneyOrderDto>();
                Expression<Func<MoneyOrder, bool>> predicate = item => item.Status == MoneyOrderStatus.Ticketed
                                                                       && item.PayeeAccountID == payeeAccountID;

                //如果不是管理员只显示自己的承兑汇票
                if (!Erp.Current.IsSuper)
                {
                    predicate = predicate.And(item => item.CreatorID == Erp.Current.ID);
                }

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
        #endregion

        #region 枚举
        /// <summary>
        /// 操作
        /// </summary>
        enum Operation
        {
            /// <summary>
            /// 普通
            /// </summary>
            Common,
            /// <summary>
            /// 承兑
            /// </summary>
            Acceptance,
            /// <summary>
            /// 背书转让
            /// </summary>
            Endorsement
        }
        #endregion
    }
}