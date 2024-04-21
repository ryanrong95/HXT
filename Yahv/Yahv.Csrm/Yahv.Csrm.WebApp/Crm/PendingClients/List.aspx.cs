using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.PendingClients
{
    /// <summary>
    /// 待审批客户列表
    /// </summary>
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
            var query = new ClientsRoll().Where(Predicate());
            return this.Paging(query.OrderBy(item => item.Enterprise.Name).ToArray().Select(item => new
            {
                item.ID,
                item.Enterprise.Name,
                item.Enterprise.AdminCode,
                item.Enterprise.Uscc,
                item.Enterprise.Corporation,
                item.Enterprise.RegAddress,
                item.TaxperNumber,
                item.Grade,
                item.DyjCode,
                Vip = item.Vip,
                Origin = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Place).GetOrigin()?.ChineseName : null,
                Type = item.AreaType.GetDescription(),
                Nature = item.Nature.GetDescription(),
                //item.Enterprise.District,
                item.ClientStatus,
                StatusName = item.ClientStatus.GetDescription(),
                item.Major
            }));
        }
        Expression<Func<Client, bool>> Predicate()
        {
            Expression<Func<Client, bool>> predicate = item => item.ClientStatus == ApprovalStatus.Waitting; ;
            var name = Request["s_name"];
            var areatype = Request["selAreaType"];
            var nature = Request["selNature"];
            var vip = Request["SelVip"];
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
                predicate = predicate.And(item => item.Nature == (ClientType)int.Parse(nature));
            }
            if (vip != "-100")
            {
                predicate = predicate.And(item => item.Vip == (VIPLevel)int.Parse(vip));
            }
            bool major = bool.Parse(Request["Major"]);
            if (major)
            {
                predicate = predicate.And(item => item.Major == major);
            }
            return predicate;
        }
        void init()
        {
            Dictionary<string, string> list = new Dictionary<string, string>() { { "-100", "全部" } };
            //状态
            this.Model.ClientStatus = list.Concat(ExtendsEnum.ToDictionary<ApprovalStatus>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
            //地区类型
            this.Model.AreaType = list.Concat(ExtendsEnum.ToDictionary<AreaType>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
            //客户性质
            this.Model.Nature = list.Concat(ExtendsEnum.ToDictionary<ClientType>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
            //Vip等级
            this.Model.Vip = list.Concat(ExtendsEnum.ToDictionary<VIPLevel>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
        }
        protected void del()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new ClientsRoll().Where(t => arry.Contains(t.ID));
            entity.Delete();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "ClientDelete", "删除客户：" + it.Enterprise.Name, "");
            }
        }
        protected void Approve()
        {
            var result = Request["status"];
            var ids = Request["ids"].Split(',');
            var clients = new ClientsRoll().Where(item => ids.Contains(item.ID));
            var Status = (ApprovalStatus)int.Parse(result);
            clients.Approve(Status);
            //操作日志
            foreach (var item in clients)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                       nameof(Yahv.Systematic.Crm),
                                      "ClientApprove", "客户" + item.ID + "审批" + item.ClientStatus.GetDescription(), "");
            }

        }
    }
}