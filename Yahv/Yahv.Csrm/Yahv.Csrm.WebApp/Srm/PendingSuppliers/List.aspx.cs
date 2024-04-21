using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;
using YaHv.Csrm.Services.Extends;

namespace Yahv.Csrm.WebApp.Srm.PendingSuppliers
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                init();
            }
        }
        void init()
        {
            Dictionary<string, string> list = new Dictionary<string, string>() { { "-100", "全部" } };
            //级别
            this.Model.Grade = list.Concat(ExtendsEnum.ToDictionary<SupplierGrade>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
            //性质
            this.Model.Nature = list.Concat(ExtendsEnum.ToDictionary<SupplierNature>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
            //类型
            this.Model.Type = list.Concat(ExtendsEnum.ToDictionary<SupplierType>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
        }
        protected object data()
        {
            var query = new SuppliersRoll().Where(Predicate());
            return this.Paging(query.OrderBy(item => item.Enterprise.Name).ToArray().Select(item => new
            {
                item.ID,
                item.Enterprise.Name,
                item.Enterprise.AdminCode,
                item.TaxperNumber,
                item.Grade,
                item.DyjCode,
                Type = item.Type.GetDescription(),
                Nature = item.Nature.GetDescription(),
                item.Enterprise.District,
                AreaType = item.AreaType.GetDescription(),
                IsFactory = item.IsFactory ? "是" : "否",
                item.AgentCompany,
                InvoiceType = item.InvoiceType.GetDescription(),
                Admin = item.Creator == null ? null : item.Creator.RealName,
                Origin = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Place).GetOrigin()?.ChineseName : null,
                //IsTax = item.IsTax ? "是" : "否"
            }));

        }
        Expression<Func<Supplier, bool>> Predicate()
        {
            Expression<Func<Supplier, bool>> predicate = item => item.SupplierStatus == ApprovalStatus.Waitting; ;
            var name = Request["s_name"];
            var areatype = Request["selAreaType"];
            var nature = Request["selNature"];
            var grade = Request["selGrade"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }
            if (areatype != "-100")
            {
                predicate = predicate.And(item => item.AreaType == (AreaType)int.Parse(areatype));
            }
            if (nature != "-100")
            {
                predicate = predicate.And(item => item.Type == (SupplierType)int.Parse(nature));
            }
            if (grade != "-100")
            {
                predicate = predicate.And(item => item.Grade == (SupplierGrade)int.Parse(grade));
            }
            if (bool.Parse(Request["factory"]))
            {
                predicate = predicate.And(item => item.IsFactory == true);
            }
            return predicate;
        }
        protected void Approve()
        {
            var result = Request["status"];
            var ids = Request["ids"].Split(',');
            var entity = new SuppliersRoll().Where(i => ids.Contains(i.ID)).ToArray();
            var Status = (ApprovalStatus)int.Parse(result);
            entity.Approve(Status);
            //操作日志
            foreach (var item in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                       nameof(Yahv.Systematic.Crm),
                                      "SupplierApprove", "供应商" + item.ID + "审批:" + Status.GetDescription(), "");
            }

        }
    }
}