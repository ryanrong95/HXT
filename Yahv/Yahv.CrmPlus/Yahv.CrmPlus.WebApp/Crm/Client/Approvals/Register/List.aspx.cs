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

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Register
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           // loadData();
        }

        //protected void loadData()
        //{

        //    //Dictionary<string, string> typelst = new Dictionary<string, string>() { { "0", "全部" } };
        //    //this.Model.ClientType = typelst.Concat(ExtendsEnum.ToDictionary<Yahv.Underly.CrmPlus.ClientType>()).Select(item => new
        //    //{
        //    //    value = int.Parse(item.Key),
        //    //    text = item.Value
        //    //});

        //    //Dictionary<string, string> list = new Dictionary<string, string>() { { "0", "全部" } };
        //    ////状态
        //    //this.Model.Status = list.Concat(ExtendsEnum.ToDictionary<AuditStatus>()).Select(item => new
        //    //{
        //    //    value = int.Parse(item.Key),
        //    //    text = item.Value.ToString()
        //    //});

        //    Dictionary<string, string> area = new Dictionary<string, string>() { { "0", "全部" } };
        //    var FixedArea = new EnumsDictionariesRoll().Where(x => x.Enum == "FixedArea").ToDictionary(c => c.ID, c => c.Description);
        //    this.Model.Areas = area.Concat(FixedArea).Select(item => new
        //    {

        //        key = item.Key,
        //        value = item.Value
        //    });

        //}


        protected object data()
        {

            var query = new WaitingClientsRoll().Where(Predicate());
            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                item.EnterpriseRegister.Nature,
                ClientType = item.ClientType.GetDescription(),
                item.Status,
                StatusDes = item.Status.GetDescription(),
                CorCompany = item.Relation?.Company?.Name,
                Creator = item.Relation?.Admin?.RealName,
                item.EnterpriseRegister.Industry,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ModifyDate = item.ModifyDate.ToString("yyyy-MM-dd HH:mm:ss"),
                IsInternational = item.EnterpriseRegister.IsInternational == true ? "是" : "否",
                IsMajor = item.IsMajor ? "是" : "否",
                IsSpecial = item.IsSpecial ? "是" : "否",
                District = item.DistrictDes,
                ClientGrade = item.ClientGrade.GetDescription(),
                Vip = item.Vip.GetDescription(),

            }));

            return result;
        }
        Expression<Func<Yahv.CrmPlus.Service.Models.Origins.WatingClient, bool>> Predicate()
        {
            Expression<Func<Yahv.CrmPlus.Service.Models.Origins.WatingClient, bool>> predicate = item=>true;
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
            if (area != "a")
            {
                predicate = predicate.And(item => item.District == area);
            }
            //Yahv.Underly.FixedArea FixedAreaData;
            //if (Enum.TryParse(area, out FixedAreaData))
            //{
            //    predicate = predicate.And(item => item.District == FixedAreaData.GetFixedID());
            //}
            return predicate;

        }


    }
}