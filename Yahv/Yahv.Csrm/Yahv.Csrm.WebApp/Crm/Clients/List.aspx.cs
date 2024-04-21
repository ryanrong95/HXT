using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Clients
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
            //状态
            Dictionary<string, string> statuslist = new Dictionary<string, string>() { { "-100", "全部" } };
            statuslist.Add(ApprovalStatus.Normal.ToString(), ApprovalStatus.Normal.GetDescription());
            statuslist.Add(ApprovalStatus.Black.ToString(), ApprovalStatus.Black.GetDescription());
            statuslist.Add(ApprovalStatus.Waitting.ToString(), ApprovalStatus.Waitting.GetDescription());
            statuslist.Add(ApprovalStatus.Voted.ToString(), ApprovalStatus.Voted.GetDescription());
            statuslist.Add(ApprovalStatus.Deleted.ToString(), ApprovalStatus.Deleted.GetDescription());
            statuslist.Add(ApprovalStatus.Closed.ToString(), ApprovalStatus.Closed.GetDescription());
            this.Model.ClientStatus = statuslist.Select(item => new
            {
                value = item.Key,
                text = item.Value
            }); ;

            Dictionary<string, string> list = new Dictionary<string, string>() { { "-100", "全部" } };
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
        protected object data()
        {
            var query = Erp.Current.Crm.Clients.Where(Predicate());
            var sale = Request["Sale"];
            if (string.IsNullOrEmpty(sale))
            {
                query = query.GroupBy(g => g.ID).Select(item => item.First());
            }
            return this.Paging(query.OrderBy(item => item.Enterprise.Name), item => new
            {
                item.ID,
                item.Major,
                item.Enterprise.Name,
                item.Enterprise.AdminCode,
                TaxperNumber = item.Enterprise.Uscc,
                item.Grade,
                item.DyjCode,
                item.Vip,
                item.Enterprise.Uscc,
                item.Enterprise.Corporation,
                item.Enterprise.RegAddress,
                Type = item.AreaType.GetDescription(),
                Nature = item.Nature.GetDescription(),
                item.Enterprise.District,
                Origin = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Place).GetOrigin()?.ChineseName : null,

                item.ClientStatus,
                StatusName = item.ClientStatus.GetDescription(),
                Creator = item.Creator == null ? null : item.Creator.RealName,
                Cooper = item.Sales.SingleOrDefault(i => i.ID == Erp.Current.ID)?.Company?.Name
            });
        }
        Expression<Func<TradingClient, bool>> Predicate()
        {
            Expression<Func<TradingClient, bool>> predicate = item => true;
            var status = Request["selStatus"];
            var name = Request["s_name"];
            var areatype = Request["selAreaType"];
            var nature = Request["selNature"];
            var vip = Request["SelVip"];
            var sale = Request["Sale"];

            ApprovalStatus approvalstatus;
            if (Enum.TryParse(status, out approvalstatus) && status != "-100")
            {
                predicate = predicate.And(item => item.ClientStatus == approvalstatus);
            }
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
            if (!string.IsNullOrWhiteSpace(sale))
            {
                predicate = predicate.And(item => item.Sales.Count(it => it.ID == sale || it.UserName.Contains(sale) || it.RealName.Contains(sale)) > 0);
            }
            bool major = bool.Parse(Request["Major"]);
            if (major)
            {
                predicate = predicate.And(item => item.Major == major);
            }
            return predicate;
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
                                          "ClientDelete", "修改客户状态：" + it.ID, "");
            }
            //else
            //{
            //    var arry = Request.Form["items"].Split(',');
            //    var entity = Erp.Current.Crm.MyClients.Where(t => arry.Contains(t.ID));

            //    foreach (var it in entity)
            //    {
            //        it.saleid = Erp.Current.ID;
            //        it.Abandon();
            //        Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
            //                                   nameof(Yahv.Systematic.Crm),
            //                                  "MapsBEnter", "删除客户：" + it.ID, "");
            //    }
            //}

        }
        protected void Enable()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new ClientsRoll().Where(t => arry.Contains(t.ID));
            entity.Enable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "ClientEnable", "启用客户：" + it.ID, "");
            }
        }
        protected void Unable()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new ClientsRoll().Where(t => arry.Contains(t.ID));
            entity.Unable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "ClientUnable", "停用客户：" + it.ID, "");
            }
        }
        protected void Black()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new ClientsRoll().Where(t => arry.Contains(t.ID));
            entity.Blacked();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "ClientBlacked", "客户加入黑名单：" + it.ID, "");
            }
        }
    }
}