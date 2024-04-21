using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.BusinessRelations
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.EnterpriseID = Request.QueryString["id"];
                Dictionary<string, string> typelist = new Dictionary<string, string>() { { "-100", "全部" } };
                this.Model.Type = typelist.Concat(ExtendsEnum.ToDictionary<Underly.BusinessRelationType>()).Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value
                });
            }
        }
        protected object data()
        {
            var query = new MapsEnterpriseExtendRoll()[Underly.ApplyTaskType.ClientBusinessRelation].Where(Predicate());
            return this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                StatusDes = item.AuditStatus,
                item.MainName,
                item.SubName,
                Relation = item.BusinessRelationType.GetDescription(),
                item.Creator,
               CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

        }
        Expression<Func<MapsEnterprise, bool>> Predicate()
        {
            Expression<Func<MapsEnterprise, bool>> predicate = item => item.AuditStatus == AuditStatus.Waiting;
            string name = Request.QueryString["Name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.MainName.Contains(name) || item.SubName.Contains(name));
            }

            string type = Request["RelationType"];
            if (!string.IsNullOrWhiteSpace(type) && type != "-100")
            {
                predicate = predicate.And(item => item.BusinessRelationType == (Underly.BusinessRelationType)int.Parse(type));
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