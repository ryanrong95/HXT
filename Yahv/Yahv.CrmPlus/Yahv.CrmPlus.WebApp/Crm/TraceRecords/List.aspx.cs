using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls.TraceRecords;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.TraceRecords
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
            this.Model.Follower = new YaHv.CrmPlus.Services.Views.Rolls.AdminsAllRoll().
             Where(item => item.RoleID == FixedRole.Sale.GetFixedID() ||
             item.RoleID == FixedRole.SaleA.GetFixedID() ||
             item.RoleID == FixedRole.SaleManager.GetFixedID() ||
             item.RoleID == FixedRole.PM.GetFixedID() ||
             item.RoleID == FixedRole.PMa.GetFixedID() ||
             item.RoleID == FixedRole.FAE.GetFixedID()
             ).Select(item => new
             {
                 value = item.ID,
                 text = $"{item.RealName}-{item.RoleName}"
             });

        }

        protected object data()
        {

            string id = Request.QueryString["id"];
            var query = new TraceRecordsRoll().Where(Predicate());
            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.ClientID,
                ClientName = item.Enterprise.Name,
                FollowWay = item.FollowWay.GetDescription(),
                TraceDate = item.TraceDate.ToString("yyyy-MM-dd"),
                NextDate = item.NextDate?.ToString("yyyy-MM-dd"),
                OwnerID = item.Owner.ID,
                Followor = item.Owner.RealName,//跟进人
                item.SupplierStaffs,
                item.CompanyStaffs,
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;

        }


        Expression<Func<TraceRecord, bool>> Predicate()
        {
            Expression<Func<TraceRecord, bool>> predicate = item => true;
            var name = Request["s_name"];
            var followWay = Request["followWay"];
            var follower = Request["follower"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(follower))
            {
                predicate = predicate.And(item => item.OwnerID == follower);
            }

            FollowWay followWaydata;
            if (Enum.TryParse(followWay, out followWaydata) && followWaydata != 0)
            {
                predicate = predicate.And(item => item.FollowWay == followWaydata);
            }
            return predicate;
        }


    }
}