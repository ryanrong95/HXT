using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls.Samples;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Samples
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Dictionary<string, string> list = new Dictionary<string, string>() { { "0", "全部" } };
                //this.Model.AuditStatus = list.Concat(ExtendsEnum.ToDictionary<AuditStatus>()).Select(item => new
                //{
                //    value = int.Parse(item.Key),
                //    text = item.Value.ToString()
                //});
                this.Model.projects = Erp.Current.CrmPlus.MyProjects.Select(x => new { value = x.ID, text = x.Name });
            }

        }




        protected object data()
        {

            var query = new SampleRoll().Where(x=>x.AuditStatus==AuditStatus.Waiting).Where(Predicate());;

            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                ClientName = item.Project.Client.Name,
                ProjectName = item.Project.Name,
                DeliveryDate = item.DeliveryDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                item.WaybillCode,
                Contact = item.Contact.Name,
                Tel = item.Contact.Mobile,
                Address = item.Address.Context,
                AuditStatusDes = item.AuditStatus.GetDescription(),
                item.AuditStatus,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;
        }


        Expression<Func<Sample, bool>> Predicate()
        {
            Expression<Func<Sample, bool>> predicate = item => true;
            var clientName = Request["ClientName"];
            var projectName = Request["ProjectName"];
            var standardPartNumber = Request["StandardPartNumber"];
            var status = Request["Status"];
            string startdate = Request.QueryString["startdate"];
            string enddate = Request.QueryString["enddate"];
            if (!string.IsNullOrWhiteSpace(clientName))
            {
                predicate = predicate.And(item => item.Project.Client.Name.Contains(clientName));
            }
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                predicate = predicate.And(item => item.Project.ID==projectName);
            }


            DateTime start;
            if (!string.IsNullOrWhiteSpace(startdate) && DateTime.TryParse(startdate, out start))//开始日期
            {
                predicate = predicate.And(item => item.DeliveryDate >= start);
            }


            DateTime end;
            if (!string.IsNullOrWhiteSpace(enddate) && DateTime.TryParse(enddate, out end))//结束日期
            {
                predicate = predicate.And(item => item.DeliveryDate < end.AddDays(1));
            }

            AuditStatus dataStatus;
            if (Enum.TryParse(status, out dataStatus) && dataStatus != 0)
            {
                predicate = predicate.And(item => item.AuditStatus == dataStatus);
            }
            return predicate;
        }


        protected void Closed()
        {
            var id = Request.Form["ID"];
            try
            {

                var entity = Erp.Current.CrmPlus.MySamples[id];
                entity.Closed();
                LogsOperating.LogOperating(Erp.Current, entity.ID, $"删除送样:{ entity.ID}");

            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"删除送样 操作失败" + ex);
            }
        }
    }
}