using Needs.Erp.Generic;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Trace
{
    /// <summary>
    /// 跟踪记录展示页面
    /// </summary>
    public partial class NList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.TypeData = EnumUtils.ToDictionary<ActionMethord>().Select(item => new { text = item.Value, value = item.Key }).Json();
                //var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients.GetTop(10000, item => item.Client.Status == ActionStatus.Complete
                //    || item.Client.Status == ActionStatus.Auditing).Select(item => new { item.Client.ID, item.Client.Name });
                var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClientsBase.Where(item => item.Status == ActionStatus.Complete
                    || item.Status == ActionStatus.Auditing).Select(item => new { item.ID, item.Name });
                this.Model.ClientData = client.Json();
                this.Model.FollowerData = new NtErp.Crm.Services.Views.AdminTopView().Select(item => new { item.ID, item.RealName }).Json();
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string ClientName = Request.QueryString["ClientName"];  //客户名称
            string AdminName = Request.QueryString["AdminName"];   //跟踪记录所有人
            string Type = Request.QueryString["Type"];       //跟进方式
            var Reports = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClientReports.AsQueryable();
            var myclient = Reports.Select(item => item.Client.ID).ToArray();
            var readReports = Needs.Erp.ErpPlot.Current.ClientSolutions.MyReadReports.Where(item => item.Client != null);

            //查询条件
            if (!string.IsNullOrWhiteSpace(ClientName))
            {
                Reports = Reports.Where(c => c.Client.ID == ClientName);
                readReports = readReports.Where(c => c.Client.ID == ClientName);
            }
            if (!string.IsNullOrWhiteSpace(AdminName))
            {
                Reports = Reports.Where(c => c.Admin.ID == AdminName);
                readReports = readReports.Where(c => c.Admin.ID == AdminName);
            }
            if (!string.IsNullOrWhiteSpace(Type))
            {
                string type = @"""Type"":" + int.Parse(Type);
                Reports = Reports.Where(c => c.Context.Contains(type));
                readReports = readReports.Where(c => c.Context.Contains(type));
            }

            var data = Reports.ToList().Union(readReports.ToList()).OrderByDescending(item => item.UpdateDate);

            //按照客户分组,取出第一笔数据
            var grouptemp = from item in data
                            group new { item.ID, item.Client, item.UpdateDate, item.Context, item.Admin } by item.Client.ID into g
                            orderby g.FirstOrDefault().UpdateDate descending
                            select new
                            {
                                g.FirstOrDefault().ID,
                                g.FirstOrDefault().Client,
                                g.FirstOrDefault().Admin,
                                g.FirstOrDefault().UpdateDate,
                                g.FirstOrDefault().Context,
                            };

            //重新组合Context里面的json
            var linq = from item in grouptemp
                       select new
                       {
                           item.ID,
                           JsonSerializerExtend.JsonTo<Report>(item.Context).Date,
                           ClientName = item.Client.Name,
                           ClientID = item.Client.ID,
                           AdminName = item.Admin.RealName,
                           AdminID = item.Admin.ID,
                           TypeName = JsonSerializerExtend.JsonTo<Report>(item.Context).Type == null ? string.Empty :
                               JsonSerializerExtend.JsonTo<Report>(item.Context).Type.GetDescription(),
                           JsonSerializerExtend.JsonTo<Report>(item.Context).Type,
                           item.UpdateDate,
                           IsRead = !myclient.Contains(item.Client.ID),
                       };

            this.Paging(linq);
        }
    }
}