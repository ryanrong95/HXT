using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.BusinessRelations
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
            string enterpriseid = Request.QueryString["id"];
            var query = new BusinessRelationsRoll(enterpriseid);
            return this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Status,
                StatusDes = item.Status.GetDescription(),
                item.MainName,
                item.SubName,
                Relation = item.BusinessRelationType.GetDescription()
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
                var entity = new BusinessRelationsRoll()[id];
                entity.Status = AuditStatus.Closed;
                entity.Enter();
                LogsOperating.LogOperating(Erp.Current, id, $"停用关联关系:{ entity.ID}");

            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"停用关联关系" + ex);
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
                var entity = new BusinessRelationsRoll()[id];
                entity.Status = AuditStatus.Closed;
                entity.Enter();
                LogsOperating.LogOperating(Erp.Current, id, $"启用关联关系:{ entity.ID}");
            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"启用关联关系失败" + ex);
            }
        }
    }
}