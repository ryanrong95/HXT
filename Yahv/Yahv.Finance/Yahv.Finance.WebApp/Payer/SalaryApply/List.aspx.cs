using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvFinance;
using Layers.Linq;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Npoi;
using Yahv.Utils.Validates;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using PKeyType = Yahv.Finance.Services.PKeyType;

namespace Yahv.Finance.WebApp.Payer.SalaryApply
{
    public partial class List : ErpParticlePage
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
        #endregion

        #region 加载数据
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Statuses = ExtendsEnum.ToDictionary<ApplyStauts>().Select(item => new { text = item.Value, value = item.Key });
            }
        }

        protected object data()
        {
            var query = Erp.Current.Finance.SalaryApplies.Where(GetExpression()).ToArray();
            return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
            {
                item.ID,
                item.Title,
                item.Price,
                item.CreatorName,
                Currency = item.Currency.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            });
        }
        #endregion

        #region 功能函数
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                string fileFullName = UploadFile();     //上传文件

                if (string.IsNullOrWhiteSpace(fileFullName))
                {
                    Easyui.Alert("操作提示", "上传文件失败!", Sign.Error);
                    return;
                }

                var npoiHelper = new NPOIHelper(fileFullName);
                var dt = npoiHelper.ExcelToDataTable();
                Dictionary<int, string> errorMsgs = CheckData(dt);

                if (errorMsgs.Count > 0)
                {
                    DownLoadFile(npoiHelper.GenerateErrorExcel(errorMsgs));
                }
                else
                {
                    //初始化 基础信息
                    InitBasicInfo(dt);

                    //添加流水
                    CreateFlowAccounts(dt);
                }

                sw.Stop();
                Easyui.Reload("操作提示", $"导入成功!{sw.ElapsedMilliseconds / 1000}秒");
            }
            catch (Exception ex)
            {
                Easyui.Alert("操作提示", $"导入失败!{ex.Message}", Sign.Error);
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
        #endregion

        #region 私有函数

        private Expression<Func<Services.Models.Origins.SalaryApply, bool>> GetExpression()
        {
            Expression<Func<Services.Models.Origins.SalaryApply, bool>> predicate = item => true;

            string name = Request.QueryString["s_name"];

            //关键字
            if (!string.IsNullOrWhiteSpace(name))
            {
                //predicate = predicate.And(item => item.AccountCode.Contains(name) || item.TargetAccountCode.Contains(name) || item.TargetAccountName.Contains(name));
            }

            return predicate;
        }

        /// <summary>
        /// 获取状态名称
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private string GetStatusName(PayerApply apply, ApplyStauts status)
        {
            string result = String.Empty;
            switch (status)
            {
                case ApplyStauts.Completed:
                    result = status.GetDescription();
                    break;
                case ApplyStauts.Rejecting:
                    result = apply.Applier?.RealName != null ? $"{status.GetDescription()}({apply.Applier?.RealName})" : status.GetDescription();
                    break;
                case ApplyStauts.Paying:
                    result = apply.Excuter?.RealName != null ? $"{status.GetDescription()}({apply.Excuter?.RealName})" : status.GetDescription();
                    break;
                default:
                    result = apply.Approver?.RealName != null ? $"{status.GetDescription()}({apply.Approver?.RealName})" : status.GetDescription();
                    break;
            }
            return result;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName"></param>
        private void DownLoadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            string name = fileName.Substring(fileName.LastIndexOf('\\') + 1, fileName.Length - Path.GetExtension(fileName).Length - fileName.LastIndexOf('\\') - 1);

            FileInfo fileInfo = new FileInfo(fileName);
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "attachment;filename=" + name + Path.GetExtension(fileName).ToLower());
            Response.AddHeader("Content-Length", fileInfo.Length.ToString());
            Response.AddHeader("Content-Transfer-Encoding", "binary");
            Response.ContentType = "application/octet-stream";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            Response.WriteFile(fileInfo.FullName);
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        private string UploadFile()
        {
            string fileFullName = string.Empty; //上传文件地址

            try
            {
                if (fileUpload.HasFile)
                {
                    DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["FileSavePath"]);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string extension = Path.GetExtension(fileUpload.PostedFile.FileName); //获取扩展名
                    fileFullName = Path.Combine(di.FullName,
                        DateTime.Now.Year.ToString(),
                        DateTime.Now.Month.ToString(),
                        DateTime.Now.Day.ToString(),
                        fileName + extension);

                    FileInfo fi = new FileInfo(fileFullName);
                    if (!fi.Directory.Exists)
                    {
                        fi.Directory.Create();
                    }

                    fileUpload.SaveAs(fileFullName);
                }
            }
            catch (Exception ex)
            {
                fileFullName = string.Empty;
            }

            return fileFullName;
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

                if (total.Keys.Contains(dt.Rows[i][PAYERCODE].ToString()))
                {
                    total[dt.Rows[i][PAYERCODE].ToString()] += decimal.Parse(dt.Rows[i][PRICE].ToString());
                }
                else
                {
                    total[dt.Rows[i][PAYERCODE].ToString()] = decimal.Parse(dt.Rows[i][PRICE].ToString());
                }
            }

            //检查余额是否足够
            string errorMsg = string.Empty;
            var flows = Erp.Current.Finance.FlowAccounts.Where(item => total.Keys.Contains(item.AccountCode));
            foreach (var key in total.Keys)
            {
                if (flows != null && flows.Any(item => item.AccountCode == key))
                {
                    if (flows.Where(item => item.AccountCode == key).Sum(item => item.Price) < total[key])
                    {
                        errorMsg += $"付款账户[{key}] 余额不足!";
                    }
                }
                else
                {
                    errorMsg += $"付款账户[{key}] 余额不足!";
                }
            }

            if (!string.IsNullOrWhiteSpace(errorMsg))
            {
                dic.Add(1, errorMsg);
            }

            return dic;
        }

        /// <summary>
        /// 初始化基础信息
        /// </summary>
        /// <param name="dt"></param>
        private void InitBasicInfo(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
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

                foreach (DataRow row in dt.Rows)
                {
                    payerCurrency = accounts.FirstOrDefault(item => item.Code == row[PAYERCODE].ToString()).Currency;
                    personId_Temp = persons.FirstOrDefault(item => item.IDCard == row[IDCARD].ToString() && item.RealName == row[PAYEENAME].ToString())?.ID;

                    //检查个人信息是否存在    不存在的话添加个人信息
                    if (string.IsNullOrWhiteSpace(personId_Temp))
                    {
                        personId_Temp = PKeySigner.Pick(PKeyType.Persons);

                        dr_person = dt_person.NewRow();

                        dr_person["ID"] = personId_Temp;
                        dr_person["RealName"] = row[PAYEENAME].ToString();
                        dr_person["IDCard"] = row[IDCARD];
                        dr_person["CreatorID"] = adminId;
                        dr_person["ModifierID"] = adminId;
                        dr_person["CreateDate"] = now;
                        dr_person["ModifyDate"] = now;
                        dr_person["Status"] = (int)GeneralStatus.Normal;

                        dt_person.Rows.Add(dr_person);
                    }

                    entId_Temp = enterprises.FirstOrDefault(item => item.Name == row[ENTNAME].ToString())?.ID;
                    //检查企业名称是否存在    不存在的话添加企业信息
                    if (string.IsNullOrEmpty(entId_Temp))
                    {
                        entId_Temp = PKeySigner.Pick(PKeyType.Enterprise);
                        dr_ent = dt_ent.NewRow();
                        dr_ent["ID"] = entId_Temp;
                        dr_ent["Name"] = row[ENTNAME].ToString();
                        dr_ent["Type"] = (int)EnterpriseAccountType.Company;
                        dr_ent["CreatorID"] = adminId;
                        dr_ent["ModifierID"] = adminId;
                        dr_ent["CreateDate"] = now;
                        dr_ent["ModifyDate"] = now;
                        dr_ent["Status"] = (int)GeneralStatus.Normal;

                        dt_ent.Rows.Add(dr_ent);
                    }

                    //检查账户是否存在  不存在添加账户信息
                    if (!accounts.Any(item => item.Code == row[PAYEECODE].ToString()))
                    {
                        dr_account = dt_account.NewRow();

                        dr_account["ID"] = PKeySigner.Pick(PKeyType.Account);
                        dr_account["Name"] = row[PAYEENAME].ToString();
                        dr_account["Code"] = row[PAYEECODE].ToString();
                        dr_account["NatureType"] = (int)NatureType.Private;
                        dr_account["ManageType"] = (int)ManageType.Normal;
                        dr_account["Currency"] = (int)payerCurrency;
                        dr_account["BankName"] = BankHelper.GetBankName(row[PAYEECODE].ToString());        //根据卡号找对应的银行名称

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
        /// 添加流水表
        /// </summary>
        /// <param name="dt"></param>
        private void CreateFlowAccounts(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                throw new Exception("数据不能为空!");
            }

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                List<Layers.Data.Sqls.PvFinance.FlowAccounts> list = new List<Layers.Data.Sqls.PvFinance.FlowAccounts>();

                //付款账户
                var accounts = Erp.Current.Finance.Accounts.ToArray();
                decimal rate;
                string adminId = Erp.Current.ID;
                DateTime now = DateTime.Now;
                DateTime? dateTime = null;
                Account account;

                //var pks = PKeySigner.Series(PKeyType.FlowAcc, dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    account = accounts.FirstOrDefault(item => item.Code == row[PAYERCODE].ToString());
                    rate = ExchangeRates.Universal[account.Currency, Currency.CNY];     //本位币 汇率

                    if (string.IsNullOrWhiteSpace(row[PRICE].ToString()) || !row[PRICE].ToString().IsNumber())
                    {
                        continue;
                    }

                    dateTime = null;
                    if (!string.IsNullOrWhiteSpace(row[PAYDATE].ToString()))
                    {
                        dateTime = DateTime.Parse(row[PAYDATE].ToString());
                    }

                    list.Add(new Layers.Data.Sqls.PvFinance.FlowAccounts()
                    {
                        ID = PKeySigner.Pick(PKeyType.FlowAcc),     //单个获取ID，触发器余额能正确计算，批量生成余额就计算错误
                        AccountMethord = (int)AccountMethord.Payment,
                        AccountID = account.ID,
                        Currency = (int)account.Currency,
                        Price = -decimal.Parse(row[PRICE].ToString()),
                        FormCode = row[FORMCODE].ToString(),
                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = -decimal.Parse(row[PRICE].ToString()) * rate,
                        CreatorID = adminId,
                        CreateDate = now,
                        TargetAccountName = row[PAYEENAME].ToString(),
                        TargetAccountCode = row[PAYEECODE].ToString(),
                        PaymentMethord = (int)PaymentMethord.BankTransfer,
                        PaymentDate = dateTime,
                    });
                }

                if (list.Count > 0)
                {
                    reponsitory.Insert<Layers.Data.Sqls.PvFinance.FlowAccounts>(list);
                }
            }
        }

        #region _bak CreateFlowAccounts
        private void _bak_CreateFlowAccounts(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                throw new Exception("数据不能为空!");
            }

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //付款账户
                Account account = Erp.Current.Finance.Accounts.FirstOrDefault(item => item.Code == dt.Rows[0][PAYERCODE].ToString());
                if (account == null || string.IsNullOrWhiteSpace(account.ID))
                {
                    throw new Exception("付款账户不存在!");
                }
                var rate = ExchangeRates.Universal[account.Currency, Currency.CNY];     //本位币 汇率

                var dt_Flows = CreateDataTableByModule(new Layers.Data.Sqls.PvFinance.FlowAccounts());
                DataRow dr;
                string adminId = Erp.Current.ID;
                DateTime now = DateTime.Now;

                foreach (DataRow row in dt.Rows)
                {
                    if (string.IsNullOrWhiteSpace(row[PRICE].ToString()) || !row[PRICE].ToString().IsNumber())
                    {
                        continue;
                    }

                    dr = dt_Flows.NewRow();

                    dr["ID"] = PKeySigner.Pick(PKeyType.FlowAcc);
                    dr["AccountMethord"] = (int)AccountMethord.Payment;
                    dr["AccountID"] = account.ID;
                    dr["Currency"] = (int)account.Currency;
                    dr["Price"] = row[PRICE].ToString();
                    //dr["PaymentDate"] = account.ID;     //付款日期
                    dr["FormCode"] = row[FORMCODE].ToString();
                    dr["Currency1"] = (int)Currency.CNY;
                    dr["ERate1"] = rate;
                    dr["Price1"] = decimal.Parse(row[PRICE].ToString()) * rate;

                    dr["CreatorID"] = adminId;
                    dr["CreateDate"] = now;
                    dr["TargetAccountName"] = row[PAYEENAME].ToString();
                    dr["TargetAccountCode"] = row[PAYEECODE].ToString();
                    dr["PaymentMethord"] = (int)PaymentMethord.BankTransfer;

                    dt_Flows.Rows.Add(dr);
                }

                if (dt_Flows.Rows.Count > 0)
                {
                    reponsitory.SqlBulkCopyByDatatable(nameof(Layers.Data.Sqls.PvFinance.FlowAccounts), dt_Flows);
                }
            }
        }
        #endregion
        #endregion
    }
}