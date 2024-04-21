using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.Approvals.Specials
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected object data()
        {
            var query = new SpecialsRoll().Where(Predicate());
            return this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.SupplierName,
                item.Brand,
                item.PartNumber,
                item.Summary,
                Type = item.Type.GetDescription(),
                item.CreatorName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

        }
        Expression<Func<Special, bool>> Predicate()
        {
            Expression<Func<Special, bool>> predicate = item => item.Status == AuditStatus.Waiting;
            string name = Request.QueryString["Name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.SupplierName.Contains(name));
            }

            string brand = Request["Brand"];
            if (!string.IsNullOrWhiteSpace(brand))
            {
                predicate = predicate.And(item => item.Brand.Contains(brand));
            }

            string pn = Request["Pn"];
            if (!string.IsNullOrWhiteSpace(pn))
            {
                predicate = predicate.And(item => item.PartNumber.Contains(pn));
            }
            nBrandType type;
            if (Enum.TryParse(Request["nBrandType"],out type))
            {
                predicate = predicate.And(item => item.Type == type);
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
            var specils = new SpecialsRoll()[id];
            AuditStatus status = result ? AuditStatus.Normal : AuditStatus.Voted;
            specils.EnterSuccess += Specils_EnterSuccess;
            specils.Approve(status, Erp.Current.ID);
            return success;
        }

        private void Specils_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            success = true;
        }
    }
}