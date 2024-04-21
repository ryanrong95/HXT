using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.BookAccounts
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.EnterpriseID = Request.QueryString["id"];
            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            var query = Erp.Current.CrmPlus.BookAccounts[id, RelationType.Suppliers];
            //.OrderByDescending(item => item.CreateDate)
            return this.Paging(query.ToArray().Select(item => new
            {
                item.ID,
                item.Bank,
                item.Account,
                item.BankAddress,
                item.SwiftCode,
                nature = item.IsPersonal ? "个人" : "公司",
                item.BankCode,
                BookAccountMethord = item.BookAccountMethord.GetDescription(),
                item.Status,
                CurrencyDes = item.Currency.GetDescription(),
                StatusName = item.Status.GetDescription(),
                Creator = item.Admin.RealName,
                item.Transfer
            }));
        }

        /// <summary>
        /// 停用
        /// </summary>
        protected void Closed()
        {
            var id = Request.Form["ID"];
            try
            {
                var entity = Erp.Current.CrmPlus.BookAccounts[id];
                entity.Closed();
                LogsOperating.LogOperating(Erp.Current, entity.EnterpriseID, $"停用账户:{ entity.ID}");

            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"停用账户失败" + ex);
            }
        }


        /// <summary>
        /// 启用
        /// </summary>
        protected void Enable()
        {
            var id = Request.Form["ID"];
            try
            {
                var entity = Erp.Current.CrmPlus.BookAccounts[id];
                entity.Enable();
                LogsOperating.LogOperating(Erp.Current, entity.EnterpriseID, $"启用账户:{ entity.ID}");
            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"启用账户失败" + ex);
            }
        }
    }
}