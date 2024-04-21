using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.TraceComments
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadData();
            }
        }

        public void loadData()
        {

            this.Model.Ownerid = Erp.Current.ID;
            //Dictionary<string, string> list = new Dictionary<string, string>() { { "0", "全部" } };
            //this.Model.FollowWay = list.Concat(ExtendsEnum.ToDictionary<FollowWay>()).Select(item => new
            //{

            //    value = int.Parse(item.Key),
            //    text = item.Value.ToString()
            //});
            this.Model.Follower =new YaHv.CrmPlus.Services.Views.Rolls.AdminsAllRoll().Where(item => item.RoleID == FixedRole.Sale.GetFixedID() || item.RoleID == FixedRole.SaleA.GetFixedID() || item.RoleID == FixedRole.SaleManager.GetFixedID()).Select(item => new
            {
                value = item.ID,
                text = $"{item.RealName}-{item.RoleName}"
            });

        }

        protected object data()
        {

            var query = Erp.Current.CrmPlus.MyTraceComments.Where(x=>x.IsPointed==true &&x.Comments==null).Where(Predicate());
            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.TraceRecord.ClientID,
                item.TraceRecordID,
                ClientName = item.TraceRecord.Enterprise.Name,
                FollowWay = item.TraceRecord.FollowWay.GetDescription(),
                TraceDate = item.TraceRecord.TraceDate.ToString("yyyy-MM-dd"),
                NextDate = item.TraceRecord.NextDate?.ToString("yyyy-MM-dd"),
                Owner = item.TraceRecord.Owner.RealName,
                Reader = item.Admin.RealName,
                item.TraceRecord.SupplierStaffs,
                item.TraceRecord.CompanyStaffs,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;

        }


        Expression<Func<TraceComment, bool>> Predicate()
        {
            Expression<Func<TraceComment, bool>> predicate = item => true;
            var name = Request["s_name"];
            var followWay = Request["followWay"];
            var follower = Request["follower"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.TraceRecord.Enterprise.Name.Contains(name));
            }

            //if (!string.IsNullOrWhiteSpace(follower))
            //{
            //    predicate = predicate.And(item => item.TraceRecord.OwnerID == follower);
            //}

            FollowWay followWaydata;
            if (Enum.TryParse(followWay, out followWaydata) && followWaydata != 0)
            {
                predicate = predicate.And(item => item.TraceRecord.FollowWay == followWaydata);
            }
            return predicate;
        }
    }
}