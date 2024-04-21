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
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.ChargeApply
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
                var model = Erp.Current.Finance.ChargeApplies.GetApply(id);
                model.PaymentDateDes = DateTime.Now.ToString("yyyy-MM-dd");

                this.Model.Data = model;
            }

            //审批结果
            this.Model.ApprovalStatus = ExtendsEnum.ToDictionary<Services.Enums.ApprovalStatus>()
                .Where(item => int.Parse(item.Key) > 0)
                .Select(item => new { text = item.Value, value = item.Key });

            //付款方式
            this.Model.PaymentMethord = ExtendsEnum.ToDictionary<PaymentMethord>().Select(item => new { value = item.Key, text = item.Value });

        }
        #endregion

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "操作成功!" };

            string id = Request.QueryString["id"];
            List<ChargeApplyItem> costItems = null;

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (tran = reponsitory.OpenTransaction())
            using (var flowsView = new FlowAccountsRoll(reponsitory))
            {
                try
                {
                    var entity = Erp.Current.Finance.ChargeApplies[id];
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


                    costItems = Request.Form["Items"].JsonTo<List<ChargeApplyItem>>();

                    //添加流水
                    var rate = ExchangeRates.Universal[entity.Currency, Currency.CNY];
                    string flowId_temp = string.Empty;

                    if (!string.IsNullOrEmpty(Request.Form["FormCode"]) && flowsView.Any(item => item.FormCode == Request.Form["FormCode"]))
                    {
                        json.success = false;
                        json.data = "流水号已存在，不能重复录入!";
                        return json;
                    }

                    foreach (var applyItem in costItems)
                    {
                        flowId_temp = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);

                        new FlowAccount()
                        {
                            ID = flowId_temp,
                            Currency = entity.Currency,
                            CreateDate = DateTime.Now,
                            AccountID = entity.PayerAccountID,
                            AccountMethord = AccountMethord.Payment,
                            Balance = 0,        //余额 触发器做
                            Balance1 = 0,       //人民币 余额 触发器做
                            CreatorID = Erp.Current.ID,
                            Currency1 = Currency.CNY,
                            ERate1 = rate,
                            FormCode = Request.Form["FormCode"],
                            TargetAccountCode = Request.Form["PayeeCode"],
                            TargetAccountName = Request.Form["PayeeName"],
                            PaymentDate = DateTime.Parse(Request.Form["PaymentDateDes"]),
                            Price = -applyItem.Price.Value,
                            Price1 = -(applyItem.Price.Value * rate).Round(),
                            PaymentMethord = (PaymentMethord)int.Parse(Request.Form["PaymentMethord"]),
                            Type = FlowAccountType.BankStatement,
                        }.Enter();

                        applyItem.Status = ApplyItemStauts.Paid;
                        applyItem.FlowID = flowId_temp;
                    }

                    //资金申请项
                    Erp.Current.Finance.ChargeApplyItems.InsertOrUpdate(id, costItems);

                    //附件
                    FilesEnter(id);

                    //添加审批日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.CostApply, Services.Enums.ApprovalStatus.Payment, id, Erp.Current.ID, Request.Form["Comments"]);

                    entity.EnterSuccess += Entity_EnterSuccess;
                    entity.Enter();
                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用申请, Services.Oplogs.GetMethodInfo(), "支付", new { costItems }.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }
                    json.success = false;
                    json.data = "操作失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用申请, Services.Oplogs.GetMethodInfo(), "支付 异常!", new { costItems, exception = ex.ToString() }.Json());
                    return json;
                }
            }

            return json;
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Services.Models.Origins.ChargeApply;
            var result = string.Empty;
            var url = XdtApi.FinanceFee.GetApiAddress();
            InputParam<ChargeInputDto> data = null;
            try
            {
                if (tran?.Connection != null)
                {
                    tran.Commit();
                }

                using (var chargeItemsView = new ChargeApplyItemsRoll())
                using (var flowsView = new FlowAccountsRoll())
                using (var accountsView = new AccountsRoll())
                using (var filesView = new FilesDescriptionRoll())
                {
                    var chargeItems = chargeItemsView.Where(item => item.ApplyID == entity.ID).ToArray();
                    var flowsId = chargeItems.Select(item => item.FlowID).ToArray();
                    var flows = flowsView.Where(item => flowsId.Contains(item.ID)).ToArray();
                    var files = filesView.SearchByFilesMapValue(FilesMapName.ChargeApplyID.ToString(), entity.ID).ToArray();
                    if (flows == null || flows.Length <= 0 || chargeItems == null || chargeItems.Length <= 0)
                    {
                        Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 异常", $"未找到费用信息!{entity.Json()}", url: url);
                        return;
                    }
                    else
                    {
                        var payeeAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayeeAccountID);
                        var payerAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayerAccountID);

                        if (payeeAccount == null || string.IsNullOrEmpty(payeeAccount.ID))
                        {
                            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 异常", $"未找到收款账户信息!{entity.Json()}", url: url);
                            return;
                        }

                        if (payerAccount == null || string.IsNullOrEmpty(payerAccount.ID))
                        {
                            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 异常", $"未找到付款账户信息!{entity.Json()}", url: url);
                            return;
                        }

                        //付款账户 如果不是芯达通不需要互通
                        if (ConfigurationManager.AppSettings["XdtCompanyName"] != null && !ConfigurationManager
                            .AppSettings["XdtCompanyName"].Contains($",{payerAccount?.Enterprise?.Name},"))
                        {
                            return;
                        }

                        List<ChargeItemDto> feeItems = new List<ChargeItemDto>();

                        foreach (var chargeItem in chargeItems)
                        {
                            feeItems.Add(new ChargeItemDto()
                            {
                                FeeType = chargeItem.AccountCatalogID,
                                Amount = chargeItem?.Price ?? 0,
                                FeeDesc = chargeItem.Summary,
                            });
                        }

                        var flow = flows.FirstOrDefault();
                        data = new InputParam<ChargeInputDto>()
                        {
                            Sender = SystemSender.Center.GetFixedID(),
                            Option = OptionConsts.insert,
                            Model = new ChargeInputDto()
                            {
                                CreatorID = entity.ApplierID,
                                Rate = flow.ERate1,
                                Amount = entity.Price,
                                SeqNo = flow.FormCode,
                                Currency = entity.Currency.GetCurrency().ShortName,
                                ReceiveAccountNo = payeeAccount.Code,
                                PaymentType = (int)flow.PaymentMethord,
                                AccountNo = payerAccount.Code,
                                PaymentDate = DateTime.Parse(flow.PaymentDate.ToString()),
                                FeeItems = feeItems,
                                MoneyType = entity.IsPaid ? 2 : 1,
                                Files = files.Select(item => new CenterFeeFile()
                                {
                                    FileType = MapXdt.MapFileDescTypeToXdt(item.Type),
                                    Url = string.Concat(ConfigurationManager.AppSettings["FileWebUrlPrefix"], "/" + item.Url),
                                    FileFormat = VirtualPathUtility.GetExtension(item.Url),
                                    FileName = item.CustomName,
                                }).ToList()
                            }
                        };
                    };
                    result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 新增", result + data.Json(), url: url);
                }
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 新增异常!" + ex.Message, result + data.Json(), url: url);
            }
        }
        #endregion

        #region 功能函数
        /// <summary>
        /// 加载附件
        /// </summary>
        protected void filedata()
        {
            var files = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.ChargeApplyID.ToString(), Request.QueryString["id"]);
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
        /// 加载申请项
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var id = Request.QueryString["id"];
            if (string.IsNullOrWhiteSpace(id)) return null;
            var applyItems = Erp.Current.Finance.ChargeApplyItems.Where(item => item.ApplyID == id).ToArray();
            var catalogs = Erp.Current.Finance.AccountCatalogs.ToArray();

            return from applyItem in applyItems
                   join catalog in catalogs on applyItem.AccountCatalogID equals catalog.ID
                   select new
                   {
                       applyItem.ID,
                       AccountCatalogID = applyItem.AccountCatalogID,
                       AccountCatalogName = catalog.Name,
                       Price = applyItem.Price,
                       CreateDate = applyItem.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                       applyItem.Summary,
                   };
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
                files[i].Type = Services.Enums.FileDescType.ChargeApplyVoucher;
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