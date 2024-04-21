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

namespace Yahv.CrmPlus.WebApp.Srm.Approvals.Suppliers
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Dictionary<string, string> typelist = new Dictionary<string, string>() { { "-100", "全部" } };
                this.Model.SupplierMode = typelist.Concat(ExtendsEnum.ToDictionary<Underly.CrmPlus.SupplierType>()).Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value
                });
                Dictionary<string, string> gradelist = new Dictionary<string, string>() { { "-100", "全部" } };
                this.Model.Grade = gradelist.Concat(ExtendsEnum.ToDictionary<SupplierGrade>()).Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value
                });

            }
        }
        protected object data()
        {
            var query = Erp.Current.CrmPlus.Suppliers[true].Where(Predicate());
            return this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                //Corperation = item.EnterpriseRegister.Corperation,
                //item.EnterpriseRegister.Uscc,
                //item.EnterpriseRegister.RegAddress,
                TypeDes = item.Type.GetDescription(),
                item.Grade,
                SettlementDesc = item.SettlementType.GetDescription(),
                OrderTypeDesc = item.OrderType.GetDescription(),
                item.Status,
                StatusDes = item.Status.GetDescription(),
                Summary = item.EnterpriseRegister.Summary,
                Area = item.DistrictDes,
                Creator = item.CreatorAdmin?.RealName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm")
            }));

        }
        Expression<Func<Supplier, bool>> Predicate()
        {
            Expression<Func<Supplier, bool>> predicate = item => item.SupplierStatus == AuditStatus.Waiting;
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
            if (Enum.TryParse(Request.QueryString["Grade"], out grade))
            {
                predicate = predicate.And(item => item.SupplierGrade == (int)grade);
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
    }
}