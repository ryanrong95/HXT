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
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.AcceptanceApply
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
                using (var view = new AcceptanceAppliesRoll())
                using (var moneyOrdersView = new MoneyOrdersRoll())
                {
                    var data = view.Find(id);

                    var leftOut = view.GetAcceptanceLeft(id, AccountMethord.Output);
                    var leftIn = view.GetAcceptanceLeft(id, AccountMethord.Input);
                    var moneyOrder = moneyOrdersView.Find(data.MoneyOrderID);

                    this.Model.Data = new
                    {
                        PayeeAccountID = data.PayeeAccountID,
                        PayeeCode = data.PayeeCode,
                        PayeeBank = data.PayeeBank,
                        PayerAccountName = data.PayerAccountName,

                        data.PayeeAccountName,
                        PayerAccountID = data.PayerAccountID,
                        PayerCode = data.PayerCode,
                        PayerBank = data.PayerBank,

                        ApplierID = data.ApplierName,
                        ApproverID = data.ApproverName,
                        Summary = data.Summary,
                        PayerPrice = leftOut.Price,
                        PayeePrice = leftIn.Price,
                        data.ExcuterID,
                        data.TypeName,
                        MoneyOrderID = moneyOrder?.Code,
                        data.ApproverName,
                        data.Type,
                        PaymentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        ExchangeDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    };

                    //付款方式
                    var acceptances = new string[]
                    {
                        ((int)PaymentMethord.BankAcceptanceBill).ToString()
                        ,((int)PaymentMethord.CommercialAcceptanceBill).ToString()
                    };

                    this.Model.PaymentMethord = ExtendsEnum.ToDictionary<PaymentMethord>()
                        .Where(item => acceptances.Contains(item.Key))
                        .Select(item => new
                        {
                            value = item.Key,
                            text = item.Value,
                            selected = (moneyOrder.Type == MoneyOrderType.Bank) && item.Key == ((int)PaymentMethord.BankAcceptanceBill).ToString() ? "selected" : ""
                        });
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
            AcceptanceRight payerRight = null;        //调出核销
            FlowAccount payeeFlow = null;       //调入流水
            AcceptanceRight payeeRight = null;        //调入核销
            MoneyOrder moneyOrder = null;       //承兑汇票
            Endorsement endorsement = null;     //背书转让

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var appliesView = new AcceptanceAppliesRoll(reponsitory))
            using (tran = reponsitory.OpenTransaction())
            using (var flowsView = new FlowAccountsRoll(reponsitory))
            using (var moneyOrderView = new MoneyOrdersRoll(reponsitory))
            {
                try
                {
                    var entity = appliesView[id];
                    var entityDto = appliesView.Find(id);
                    if (entity == null || entityDto == null)
                    {
                        json.success = false;
                        json.data = "未找到该申请信息!";
                        return json;
                    }

                    //修改申请状态
                    entity.Status = ApplyStauts.Completed;

                    var outFormCode = Request.Form["OutFormCode"];
                    var inFormCode = Request.Form["InFormCode"];
                    var paymentMethord = (PaymentMethord)int.Parse(Request.Form["PaymentMethord"]);     //支付方式
                    var faType = FlowAccountType.MoneyOrder;     //账户管理类型
                    string moneyOrderId = entity.MoneyOrderID;
                    var inPrice = Request.Form["PayeePrice"];
                    var currency = Currency.CNY;
                    var rate = 1;

                    #region 调出
                    var payerLeft = appliesView.GetAcceptanceLeft(id, AccountMethord.Output);
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

                    moneyOrder = moneyOrderView[moneyOrderId];
                    faType = FlowAccountType.MoneyOrder;
                    moneyOrder.Status = MoneyOrderStatus.Ticketed;
                    moneyOrder.ModifierID = Erp.Current.ID;
                    moneyOrder.ModifyDate = DateTime.Now;

                    //背书
                    if (entity.Type == AcceptanceType.Endorsement)
                    {
                        moneyOrder.PayeeAccountID = entity.PayeeAccountID;
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
                    else
                    {
                        moneyOrder.Status = MoneyOrderStatus.Exchanged;

                        moneyOrder.ExchangeDate = DateTime.Parse(Request.Form["ExchangeDate"]);
                        moneyOrder.ExchangePrice = entity.Price - (entity.Price - decimal.Parse(inPrice));

                        if (!moneyOrder.IsMoney)
                        {
                            throw new Exception("该承兑汇票不能贴现!");
                        }
                    }


                    //调出流水
                    payerFlow = new FlowAccount()
                    {
                        ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                        Currency = currency,
                        CreateDate = DateTime.Now,
                        AccountID = entity.PayerAccountID,
                        AccountMethord = AccountMethord.Output,
                        CreatorID = Erp.Current.ID,
                        Currency1 = Currency.CNY,
                        ERate1 = rate,
                        FormCode = outFormCode,
                        TargetAccountCode = entityDto.PayeeCode,
                        TargetAccountName = entityDto.PayeeAccountName,
                        PaymentDate = DateTime.Parse(Request.Form["PaymentDate"]),
                        Price = -entity.Price,
                        Price1 = -(entity.Price * rate).Round(),
                        PaymentMethord = paymentMethord,
                        Type = faType,
                        MoneyOrderID = moneyOrderId ?? null,
                    };

                    //调出核销
                    payerRight = new AcceptanceRight()
                    {
                        CreatorID = Erp.Current.ID,
                        AcceptanceLeftID = payerLeft.ID,
                        FlowID = payerFlow.ID,
                        Price = entity.Price,
                    };

                    #endregion

                    #region 调入
                    var payeeLeft = appliesView.GetAcceptanceLeft(id, AccountMethord.Input);
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

                    //调入流水
                    payeeFlow = new FlowAccount()
                    {
                        ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                        Currency = entity.Currency,
                        CreateDate = DateTime.Now,
                        AccountID = entity.PayeeAccountID,
                        AccountMethord = AccountMethord.Input,
                        CreatorID = Erp.Current.ID,
                        Currency1 = Currency.CNY,
                        ERate1 = rate,
                        FormCode = inFormCode,
                        TargetAccountCode = entityDto.PayerCode,
                        TargetAccountName = entityDto.PayerAccountName,
                        PaymentDate = DateTime.Parse(Request.Form["PaymentDate"]),
                        Price = decimal.Parse(inPrice),
                        Price1 = (decimal.Parse(inPrice) * rate).Round(),
                        PaymentMethord = paymentMethord,
                        Type = faType,
                        MoneyOrderID = moneyOrderId ?? null,
                    };

                    //调入核销
                    payeeRight = new AcceptanceRight()
                    {
                        AcceptanceLeftID = payeeLeft.ID,
                        CreatorID = Erp.Current.ID,
                        FlowID = payeeFlow.ID,
                        Price = decimal.Parse(inPrice),
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

                    FilesEnter(id, entity.Type, moneyOrder?.ID, endorsement?.ID);     //附件
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.FundTransfer, Services.Enums.ApprovalStatus.Payment, id, Erp.Current.ID, Request.Form["Comments"]);        //添加审批日志
                    //申请
                    entity.UpdateSuccess += Entity_UpdateSuccess;
                    entity.Enter();

                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑调拨申请, Services.Oplogs.GetMethodInfo(), "支付", new { payerRight, payerFlow, payeeRight, payeeFlow }.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }
                    json.success = false;
                    json.data = "支付失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑调拨申请, Services.Oplogs.GetMethodInfo(), "支付 异常!", new { payerRight, payerFlow, payeeRight, payeeFlow, exception = ex.ToString() }.Json());
                    return json;
                }
            }

            return json;
        }

        private void Entity_UpdateSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Services.Models.Origins.AcceptanceApply;
            var result = string.Empty;
            var url = XdtApi.FundTransfer.GetApiAddress();
            InputParam<FundTransferInputDto> data = null;
            try
            {
                if (tran?.Connection != null)
                {
                    tran.Commit();
                }

                using (var leftsView = new AcceptanceLeftsRoll())
                using (var rightsView = new AcceptanceRightsRoll())
                using (var flowsView = new FlowAccountsRoll())
                using (var accountsView = new AccountsRoll())
                using (var moneyOrdersView = new MoneyOrdersRoll())
                {
                    var outLeft = leftsView.FirstOrDefault(item => item.ApplyID == entity.ID && item.AccountMethord == AccountMethord.Output);
                    var inLeft = leftsView.FirstOrDefault(item => item.ApplyID == entity.ID && item.AccountMethord == AccountMethord.Input);

                    var outRight = rightsView.FirstOrDefault(item => item.AcceptanceLeftID == outLeft.ID);
                    var inRight = rightsView.FirstOrDefault(item => item.AcceptanceLeftID == inLeft.ID);

                    var outFlow = flowsView.FirstOrDefault(item => item.ID == outRight.FlowID);
                    var inFlow = flowsView.FirstOrDefault(item => item.ID == inRight.FlowID);

                    var moneyOrder = moneyOrdersView.FirstOrDefault(item => item.ID == entity.MoneyOrderID);

                    if (outFlow == null || string.IsNullOrEmpty(outFlow.ID) || inFlow == null || string.IsNullOrEmpty(inFlow.ID))
                    {
                        Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑调拨申请_Api, Services.Oplogs.GetMethodInfo(), "Api 异常", $"未找到流水!{entity.Json()}", url: url);
                        return;
                    }
                    else
                    {
                        var payeeAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayeeAccountID);
                        var payerAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayerAccountID);
                        var erate = ExchangeRates.Universal[outFlow.Currency, inFlow.Currency];

                        if (entity.Type != AcceptanceType.Discount)
                        {
                            return;
                        }

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
                                FeeType = ((int)FundTransferType.Discount).ToString(),
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

                                DiscountSeqNo = moneyOrder.Code,     //贴现流水号
                                DiscountInterest = Math.Abs(outFlow.Price) - inFlow.Price,    //贴现利息
                            }
                        };
                    };
                    result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑调拨申请_Api, Services.Oplogs.GetMethodInfo(), "向芯达通互通", result + data.Json(), url: url);
                }
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑调拨申请_Api, Services.Oplogs.GetMethodInfo(), "向芯达通互通异常!" + ex.Message, result + data.Json(), url: url);
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
            var files = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.AcceptanceApplyID.ToString(), Request.QueryString["id"]);
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
        private void FilesEnter(string applyId, AcceptanceType type, string moId, string endorsId)
        {
            var fileIds = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.AcceptanceApplyID.ToString(), applyId).Select(item => item.ID).ToArray();

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
                    Name = FilesMapName.AcceptanceApplyID.ToString(),
                    Value = applyId,
                });
                files[i].FilesMapsArray = filesMaps.ToArray();
            }

            //承兑贴现
            if (type == AcceptanceType.Acceptance || type == AcceptanceType.Discount)
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
            if (type == AcceptanceType.Endorsement)
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
        #endregion
    }
}