using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.ApprovalRecords
{
    public partial class MyProtecteds : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            string supplierid = Request.QueryString["supplierid"];
            var query = Yahv.CrmPlus.Service.ApprovalRecords.MySupplierProtecteds(Erp.Current, supplierid).Where(Predicate());
            return this.Paging(query.OrderByDescending(item => item.ApplyDate).ToArray().Select(item => new
            {
                item.ID,
                item.MainID,
                item.EnterpriseName,
                Applyer = item.ApplyAdmin?.RealName,
                Approver = item.ApproveAdmin?.RealName,
                ApplyDate = item.ApplyDate.ToString("yyyy-MM-dd HH:mm"),
                ApproveDate = item.ApproveDate?.ToString("yyyy-MM-dd HH:mm"),
                Status = item.Status.GetDescriptions()
            }));

        }
        Expression<Func<ProtectedRecord, bool>> Predicate()
        {
            Expression<Func<ProtectedRecord, bool>> predicate = item => true;
           
            string applyStartDate = Request.QueryString["RegisteStartDate"];
            DateTime applystart;
            if (!string.IsNullOrWhiteSpace(applyStartDate) && DateTime.TryParse(applyStartDate, out applystart))//开始日期
            {
                predicate = predicate.And(item => item.ApplyDate >= applystart);
            }

            string applyEnddate = Request.QueryString["RegisteEndDate"];
            DateTime applyend;
            if (!string.IsNullOrWhiteSpace(applyEnddate) && DateTime.TryParse(applyEnddate, out applyend))//结束日期
            {
                predicate = predicate.And(item => item.ApplyDate < applyend.AddDays(1));
            }

            string approveStartDate = Request.QueryString["ApproveStartDate"];
            DateTime approvestart;
            if (!string.IsNullOrWhiteSpace(approveStartDate) && DateTime.TryParse(approveStartDate, out approvestart))//开始日期
            {
                predicate = predicate.And(item => item.ApproveDate >= approvestart);
            }

            string approveEnddate = Request.QueryString["ApproveEndDate"];
            DateTime approveend;
            if (!string.IsNullOrWhiteSpace(approveEnddate) && DateTime.TryParse(approveEnddate, out approveend))//结束日期
            {
                predicate = predicate.And(item => item.ApproveDate < approveend.AddDays(1));
            }
          

            return predicate;
        }
    }
}