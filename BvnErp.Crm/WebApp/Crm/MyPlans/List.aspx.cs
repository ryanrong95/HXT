using Needs.Erp.Generic;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Descriptions;

namespace WebApp.Crm.MyPlans
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                string adminid = Needs.Erp.ErpPlot.Current.ID.Json();
                var admins = new NtErp.Crm.Services.Views.AdminTopView();
                var admintype = admins.Where(item => item.ID == adminid.Replace("\"", "")).SingleOrDefault();                  
                this.AdminType = (int)admintype.JobType;
            }
        }

        protected int AdminType;

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string Name = Request.QueryString["Name"];
            var MyPlan = Needs.Erp.ErpPlot.Current.ClientSolutions.MyPlans.Where(item => true);

            Func<Plan, object> linq = item => new
            {
                ID = item.ID,
                Name = item.Name,
                ClientName = item.client.Name,
                CompanyName = item.Companys.Name,
                TargetName = item.Target.GetDescription(),
                MethordName = item.Methord.GetDescription(),
                StatusName = item.Status.GetDescription(),
                PlanDate = item.PlanDate.ToString("yyyy-MM-dd HH:mm:ss"),
                StartDate = item.StartDate==null?"": item.StartDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = item.EndDate==null?"": item.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                AdminName = item.Admin.UserName,
                Summary = item.Summary,
                Status = item.Status,
            };

            if (!string.IsNullOrWhiteSpace(Name))
            {
                MyPlan = MyPlan.Where(item => item.Name == Name);
            }
            this.Paging(MyPlan, linq);

            //if (!string.IsNullOrWhiteSpace(Name))
            //{
            //    var data = MyPlan.Where(item => item.Name == Name);
            //    this.Paging(data);
            //}
            //else
            //{
            //    this.Paging(MyPlan);
            //}
            
        }

        /// <summary>
        /// 数据删除
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Erp.ErpPlot.Current.ClientSolutions.MyPlans[id];
            if(del !=null)
            {
                del.AbandonSuccess += Del_AbandonSuccess;
                del.Abandon();
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }

        /// <summary>
        /// 申请
        /// </summary>
        protected void Apply()
        {
            string id = Request.Form["ID"];
            var apply = new NtErp.Crm.Services.Models.Apply();
            apply.MainID = id;
            apply.Type = ApplyType.Action;
            apply.Admin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            //apply.AdminID = Needs.Erp.ErpPlot.Current.ID;
            apply.Summary = "计划申请";
            apply.Status = ApplyStatus.Audting;
            apply.Enter();

            //更新plan状态
            var plan = Needs.Erp.ErpPlot.Current.ClientSolutions.MyPlans[id] as NtErp.Crm.Services.Models.Plan ??
                new NtErp.Crm.Services.Models.Plan();
            plan.Status = ActionStatus.Auditing;
            plan.Enter();
        }
    }
}