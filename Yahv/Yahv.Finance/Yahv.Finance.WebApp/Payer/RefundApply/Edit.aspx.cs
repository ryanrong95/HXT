using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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

namespace Yahv.Finance.WebApp.Payer.RefundApply
{
    public partial class Edit : ErpParticlePage
    {
        private const string AccountCatalogID = "AccCatType0072";        //销售退款

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


                using (var view = new RefundAppliesRoll())
                {
                    var data = view[id];

                    this.Model.Data = new
                    {
                        PayerCurrencyHidden = data.Currency,
                        PayeeCurrencyHidden = data.Currency,
                        PayeeLeftID = data.PayeeLeftID,
                        PayeeCurrency = data.Currency,
                        PayerAccountID = data.PayerAccountID,
                        PayeeAccountID = data.PayeeAccountID,
                        Price = data.Price,
                        ApplierID = data.ApplierID,
                        ApproverID = data.ApproverID,
                        Summary = data.Summary,
                    };
                }
            }

            //币种
            this.Model.PayeeCurrencies = ExtendsEnum.ToDictionary<Currency>().Select(item => new { value = item.Key, text = item.Value });

            //账户类型
            this.Model.Types = ExtendsEnum.ToDictionary<FlowAccountType>().Select(item => new { value = item.Key, text = item.Value });

            //收款单据
            var payeeLeft = new PayeeLeftsRoll();

            //汇票账户数据
            var lefts = payeeLeft.GetPayeeLeftsForMoneyOrder();
            if (!Erp.Current.IsSuper)
            {
                lefts = lefts.Where(item => item.CreatorID == Erp.Current.ID);
            }
            this.Model.PayeeLeftsForMO = lefts.Select(item => new
            {
                item.ID,
                PayeeName = item.AccountName,
                item.Balance,
                item.Currency,
                item.FormCode,
                item.Price,
                item.AccountID,
            });

            //银行账户 数据
            var bankLefts = payeeLeft.GetPayeeLeftsForBank();
            if (!Erp.Current.IsSuper)
            {
                bankLefts = bankLefts.Where(item => item.CreatorID == Erp.Current.ID);
            }
            this.Model.PayeeLeftsForBank = bankLefts.Select(item => new
            {
                item.ID,
                PayeeName = item.AccountName,
                item.Balance,
                item.Currency,
                item.FormCode,
                item.Price,
                item.AccountID,
            });

            //收款账户 客户
            var accounts = Erp.Current.Finance.Accounts.Where(item => item.Status == GeneralStatus.Normal && item.EnterpriseID != null);
            this.Model.PayeeAccounts = accounts.Where(item => item.EnterpriseID != null && (item.Enterprise.Type & EnterpriseAccountType.Client) != 0).ToArray()
                .Select(item => new
                {
                    item.ID,
                    CompanyName = item?.Enterprise?.Name ?? item.Name,
                    item.BankName,
                    Currency = item.Currency.GetDescription(),
                    CurrencyID = (int)item.Currency,
                    item.Code,
                });

            //付款账户
            if (!Erp.Current.IsSuper)
            {
                accounts = accounts.Where(item => item.OwnerID == Erp.Current.ID);
            }
            this.Model.PayerAccounts = accounts.Where(item => item.NatureType == NatureType.Public && (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray()
                .Select(item => new
                {
                    item.ID,
                    ShortName = item.ShortName ?? item.Name,
                    CompanyName = item?.Enterprise?.Name,
                    item.BankName,
                    Currency = item.Currency.GetDescription(),
                    CurrencyID = (int)item.Currency,
                    item.Code,
                });

            //类型
            this.Model.Types = ExtendsEnum.ToDictionary<FlowAccountType>()
                .Select(item => new { text = item.Value, value = item.Key });
        }
        #endregion

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };

            Services.Models.Origins.RefundApply apply = null;
            string id = Request.QueryString["id"];

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            using (var refundAppliesView = new RefundAppliesRoll())
            {
                try
                {
                    var payeeLeftID = Request.Form["PayeeLeftID"];
                    var payeeAccountID = Request.Form["PayeeAccountID"];        //收款账户
                    var price = decimal.Parse(Request.Form["Price"]);
                    //var payerAccountID = Request.Form["PayerAccountID"];        //付款账户
                    var applierID = Request.Form["ApplierID"];
                    var approverID = Request.Form["ApproverID"];
                    var summary = Request.Form["Summary"];
                    var currency = (Currency)int.Parse(Request.Form["PayerCurrencyHidden"]);
                    var type = (FlowAccountType)int.Parse(Request.Form["Type"]);

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        //申请
                        apply = new Services.Models.Origins.RefundApply()
                        {
                            Type = type,
                            AccountCatalogID = AccountCatalogID,
                            ApplierID = applierID,
                            ApproverID = approverID,
                            CreateDate = DateTime.Now,
                            CreatorID = Erp.Current.ID,
                            Currency = currency,
                            PayeeAccountID = payeeAccountID,
                            PayeeLeftID = payeeLeftID,
                            //PayerAccountID = payerAccountID,
                            Price = price,
                            Status = ApplyStauts.Waiting,
                            SenderID = SystemSender.Center.GetFixedID(),
                            Summary = summary,
                        };

                        apply.Enter();
                    }
                    else
                    {
                        apply = refundAppliesView.FindRefundApply(id);
                        //申请
                        apply.ApplierID = applierID;
                        apply.ApproverID = approverID;
                        apply.Currency = currency;
                        apply.PayeeAccountID = payeeAccountID;
                        apply.PayeeLeftID = payeeLeftID;
                        //apply.PayerAccountID = payerAccountID;
                        apply.Price = price;
                        apply.Status = ApplyStauts.Waiting;
                        apply.SenderID = SystemSender.Center.GetFixedID();
                        apply.Summary = summary;
                        apply.Enter();
                    }

                    //附件
                    FilesEnter(apply.ID);
                    //日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.RefundApply, ApprovalStatus.Submit, apply.ID, Erp.Current.ID);
                    tran.Commit();

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.预收退款申请, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(id) ? "新增" : "修改", new { apply }.Json());
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    json.success = false;
                    json.data = "添加失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.预收退款申请, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(id) ? "新增" : "修改") + " 异常!", new { apply, exception = ex.ToString() }.Json());
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
            string fileMapName = FilesMapName.RefundApplyID.ToString();

            var files = Erp.Current.Finance.FilesDescriptionView
                .SearchByFilesMapValue(fileMapName, Request.QueryString["id"]);
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
            string fileMapName = FilesMapName.RefundApplyID.ToString();
            var fileType = Services.Enums.FileDescType.RefundApply;

            var fileIds = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(fileMapName, applyId).Select(item => item.ID).ToArray();

            //前端获取的附件数据
            string fileData = Request.Form["files"].Replace("&quot;", "'");
            List<FilesDescription> files = fileData.JsonTo<List<FilesDescription>>();

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
            //删除不存在的附件
            var ids = fileIds.Where(item => !fileIds_exist.Contains(item)).ToArray();

            Erp.Current.Finance.FilesDescriptionView.Add(files.Where(item => string.IsNullOrEmpty(item.ID)).ToArray());
            Erp.Current.Finance.FilesDescriptionView.Delete(ids);
        }
        #endregion
    }
}