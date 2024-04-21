using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.BookAccounts
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ID = Request.QueryString["id"];
        }

        #region  开户行账号
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected object data()
        {

            string id = Request.QueryString["id"];
            var query = Erp.Current.CrmPlus.BookAccounts[id, RelationType.Trade];

            return this.Paging(query, item => new
            {
                item.ID,
                item.Bank,
                item.Account,
                item.BankAddress,
                nature = item.IsPersonal == false ? Nature.Company.GetDescription() : Nature.Person.GetDescription(),
                item.BankCode,
                BookAccountType = item.BookAccountType.GetDescription(),
                BookAccountMethord = item.BookAccountMethord.GetDescription(),
                item.Status,
                Currency = item.Currency.GetDescription(),
                StatusName = item.Status.GetDescription(),
                Creator = item.Admin.RealName,
            });

        }


        /// <summary>
        /// 停用
        /// </summary>
        protected void BookAccountClosed()
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
        protected void BookAccountEnable()
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
        #endregion
    }
}