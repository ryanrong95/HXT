using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.MoneyOrders
{
    public partial class Detail : ErpParticlePage
    {
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
            //收款账户（内部公司）
            var myAccounts = Erp.Current.Finance.Accounts.GetMoneyOrderAccounts().Where(item => (item.Enterprise.Type & EnterpriseAccountType.Company) != 0);

            //只显示自己管理的账户
            if (!Erp.Current.IsSuper)
            {
                myAccounts = myAccounts.Where(item => item.OwnerID == Erp.Current.ID);
            }

            this.Model.PayeeAccounts = myAccounts.ToArray()
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
            this.Model.PayerAccounts = accounts.Where(item => (item.Enterprise.Type & EnterpriseAccountType.Client) != 0
                                                              && item.Currency == Currency.CNY).ToArray()
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
                this.Model.Data = new MoneyOrdersRoll().Find(id);
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

        protected object getRecords()
        {
            string moId = Request.QueryString["id"];
            return new EndorsementsRoll().Where(item => item.MoneyOrderID == moId)
                .ToMyPage(1, int.MaxValue);
        }
        #endregion
        #endregion
    }
}