using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Layers.Data.Sqls;
using Yahv.Linq.Extends;
using Yahv.Payments;
using Yahv.Underly;
using Yahv.Web.Erp;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.CreditsGrant
{
    public partial class List : ErpParticlePage
    {
        private string companyid = "DBAEAB43B47EB4299DD1D62F764E6B6A";      //深圳市芯达通供应链管理有限公司
        private string companyid_hk = "8C7BF4F7F1DE9F69E1D96C96DAF6768E";  //"香港畅运国际物流有限公司"
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
            this.Model.CompanyID = companyid_hk;
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
            var wsclients = Erp.Current.Whs.WsClients[company].Where(Predicate()).ToArray();
            var creditsView = new CreditsUsdStatisticsView().Where(item => item.Catalog == CatalogConsts.仓储服务费 && item.Payee == companyid_hk).ToArray();

            return this.Paging(wsclients.OrderByDescending(item => item.CreateDate), item => new
            {
                item.ID,
                item.Enterprise.Name,
                item.Vip,
                item.WsClientStatus,
                StatusName = item.WsClientStatus.GetDescription(),
                item.EnterCode,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ServiceManager = item.ServiceManager == null ? "" : item.ServiceManager.RealName,
                Merchandiser = item.Merchandiser == null ? "" : item.Merchandiser.RealName,
                Total = creditsView.FirstOrDefault(c => c.Payer == item.ID)?.Total,
                Cost = creditsView.FirstOrDefault(c => c.Payer == item.ID)?.Cost,
            });
        }
        Expression<Func<WsClient, bool>> Predicate()
        {
            Expression<Func<WsClient, bool>> predicate = item => true;
            var status = Request["selStatus"];
            var name = Request["s_name"];
            ApprovalStatus approvalstatus;
            if (Enum.TryParse(status, out approvalstatus) && status != "-100")
            {
                predicate = predicate.And(item => item.WsClientStatus == approvalstatus);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name) | item.EnterCode.Contains(name));
            }

            return predicate;
        }
    }
}