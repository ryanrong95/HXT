using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.WareHouses
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>() { { "-100", "全部" } };
                dic.Add(ApprovalStatus.Normal.ToString(), ApprovalStatus.Normal.GetDescription());
                dic.Add(ApprovalStatus.Deleted.ToString(), ApprovalStatus.Deleted.GetDescription());
                this.Model.Status = dic.Select(item => new
                {
                    value = item.Key,
                    text = item.Value
                });
            }
        }
        protected object data()
        {
            Expression<Func<WareHouse, bool>> predicate = item => true;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }
            ApprovalStatus status;
            if (Enum.TryParse(Request["status"], out status) && Request["status"] != "-100")
            {
                predicate = predicate.And(item => item.Status == status);
            }
            var query = Erp.Current.Crm.WareHouses.Where(predicate);
            return this.Paging(query.OrderBy(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.WsCode,
                item.Enterprise.Name,
                item.DyjCode,
                District = item.District.GetDescription(),
                item.Grade,
                item.Address,
                item.Enterprise.Corporation,
                item.Enterprise.Uscc,
                item.Enterprise.RegAddress,
                item.Status,
                StatusName = item.Status.GetDescription(),
                Admin = item.Creator == null ? null : item.Creator.RealName
            }));
        }

        protected void Del()
        {
            var arry = Request.Form["ids"].Split(',');
            var entity = new WareHousesRoll().Where(t => arry.Contains(t.ID));
            entity.Delete();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "WarehouseDelete", "删除库房：" + it.ID, "");
            }
        }
    }
}