using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals
{
    public partial class ApprovedList : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected object data()
        {

            var query = Erp.Current.CrmPlus.MyClients.Where(x => x.IsDraft == false && x.Status == AuditStatus.Normal).Where(Predicate());
            var result = this.Paging(query.OrderByDescending(x=>x.MapsTopN.TopOrder).ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                ClientType = item.ClientType.GetDescription(),
                item.EnterpriseRegister.Nature,
                IsInternational = item.EnterpriseRegister.IsInternational ,
                item.IsMajor,
                item.IsSpecial,
                District = item.DistrictDes,
                item.Status,
                item.Grade,
                item.Vip,
                item.IsDraft,
                StatusDes = item.Status.GetDescription(),
                Creator = item.Admin?.RealName,
                item.EnterpriseRegister.Industry,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ModifyDate = item.ModifyDate.ToString("yyyy-MM-dd HH:mm:ss")

            }));

            return result;
        }
        Expression<Func<Yahv.CrmPlus.Service.Models.Origins.Client, bool>> Predicate()
        {
            Expression<Func<Yahv.CrmPlus.Service.Models.Origins.Client, bool>> predicate = item => true;
            var name = Request["s_name"];
            var status = Request["status"];
            var clientType = Request["clientType"];
            var area = Request["area"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            AuditStatus dataStatus;
            if (Enum.TryParse(status, out dataStatus) && dataStatus != 0)
            {
                predicate = predicate.And(item => item.Status == dataStatus);
            }
            Yahv.Underly.CrmPlus.ClientType clientTypeData;
            if (Enum.TryParse(clientType, out clientTypeData))
            {
                predicate = predicate.And(item => item.ClientType == clientTypeData);
            }
            //Yahv.Underly.FixedArea FixedAreaData;
            //if (Enum.TryParse(area, out FixedAreaData))
            //{
            //    predicate = predicate.And(item => item.District == FixedAreaData.GetFixedID());
            //}
            if (area != "a")
            {
                predicate = predicate.And(item => item.District == area);
            }

            return predicate;

        }

    }
}