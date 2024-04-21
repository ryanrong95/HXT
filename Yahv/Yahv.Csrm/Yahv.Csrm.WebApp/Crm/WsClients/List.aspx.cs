using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Web.Erp;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.WsClients
{
    public partial class List : ErpParticlePage
    {
        string companyid = "DBAEAB43B47EB4299DD1D62F764E6B6A";//"深圳市芯达通供应链管理有限公司"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                init();
            }
        }
        void init()
        {
            Dictionary<string, string> statuslist = new Dictionary<string, string>() { { "-100", "全部" } };
            statuslist.Add(ApprovalStatus.Normal.ToString(), ApprovalStatus.Normal.GetDescription());
            statuslist.Add(ApprovalStatus.UnComplete.ToString(), ApprovalStatus.UnComplete.GetDescription());
            //statuslist.Add(ApprovalStatus.Deleted.ToString(), ApprovalStatus.Deleted.GetDescription());
            this.Model.CompanyID = companyid;
            //状态
            this.Model.Status = statuslist.Select(item => new
            {
                value = item.Key,
                text = item.Value
            });
        }
        protected object data()
        {
            var company = new CompaniesRoll()[companyid].Enterprise;
            var query = Erp.Current.Whs.MyWsClients[company].Where(Predicate());

            return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
            {
                item.ID,
                item.Enterprise.Name,
                item.Enterprise.AdminCode,
                item.Grade,
                Nature = item.Nature.GetDescription(),
                item.Vip,
                item.Enterprise.District,
                item.WsClientStatus,
                StatusName = item.WsClientStatus.GetDescription(),
                Admin = item.Admin == null ? null : item.Admin.RealName,
                item.EnterCode,
                item.CustomsCode,
                item.Enterprise.Uscc,
                item.Enterprise.Corporation,
                item.Enterprise.RegAddress,
                Origin = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Place).GetOrigin()?.ChineseName : null,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ServiceManager = item.ServiceManager == null ? "" : item.ServiceManager.RealName,
                Merchandiser = item.Merchandiser == null ? "" : item.Merchandiser.RealName,
                Refferer = item.Referrer == null ? "" : item.Referrer.RealName,
                item.ServiceType,
                ServiceName=item.ServiceType.GetDescription(),
                StorageType=item.StorageType.GetDescription()
            });
        }
        Expression<Func<WsClient, bool>> Predicate()
        {
            Expression<Func<WsClient, bool>> predicate = item => true;
            var status = Request["selStatus"];
            var name = Request["s_name"];
            string sale = Request["sale"];
            string merchandiser = Request["Merchandiser"];
            ApprovalStatus approvalstatus;
            if (Enum.TryParse(status, out approvalstatus) && status != "-100")
            {
                predicate = predicate.And(item => item.WsClientStatus == approvalstatus);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }

            return predicate;
        }


        protected void del()
        {
            var arry = Request.Form["items"].Split(',');
            Enterprise company = new CompaniesRoll()[Request.Form["CompanyID"]].Enterprise;
            var entity = Erp.Current.Whs.MyWsClients[company].Where(t => arry.Contains(t.ID));
            entity.Delete();
        }

        //protected void Enable()
        //{
        //    var arry = Request.Form["items"].Split(',');
        //    var entity = Erp.Current.Whs.MyWsClients.Where(t => arry.Contains(t.ID));
        //    entity.Enable();
        //}
        //protected void Unable()
        //{
        //    var arry = Request.Form["items"].Split(',');
        //    var entity = Erp.Current.Whs.MyWsClients.Where(t => arry.Contains(t.ID));
        //    entity.Unable();

        //}
        //protected void Black()
        //{
        //    var arry = Request.Form["items"].Split(',');
        //    var entity = Erp.Current.Whs.MyWsClients.Where(t => arry.Contains(t.ID));
        //    entity.Blacked();
        //}
    }
}