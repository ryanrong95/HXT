using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Invoices
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ID = Request.QueryString["id"];
        }


        #region 客户发票
        /// <summary>
        /// 
        /// </summary>
        protected object data()
        {
            var enterpriseid = Request.QueryString["ID"];
            var invoices = Erp.Current.CrmPlus.Invoices[enterpriseid, RelationType.Trade];
            return this.Paging(invoices.ToArray().Select(item=> new {

                item.ID,
                item.Enterprise.Name,
                item.Address,
                item.Tel,
                item.Bank,
                item.Account,
                item.Status,
                StatusDes= item.Status.GetDescription(),
                CreateDate=item.CreateDate.ToShortDateString(),
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

                var entity = Erp.Current.CrmPlus.Invoices[id];
                entity.Closed();
                LogsOperating.LogOperating(Erp.Current, entity.EnterpriseID, $"停用发票信息:{ entity.ID}");

            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"停用发票信息 操作失败" + ex);
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
                var entity = Erp.Current.CrmPlus.Invoices[id];
                entity.Enable();
                LogsOperating.LogOperating(Erp.Current, entity.EnterpriseID, $"启用发票信息:{ entity.ID}");
            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"启用发票信息 操作失败" + ex);
            }
        }
        #endregion


      
    }
}