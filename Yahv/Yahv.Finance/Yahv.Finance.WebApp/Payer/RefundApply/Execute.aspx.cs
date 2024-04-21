using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.DynamicData;
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
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using ApprovalStatus = Yahv.Underly.ApprovalStatus;

namespace Yahv.Finance.WebApp.Payer.RefundApply
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
                if (id.IsNullOrEmpty())
                {
                    return;
                }

                using (var view = new RefundAppliesRoll())
                {
                    var data = view[id];

                    this.Model.Data = new
                    {
                        PayeeLeftID = data.PayeeLeftID,
                        PayeeCurrency = data.CurrencyName,
                        PayerAccountName = data.PayerAccountName,
                        PayeeAccountName = data.PayeeAccountName,
                        Price = data.Price,
                        ApplierName = data.ApplierName,
                        ApproverName = data.ApproverName,
                        Summary = data.Summary,
                        PayeeAccountCode = data.PayeeAccountCode,
                        PayerAccountCode = data.PayerAccountCode,
                        PaymentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        TypeName = data.TypeName,
                    };

                    //付款方式
                    var acceptances = new string[]
                    {
                        ((int)PaymentMethord.BankAcceptanceBill).ToString()
                        ,((int)PaymentMethord.CommercialAcceptanceBill).ToString()
                    };
                    this.Model.PaymentMethord = ExtendsEnum.ToDictionary<PaymentMethord>()
                        .Where(item => !acceptances.Contains(item.Key))
                        .Select(item => new { value = item.Key, text = item.Value });
                }
            }
        }
        #endregion

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "操作成功!" };

            FlowAccount flowAccount = null;
            Services.Models.Origins.RefundApply entity = null;

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var flowsView = new FlowAccountsRoll(reponsitory))
            using (var applyView = new RefundAppliesRoll(reponsitory))
            using (var accountsView = new AccountsRoll(reponsitory))
            using (var payeeLefsView = new PayeeLeftsRoll(reponsitory))
            using (tran = reponsitory.OpenTransaction())
            {
                try
                {
                    string id = Request.QueryString["id"];
                    entity = applyView.FindRefundApply(id);
                    if (entity == null)
                    {
                        json.success = false;
                        json.data = "未找到该申请信息!";
                        return json;
                    }

                    if (!string.IsNullOrEmpty(Request.Form["FormCode"]) && flowsView.Any(item => item.FormCode == Request.Form["FormCode"]))
                    {
                        json.success = false;
                        json.data = "流水号已存在，不能重复录入!";
                        return json;
                    }

                    //修改申请状态
                    entity.Status = ApplyStauts.Completed;
                    var account = accountsView.FirstOrDefault(item => item.ID == entity.PayerAccountID);

                    //收款信息
                    var payeeLeft = payeeLefsView[entity.PayeeLeftID];
                    if (payeeLeft == null)
                    {
                        throw new Exception("收款信息不存在!");
                    }

                    //获取上次收款的流水
                    var flowOld = flowsView.FirstOrDefault(item => item.ID == payeeLeft.FlowID);
                    var targetAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayeeAccountID);
                    var paymentMethord = (Underly.PaymentMethord)(int.Parse(Request.Form["PaymentMethord"]));
                    var rate = flowOld.ERate1;
                    string moneyOrderId = flowOld?.MoneyOrderID ?? null;

                    //承兑账户
                    if (entity.Type == FlowAccountType.MoneyOrder)
                    {
                        //扣除承兑账户 金额
                        var flowAcceptance = new FlowAccount()
                        {
                            ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                            Currency = flowOld.Currency,
                            CreateDate = DateTime.Now,
                            AccountID = flowOld.AccountID,
                            AccountMethord = AccountMethord.Payment,
                            CreatorID = Erp.Current.ID,
                            Currency1 = Currency.CNY,
                            ERate1 = rate,
                            FormCode = flowOld.FormCode,
                            TargetAccountCode = targetAccount?.Code,
                            TargetAccountName = targetAccount?.Name,
                            TargetRate = 1,
                            PaymentDate = DateTime.Parse(Request.Form["PaymentDate"]),
                            Price = -entity.Price,
                            Price1 = -(entity.Price * rate).Round(),
                            PaymentMethord = flowOld.PaymentMethord,
                            Type = FlowAccountType.MoneyOrder,
                            MoneyOrderID = moneyOrderId,
                        };

                        flowAcceptance.Add();
                    }
                    else
                    {
                        //核销 退款金额
                        var flowHistory = new FlowAccount()
                        {
                            ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                            Currency = flowOld.Currency,
                            CreateDate = DateTime.Now,
                            AccountID = flowOld.AccountID,
                            AccountMethord = AccountMethord.Payment,
                            CreatorID = Erp.Current.ID,
                            Currency1 = Currency.CNY,
                            ERate1 = rate,
                            FormCode = flowOld.FormCode,
                            TargetAccountCode = targetAccount?.Code,
                            TargetAccountName = targetAccount?.Name,
                            TargetRate = 1,
                            PaymentDate = DateTime.Parse(Request.Form["PaymentDate"]),
                            Price = -entity.Price,
                            Price1 = -(entity.Price * rate).Round(),
                            PaymentMethord = flowOld.PaymentMethord,
                            Type = FlowAccountType.BankStatement,
                            MoneyOrderID = moneyOrderId,
                        };

                        flowHistory.Add();
                    }

                    //添加 付款金额流水
                    flowAccount = new FlowAccount()
                    {
                        ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                        Currency = entity.Currency,
                        CreateDate = DateTime.Now,
                        AccountID = entity.PayerAccountID,
                        AccountMethord = AccountMethord.Payment,
                        CreatorID = Erp.Current.ID,
                        Currency1 = Currency.CNY,
                        ERate1 = rate,
                        FormCode = Request.Form["FormCode"],
                        TargetAccountCode = targetAccount?.Code,
                        TargetAccountName = targetAccount?.Name,
                        TargetRate = 1,
                        PaymentDate = DateTime.Parse(Request.Form["PaymentDate"]),
                        Price = -entity.Price,
                        Price1 = -(entity.Price * rate).Round(),
                        PaymentMethord = paymentMethord,
                        Type = FlowAccountType.BankStatement,
                        MoneyOrderID = moneyOrderId,
                    };
                    flowAccount.Add();
                    entity.FlowID = flowAccount.ID;

                    //附件
                    FilesEnter(id);

                    //添加审批日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.FundTransfer, Services.Enums.ApprovalStatus.Payment, id, Erp.Current.ID, Request.Form["Comments"]);
                    entity.Enter();

                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.预收退款申请, Services.Oplogs.GetMethodInfo(), "支付", new { flowAccount, entity }.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }
                    json.success = false;
                    json.data = "操作失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.预收退款申请, Services.Oplogs.GetMethodInfo(), "支付 异常!", new { flowAccount, entity, exception = ex.ToString() }.Json());
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
            var files = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.RefundApplyID.ToString(), Request.QueryString["id"]);
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
        private void FilesEnter(string applyId)
        {
            if (applyId.IsNullOrEmpty())
            {
                return;
            }

            string fileMapName = FilesMapName.RefundApplyID.ToString();
            var fileIds = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(fileMapName, applyId).Select(item => item.ID).ToArray();
            //前端获取的附件数据
            string fileData = Request.Form["files"].Replace("&quot;", "'");
            List<FilesDescription> files = fileData.JsonTo<List<FilesDescription>>();

            //非新增附件
            var fileIds_exist = files.Where(item => !string.IsNullOrEmpty(item.ID)).Select(item => item.ID).ToArray();

            for (int i = 0; i < files.Count(); i++)
            {
                files[i].Type = Services.Enums.FileDescType.RefundApplyVoucher;
                files[i].CreatorID = Erp.Current.ID;

                List<FilesMap> filesMaps = new List<FilesMap>();
                filesMaps.Add(new FilesMap()
                {
                    Name = fileMapName,
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