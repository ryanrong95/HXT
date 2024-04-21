using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Yahv.Underly.Enums;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using ApprovalStatus = Yahv.Underly.ApprovalStatus;

namespace Yahv.Finance.WebApp.Payer.SalaryApply
{
    public partial class Edit : ErpParticlePage
    {
        #region 常量

        private const string IDCARD = "身份证号码";
        private const string PAYEENAME = "收款姓名";
        private const string PAYERCODE = "付款账号";
        private const string PAYEECODE = "收款账号";
        private const string ENTNAME = "所属公司";
        private const string PRICE = "金额";
        private const string FORMCODE = "流水号";
        private const string PAYDATE = "付款日期";
        private const string ACCOUNTCATALOGID = "职工薪酬";
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                var entity = new Services.Models.Origins.SalaryApply()
                {
                    Currency = Currency.CNY,
                };

                if (!string.IsNullOrEmpty(id))
                {
                    entity = Erp.Current.Finance.SalaryApplies[id];
                }

                this.Model.Data = entity;

                this.Model.Currencies = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key == ((int)Currency.CNY).ToString() || item.Key == ((int)Currency.HKD).ToString())
                    .Select(item => new { text = item.Value, value = item.Key });
            }
        }

        #region 功能函数

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };

            var id = Request.Form["ID"];
            Services.Models.Origins.SalaryApply entity = null;

            try
            {
                string title = Request.Form["Title"];
                string currency = Request.Form["Currency"];
                string summary = Request.Form["Summary"];
                var data = Request.Form["data"];
                var salaries = data.JsonTo<List<dynamic>>().Select(item => new Salary()
                {
                    Price = item["金额"],
                    PaymentDate = item["付款日期"],
                    FormCode = item["流水号"],
                    CompanyName = item["所属公司"],
                    IdCard = item["身份证号码"],
                    PayeeCode = item["收款账号"],
                    PayeeName = item["收款姓名"],
                    PayerCode = item["付款账号"]
                }).ToList();

                var apply = Erp.Current.Finance.SalaryApplies;

                if (string.IsNullOrWhiteSpace(id))
                {
                    id = PKeySigner.Pick(Services.PKeyType.SalaryApplies);
                    entity = new Services.Models.Origins.SalaryApply()
                    {
                        ID = id,
                        Title = title,
                        Currency = (Currency)int.Parse(currency),
                        Price = salaries.Sum(item => item.Price),
                        Summary = summary,
                        SenderID = SystemSender.Center.GetFixedID(),
                        CreatorID = Erp.Current.ID,
                        CreateDate = DateTime.Now,
                        Status = ApplyStauts.Completed
                    };
                }

                InitBasicInfo(salaries);        //初始化基础信息
                entity.Enter();
                CreateItems(id, salaries);       //添加工资项、流水


                XdtSync(entity);
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.工资申请, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(id) ? "新增" : "修改", entity.Json());
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = "添加失败!" + ex.Message;
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.工资申请, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(id) ? "新增" : "修改") + "失败!", new { entity, ex = ex.ToString() }.Json());
                return json;
            }

            return json;
        }

        /// <summary>
        /// 同步芯达通
        /// </summary>
        /// <param name="entity"></param>
        private void XdtSync(Services.Models.Origins.SalaryApply entity)
        {
            var result = string.Empty;
            var url = XdtApi.FinanceFee.GetApiAddress();
            InputParam<ChargeInputDto> data = null;
            try
            {
                using (var flowsView = new FlowAccountsRoll())
                using (var accountsView = new AccountsRoll())
                using (var salaryItemsView = new SalaryApplyItemsRoll())
                {
                    var first = salaryItemsView.FirstOrDefault(item => item.ApplyID == entity.ID);

                    var payerAccountID = first?.PayerAccountID;
                    var payerAccount = accountsView.FirstOrDefault(item => item.ID == payerAccountID);
                    var paymentDate = first.PaymentDate;

                    if (payerAccount == null || string.IsNullOrEmpty(payerAccount.ID))
                    {
                        Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 异常", $"未找到付款账户信息!{entity.Json()}", url: url);
                        return;
                    }

                    List<ChargeItemDto> feeItems = new List<ChargeItemDto>()
                    {
                        new ChargeItemDto()
                        {
                            FeeType = Erp.Current.Finance.AccountCatalogs.Get("付款类型", "综合业务", "费用")
                                .FirstOrDefault(item => item.Name == ACCOUNTCATALOGID)?.ID,
                            Amount = entity.Price,
                        }
                    };

                    var flow = flowsView.FirstOrDefault(item => item.ID == first.FlowID);
                    data = new InputParam<ChargeInputDto>()
                    {
                        Sender = SystemSender.Center.GetFixedID(),
                        Option = OptionConsts.insert,
                        Model = new ChargeInputDto()
                        {
                            CreatorID = entity.CreatorID,
                            Rate = flow.ERate1,
                            Amount = entity.Price,
                            SeqNo = first.FormCode,
                            Currency = entity.Currency.GetCurrency().ShortName,
                            //ReceiveAccountNo = payeeAccount.Code,
                            PaymentType = (int)PaymentMethord.BankTransfer,
                            AccountNo = payerAccount.Code,
                            PaymentDate = DateTime.Parse(paymentDate.ToString()),
                            FeeItems = feeItems,
                            MoneyType = 1,
                        }
                    };
                };
                result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 新增", result + data.Json(), url: url);
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 新增异常!" + ex.Message, result + data.Json(), url: url);
            }
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\") + "wagesApply.xls";
            DownLoadFile(fileName);
        }

        protected object GetReadData()
        {
            var url = Request["urls"];//excel存 放的路径
            string getUrl = string.Empty;
            string str = "_uploader/Salary/";

            if (!string.IsNullOrEmpty(url))
            {
                int IndexofA = url.LastIndexOf(str);
                getUrl = url.Substring(IndexofA).Substring(str.Length);
            }
            var vpath = $"/{string.Join("/", str)}/";
            var path2 = Server.MapPath(vpath) + getUrl;

            try
            {
                var npoiHelper = new NPOIHelper(path2);
                var dt = npoiHelper.ExcelToDataTable();
                Dictionary<int, string> errorMsgs = CheckData(dt);

                if (errorMsgs.Count > 0)
                {
                    string fileName = npoiHelper.GenerateErrorExcel(errorMsgs);
                    FileInfo fileInfo = new FileInfo(fileName);
                    var fileFullName = fileInfo.FullName;
                    Uri uri = new Uri(url);
                    var name = "http://" + uri.Host + "//" + fileFullName.Substring(fileFullName.IndexOf(str.Replace("/", "\\")), fileFullName.Length - fileFullName.IndexOf(str.Replace("/", "\\")));

                    return new
                    {
                        success = true,
                        code = 500,
                        data = "请您检查导入文件!",
                        path = path2,
                        errorUrl = name,
                    };
                }
                else
                {
                    string JsonString = dt.Json();
                    return new
                    {
                        success = true,
                        code = 200,
                        data = JsonString,
                        path = path2,
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    success = true,
                    code = 500,
                    data = ex.Message,
                    path = path2,
                };
            }

        }
        #endregion

        #region 加载数据
        /// <summary>
        /// 加载 handsontable
        /// </summary>
        /// <returns></returns>
        protected object getData()
        {
            var id = Request.QueryString["id"];
            var data = Erp.Current.Finance.SalaryApplyItems.Where(item => item.ApplyID == id)
                .Select(item => new
                {
                    流水号 = item.FormCode,
                    付款账号 = item.PayerCode,
                    付款日期 = DateTime.Parse(item.PaymentDate.ToString()).ToString("yyyy-MM-dd"),
                    收款姓名 = item.PayeeName,
                    身份证号码 = item.PayeeIDCard,
                    所属公司 = item.PayeeCompany,
                    收款账号 = item.PayeeCode,
                    金额 = item.Price,
                });

            return data.Json();
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 初始化基础信息
        /// </summary>
        /// <param name="salaries"></param>
        private void InitBasicInfo(List<Salary> salaries)
        {
            if (salaries == null || salaries.Count <= 0)
            {
                throw new Exception("导入信息不能为空!");
            }

            //个人信息
            var persons = Erp.Current.Finance.Persons.ToArray();
            //账户信息
            var accounts = Erp.Current.Finance.Accounts.Select(item => new { item.Code, item.Currency }).ToArray();
            //企业信息
            var enterprises = Erp.Current.Finance.Enterprises.Select(item => new { item.Name, item.ID }).ToArray();

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                var dt_person = CreateDataTableByModule(new Layers.Data.Sqls.PvFinance.Persons());
                var dt_account = CreateDataTableByModule(new Layers.Data.Sqls.PvFinance.Accounts());
                var dt_ent = CreateDataTableByModule(new Layers.Data.Sqls.PvFinance.Enterprises());
                var dt_mapPurpose = CreateDataTableByModule(new Layers.Data.Sqls.PvFinance.MapsPurpose());      //账户用途
                var dt_mapsAccType = CreateDataTableByModule(new Layers.Data.Sqls.PvFinance.MapsAccountType());      //银行账户类型

                DataRow dr_person;
                DataRow dr_account;
                DataRow dr_ent;
                DataRow dr_mapPurpose;
                DataRow dr_mapsAccType;


                string adminId = Erp.Current.ID;
                DateTime now = DateTime.Now;

                Currency payerCurrency;
                string personId_Temp = string.Empty;    //个人ID
                string entId_Temp = string.Empty;       //企业ID

                var defaultPurposeID = Erp.Current.Finance.AccountPurposes.FirstOrDefault(item => item.Name == "工资").ID;
                var defaultAccTypeID = Erp.Current.Finance.AccountTypes.FirstOrDefault(item => item.Name == "基本账户").ID;

                foreach (var salary in salaries)
                {
                    payerCurrency = accounts.FirstOrDefault(item => item.Code == salary.PayerCode).Currency;
                    personId_Temp = persons.FirstOrDefault(item => item.IDCard == salary.IdCard && item.RealName == salary.PayeeName)?.ID;

                    //检查个人信息是否存在    不存在的话添加个人信息
                    if (string.IsNullOrWhiteSpace(personId_Temp))
                    {
                        personId_Temp = PKeySigner.Pick(Services.PKeyType.Persons);

                        dr_person = dt_person.NewRow();

                        dr_person["ID"] = personId_Temp;
                        dr_person["RealName"] = salary.PayeeName;
                        dr_person["IDCard"] = salary.IdCard;
                        dr_person["CreatorID"] = adminId;
                        dr_person["ModifierID"] = adminId;
                        dr_person["CreateDate"] = now;
                        dr_person["ModifyDate"] = now;
                        dr_person["Status"] = (int)GeneralStatus.Normal;

                        dt_person.Rows.Add(dr_person);
                    }

                    entId_Temp = enterprises.FirstOrDefault(item => item.Name == salary.CompanyName)?.ID;
                    //检查企业名称是否存在    不存在的话添加企业信息
                    if (string.IsNullOrEmpty(entId_Temp))
                    {
                        entId_Temp = PKeySigner.Pick(Services.PKeyType.Enterprise);
                        dr_ent = dt_ent.NewRow();
                        dr_ent["ID"] = entId_Temp;
                        dr_ent["Name"] = salary.CompanyName;
                        dr_ent["Type"] = (int)EnterpriseAccountType.Company;
                        dr_ent["CreatorID"] = adminId;
                        dr_ent["ModifierID"] = adminId;
                        dr_ent["CreateDate"] = now;
                        dr_ent["ModifyDate"] = now;
                        dr_ent["Status"] = (int)GeneralStatus.Normal;

                        dt_ent.Rows.Add(dr_ent);
                    }

                    //检查账户是否存在  不存在添加账户信息
                    if (!accounts.Any(item => item.Code == salary.PayeeCode))
                    {
                        dr_account = dt_account.NewRow();

                        dr_account["ID"] = PKeySigner.Pick(Services.PKeyType.Account);
                        dr_account["Name"] = salary.PayeeName;
                        dr_account["Code"] = salary.PayeeCode;
                        dr_account["NatureType"] = (int)NatureType.Private;
                        dr_account["ManageType"] = (int)ManageType.Normal;
                        dr_account["Currency"] = (int)payerCurrency;
                        dr_account["BankName"] = BankHelper.GetBankName(salary.PayeeCode);        //根据卡号找对应的银行名称

                        dr_account["EnterpriseID"] = entId_Temp;
                        dr_account["PersonID"] = personId_Temp;
                        dr_account["CreatorID"] = adminId;
                        dr_account["ModifierID"] = adminId;
                        dr_account["CreateDate"] = now;
                        dr_account["ModifyDate"] = now;
                        dr_account["Status"] = (int)GeneralStatus.Normal;

                        dt_account.Rows.Add(dr_account);

                        dr_mapPurpose = dt_mapPurpose.NewRow();
                        dr_mapPurpose["AccountID"] = dr_account["ID"].ToString();
                        dr_mapPurpose["AccountPurposeID"] = defaultPurposeID;     //账户用途 默认工资
                        dt_mapPurpose.Rows.Add(dr_mapPurpose);

                        dr_mapsAccType = dt_mapsAccType.NewRow();
                        dr_mapsAccType["AccountID"] = dr_account["ID"].ToString();
                        dr_mapsAccType["AccountTypeID"] = defaultAccTypeID;     //账户类型 默认一般账户
                        dt_mapsAccType.Rows.Add(dr_mapsAccType);
                    }
                }

                if (dt_person.Rows.Count > 0)
                {
                    reponsitory.SqlBulkCopyByDatatable(nameof(Layers.Data.Sqls.PvFinance.Persons), dt_person);
                }
                if (dt_ent.Rows.Count > 0)
                {
                    reponsitory.SqlBulkCopyByDatatable(nameof(Layers.Data.Sqls.PvFinance.Enterprises), dt_ent);
                }
                if (dt_account.Rows.Count > 0)
                {
                    reponsitory.SqlBulkCopyByDatatable(nameof(Layers.Data.Sqls.PvFinance.Accounts), dt_account);
                    reponsitory.SqlBulkCopyByDatatable(nameof(Layers.Data.Sqls.PvFinance.MapsPurpose), dt_mapPurpose);
                    reponsitory.SqlBulkCopyByDatatable(nameof(Layers.Data.Sqls.PvFinance.MapsAccountType), dt_mapsAccType);
                }
            }
        }

        /// <summary>
        /// 根据实体转换DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        private DataTable CreateDataTableByModule<T>(T model)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            var propeties = typeof(T).GetProperties().Where(item => !item.PropertyType.FullName.Contains(nameof(Layers.Data.Sqls))).ToArray();
            foreach (var property in propeties)
            {
                var propType = property.PropertyType;
                if ((propType.IsGenericType) && (propType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    propType = propType.GetGenericArguments()[0];
                }
                dataTable.Columns.Add(new DataColumn(property.GetCustomAttribute<ColumnAttribute>()?.Name ?? property.Name, propType));
            }
            return dataTable;
        }

        /// <summary>
        /// 添加工资项/流水表
        /// </summary>
        /// <param name="salaries"></param>
        private void CreateItems(string applyId, List<Salary> salaries)
        {
            if (salaries == null || salaries.Count <= 0)
            {
                throw new Exception("数据不能为空!");
            }

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                List<Layers.Data.Sqls.PvFinance.FlowAccounts> flows = new List<Layers.Data.Sqls.PvFinance.FlowAccounts>();
                List<Layers.Data.Sqls.PvFinance.SalaryApplyItems> salaryItems = new List<Layers.Data.Sqls.PvFinance.SalaryApplyItems>();

                //付款账户
                var accounts = Erp.Current.Finance.Accounts.ToArray();
                decimal rate;
                string adminId = Erp.Current.ID;
                DateTime now = DateTime.Now;
                DateTime? dateTime = null;
                Account payerAccount;       //付款账户
                Account payeeAccount;       //收款账户
                string flowId = string.Empty;
                string accountCatalogID = Erp.Current.Finance.AccountCatalogs.Get("付款类型", "综合业务", "费用")
                    .FirstOrDefault(item => item.Name == ACCOUNTCATALOGID)?.ID;     //职工薪酬


                //var pks = PKeySigner.Series(PKeyType.FlowAcc, dt.Rows.Count);
                for (int i = 0; i < salaries.Count; i++)
                {
                    Salary salary = salaries[i];
                    payerAccount = accounts.FirstOrDefault(item => item.Code == salary.PayerCode);
                    payeeAccount = accounts.FirstOrDefault(item => item.Code == salary.PayeeCode);
                    rate = ExchangeRates.Universal[payerAccount.Currency, Currency.CNY];     //本位币 汇率

                    if (salary.Price <= 0)
                    {
                        continue;
                    }

                    dateTime = null;
                    if (!string.IsNullOrWhiteSpace(salary.PaymentDate))
                    {
                        dateTime = DateTime.Parse(salary.PaymentDate);
                    }

                    flowId = PKeySigner.Pick(Services.PKeyType.FlowAcc);
                    flows.Add(new Layers.Data.Sqls.PvFinance.FlowAccounts()
                    {
                        ID = flowId,     //单个获取ID，触发器余额能正确计算，批量生成余额就计算错误
                        AccountMethord = (int)AccountMethord.Payment,
                        AccountID = payerAccount.ID,
                        Currency = (int)payerAccount.Currency,
                        Price = -salary.Price,
                        FormCode = salary.FormCode,
                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = -salary.Price * rate,
                        CreatorID = adminId,
                        CreateDate = now,
                        TargetAccountName = salary.PayeeName,
                        TargetAccountCode = salary.PayeeCode,
                        PaymentMethord = (int)PaymentMethord.BankTransfer,
                        PaymentDate = dateTime,
                    });

                    salaryItems.Add(new Layers.Data.Sqls.PvFinance.SalaryApplyItems()
                    {
                        ID = PKeySigner.Pick(Services.PKeyType.SalaryApplyItems),
                        CreateDate = DateTime.Now,
                        Status = (int)GeneralStatus.Normal,
                        Price = salary.Price,
                        ApplyID = applyId,
                        AccountCatalogID = accountCatalogID,
                        FlowID = flowId,
                        ModifyDate = DateTime.Now,
                        PayeeAccountID = payeeAccount.ID,        //收款账户ID
                        PayerAccountID = payerAccount.ID,
                    });
                }

                if (flows.Count > 0)
                {
                    reponsitory.Insert<Layers.Data.Sqls.PvFinance.FlowAccounts>(flows);
                }

                if (salaryItems.Count > 0)
                {
                    reponsitory.Insert<Layers.Data.Sqls.PvFinance.SalaryApplyItems>(salaryItems);
                }
            }
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> CheckData(DataTable dt)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            var accounts = Erp.Current.Finance.Accounts.Select(item => item.Code).ToArray();
            Dictionary<string, decimal> total = new Dictionary<string, decimal>();      //余额检查

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //判断付款账号是否存在
                if (!accounts.Contains(dt.Rows[i][PAYERCODE].ToString()))
                {
                    dic.Add(i + 1, $"【{dt.Rows[i][PAYERCODE]}】不存在，请检查付款账号是否正确!");
                    continue;
                }

                //收款账号
                if (string.IsNullOrWhiteSpace(dt.Rows[i][PAYEECODE].ToString()))
                {
                    dic.Add(i + 1, $"【{dt.Rows[i][PAYEECODE]}】不能为空，请输入{PAYEECODE}!");
                    continue;
                }

                //if (total.Keys.Contains(dt.Rows[i][PAYERCODE].ToString()))
                //{
                //    total[dt.Rows[i][PAYERCODE].ToString()] += decimal.Parse(dt.Rows[i][PRICE].ToString());
                //}
                //else
                //{
                //    total[dt.Rows[i][PAYERCODE].ToString()] = decimal.Parse(dt.Rows[i][PRICE].ToString());
                //}
            }

            ////检查余额是否足够
            //string errorMsg = string.Empty;
            //var flows = Erp.Current.Finance.FlowAccounts.Where(item => total.Keys.Contains(item.AccountCode));
            //foreach (var key in total.Keys)
            //{
            //    if (flows != null && flows.Any(item => item.AccountCode == key))
            //    {
            //        if (flows.Where(item => item.AccountCode == key).Sum(item => item.Price) < total[key])
            //        {
            //            errorMsg += $"付款账户[{key}] 余额不足!";
            //        }
            //    }
            //    else
            //    {
            //        errorMsg += $"付款账户[{key}] 余额不足!";
            //    }
            //}

            //if (!string.IsNullOrWhiteSpace(errorMsg))
            //{
            //    dic.Add(1, errorMsg);
            //}

            return dic;
        }
        #endregion
    }
}