using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views;
using Yahv.CrmPlus.Service.Views.Rolls.ProjectReports;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.ProjectReports
{
    public partial class AddPrice : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ID = Request.QueryString["ID"];
            this.Model.Currencys = ExtendsEnum.ToArray<Currency>(Currency.Unknown).Select(item => new
            {
                value = item,
                text = item.GetDescription()
            });
            this.Model.QuoteTypes = ExtendsEnum.ToDictionary<QuoteType>().Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
        }


        protected object data()
        {


            var id = Request.QueryString["ID"];
            var report = new ProjectReportView().ToMyArray().SingleOrDefault(x => x.ID == id);
            //  var report = new  ProjectReportRoll().FirstOrDefault(x=>x.ID==id);
            var query = Erp.Current.CrmPlus.MyAgentQuots.Where(x => x.SpnID == report.SpnID && x.Status == DataStatus.Normal);
            var result = this.Paging(query.ToArray().Select(entity => new
            {
                entity.ID,
                SpnID = entity.SpnID,
                QuoteType = (QuoteType)entity.QuoteType,
                ClientID = entity.ClientID,
                MinQuantity = entity.MinQuantity,
                MaxQuantity = entity.MaxQuantity,
                Currency = (Currency)entity.Currency,
                UnitCostPrice = entity.UnitCostPrice,
                ResalePrice = entity.ResalePrice,
                ApprovedPrice = entity.ApprovedPrice,
                ProfitRate = entity.ProfitRate,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = (DataStatus)entity.Status,
                CreatorID = entity.CreatorID
            }));

            return result;

        }


        protected void Save()
        {
            string strProducts = Request.Form["products"].Replace("&quot;", "'");
            var projectReportID = Request.Form["ReportID"];
            var products = strProducts.JsonTo<List<dynamic>>();
            var entity = Erp.Current.CrmPlus.MyProjectReports[projectReportID];
            entity.lstAgentQuotes = new List<AgentQuote>();
            foreach (var item in products)
            {
                int? maxQuantity = item.MaxQuantity;

                var list = new AgentQuote();
                list.SpnID = entity.SpnID;
                list.ClientID = entity.ClientID;
                list.MinQuantity =item.MinQuantity;
                list.MaxQuantity = maxQuantity;
                list.QuoteType = item.QuoteType;
                list.Currency = item.Currency;
                list.UnitCostPrice = item.UnitCostPrice;
                list.ResalePrice = item.ResalePrice;
                list.ApprovedPrice = item.ApprovedPrice ?? 0M;
                list.CreatorID = Erp.Current.ID;
                string startDate = item.StartDate.ToString();
                if (!string.IsNullOrEmpty(startDate))
                    list.StartDate = Convert.ToDateTime(item.StartDate);
                string  enddate = item.EndDate.ToString();
                if (!string.IsNullOrEmpty(enddate))
                    list.EndDate = Convert.ToDateTime(item.EndDate);
                list.Status = Underly.DataStatus.Normal;
                entity.lstAgentQuotes.Add(list);
            }
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.AddPrice();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var projectPeport = sender as ProjectReport;
            Service.LogsOperating.LogOperating(Erp.Current, projectPeport.ID, $"维护了报价,报备ID:{projectPeport.ID}");
            Response.Write((new { success = true, message = "保存成功！" }).Json());
        }


        #region 删除
        bool disableSuccess = false;
        protected bool disable()
        {
            var id = Request.Form["id"];
            var entity = Erp.Current.CrmPlus.MyAgentQuots[id];
            if (entity == null)
            {
                return false;
            }
            entity.AbandonSuccess += Entity_AbandonSuccess; ;
            entity.Abandon();
            return disableSuccess;
        }


        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var id = Request.Form["id"];
            LogsOperating.LogOperating(Erp.Current, id, $"删除该竞品:{ id}");
            this.disableSuccess = true;
        }

        #endregion

    }
}