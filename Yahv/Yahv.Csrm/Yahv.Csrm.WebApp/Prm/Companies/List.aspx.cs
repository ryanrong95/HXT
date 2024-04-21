using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Prm.Companies
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
        protected object data()
        {
            var query = new CompaniesRoll().Where(Predicate());
            return this.Paging(query.OrderBy(item => item.Enterprise.Name).ToArray().Select(item => new
            {
                item.ID,
                item.Enterprise.Name,
                item.Enterprise.AdminCode,
                item.Enterprise.Corporation,
                item.Enterprise.Uscc,
                item.Enterprise.RegAddress,
                Range = item.Range.GetDescription(),
                Type = item.Type.GetDescription(),
                item.Enterprise.District,
                item.CompanyStatus,
                Status = item.CompanyStatus.GetDescription()
            }));
        }
        //搜索条件
        Expression<Func<Company, bool>> Predicate()
        {
            Expression<Func<Company, bool>> predicate = item => true;
            var name = Request["s_name"];
            var type = Request["selType"];
            var range = Request["selRange"];
            var status = Request["selStatus"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }
            if (type != "-100")
            {
                predicate = predicate.And(item => item.Type == (CompanyType)int.Parse(type));
            }
            if (range != "-100")
            {
                predicate = predicate.And(item => item.Range == (AreaType)int.Parse(range));
            }
            ApprovalStatus approvalstatus;
            if (Enum.TryParse(status, out approvalstatus) && approvalstatus != 0)
            {
                predicate = predicate.And(item => item.CompanyStatus == approvalstatus);
            }
            return predicate;
        }
        void init()
        {
            Dictionary<string, string> list = new Dictionary<string, string>() { { "-100", "全部" } };
            Dictionary<string, string> statuslist = new Dictionary<string, string>() { { "0", "全部" } };
            statuslist.Add(ApprovalStatus.Normal.ToString(), ApprovalStatus.Normal.GetDescription());
            statuslist.Add(ApprovalStatus.Black.ToString(), ApprovalStatus.Black.GetDescription());
            statuslist.Add(ApprovalStatus.Closed.ToString(), ApprovalStatus.Closed.GetDescription());
            statuslist.Add(ApprovalStatus.Deleted.ToString(), ApprovalStatus.Deleted.GetDescription());
            //状态
            this.Model.ClientStatus = statuslist.Select(item => new
            {
                value = item.Key,
                text = item.Value
            });
            //类型
            this.Model.Type = list.Concat(ExtendsEnum.ToDictionary<CompanyType>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
            //范围
            this.Model.Range = list.Concat(ExtendsEnum.ToDictionary<AreaType>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
        }
        protected void del()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new CompaniesRoll().Where(t => arry.Contains(t.ID));
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Plat),
                                          "删除内部公司", "删除内部公司：" + it.Enterprise.Name, "");
            }
            entity.Delete();
        }
        protected void Enable()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new CompaniesRoll().Where(t => arry.Contains(t.ID));
            entity.Enable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Plat),
                                          "启用内部公司", "启用内部公司：" + it.Enterprise.Name, "");
            }
        }
        protected void Unable()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new CompaniesRoll().Where(t => arry.Contains(t.ID));
            entity.Unable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Plat),
                                          "停用内部公司", "停用内部公司：" + it.Enterprise.Name, "");
            }
        }
    }
}