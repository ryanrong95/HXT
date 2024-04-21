
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

namespace Yahv.CrmPlus.WebApp.Crm.Client.PublicSeas
{
    /// <summary>
    /// 销售的公海客户列表专用
    /// </summary>
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


      

        protected object data()
        {


            var query = new PublicClientsRoll().Where(Predicate());
            var result = this.Paging(query.ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                ClientType = item.ClientType.GetDescription(),
                //item.EnterpriseRegister.Nature,
                IsInternational = item.EnterpriseRegister.IsInternational,
                item.IsMajor,
                item.IsSpecial,
                District = item.DistrictDes,
                item.Status,
                item.Grade,
                Vip = item.Vip,
                StatusDes = item.Status.GetDescription(),
                //Creator = item.Admin?.RealName,
                //Claimant = item.Relation.Admin.RealName,
                item.EnterpriseRegister.Industry,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            }));

            return result;
        }
        Expression<Func<Yahv.CrmPlus.Service.Models.Origins.PublicClient, bool>> Predicate()
        {
            Expression<Func<Yahv.CrmPlus.Service.Models.Origins.PublicClient, bool>> predicate = item => true;
            var name = Request["s_name"];
            var clientType = Request["clientType"];
            var startDate = Request["startDate"];
            var endDate = Request["endDate"];
            
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                predicate = predicate.And(item => item.CreateDate >= from);

            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                predicate.And(item => item.CreateDate < to.AddDays(1));
            }
            Yahv.Underly.CrmPlus.ClientType clientTypeData;
            if (Enum.TryParse(clientType, out clientTypeData))
            {
                predicate = predicate.And(item => item.ClientType == clientTypeData);
            }
            return predicate;

        }


    }
}