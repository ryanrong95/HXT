using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.PayerImport
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //付款账户 内部公司
#if DEBUG
                var accounts = Erp.Current.Finance.Accounts.Where(item => item.Status == GeneralStatus.Normal && item.Enterprise != null && (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray();
#else
                var accounts = Erp.Current.Finance.Accounts.Where(item => item.Status == GeneralStatus.Normal && item.OwnerID == Erp.Current.ID && item.Enterprise != null && (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray();
#endif
                var payerAccounts = accounts.Where(item => item.NatureType == NatureType.Public).ToArray()
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
                this.Model.PayerAccounts = payerAccounts;
            }
        }

        #region 功能函数
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\") + "payment.xls";
            DownLoadFile(fileName);
        }

        protected object addRange()
        {
            var result = new JMessage();

            var url = Request["urls"];//excel存 放的路径
            string accountId = Request["accountId"];
            string getUrl = string.Empty;
            string str = "_uploader/Payer/";

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

                    result.IsFailed(message: name);
                }
                else
                {
                    return AddRange(dt, accountId);
                }
            }
            catch (Exception ex)
            {
                result.IsFailed(ex);
                return result;
            }

            return result;
        }
        #endregion

        #region 自定义函数
        /// <summary>
        /// 数据验证
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> CheckData(DataTable dt)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();

            using (var accountCatalogs = new AccountCatalogsRoll())
            using (var accountsView = new AccountsRoll())
            {
                var accounts = accountsView.Select(item => item.Code).ToArray();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //交易时间
                    if (!dt.Columns.Contains("交易时间") || string.IsNullOrWhiteSpace(dt.Rows[i]["交易时间"].ToString()))
                    {
                        dic.Add(i + 1, $"交易时间不能为空!");
                        continue;
                    }

                    //摘要
                    if (!dt.Columns.Contains("摘要") || string.IsNullOrWhiteSpace(dt.Rows[i]["摘要"].ToString()))
                    {
                        dic.Add(i + 1, $"摘要不能为空!");
                        continue;
                    }

                    if (string.IsNullOrEmpty(accountCatalogs.GetID(dt.Rows[i]["摘要"].ToString().Split(new string[] { "-", "—" }, StringSplitOptions.RemoveEmptyEntries))))
                    {
                        dic.Add(i + 1, $"[{dt.Rows[i]["摘要"].ToString()}]摘要不正确!");
                        continue;
                    }

                    //发生额
                    if (!dt.Columns.Contains("发生额") || string.IsNullOrWhiteSpace(dt.Rows[i]["发生额"].ToString()))
                    {
                        dic.Add(i + 1, $"发生额不能为空!");
                        continue;
                    }

                    //对方账号
                    if (!dt.Columns.Contains("对方账号") || string.IsNullOrWhiteSpace(dt.Rows[i]["对方账号"].ToString()))
                    {
                        dic.Add(i + 1, $"对方账号不能为空!");
                        continue;
                    }

                    if (accounts.All(item => item != dt.Rows[i]["对方账号"].ToString().Trim()))
                    {
                        dic.Add(i + 1, $"对方账号不存在，请您先创建账户!");
                        continue;
                    }

                    //借贷标志
                    if (!dt.Columns.Contains("借贷标志") || string.IsNullOrWhiteSpace(dt.Rows[i]["借贷标志"].ToString()))
                    {
                        dic.Add(i + 1, $"借贷标志不能为空!");
                        continue;
                    }

                    //流水号
                    if (!dt.Columns.Contains("流水号") || string.IsNullOrWhiteSpace(dt.Rows[i]["流水号"].ToString()))
                    {
                        dic.Add(i + 1, $"流水号不能为空!");
                        continue;
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="dt"></param>
        private JMessage AddRange(DataTable dt, string accountId)
        {
            var result = new JMessage();

            using (var accountCatalogs = new AccountCatalogsRoll())
            {
                try
                {
                    var message = string.Empty;
                    int i = 1;
                    foreach (DataRow dr in dt.Rows)
                    {
                        var errorMessage = new PayerImportInput()
                        {
                            AccountCatalogID = accountCatalogs.GetID(dr["摘要"].ToString().Split(new string[] { "-", "—" }, StringSplitOptions.RemoveEmptyEntries)),
                            AccountID = accountId,
                            CreateDate = DateTime.Parse(dr["交易时间"].ToString()),
                            CreatorID = Erp.Current.ID,
                            FormCode = dr["流水号"].ToString().Trim(),
                            Price = decimal.Parse(dr["发生额"].ToString()),
                            TargetCode = dr["对方账号"].ToString().Trim(),
                            Type = dr["借贷标志"].ToString() == "借" ? PayerImportType.Borrow : PayerImportType.Loan,
                        }.Enter();

                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            message += $"<br />{i++}. [{dr["流水号"]}] 导入成功!";
                        }
                        else
                        {
                            message += $"<br />{i++}. [{dr["流水号"]}] 导入失败! {errorMessage}";
                        }
                    }

                    result.IsSuccess(message);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.付款导入, Services.Oplogs.GetMethodInfo(), "付款导入", dt.Rows.Json());
                }
                catch (Exception ex)
                {
                    result.IsFailed(ex);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.付款导入, Services.Oplogs.GetMethodInfo(), "付款导入 异常!", dt.Rows.Json());
                }
            }

            return result;
        }
        #endregion
    }
}