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
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using ApprovalStatus = Yahv.Finance.Services.Enums.ApprovalStatus;

namespace Yahv.Finance.WebApp.Payer.ProductsFee
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

                //目标汇率，如果不为1，表示非相同币种
                var targetRate = ExchangeRates.Universal[data.PayerAccount.Currency, data.PayeeAccount.Currency];

                this.Model.Data = new
                {
                    PayeeAccountID = data.PayeeAccount?.Name,
                    PayeeCurrency = data.PayeeAccount?.Currency.GetDescription(),
                    PayeeCode = data.PayeeAccount?.Code,
                    PayeeBank = data.PayeeAccount?.BankName,
                    PayerAccountID = data.PayerAccount?.Name,
                    PayerCurrency = data.PayerAccount?.Currency.GetDescription(),
                    PayerCode = data.PayerAccount?.Code,
                    PayerBank = data.PayerAccount?.BankName,
                    IsPaid = data.IsPaid,
                    //PaymentDate = data.PaymentDate?.ToString("yyyy-MM-dd"),
                    AccountCatalogID = data.PayerLeft.AccountCatalogID,
                    Price = data.Price,
                    ApplierID = data.Applier?.RealName,
                    ApproverID = data.Approver?.RealName,
                    Summary = data.Summary,
                    PaymentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    TargetRate = targetRate,
                };

                //付款类型
                this.Model.AccountCatalogs = AccountCatalogsAlls.Current.Json(AccountCatalogType.Output.GetDescription());
                //付款方式
                this.Model.PaymentMethord = ExtendsEnum.ToDictionary<PaymentMethord>().Select(item => new { value = item.Key, text = item.Value });

                //银行承兑账户
                this.Model.MosBank = GetMoneyOrdersByType(PaymentMethord.BankAcceptanceBill, data.PayerAccountID);
                //商业承兑账户
                this.Model.MosCommercial = GetMoneyOrdersByType(PaymentMethord.CommercialAcceptanceBill, data.PayerAccountID);
            }



        }
        #endregion

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "操作成功!" };

            FlowAccount flowAccount = null;
            PayerRight payerRight = null;
            MoneyOrder moneyOrder = null;
            Endorsement endorsement = null;     //背书转让

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var flowsView = new FlowAccountsRoll(reponsitory))
            using (var moneyOrderView = new MoneyOrdersRoll(reponsitory))
            using (tran = reponsitory.OpenTransaction())
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

                    if (!string.IsNullOrEmpty(Request.Form["FormCode"]) && flowsView.Any(item => item.FormCode == Request.Form["FormCode"]))
                    {
                        json.success = false;
                        json.data = "流水号已存在，不能重复录入!";
                        return json;
                    }

                    //修改申请状态
                    entity.Status = ApplyStauts.Completed;

                    if (!string.IsNullOrEmpty(Request.Form["Price"]))
                    {
                        entity.Price = decimal.Parse(Request.Form["Price"]);
                    }

                    var paymentMethord = (Underly.PaymentMethord)(int.Parse(Request.Form["PaymentMethord"]));
                    FlowAccountType faType = FlowAccountType.BankStatement;

                    //如果是承兑汇票，修改流水表账户类型
                    if (paymentMethord == Underly.PaymentMethord.CommercialAcceptanceBill ||
                        paymentMethord == Underly.PaymentMethord.BankAcceptanceBill)
                    {
                        moneyOrder = moneyOrderView[Request.Form["MoneyOrderID"]];
                        faType = FlowAccountType.MoneyOrder;
                        moneyOrder.Status = MoneyOrderStatus.Transferred;
                        moneyOrder.ModifierID = Erp.Current.ID;
                        moneyOrder.ModifyDate = DateTime.Now;
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

                    //添加流水
                    var rate = ExchangeRates.Universal[entity.Currency, Currency.CNY];
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
                        TargetAccountCode = entity.PayeeAccount.Code,
                        TargetAccountName = entity.PayeeAccount.Name,
                        TargetRate = decimal.Parse(Request.Form["TargetRate"]),
                        PaymentDate = DateTime.Parse(Request.Form["PaymentDate"]),
                        Price = -entity.Price,
                        Price1 = -(entity.Price * rate).Round(),
                        PaymentMethord = paymentMethord,
                        Type = faType,
                        MoneyOrderID = moneyOrder?.ID,
                    };
                    flowAccount.Enter();

                    //添加核销
                    payerRight = new PayerRight()
                    {
                        Currency = entity.Currency,
                        CreateDate = DateTime.Now,
                        CreatorID = Erp.Current.ID,
                        Currency1 = Currency.CNY,
                        FlowID = flowAccount.ID,
                        ERate1 = rate,
                        PayerLeftID = entity.PayerLeft.ID,
                        Price = entity.Price,
                        Price1 = (entity.Price * rate).Round()
                    };
                    payerRight.Enter();

                    //附件
                    FilesEnter(id, FilesMapName.PayerApplyID.ToString(), Services.Enums.FileDescType.ProductsFeeVoucher, paymentMethord, endorsement?.ID);


                    //添加审批日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.ProductsFee, ApprovalStatus.Payment, id, Erp.Current.ID, Request.Form["Comments"]);
                    moneyOrder?.Enter();
                    entity.Enter();

                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.货款申请, Services.Oplogs.GetMethodInfo(), "支付", new { flowAccount, payerRight }.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }
                    json.success = false;
                    json.data = "操作失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.货款申请, Services.Oplogs.GetMethodInfo(), "支付 异常!", new { flowAccount, payerRight, exception = ex.ToString() }.Json());
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
            var files = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.PayerApplyID.ToString(), Request.QueryString["id"]);
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
        private void FilesEnter(string applyId, string fileMapName, Services.Enums.FileDescType fileType, PaymentMethord paymentMethord,string endorsId)
        {
            if (applyId.IsNullOrEmpty())
            {
                return;
            }
            var fileIds = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(fileMapName, applyId).Select(item => item.ID).ToArray();
            //前端获取的附件数据
            string fileData = Request.Form["files"].Replace("&quot;", "'");
            List<FilesDescription> files = fileData.JsonTo<List<FilesDescription>>();
            List<FilesDescription> files_endors = fileData.JsonTo<List<FilesDescription>>();

            //非新增附件
            var fileIds_exist = files.Where(item => !string.IsNullOrEmpty(item.ID)).Select(item => item.ID).ToArray();

            for (int i = 0; i < files.Count(); i++)
            {
                files[i].Type = fileType;
                files[i].CreatorID = Erp.Current.ID;

                List<FilesMap> filesMaps = new List<FilesMap>();
                filesMaps.Add(new FilesMap()
                {
                    Name = fileMapName,
                    Value = applyId,
                });
                files[i].FilesMapsArray = filesMaps.ToArray();
            }

            //如果是承兑汇票，同步附件
            if (paymentMethord == PaymentMethord.BankAcceptanceBill ||
                paymentMethord == PaymentMethord.CommercialAcceptanceBill)
            {
                for (int i = 0; i < files_endors.Count(); i++)
                {
                    files_endors[i].Type = Services.Enums.FileDescType.Endorsement;
                    files_endors[i].CreatorID = Erp.Current.ID;

                    List<FilesMap> filesMaps = new List<FilesMap>();
                    filesMaps.Add(new FilesMap()
                    {
                        Name = FilesMapName.EndorsementID.ToString(),
                        Value = endorsId,
                    });
                    files_endors[i].FilesMapsArray = filesMaps.ToArray();
                }

                files = files.Concat(files_endors).ToList();
            }

            //删除不存在的附件
            var ids = fileIds.Where(item => !fileIds_exist.Contains(item)).ToArray();

            Erp.Current.Finance.FilesDescriptionView.Add(files.Where(item => string.IsNullOrEmpty(item.ID)).ToArray());
            Erp.Current.Finance.FilesDescriptionView.Delete(ids);
        }

        /// <summary>
        /// 获取未签收汇票
        /// </summary>
        /// <param name="type">付款方式</param>
        /// <param name="payeeAccountID">持票人账户ID</param>
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

    }
}