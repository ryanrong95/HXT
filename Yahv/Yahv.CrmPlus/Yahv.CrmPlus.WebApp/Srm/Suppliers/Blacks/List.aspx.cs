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
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Srm.Suppliers.Blacks
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            var query = Erp.Current.CrmPlus.Suppliers.Where(Predicate());
            return this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                EnterpriseID = item.ID,
                item.Name,
                item.Summary,
                TypeDes = item.Type.GetDescription(),
                item.SupplierGrade,
                SettlementDesc = item.SettlementType.GetDescription(),
                item.Status,
                StatusDes = item.Status.GetDescription(),
                Area = item.DistrictDes,
                //Creator = item.cre?.RealName,
                item.IsSpecial,
                item.IsAccount,
                item.IsClient,
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

        }
        Expression<Func<Supplier, bool>> Predicate()
        {
            Expression<Func<Supplier, bool>> predicate = item => item.Status == AuditStatus.Black;
            string name = Request.QueryString["Name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            Underly.CrmPlus.SupplierType type;
            if (Enum.TryParse(Request.QueryString["SupplierType"], out type))
            {
                predicate = predicate.And(item => item.Type == type);
            }

            SupplierGrade grade;
            if (Enum.TryParse(Request.QueryString["Grade"],out grade))
            {
                predicate = predicate.And(item => item.Grade == (int)grade);
            }

            string startdate = Request.QueryString["StartDate"];
            DateTime start;
            if (!string.IsNullOrWhiteSpace(startdate) && DateTime.TryParse(startdate, out start))//开始日期
            {
                predicate = predicate.And(item => item.CreateDate >= start);
            }

            string enddate = Request.QueryString["EndDate"];
            DateTime end;
            if (!string.IsNullOrWhiteSpace(enddate) && DateTime.TryParse(enddate, out end))//结束日期
            {
                predicate = predicate.And(item => item.CreateDate < end.AddDays(1));
            }
            return predicate;
        }
        object cancelresult = new { success = false, message = "" };
        protected object Cancel()
        {
            try
            {
                string enterpriseid = Request.Form["ID"];
                var enterprise = new Service.Views.Rolls.EnterprisesRoll()[enterpriseid];
                enterprise.EnterSuccess += Enterprise_EnterSuccess;
                enterprise.CancelBlack();
                return cancelresult;
            }
            catch (Exception ex)
            {
                return new { success = false, message = ex.ToString() };
            }
        }
        private void Enterprise_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            cancelresult = new { success = true, message = "操作成功" };
            var entity = sender as Enterprise;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"撤销企业黑名单:{entity.Name}");
        }
    }
}