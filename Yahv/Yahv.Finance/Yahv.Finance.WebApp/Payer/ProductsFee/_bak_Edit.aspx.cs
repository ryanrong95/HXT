using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.ProductsFee
{
    public partial class _bak_Edit : ErpParticlePage
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
            var accounts = Erp.Current.Finance.Accounts.Where(item => item.Status == GeneralStatus.Normal && item.EnterpriseID != null);

            //收款账户
            this.Model.PayeeAccounts = accounts.Where(item => item.Enterprise.Type == EnterpriseAccountType.Company).ToArray()
                .Select(item => new
                {
                    item.ID,
                    item.Name,
                    item.OpeningBank,
                    Currency = item.Currency.GetDescription(),
                    item.Code,
                });

            //付款账户
            this.Model.PayerAccounts = accounts.Where(item => item.Enterprise.Type == EnterpriseAccountType.Client).ToArray()
                .Select(item => new
                {
                    item.ID,
                    item.Name,
                    item.OpeningBank,
                    Currency = item.Currency.GetDescription(),
                    item.Code,
                });

            //付款类型
            this.Model.AccountCatalogs = AccountCatalogsAlls.Current.Select(item => new { id = item.ID, text = item.Name });
            this.Model.AccountCatalogsJson = AccountCatalogsAlls.Current.Json(AccountCatalogType.Output.GetDescription());
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
        /// 加载付款项
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var id = Request.QueryString["id"];
            var view = Erp.Current.Finance.PayerStatementsView.Where(item => item.Status == GeneralStatus.Normal && item.ApplyID == id);

            return view.ToArray().Select(item => new
            {
                item.AccountCatalogID,
                item.LeftPrice,
                Currency = item.Currency.GetDescription(),
            });
        }

        protected object getTree()
        {
            var json = AccountCatalogsAlls.Current.Json(AccountCatalogType.Output.GetDescription());
            return json;
        }
        #endregion
    }
}