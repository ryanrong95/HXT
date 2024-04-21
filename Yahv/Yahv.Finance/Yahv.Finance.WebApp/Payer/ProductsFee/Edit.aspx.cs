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
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using AccountType = Yahv.Underly.AccountType;
using ApprovalStatus = Yahv.Finance.Services.Enums.ApprovalStatus;

namespace Yahv.Finance.WebApp.Payer.ProductsFee
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
                var data = Erp.Current.Finance.PayerAppliesView[id];

                using (var payerRightsView = new PayerRightsRoll())
                {
                    int? paymentMethord = null; //付款方式
                    string formCode = string.Empty; //流水号
                    string paymentDate = string.Empty; //付款日期

                    var payerRight = payerRightsView.FirstOrDefault(item => item.PayerLeftID == data.PayerLeft.ID);
                    if (payerRight != null && !string.IsNullOrWhiteSpace(payerRight.FlowID))
                    {
                        var flowAccount = Erp.Current.Finance.FlowAccounts.FirstOrDefault(item => item.ID == payerRight.FlowID);
                        if (flowAccount != null && !string.IsNullOrWhiteSpace(flowAccount.ID))
                        {
                            paymentMethord = (int)flowAccount.PaymentMethord;
                            formCode = flowAccount.FormCode;
                            paymentDate = flowAccount.PaymentDate?.ToString("yyyy-MM-dd");
                        }
                    }

                    var account = Erp.Current.Finance.Accounts.FirstOrDefault(item => item.ID == data.PayeeAccountID);

                    this.Model.Data = new
                    {
                        PayeeAccountID = data.PayeeAccountID,
                        PayeeAccountName = account.Name,
                        PayeeAccountCode = account.Code,
                        PayeeCurrency = (int)account.Currency,
                        PayeeCode = account.Code,
                        PayeeBank = account.BankName,

                        PayerAccountID = data.PayerAccountID,
                        PayerCurrency = data.PayerAccount?.Currency.GetDescription(),
                        PayerCode = data.PayerAccount?.Code,
                        PayerBank = data.PayerAccount?.BankName,
                        IsPaid = data.IsPaid,
                        PayerLeft = data.PayerLeft,
                        ApplierID = data.ApplierID,
                        ApproverID = data.ApproverID,
                        Summary = data.Summary,
                        Price = data.Price,
                        FormCode = formCode,
                        PaymentDate = paymentDate,
                        PaymentMethord = paymentMethord,
                        data.Status,
                        PayerID = data.PayerID,
                        Currency = data.Currency,
                    };
                }
            }


            var accounts = Erp.Current.Finance.Accounts.Where(item => item.Status == GeneralStatus.Normal && item.EnterpriseID != null);

            //收款账户 客户、供应商
            this.Model.PayeeAccounts = accounts.Where(item => (item.Enterprise.Type & EnterpriseAccountType.Client) != 0 || (item.Enterprise.Type & EnterpriseAccountType.Supplier) != 0).ToArray()
                .Select(item => new
                {
                    item.ID,
                    CompanyName = item?.Enterprise?.Name ?? item.Name,
                    item.BankName,
                    Currency = item.Currency.GetDescription(),
                    CurrencyID = (int)item.Currency,
                    item.Code,
                });

            //付款账户 内部公司
            //this.Model.PayerAccounts = accounts.Where(item => item.NatureType == NatureType.Public && (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray()
            //    .Select(item => new
            //    {
            //        item.ID,
            //        ShortName = item.ShortName ?? item.Name,
            //        CompanyName = item?.Enterprise?.Name,
            //        item.BankName,
            //        Currency = item.Currency.GetDescription(),
            //        CurrencyID = (int)item.Currency,
            //        item.Code,
            //    });

            //付款类型
            this.Model.AccountCatalogs = new MyAccountCatalogsTree(Erp.Current).Json(AccountCatalogType.Output.GetDescription()).Replace("\"name\":", "\"text\":");
            //付款方式
            this.Model.PaymentMethord = ExtendsEnum.ToDictionary<PaymentMethord>().Select(item => new { value = item.Key, text = item.Value });
            //币种
            this.Model.PayeeCurrencies = ExtendsEnum.ToDictionary<Currency>().Select(item => new { value = item.Key, text = item.Value });
        }
        #endregion

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };

            PayerApply payerApplie = null;
            PayerLeft payerLeft = null;
            FlowAccount flowAccount = null;
            PayerRight payerRight = null;
            string id = Request.QueryString["id"];

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                try
                {
                    var payeeAccountID = Request.Form["PayeeAccountID"];        //收款账户
                    var payerID = Request.Form["PayerID"];
                    var currency = (Currency)int.Parse(Request.Form["Currency"]);
                    var price = decimal.Parse(Request.Form["Price"]);

                    var rate = ExchangeRates.Universal[currency, Currency.CNY];
                    bool isPaid = false;       //默认false

                    var payeeAccount = Erp.Current.Finance.Accounts.FirstOrDefault(item => item.ID == payeeAccountID);
                    if (payeeAccount == null)
                    {
                        throw new Exception("收款人不存在!");
                    }

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        //申请
                        payerApplie = new PayerApply()
                        {
                            Currency = currency,
                            ApplierID = Request.Form["ApplierID"],
                            ApproverID = Request.Form["ApproverID"],
                            CreatorID = Erp.Current.ID,
                            IsPaid = isPaid,
                            Price = price,
                            Status = ApplyStauts.Waiting,
                            Summary = Request.Form["Summary"] ?? null,
                            PayeeAccountID = payeeAccount?.ID,
                            PayerID = payerID,
                            SenderID = SystemSender.Center.GetFixedID(),
                        };
                        payerApplie.Enter();
                        //应付
                        payerLeft = new PayerLeft()
                        {
                            PayeeAccountID = payeeAccount?.ID,
                            PayerID = payerID,
                            Currency = currency,
                            ApplyID = payerApplie.ID,
                            CreatorID = Erp.Current.ID,
                            Currency1 = Currency.CNY,
                            ERate1 = rate,
                            Price1 = (price * rate).Round(),
                            Price = price,
                            Status = GeneralStatus.Normal,
                            AccountCatalogID = Request.Form["AccountCatalogID"],
                        };
                        payerLeft.Enter();
                    }
                    else
                    {
                        //申请
                        payerApplie = Erp.Current.Finance.PayerAppliesView[id];
                        payerApplie.Currency = currency;
                        payerApplie.ApplierID = Request.Form["ApplierID"];
                        payerApplie.ApproverID = Request.Form["ApproverID"];
                        payerApplie.CreatorID = Erp.Current.ID;
                        payerApplie.Price = price;
                        payerApplie.Status = ApplyStauts.Waiting;
                        payerApplie.Summary = Request.Form["Summary"] ?? null;
                        payerApplie.PayeeAccountID = payeeAccount?.ID;
                        payerApplie.PayerID = payerID;
                        payerApplie.SenderID = SystemSender.Center.GetFixedID();

                        //应付
                        payerLeft = payerApplie.PayerLeft;
                        payerLeft.PayeeAccountID = payeeAccount?.ID;
                        payerLeft.PayerID = payerID;
                        payerLeft.Currency = currency;
                        payerLeft.ApplyID = payerApplie.ID;
                        payerLeft.CreatorID = Erp.Current.ID;
                        payerLeft.Currency1 = Currency.CNY;
                        payerLeft.ERate1 = rate;
                        payerLeft.Price1 = (price * rate).Round();
                        payerLeft.Price = price;
                        payerLeft.Status = GeneralStatus.Normal;
                        payerLeft.AccountCatalogID = Request.Form["AccountCatalogID"];

                        payerApplie.Enter();
                        payerLeft.Enter();
                    }

                    //附件
                    FilesEnter(payerApplie.ID);
                    //日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.ProductsFee, ApprovalStatus.Submit, payerApplie.ID, Erp.Current.ID);
                    tran.Commit();

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.货款申请, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(id) ? "新增" : "修改", new { payerApplie, payerLeft, flowAccount, payerRight }.Json());
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    json.success = false;
                    json.data = "添加失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.货款申请, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(id) ? "新增" : "修改") + " 异常!", new { payerApplie, payerLeft, flowAccount, payerRight, exception = ex.ToString() }.Json());
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
                .SearchByFilesMapValue(FilesMapName.PayerApplyID.ToString(), Request.QueryString["id"]);
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
            var fileIds = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.PayerApplyID.ToString(), applyId).Select(item => item.ID).ToArray();

            //前端获取的附件数据
            string fileData = Request.Form["files"].Replace("&quot;", "'");
            List<FilesDescription> files = fileData.JsonTo<List<FilesDescription>>();

            //非新增附件
            var fileIds_exist = files.Where(item => !string.IsNullOrEmpty(item.ID)).Select(item => item.ID).ToArray();

            for (int i = 0; i < files.Count(); i++)
            {
                files[i].Type = Services.Enums.FileDescType.ProductsFeeApply;
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
        #endregion
    }
}