using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.Approvals.BusinessRelations
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            var query = new BusinessRelationsRoll()[Underly.ApplyTaskType.SupplierBusinessRelation].Where(Predicate());
            return this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.MainID,
                StatusDes = item.Status,
                item.MainName,
                item.SubName,
                Relation = item.BusinessRelationType.GetDescription(),
                item.CreatorName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

        }
        Expression<Func<BusinessRelation, bool>> Predicate()
        {
            Expression<Func<BusinessRelation, bool>> predicate = item=>true;
            string name = Request.QueryString["Name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.MainName.Contains(name) || item.SubName.Contains(name));
            }

            Underly.BusinessRelationType type;
            if (Enum.TryParse(Request["RelationsType"],out type))
            {
                predicate = predicate.And(item => item.BusinessRelationType == type);
            }

            return predicate;
        }
        bool success = false;
        /// <summary>
        /// 审批
        /// </summary>
        protected bool Approve()
        {
            bool result = bool.Parse(Request["result"]);
            string id = Request["id"];
            var entity = new BusinessRelationsRoll()[id];
            AuditStatus status = result ? AuditStatus.Normal : AuditStatus.Voted;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Approve(status, Erp.Current.ID);
            return success;
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            success = true;
        }
    }
}