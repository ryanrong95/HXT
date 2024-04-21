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
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.MoneyOrders
{
    public partial class Edit : ErpParticlePage
    {
        private const string AccountCatalog = "AccCatType0034";     //预收账款
        private DbTransaction tran = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        #region 加载数据
        public void InitData()
        {
            var accounts = Erp.Current.Finance.Accounts.Where(item => item.Status == GeneralStatus.Normal && item.EnterpriseID != null && item.NatureType == NatureType.Public);
            ////收款账户（内部公司）
            //var myAccounts = Erp.Current.Finance.Accounts.GetMoneyOrderAccounts().Where(item => (item.Enterprise.Type & EnterpriseAccountType.Company) != 0);

            ////只显示自己管理的账户
            //if (!Erp.Current.IsSuper)
            //{
            //    myAccounts = myAccounts.Where(item => item.OwnerID == Erp.Current.ID);
            //}

            //this.Model.PayeeAccounts = myAccounts.ToArray()
            //    .Select(item => new
            //    {
            //        item.ID,
            //        ShortName = item.ShortName ?? item.Name,
            //        CompanyName = item.Enterprise?.Name,
            //        item.BankName,
            //        Currency = item.Currency.GetDescription(),
            //        CurrencyID = (int)item.Currency,
            //        item.Code,
            //    });

            this.Model.PayeeAccounts = accounts.Where(item => item.Currency == Currency.CNY).ToArray()
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

            //出票账户（客户）
            //this.Model.PayerAccounts = accounts.Where(item => (item.Enterprise.Type & EnterpriseAccountType.Client) != 0
            //                                                  && item.Currency == Currency.CNY).ToArray()
            this.Model.PayerAccounts = accounts.Where(item => item.Currency == Currency.CNY).ToArray()
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

            //汇票类型
            this.Model.Types = ExtendsEnum.ToDictionary<MoneyOrderType>().Select(item => new { text = item.Value, value = item.Key });

            //承兑性质
            this.Model.Natures = ExtendsEnum.ToDictionary<MoneyOrderNature>()
                .Select(item => new { text = item.Value, value = item.Key });

            string id = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(id))
            {
                this.Model.Data = new MoneyOrdersRoll()[id];
            }
        }

        #region 加载附件
        /// <summary>
        /// 加载附件
        /// </summary>
        protected void filedata()
        {
            string id = Request.QueryString["id"];

            using (var query1 = new Yahv.Finance.Services.Views.Rolls.FilesDescriptionRoll())
            {
                var view = query1;

                view = view.SearchByFilesMapValue(FilesMapName.MoneyOrderID.ToString(), id);

                var files = view.ToArray();

                string fileWebUrlPrefix = ConfigurationManager.AppSettings["FileWebUrlPrefix"];

                Func<FilesDescription, object> convert = item => new
                {
                    FileID = item.ID,
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
        }
        #endregion
        #endregion

        #region 功能按钮
        #region 提交保存
        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };

            var id = Request.QueryString["id"];
            MoneyOrder entity = null;
            //FlowAccount flowAccount = null;
            //PayeeLeft payeeLeft = null;
            try
            {
                using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
                using (tran = reponsitory.OpenTransaction())
                using (var moneyOrdersView = new MoneyOrdersRoll(reponsitory))
                using (var accountsView = new AccountsRoll(reponsitory))
                {
                    string payerAccountID = Request.Form["PayerAccountID"];
                    string payeeAccountID = Request.Form["PayeeAccountID"];
                    string name = Request.Form["Name"];
                    int type = int.Parse(Request.Form["Type"]);
                    string code = Request.Form["Code"];
                    string bankCode = Request.Form["BankCode"];
                    string bankName = Request.Form["BankName"];
                    string bankNo = Request.Form["BankNo"];
                    decimal price = decimal.Parse(Request.Form["Price"]);
                    bool isTransfer = Request.Form["IsTransfer"] == "1";
                    bool isMoney = Request.Form["IsMoney"] == "1";
                    DateTime startDate = DateTime.Parse(Request.Form["StartDate"]);
                    DateTime endDate = DateTime.Parse(Request.Form["EndDate"]);
                    int nature = int.Parse(Request.Form["Nature"]);

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        //检查票据号码是否已经存在
                        if (moneyOrdersView.CheckCode(code))
                        {
                            json.IsFailed("票据号码已经存在!");
                            return json;
                        }

                        entity = new MoneyOrder()
                        {
                            ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.MoneyOrders),
                            BankCode = bankCode,
                            BankName = bankName,
                            BankNo = bankNo,
                            Code = code,
                            CreatorID = Erp.Current.ID,
                            CreateDate = DateTime.Now,
                            EndDate = endDate,
                            IsTransfer = isTransfer,
                            ModifierID = Erp.Current.ID,
                            ModifyDate = DateTime.Now,
                            Name = name,
                            Nature = (MoneyOrderNature)nature,
                            PayeeAccountID = payeeAccountID,
                            PayerAccountID = payerAccountID,
                            Price = price,
                            StartDate = startDate,
                            Type = (MoneyOrderType)type,
                            Status = MoneyOrderStatus.Ticketed,
                            IsMoney = isMoney,
                        };


                    }


                    //string newPayeeLeftID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.PayeeLeft);
                    //string newFlowID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);

                    //var payerAccount = accountsView.FirstOrDefault(item => item.ID == payerAccountID);

                    //flowAccount = new FlowAccount()
                    //{
                    //    ID = newFlowID,
                    //    Type = FlowAccountType.MoneyOrder,
                    //    AccountMethord = AccountMethord.Receipt,
                    //    AccountID = payeeAccountID,
                    //    Currency = Currency.CNY,
                    //    Price = price,
                    //    PaymentDate = endDate,
                    //    FormCode = code,
                    //    Currency1 = Currency.CNY,
                    //    ERate1 = 1,
                    //    Price1 = price,
                    //    CreatorID = Erp.Current.ID,
                    //    TargetAccountName = payerAccount?.Name,
                    //    TargetAccountCode = payerAccount?.Code,

                    //    PaymentMethord = (type == (int)MoneyOrderType.Commercial) ? PaymentMethord.CommercialAcceptanceBill : PaymentMethord.BankAcceptanceBill,
                    //    MoneyOrderID = entity.ID,
                    //};
                    //flowAccount.Enter();

                    //payeeLeft = new PayeeLeft()
                    //{
                    //    ID = newPayeeLeftID,
                    //    AccountCatalogID = AccountCatalog,
                    //    AccountID = payeeAccountID,
                    //    PayerName = payerAccount?.Name,
                    //    Currency = Currency.CNY,
                    //    Price = price,

                    //    Currency1 = Currency.CNY,
                    //    ERate1 = 1,
                    //    Price1 = price,

                    //    CreatorID = Erp.Current.ID,
                    //    FlowID = newFlowID,
                    //    PayerNature = NatureType.Public,
                    //};
                    //payeeLeft.Enter();
                    FilesEnter(entity.ID);

                    entity.AddSuccess += Entity_AddSuccess;
                    entity?.Enter();


                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑汇票管理, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(id) ? "新增" : "修改", new { entity }.Json(), url: Request.Url.ToString());
                    return json;
                }
            }
            catch (Exception ex)
            {
                if (tran?.Connection != null)
                {
                    tran.Rollback();
                }

                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑汇票管理, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(id) ? "新增" : "修改") + " 异常!", new { entity, exception = ex.ToString() }.Json(), url: Request.Url.ToString());
                json.success = false;
                json.data = $"提交异常!{ex.Message}";
                return json;
            }
        }

        /// <summary>
        /// 同步芯达通
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entity_AddSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as MoneyOrder;
            var result = string.Empty;
            var url = XdtApi.AcceptanceBill.GetApiAddress();
            InputParam<CenterAcceptanceBillInput> data = null;

            try
            {
                if (tran?.Connection != null)
                {
                    tran.Commit();
                }

                using (var accountsView = new AccountsRoll())
                using (var filesView = new FilesDescriptionRoll())
                {
                    var payerAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayerAccountID);
                    var payeeAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayeeAccountID);
                    var files = filesView.SearchByFilesMapValue(FilesMapName.MoneyOrderID.ToString(), entity.ID).ToArray();

                    data = new InputParam<CenterAcceptanceBillInput>()
                    {
                        Sender = SystemSender.Center.GetFixedID(),
                        Option = OptionConsts.insert,
                        Model = new CenterAcceptanceBillInput()
                        {
                            Type = entity.Type,
                            Code = entity.Code,
                            Name = entity.Name,
                            BankCode = entity.BankCode,
                            BankName = entity.BankName,
                            BankNo = entity.BankNo,
                            Currency = entity.Currency.GetCurrency().ShortName,
                            Price = entity.Price,
                            IsTransfer = entity.IsTransfer,
                            IsMoney = entity.IsMoney,
                            PayerAccountNo = payerAccount?.Code,
                            PayeeAccountNo = payeeAccount?.Code,
                            StartDate = entity.StartDate,
                            EndDate = entity.EndDate,
                            Nature = entity.Nature,
                            Status = entity.Status,
                            CreateDate = entity.CreateDate,
                            CreatorID = entity.CreatorID,
                            Files = files.Select(item => new CenterAcceptanceBillInput.AcceptanceBillFile()
                            {
                                FileType = MapXdt.MapMoFileDescTypeToXdt(item.Type),
                                Url = string.Concat(ConfigurationManager.AppSettings["FileWebUrlPrefix"], "/" + item.Url),
                                FileFormat = VirtualPathUtility.GetExtension(item.Url),
                                FileName = item.CustomName,
                            }).ToList(),
                        }
                    };

                    result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑汇票管理_Api, Services.Oplogs.GetMethodInfo(), "Api 新增", result + data.Json(), url: url);
                }
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑汇票管理_Api, Services.Oplogs.GetMethodInfo(), "Api 新增异常!" + ex.Message, result + data.Json(), url: url);
            }
        }
        #endregion

        #region 上传文件
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
        #endregion

        #region 私有函数
        /// <summary>
        /// 新增附件
        /// </summary>
        /// <param name="moId">承兑汇票Id</param>
        private void FilesEnter(string moId)
        {
            var fileIds = Erp.Current.Finance.FilesDescriptionView.SearchByFilesMapValue(FilesMapName.MoneyOrderID.ToString(), moId).Select(item => item.ID).ToArray();

            //前端获取的附件数据
            string fileData = Request.Form["files"].Replace("&quot;", "'");
            List<FilesDescription> files = fileData.JsonTo<List<FilesDescription>>();

            //非新增附件
            var fileIds_exist = files.Where(item => !string.IsNullOrEmpty(item.ID)).Select(item => item.ID).ToArray();

            for (int i = 0; i < files.Count(); i++)
            {
                files[i].Type = Services.Enums.FileDescType.MoneyOrder;
                files[i].CreatorID = Erp.Current.ID;

                List<FilesMap> filesMaps = new List<FilesMap>();
                filesMaps.Add(new FilesMap()
                {
                    Name = FilesMapName.MoneyOrderID.ToString(),
                    Value = moId,
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